using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Claims;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using static RazorPagesLearning.Service.DB.DepartmentAdminService;
using Microsoft.EntityFrameworkCore;
using RazorPagesLearning.Service.ExcelAnalysis;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// ワークテーブルサービス
    /// </summary>
    public class WkStockService : DBServiceBase
    {
        public WkStockService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {

        }

        /// <summary>
        /// 取得
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public async Task<Result.ExecutionResult<IQueryable<WK_STOCK>>> read(string kind)
        {
            return await RazorPagesLearning.Service.DB.Helper.ServiceHelper
                .doOperationWithErrorManagementAsync<IQueryable<WK_STOCK>>(async (ret) =>
            {
                var user = await readLoggedUserInfo();

                //取得したユーザーアカウントIDと等しいデータを取得
                ret.result = db.WK_STOCKs
                .Where(e => e.USER_ACOUNT_ID == user.ID).Where(e => e.KIND == kind);
                ret.succeed = true;

                return ret;
            });
        }

        // 取得(再入庫ダイレクト_重複バーコード確認)
        public Result.ExecutionResult<WK_STOCK> readDuplicationCheck(long stockId, string kind)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<WK_STOCK>(async (ret) =>
            {
                var user = await readLoggedUserInfo();

                ret.result = db.WK_STOCKs
                // 種別ダイレクト入庫のユーザーアカウントとSTOCK_IDが一致する在庫ワーク
                .Where(e => e.STOCK_ID == stockId)
                .Where(e => e.KIND == kind)
                .Where(e => e.USER_ACOUNT_ID == user.ID)
                .FirstOrDefault();
            });
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public async Task<Result.ExecutionResult<IQueryable<WK_STOCK>>> delete(string kind)
        {
            return await RazorPagesLearning.Service.DB.Helper.ServiceHelper
                .doOperationWithErrorManagementAsync<IQueryable<WK_STOCK>>(async (ret) =>
            {
                var user = await readLoggedUserInfo();

                var q = db.WK_STOCKs

                // ユーザーアカウントIDと処理種別で削除する
                .Where(e => e.USER_ACOUNT_ID == user.ID).Where(e => e.KIND == kind);

                db.WK_STOCKs.RemoveRange(q);

                return ret;
            });
        }

        /// <summary>
        /// 削除(ダイレクト入庫)
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public async Task<Result.ExecutionResult<IQueryable<WK_STOCK>>> delete(long stockId, string kind)
        {
            return await RazorPagesLearning.Service.DB.Helper.ServiceHelper
                .doOperationWithErrorManagementAsync<IQueryable<WK_STOCK>>(async (ret) =>
            {
                var user = await readLoggedUserInfo();

                var q = db.WK_STOCKs
                // ユーザーアカウントID、在庫ID、処理種別で削除する
                .Where(e => e.USER_ACOUNT_ID == user.ID).Where(e => e.STOCK_ID == stockId).Where(e => e.KIND == kind);

                db.WK_STOCKs.RemoveRange(q);

                return ret;
            });
        }
      
        /// <summary>
        /// 追加
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task add(List<WK_STOCK> list)
        {
            var user = await readLoggedUserInfo();
            foreach (var ele in list)
            {
                ele.USER_ACOUNT_ID = user.ID;
            }

            await this.setBothManagementInformation(list);
            this.db.WK_STOCKs.AddRange(list);

        }

        /// <summary>
        /// 追加(データ取り込み 新規入庫)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<List<WK_STOCK>> addNewWkstock(List<wkstock_Result> formatResult, long userId)
        {
            var wkStockList = new List<WK_STOCK>();

            foreach (var wkStock in formatResult)
            {
                var wkstk = new WK_STOCK();

                // 解析結果をWK_STOCKへ入れる            
                if (wkStock.stock.STORAGE_MANAGE_NUMBER == null)
                {
                    
                    wkstk.STORAGE_MANAGE_NUMBER = "";
                }
                else
                {
                    wkstk.STORAGE_MANAGE_NUMBER = wkStock.stock.STORAGE_MANAGE_NUMBER;
                }

                wkstk.CUSTOMER_MANAGE_NUMBER = wkStock.stock.CUSTOMER_MANAGE_NUMBER;
                wkstk.CLASS1 = wkStock.stock.CLASS1;
                wkstk.CLASS2 = wkStock.stock.CLASS2;
                wkstk.SHAPE = wkStock.stock.SHAPE;
                wkstk.TIME1 = wkStock.stock.TIME1;
                wkstk.TIME2 = wkStock.stock.TIME2;
                
                if (wkStock.stock.TITLE == null)
                {
                    wkstk.TITLE = "";
                }
                else
                {
                    wkstk.TITLE = wkStock.stock.TITLE;
                }
                wkstk.SUBTITLE = wkStock.stock.SUBTITLE;
                wkstk.REMARK1 = wkStock.stock.REMARK1;
                wkstk.REMARK2 = wkStock.stock.REMARK2;

                
                if (wkStock.stock.SCRAP_SCHEDULE_DATE.HasValue == true)
                {
                    wkstk.SCRAP_SCHEDULE_DATE = wkStock.stock.SCRAP_SCHEDULE_DATE.Value;
                }
                else
                {
                    // null非許容
                    //wkstk.SCRAP_SCHEDULE_DATE = new DateTimeOffset(1111, 1, 1, 1, 1, 1, new TimeSpan(9, 0, 0));
                    wkstk.SCRAP_SCHEDULE_DATE = null;
                }
                
                wkstk.PRODUCT_DATE = wkStock.stock.PRODUCT_DATE;
                wkstk.NOTE = wkStock.stock.NOTE;
                wkstk.SHIPPER_NOTE = wkStock.stock.SHIPPER_NOTE;
                wkstk.IMPORT_ERROR_MESSAGE = wkStock.IMPORT_ERROR_MESSAGE;
                wkstk.IMPORT_LINE_NUMBER = wkStock.INPORT_LINE_NUMBER;

                // not null対策で空文字挿入
                if (wkStock.stock.DEPARTMENT_CODE == null)
                {
                    wkstk.DEPARTMENT_CODE = "";
                }
                else
                {
                    wkstk.DEPARTMENT_CODE = wkStock.stock.DEPARTMENT_CODE;
                }

                /* ドメインの中の
                  DOMAIN(KIND=00010010)
                    1:新規登録
                    2:データ取込
                    3:再入庫ダイレクト*/
                wkstk.KIND = "20";
                wkstk.USER_ACOUNT_ID = userId;

                wkStockList.Add(wkstk);
            }
            // WK_STOCKに反映準備
            await add(wkStockList);

            // 反映
            await this.db.SaveChangesAsync();

            return wkStockList;
        }

        /// <summary>
        /// 追加(データ取り込み その他依頼)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<List<WK_STOCK>> addOtherWkstock(List<wkstock_Result> formatResult, long userId)
        {

            var wkStockList = new List<WK_STOCK>();

            foreach (var wkStock in formatResult)
            {
                var wkstk = new WK_STOCK();

                // 解析結果をWK_STOCKへ入れる
                wkstk.STOCK_ID = wkStock.stock.STOCK_ID;
                wkstk.USER_ACOUNT_ID = userId;
                wkstk.KIND = "20";
                if (wkStock.stock.STORAGE_MANAGE_NUMBER == null)
                {
                    wkstk.STORAGE_MANAGE_NUMBER = "";
                }
                else
                {
                    wkstk.STORAGE_MANAGE_NUMBER = wkStock.stock.STORAGE_MANAGE_NUMBER;
                }

                if (wkStock.stock.CUSTOMER_MANAGE_NUMBER == null)
                {
                    wkstk.CUSTOMER_MANAGE_NUMBER = "";
                }
                else
                {
                    wkstk.CUSTOMER_MANAGE_NUMBER = wkStock.stock.CUSTOMER_MANAGE_NUMBER;
                }

                if (wkStock.stock.TITLE == null)
                {
                    wkstk.TITLE = "";
                }
                else
                {
                    wkstk.TITLE = wkStock.stock.TITLE;
                }

                wkstk.SUBTITLE = wkStock.stock.SUBTITLE;

                if (wkStock.stock.DEPARTMENT_CODE == null)
                {
                    wkstk.DEPARTMENT_CODE = "";
                }
                else
                {
                    wkstk.DEPARTMENT_CODE = wkStock.stock.DEPARTMENT_CODE;
                }

                wkstk.SHAPE = wkStock.stock.SHAPE;
                wkstk.CLASS1 = wkStock.stock.CLASS1;
                wkstk.CLASS2 = wkStock.stock.CLASS2;
                wkstk.REMARK1 = wkStock.stock.REMARK1;
                wkstk.REMARK2 = wkStock.stock.REMARK2;
                wkstk.NOTE = wkStock.stock.NOTE;
                wkstk.SHIPPER_NOTE = wkStock.stock.SHIPPER_NOTE;
                wkstk.PRODUCT_DATE = wkStock.stock.PRODUCT_DATE;
                wkstk.SCRAP_SCHEDULE_DATE = wkStock.stock.SCRAP_SCHEDULE_DATE;
                wkstk.TIME1 = wkStock.stock.TIME1;
                wkstk.TIME2 = wkStock.stock.TIME2;
                wkstk.IMPORT_LINE_NUMBER = wkStock.INPORT_LINE_NUMBER;       
                wkstk.IMPORT_ERROR_MESSAGE = wkStock.IMPORT_ERROR_MESSAGE;

                wkStockList.Add(wkstk);
            }

            // WK_STOCKに反映準備
            await add(wkStockList);

            // 反映
            await this.db.SaveChangesAsync();

            return wkStockList;
        }

            /// <summary>
            /// データを追加する(在庫ワークと同時に在庫にもデータを追加）
            /// </summary>
            /// <param name="data"></param>
            /// <param name="kind"></param>
            /// <param name="status"></param>
            /// <returns></returns>
            public async Task add(WK_STOCK data, string kind, string status)
        {
            var user = await readLoggedUserInfo();

            // 新規登録
            WK_STOCK wkStock = new WK_STOCK();

            wkStock.USER_ACOUNT_ID = user.ID;
            wkStock.KIND = kind;
            wkStock.STORAGE_MANAGE_NUMBER = "";     // nullでは追加できないので、空で追加する
            wkStock.CUSTOMER_MANAGE_NUMBER = data.CUSTOMER_MANAGE_NUMBER;
            wkStock.TITLE = data.TITLE;
            wkStock.SUBTITLE = data.SUBTITLE;
            wkStock.DEPARTMENT_CODE = data.DEPARTMENT_CODE;
            wkStock.SHAPE = data.SHAPE;
            wkStock.CLASS1 = data.CLASS1;
            wkStock.CLASS2 = data.CLASS2;
            wkStock.REMARK1 = data.REMARK1;
            wkStock.REMARK2 = data.REMARK2;
            wkStock.NOTE = data.NOTE;
            wkStock.SHIPPER_NOTE = data.SHIPPER_NOTE;
            wkStock.PRODUCT_DATE = data.PRODUCT_DATE;
            wkStock.SCRAP_SCHEDULE_DATE = data.SCRAP_SCHEDULE_DATE;
            wkStock.TIME1 = data.TIME1;
            wkStock.TIME2 = data.TIME2;

            await this.setBothManagementInformation(wkStock);

            wkStock.STOCK = new STOCK();

            wkStock.STOCK.STORAGE_MANAGE_NUMBER = data.STORAGE_MANAGE_NUMBER;
            wkStock.STOCK.STATUS = status;
            wkStock.STOCK.CUSTOMER_MANAGE_NUMBER = data.CUSTOMER_MANAGE_NUMBER;
            wkStock.STOCK.TITLE = data.TITLE;
            wkStock.STOCK.SUBTITLE = data.SUBTITLE;
            //wkStock.STOCK.SHIPPER_CODE = 登録したユーザーの荷主コードを入れる。ただし、管理者、作業者の場合は、選択している荷主の情報を入れる必要がある
            wkStock.STOCK.DEPARTMENT_CODE = data.DEPARTMENT_CODE;
            wkStock.STOCK.SHAPE = data.SHAPE;
            wkStock.STOCK.CLASS1 = data.CLASS1;
            wkStock.STOCK.CLASS2 = data.CLASS2;
            wkStock.STOCK.REMARK1 = data.REMARK1;
            wkStock.STOCK.REMARK2 = data.REMARK2;
            wkStock.STOCK.NOTE = data.NOTE;
            wkStock.STOCK.SHIPPER_NOTE = data.SHIPPER_NOTE;
            wkStock.STOCK.PRODUCT_DATE = data.PRODUCT_DATE;
            wkStock.STOCK.SCRAP_SCHEDULE_DATE = data.SCRAP_SCHEDULE_DATE;
            wkStock.STOCK.TIME1 = data.TIME1;
            wkStock.STOCK.TIME2 = data.TIME2;

            await this.setBothManagementInformation(wkStock.STOCK);

            this.db.WK_STOCKs.Add(wkStock);

            await this.db.SaveChangesAsync();
        }

        /// <summary>
        /// データを更新する(在庫ワークと同時に在庫もデータを更新）
        /// </summary>
        /// <param name="wkStockId"></param>
        /// <param name="data"></param>
        /// <param name="kind"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task update(long wkStockId, WK_STOCK data, string kind, string status)
        {
            var user = await readLoggedUserInfo();

            var wkStock = this.db.WK_STOCKs
                .Include(e => e.STOCK)
                .Where(e => e.USER_ACOUNT_ID == user.ID)
                .Where(e => e.KIND == kind)
                .Where(e => e.ID == wkStockId).FirstOrDefault();

            wkStock.USER_ACOUNT_ID = user.ID;
            wkStock.KIND = kind;
            wkStock.STORAGE_MANAGE_NUMBER = "";     // nullでは追加できないので、空で追加する
            wkStock.CUSTOMER_MANAGE_NUMBER = data.CUSTOMER_MANAGE_NUMBER;
            wkStock.TITLE = data.TITLE;
            wkStock.SUBTITLE = data.SUBTITLE;
            wkStock.DEPARTMENT_CODE = data.DEPARTMENT_CODE;
            wkStock.SHAPE = data.SHAPE;
            wkStock.CLASS1 = data.CLASS1;
            wkStock.CLASS2 = data.CLASS2;
            wkStock.REMARK1 = data.REMARK1;
            wkStock.REMARK2 = data.REMARK2;
            wkStock.NOTE = data.NOTE;
            wkStock.SHIPPER_NOTE = data.SHIPPER_NOTE;
            wkStock.PRODUCT_DATE = data.PRODUCT_DATE;
            wkStock.SCRAP_SCHEDULE_DATE = data.SCRAP_SCHEDULE_DATE;
            wkStock.TIME1 = data.TIME1;
            wkStock.TIME2 = data.TIME2;

            wkStock.STOCK.STORAGE_MANAGE_NUMBER = data.STORAGE_MANAGE_NUMBER;
            wkStock.STOCK.STATUS = status;
            wkStock.STOCK.CUSTOMER_MANAGE_NUMBER = data.CUSTOMER_MANAGE_NUMBER;
            wkStock.STOCK.TITLE = data.TITLE;
            wkStock.STOCK.SUBTITLE = data.SUBTITLE;
            //wkStock.STOCK.SHIPPER_CODE = 登録したユーザーの荷主コードを入れる。ただし、管理者、作業者の場合は、選択している荷主の情報を入れる必要がある
            wkStock.STOCK.DEPARTMENT_CODE = data.DEPARTMENT_CODE;
            wkStock.STOCK.SHAPE = data.SHAPE;
            wkStock.STOCK.CLASS1 = data.CLASS1;
            wkStock.STOCK.CLASS2 = data.CLASS2;
            wkStock.STOCK.REMARK1 = data.REMARK1;
            wkStock.STOCK.REMARK2 = data.REMARK2;
            wkStock.STOCK.NOTE = data.NOTE;
            wkStock.STOCK.SHIPPER_NOTE = data.SHIPPER_NOTE;
            wkStock.STOCK.PRODUCT_DATE = data.PRODUCT_DATE;
            wkStock.STOCK.SCRAP_SCHEDULE_DATE = data.SCRAP_SCHEDULE_DATE;
            wkStock.STOCK.TIME1 = data.TIME1;
            wkStock.STOCK.TIME2 = data.TIME2;

            await setUpdateManagementInformation(wkStock);

            await this.db.SaveChangesAsync();
        }

        /// <summary>
        /// データを削除する(在庫ワークと同時に在庫のデータも削除）
        /// </summary>
        /// <param name="wkStockId"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public async Task deleteWkStockAndStock(long wkStockId, string kind)
        {
            var user = await readLoggedUserInfo();

            // 削除対象を取得
            var wkStock = this.db.WK_STOCKs
                .Include(e => e.STOCK)
                .Where(e => e.USER_ACOUNT_ID == user.ID)
                .Where(e => e.KIND == kind)
                .Where(e => e.ID == wkStockId).FirstOrDefault();

            this.db.STOCKs.RemoveRange(wkStock.STOCK);
            this.db.WK_STOCKs.RemoveRange(wkStock);

            await this.db.SaveChangesAsync();
        }

    }
}
