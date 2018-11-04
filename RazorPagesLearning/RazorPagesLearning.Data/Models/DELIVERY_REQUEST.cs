using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 集配依頼
    /// </summary>
    [Serializable]
    public class DELIVERY_REQUEST : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// 運送会社コード
		/// </summary>
		[StringLength(128)]
		[DisplayName("運送会社コード")]
		public string TRANSPORT_ADMIN_CODE { get; set; }

		/// <summary>
		/// 集配依頼番号
		/// </summary>
		[StringLength(128)]
		[DisplayName("集配依頼番号")]
		public string DELIVERY_REQUEST_NUMBER { get; set; }

		/// <summary>
		/// ステータス
		/// DOMAIN.CODE(KIND=00100000)
		/// </summary>
		[Required]
		[StringLength(8)]
		[DisplayName("ステータス")]
		public string STATUS { get; set; }

		/// <summary>
		/// 集配日
		/// YYYY/MM/DD AM or PM
		/// </summary>
		[DisplayName("集配日")]
		public DateTimeOffset? DELIVERY_DATE { get; set; }

		/// <summary>
		/// 確定日時
		/// </summary>
		[DisplayName("確定日時")]
		public DateTimeOffset? CONFIRM_DATETIME { get; set; }

		/// <summary>
		/// 実績修正日時
		/// </summary>
		[DisplayName("実績修正日時")]
		public DateTimeOffset? CORRECTION_DATETIME { get; set; }

        #region Navigation

        /// <summary>
        /// 運送会社マスタ
        /// </summary>
        [ForeignKey("TRANSPORT_ADMIN_CODE")]
		[DisplayName("運送会社マスタ")]
		public TRANSPORT_ADMIN TRANSPORT_ADMIN { get; set; }

		/// <summary>
		/// 集配依頼詳細
		/// </summary>
		[DisplayName("集配依頼詳細")]
		public List<DELIVERY_REQUEST_DETAIL> DELIVERY_REQUEST_DETAILs { get; set; }

		#endregion // Navigation

    }
}