using RazorPagesLearning.Data.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ユーザー集配先
    /// </summary>   
    [Serializable]
    public class USER_DELIVERY : COMMON_MANAGEMENT_INFORMATION
    {
        /// <summary>
        /// 集配先マスタID
        /// </summary>
		[DisplayName("集配先マスタID")]
        public Int64 DELIVERY_ADMIN_ID { get; set; }

        /// <summary>
        /// ユーザーアカウントID
        /// </summary>
        [Required]
        [DisplayName("ユーザーアカウントID")]
		public Int64 USER_ACCOUNT_ID { get; set; }

		#region Navigation

		/// <summary>
		/// ユーザーアカウント
		/// </summary>
		[ForeignKey("USER_ACCOUNT_ID")]
		[DisplayName("ユーザーアカウント")]
		public USER_ACCOUNT USER_ACCOUNT { get; set; }

		/// <summary>
		/// 集配先マスタ
		/// </summary>
		[ForeignKey("DELIVERY_ADMIN_ID")]
		[DisplayName("集配先マスタ")]
		public DELIVERY_ADMIN DELIVERY_ADMIN { get; set; }

		#endregion // Navigation

    }

}