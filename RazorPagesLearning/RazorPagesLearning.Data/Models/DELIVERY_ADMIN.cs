using RazorPagesLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// �W�z��}�X�^
    /// </summary>   
    [Serializable]
    public class DELIVERY_ADMIN : MODIFY_USER_INFORMATION
    {
        /// <summary>
        /// ID
        /// </summary>
		[Key]
		[Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 ID { get; set; }

		/// <summary>
		/// �W�z��R�[�h
		/// WMS����Ԃ��Ă�����A�W�z��R�[�h(DELIVERY_CODE)�ɒl������
		/// </summary>
		[StringLength(8)]
		[DisplayName("�W�z��R�[�h")]
		public string DELIVERY_CODE { get; set; }

		/// <summary>
		/// �׎�R�[�h
		/// </summary>
		[Required]
		[StringLength(3)]
		[DisplayName("�׎�R�[�h")]
		public string SHIPPER_CODE { get; set; }

		/// <summary>
		/// �Ж�
		/// </summary>
		//[Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
		[StringLength(72, MinimumLength = 1, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[DisplayName("�Ж�")]
		public string COMPANY { get; set; }

		/// <summary>
		/// ������
		/// </summary>
		[StringLength(72)]
		[DisplayName("������")]
		public string DEPARTMENT { get; set; }

		/// <summary>
		/// �S���Җ�
		/// </summary>
		[StringLength(72)]
		[DisplayName("�S���Җ�")]
		public string CHARGE_NAME { get; set; }

		/// <summary>
		/// �X�֔ԍ�
		/// </summary>
		[StringLength(8)]
		[RegularExpression(@"\d{3}-\d{4}$", ErrorMessage = "{0}��3��-4���̐����œ��͂��Ă��������B")]
		[DisplayName("�X�֔ԍ�")]
		public string ZIPCODE { get; set; }

		/// <summary>
		/// �Z��1
		/// </summary>
		[Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
		[StringLength(72, MinimumLength = 1, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[DisplayName("�Z��1")]
		public string ADDRESS1 { get; set; }

		/// <summary>
		/// �Z��2
		/// </summary>
		[StringLength(72)]
		[DisplayName("�Z��2")]
		public string ADDRESS2 { get; set; }

		/// <summary>
		/// TEL
		/// </summary>
		[Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
		[RegularExpression(@"\d{0,14}$", ErrorMessage = "{0}��0����14���̐����œ��͂��Ă��������B")]
		[DisplayName("TEL")]
		public string TEL { get; set; }

		/// <summary>
		/// FAX
		/// </summary>
		[RegularExpression(@"\d{0,14}$", ErrorMessage = "{0}��0����14���̐����œ��͂��Ă��������B")]
		[DisplayName("FAX")]
		public string FAX { get; set; }

		/// <summary>
		/// ���[���A�h���X
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
		[DisplayName("���[���A�h���X")]
		public string MAIL { get; set; }

		/// <summary>
		/// �f�t�H���g�փR�[�h
		/// </summary>
		[StringLength(8)]
        [DisplayName("�f�t�H���g�փR�[�h")]
        public string DEFAULT_FLIGHT_CODE { get; set; }

		/// <summary>
		/// �����z�M���[��1
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
		[DisplayName("�����z�M���[��1")]
		public string MAIL1 { get; set; }

		/// <summary>
		/// �����z�M���[��2
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
		[DisplayName("�����z�M���[��2")]
		public string MAIL2 { get; set; }

		/// <summary>
		/// �����z�M���[��3
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
		[DisplayName("�����z�M���[��3")]
		public string MAIL3 { get; set; }

		/// <summary>
		/// �����z�M���[��4
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
		[DisplayName("�����z�M���[��4")]
		public string MAIL4 { get; set; }

		/// <summary>
		/// �����z�M���[��5
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
		[DisplayName("�����z�M���[��5")]
		public string MAIL5 { get; set; }

		/// <summary>
		/// �����z�M���[��6
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
		[DisplayName("�����z�M���[��6")]
		public string MAIL6 { get; set; }

		/// <summary>
		/// �����z�M���[��7
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
		[DisplayName("�����z�M���[��7")]
		public string MAIL7 { get; set; }

		/// <summary>
		/// �����z�M���[��8
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
		[DisplayName("�����z�M���[��8")]
		public string MAIL8 { get; set; }

		/// <summary>
		/// �����z�M���[��9
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
		[DisplayName("�����z�M���[��9")]
		public string MAIL9 { get; set; }

		/// <summary>
		/// �����z�M���[��10
		/// </summary>
		[StringLength(50, MinimumLength = 6, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
		[EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
		[DisplayName("�����z�M���[��10")]
		public string MAIL10 { get; set; }

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
        /// ���[�U�[�W�z��
        /// </summary>
        [ForeignKey("ID")]
        [DisplayName("���[�U�[�W�z��")]
		public List<USER_DELIVERY> USER_DELIVERies { get; set; }

		#endregion // Navigation

    }
}