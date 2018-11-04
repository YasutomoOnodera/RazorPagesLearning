using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ìÆËðÚ×
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
		/// ìÆËðID
		/// </summary>
		[Required]
		[DisplayName("ìÆËðID")]
		public Int64 REQUEST_HISTORY_ID { get; set; }

        /// <summary>
        /// ótÔ
        /// </summary>
        [StringLength(8)]
        [DisplayName("ótÔ")]
        public string ORDER_NUMBER { get; set; }

        /// <summary>
        /// `[Ô
        /// </summary>
        [StringLength(128)]
        [DisplayName("`[Ô")]
        public string SLIP_NUMBER { get; set; }

        /// <summary>
        /// WMSóÔ
        /// DOMAIN.CODE(KIND=00090000)
        /// </summary>
        [StringLength(8)]
        [DisplayName("WMSóÔ")]
        public string WMS_STATUS { get; set; }

        /// <summary>
        /// Ë
        /// </summary>
        [DisplayName("Ë")]
        public int? REQUEST_COUNT { get; set; }

        /// <summary>
        /// mè
        /// </summary>
        [DisplayName("mè")]
        public int? CONFIRM_COUNT { get; set; }

        /// <summary>
        /// ÝÉID
        /// </summary>
        [Required]
		[DisplayName("ÝÉID")]
		public Int64 STOCK_ID { get; set; }

        /// <summary>
        /// qÉÇÔ
        /// </summary>
        [StringLength(30)]
        [DisplayName("qÉÇÔ")]
        public string STORAGE_MANAGE_NUMBER { get; set; }

        /// <summary>
        /// Xe[^X
        /// </summary>
        [StringLength(8)]
        [DisplayName("Xe[^X")]
        public string STATUS { get; set; }

        /// <summary>
        /// ¨qlÇÔ
        /// </summary>
        [StringLength(30)]
        [DisplayName("¨qlÇÔ")]
        public string CUSTOMER_MANAGE_NUMBER { get; set; }

        /// <summary>
        /// è¼
        /// </summary>
        [StringLength(200)]
		[DisplayName("è¼")]
		public string TITLE { get; set; }

		/// <summary>
		/// è
		/// </summary>
		[StringLength(200)]
		[DisplayName("è")]
		public string SUBTITLE { get; set; }

        /// <summary>
        /// ×åR[h
        /// </summary>
        [StringLength(3)]
        [DisplayName("×åR[h")]
        public string SHIPPER_CODE { get; set; }

        /// <summary>
        /// ÛR[h
        /// </summary>
        [StringLength(128)]
        [DisplayName("ÛR[h")]
        public string DEPARTMENT_CODE { get; set; }

        /// <summary>
        /// `ó
        /// </summary>
        [StringLength(20)]
        [DisplayName("`ó")]
        public string SHAPE { get; set; }

        /// <summary>
        /// æª1
        /// DOMAIN.CODE(KIND=00010002)
        /// </summary>
        [StringLength(8)]
        [DisplayName("æª1")]
        public string CLASS1 { get; set; }

        /// <summary>
        /// æª2
        /// DOMAIN.CODE(KIND=00010003)
        /// </summary>
        [StringLength(8)]
        [DisplayName("æª2")]
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
        /// õl
        /// </summary>
        [StringLength(200)]
        [DisplayName("õl")]
        public string NOTE { get; set; }

        /// <summary>
        /// ×åÚ
        /// </summary>
        [StringLength(2000)]
		[DisplayName("×åÚ")]
		public string SHIPPER_NOTE { get; set; }

		/// <summary>
		/// §ìú
		/// </summary>
		[StringLength(10)]
		[DisplayName("§ìú")]
        public string PRODUCT_DATE { get; set; }

		/// <summary>
		/// üÉú
		/// </summary>
		[DisplayName("üÉú")]
		public DateTimeOffset? STORAGE_DATE { get; set; }

		/// <summary>
		/// ú
		/// </summary>
		[DisplayName("ú")]
        public DateTimeOffset? PROCESSING_DATE { get; set; }

        /// <summary>
        /// pü\èú
        /// </summary>
        [DisplayName("pü\èú")]
        public DateTimeOffset? SCRAP_SCHEDULE_DATE { get; set; }

		/// <summary>
		/// Ô1
		/// </summary>
		[StringLength(3)]
		[DisplayName("Ô1")]
		public string TIME1{ get; set; }

		/// <summary>
		/// Ô2
		/// </summary>
		[StringLength(3)]
		[DisplayName("Ô2")]
		public string TIME2 { get; set; }

		/// <summary>
		/// ÝÉ
		/// </summary>
		[DisplayName("ÝÉ")]
		public int? STOCK_COUNT { get; set; }

		/// <summary>
		/// üoÉú
		/// </summary>
		[DisplayName("üoÉú")]
		public DateTimeOffset? STORAGE_RETRIEVAL_DATE { get; set; }

		/// <summary>
		/// Ô
		/// </summary>
		[StringLength(128)]
		[DisplayName("Ô")]
		public string ARRIVAL_TIME { get; set; }

		/// <summary>
		/// o[R[h
		/// </summary>
		[StringLength(9)]
		[DisplayName("o[R[h")]
		public string BARCODE { get; set; }

		/// <summary>
		/// ÝÉíÊ
		/// DOMAIN.CODE(KIND=00010008)
		/// </summary>
		[StringLength(8)]
        [DisplayName("ÝÉíÊ")]
        public string STOCK_KIND { get; set; }

        /// <summary>
        /// P¿
        /// ÞêpÚ
        /// </summary>
        [DisplayName("P¿")]
        public int? UNIT { get; set; }

        /// <summary>
        /// WMSo^ú
        /// </summary>
        [DisplayName("WMSo^ú")]
		public DateTimeOffset? WMS_REGIST_DATE{ get; set; }

        /// <summary>
        /// WMSXVú
        /// </summary>
        [DisplayName("WMSXVú")]
		public DateTimeOffset? WMS_UPDATE_DATE { get; set; }

		/// <summary>
		/// ProjectNo1
		/// ÚqêpÚ
		/// </summary>
		[StringLength(20)]
		[DisplayName("ProjectNo1")]
		public string PROJECT_NO1 { get; set; }

		/// <summary>
		/// ProjectNo2
		/// ÚqêpÚ
		/// </summary>
		[StringLength(50)]
		[DisplayName("ProjectNo2")]
		public string PROJECT_NO2 { get; set; }

		/// <summary>
		/// ì 1
		/// ÚqêpÚ
		/// DOMAIN.CODE(KIND=00010005)
		/// </summary>
		[StringLength(8)]
		[DisplayName("ì 1")]
		public string COPYRIGHT1 { get; set; }

		/// <summary>
		/// ì 2
		/// ÚqêpÚ
		/// </summary>
		[StringLength(50)]
		[DisplayName("ì 2")]
		public string COPYRIGHT2 { get; set; }

		/// <summary>
		/// _ñ1
		/// ÚqêpÚ
		/// DOMAIN.CODE(KIND=00010006)
		/// </summary>
		[StringLength(8)]
		[DisplayName("_ñ1")]
		public string CONTRACT1 { get; set; }

		/// <summary>
		/// _ñ2
		/// ÚqêpÚ
		/// </summary>
		[StringLength(50)]
		[DisplayName("_ñ2")]
		public string CONTRACT2 { get; set; }

		/// <summary>
		/// f[^NO1
		/// ÚqêpÚ
		/// </summary>
		[StringLength(20)]
		[DisplayName("f[^NO1")]
		public string DATA_NO1 { get; set; }

		/// <summary>
		/// f[^NO2
		/// ÚqêpÚ
		/// </summary>
		[StringLength(50)]
		[DisplayName("f[^NO2")]
		public string DATA_NO2 { get; set; }

		/// <summary>
		/// »è1
		/// ÚqêpÚ
		/// DOMAIN.CODE(KIND=00010007)
		/// </summary>
		[StringLength(8)]
		[DisplayName("»è1")]
		public string PROCESS_JUDGE1 { get; set; }

		/// <summary>
		/// »è2
		/// ÚqêpÚ
		/// </summary>
		[StringLength(50)]
		[DisplayName("»è2")]
		public string PROCESS_JUDGE2 { get; set; }

        #region Navigation

        /// <summary>
        /// ìÆËð
        /// </summary>
        [ForeignKey("REQUEST_HISTORY_ID")]
		[DisplayName("ìÆËð")]
		public REQUEST_HISTORY REQUEST_HISTORY { get; set; }

		/// <summary>
		/// WMS_üoÉÀÑ
		/// </summary>
		[DisplayName("WMS_üoÉÀÑ")]
		public WMS_RESULT_HISTORY WMS_RESULT_HISTORY { get; set; }

		#endregion // Navigation

    }

}