using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// �K���ۗ����̃��[�U�[�A�J�E���g���
    /// </summary>   
    [Serializable]
    public class WK_USER_ACCOUNT : MODIFY_USER_INFORMATION
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public Int64 ID { get; set; }

        /// <summary>
        /// �X�V�ΏۂƂȂ郆�[�U�[�A�J�E���g���
        /// </summary>
        [Required]
        public Int64 USER_ACCOUNT_ID { get; set; }

        /// <summary>
        /// ���[�U�[ID
        /// </summary>
        [Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
		[StringLength(40, MinimumLength = 1, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[DisplayName("���[�U�[ID")]
		public string USER_ID { get; set; }

		/// <summary>
		/// �p�X���[�h
        /// 
        /// [Memo]
        /// �X�V�ۗ����̏ꍇ�A
        /// �p�X���[�h�̍X�V�𔺂�Ȃ��ꍇ�����邽�߁A
        /// �p�X���[�h�v���͕K�{�Ƃ��Ȃ��B
        /// 
		/// </summary>
		public string PASSWORD { get; set; }

        /// <summary>
        /// USER_ACCOUNT�e�[�u���Ƃ̓������Ƀp�X���[�h�̍X�V���K�v
        /// </summary>
        public bool IS_NEED_PASSWORD_UPDATE { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [DisplayName("����")]
        public USER_ACCOUNT.ACCOUNT_PERMISSION PERMISSION { get; set; }

        /// <summary>
        /// �I�𒆉׎�R�[�h
        /// </summary>
        [StringLength(3)]
        [DisplayName("�I�𒆉׎�R�[�h")]
        public string CURRENT_SHIPPER_CODE { get; set; }

        /// <summary>
        /// �f�t�H���g���ۃR�[�h
        /// </summary>
        [StringLength(128)]
        [DisplayName("�f�t�H���g���ۃR�[�h")]
        public string DEFAULT_DEPARTMENT_CODE { get; set; }

        /// <summary>
        /// �^����ЃR�[�h
        /// </summary>
        [StringLength(128)]
        [DisplayName("�^����ЃR�[�h")]
        public string TRANSPORT_ADMIN_CODE { get; set; }

        /// <summary>
        /// ���[�U�[��
        /// </summary>
        [Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
		[StringLength(72, MinimumLength = 1, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[DisplayName("���[�U�[��")]
		public string NAME { get; set; }

		/// <summary>
		/// ���[�U�[��(�J�i)
		/// </summary>
		[Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
		[StringLength(50, MinimumLength = 1, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[DisplayName("���[�U�[��(�J�i)")]
		public string KANA { get; set; }

        #region Navigation

		/// <summary>
		/// ���[�U�[���������镔�ۖ�
		/// </summary>
		[DisplayName("���[�U�[����")]        
		public List<WK_USER_DEPARTMENT> USER_DEPARTMENTs { get; set; }

		#endregion // Navigation

    }
}