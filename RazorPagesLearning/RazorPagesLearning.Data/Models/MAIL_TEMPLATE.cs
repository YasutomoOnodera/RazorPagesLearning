using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// メールテンプレート
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
        /// コード
        /// </summary>
        [StringLength(128)]
        [DisplayName("メールテンプレート識別コード")]
        public string MAIL_TEMPLATE_CODE { get; set; }

        /// <summary>
        /// テンプレート本文
        /// </summary>
		[DisplayName("メールテンプレート")]
		public string TEXT { get; set; }

    }
}