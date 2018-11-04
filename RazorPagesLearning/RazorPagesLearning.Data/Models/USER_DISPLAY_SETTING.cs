using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ���[�U�[�\���ݒ�
    /// </summary>
    [Serializable]
    public class USER_DISPLAY_SETTING : COMMON_MANAGEMENT_INFORMATION
    {
		/// <summary>
		/// ���ID
		/// </summary>
		public enum SCREEN
		{
			/// <summary>
			/// 03.�����E�{��(���������w���A��������)
			/// </summary>
			Search = 3
		}

		/// <summary>
		/// ���ID
		/// </summary>
		[DisplayName("���ID")]
		public SCREEN SCREEN_ID { get; set; }

        /// <summary>
        /// ���[�U�[�A�J�E���gID
        /// </summary>
        [DisplayName("���[�U�[�A�J�E���gID")]
		public Int64 USER_ACCOUNT_ID { get; set; }

		/// <summary>
		/// ��(����)
		/// </summary>
		[StringLength(128)]
		[DisplayName("��(����)")]
		public string PHYS_COLUMN_NAME { get; set; }

		/// <summary>
		/// ��(�_��)
		/// </summary>
		[StringLength(128)]
		[DisplayName("��(�_��)")]
		public string LOGI_COLUMN_NAME { get; set; }

		/// <summary>
		/// �I�����
		/// </summary>
		[Required]
		[DisplayName("�I�����")]
		public bool CHECK_STATUS { get; set; }

		/// <summary>
		/// �\����(�����l)
		/// </summary>
		[Required]
		[DisplayName("�\����(�����l)")]
		public int DEFAULT_ORDER { get; set; }

		/// <summary>
		/// �\����
		/// </summary>
		[DisplayName("�\����")]
		public int? DISPLAY_ORDER { get; set; }

		/// <summary>
		/// �\�[�g
		/// </summary>
		[DisplayName("�\�[�g")]
		public int? SORT { get; set; }

		#region Navigation

		/// <summary>
		/// ���[�U�[�A�J�E���g
		/// </summary>
		[ForeignKey("USER_ACCOUNT_ID")]
		[DisplayName("���[�U�[�A�J�E���g")]
		public USER_ACCOUNT USER_ACCOUNT { get; set; }

		#endregion // Navigation

    }

}