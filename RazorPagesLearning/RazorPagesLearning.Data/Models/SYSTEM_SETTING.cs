using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// �V�X�e���ݒ�
    /// </summary>
    [Serializable]
    public class SYSTEM_SETTING : MODIFY_USER_INFORMATION
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public Int64 ID { get; set; }

        /// <summary>
        /// ���[���T�[�o�[
        /// </summary>
        [StringLength(512)]
		[DisplayName("���[���T�[�o�[")]
		public string MAIL_SERVER { get; set; }

        /// <summary>
        /// ���[���T�[�o�[�|�[�g
        /// </summary>
        [DisplayName("���[���T�[�o�[�|�[�g")]
        public int MAIL_PORT { get; set; }

        /// <summary>
        /// �Ǘ��҃��[���A�h���X
        /// </summary>
        [EmailAddressAttribute(ErrorMessage = "���[���A�h���X�`���œ��͂��Ă��������B")]
        [DisplayName("�Ǘ��҃��[���A�h���X")]
        public string ADMIN_MAIL { get; set; }

        #region Navigation

        #endregion // Navigation

    }
}