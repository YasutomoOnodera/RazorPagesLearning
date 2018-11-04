using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// �ԗ��}�X�^
    /// </summary>   
    ////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class TRUCK_ADMIN : MODIFY_USER_INFORMATION
    {
		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public TRUCK_ADMIN()
		{
		}

		/// <summary>
		/// �^����ЃR�[�h
		/// </summary>
		[StringLength(128)]
		[DisplayName("�^����ЃR�[�h")]
		public string TRANSPORT_ADMIN_CODE { get; set; }

		/// <summary>
		/// �ԗ��Ǘ��ԍ�
		/// �Ԕ�1�`10�̂ǂ��ɕ\�����邩���w�肷��ԍ�
		/// </summary>
		[DisplayName("�ԗ��Ǘ��ԍ�")]
		public int TRUCK_MANAGE_NUMBER { get; set; }

		/// <summary>
		/// �Ԕ�
		/// </summary>
		[StringLength(4)]
		[DisplayName("�Ԕ�")]
		public string NUMBER { get; set; }

		/// <summary>
		/// �S����
		/// </summary>
		[StringLength(20)]
		[DisplayName("�S����")]
		public string CHARGE { get; set; }

        /// <summary>
        /// �폜�t���O
        /// </summary>
        [Required]
        [DisplayName("�폜�t���O")]
        public bool DELETE_FLAG { get; set; }

		#region Navigation

		/// <summary>
		/// �^����Ѓ}�X�^
		/// </summary>
		[ForeignKey("TRANSPORT_ADMIN_CODE")]
		[DisplayName("�^����Ѓ}�X�^")]
		public TRANSPORT_ADMIN TRANSPORT_ADMIN { get; set; }

		#endregion // Navigation

    }

}