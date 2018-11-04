using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Claims;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using RazorPagesLearning.Service.Definition;
using RazorPagesLearning.Service.DB;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// 作業依頼履歴サービス
    /// </summary>
    public class RequestHistoryService : DBServiceBase
    {
        public RequestHistoryService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {

        }


        /// <summary>
        /// 書き込み設定
        /// </summary>
        public class WriteConfig
        {
            /// <summary>
            /// 集配先ワーク
            /// </summary>
            public WK_REQUEST_DELIVERY wkDelivery;

            /// <summary>
            /// 作業依頼内容
            /// </summary>
            public string REQUEST_KIND;

            /// <summary>
            /// ユーザーアカウント
            /// </summary>
            public USER_ACCOUNT userAccount;
        }

        public async Task<REQUEST_HISTORY> save(WriteConfig writeConfig)
        {
            var now = DateTimeOffset.Now;
            var rh = new REQUEST_HISTORY();

            rh.USER_ACCOUNTID = writeConfig.wkDelivery.USER_ACCOUNT_ID;
            rh.REQUEST_DATE = DateTimeOffset.Now;
            rh.DELIVERY_ADMIN_ID = writeConfig.wkDelivery.DELIVERY_ADMIN_ID;
            //rh.ORDER_NUMBER = null; // TODO 値設定 ←　RazorPagesLearning確認(9/13)
            rh.REQUEST_KIND = writeConfig.REQUEST_KIND; 
            rh.DETAIL_COUNT = writeConfig.wkDelivery.WK_REQUEST.DETAIL_COUNT;
            rh.WMS_STATUS = "1";    // memo　※「1:作業中」で固定値とする
            rh.REQUEST_COUNT = writeConfig.wkDelivery.WK_REQUEST.REQUEST_COUNT_SUM; 
            rh.CONFIRM_COUNT = 0; // APIで来た時に更新するので現時点では0。型は要修正。

            // 集配先
            rh.SHIP_RETURN_CODE = writeConfig.wkDelivery.DELIVERY_ADMIN.SHIPPER_CODE;
            rh.SHIP_RETURN_COMPANY = writeConfig.wkDelivery.DELIVERY_ADMIN.COMPANY;
#if false // 20180925_DBModel修正 (REQUEST_HISTORY.部課コードは廃止)
			rh.DELIVERY_DEPARTMENT_CODE = writeConfig.wkDelivery.DELIVERY_ADMIN.SHIPPER_ADMIN.DEPARTMENT_ADMINs[0].DEPARTMENT_CODE; // TODO 複数の部課に所属するときの対応
#endif // 20180925_DBModel修正 (REQUEST_HISTORY.部課コードは廃止)
			rh.SHIP_RETURN_DEPARTMENT = writeConfig.wkDelivery.DELIVERY_ADMIN.DEPARTMENT;
            rh.SHIP_RETURN_CHARGE_NAME = writeConfig.wkDelivery.DELIVERY_CHARGE;
            rh.SHIP_RETURN_ZIPCODE = writeConfig.wkDelivery.DELIVERY_ADMIN.ZIPCODE;
            rh.SHIP_RETURN_ADDRESS = string.Format("{0} {1}", writeConfig.wkDelivery.DELIVERY_ADMIN.ADDRESS1, writeConfig.wkDelivery.DELIVERY_ADMIN.ADDRESS2);
            rh.SHIP_RETURN_TEL = writeConfig.wkDelivery.DELIVERY_ADMIN.TEL;

            // 依頼者
            rh.OWNER_SHIPPER_CODE = writeConfig.userAccount.CURRENT_SHIPPER_CODE;
            rh.OWNER_COMPANY = writeConfig.userAccount.COMPANY;
#if false // 20180925_DBModel修正 (REQUEST_HISTORY.部課コードは廃止)
			rh.OWNER_DEPARTMENT_CODE = writeConfig.userAccount.DEFAULT_DEPARTMENT_CODE;
#endif // 20180925_DBModel修正 (REQUEST_HISTORY.部課コードは廃止)
			rh.OWNER_DEPARTMENT = writeConfig.userAccount.DEPARTMENT;
            rh.OWNER_CHARGE = writeConfig.userAccount.NAME;
            rh.OWNER_ZIPCODE = writeConfig.userAccount.ZIPCODE;
            rh.OWNER_ADDRESS = string.Format("{0} {1}", writeConfig.userAccount.ADDRESS1, writeConfig.userAccount.ADDRESS2);
            rh.OWNER_TEL = writeConfig.userAccount.TEL;

            rh.SPECIFIED_DATE = writeConfig.wkDelivery.SPECIFIED_DATE;
            rh.SPECIFIED_TIME = writeConfig.wkDelivery.SPECIFIED_TIME;

            //共通情報を設定する
            await setBothManagementInformation(rh);

            db.AddRange(rh);
            db.SaveChanges();

            //[ToDo]
            //暫定対応
            //シーケンスの値としてREQUEST_HISTORYのIDを採用する。
            //将来的にはシーケンスのスタート値を変更したとのことなので、
            //シーケンスの種として別の種を採用する必要あり。
            //
            //以下処理で実現できる可能性あり。
            //
            //Sequences
            //https://docs.microsoft.com/en-us/ef/core/modeling/relational/sequences
            rh.ORDER_NUMBER = rh.ID.ToString();

            return rh;
        }

        /// <summary>
        /// 統計情報
        /// </summary>
        public class StatisticsInfo
        {
            /// <summary>
            /// データ件数
            /// </summary>
            public int numbers;
        }

        /// <summary>
        /// 統計情報
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public StatisticsInfo getStatistics()
        {
            return new StatisticsInfo
            {
                numbers = db.REQUEST_HISTORies.Count()
            };
        }


        /// <summary>
        /// 作業依頼履歴検索結果
        /// 
        /// </summary>
        public class RequestHistoryResult
        {
            public REQUEST_HISTORY result { get; set; }

        }

        /// <summary>
        /// 検索条件
        /// </summary>
        public class ReadConfig
        {
            /// <summary>
            /// 依頼内容（TODO:チェックボックスで複数）
            /// </summary>
            public string requestKind;

            /// <summary>
            /// 状態　(TODO:チェックボックスで複数）
            /// </summary>
            public string wmsStatus;

            /// <summary>
            /// 依頼日の範囲検索開始
            /// </summary>
            public string requestDateStart;
            /// <summary>
            /// 依頼日の範囲検索終了
            /// </summary>
            public string requestDateEnd;

            /// <summary>
            /// 受付番号の範囲検索開始
            /// </summary>
            public string orderNumberStart;
            /// <summary>
            /// 受付番号の範囲検索終了
            /// </summary>
            public string orderNumberEnd;

            /// <summary>
            /// 集配先コード
            /// </summary>
            public string deliveryShipperCode;

            /// <summary>
            /// 集配先名
            /// </summary>
            public string deliveryCompany;

            /// <summary>
            /// 集配先名検索のAND/OR指定
            /// </summary>
            public string deliveryCompanyKind;

            /// <summary>
            /// 依頼者
            /// </summary>
            public string ownerShipperCodel;

            /// <summary>
            /// 読み取り開始位置
            /// </summary>
            public int start;

            /// <summary>
            /// 取り出しデータ件数
            /// </summary>
            public int take;

            /// <summary>
            /// ソート順序
            /// </summary>
            public string sortOrder;

            /// <summary>
            /// ソート順序(昇順/降順)
            /// </summary>
            public SortDirection sortDirection;

            public ReadConfig()
            {
                ///倉庫管理番号をソート順序とする
                this.sortOrder = "REQUEST_DATE";
            }
        }

        /// <summary>
        /// 作業依頼履歴をIDで取得
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public Result.ExecutionResult<REQUEST_HISTORY> readById(long id)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<REQUEST_HISTORY>((ret) =>
            {
                ret.result = db.REQUEST_HISTORies
                 .Where(e => e.ID == id)
                 .FirstOrDefault();
                ret.succeed = true;
            });
        }


        /// <summary>
        /// 作業依頼履歴一覧を取得
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public IEnumerable<REQUEST_HISTORY> read(ReadConfig readConfig)
        {

            var q = db.REQUEST_HISTORies.AsQueryable();
            // q = setSortOrder(readConfig.sortOrder, q);
            if (readConfig.deliveryShipperCode != null && readConfig.deliveryShipperCode.Length > 0)
            {
                q = q.Where(e => e.SHIP_RETURN_CODE.Contains(readConfig.deliveryShipperCode));
            }
            return q
                .Skip(readConfig.start)
                .Take(readConfig.take)
                .ToList();
        }
    }
}
