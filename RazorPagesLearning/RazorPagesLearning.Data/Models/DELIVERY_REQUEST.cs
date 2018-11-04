using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// �W�z�˗�
    /// </summary>
    [Serializable]
    public class DELIVERY_REQUEST : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// �^����ЃR�[�h
		/// </summary>
		[StringLength(128)]
		[DisplayName("�^����ЃR�[�h")]
		public string TRANSPORT_ADMIN_CODE { get; set; }

		/// <summary>
		/// �W�z�˗��ԍ�
		/// </summary>
		[StringLength(128)]
		[DisplayName("�W�z�˗��ԍ�")]
		public string DELIVERY_REQUEST_NUMBER { get; set; }

		/// <summary>
		/// �X�e�[�^�X
		/// DOMAIN.CODE(KIND=00100000)
		/// </summary>
		[Required]
		[StringLength(8)]
		[DisplayName("�X�e�[�^�X")]
		public string STATUS { get; set; }

		/// <summary>
		/// �W�z��
		/// YYYY/MM/DD AM or PM
		/// </summary>
		[DisplayName("�W�z��")]
		public DateTimeOffset? DELIVERY_DATE { get; set; }

		/// <summary>
		/// �m�����
		/// </summary>
		[DisplayName("�m�����")]
		public DateTimeOffset? CONFIRM_DATETIME { get; set; }

		/// <summary>
		/// ���яC������
		/// </summary>
		[DisplayName("���яC������")]
		public DateTimeOffset? CORRECTION_DATETIME { get; set; }

        #region Navigation

        /// <summary>
        /// �^����Ѓ}�X�^
        /// </summary>
        [ForeignKey("TRANSPORT_ADMIN_CODE")]
		[DisplayName("�^����Ѓ}�X�^")]
		public TRANSPORT_ADMIN TRANSPORT_ADMIN { get; set; }

		/// <summary>
		/// �W�z�˗��ڍ�
		/// </summary>
		[DisplayName("�W�z�˗��ڍ�")]
		public List<DELIVERY_REQUEST_DETAIL> DELIVERY_REQUEST_DETAILs { get; set; }

		#endregion // Navigation

    }
}