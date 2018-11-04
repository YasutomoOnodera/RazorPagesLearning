//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RazorPagesLearning.Utility.ReportHelper
//{
//    /// <summary>
//    /// レポート出力補助関数
//    /// </summary>
//    public static class HelperFunctions
//    {
//        /// <summary>
//        /// 書き出し処理用の設定
//        /// </summary>
//        public class WriteConfig
//        {

//            /// <summary>
//            /// データを書き出す対象
//            /// </summary>
//            public HttpResponse target;

//            /// <summary>
//            /// 書き出し対象のファイル名
//            /// </summary>
//            public string fileName;

//            /// <summary>
//            /// 書き出し対象のレポートオブジェクト
//            /// </summary>
//            public GrapeCity.ActiveReports.SectionReport report;

//        }

//        /// <summary>
//        /// Exce形式でレポート出力を行う
//        /// </summary>
//        /// <param name="arg"></param>
//        public static void writeExcel(WriteConfig arg)
//        {
//            var r = new RazorPagesLearning.Report.Export();

//            // ブラウザに対してPDFドキュメントの適切なビューワを使用するように指定します。
//            // エクスポート形式別にコンテンツタイプを
//            // 以下のように変更できます。
//            //	ExportType  ContentType
//            //	PDF	   "application/pdf"  （小文字）
//            //	RTF	   "application/rtf"
//            //	TIFF	  "image/tiff"	   （ブラウザとは別のビューワで表示される）
//            //	HTML	  "message/rfc822"   （画像を含む圧縮されたHTMLページに適用する）
//            //	Excel	 "application/vnd.ms-excel"
//            //	Excel	 "application/excel" （いずれかが動作される）
//            //	Text	  "text/plain"  
//            // Response.ContentType = "application/pdf";
//            //Response.Headers.Add("content-disposition", "inline; filename=MyPDF.PDF");
//            arg.target.ContentType = "application/excel";
//            arg.target.Headers.Add("content-disposition", $"inline; filename={arg.fileName}");
//            r.exportExcel(arg.report, arg.target);

//        }


//        /// <summary>
//        /// Csv形式でレポート出力を行う
//        /// </summary>
//        /// <param name="arg"></param>
//        public static void writeCsv(WriteConfig arg)
//        {
//            var r = new RazorPagesLearning.Report.Export();

//            // ブラウザに対してPDFドキュメントの適切なビューワを使用するように指定します。
//            // エクスポート形式別にコンテンツタイプを
//            // 以下のように変更できます。
//            //	ExportType  ContentType
//            //	PDF	   "application/pdf"  （小文字）
//            //	RTF	   "application/rtf"
//            //	TIFF	  "image/tiff"	   （ブラウザとは別のビューワで表示される）
//            //	HTML	  "message/rfc822"   （画像を含む圧縮されたHTMLページに適用する）
//            //	Excel	 "application/vnd.ms-excel"
//            //	Excel	 "application/excel" （いずれかが動作される）
//            //	Text	  "text/plain"  
//            // Response.ContentType = "application/pdf";
//            //Response.Headers.Add("content-disposition", "inline; filename=MyPDF.PDF");
//            arg.target.ContentType = "text/csv";
//            arg.target.Headers.Add("content-disposition", $"inline; filename={arg.fileName}");
//            r.exportCSV(arg.report, arg.target);
//        }

//        /// <summary>
//        /// Pdf形式でレポート出力を行う
//        /// </summary>
//        /// <param name="arg"></param>
//        public static void writePdf(WriteConfig arg)
//        {
//            var r = new RazorPagesLearning.Report.Export();

//            // ブラウザに対してPDFドキュメントの適切なビューワを使用するように指定します。
//            // エクスポート形式別にコンテンツタイプを
//            // 以下のように変更できます。
//            //	ExportType  ContentType
//            //	PDF	   "application/pdf"  （小文字）
//            //	RTF	   "application/rtf"
//            //	TIFF	  "image/tiff"	   （ブラウザとは別のビューワで表示される）
//            //	HTML	  "message/rfc822"   （画像を含む圧縮されたHTMLページに適用する）
//            //	Excel	 "application/vnd.ms-excel"
//            //	Excel	 "application/excel" （いずれかが動作される）
//            //	Text	  "text/plain"  
//            arg.target.ContentType = "application/pdf";
//            arg.target.Headers.Add("content-disposition", $"inline; filename={arg.fileName}");
//            r.exportPdf(arg.report, arg.target);
//        }


//        /// <summary>
//        /// 既存のファイル書き出し設定
//        /// </summary>
//        public class WriteExistingFileConfig
//        {

//            /// <summary>
//            /// データを書き出す対象
//            /// </summary>
//            public HttpResponse target;

