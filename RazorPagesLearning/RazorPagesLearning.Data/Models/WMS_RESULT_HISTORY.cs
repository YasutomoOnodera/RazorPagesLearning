using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// WMS_入出庫実績
    /// </summary>
    [Serializable]
    public class WMS_RESULT_HISTORY : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
		public Int64 ID { get; set; }

        /// <summary>
        /// 作業依頼履歴詳細ID
        /// </summary>
        [Required]
        [DisplayName("作業依頼履歴詳細ID")]
        public Int64 REQUEST_HISTORY_DETAIL_ID { get; set; }

        /// <summary>
        /// 在庫ID
        /// </summary>
        [Required]
		[DisplayName("在庫ID")]
		public Int64 STOCK_ID { get; set; }

		/// <summary>
		/// 倉庫管理番号
		/// </summary>
		[Required]
		[StringLength(30)]
		[DisplayName("倉庫管理番号")]
		public string STORAGE_MANAGE_NUMBER { get; set; }

        /// <summary>
        /// ステータス
        /// DOMAIN.CODE(KIND=00010004)
        /// </summary>
        [StringLength(8)]
        [DisplayName("ステータス")]
        public string STATUS { get; set; }

        /// <summary>
        /// お客様管理番号
        /// </summary>
        [StringLength(30)]
        [DisplayName("お客様管理番号")]
        public string CUSTOMER_MANAGE_NUMBER { get; set; }

        /// <summary>
        /// 題名
        /// </summary>
        [StringLength(200)]
		[DisplayName("題名")]
		public string TITLE { get; set; }

		/// <summary>
		/// 副題
		/// </summary>
		[StringLength(200)]
		[DisplayName("副題")]
		public string SUBTITLE { get; set; }

        /// <summary>
        /// 形状
        /// </summary>
        [StringLength(20)]
        [DisplayName("形状")]
        public string SHAPE { get; set; }

        /// <summary>
        /// Remark1
        /// </summary>
        [StringLength(72)]
        [DisplayName("Remark1")]
        public string REMARK1 { get; set; }

        /// <summary>
        /// Remark2
        /// </summary>
        [StringLength(72)]
        [DisplayName("Remark2")]
        public string REMARK2 { get; set; }

        /// <summary>
        /// 備考
        /// </summary>
        [StringLength(200)]
        [DisplayName("備考")]
        public string NOTE { get; set; }

        /// <summary>
        /// 荷主項目
        /// </summary>
        [StringLength(2000)]
		[DisplayName("荷主項目")]
		public string SHIPPER_NOTE { get; set; }

        /// <summary>
        /// 制作日
        /// </summary>
        [StringLength(10)]
        [DisplayName("制作日")]
        public string PRODUCT_DATE { get; set; }

        /// <summary>
        /// 入庫日
        /// </summary>
        [DisplayName("入庫日")]
        public DateTimeOffset? STORAGE_DATE { get; set; }

        /// <summary>
        /// 処理日
        /// </summary>
        [DisplayName("処理日")]
		public DateTimeOffset? PROCESSING_DATE { get; set; }

        /// <summary>
        /// 廃棄予定日
        /// </summary>
        [DisplayName("廃棄予定日")]
        public DateTimeOffset? SCRAP_SCHEDULE_DATE { get; set; }

        /// <summary>
        /// 時間1
        /// </summary>
        [StringLength(3)]
        [DisplayName("時間1")]
        public string TIME1 { get; set; }

        /// <summary>
        /// 時間2
        /// </summary>
        [StringLength(3)]
        [DisplayName("時間2")]
        public string TIME2 { get; set; }

        /// <summary>
        /// 入出庫日
        /// </summary>
        [DisplayName("入出庫日")]
        public DateTimeOffset? STORAGE_RETRIEVAL_DATE { get; set; }

        /// <summary>
        /// 着時間
        /// </summary>
        [StringLength(128)]
        [DisplayName("着時間")]
        public string ARRIVAL_TIME { get; set; }

        /// <summary>
        /// 伝票番号
        /// </summary>
        [StringLength(128)]
        [DisplayName("伝票番号")]
        public string SLIP_NUMBER { get; set; }

        /// <summary>
        /// 受付番号
        /// </summary>
        [StringLength(8)]
        [DisplayName("受付番号")]
        public string ORDER_NUMBER { get; set; }

        /// <summary>
        /// 出荷先/返却元(集配先コード)
        /// </summary>
        [StringLength(8)]
        [DisplayName("出荷先/返却元(集配先コード)")]
        public string SHIP_RETURN_CODE { get; set; }

        /// <summary>
        /// 出荷先/返却元(社名)
        /// </summary>
        [StringLength(72)]
        [DisplayName("出荷先/返却元(社名)")]
        public string SHIP_COMPANY { get; set; }

        /// <summary>
        /// 出荷先/返却元(部署名)
        /// </summary>
        [StringLength(72)]
        [DisplayName("出荷先/返却元(部署名)")]
        public string SHIP_DEPARTMENT { get; set; }

        /// <summary>
        /// 出荷先/返却元(担当者名)
        /// </summary>
        [StringLength(72)]
        [DisplayName("出荷先/返却元(担当者名)")]
        public string SHIP_CHARGE_NAME { get; set; }

        /// <summary>
        /// 依頼数
        /// </summary>
        [DisplayName("依頼数")]
		public int REQUEST_COUNT { get; set; }

		/// <summary>
		/// 確定数
		/// </summary>
		[DisplayName("確定数")]
		public int CONFIRM_COUNT { get; set; }

        #region Navigation

        /// <summary>
        /// 作業依頼履歴詳細
        /// </summary>
        [ForeignKey("REQUEST_HISTORY_DETAIL_ID")]
		[DisplayName("作業依頼履歴詳細")]
		public REQUEST_HISTORY_DETAIL REQUEST_HISTORY_DETAIL { get; set; }

		#endregion // Navigation

	}
}
