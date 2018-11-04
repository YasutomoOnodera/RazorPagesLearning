using RazorPagesLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 作業依頼ワーク
    /// </summary>
    [Serializable]
    public class WK_REQUEST : COMMON_MANAGEMENT_INFORMATION
    {
		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
		public Int64 ID { get; set; }

		/// <summary>
		/// ユーザーアカウントID
		/// </summary>
		[Required]
		[DisplayName("ユーザーアカウントID")]
		public Int64 USER_ACCOUNT_ID { get; set; }

		/// <summary>
		/// 依頼内容
		/// DOMAIN.CODE(KIND=00020001)
		/// </summary>
		[Required]
		[StringLength(8)]
		[DisplayName("依頼内容")]
		public string REQUEST_KIND { get; set; }

		/// <summary>
		/// 依頼数合計
		/// </summary>
		[Required]
		[DisplayName("依頼数合計")]
		public int REQUEST_COUNT_SUM { get; set; }

		/// <summary>
		/// 明細数
		/// </summary>
		[Required]
		[DisplayName("明細数")]
		public int DETAIL_COUNT { get; set; }

		#region Navigation

		/// <summary>
		/// ユーザーアカウント
		/// </summary>
		[ForeignKey("USER_ACCOUNT_ID")]
		[DisplayName("ユーザーアカウント")]
		public USER_ACCOUNT USER_ACCOUNT { get; set; }

        /// <summary>
        /// 作業依頼集配先ワーク
        /// </summary>
        [DisplayName("作業依頼集配先ワーク")]
        public WK_REQUEST_DELIVERY WK_REQUEST_DELIVERY { get; set; }

        /// <summary>
        /// 作業依頼詳細ワーク
        /// </summary>
        [DisplayName("作業依頼詳細ワーク")]
        public List<WK_REQUEST_DETAIL> WK_REQUEST_DETAILs { get; set; }

        #endregion // Navigation
    }
}
