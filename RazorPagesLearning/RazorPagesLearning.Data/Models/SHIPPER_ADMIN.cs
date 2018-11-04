using RazorPagesLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// �׎�}�X�^
    /// </summary>
    [Serializable]
    public class SHIPPER_ADMIN : MODIFY_USER_INFORMATION
    {
        /// <summary>
        /// �׎�R�[�h
        /// </summary>
		[Key]
		[Required]
		[StringLength(3)]
		[DisplayName("�׎�R�[�h")]
        public string SHIPPER_CODE { get; set; }

		/// <summary>
		/// �׎喼
		/// </summary>
		[Required]
		[StringLength(128)]
		[DisplayName("�׎喼")]
		public string SHIPPER_NAME { get; set; }

		/// <summary>
		/// �ߌ�ݒ�
		/// </summary>
		[Required]
		[DisplayName("�ߌ�w��")]
		public bool AFTERNOON_FLAG { get; set; }

		/// <summary>
		/// �p�X���[�h�L������
		/// </summary>
		[Required]
		[DisplayName("�p�X���[�h�L������")]
		public bool PASSWORD_FLAG { get; set; }

		/// <summary>
		/// �ڋq��p����
		/// </summary>
		[Required]
		[DisplayName("�ڋq��p����")]
		public bool CUSTOMER_ONLY_FLAG { get; set; }

        /// <summary>
        /// �폜�t���O
        /// </summary>
        [Required]
        [DisplayName("�폜�t���O")]
        public bool DELETE_FLAG { get; set; }

        #region Navigation

        /// <summary>
        /// ���ۃ}�X�^
        /// </summary>
        [DisplayName("���ۃ}�X�^")]
		public List<DEPARTMENT_ADMIN> DEPARTMENT_ADMINs { get; set; }

		/// <summary>
		/// �W�z��}�X�^
		/// </summary>
		[DisplayName("�W�z��}�X�^")]
		public List<DELIVERY_ADMIN> DELIVERY_ADMINs { get; set; }

		#endregion // Navigation
    }

}