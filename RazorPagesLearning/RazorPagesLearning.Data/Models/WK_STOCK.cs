using RazorPagesLearning.Data.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 在庫ワーク
    /// </summary>   
    [Serializable]
    public class WK_STOCK : COMMON_MANAGEMENT_INFORMATION
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WK_STOCK()
        {
        }

        /// <summary>
		/// ID
		/// </summary>
		[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public Int64 ID { get; set; }

        /// <summary>
		/// 在庫ID
		/// </summary>
        [DisplayName("在庫ID")]
        public Int64 STOCK_ID { get; set; }

        /// <summary>
		/// ユーザーアカウントID
		/// </summary>
        [Required]
		[DisplayName("ユーザーアカウントID")]
		public Int64 USER_ACOUNT_ID { get; set; }

		/// <summary>
		/// 処理種別
		/// DOMAIN.CODE(KIND=00010010)
		/// </summary>
		[Required]
		[StringLength(8)]
        [DisplayName("処理種別")]
        public string KIND { get; set; }

		/// <summary>
		/// 倉庫管理番号
		/// </summary>
		[StringLength(30)]
        [DisplayName("倉庫管理番号")]
        public string STORAGE_MANAGE_NUMBER { get; set; }

        /// <summary>
		/// お客様管理番号
		/// </summary>
		[StringLength(30)]
        [DisplayName("お客様管理番号")]
        public string CUSTOMER_MANAGE_NUMBER { get; set; }

        /// <summary>
		/// 題名
		/// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
        [DisplayName("題名")]
        public string TITLE { get; set; }

        /// <summary>
        /// 副題
        /// </summary>
        [StringLength(200)]
        [DisplayName("副題")]
        public string SUBTITLE { get; set; }

        /// <summary>
		/// 部課コード
		/// </summary>
		[StringLength(128)]
        [DisplayName("部課コード")]
        public string DEPARTMENT_CODE { get; set; }

        /// <summary>
		/// 形状
		/// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
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
		/// 取込行番号
		/// </summary>
		[DisplayName("取込行番号")]
		public int IMPORT_LINE_NUMBER { get; set; }

        /// <summary>
		/// エラーメッセージ
		/// </summary>
		[StringLength(128)]
        [DisplayName("エラーメッセージ")]
        public string IMPORT_ERROR_MESSAGE { get; set; }

		#region Navigation

		/// <summary>
		/// 在庫
		/// </summary>
		[DisplayName("在庫")]
        public STOCK STOCK { get; set; }

        #endregion // Navigation
    }
}