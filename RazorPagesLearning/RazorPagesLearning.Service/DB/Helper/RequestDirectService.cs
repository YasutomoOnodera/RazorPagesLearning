using Microsoft.AspNetCore.Identity;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Service.ExcelAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
    public class RequestDirectService : DBServiceBase
    {
        public RequestDirectService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {

        }

        /// <summary>
        /// 再入庫ダイレクト バーコード読み取り
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public async Task<wkstock_Result> createDirect(string barcode, DB.StockService stockService, DB.WkStockService wkStockService, long userId)
        {
            var wk = new wkstock_Result();
            var wkstk = new Data.Models.WK_STOCK();
            var WkStockList = new List<WK_STOCK>();

            var stkdata = stockService.barcodeRead(barcode).result;

            if (barcode == null)
            {
                wk.NG_IMPORT_ERROR_MESSAGE = "バーコードが入力されていません。";
                return wk;
            }
            else
            {
                if (stkdata == null)
                {
                    // 入力したバーコードの在庫が無い場合
                    wk.NG_IMPORT_ERROR_MESSAGE = "存在しない在庫です。";
                    return wk;
                }
                else
                {
                    // 読み込みエラーチェック
                    // 在庫ステータスが出荷中(30)、抹消(50)、複数品(70)の場合は追加しない
                    if (stkdata.STATUS == "30" || stkdata.STATUS == "50" || stkdata.STATUS == "70")
                    {
                        if (stkdata.STATUS == "30")
                        {
                            wk.NG_IMPORT_ERROR_MESSAGE = "出荷中の在庫の為、追加できません。";
                        }
                        else if (stkdata.STATUS == "50")
                        {
                            wk.NG_IMPORT_ERROR_MESSAGE = "抹消済の在庫の為、追加できません。";
                        }
                        else
                        {
                            wk.NG_IMPORT_ERROR_MESSAGE = "複数品の在庫の為、追加できません。";
                        }
                        return wk;
                    }
                    else
                    {
                        // すでにWK_STOCKにある場合
                        var wkStockCheck = wkStockService.readDuplicationCheck(stkdata.ID, "30");

                        // すでに在庫ワークに同じデータがある場合は重複
                        if (wkStockCheck.result != null)
                        {
                            wk.NG_IMPORT_ERROR_MESSAGE = "入力したバーコードは既に一覧に追加されています。";
                            return wk;
                        }
                        else
                        {
                            // 在庫データ追加
                            wkstk.STOCK_ID = stkdata.ID;
                            wkstk.USER_ACOUNT_ID = userId;
                            wkstk.KIND = "30";
                            wkstk.STORAGE_MANAGE_NUMBER = stkdata.STORAGE_MANAGE_NUMBER;
                            wkstk.CUSTOMER_MANAGE_NUMBER = stkdata.CUSTOMER_MANAGE_NUMBER;
                            wkstk.TITLE = stkdata.TITLE;
                            wkstk.SUBTITLE = stkdata.SUBTITLE;
                            wkstk.DEPARTMENT_CODE = stkdata.DEPARTMENT_CODE;
                            wkstk.SHAPE = stkdata.SHAPE;
                            wkstk.CLASS1 = stkdata.CLASS1;
                            wkstk.CLASS2 = stkdata.CLASS2;
                            wkstk.REMARK1 = stkdata.REMARK1;
                            wkstk.REMARK2 = stkdata.REMARK2;
                            wkstk.NOTE = stkdata.NOTE;
                            wkstk.SHIPPER_NOTE = stkdata.SHIPPER_NOTE;
                            wkstk.TIME1 = stkdata.TIME1;
                            wkstk.TIME2 = stkdata.TIME2;

                            WkStockList.Add(wkstk);

                            // WK_STOCKに反映準備
                            await wkStockService.add(WkStockList);

                            // 反映
                            await wkStockService.db.SaveChangesAsync();

                        }
                    }
                }
            }
            return wk;
        }

        ///// <summary>
        ///// 再入庫ダイレクト 削除
        ///// </summary>
        ///// <param name="stockId"></param>
        ///// <returns></returns>
        //public async Task<wkstock_Result> deleteDirect(bool selected, DB.WkStockService wkStockService)
        //{
        //    var wkstk = new WK_STOCK();

        //    var result = new List<wkstock_Result>();
        //    var wk = new wkstock_Result();
        //    int checkCount = 0;

        //    //foreach (var wkStock in this.viewModel.val_WK_STOCKs)
        //    //{
        //    ////チェックされたデータ
        //    //if (wkStock.selected == true)
        //    //    {
        //    //        wkstk.MWL_STOCK_ID = wkStock.WkStockList.MWL_STOCK_ID;

        //    //        // 反映準備
        //    //        await wkStockService.delete(wkstk.MWL_STOCK_ID, "30");

        //    //        // 反映
        //    //        await wkStockService.db.SaveChangesAsync();

        //    //        checkCount++;
        //    //    }
        //    //}
        //    //if (checkCount == 0)
        //    //{
        //    //    wk.NG_IMPORT_ERROR_MESSAGE = "依頼が選択されていません。";
        //    //    result.Add(wk);
        //    //    this.viewModel.Result = result;
        //    //}


        //    return wk;
        //}
    }
}