using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.ExcelAnalysis
{
    /// <summary>
    /// 読み取られた在庫情報
    /// </summary>   
    public class ReadStockInfo
    {
        /// <summary>
        /// 在庫ID
        /// </summary>
        public Int64 STOCK_ID { get; set; }

        /// <summary>
        /// 倉庫管理番号
        /// </summary>
        public string STORAGE_MANAGE_NUMBER { get; set; }

        /// <summary>
        /// お客様番号
        /// </summary>
        public string CUSTOMER_MANAGE_NUMBER { get; set; }

        /// <summary>
        /// 区分1
        /// </summary>
        public string CLASS1 { get; set; }

        /// <summary>
        /// 区分2
        /// </summary>
        public string CLASS2 { get; set; }

        /// <summary>
        /// 寄託部課
        /// </summary>
        public string DEPARTMENT_CODE { get; set; } 

        /// <summary>
        /// 形状
        /// </summary>
        public string SHAPE { get; set; }

        /// <summary>
        /// 時間1
        /// </summary>
        public string TIME1 { get; set; }

        /// <summary>
        /// 時間2
        /// </summary>
        public string TIME2 { get; set; }

        /// <summary>
        /// 題名
        /// </summary>
        public string TITLE { get; set; }

        /// <summary>
        /// 副題
        /// </summary>
        public string SUBTITLE { get; set; }

        /// <summary>
        /// Remark1
        /// </summary>
        public string REMARK1 { get; set; }

        /// <summary>
        /// Remark2
        /// </summary>
        public string REMARK2 { get; set; }

        /// <summary>
        /// 破棄予定日
        /// </summary>
        public DateTimeOffset? SCRAP_SCHEDULE_DATE { get; set; }

        /// <summary>
        /// 制作日
        /// </summary>
        public string PRODUCT_DATE { get; set; }

        /// <summary>
        /// 備考
        /// </summary>
        public string NOTE { get; set; }

        /// <summary>
        /// 荷主項目
        /// </summary>
        public string SHIPPER_NOTE { get; set; }

    }
}
