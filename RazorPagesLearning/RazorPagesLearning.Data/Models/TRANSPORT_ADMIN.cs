using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// �^����Ѓ}�X�^
    /// </summary>   
    [Serializable]
    public class TRANSPORT_ADMIN : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		public TRANSPORT_ADMIN()
		{
		}

        /// <summary>
        /// �^����ЃR�[�h
        /// </summary>
		[Key]
		[Required]
		[StringLength(128)]
		[DisplayName("�^����ЃR�[�h")]
        public string CODE { get; set; }

		/// <summary>
		/// �^����Ж�
		/// </summary>
		[Required]
		[StringLength(128)]
		[DisplayName("�^����Ж�")]
		public string NAME { get; set; }

        /// <summary>
        /// �폜�t���O
        /// </summary>
        [Required]
        [DisplayName("�폜�t���O")]
        public bool DELETE_FLAG { get; set; }

        #region Navigation

        /// <summary>
        /// �ԗ��}�X�^
        /// </summary>
        [DisplayName("�ԗ��}�X�^")]
		public IEnumerable<TRUCK_ADMIN> TRUCK_ADMINs { get; set; }

		/// <summary>
		/// �W�z�˗�
		/// </summary>
		[DisplayName("�W�z�˗�")]
		public List<DELIVERY_REQUEST> DELIVERY_REQUESTs { get; set; }

		/// <summary>
		/// �W�z�˗��ڍ�
		/// </summary>
		[DisplayName("�W�z�˗��ڍ�")]
		public List<DELIVERY_REQUEST_DETAIL> DELIVERY_REQUEST_DETAILs { get; set; }

		#endregion // Navigation

    }
}