using RazorPagesLearning.Service.ExcelAnalysis;
using RazorPagesLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.ExcelAnalysis
{
    /// <summary>
    /// エクセル解析結果構造体
    /// </summary>
    public class wkstock_Result
    {
        /// <summary>
        /// WK_STOCKのデータ
        /// </summary>
        public ReadStockInfo stock { get; set; }

        /// <summary>
        ///取り込み行番号 
        /// </summary>
        public int INPORT_LINE_NUMBER { get; set; }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string IMPORT_ERROR_MESSAGE { get; set; }

        /// <summary>
        /// エラーメッセージ(取り込み失敗時)
        /// </summary>
        public string NG_IMPORT_ERROR_MESSAGE { get; set; }

        /// <summary>
        /// 処理成功結果
        /// </summary>
        public bool Successed;

        public wkstock_Result()
        {
            stock = new ReadStockInfo();
            INPORT_LINE_NUMBER = -1;
            Successed = false;
        }

    }
}
