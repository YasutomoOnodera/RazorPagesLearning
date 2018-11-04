using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Claims;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RazorPagesLearning.Service.Definition;
using RazorPagesLearning.Service.ExcelAnalysis;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// 作業依頼一覧サービス
    /// </summary>
    public class RequestListService : DBServiceBase
    {
        public RequestListService(
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
        /// 検索条件
        /// </summary>
        public class SearchConfig
        {
            /// <summary>
            /// 検索対象ステータス１
            /// </summary>
            public string searchStatus1;

            /// <summary>
            /// 検索対象ステータス２
            /// </summary>
            public string seatchStatus2;

            /// <summary>
            /// 作業依頼内容
            /// </summary>
            public string kind;

            /// <summary>
            /// ユーザーアカウント
            /// </summary>
            public USER_ACCOUNT userAccount;
        }

        /// <summary>
        /// 作業依頼一覧に追加
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
        /// データ取り込み画面 作業依頼追加
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="selected"></param>
        /// <param name="wkStockService"></param>
        /// <param name="stockService"></param>
        /// <returns></returns>
        public async Task<List<wkstock_Result>> addRequest(long userId, string selected, DB.WkStockService wkStockService, DB.StockService stockService)
        {
            var StockList = new List<STOCK>();
            var ReqList = new List<REQUEST_LIST>();
            var result = new List<wkstock_Result>();

            // 取り込みモードで分岐 新規orその他依頼
            if (selected == "新規入庫")
            {
                var wksVal = await wkStockService.read("20");
                try
                {
                    // WK_STOCKの中身をSTOCKへ追加
                    foreach (var wksresult in wksVal.result)
                    {
                        var stk = new STOCK();

                        stk.ID = wksresult.STOCK_ID;
                        stk.STORAGE_MANAGE_NUMBER = wksresult.STORAGE_MANAGE_NUMBER;
                        stk.CUSTOMER_MANAGE_NUMBER = wksresult.CUSTOMER_MANAGE_NUMBER;
                        stk.DEPARTMENT_CODE = wksresult.DEPARTMENT_CODE;
                        stk.TITLE = wksresult.TITLE;
                        stk.SUBTITLE = wksresult.SUBTITLE;
                        stk.SHAPE = wksresult.SHAPE;
                        stk.CLASS1 = wksresult.CLASS1;
                        stk.CLASS2 = wksresult.CLASS2;
                        stk.REMARK1 = wksresult.REMARK1;
                        stk.REMARK2 = wksresult.REMARK2;
                        stk.NOTE = wksresult.NOTE;
                        stk.SHIPPER_NOTE = wksresult.SHIPPER_NOTE;
                        stk.PRODUCT_DATE = wksresult.PRODUCT_DATE;
                        stk.SCRAP_SCHEDULE_DATE = wksresult.SCRAP_SCHEDULE_DATE;
                        stk.TIME1 = wksresult.TIME1;
                        stk.TIME2 = wksresult.TIME2;
                       
                        StockList.Add(stk);
                    }

                    // 反映準備
                    await stockService.add(StockList);

                    // 反映
                    await stockService.db.SaveChangesAsync();

                    //WK_STOCKのデータを削除する
                    await wkStockService.delete("20");


                    // STOCKの中身をREQUEST_LISTに追加
                    foreach (var stkresult in StockList)
                    {
                        var req = new REQUEST_LIST();

                        req.STOCK_ID = stkresult.ID;
                        req.USER_ACCOUNT_ID = userId;

                        ReqList.Add(req);
                    }

                    // 反映準備
                    await add(ReqList);

                    // 反映
                    await this.db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    var wk = new wkstock_Result();
                    wk.NG_IMPORT_ERROR_MESSAGE = "作業依頼追加に失敗しました。";
                    result.Add(wk);
                  
                }
            }
            else
            {
                var wksVal = await wkStockService.read("20");
                try
                {
                    // STOCKの中身を取得 
                    foreach (var wksktdata in wksVal.result)
                    {
                        var stkdata = stockService.read(wksktdata.STORAGE_MANAGE_NUMBER, wksktdata.TITLE).result;

                        var req = new REQUEST_LIST();

                        req.STOCK_ID = stkdata.ID;
                        req.USER_ACCOUNT_ID = userId;

                        ReqList.Add(req);
                    }

                    // 反映準備
                    await add(ReqList);

                    //WK_STOCKのデータを削除する
                    await wkStockService.delete("20");

                    // 反映
                    await this.db.SaveChangesAsync();
                    
                }
                catch (Exception)
                {                   
                    var wk = new wkstock_Result();
                   
                    wk.NG_IMPORT_ERROR_MESSAGE = "既に作業依頼に追加されている在庫です。";
                    result.Add(wk);
                }
            }
            return result;
        }
    

        /// <summary>
        /// 作業依頼一覧に追加
        /// </summary>
        /// <param name="mwlStockIds"></param>
        /// <param name="userAccountId"></param>
        /// <returns></returns>
        public async Task<Result.DefaultExecutionResult> add(List<Int64> mwlStockIds, Int64 userAccountId)
		{
			async Task<Result.DefaultExecutionResult> task()
			{
				// -----------------------------------------
				// 作業依頼一覧に追加
				// -----------------------------------------
				var requests = new List<REQUEST_LIST>();
				foreach (Int64 mwlStockId in mwlStockIds)
				{
					var request = db.REQUEST_LISTs
						.Where(e => e.STOCK_ID == mwlStockId)
						.Where(e => e.USER_ACCOUNT_ID == userAccountId)
						.FirstOrDefault();

					if (null != request)
					{
						await setUpdateManagementInformation(request);
						db.REQUEST_LISTs.Update(request);
					}
					else
					{
						request = new REQUEST_LIST
						{
							STOCK_ID = mwlStockId,
							USER_ACCOUNT_ID = userAccountId
						};
						await setBothManagementInformation(request);
						db.REQUEST_LISTs.Add(request);
					}
				}

				// DBに登録
				db.SaveChanges();

				// -----------------------------------------
				// 在庫ステータスを更新
				// -----------------------------------------
				var stockService = new StockService(this.db, this.user, this.signInManager, this.userManager);
				var stocks = new List<STOCK>();
				foreach (Int64 mwlStockId in mwlStockIds)
				{
					var wstock = stockService.readWithUserItemByStockId(mwlStockId).Item1;
					if (null == wstock)
					{
						return Result.DefaultExecutionResult.makeError("在庫が存在しません。");
					}
					wstock.STATUS = DOMAIN.StockStatusCode.REQUEST;
				}
				await setUpdateManagementInformation(stocks);

				// DBに登録
				db.STOCKs.UpdateRange(stocks);
				db.SaveChanges();

				return new Result.DefaultExecutionResult
				{
					succeed = true
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
				return Result.DefaultExecutionResult.makeError(e.Message);
			}
		}


        /// <summary>
        /// 作業依頼一覧からデータを削除
        /// </summary>
        /// <param name="config">検索条件</param>
        /// <param name="deleteIdList"></param>
        public void delete(USER_ACCOUNT userAccount, List<long> deleteIdList, ViewTableType type)
        {
            foreach (var elm in deleteIdList)
            {
                // 作業依頼一覧テーブル
                var tmp = db.REQUEST_LISTs
                    .Where(e => e.USER_ACCOUNT_ID == userAccount.ID)
                    .Where(e => e.STOCK_ID == elm);
                this.db.REQUEST_LISTs.RemoveRange(tmp);

                // チェックボックス状態保持テーブル
                var tmp2 = db.WK_TABLE_SELECTION_SETTINGs
                    .Where(e => e.USER_ACCOUNT_ID == userAccount.ID)
                    .Where(e => e.viewTableType == type)
                    .Where(e => e.originalDataId == elm);
                this.db.WK_TABLE_SELECTION_SETTINGs.RemoveRange(tmp2);
            }

            this.db.SaveChanges();
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
        /// 統計情報の取得
        /// </summary>
        /// <returns></returns>
        public StatisticsInfo getStatistics(SearchConfig searchConfig)
        {
            return new StatisticsInfo
            {
                // 表示する一覧の全件数を返す
                numbers = getSearchQuery(searchConfig).Count()
            };
        }

        /// <summary>
        /// 指定された情報を読み取る
        /// </summary>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public IQueryable<REQUEST_LIST> read(ReadConfig readConfig, SearchConfig searchConfig, bool b)
        {
            // データ取得の内容
            var requestList = getSearchQuery(searchConfig);

            // ソートの内容
            requestList = setSortOrder(readConfig.sortOrder, readConfig.sortDirection, requestList);

            if (b == true)
            {
                return requestList;
            }
            else
            {
                // 必要な部分のみ抽出して返す
                return requestList
                    .Skip(readConfig.start)
                    .Take(readConfig.take);
            }
        }

        /// <summary>
        /// 一件のみ取得
        /// </summary>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public async Task<Result.ExecutionResult<REQUEST_LIST>> Read(long userId, long mwlStockId)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<REQUEST_LIST>((ret) =>
            {
                ret.result = db.REQUEST_LISTs
               // ユーザーID MWL在庫IDが一致
               .Where(e => e.USER_ACCOUNT_ID == userId).Where(e => e.STOCK_ID == mwlStockId)
               .FirstOrDefault();
            });
        }


        /// <summary>
        /// データ取得までの内容を取得
        /// </summary>
        /// <param name="searchConfig"></param>
        /// <returns></returns>
        private IQueryable<REQUEST_LIST> getSearchQuery(SearchConfig searchConfig)
        {
            // データ取得
            var requestList = db.REQUEST_LISTs
                // 指定したリレーション先のデータを取得してくる
                .Include(e => e.STOCK)
                .AsQueryable();

            // 新規入庫の場合は、ユーザーID単位ではなく、荷主単位の情報を表示する
            if (searchConfig.kind == "30")
            {
                requestList = requestList.Where(e => e.STOCK.SHIPPER_CODE == searchConfig.userAccount.CURRENT_SHIPPER_CODE);
            }
            else
            {
                requestList = requestList.Where(e => e.USER_ACCOUNT_ID == searchConfig.userAccount.ID);
            }

            // 取得するステータスを指定する
            if (searchConfig.searchStatus1 != null && searchConfig.seatchStatus2 != null)
            {
                requestList = requestList.Where(e => e.STOCK.STATUS == searchConfig.searchStatus1 || e.STOCK.STATUS == searchConfig.seatchStatus2);
            }
            else if (searchConfig.searchStatus1 != null)
            {
                requestList = requestList.Where(e => e.STOCK.STATUS == searchConfig.searchStatus1);
            }

            return requestList;
        }


        /// <summary>
        /// ソート項目の内容を設定する
        /// </summary>
        /// <param name="column"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        private IQueryable<REQUEST_LIST> setSortOrder(string column, SortDirection sortDirection, IQueryable<REQUEST_LIST> q)
        {
            #region ローカル関数

            //主条件でソートする
            IOrderedQueryable<REQUEST_LIST> LF_sortByPrimaryKey()
            {

                //デフォルトソート順序
                if (null == column)
                {
                    column = "REQUEST_STORAGE_MANAGE_NUMBER";
                }

                switch (column.Trim())
                {
                    // カラムの表示情報
                    case "REQUEST_STORAGE_MANAGE_NUMBER":
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

                    case "REQUEST_STATUS":
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

                    case "REQUEST_TITLE":
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

                    case "REQUEST_SUBTITLE":
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

                    case "REQUEST_NOTE":
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

                    case "REQUEST_SHIPPER_NOTE":
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

                    case "REQUEST_CUSTOMER_MANAGE_NUMBER":
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

                    case "REQUEST_PROCESSING_DATE":
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

                    case "REQUEST_SHAPE":
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

                    case "REQUEST_REMARK1":
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

                    case "REQUEST_REMARK2":
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

                    case "REQUEST_STOCK_COUNT":
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
        /// 作業依頼へ追加した後、在庫ワークの情報を削除する
        /// </summary>
        /// <param name="wkStockId"></param>
        /// <returns></returns>
        public async Task addDataAndDeleteWkStock(long wkStockId)
        {
            var user = await readLoggedUserInfo();

            // 在庫ワークIDから、在庫IDを取得
            var wkStock = this.db.WK_STOCKs
                .Include(e => e.STOCK)
                .FirstOrDefault(e => e.ID == wkStockId);

            // 取得した在庫IDを作業依頼へ入れる
            var requestList = new REQUEST_LIST();
            requestList.STOCK_ID = wkStock.STOCK_ID;
            requestList.USER_ACCOUNT_ID = user.ID;

            await this.setBothManagementInformation(requestList);

            this.db.REQUEST_LISTs.AddRange(requestList);

            // 在庫ワークから対象を削除(在庫ワークのデータのみ削除。在庫には残す）
            this.db.WK_STOCKs.RemoveRange(wkStock);

            await this.db.SaveChangesAsync();

        }
    }
}
