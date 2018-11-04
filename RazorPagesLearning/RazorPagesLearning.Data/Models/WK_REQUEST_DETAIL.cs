using RazorPagesLearning.Data.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 作業依頼詳細ワーク
    /// </summary>
    [Serializable]
    public class WK_REQUEST_DETAIL : COMMON_MANAGEMENT_INFORMATION
    {
		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
		public Int64 ID { get; set; }

		/// <summary>
		/// 作業依頼ワークID
		/// </summary>
		[Required]
		public Int64 WK_REQUEST_ID { get; set; }

		/// <summary>
		/// 在庫ID
		/// </summary>
		[Required]
        [DisplayName("在庫ID")]
        public Int64 STOCK_ID { get; set; }

		/// <summary>
		/// 依頼数
		/// </summary>
		[Required]
		[DisplayName("依頼数")]
		public int REQUEST_COUNT { get; set; }

		#region Navigation

		/// <summary>
		/// 作業依頼ワーク
		/// </summary>
		[ForeignKey("WK_REQUEST_ID")]
		[DisplayName("作業依頼ワーク")]
		public WK_REQUEST WK_REQUEST { get; set; }

		/// <summary>
		/// 在庫
		/// </summary>
		[ForeignKey("STOCK_ID")]
		[DisplayName("在庫")]
		public STOCK STOCK { get; set; }

		#endregion // Navigation

	}
}
