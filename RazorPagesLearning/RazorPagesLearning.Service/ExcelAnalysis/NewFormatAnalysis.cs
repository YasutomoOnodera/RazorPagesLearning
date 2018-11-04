using RazorPagesLearning.Data.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.ExcelAnalysis
{
    /// <summary>
    /// 新規入庫フォーマット取り込み
    /// </summary>
    public class NewFormatAnalysis : InterfaceExcelAnalysis
    {
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

                            // 1行目の項目が1列目は倉庫管理番号 16列目は荷主項目（登録後お客様編集可能項目WEBのみ反映) であるか
                            if (j[0, 0].ToString() == "倉庫管理番号" && j[0, 15].ToString() == "荷主項目（登録後お客様編集可能項目WEBのみ反映）")
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

                                    wk.stock.CUSTOMER_MANAGE_NUMBER = jj[0, 1] as String;

                                    wk.stock.CLASS1 = jj[0, 2] as String;

                                    wk.stock.CLASS2 = jj[0, 3] as String;

                                    wk.stock.DEPARTMENT_CODE = jj[0, 4] as String;

                                    wk.stock.SHAPE = jj[0, 5] as String;

                                    wk.stock.TIME1 = jj[0, 6] as String;

                                    wk.stock.TIME2 = jj[0, 7] as String;

                                    wk.stock.TITLE = jj[0, 8] as String;

                                    wk.stock.SUBTITLE = jj[0, 9] as String;

                                    wk.stock.REMARK1 = jj[0, 10] as String;

                                    wk.stock.REMARK2 = jj[0, 11] as String;

                                    //null判定
                                    if (jj[0, 12] == null)
                                    {
                                        wk.stock.SCRAP_SCHEDULE_DATE = null;
                                    }
                                    else
                                    {
                                        wk.stock.SCRAP_SCHEDULE_DATE = DateTimeOffset.Parse(jj[0, 12] as String);
                                    }

                                    // null判定
                                    if (jj[0, 13] == null)
                                    {
                                        wk.stock.PRODUCT_DATE = null;
                                    }
                                    else
                                    {
                                        wk.stock.PRODUCT_DATE = jj[0, 13] as String;
                                    }

                                    wk.stock.NOTE = jj[0, 14] as String;

                                    wk.stock.SHIPPER_NOTE = jj[0, 15] as String;

                                    AddCheck(result, wk);
                                }
                            }
                            else if (j[0, 0].ToString() == "倉庫管理番号（必須）" && j[0, 8].ToString() == "題名（必須）")
                            {
                                // フォーマットがその他依頼フォーマットの場合
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
                        else if (package.Workbook.Worksheets.Count >= 2)
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
            var Wk = wk.stock;

            // 取り込み行が全てがnullだった場合
            if (Wk.CUSTOMER_MANAGE_NUMBER == null && Wk.CLASS1 == null &&
                Wk.CLASS2 == null && Wk.DEPARTMENT_CODE == null && Wk.SHAPE == null &&
                Wk.TIME1 == null && Wk.TIME2 == null && Wk.TITLE == null && Wk.SUBTITLE == null && Wk.REMARK1 == null &&
                Wk.REMARK2 == null && Wk.SCRAP_SCHEDULE_DATE == null && Wk.PRODUCT_DATE == null &&
                Wk.NOTE == null && Wk.SHIPPER_NOTE == null
                )
            {
                result.Remove(wk);
            }
            // 必須項目に値が無い場合
            else if (Wk.SHAPE == null || Wk.TITLE == null)
            {
                if(Wk.SHAPE == null)
                {
                    wk.IMPORT_ERROR_MESSAGE = "「形状」に値が入力されていません。";
                }
                else
                {
                    wk.IMPORT_ERROR_MESSAGE = "「題名」に値が入力されていません。";
                }
                result.Add(wk);
            }
            // 正しいデータ
            else
            {
                // フォーマット内重複チェック
                for (int i = 0; i < result.Count(); i++)
                {
                    foreach (var resultWkStock in result.ToList())
                    {
                        if (resultWkStock.stock.TITLE == wk.stock.TITLE &&
                            resultWkStock.stock.SHAPE == wk.stock.SHAPE &&
                            resultWkStock.stock.CUSTOMER_MANAGE_NUMBER == wk.stock.CUSTOMER_MANAGE_NUMBER &&
                            resultWkStock.stock.CLASS1 == wk.stock.CLASS1 &&
                            resultWkStock.stock.CLASS2 == wk.stock.CLASS2)
                        {
                            wk.IMPORT_ERROR_MESSAGE = "フォーマット内で重複している在庫です。";
                        }
                    }
                }
                result.Add(wk);
            }
        }
    }
}

