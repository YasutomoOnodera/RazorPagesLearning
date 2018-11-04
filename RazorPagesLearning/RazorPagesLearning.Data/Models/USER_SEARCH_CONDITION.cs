using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
	/// <summary>
	/// ユーザー検索条件
	/// </summary>
	[Serializable]
	public class USER_SEARCH_CONDITION : COMMON_MANAGEMENT_INFORMATION
    {
		/// <summary>
		/// ユーザーアカウントID
		/// </summary>
		[Key]
		[Required]
		[DisplayName("ユーザーアカウントID")]
		public Int64 USER_ACCOUNT_ID { get; set; }

		/// <summary>
		/// 区分1
		/// </summary>
		[Required]
		[DisplayName("区分1")]
		public bool CLASS1 { get; set; }

		/// <summary>
		/// 区分2
		/// </summary>
		[Required]
		[DisplayName("区分2")]
		public bool CLASS2 { get; set; }

		/// <summary>
		/// 倉庫管理番号
		/// </summary>
		[Required]
		[DisplayName("倉庫管理番号")]
		public bool STORAGE_MANAGE_NUMBER { get; set; }

		/// <summary>
		/// Remark1
		/// </summary>
		[Required]
		[DisplayName("Remark1")]
		public bool REMARK1 { get; set; }

		/// <summary>
		/// Remark2
		/// </summary>
		[Required]
		[DisplayName("Remark2")]
		public bool REMARK2 { get; set; }

		/// <summary>
		/// 備考
		/// </summary>
		[Required]
		[DisplayName("備考")]
		public bool NOTE { get; set; }

		/// <summary>
		/// 制作日
		/// </summary>
		[Required]
		[DisplayName("制作日")]
		public bool PRODUCT_DATE { get; set; }

		/// <summary>
		/// 入庫日
		/// </summary>
		[Required]
		[DisplayName("入庫日")]
		public bool STORAGE_DATE { get; set; }

		/// <summary>
		/// 処理日
		/// </summary>
		[Required]
		[DisplayName("処理日")]
		public bool PROCESSING_DATE { get; set; }

		/// <summary>
		/// 廃棄予定日
		/// </summary>
		[Required]
		[DisplayName("廃棄予定日")]
		public bool SCRAP_SCHEDULE_DATE { get; set; }

		/// <summary>
		/// 出荷先/返却元
		/// </summary>
		[Required]
		[DisplayName("出荷先/返却元")]
		public bool SHIPPER_RETURN { get; set; }

		/// <summary>
		/// 登録日
		/// </summary>
		[Required]
		[DisplayName("登録日")]
		public bool REGIST_DATE { get; set; }

		/// <summary>
		/// お客様項目
		/// </summary>
		[Required]
		[DisplayName("お客様項目")]
		public bool CUSTOMER_ITEM { get; set; }

		#region // Navigation

		/// <summary>
		/// ユーザーアカウント
		/// </summary>
		[ForeignKey("USER_ACCOUNT_ID")]
		[Required]
		[DisplayName("ユーザーアカウント")]
		public USER_ACCOUNT USER_ACCOUNT { get; set; }

		#endregion // Navigation
	}
}
