using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// enum��`�̃w���p�N���X
    /// </summary>
    public static class USER_ACCOUNTExt
    {
        /// <summary>
        /// ��ʂɕ\�����閼�̂̎擾
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string DisplayName(this USER_ACCOUNT.ACCOUNT_PERMISSION target)
        {
            string[] names = { "�Ǘ���", "��Ǝ�", "�׎�(�ҏW)", "�׎�(�{��)", "�^�����" };
            return names[(int)(target - 1)];
        }

        /// <summary>
        /// �������enum�ɕϊ�����
        /// </summary>
        /// <param name="refVal"></param>
        /// <returns></returns>
        public static USER_ACCOUNT.ACCOUNT_PERMISSION toACCOUNT_PERMISSION(this string refVal)
        {
            foreach (USER_ACCOUNT.ACCOUNT_PERMISSION Value in Enum.GetValues(typeof(USER_ACCOUNT.ACCOUNT_PERMISSION)))
            {
                string name = Enum.GetName(typeof(USER_ACCOUNT.ACCOUNT_PERMISSION), Value);
                if (refVal.Trim() == name)
                {
                    return (USER_ACCOUNT.ACCOUNT_PERMISSION)Value;
                }
            }
            throw new ApplicationException("�z��O�̒l�ł��B");
        }
    }

    /// <summary>
    /// ���[�U�[�A�J�E���g
    /// </summary>   
    [Serializable]
    public class USER_ACCOUNT : MODIFY_USER_INFORMATION
    {
        #region �����g�p���

        /// <summary>
        /// �A�J�E���g����
        /// </summary>
        public enum ACCOUNT_PERMISSION
        {
            /// <summary>
            /// �Ǘ���
            /// </summary>
            Admin = 1,

            /// <summary>
            /// ��Ǝ�
            /// </summary>
            Worker = 2,

            /// <summary>
            /// �׎�(�ҏW)
            /// </summary>
            ShipperEditing = 3,

            /// <summary>
            /// �׎�(�{��)
            /// </summary>
            ShipperBrowsing = 4,

            /// <summary>
            /// �^�����
            /// </summary>
            ShippingCompany = 5
        }

        #endregion

        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public virtual Int64 ID { get; set; }

        /// <summary>
        /// ���[�U�[ID
        /// </summary>
        [Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
        [DisplayName("���[�U�[ID")]
        public virtual string USER_ID { get; set; }

        /// <summary>
        /// �p�X���[�h
        /// </summary>
        [Required]
        //[StringLength(40)]
        [DisplayName("�p�X���[�h")]
        public virtual string PASSWORD { get; set; }

        /// <summary>
        /// �p�X���[�h�̃\���g
        /// </summary>
        public virtual string PASSWORD_SALT { get; set; }

        /// <summary>
        /// ����
        /// </summary>
		[DisplayName("����")]
        public virtual ACCOUNT_PERMISSION PERMISSION { get; set; }

        /// <summary>
        /// �I�𒆉׎�R�[�h
        /// </summary>
        [MaxLength(3)]
        [DisplayName("�I�𒆉׎�R�[�h")]
        public virtual string CURRENT_SHIPPER_CODE { get; set; }

        /// <summary>
        /// �f�t�H���g���ۃR�[�h
        /// </summary>
        [StringLength(128)]
        [DisplayName("�f�t�H���g���ۃR�[�h")]
        public virtual string DEFAULT_DEPARTMENT_CODE { get; set; }

        /// <summary>
        /// �^����ЃR�[�h
        /// </summary>
        [StringLength(128)]
        [DisplayName("�^����ЃR�[�h")]
        public virtual string TRANSPORT_ADMIN_CODE { get; set; }

        /// <summary>
        /// ���[�U�[��
        /// </summary>
        [Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
        [StringLength(72, MinimumLength = 1, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
        [DisplayName("���[�U�[��")]
        public virtual string NAME { get; set; }

        /// <summary>
        /// ���[�U�[��(�J�i)
        /// </summary>
        [Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
        [DisplayName("���[�U�[��(�J�i)")]
        public virtual string KANA { get; set; }

        /// <summary>
        /// �Ж�
        /// </summary>
        [Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
        [StringLength(72, MinimumLength = 1, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
        [DisplayName("�Ж�")]
        public virtual string COMPANY { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [StringLength(72)]
        [DisplayName("������")]
        public virtual string DEPARTMENT { get; set; }

        /// <summary>
        /// �X�֔ԍ�
        /// </summary>
        [Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
        [RegularExpression(@"\d{7}$", ErrorMessage = "{0}�͂V���̐����œ��͂��Ă��������B")]
        [DisplayName("�X�֔ԍ�")]
        public virtual string ZIPCODE { get; set; }

        /// <summary>
        /// �Z��1
        /// </summary>
        [Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
        [StringLength(72, MinimumLength = 1, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
        [DisplayName("�Z��1")]
        public virtual string ADDRESS1 { get; set; }

        /// <summary>
        /// �Z��2
        /// </summary>
        [StringLength(72)]
        [DisplayName("�Z��2")]
        public virtual string ADDRESS2 { get; set; }

        /// <summary>
        /// TEL
        /// </summary>
        [Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
        [RegularExpression(@"\d{0,14}$", ErrorMessage = "{0}��0����14���̐����œ��͂��Ă��������B")]
        [DisplayName("TEL")]
        public virtual string TEL { get; set; }

        /// <summary>
        /// FAX
        /// </summary>
        [Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
        [RegularExpression(@"\d{0,14}$", ErrorMessage = "{0}��0����14���̐����œ��͂��Ă��������B")]
        [DisplayName("FAX")]
        public virtual string FAX { get; set; }

        /// <summary>
        /// ���[���A�h���X
        /// </summary>
        [Required(ErrorMessage = "{0}�ɒl����͂��Ă��������B")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "{0}��{1}�����ȏ�{2}�����ȓ��œ��͂��Ă��������B")]
        [EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
        [DisplayName("���[���A�h���X")]
        public virtual string MAIL { get; set; }

        /// <summary>
        /// �f�t�H���g�W�z��}�X�^ID
        /// </summary>
        [DisplayName("�f�t�H���g�W�z��}�X�^ID")]
        public virtual Int64 DEFAULT_DELIVERY_ADMIN_ID { get; set; }

        /// <summary>
        /// �����z�M���[��1
        /// </summary>
		[StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
        [DisplayName("�����z�M���[��1")]
        public virtual string MAIL1 { get; set; }

        /// <summary>
        /// �����z�M���[��2
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
        [DisplayName("�����z�M���[��2")]
        public virtual string MAIL2 { get; set; }

        /// <summary>
        /// �����z�M���[��3
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
        [DisplayName("�����z�M���[��3")]
        public virtual string MAIL3 { get; set; }

        /// <summary>
        /// �����z�M���[��4
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
        [DisplayName("�����z�M���[��4")]
        public virtual string MAIL4 { get; set; }

        /// <summary>
        /// �����z�M���[��5
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
        [DisplayName("�����z�M���[��5")]
        public virtual string MAIL5 { get; set; }

        /// <summary>
        /// �����z�M���[��6
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
        [DisplayName("�����z�M���[��6")]
        public virtual string MAIL6 { get; set; }

        /// <summary>
        /// �����z�M���[��7
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
        [DisplayName("�����z�M���[��7")]
        public virtual string MAIL7 { get; set; }

        /// <summary>
        /// �����z�M���[��8
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
        [DisplayName("�����z�M���[��8")]
        public virtual string MAIL8 { get; set; }

        /// <summary>
        /// �����z�M���[��9
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
        [DisplayName("�����z�M���[��9")]
        public virtual string MAIL9 { get; set; }

        /// <summary>
        /// �����z�M���[��10
        /// </summary>
        [StringLength(50)]
        [EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
        [DisplayName("�����z�M���[��10")]
        public virtual string MAIL10 { get; set; }

        /// <summary>
        /// �p�X���[�h�ύX��
        /// </summary>
        [DisplayName("�p�X���[�h�ύX��")]
        public virtual DateTimeOffset? PASSWORD_UPDATED_AT { get; set; }

        /// <summary>
        /// ���O�C���L���t���O
        /// </summary>
        [DisplayName("���O�C���L���t���O")]
        public virtual bool LOGIN_ENABLE_FLAG { get; set; }

        /// <summary>
        /// �p�X���[�h�������t���O
        /// </summary>
        [DisplayName("�p�X���[�h�������t���O")]
        public virtual bool EXPIRE_FLAG { get; set; }

        /// <summary>
        /// �p�X���[�h�ύX�v��
        /// </summary>
        [DisplayName("�p�X���[�h�ύX�v��")]
        public virtual bool PASSWORD_CHANGE_REQUEST { get; set; }

        /// <summary>
        /// �m�F�_�C�A���O�s�v�t���O
        /// </summary>
		[DisplayName("�m�F�_�C�A���O�s�v�t���O")]
        public virtual bool CONFIRM_FLAG { get; set; }

        /// <summary>
        /// �ŏI���O�C������
        /// </summary>
        [DisplayName("�ŏI���O�C������")]
        public virtual DateTimeOffset? LOGINED_AT { get; set; }

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
        public virtual TRANSPORT_ADMIN TRANSPORT_ADMIN { get; set; }

        /// <summary>
        /// ���[�U�[���������镔�ۖ�
        /// </summary>
        [DisplayName("���[�U�[����")]
        public virtual List<USER_DEPARTMENT> USER_DEPARTMENTs { get; set; }

        /// <summary>
        /// �\���ݒ�
        /// </summary>
        [DisplayName("�\���ݒ�")]
        public virtual List<USER_DISPLAY_SETTING> DISPLAY_SETTINGs { get; set; }

        /// <summary>
        /// �p�X���[�h����
        /// </summary>
        [DisplayName("�p�X���[�h����")]
        public virtual List<PASSWORD_HISTORY> PASSWORD_HISTORYs { get; set; }

        /// <summary>
        /// ���[�U�[����
        /// </summary>
        [DisplayName("���[�U�[����")]
        public virtual List<USER_ITEM> USER_ITEMs { get; set; }

        /// <summary>
        /// �E�H�b�`���X�g
        /// </summary>
        [DisplayName("�E�H�b�`���X�g")]
        public virtual List<WATCHLIST> WATCHLISTs { get; set; }

        /// <summary>
        /// ��ƈ˗��ꗗ
        /// </summary>
        [DisplayName("��ƈ˗��ꗗ")]
        public virtual List<REQUEST_LIST> REQUEST_LISTs { get; set; }

        /// <summary>
        /// ��ƈ˗����[�N
        /// </summary>
        [DisplayName("��ƈ˗����[�N")]
        public virtual List<WK_REQUEST> WK_REQUESTs { get; set; }

        /// <summary>
        /// ���[�U�[�W�z��
        /// </summary>
        [DisplayName("���[�U�[�W�z��")]
        public virtual List<USER_DELIVERY> USER_DELIVERies { get; set; }

        /// <summary>
        /// �K���ۗ����̃��[�U�[�A�J�E���g���
        /// </summary>
        [ForeignKey("USER_ACCOUNT_ID")]
        public virtual WK_USER_ACCOUNT WK_USER_ACCOUNT { get; set; }

        #endregion // Navigation


        /// <summary>
        /// �e�[�u���ɂ�����`�F�b�N�{�b�N�X�I�����
        /// </summary>
        public virtual List<WK_TABLE_SELECTION_SETTING> WK_TABLE_SELECTION_SETTINGs { get; set; }

        /// <summary>
        /// �e�[�u���ɂ�����y�[�W�l�[�V�����ݒ���
        /// </summary>
        public virtual List<WK_TABLE_PAGINATION_SETTING> WK_TABLE_PAGINATION_SETTINGs { get; set; }

    }
}