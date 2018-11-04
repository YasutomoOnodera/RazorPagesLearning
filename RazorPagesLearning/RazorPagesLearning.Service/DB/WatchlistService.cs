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
    public class WatchlistService : DBServiceBase
    {
        public WatchlistService(
        RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
        ClaimsPrincipal ref_user,
        SignInManager<IdentityUser> ref_signInManager,
        UserManager<IdentityUser> ref_userManager)
        : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {

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
        /// 読み取り設定
        /// </summary>
        public class ReadConfig
        {
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
                this.sortOrder = "STORAGE_MANAGE_NUMBER";
            }
        }

        /// <summary>
        /// 統計情報の取得
        /// </summary>
        /// <returns></returns>
        public StatisticsInfo getStatistics(long stockId)
        {
            return new StatisticsInfo
            {
                // 表示する一覧の全件数を返す
                numbers = read(stockId).Count()
            };
        }


        /// <summary>
        /// ウォッチリスト 画面表示
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public IQueryable<WATCHLIST> dispWatchList(ReadConfig readConfig, long userId, bool b)
        {
            // データ取得の内容
            var wacthList = read(userId);

            // ソートの内容
            wacthList = setSortOrder(readConfig.sortOrder, readConfig.sortDirection, wacthList);

            if (b == true)
            {
                // 全件取得
                return wacthList;
            }
            else
            {
                // 必要な部分のみ抽出して返す
                return wacthList
                    .Skip(readConfig.start)
                    .Take(readConfig.take);
            }
        }

        /// <summary>
        /// データ取得までの内容を取得
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private IQueryable<WATCHLIST> read(long userId)
        {
            // STOCKのSHIPPER_CODEとUSER_ACCOUNTのCURRENT_SHIPPER_CODEが一致する在庫でなければ、出してはいけない
            var wacthList = db.WATCHLISTs
            .Include(e => e.STOCK)
            .Where(e => e.USER_ACCOUNT_ID == userId)
            .Where(e => e.STOCK.SHIPPER_CODE == e.USER_ACCOUNT.CURRENT_SHIPPER_CODE);

            return wacthList;
        }

		/// <summary>
		/// 一件のみ取得
		/// </summary>
		/// <param name="stockId"></param>
		/// <param name="userAccountId"></param>
		/// <returns></returns>
		public async Task<Result.ExecutionResult<WATCHLIST>> Read(Int64 stockId, Int64 userAccountId)
		{
			return Helper.ServiceHelper.DoOperationWithErrorManagement<WATCHLIST>((ret) =>
			{
                // STOCKのSHIPPER_CODEとUSER_ACCOUNTのCURRENT_SHIPPER_CODEが一致する在庫でなければ、IDが一致していても出してはいけない
                ret.result = db.WATCHLISTs
                    .Include(e => e.STOCK)
					.Where(e => e.STOCK_ID == stockId)
					.Where(e => e.USER_ACCOUNT_ID == userAccountId)
                    .Where(e => e.STOCK.SHIPPER_CODE == e.USER_ACCOUNT.CURRENT_SHIPPER_CODE)
					.FirstOrDefault();
            });
		}

        /// <summary>
        /// ソート項目の内容を設定する
        /// </summary>
        /// <param name="column"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        private IQueryable<WATCHLIST> setSortOrder(string column, SortDirection sortDirection, IQueryable<WATCHLIST> q)
        {

            #region ローカル関数

            //主条件でソートする
            IOrderedQueryable<WATCHLIST> LF_sortByPrimaryKey()
            {
                //デフォルトソート順序
                if (null == column)
                {
                    column = "WACTH_STORAGE_MANAGE_NUMBER";
                }

                switch (column.Trim())
                {
                    // カラムの表示情報
                    case "WACTH_STORAGE_MANAGE_NUMBER":
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

                    case "WACTH_STATUS":
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

                    case "WACTH_TITLE":
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

                    case "WACTH_SUBTITLE":
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

                    case "WACTH_NOTE":
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

                    case "WACTH_SHIPPER_NOTE":
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

                    case "WACTH_CUSTOMER_MANAGE_NUMBER":
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

                    case "WACTH_PROCESSING_DATE":
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

                    case "WACTH_SHAPE":
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

                    case "WACTH_REMARK1":
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

                    case "WACTH_REMARK2":
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

                    case "WACTH_STOCK_COUNT":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.STOCK.STOCK_COUNT);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.STOCK.STOCK_COUNT);
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
        /// 作業依頼一覧に追加(RequestListServiceに同様のものはある)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task add(List<REQUEST_LIST> list)
        {
            var user = await readLoggedUserInfo();

            await this.setBothManagementInformation(list);
            this.db.REQUEST_LISTs.AddRange(list);

        }

		/// <summary>
		/// ウォッチリストに追加
		/// </summary>
		/// <param name="mwlStockIds"></param>
		/// <param name="userAccountId"></param>
		/// <returns></returns>
		public async Task<Result.DefaultExecutionResult> add(List<Int64> mwlStockIds, Int64 userAccountId)
		{
			async Task<Result.DefaultExecutionResult> task()
			{
				// -----------------------------------------
				// ウォッチリストに追加
				// -----------------------------------------
				var requests = new List<WATCHLIST>();
				foreach (Int64 stockId in mwlStockIds)
				{
					// 既にある場合は追加しない
					var record = await Read(stockId, userAccountId);
					if (null != record.result)
					{
						continue;
					}

					// 追加する
					requests.Add(new WATCHLIST
					{
						STOCK_ID = stockId,
						USER_ACCOUNT_ID = userAccountId
					});
				}
				await setBothManagementInformation(requests);

				// DBに登録
				db.WATCHLISTs.AddRange(requests);
				db.SaveChanges();

				return new Result.DefaultExecutionResult
				{
					succeed = true,
					result = requests
				};
			};

			try
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					var ret = await task();
					if (false == ret.succeed)
					{
						throw new ApplicationException(string.Join(",", ret.errorMessages.ToString()));
					}

					// DB保存する
					transaction.Commit();

					return ret;
				}
			}
			catch (Exception e)
			{
				return new Result.DefaultExecutionResult
				{
					errorMessages = new List<string> { e.Message },
					succeed = false,
					result = new List<WATCHLIST>()
				};
			}
		}


        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public async Task<Result.ExecutionResult<IQueryable<WATCHLIST>>> delete(long stockId)
        {
            return await RazorPagesLearning.Service.DB.Helper.ServiceHelper
                .doOperationWithErrorManagementAsync<IQueryable<WATCHLIST>>(async (ret) =>
            {
                var user = await readLoggedUserInfo();

                var q = db.WATCHLISTs

                // ユーザーアカウントID、在庫IDで削除する
                .Where(e => e.USER_ACCOUNT_ID == user.ID).Where(e => e.STOCK_ID == stockId);

                db.WATCHLISTs.RemoveRange(q);

                return ret;
            });
        }
    }
}
