using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Claims;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using RazorPagesLearning.Service.Definition;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// 集配依頼サービス
    /// </summary>
    public class DeliveryRequestService : DBServiceBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// コンストラクタ
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public DeliveryRequestService(
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
            /// 運送会社コード
            /// </summary>
            public string TRANSPORT_ADMIN_CODE;

            /// <summary>
            /// 集配依頼番号
            /// </summary>
            public string DELIVERY_REQUEST_NUMBER;

            /// <summary>
            /// ソート順序
            /// </summary>
            public string sortColumn;

            /// <summary>
            /// ソート順序(昇順/降順)
            /// </summary>
            public SortDirection sortDirection;
        }

        /// <summary>
        /// ソート項目の内容を設定する
        /// </summary>
        /// <param name="column"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        private IQueryable<DELIVERY_REQUEST> setSortOrder(string column, SortDirection sortDirection, IQueryable<DELIVERY_REQUEST> q)
        {
            #region ローカル関数

            //主条件でソートする
            IOrderedQueryable<DELIVERY_REQUEST> LF_sortByPrimaryKey()
            {
                //デフォルトソート順序
                if (null == column)
                {
                    column = "DELIVERY_STATUS";
                }

                switch (column.Trim())
                {
                    // ステータス
                    case "DELIVERY_STATUS":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STATUS);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STATUS);
                                    }
                            }
                            break;
                        }
                    //集配日
                    case "DELIVERY_DELIVERY_DATE":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.DELIVERY_DATE);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.DELIVERY_DATE);
                                    }
                            }
                            break;
                        }
                    //確定日
                    case "DELIVERY_CONFIRM_DATETIME":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.CONFIRM_DATETIME);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.CONFIRM_DATETIME);
                                    }
                            }
                            break;
                        }
                    //実績修正日
                    case "DELIVERY_CORRECTION_DATETIME":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.CORRECTION_DATETIME);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.CORRECTION_DATETIME);
                                    }
                            }
                            break;
                        }

                }
                throw new ApplicationException("想定外のソート条件です。");
            }

            #endregion

            return LF_sortByPrimaryKey().ThenBy(e => e.STATUS);

        }

        /// <summary>
        /// 集配先コードを読み取る
        /// </summary>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public Result.ExecutionResult<IQueryable<DELIVERY_REQUEST>> read(ReadConfig readConfig)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<IQueryable<DELIVERY_REQUEST>>((ret) =>
            {
                var q = db.DELIVERY_REQUESTs
                .Where(e => e.TRANSPORT_ADMIN_CODE == readConfig.TRANSPORT_ADMIN_CODE);

                if (null != readConfig.DELIVERY_REQUEST_NUMBER)
                {
                    if (String.Empty != readConfig.DELIVERY_REQUEST_NUMBER)
                    {
                        q = q.Where(e => e.DELIVERY_REQUEST_NUMBER == readConfig.DELIVERY_REQUEST_NUMBER);
                    }
                }

                //ソート順序を追加
                ret.result = setSortOrder(readConfig.sortColumn, readConfig.sortDirection, q);
                ret.succeed = true;
            });
        }

        /// <summary>
        /// ステータス定義
        /// </summary>
        public enum DeliveryStatus
        {
            入力可能,
            確定済み,
            修正依頼中,
            削除
        }

        /// <summary>
        /// enum定義を変換する
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DeliveryStatus toDeliveryStatus(string d)
        {
            //Fruits列挙体のメンバの値を列挙する
            foreach (DeliveryStatus val in Enum.GetValues(typeof(DeliveryStatus)))
            {
                //メンバ名を取得する
                string eName = Enum.GetName(typeof(DeliveryStatus), val);
                if (d == eName)
                {
                    return val;
                }
            }
            throw new ApplicationException($"{d}をDeliveryStatus型に変換できませんでした。");
        }

        /// <summary>
        /// ステータス更新設定
        /// </summary>
        public class StatusUpdateConfig
        {
            /// <summary>
            /// 運送会社コード
            /// </summary>
            public string TRANSPORT_ADMIN_CODE;

            /// <summary>
            /// 集配依頼番号
            /// </summary>
            public string DELIVERY_REQUEST_NUMBER;

            /// <summary>
            /// 設定対象ステータス
            /// </summary>
            public DeliveryStatus status;

        }

        /// <summary>
        /// ステータス更新
        /// </summary>
        /// <param name="statusConfig"></param>
        /// <returns></returns>
        public async Task< Result.DefaultExecutionResult> statusUpdate(StatusUpdateConfig statusConfig)
        {
            #region ローカル関数

            ///ドメイン情報のコードを返す
            string LF_getStatusCode(DeliveryStatus sts)
            {
                const string KIND = "00100000";

                //ドメイン情報を取得
                var domainService = new Service.DB.DomainService
                    (db,
                    user,
                    signInManager,
                    userManager);

                //DBからドメイン情報を取得する
                var domainInfo = domainService.getValueList(KIND)
                    .Where(e=>e.VALUE== sts.ToString())
                    .FirstOrDefault();
                if (null == domainInfo)
                {
                    throw new ApplicationException($"ドメイン情報が見つかりません。 KIND : {KIND} , VALUE : {sts}");
                }

                return domainInfo.CODE;
            }

            //確定済みに決定
            async Task<bool> LF_OnConfirmed(DELIVERY_REQUEST target)
            {
                //確定済み時間を更新する
                target.CONFIRM_DATETIME = DateTimeOffset.Now;

                //車番、担当者を確定情報に移す
                var details = db.DELIVERY_REQUEST_DETAILs
                    .Where(e => e.TRANSPORT_ADMIN_CODE == statusConfig.TRANSPORT_ADMIN_CODE &&
                    e.DELIVERY_REQUEST_NUMBER == statusConfig.DELIVERY_REQUEST_NUMBER);

                //車番情報を取得
                var TRUCKs = db.TRUCK_ADMINs
                    .Where(e => e.TRANSPORT_ADMIN_CODE == statusConfig.TRANSPORT_ADMIN_CODE);

                //台車関係のデータを差し替え
                foreach (var ele in details)
                {
                    var td = TRUCKs.Where(e => e.TRUCK_MANAGE_NUMBER == ele.TRUCK_MANAGE_NUMBER).First();
                    ele.TRUCK_CHARGE = td.CHARGE;
                    ele.TRUCK_NUMBER = td.NUMBER;
                    ele.TRUCK_MANAGE_NUMBER = null;
                }

                //更新関係の情報を追加
               await setUpdateManagementInformation(details);

                return true;
            }

            #endregion

            return await RazorPagesLearning.Service.DB.Helper.ServiceHelper.doOperationWithErrorManagementAsync
                ( async (ret) =>
            {
                var q = db.DELIVERY_REQUESTs
                .Where(e => e.TRANSPORT_ADMIN_CODE == statusConfig.TRANSPORT_ADMIN_CODE &&
                e.DELIVERY_REQUEST_NUMBER == statusConfig.DELIVERY_REQUEST_NUMBER
                 ).FirstOrDefault();
                if (null != q)
                {
                    //状態を更新する
                    q.STATUS = LF_getStatusCode(statusConfig.status);

                    if (statusConfig.status == DeliveryStatus.確定済み)
                    {
                        //確定済み決定時のデータ移行処理
                        await LF_OnConfirmed(q);
                    }

                    ret.succeed = true;
                }
                else
                {
                    ret.errorMessages = new List<string> {
                        "指定された集配依頼情報が見つかりません。"
                    };
                    ret.succeed = false;
                }

                return ret;
            });
        }

    }
}
