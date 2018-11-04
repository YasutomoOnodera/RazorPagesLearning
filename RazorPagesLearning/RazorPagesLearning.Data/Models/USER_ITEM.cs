using RazorPagesLearning.Data.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ユーザー項目
    /// </summary>   
    [Serializable]
    public class USER_ITEM : MODIFY_USER_INFORMATION
    {
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
        public Int64 USER_ACCOUNT_ID { get; set; }

        /// <summary>
        /// お客様項目1_コード
        /// </summary>
        [StringLength(8)]
		[DisplayName("お客様項目1_コード")]
		public string ITEM1_CODE { get; set; }

		/// <summary>
		/// お客様項目1_値
		/// </summary>
		[StringLength(50)]
		[DisplayName("お客様項目1_値")]
		public string ITEM1_VALUE { get; set; }

		/// <summary>
		/// お客様項目2_コード
		/// </summary>
		[StringLength(8)]
		[DisplayName("お客様項目2_コード")]
		public string ITEM2_CODE { get; set; }

		/// <summary>
		/// お客様項目2_値
		/// </summary>
		[StringLength(50)]
		[DisplayName("お客様項目2_値")]
		public string ITEM2_VALUE { get; set; }

		/// <summary>
		/// お客様項目3_コード
		/// </summary>
		[StringLength(8)]
		[DisplayName("お客様項目3_コード")]
		public string ITEM3_CODE { get; set; }

		/// <summary>
		/// お客様項目3_値
		/// </summary>
		[StringLength(50)]
		[DisplayName("お客様項目3_値")]
		public string ITEM3_VALUE { get; set; }

        #region Navigation

        /// <summary>
        /// ユーザーアカウント
        /// </summary>
        [ForeignKey("USER_ACCOUNT_ID")]
        [DisplayName("ユーザーアカウント")]
        public USER_ACCOUNT USER_ACCOUNT { get; set; }

        /// <summary>
        /// 在庫
        /// </summary>
        [ForeignKey("STOCK_ID")]
        [DisplayName("在庫")]
        public STOCK STOCK { get; set; }

        #endregion // Navigation

    }
}
