using RazorPagesLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 集配先マスタ
    /// </summary>   
    [Serializable]
    public class DELIVERY_ADMIN : MODIFY_USER_INFORMATION
    {
        /// <summary>
        /// ID
        /// </summary>
		[Key]
		[Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 ID { get; set; }

		/// <summary>
		/// 集配先コード
		/// WMSから返ってきたら、集配先コード(DELIVERY_CODE)に値を入れる
		/// </summary>
		[StringLength(8)]
		[DisplayName("集配先コード")]
		public string DELIVERY_CODE { get; set; }

		/// <summary>
		/// 荷主コード
		/// </summary>
		[Required]
		[StringLength(3)]
		[DisplayName("荷主コード")]
		public string SHIPPER_CODE { get; set; }

		/// <summary>
		/// 社名
		/// </summary>
		//[Required(ErrorMessage = "{0}に値を入力してください。")]
		[StringLength(72, MinimumLength = 1, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[DisplayName("社名")]
		public string COMPANY { get; set; }

		/// <summary>
		/// 部署名
		/// </summary>
		[StringLength(72)]
		[DisplayName("部署名")]
		public string DEPARTMENT { get; set; }

		/// <summary>
		/// 担当者名
		/// </summary>
		[StringLength(72)]
		[DisplayName("担当者名")]
		public string CHARGE_NAME { get; set; }

		/// <summary>
		/// 郵便番号
		/// </summary>
		[StringLength(8)]
		[RegularExpression(@"\d{3}-\d{4}$", ErrorMessage = "{0}は3桁-4桁の数字で入力してください。")]
		[DisplayName("郵便番号")]
		public string ZIPCODE { get; set; }

		/// <summary>
		/// 住所1
		/// </summary>
		[Required(ErrorMessage = "{0}に値を入力してください。")]
		[StringLength(72, MinimumLength = 1, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[DisplayName("住所1")]
		public string ADDRESS1 { get; set; }

		/// <summary>
		/// 住所2
		/// </summary>
		[StringLength(72)]
		[DisplayName("住所2")]
		public string ADDRESS2 { get; set; }

		/// <summary>
		/// TEL
		/// </summary>
		[Required(ErrorMessage = "{0}に値を入力してください。")]
		[RegularExpression(@"\d{0,14}$", ErrorMessage = "{0}は0から14桁の数字で入力してください。")]
		[DisplayName("TEL")]
		public string TEL { get; set; }

		/// <summary>
		/// FAX
		/// </summary>
		[RegularExpression(@"\d{0,14}$", ErrorMessage = "{0}は0から14桁の数字で入力してください。")]
		[DisplayName("FAX")]
		public string FAX { get; set; }

		/// <summary>
		/// メールアドレス
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
		[DisplayName("メールアドレス")]
		public string MAIL { get; set; }

		/// <summary>
		/// デフォルト便コード
		/// </summary>
		[StringLength(8)]
        [DisplayName("デフォルト便コード")]
        public string DEFAULT_FLIGHT_CODE { get; set; }

		/// <summary>
		/// 同時配信メール1
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
		[DisplayName("同時配信メール1")]
		public string MAIL1 { get; set; }

		/// <summary>
		/// 同時配信メール2
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
		[DisplayName("同時配信メール2")]
		public string MAIL2 { get; set; }

		/// <summary>
		/// 同時配信メール3
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
		[DisplayName("同時配信メール3")]
		public string MAIL3 { get; set; }

		/// <summary>
		/// 同時配信メール4
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
		[DisplayName("同時配信メール4")]
		public string MAIL4 { get; set; }

		/// <summary>
		/// 同時配信メール5
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
		[DisplayName("同時配信メール5")]
		public string MAIL5 { get; set; }

		/// <summary>
		/// 同時配信メール6
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
		[DisplayName("同時配信メール6")]
		public string MAIL6 { get; set; }

		/// <summary>
		/// 同時配信メール7
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
		[DisplayName("同時配信メール7")]
		public string MAIL7 { get; set; }

		/// <summary>
		/// 同時配信メール8
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
		[DisplayName("同時配信メール8")]
		public string MAIL8 { get; set; }

		/// <summary>
		/// 同時配信メール9
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
		[DisplayName("同時配信メール9")]
		public string MAIL9 { get; set; }

		/// <summary>
		/// 同時配信メール10
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
		[DisplayName("同時配信メール10")]
		public string MAIL10 { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        [Required]
        [DisplayName("削除フラグ")]
        public bool DELETE_FLAG { get; set; }

        #region Navigation

        /// <summary>
        /// 荷主マスタ
        /// </summary>
        [ForeignKey("SHIPPER_CODE")]
		[DisplayName("荷主マスタ")]
		public SHIPPER_ADMIN SHIPPER_ADMIN { get; set; }

        /// <summary>
        /// ユーザー集配先
        /// </summary>
        [ForeignKey("ID")]
        [DisplayName("ユーザー集配先")]
		public List<USER_DELIVERY> USER_DELIVERies { get; set; }

		#endregion // Navigation

    }
}