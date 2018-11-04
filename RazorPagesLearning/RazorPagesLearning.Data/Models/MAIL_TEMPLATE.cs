using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ���[���e���v���[�g
    /// </summary>
    [Serializable]
    public class MAIL_TEMPLATE : MODIFY_USER_INFORMATION
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public Int64 ID { get; set; }

        /// <summary>
        /// �R�[�h
        /// </summary>
        [StringLength(128)]
        [DisplayName("���[���e���v���[�g���ʃR�[�h")]
        public string MAIL_TEMPLATE_CODE { get; set; }

        /// <summary>
        /// �e���v���[�g�{��
        /// </summary>
		[DisplayName("���[���e���v���[�g")]
		public string TEXT { get; set; }

    }
}