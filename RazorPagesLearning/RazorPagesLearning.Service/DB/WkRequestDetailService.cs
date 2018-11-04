using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.Definition;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
	public class WkRequestDetailService : DBServiceBase
	{
		public WkRequestDetailService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
			ClaimsPrincipal ref_user,
			SignInManager<IdentityUser> ref_signInManager,
			UserManager<IdentityUser> ref_userManager)
			: base(ref_db, ref_user, ref_signInManager, ref_userManager)
		{
		}

		/// <summary>
		/// 読み取り設定
		/// </summary>
		public class ReadConfig
		{
			/// <summary>
			/// 作業依頼ワークID
			/// </summary>
			public Int64 WK_REQUEST_ID;

            /// <summary>
            /// ソート順序
            /// </summary>
            public string sortColumn { get; set; }

            /// <summary>
            /// ソート方向
            /// </summary>
            public SortDirection sortDirection { get; set; }
        }

        /// <summary>
        /// 追加データの設定
        /// </summary>
        public class AddConfig
        {
            /// <summary>
            /// MWL在庫ID
            /// </summary>
            public long mwlStockId { get; set; }

            /// <summary>
            /// 処理日
            /// </summary>
            public DateTimeOffset processingDate { get; set; }

            /// <summary>
            /// 依頼数
            /// </summary>
            public int requestCount { get; set; }
        }

        /// <summary>
        /// ソート項目の内容を設定する
        /// </summary>
        /// <param name="column"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        private IQueryable<WK_REQUEST_DETAIL> setSortOrder(string column, SortDirection sortDirection, IQueryable<WK_REQUEST_DETAIL> q)
        {

            #region ローカル関数

            //主条件でソートする
            IOrderedQueryable<WK_REQUEST_DETAIL> LF_sortByPrimaryKey()
            {
                //デフォルトソート順序
                if (null == column)
                {
                    column = "REQUEST_CONFIRM_STORAGE_MANAGE_NUMBER";
                }

                switch (column.Trim())
                {
                    // カラムの表示情報
                    //倉庫管理番号
                    case "REQUEST_CONFIRM_STORAGE_MANAGE_NUMBER":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STOCK.STORAGE_MANAGE_NUMBER);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STOCK.STORAGE_MANAGE_NUMBER);
                                    }
                            }
                            break;
                        }
                        //ステータス
                    case "REQUEST_CONFIRM_STATUS":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STOCK.STATUS);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STOCK.STATUS);
                                    }
                            }
                            break;
                        }
                    //題名
                    case "REQUEST_CONFIRM_TITLE":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STOCK.TITLE);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STOCK.TITLE);
                                    }
                            }
                            break;
                        }
                    //副題
                    case "REQUEST_CONFIRM_SUBTITLE":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STOCK.SUBTITLE);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STOCK.SUBTITLE);
                                    }
                            }
                            break;
                        }
                    //備考
                    case "REQUEST_CONFIRM_NOTE":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STOCK.NOTE);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STOCK.NOTE);
                                    }
                            }
                            break;
                        }
                    //荷主項目
                    case "REQUEST_CONFIRM_SHIPPER_NOTE":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STOCK.SHIPPER_NOTE);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STOCK.SHIPPER_NOTE);
                                    }
                            }
                            break;
                        }
                    //お客様管理番号
                    case "REQUEST_CONFIRM_CUSTOMER_MANAGE_NUMBER":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STOCK.CUSTOMER_MANAGE_NUMBER);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STOCK.CUSTOMER_MANAGE_NUMBER);
                                    }
                            }
                            break;
                        }
                    //処理日
                    case "REQUEST_CONFIRM_PROCESSING_DATE":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STOCK.PROCESSING_DATE);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STOCK.PROCESSING_DATE);
                                    }
                            }
                            break;
                        }
                    //形状
                    case "REQUEST_CONFIRM_SHAPE":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STOCK.SHAPE);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STOCK.SHAPE);
                                    }
                            }
                            break;
                        }
                    //Remark1
                    case "REQUEST_CONFIRM_REMARK1":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STOCK.REMARK1);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STOCK.REMARK1);
                                    }
                            }
                            break;
                        }
                    //Remark2
                    case "REQUEST_CONFIRM_REMARK2":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STOCK.REMARK2);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STOCK.REMARK2);
                                    }
                            }
                            break;
                        }
                    //依頼数
                    case "REQUEST_CONFIRM_REQUEST_COUNT":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.REQUEST_COUNT);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.REQUEST_COUNT);
                                    }
                            }
                            break;
                        }

                }
                throw new ApplicationException("想定外のソート条件です。");
            }

            #endregion

            return LF_sortByPrimaryKey().ThenBy(e => e.STOCK_ID);

        }

        /// <summary>
        /// データを読み取り
        /// </summary>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public Result.ExecutionResult<IQueryable<WK_REQUEST_DETAIL>> read(ReadConfig readConfig)
		{
			return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<IQueryable<WK_REQUEST_DETAIL>>((ret) =>
			{
				ret.result = null;
                var q =  db.WK_REQUEST_DETAILs
                    .Where(e => e.WK_REQUEST_ID == readConfig.WK_REQUEST_ID)
                    .Include(e => e.STOCK);

                //ソート条件を追加する
                ret.result = setSortOrder(readConfig.sortColumn , readConfig.sortDirection ,q);

                ret.succeed = true;

			});
		}

        /// <summary>
        /// 対象のデータを削除する
        /// </summary>
        /// <param name="readConfig"></param>
        public void delete(ReadConfig readConfig)
        {
            var deleteWkRequestDetailData = db.WK_REQUEST_DETAILs
                .Where(e => e.WK_REQUEST_ID == readConfig.WK_REQUEST_ID);

            db.WK_REQUEST_DETAILs.RemoveRange(deleteWkRequestDetailData);
        }

        /// <summary>
        /// 追加用の作業依頼詳細ワークデータの取得
        /// </summary>
        /// <param name="addConfigList"></param>
        /// <returns></returns>
        public async Task<List<WK_REQUEST_DETAIL>> getAddList(List<AddConfig> addConfigList)
        {
            List<WK_REQUEST_DETAIL> retList = new List<WK_REQUEST_DETAIL>();
            foreach (var elm in addConfigList)
            {
                var wkRequestDetail = new WK_REQUEST_DETAIL();

                wkRequestDetail.STOCK_ID = elm.mwlStockId;
                wkRequestDetail.REQUEST_COUNT = elm.requestCount;

                await setCreateManagementInformation(wkRequestDetail);
                await setUpdateManagementInformation(wkRequestDetail);
            }

            return retList;
        }

    }
}
