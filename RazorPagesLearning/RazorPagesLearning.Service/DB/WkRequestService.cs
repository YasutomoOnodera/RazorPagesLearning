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

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// 作業依頼ワークサービス
    /// </summary>
    public class WkRequestService : DBServiceBase
    {
        public WkRequestService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }

        /// <summary>
        /// 追加データの設定
        /// </summary>
        public class AddConfig
        {
            /// <summary>
            /// ユーザーアカウントID
            /// </summary>
            public long userAccountId { get; set; }

            /// <summary>
            /// 依頼内容
            /// </summary>
            public string requestKind { get; set; }

            /// <summary>
            /// 依頼数合計
            /// </summary>
            public int requestCountSum { get; set; }

            /// <summary>
            /// 明細数
            /// </summary>
            public int detailCount { get; set; }

            /// <summary>
            /// 詳細データ
            /// </summary>
            public List<WkRequestDetailService.AddConfig> detailConfig { get; set; }
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
                this.sortOrder = "REQUEST_DELIVERY_STORAGE_MANAGE_NUMBER";
            }
        }


        /// <summary>
        /// 取得
        /// </summary>
        /// <returns></returns>
        public Result.ExecutionResult<WK_REQUEST> read(long userId, long wkRequestId)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<WK_REQUEST>((ret) =>
            {
                ret.result = db.WK_REQUESTs
                   .Include(e => e.WK_REQUEST_DETAILs)
                        .ThenInclude(e => e.STOCK)
                   .Include(e => e.WK_REQUEST_DELIVERY)
                        .ThenInclude(e => e.DELIVERY_ADMIN)
                   .Where(e => e.USER_ACCOUNT_ID == userId).Where(e => e.ID == wkRequestId)
                   .FirstOrDefault();
                ret.succeed = true;
            });
        }

        /// <summary>
        /// 取得
        /// 1ユーザー、1作業依頼なので、1つしかデータは取れない
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="wkRequestId"></param>
        /// <returns></returns>
        public Result.ExecutionResult<WK_REQUEST> read(long userId)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<WK_REQUEST>((ret) =>
            {
                ret.result = db.WK_REQUESTs
                   .Include(e => e.WK_REQUEST_DETAILs)
                   .Where(e => e.USER_ACCOUNT_ID == userId)
                   .FirstOrDefault();
            });
        }


        /// <summary>
        /// 作業依頼ワーク情報の追加
        /// </summary>
        /// <param name="addConfig"></param>
        /// <returns></returns>
        public async Task<long> add(AddConfig addConfig)
        {
            WK_REQUEST wkRequest = new WK_REQUEST();
            wkRequest.USER_ACCOUNT_ID = addConfig.userAccountId;
            wkRequest.REQUEST_KIND = addConfig.requestKind;
            wkRequest.REQUEST_COUNT_SUM = addConfig.requestCountSum;
            wkRequest.DETAIL_COUNT = addConfig.detailCount;

            wkRequest.WK_REQUEST_DETAILs = new List<WK_REQUEST_DETAIL>();
            foreach (var elm in addConfig.detailConfig)
            {
                WK_REQUEST_DETAIL wkRequestDetail = new WK_REQUEST_DETAIL();
                wkRequestDetail.STOCK_ID = elm.mwlStockId;
                wkRequestDetail.REQUEST_COUNT = elm.requestCount;

                await setCreateManagementInformation(wkRequestDetail);
                await setUpdateManagementInformation(wkRequestDetail);

                wkRequest.WK_REQUEST_DETAILs.Add(wkRequestDetail);
            }

            await setCreateManagementInformation(wkRequest);
            await setUpdateManagementInformation(wkRequest);

            db.WK_REQUESTs.Add(wkRequest);

            // IDが欲しいので、一度保存
            await db.SaveChangesAsync();

            return wkRequest.ID;
        }

        /// <summary>
        /// 作業依頼情報の削除
        /// 同時に、関連する作業依頼詳細、集配先ワークも削除する
        /// </summary>
        /// <param name="requestId"></param>
        public void delete(long requestId)
        {
            var target = this.db.WK_REQUESTs.Where(e => e.ID == requestId)
                .Include(e => e.WK_REQUEST_DETAILs)
                .Include(e => e.WK_REQUEST_DELIVERY)
                .AsQueryable();

            // これで関連テーブルの情報も消える
            this.db.WK_REQUESTs.RemoveRange(target);
            this.db.SaveChanges();
        }

        /// <summary>
        /// 作業依頼ワーク、作業依頼詳細ワークに追加
        /// </summary>
        /// <param name="WK_REQUEST_DETAILs"></param>
        /// <param name="userAccountId"></param>
        /// <returns></returns>
        public async Task<Result.DefaultExecutionResult> add(List<WK_REQUEST_DETAIL> WK_REQUEST_DETAILs, Int64 userAccountId)
        {
            async Task<Result.DefaultExecutionResult> task()
            {
                // -----------------------------------------
                // 作業依頼ワークに追加
                // -----------------------------------------
                var request = new WK_REQUEST();

                request.USER_ACCOUNT_ID = userAccountId;
                request.REQUEST_KIND = "70";
                request.REQUEST_COUNT_SUM = WK_REQUEST_DETAILs.Select(e => e.REQUEST_COUNT).Sum();
                request.DETAIL_COUNT = WK_REQUEST_DETAILs.Count();

                await setBothManagementInformation(request);

                // DBに登録
                db.WK_REQUESTs.Add(request);
                db.SaveChanges();

                // -----------------------------------------
                // 作業依頼詳細ワークに追加
                // -----------------------------------------
                var requestDetails = new List<WK_REQUEST_DETAIL>();
                foreach (var WK_REQUEST_DETAIL in WK_REQUEST_DETAILs)
                {
                    requestDetails.Add(new WK_REQUEST_DETAIL
                    {
                        WK_REQUEST_ID = request.ID,
                        STOCK_ID = WK_REQUEST_DETAIL.STOCK_ID,
                        REQUEST_COUNT = WK_REQUEST_DETAIL.REQUEST_COUNT
                    });
                }

                await setBothManagementInformation(requestDetails);

                // DBに登録
                db.WK_REQUEST_DETAILs.AddRange(requestDetails);
                db.SaveChanges();

                return new Result.DefaultExecutionResult
                {
                    succeed = true,
					result = request.ID
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
        public StatisticsInfo getStatistics(long wkRequestId)
        {
            var tmp = getSearchQuery(wkRequestId);
            int count = tmp.FirstOrDefault().WK_REQUEST_DETAILs.Count();

            return new StatisticsInfo
            {
                // 表示する一覧の全件数を返す
                numbers = count
            };
        }

        /// <summary>
        /// 指定された情報を読み取る
        /// </summary>
        /// <param name="readConfig">読み取り設定</param>
        /// <param name="wkRequestId">作業依頼ワークID</param>
        /// <param name="allGetOption">対象データをそのまま返す</param>
        /// <returns></returns>
        public IQueryable<WK_REQUEST> read(ReadConfig readConfig, long wkRequestId, bool allGetOption)
        {
            // データ取得の内容
            var wkRequestList = getSearchQuery(wkRequestId);

            // ソートの内容
            wkRequestList = setSortOrder(readConfig.sortOrder, readConfig.sortDirection, wkRequestList);

            if (allGetOption == true)
            {
                return wkRequestList;
            }
            else
            {
                // 必要な部分のみ抽出して返す
                return wkRequestList
                    .Skip(readConfig.start)
                    .Take(readConfig.take);
            }
        }

        /// <summary>
        /// データ取得までの内容を取得
        /// </summary>
        /// <param name="searchConfig"></param>
        /// <returns></returns>
        private IQueryable<WK_REQUEST> getSearchQuery(long wkRequestId)
        {
            // データ取得の内容
            var wkRequestList = db.WK_REQUESTs
                // 指定したリレーション先のデータを取得してくる
                .Include(e => e.WK_REQUEST_DETAILs)
                    .ThenInclude(e => e.STOCK)
                .Include(e => e.WK_REQUEST_DELIVERY)
                    .ThenInclude(e => e.DELIVERY_ADMIN)
                .Where(e => e.ID == wkRequestId)
                .AsQueryable();

            return wkRequestList;
        }

        /// <summary>
        /// ソート項目の内容を設定する
        /// </summary>
        /// <param name="column"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        private IQueryable<WK_REQUEST> setSortOrder(string column, SortDirection sortDirection, IQueryable<WK_REQUEST> q)
        {
            #region ローカル関数

            //主条件でソートする
            IOrderedQueryable<WK_REQUEST> LF_sortByPrimaryKey()
            {

                //デフォルトソート順序
                if (null == column)
                {
                    column = "REQUEST_DELIVERY_STORAGE_MANAGE_NUMBER";
                }

                //debug
                column = "REQUEST_DELIVERY_STORAGE_MANAGE_NUMBER";
                switch (column.Trim())
                {
                    // カラムの表示情報
                    case "REQUEST_DELIVERY_STORAGE_MANAGE_NUMBER":
                        {
                            switch (sortDirection)
                            {
                                case SortDirection.ASC:
                                    {
                                        return q.OrderBy(e => e.WK_REQUEST_DETAILs.FirstOrDefault().STOCK.STORAGE_MANAGE_NUMBER);
                                    }
                                case SortDirection.DES:
                                    {
                                        return q.OrderByDescending(e => e.WK_REQUEST_DETAILs.FirstOrDefault().STOCK.STORAGE_MANAGE_NUMBER);
                                    }
                            }
                            break;
                        }

                        //case "REQUEST_DELIVERY_STATUS":
                        //    {
                        //        switch (sortDirection)
                        //        {
                        //            case SortDirection.ASC:
                        //                {
                        //                    return q.OrderBy(e => e.STOCK.STATUS);
                        //                }
                        //            case SortDirection.DES:
                        //                {
                        //                    return q.OrderByDescending(e => e.STOCK.STATUS);
                        //                }
                        //        }
                        //        break;
                        //    }

                        //case "REQUEST_DELIVERY_TITLE":
                        //    {
                        //        switch (sortDirection)
                        //        {
                        //            case SortDirection.ASC:
                        //                {
                        //                    return q.OrderBy(e => e.STOCK.TITLE);
                        //                }
                        //            case SortDirection.DES:
                        //                {
                        //                    return q.OrderByDescending(e => e.STOCK.TITLE);
                        //                }
                        //        }
                        //        break;
                        //    }

                        //case "REQUEST_DELIVERY_SUBTITLE":
                        //    {
                        //        switch (sortDirection)
                        //        {
                        //            case SortDirection.ASC:
                        //                {
                        //                    return q.OrderBy(e => e.STOCK.SUBTITLE);
                        //                }
                        //            case SortDirection.DES:
                        //                {
                        //                    return q.OrderByDescending(e => e.STOCK.SUBTITLE);
                        //                }
                        //        }
                        //        break;
                        //    }

                        //case "REQUEST_DELIVERY_NOTE":
                        //    {
                        //        switch (sortDirection)
                        //        {
                        //            case SortDirection.ASC:
                        //                {
                        //                    return q.OrderBy(e => e.STOCK.NOTE);
                        //                }
                        //            case SortDirection.DES:
                        //                {
                        //                    return q.OrderByDescending(e => e.STOCK.NOTE);
                        //                }
                        //        }
                        //        break;
                        //    }

                        //case "REQUEST_DELIVERY_SHIPPER_NOTE":
                        //    {
                        //        switch (sortDirection)
                        //        {
                        //            case SortDirection.ASC:
                        //                {
                        //                    return q.OrderBy(e => e.STOCK.SHIPPER_NOTE);
                        //                }
                        //            case SortDirection.DES:
                        //                {
                        //                    return q.OrderByDescending(e => e.STOCK.SHIPPER_NOTE);
                        //                }
                        //        }
                        //        break;
                        //    }

                        //case "REQUEST_DELIVERY_CUSTOMER_MANAGE_NUMBER":
                        //    {
                        //        switch (sortDirection)
                        //        {
                        //            case SortDirection.ASC:
                        //                {
                        //                    return q.OrderBy(e => e.STOCK.CUSTOMER_MANAGE_NUMBER);
                        //                }
                        //            case SortDirection.DES:
                        //                {
                        //                    return q.OrderByDescending(e => e.STOCK.CUSTOMER_MANAGE_NUMBER);
                        //                }
                        //        }
                        //        break;
                        //    }

                        //case "REQUEST_DELIVERY_PROCESSING_DATE":
                        //    {
                        //        switch (sortDirection)
                        //        {
                        //            case SortDirection.ASC:
                        //                {
                        //                    return q.OrderBy(e => e.STOCK.PROCESSING_DATE);
                        //                }
                        //            case SortDirection.DES:
                        //                {
                        //                    return q.OrderByDescending(e => e.STOCK.PROCESSING_DATE);
                        //                }
                        //        }
                        //        break;
                        //    }

                        //case "REQUEST_DELIVERY_SHAPE":
                        //    {
                        //        switch (sortDirection)
                        //        {
                        //            case SortDirection.ASC:
                        //                {
                        //                    return q.OrderBy(e => e.STOCK.SHAPE);
                        //                }
                        //            case SortDirection.DES:
                        //                {
                        //                    return q.OrderByDescending(e => e.STOCK.SHAPE);
                        //                }
                        //        }
                        //        break;
                        //    }

                        //case "REQUEST_DELIVERY_REMARK1":
                        //    {
                        //        switch (sortDirection)
                        //        {
                        //            case SortDirection.ASC:
                        //                {
                        //                    return q.OrderBy(e => e.STOCK.REMARK1);
                        //                }
                        //            case SortDirection.DES:
                        //                {
                        //                    return q.OrderByDescending(e => e.STOCK.REMARK1);
                        //                }
                        //        }
                        //        break;
                        //    }

                        //case "REQUEST_DELIVERY_REMARK2":
                        //    {
                        //        switch (sortDirection)
                        //        {
                        //            case SortDirection.ASC:
                        //                {
                        //                    return q.OrderBy(e => e.STOCK.REMARK2);
                        //                }
                        //            case SortDirection.DES:
                        //                {
                        //                    return q.OrderByDescending(e => e.STOCK.REMARK2);
                        //                }
                        //        }
                        //        break;
                        //    }

                        //case "REQUEST_DELIVERY_STOCK_COUNT":
                        //    {
                        //        switch (sortDirection)
                        //        {
                        //            case SortDirection.ASC:
                        //                {
                        //                    return q.OrderBy(e => e.STOCK.STOCK_COUNT);
                        //                }
                        //            case SortDirection.DES:
                        //                {
                        //                    return q.OrderByDescending(e => e.STOCK.STOCK_COUNT);
                        //                }
                        //        }
                        //        break;
                        //    }

                        //case "REQUEST_DELIVERY_REQUEST_COUNT":
                        //    {
                        //        switch (sortDirection)
                        //        {
                        //            case SortDirection.ASC:
                        //                {
                        //                    return q.OrderBy(e => e.STOCK.STOCK_COUNT);
                        //                }
                        //            case SortDirection.DES:
                        //                {
                        //                    return q.OrderByDescending(e => e.wo);
                        //                }
                        //        }
                        //        break;
                        //    }

                }
                throw new ApplicationException("想定外のソート条件です。");
            }

            #endregion

            return LF_sortByPrimaryKey().ThenBy(e => e.ID);
        }
    }

}