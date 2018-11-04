using AutoMapper;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// 車両マスタサービス
    /// </summary>
    public class TruckAdminService : DBServiceBase
    {

        public TruckAdminService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }

        /// <summary>
        /// 運送会社コードで指定して読み取る
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public IQueryable<TRUCK_ADMIN> read(string ref_TRANSPORT_ADMIN_CODE)
        {
            return db.TRUCK_ADMINs
                .Where(e => e.TRANSPORT_ADMIN_CODE == ref_TRANSPORT_ADMIN_CODE);
        }

        /// <summary>
        /// 運送会社と車両管理番号をセットして読み出す
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public IQueryable<TRUCK_ADMIN> read(string ref_TRANSPORT_ADMIN_CODE,
            int ref_TRUCK_MANAGE_NUMBER)
        {
            return db.TRUCK_ADMINs
                .Where(e => (e.TRANSPORT_ADMIN_CODE == ref_TRANSPORT_ADMIN_CODE) &&
                e.TRUCK_MANAGE_NUMBER == ref_TRUCK_MANAGE_NUMBER);
        }

        /// <summary>
        /// 更新結果
        /// </summary>
        public enum UpdateResults
        {
            /// <summary>
            /// 更新失敗
            /// </summary>
            Failure,
            /// <summary>
            /// 更新成功
            /// </summary>
            Success,
            /// <summary>
            /// 関連情報クリア済み
            /// </summary>
            RelatedInformationClear
        }

        // 追加
        public async Task<UpdateResults> update(IEnumerable<CollisionDetectableTRUCK_ADMIN> list)
        {
            #region ローカル関数

            //該当データがクリアされた状態か確認する
            bool LF_isClear(TRUCK_ADMIN target)
            {
                if ((null == target.CHARGE) && (null == target.NUMBER))
                {
                    return true;
                }
                if ((String.Empty == target.CHARGE) && (String.Empty == target.NUMBER))
                {
                    return true;
                }
                return false;
            }

            //集配依頼詳細を消去する
            async Task<bool> LF_removeDELIVERY(List<TRUCK_ADMIN> targetList)
            {
                var deliveryDetailService = new DeliveryDetailService(
                    this.db,
                    this.user,
                    this.signInManager,
                    this.userManager);

                //消去対象となる配送依頼詳細
                var removeTarget =
                    targetList.Select(e => e.TRANSPORT_ADMIN_CODE)
                    .Distinct().Select(e =>
                    deliveryDetailService.read(new DeliveryDetailService.ReadConfig
                    {
                        TRANSPORT_ADMIN_CODE = e
                    }
                ))
                .Select(e =>
                {

                    //処理エラーが出ていたら例外を投げる
                    if (false == e.succeed)
                    {
                        throw new ApplicationException(String.Join("<br>", e.errorMessages));
                    }

                    return e.result;
                });

                //ステータスコードを取得する
                const string KIND = "00100000";
                var domainService = new Service.DB.DomainService
                    (db,
                    user,
                    signInManager,
                    userManager);

                //該当項目に関して、車番、担当者をクリアする
                foreach (var ele in removeTarget)
                {
                    var tl = ele
                        .Include(e => e.DELIVERY_REQUEST)
                        .ToList();
                    foreach (var in_e in tl)
                    {
                        //入力可能な項目だけ消去する
                        //ドメイン情報を取得
                        var domainInfo = domainService.getValue(KIND, in_e.DELIVERY_REQUEST.STATUS);
                        if (domainInfo.VALUE == "入力可能")
                        {
                            in_e.TRUCK_CHARGE = "";
                            in_e.TRUCK_NUMBER = "";
                            in_e.TRUCK_MANAGE_NUMBER = null;
                            in_e.ROUTE = null;
                        }
                    }

                    await this.setUpdateManagementInformation(tl);
                }

                return true;
            }

            #endregion

            //消去が確認されたデータ一覧
            var removed = new List<TRUCK_ADMIN>();

            var upData = list.Select(e =>
            {
                //viewモデルが持っているデータはDBにあるものと異なるので
                //DBから値をとってきて必要データを追加して渡す
                var t = this.db.TRUCK_ADMINs
                .Where(in_e => e.TRUCK_MANAGE_NUMBER == in_e.TRUCK_MANAGE_NUMBER &&
                    e.TRANSPORT_ADMIN_CODE == in_e.TRANSPORT_ADMIN_CODE)
                .First();

                //車番号と担当者が消去された場合、
                //該当する車両管理番号を使っている、
                //全ての集配依頼詳細を消去する
                if ((false == LF_isClear(t)) && (true == LF_isClear(e)))
                {
                    removed.Add(t);
                }

                t.CHARGE = e.CHARGE;
                t.NUMBER = e.NUMBER;

                //衝突検知
                if (e.collisionDetection_Timestamp != t.timestampToString())
                {
                    //衝突検知
                    throw new ApplicationException
                    ("車両マスタ(TRUCK_ADMIN)テーブルに関して、他ユーザーによる編集を検知しました。");
                }

                return t;
            });

            await this.setUpdateManagementInformation(upData);

            //消去されているデータを使用している、
            //集配依頼項目を削除する
            if (0 != removed.Count())
            {
                await LF_removeDELIVERY(removed);
                return UpdateResults.RelatedInformationClear;
            }

            return UpdateResults.Success;
        }


        /// <summary>
        /// 衝突検知可能な車両マスタ
        /// </summary>
        public class CollisionDetectableTRUCK_ADMIN : TRUCK_ADMIN
        {

            /// <summary>
            /// 後から追加で設定された値
            /// </summary>
            private string _collisionDetection_Timestamp;


            /// <summary>
            /// 衝突検知用のタイムスタンプ
            /// </summary>
            public string collisionDetection_Timestamp
            {
                get
                {
                    if (null != _collisionDetection_Timestamp)
                    {
                        return _collisionDetection_Timestamp;
                    }
                    else
                    {
                        return this.timestampToString();
                    }
                }
                set
                {
                    this._collisionDetection_Timestamp = value;

                }
            }

            /// <summary>
            /// データ形式を変換する
            /// </summary>
            /// <param name="src"></param>
            /// <returns></returns>
            public static CollisionDetectableTRUCK_ADMIN convert(TRUCK_ADMIN src)
            {
                //データ形式を変換する
                var m = Mapper.Map<CollisionDetectableTRUCK_ADMIN>(src);
                return m;
            }

        }

    }
}
