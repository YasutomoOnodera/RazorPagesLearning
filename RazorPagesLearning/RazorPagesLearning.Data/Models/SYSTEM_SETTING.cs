using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// システム設定
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
        /// メールサーバー
        /// </summary>
        [StringLength(512)]
		[DisplayName("メールサーバー")]
		public string MAIL_SERVER { get; set; }

        /// <summary>
        /// メールサーバーポート
        /// </summary>
        [DisplayName("メールサーバーポート")]
        public int MAIL_PORT { get; set; }

        /// <summary>
        /// 管理者メールアドレス
        /// </summary>
        [EmailAddressAttribute(ErrorMessage = "メールアドレス形式で入力してください。")]
        [DisplayName("管理者メールアドレス")]
        public string ADMIN_MAIL { get; set; }

        #region Navigation

        #endregion // Navigation

    }
}