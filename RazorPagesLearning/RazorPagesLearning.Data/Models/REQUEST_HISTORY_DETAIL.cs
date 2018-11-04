using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 作業依頼履歴詳細
    /// </summary>   
    [Serializable]
    public class REQUEST_HISTORY_DETAIL : MODIFY_USER_INFORMATION
    {
        /// <summary>
        /// ID
        /// </summary>
		[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
        public Int64 ID { get; set; }

		/// <summary>
		/// 作業依頼履歴ID
		/// </summary>
		[Required]
		[DisplayName("作業依頼履歴ID")]
		public Int64 REQUEST_HISTORY_ID { get; set; }

        /// <summary>
        /// 受付番号
        /// </summary>
        [StringLength(8)]
        [DisplayName("受付番号")]
        public string ORDER_NUMBER { get; set; }

        /// <summary>
        /// 伝票番号
        /// </summary>
        [StringLength(128)]
        [DisplayName("伝票番号")]
        public string SLIP_NUMBER { get; set; }

        /// <summary>
        /// WMS状態
        /// DOMAIN.CODE(KIND=00090000)
        /// </summary>
        [StringLength(8)]
        [DisplayName("WMS状態")]
        public string WMS_STATUS { get; set; }

        /// <summary>
        /// 依頼数
        /// </summary>
        [DisplayName("依頼数")]
        public int? REQUEST_COUNT { get; set; }

        /// <summary>
        /// 確定数
        /// </summary>
        [DisplayName("確定数")]
        public int? CONFIRM_COUNT { get; set; }

        /// <summary>
        /// 在庫ID
        /// </summary>
        [Required]
		[DisplayName("在庫ID")]
		public Int64 STOCK_ID { get; set; }

        /// <summary>
        /// 倉庫管理番号
        /// </summary>
        [StringLength(30)]
        [DisplayName("倉庫管理番号")]
        public string STORAGE_MANAGE_NUMBER { get; set; }

        /// <summary>
        /// ステータス
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
        /// 荷主コード
        /// </summary>
        [StringLength(3)]
        [DisplayName("荷主コード")]
        public string SHIPPER_CODE { get; set; }

        /// <summary>
        /// 部課コード
        /// </summary>
        [StringLength(128)]
        [DisplayName("部課コード")]
        public string DEPARTMENT_CODE { get; set; }

        /// <summary>
        /// 形状
        /// </summary>
        [StringLength(20)]
        [DisplayName("形状")]
        public string SHAPE { get; set; }

        /// <summary>
        /// 区分1
        /// DOMAIN.CODE(KIND=00010002)
        /// </summary>
        [StringLength(8)]
        [DisplayName("区分1")]
        public string CLASS1 { get; set; }

        /// <summary>
        /// 区分2
        /// DOMAIN.CODE(KIND=00010003)
        /// </summary>
        [StringLength(8)]
        [DisplayName("区分2")]
        public string CLASS2 { get; set; }

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
		public string TIME1{ get; set; }

		/// <summary>
		/// 時間2
		/// </summary>
		[StringLength(3)]
		[DisplayName("時間2")]
		public string TIME2 { get; set; }

		/// <summary>
		/// 在庫数
		/// </summary>
		[DisplayName("在庫数")]
		public int? STOCK_COUNT { get; set; }

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
		/// バーコード
		/// </summary>
		[StringLength(9)]
		[DisplayName("バーコード")]
		public string BARCODE { get; set; }

		/// <summary>
		/// 在庫種別
		/// DOMAIN.CODE(KIND=00010008)
		/// </summary>
		[StringLength(8)]
        [DisplayName("在庫種別")]
        public string STOCK_KIND { get; set; }

        /// <summary>
        /// 単価
        /// 資材専用項目
        /// </summary>
        [DisplayName("単価")]
        public int? UNIT { get; set; }

        /// <summary>
        /// WMS登録日
        /// </summary>
        [DisplayName("WMS登録日")]
		public DateTimeOffset? WMS_REGIST_DATE{ get; set; }

        /// <summary>
        /// WMS更新日
        /// </summary>
        [DisplayName("WMS更新日")]
		public DateTimeOffset? WMS_UPDATE_DATE { get; set; }

		/// <summary>
		/// ProjectNo1
		/// 顧客専用項目
		/// </summary>
		[StringLength(20)]
		[DisplayName("ProjectNo1")]
		public string PROJECT_NO1 { get; set; }

		/// <summary>
		/// ProjectNo2
		/// 顧客専用項目
		/// </summary>
		[StringLength(50)]
		[DisplayName("ProjectNo2")]
		public string PROJECT_NO2 { get; set; }

		/// <summary>
		/// 著作権1
		/// 顧客専用項目
		/// DOMAIN.CODE(KIND=00010005)
		/// </summary>
		[StringLength(8)]
		[DisplayName("著作権1")]
		public string COPYRIGHT1 { get; set; }

		/// <summary>
		/// 著作権2
		/// 顧客専用項目
		/// </summary>
		[StringLength(50)]
		[DisplayName("著作権2")]
		public string COPYRIGHT2 { get; set; }

		/// <summary>
		/// 契約書1
		/// 顧客専用項目
		/// DOMAIN.CODE(KIND=00010006)
		/// </summary>
		[StringLength(8)]
		[DisplayName("契約書1")]
		public string CONTRACT1 { get; set; }

		/// <summary>
		/// 契約書2
		/// 顧客専用項目
		/// </summary>
		[StringLength(50)]
		[DisplayName("契約書2")]
		public string CONTRACT2 { get; set; }

		/// <summary>
		/// データNO1
		/// 顧客専用項目
		/// </summary>
		[StringLength(20)]
		[DisplayName("データNO1")]
		public string DATA_NO1 { get; set; }

		/// <summary>
		/// データNO2
		/// 顧客専用項目
		/// </summary>
		[StringLength(50)]
		[DisplayName("データNO2")]
		public string DATA_NO2 { get; set; }

		/// <summary>
		/// 処理判定1
		/// 顧客専用項目
		/// DOMAIN.CODE(KIND=00010007)
		/// </summary>
		[StringLength(8)]
		[DisplayName("処理判定1")]
		public string PROCESS_JUDGE1 { get; set; }

		/// <summary>
		/// 処理判定2
		/// 顧客専用項目
		/// </summary>
		[StringLength(50)]
		[DisplayName("処理判定2")]
		public string PROCESS_JUDGE2 { get; set; }

        #region Navigation

        /// <summary>
        /// 作業依頼履歴
        /// </summary>
        [ForeignKey("REQUEST_HISTORY_ID")]
		[DisplayName("作業依頼履歴")]
		public REQUEST_HISTORY REQUEST_HISTORY { get; set; }

		/// <summary>
		/// WMS_入出庫実績
		/// </summary>
		[DisplayName("WMS_入出庫実績")]
		public WMS_RESULT_HISTORY WMS_RESULT_HISTORY { get; set; }

		#endregion // Navigation

    }

}