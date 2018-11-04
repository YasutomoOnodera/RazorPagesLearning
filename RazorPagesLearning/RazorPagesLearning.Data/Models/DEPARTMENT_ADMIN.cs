using RazorPagesLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ���ۃ}�X�^
    /// </summary>   
    [Serializable]
    public class DEPARTMENT_ADMIN : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		public DEPARTMENT_ADMIN()
		{
		}

        /// <summary>
        /// �׎�R�[�h
        /// </summary>
		[StringLength(3)]
		[DisplayName("�׎�R�[�h")]
        public string SHIPPER_CODE { get; set; }

        /// <summary>
        /// ���ۃR�[�h
        /// </summary>
		[StringLength(128)]
		[DisplayName("���ۃR�[�h")]
		public string DEPARTMENT_CODE { get; set; }

        /// <summary>
        /// ���ۖ�
        /// </summary>
        [Required]
		[StringLength(72)]
		[DisplayName("���ۖ�")]
		public string DEPARTMENT_NAME { get; set; }

        /// <summary>
        /// �폜�t���O
        /// </summary>
        [Required]
        [DisplayName("�폜�t���O")]
        public bool DELETE_FLAG { get; set; }

        #region Navigation

        /// <summary>
        /// �׎�}�X�^
        /// </summary>
        [ForeignKey("SHIPPER_CODE")]
		[DisplayName("�׎�}�X�^")]
		public SHIPPER_ADMIN SHIPPER_ADMIN { get; set; }

		/// <summary>
		/// ���[�U�[����
		/// </summary>
		[DisplayName("���[�U�[����")]
		public List<USER_DEPARTMENT> USER_DEPARTMENTs { get; set; }

		/// <summary>
		/// �݌�
		/// </summary>
		[DisplayName("�݌�")]
		public List<STOCK> STOCKs { get; set; }

		#endregion // Navigation

    }

}