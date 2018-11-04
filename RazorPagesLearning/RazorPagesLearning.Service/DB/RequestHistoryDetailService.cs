﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Claims;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RazorPagesLearning.Service.DB;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// 作業依頼履歴詳細サービス
    /// </summary>
    public class RequestHistoryDetailService : DBServiceBase
    {

        public RequestHistoryDetailService(
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
            /// 作業依頼履歴
            /// </summary>
            public REQUEST_HISTORY requestHistory;

            /// <summary>
            /// 作業依頼詳細ワーク
            /// </summary>
            public List<WK_REQUEST_DETAIL> wkRequestHistoryDetail;
        }

        public async Task<bool> save(WriteConfig writeConfig)
        {
            bool ret = false;
            var now = DateTimeOffset.Now;

            try
            {
                foreach (var requestHistoryDetail in writeConfig.wkRequestHistoryDetail)
                {
                    var rhd = new REQUEST_HISTORY_DETAIL();

                    rhd.REQUEST_HISTORY_ID = writeConfig.requestHistory.ID;

                    // 在庫
                    rhd.TITLE = requestHistoryDetail.STOCK.TITLE;
                    rhd.SUBTITLE = requestHistoryDetail.STOCK.SUBTITLE;
                    rhd.CUSTOMER_MANAGE_NUMBER = requestHistoryDetail.STOCK.CUSTOMER_MANAGE_NUMBER; 
                    rhd.PROCESSING_DATE = requestHistoryDetail.STOCK.PROCESSING_DATE;
                    rhd.SHAPE = requestHistoryDetail.STOCK.SHAPE;
                    rhd.REMARK1 = requestHistoryDetail.STOCK.REMARK1;
                    rhd.REMARK2 = requestHistoryDetail.STOCK.REMARK2;
                    rhd.WMS_STATUS = "1";   // TODO 正しい値にする　ドメインの固定値を入れる
                    rhd.REQUEST_COUNT = requestHistoryDetail.REQUEST_COUNT;
                    rhd.CONFIRM_COUNT = 0;  // 0固定
                    rhd.CLASS1 = requestHistoryDetail.STOCK.CLASS1;
                    rhd.CLASS2 = requestHistoryDetail.STOCK.CLASS2;
                    rhd.NOTE = requestHistoryDetail.STOCK.NOTE;
                    rhd.PRODUCT_DATE = requestHistoryDetail.STOCK.PRODUCT_DATE;
                    rhd.STORAGE_DATE = requestHistoryDetail.STOCK.STORAGE_DATE;
                    rhd.SCRAP_SCHEDULE_DATE = requestHistoryDetail.STOCK.SCRAP_SCHEDULE_DATE;
					rhd.TIME1 = requestHistoryDetail.STOCK.TIME1;
                    rhd.TIME2 = requestHistoryDetail.STOCK.TIME2;
                    rhd.STOCK_COUNT = requestHistoryDetail.STOCK.STOCK_COUNT;  
                    rhd.STORAGE_RETRIEVAL_DATE = requestHistoryDetail.STOCK.STORAGE_RETRIEVAL_DATE;
                    rhd.ARRIVAL_TIME = requestHistoryDetail.STOCK.ARRIVAL_TIME;
                    rhd.BARCODE = requestHistoryDetail.STOCK.BARCODE;
#if false // 20180925_DBModel修正 (WMS登録日、WMS更新日をSTOCKだけで保持する)
					rhd.REGIST_DATE = requestHistoryDetail.STOCK.REGIST_DATE;
                    rhd.UPDATE_DATE = requestHistoryDetail.STOCK.UPDATE_DATE;
#endif // 20180925_DBModel修正 (WMS登録日、WMS更新日はSTOCKだけで保持する)
					rhd.SLIP_NUMBER = null;

                    // 顧客専用項目
                    rhd.PROJECT_NO1 = requestHistoryDetail.STOCK.PROJECT_NO1;  
                    rhd.PROJECT_NO2 = requestHistoryDetail.STOCK.PROJECT_NO2;  
                    rhd.COPYRIGHT1 = requestHistoryDetail.STOCK.COPYRIGHT1;
                    rhd.COPYRIGHT2 = requestHistoryDetail.STOCK.COPYRIGHT2;
                    rhd.CONTRACT1 = requestHistoryDetail.STOCK.CONTRACT1;
                    rhd.CONTRACT2 = requestHistoryDetail.STOCK.CONTRACT2;
                    rhd.DATA_NO1 = requestHistoryDetail.STOCK.DATA_NO1;
                    rhd.DATA_NO2 = requestHistoryDetail.STOCK.DATA_NO2;
                    rhd.PROCESS_JUDGE1 = requestHistoryDetail.STOCK.PROCESS_JUDGE1;
                    rhd.PROCESS_JUDGE2 = requestHistoryDetail.STOCK.PROCESS_JUDGE2;
                    rhd.UNIT = requestHistoryDetail.STOCK.UNIT;
                    rhd.CREATED_AT = now;
                    rhd.UPDATED_AT = now;

                    rhd.STATUS = requestHistoryDetail.STOCK.STATUS;
                    rhd.STORAGE_MANAGE_NUMBER = requestHistoryDetail.STOCK.STORAGE_MANAGE_NUMBER;

                    //共通情報を設定する
                    await setBothManagementInformation(rhd);

                    db.AddRange(rhd);
                    db.SaveChanges();
                }

                ret = true;
            }
            catch (Exception)
            {
                ret = false;
                //throw;
            }

            return ret;
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
            public int id;

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

			public ReadConfig()
			{
			}

            public ReadConfig(int id)
            {
                this.id = id;

                ///倉庫管理番号をソート順序とする
                this.sortOrder = "REQUEST_DATE";
            }
        }

        /// <summary>
        /// ウォッチリストから作業依頼履歴詳細一覧を取得
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public IQueryable<REQUEST_HISTORY_DETAIL> readRequestHistoryDetail(long userId)
        {

            var requestHistoryDetail = db.REQUEST_HISTORY_DETAILs;

            return requestHistoryDetail
                .Include(e => e.REQUEST_HISTORY)
                .Where(e => e.REQUEST_HISTORY.USER_ACCOUNTID == userId);
        }

        /// <summary>
        /// 作業依頼履歴詳細一覧を取得
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public IEnumerable<REQUEST_HISTORY_DETAIL> read(ReadConfig readConfig)
        {

            var q = db.REQUEST_HISTORY_DETAILs.Include(e => e.WMS_RESULT_HISTORY);

            return q
                .Where(e => e.REQUEST_HISTORY.ID == readConfig.id)
                .Skip(readConfig.start)
                .Take(readConfig.take)
                .ToList();
        }

        /// <summary>
        /// 作業依頼履歴詳細一覧(全件)を取得
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public IEnumerable<REQUEST_HISTORY_DETAIL> readAll(ReadConfig readConfig)
        {

            var q = db.REQUEST_HISTORY_DETAILs;

            return q
                .Where(e => e.REQUEST_HISTORY.ID == readConfig.id)
                .ToList();
        }

#if false // 20180925_DBModel修正 (STOCKから取れるようにしたため、本関数は不要)
		/// <summary>
		/// 在庫IDから最新レコード検索 MEMO オーダーは引数にしたほうがいいかも
		/// </summary>
		/// <param name="stockId"></param>
		/// <returns></returns>
		public REQUEST_HISTORY_DETAIL readByMwlStockId(long stockId)
        {
            var requestHistoryDetail = db.REQUEST_HISTORY_DETAILs;

            return requestHistoryDetail
                // memo：JsonConvertでエラーが出たため、コメントアウト
                //.Include(e => e.REQUEST_HISTORY)
                .Where(e => e.MWL_STOCK_ID == stockId)
                .OrderByDescending(e => e.ID)
                .FirstOrDefault();
        }
#endif // 20180925_DBModel修正 (STOCKから取れるようにしたため、本関数は不要)

#if false // 20180925_DBModel修正 (STOCKから取れるようにしたため、本関数は不要)
		public Result.ExecutionResult<REQUEST_HISTORY_DETAIL> readByMwlStockIdOrderByCreatedAt(long stockId)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.doOperationWithErrorManagement<REQUEST_HISTORY_DETAIL>((ret) =>
            {
                ret.result = db.REQUEST_HISTORY_DETAILs
                 .Where(e => e.MWL_STOCK_ID == stockId)
                 .OrderByDescending(e => e.CREATED_AT)
                 .FirstOrDefault();
                ret.succeed = true;
            });
        }
#endif // 20180925_DBModel修正 (STOCKから取れるようにしたため、本関数は不要)

    }
}
