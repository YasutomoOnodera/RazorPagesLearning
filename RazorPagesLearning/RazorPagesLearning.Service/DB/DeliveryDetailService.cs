using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Claims;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// 集配依頼詳細サービス
    /// </summary>
    public class DeliveryDetailService : DBServiceBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// コンストラクタ
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public DeliveryDetailService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 読み取り設定
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public class ReadConfig
        {
            /// <summary>
            /// 作業依頼番号
            /// </summary>
            public string DELIVERY_REQUEST_NUMBER;

            /// <summary>
            /// 運送会社会社番号
            /// </summary>
            public string TRANSPORT_ADMIN_CODE;
        }

        /// <summary>
        /// 集配先コードを読み取る
        /// </summary>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public Result.ExecutionResult<IQueryable<DELIVERY_REQUEST_DETAIL>> read(ReadConfig readConfig)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<IQueryable<DELIVERY_REQUEST_DETAIL>>((ret) =>
            {
                if (null == readConfig.DELIVERY_REQUEST_NUMBER && null == readConfig.TRANSPORT_ADMIN_CODE)
                {
                    throw new ArgumentNullException();
                }
                
                //運送会社コードは必須とする
                if ( null == readConfig.TRANSPORT_ADMIN_CODE)
                {
                    throw new ArgumentNullException();
                }

                ret.result = db.DELIVERY_REQUEST_DETAILs
                .Where(e => e.TRANSPORT_ADMIN_CODE == readConfig.TRANSPORT_ADMIN_CODE);

                //指定が出てきたら追加する
                if (null != readConfig.DELIVERY_REQUEST_NUMBER)
                {
                    ret.result = ret.result.Where(e => e.DELIVERY_REQUEST_NUMBER == readConfig.DELIVERY_REQUEST_NUMBER);
                }

                //枝番でソートする
                ret.result = ret.result.OrderBy(e => e.DELIVERY_REQUEST_DETAIL_NUMBER);

                ret.succeed = true;
            });
        }

        /// <summary>
        /// 車に関する情報のみ更新する
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<Result.DefaultExecutionResult> updateBase
            (IEnumerable<CollisionDetectable_DELIVERY_DETAIL> list, Action<DELIVERY_REQUEST_DETAIL, CollisionDetectable_DELIVERY_DETAIL> copyOpe)
        {
            return await RazorPagesLearning.Service.DB.Helper.ServiceHelper.doOperationWithErrorManagementAsync(async (ret) =>
           {
               var upData = list.Select(e =>
               {
                   //viewモデルが持っているデータはDBにあるものと異なるので
                   //DBから値をとってきて必要データを追加して渡す
                   var t = this.db.DELIVERY_REQUEST_DETAILs
                       .Where(in_e => 
                       e.ID == in_e.ID &&
                       e.TRANSPORT_ADMIN_CODE == in_e.TRANSPORT_ADMIN_CODE &&
                       e.DELIVERY_REQUEST_NUMBER == in_e.DELIVERY_REQUEST_NUMBER)
                       .First();

                   //更新対象の情報
                   copyOpe(t, e);

                   //衝突検知
                   if (e.collisionDetection_Timestamp != t.timestampToString())
                   {
                       //衝突検知
                       throw new ApplicationException
                           ("集配依頼詳細(DELIVERY_DETAIL)テーブルに関して、他ユーザーによる編集を検知しました。");
                   }
                   return t;
               });

               await this.setUpdateManagementInformation(upData);

               //実行成功
               ret.succeed = true;

               return ret;
           });
        }

        /// <summary>
        /// 車に関する情報のみ更新する
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<Result.DefaultExecutionResult> updateOnlyTRUCKInfo(IEnumerable<CollisionDetectable_DELIVERY_DETAIL> list)
        {
            return await updateBase(list, new Action<DELIVERY_REQUEST_DETAIL, CollisionDetectable_DELIVERY_DETAIL>((dst, src) =>
            {
                //更新対象の情報
                dst.TRUCK_CHARGE = src.TRUCK_CHARGE;
                dst.TRUCK_NUMBER = src.TRUCK_NUMBER;
                dst.TRUCK_MANAGE_NUMBER = src.TRUCK_MANAGE_NUMBER;
            }));
        }

        /// <summary>
        /// 車に関する情報のみ更新する
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<Result.DefaultExecutionResult> updateOnlyROUTEInfo(IEnumerable<CollisionDetectable_DELIVERY_DETAIL> list)
        {
            return await updateBase(list, new Action<DELIVERY_REQUEST_DETAIL, CollisionDetectable_DELIVERY_DETAIL>((dst, src) =>
            {
                //更新対象の情報
                dst.ROUTE = src.ROUTE;
            }));
        }

        /// <summary>
        /// 衝突検知可能な集配依頼詳細
        /// </summary>
        public class CollisionDetectable_DELIVERY_DETAIL : DELIVERY_REQUEST_DETAIL
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
            public static CollisionDetectable_DELIVERY_DETAIL convert(DELIVERY_REQUEST_DETAIL src)
            {
                //データ形式を変換する
                var m = Mapper.Map<CollisionDetectable_DELIVERY_DETAIL>(src);
                return m;
            }

        }


    }
}
