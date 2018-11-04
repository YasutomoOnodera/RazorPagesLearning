using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// �K���ۗ����̃��[�U�[����
    /// </summary>   
    [Serializable]
    public class WK_USER_DEPARTMENT : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		public WK_USER_DEPARTMENT()
		{
		}

		/// <summary>
		/// ���[�U�[�A�J�E���gID
		/// </summary>
		[DisplayName("���[�U�[�A�J�E���gID")]
		public Int64 USER_ACCOUNT_ID { get; set; }

		/// <summary>
		/// �׎�R�[�h
		/// </summary>
		//[Key, Column(Order = 1)]
		//[Required]
		[StringLength(3)]
		[DisplayName("�׎�R�[�h")]
		public string SHIPPER_CODE { get; set; }

		/// <summary>
		/// ���ۃR�[�h
		/// </summary>
		[StringLength(128)]
		[DisplayName("���ۃR�[�h")]
		public string DEPARTMENT_CODE { get; set; }

        #region Navigation

        /// <summary>
        /// ���[�U�[�A�J�E���g
        /// </summary>
        [ForeignKey("USER_ACCOUNT_ID")]
        [DisplayName("���[�U�[�A�J�E���g")]
        public WK_USER_ACCOUNT USER_ACCOUNT { get; set; }

        #endregion // Navigation

    }

}