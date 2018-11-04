using RazorPagesLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 作業依頼集配先ワーク
    /// </summary>
    [Serializable]
    public class WK_REQUEST_DELIVERY : COMMON_MANAGEMENT_INFORMATION
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
        [DisplayName("作業依頼ワークID")]
        public Int64 WK_REQUEST_ID { get; set; }

		/// <summary>
		/// ユーザーアカウントID
		/// </summary>
		[Required]
		[DisplayName("ユーザーアカウントID")]
		public Int64 USER_ACCOUNT_ID { get; set; }

        /// <summary>
        /// 集配先マスタID
        /// </summary>
        [Required]
		[DisplayName("集配先マスタID")]
		public Int64 DELIVERY_ADMIN_ID { get; set; }

		/// <summary>
		/// 集配先_部署名
		/// </summary>
		[StringLength(72)]
		[DisplayName("集配先_部署名")]
		public string DELIVERY_DEPARTMENT { get; set; }

		/// <summary>
		/// 集配先_担当者名
		/// </summary>
		[StringLength(72)]
		[DisplayName("集配先_担当者名")]
		public string DELIVERY_CHARGE { get; set; }

		/// <summary>
		/// 指定日
		/// </summary>
		[DisplayName("指定日")]
		public DateTimeOffset? SPECIFIED_DATE { get; set; }

		/// <summary>
		/// 指定時間
		/// DOMAIN.CODE(KIND=00080002)
		/// </summary>
		[StringLength(8)]
		[DisplayName("指定時間")]
		public string SPECIFIED_TIME { get; set; }

		/// <summary>
		/// 便
		/// DOMAIN.CODE(KIND=00080001)
		/// </summary>
		[StringLength(8)]
		[DisplayName("便")]
		public string FLIGHT { get; set; }

		/// <summary>
		/// コメント
		/// </summary>
		[StringLength(100)]
		[DisplayName("コメント")]
		public string COMMENT { get; set; }

		#region Navigation

		/// <summary>
		/// 作業依頼ワーク
		/// </summary>
		[ForeignKey("WK_REQUEST_ID")]
		[DisplayName("作業依頼ワーク")]
		public WK_REQUEST WK_REQUEST { get; set; }

		/// <summary>
		/// 集配先マスタ
		/// </summary>
		[ForeignKey("DELIVERY_ADMIN_ID")]
		[DisplayName("集配先マスタ")]
		public DELIVERY_ADMIN DELIVERY_ADMIN { get; set; }

		#endregion // Navigation

	}
}
