using RazorPagesLearning.Data.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// �E�H�b�`���X�g
    /// </summary>
    [Serializable]
    public class WATCHLIST : COMMON_MANAGEMENT_INFORMATION
    {
		/// <summary>
		/// �݌�ID
		/// </summary>
        [DisplayName("�݌�ID")]
        public Int64 STOCK_ID { get; set; }

        /// <summary>
        /// ���[�U�[�A�J�E���gID
        /// </summary>
        [Required]
        [DisplayName("���[�U�[�A�J�E���gID")]
		public Int64 USER_ACCOUNT_ID { get; set; }

		#region Navigation

		/// <summary>
		/// �݌�
		/// </summary>
		[ForeignKey("STOCK_ID")]
		[DisplayName("�݌�")]
		public STOCK STOCK { get; set; }

		/// <summary>
		/// ���[�U�[�A�J�E���g
		/// </summary>
		[ForeignKey("USER_ACCOUNT_ID")]
		[DisplayName("���[�U�[�A�J�E���g")]
		public USER_ACCOUNT USER_ACCOUNT { get; set; }

		#endregion // Navigation

    }

}