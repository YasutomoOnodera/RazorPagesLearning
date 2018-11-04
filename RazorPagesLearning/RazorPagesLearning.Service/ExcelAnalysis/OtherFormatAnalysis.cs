using RazorPagesLearning.Service.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RazorPagesLearning.Service.ExcelAnalysis
{
    public class OtherFormatAnalysis : InterfaceExcelAnalysis
    {

        public OtherFormatAnalysis(UserService userService, DB.StockService stockService)
        {
            this.userService = userService;
            this.StockService = stockService;
        }

        // サービス
        private RazorPagesLearning.Service.DB.StockService StockService;
        private Service.User.UserService userService;

        /// <summary>
        /// DBアクセスオブジェクト
        /// </summary>
        public readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;

        /// <summary>
        /// Excelデータ分析
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<wkstock_Result> Analysis(string fileName)
        {
            var result = new List<wkstock_Result>();

            // Excelファイルを開く
            using (var xlsxFile = File.OpenRead(fileName))
                try
                {
                    using (var package = new ExcelPackage(xlsxFile))
                    {
                        // ワークシートが1シートのみの場合
                        if (package.Workbook.Worksheets.Count == 1)
                        {
                            // エクセルシートの先頭のデータを取得する
                            ExcelWorksheet sheet = package.Workbook.Worksheets.First();

                            // 1列目の1行目、1列目の16行目を読み込む(項目名の部分)
                            var rlist = sheet.Cells[1, 1, 1, 16];

                            // キャスト 例外を出さない
                            Object[,] j = rlist.Value as Object[,];

                            // 1行目の項目が1列目は倉庫管理番号（必須） 16列目は荷主項目（登録後お客様編集可能項目WEBのみ反映) であるか
                            if (j[0, 0].ToString() == "倉庫管理番号（必須）" && j[0, 15].ToString() == "荷主項目（登録後お客様編集可能項目WEBのみ反映）")
                            {
                                // シートの終わりまで行と列を読み込む
                                // 最後の行数で取得できる為、終わりの行を確認する為には+1が必要。
                                for (int i = 2; i < sheet.Dimension.End.Row + 1; i++)
                                {
                                    // i列目の1行目からi列目の16行を読み込む
                                    var rrlist = sheet.Cells[i, 1, i, 16];

                                    Object[,] jj = rrlist.Value as Object[,];

                                    var wk = new wkstock_Result();

                                    // 取り込み行番号
                                    wk.INPORT_LINE_NUMBER = i;

                                    // エクセルの値を入れる
                                    wk.stock.STORAGE_MANAGE_NUMBER = jj[0, 0] as String;
 
                                    wk.stock.TITLE = jj[0, 8] as String;

                                    AddCheck(result, wk);
                                }
                            }
                            else if (j[0, 5].ToString() == "形状(必須）" && j[0, 8].ToString() == "題名（必須）")
                            {
                                // フォーマットが新規入庫フォーマットの場合
                                var wk = new wkstock_Result();
                                wk.NG_IMPORT_ERROR_MESSAGE = "選択した取り込みモードが違います。";
                                wk.IMPORT_ERROR_MESSAGE = "取り込みエラー";
                                result.Add(wk);
                                return result;
                            }
                            else
                            {
                                // フォーマットが違う場合
                                var wk = new wkstock_Result();
                                wk.NG_IMPORT_ERROR_MESSAGE = "取り込んだフォーマットが不正です。";
                                wk.IMPORT_ERROR_MESSAGE = "取り込みエラー";
                                result.Add(wk);
                                return result;
                            }
                        }
                        else if(package.Workbook.Worksheets.Count >= 2)
                        {
                            // ワークシートが2つ以上ある場合
                            var wk = new wkstock_Result();
                            wk.NG_IMPORT_ERROR_MESSAGE = "取り込んだフォーマットが不正です。";
                            wk.IMPORT_ERROR_MESSAGE = "取り込みエラー";
                            result.Add(wk);
                            return result;
                        }
                        else if (package.Workbook.Worksheets.Count == 0)
                        {
                            // ファイル形式が違う場合(textファイル、ファイル非選択)
                            var wk = new wkstock_Result();
                            wk.NG_IMPORT_ERROR_MESSAGE = "ファイルが選択されていないか、ファイル形式が不正です。";
                            wk.IMPORT_ERROR_MESSAGE = "取り込みエラー";
                            result.Add(wk);
                            return result;
                        }
                    }
                }
                catch (Exception)
                {
                    // ファイル形式が違う場合(textファイル、ファイル非選択以外)
                    var wk = new wkstock_Result();
                    wk.NG_IMPORT_ERROR_MESSAGE = "ファイルが選択されていないか、ファイル形式が不正です。";
                    wk.IMPORT_ERROR_MESSAGE = "取り込みエラー";
                    result.Add(wk);
                    return result;
                }

            return result;
        }
        private void AddCheck(List<wkstock_Result> result, wkstock_Result wk)
        {          
            
            // 1列全てががnullだった場合
            if (wk.stock.STORAGE_MANAGE_NUMBER == null && wk.stock.TITLE == null)
            {
                // リストに入れない
                result.Remove(wk);
            }         
            else
            {
                // 必須チェック
                if(wk.stock.STORAGE_MANAGE_NUMBER == null || wk.stock.TITLE == null)
                {
                        // 必須チェックに引っかかったら、notNull対策に空文字を追加
                        if (wk.stock.STORAGE_MANAGE_NUMBER == null)
                        {
                            wk.stock.STORAGE_MANAGE_NUMBER = "";
                        }
                        if (wk.stock.TITLE == null)
                        {
                            wk.stock.TITLE = "";
                        }
                        
                        wk.IMPORT_ERROR_MESSAGE = "必須項目が入力されていません。";
                }

                else
                {
                   // 倉庫管理番号 タイトルから在庫を取得                  
                   var stkResult = this.StockService.read(wk.stock.STORAGE_MANAGE_NUMBER, wk.stock.TITLE);

                    // 在庫の存在チェック
                    if (stkResult == null)
                    {
                        // 画面表示情報
                        wk.stock.STORAGE_MANAGE_NUMBER = wk.stock.STORAGE_MANAGE_NUMBER;
                        wk.stock.CUSTOMER_MANAGE_NUMBER = wk.stock.CUSTOMER_MANAGE_NUMBER;
                        wk.stock.TITLE = wk.stock.TITLE;
                        wk.IMPORT_ERROR_MESSAGE = "存在しない在庫です。";
                    }
                    // 在庫のステータスチェック[廃棄済],[抹消済],[依頼中] は取り込まない
                    else if (stkResult.result.STATUS == "40" || stkResult.result.STATUS == "50" || stkResult.result.STATUS == "60")
                    {
                        wk.stock.STORAGE_MANAGE_NUMBER = stkResult.result.STORAGE_MANAGE_NUMBER;
                        wk.stock.CUSTOMER_MANAGE_NUMBER = stkResult.result.CUSTOMER_MANAGE_NUMBER;
                        wk.stock.TITLE = stkResult.result.TITLE;
                        wk.IMPORT_ERROR_MESSAGE = "取り込み不可能の在庫です。";
                    }
                    
                    else
                    {
                        // 在庫データ追加
                        wk.stock.STOCK_ID = stkResult.result.ID;
                        wk.stock.STORAGE_MANAGE_NUMBER = stkResult.result.STORAGE_MANAGE_NUMBER;
                        wk.stock.CUSTOMER_MANAGE_NUMBER = stkResult.result.CUSTOMER_MANAGE_NUMBER;
                        wk.stock.TITLE = stkResult.result.TITLE;
                        wk.stock.SUBTITLE = stkResult.result.SUBTITLE;
                        wk.stock.DEPARTMENT_CODE = stkResult.result.DEPARTMENT_CODE;
                        wk.stock.SHAPE = stkResult.result.SHAPE;
                        wk.stock.CLASS1 = stkResult.result.CLASS1;
                        wk.stock.CLASS2 = stkResult.result.CLASS2;
                        wk.stock.REMARK1 = stkResult.result.REMARK1;
                        wk.stock.REMARK2 = stkResult.result.REMARK2;
                        wk.stock.NOTE = stkResult.result.NOTE;
                        wk.stock.SHIPPER_NOTE = stkResult.result.SHIPPER_NOTE;
                        wk.stock.PRODUCT_DATE = stkResult.result.PRODUCT_DATE;
                        wk.stock.SCRAP_SCHEDULE_DATE = stkResult.result.SCRAP_SCHEDULE_DATE;
                        wk.stock.TIME1 = stkResult.result.TIME1;
                        wk.stock.TIME2 = stkResult.result.TIME2;
 
                    }

                }

                // データをリストに入れる
                result.Add(wk);
            }
        }
    }
}