//            /// <summary>
//            /// 書き出し対象のファイル名
//            /// </summary>
//            public string fileName;

//            /// <summary>
//            /// 出力対象ファイルパス
//            /// </summary>
//            public string targetFilePath;

//            /// <summary>
//            /// ヘッダーに付加するコンテンツ種別
//            /// </summary>
//            public string ContentType;

//        }

//        /// <summary>
//        /// 既存のファイルを書き出す
//        /// </summary>
//        /// <param name="arg"></param>
//        public static void writeExistingFile(WriteExistingFileConfig arg)
//        {
//            // ブラウザに対してPDFドキュメントの適切なビューワを使用するように指定します。
//            // エクスポート形式別にコンテンツタイプを
//            // 以下のように変更できます。
//            //	ExportType  ContentType
//            //	PDF	   "application/pdf"  （小文字）
//            //	RTF	   "application/rtf"
//            //	TIFF	  "image/tiff"	   （ブラウザとは別のビューワで表示される）
//            //	HTML	  "message/rfc822"   （画像を含む圧縮されたHTMLページに適用する）
//            //	Excel	 "application/vnd.ms-excel"
//            //	Excel	 "application/excel" （いずれかが動作される）
//            //	Text	  "text/plain"  
//            using (FileStream source = new FileStream(arg.targetFilePath, FileMode.Open))
//            using (System.IO.MemoryStream memStream = new System.IO.MemoryStream())
//            {
//                //zipファイルの中身をメモリにコピーする
//                source.Position = 0;
//                source.CopyTo(memStream);

//                var t = memStream.ToArray();

//                arg.target.ContentType = arg.ContentType;
//                arg.target.Headers.Add("content-disposition", $"inline; filename={arg.fileName}");
//                arg.target.Headers.Add("Content-Length", t.Length.ToString()); // <- important これ入れないとchrome firefoxでダウンロードできない。

//                arg.target.Body.Write(t, 0, t.Count());
//            }

//        }


//        /// <summary>
//        /// 一時ディレクトリを用意して引数関数に渡す。
//        /// 関数を抜けたタイミングで一時ディレクトリも合わせて消去する。
//        /// </summary>
//        /// <typeparam name="RetType"></typeparam>
//        /// <param name="operation"></param>
//        /// <returns></returns>
//        public static async Task<RetType> TemporaryDirectoryAccompanyBlock
//            <RetType>(Func<string, Task<RetType>> operation)
//        {
//            //一時ディレクトリを取得する
//            var workPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

//            try
//            {
//                //作業用の一時ディレクトリを生成する
//                Directory.CreateDirectory(workPath);

//                //処理アクションを呼び出す
//                return await operation(workPath);
//            }
//            finally
//            {
//                try
//                {
//                    Report.Helper.DirectoryHelper.DeleteDirectory(workPath);
//                }
//                catch
//                {
//                    //消去に失敗しても無視する
//                }
//            }
//        }

//        /// <summary>
//        /// エラーCSVを出力する
//        /// </summary>
//        public class ErrorCsvConfig
//        {
//            /// <summary>
//            /// データを書き出す対象
//            /// </summary>
//            public HttpResponse target;

//            /// <summary>
//            /// 書き出し対象のファイル名
//            /// </summary>
//            public string fileName;

//            /// <summary>
//            /// 出力エンコーディング
//            /// </summary>
//            public Encoding encoding;

//            /// <summary>
//            /// 書き出し処理
//            /// </summary>
//            public Action<System.IO.StreamWriter> writerOperation;

//            public ErrorCsvConfig()
//            {
//                //デフォルトはshift_jisとする
//                this.encoding = System.Text.Encoding.GetEncoding("shift_jis");
//            }
//        }

//        /// <summary>
//        /// エラーCSVファイルを書き出す
//        /// </summary>
//        /// <param name="arg"></param>
//        public static void writeErrorCsvFile(ErrorCsvConfig arg)
//        {
//            using (var ms = new System.IO.MemoryStream())
//            using (var csv = new System.IO.StreamWriter(ms, arg.encoding))
//            {
//                //書き出し操作コールバックを呼び出し
//                arg.writerOperation(csv);

//                //書き出しフラッシュ
//                csv.Flush();

//                //出力データ生成
//                var t = ms.ToArray();

//                //データ書き出し
//                arg.target.ContentType = "text/csv";
//                arg.target.Headers.Add("content-disposition", $"inline; filename={arg.fileName}");
//                arg.target.Headers.Add("Content-Length", t.Length.ToString()); // <- important これ入れないとchrome firefoxでダウンロードできない。
//                arg.target.Body.Write(t, 0, t.Count());
//            }
//        }
//    }
//}
