using RazorPagesLearning.Data.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ウォッチリスト
    /// </summary>
    [Serializable]
    public class WATCHLIST : COMMON_MANAGEMENT_INFORMATION
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

		#region Navigation

		/// <summary>
		/// 在庫
		/// </summary>
		[ForeignKey("STOCK_ID")]
		[DisplayName("在庫")]
		public STOCK STOCK { get; set; }

		/// <summary>
		/// ユーザーアカウント
		/// </summary>
		[ForeignKey("USER_ACCOUNT_ID")]
		[DisplayName("ユーザーアカウント")]
		public USER_ACCOUNT USER_ACCOUNT { get; set; }

		#endregion // Navigation

    }

}