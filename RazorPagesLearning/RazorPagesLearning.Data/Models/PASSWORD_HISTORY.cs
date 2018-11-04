using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// パスワード履歴
    /// </summary>   
    [Serializable]
    public class PASSWORD_HISTORY : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PASSWORD_HISTORY()
		{
			var now = DateTimeOffset.Now;
			this.CREATED_AT = now;
			this.UPDATED_AT = now;
		}

		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
		public Int64 ID { get; set; }

		/// <summary>
		/// ユーザーアカウントID
		/// </summary>
		[Required]
		[DisplayName("ユーザーアカウントID")]
		public Int64 USER_ACCOUNT_ID { get; set; }

		/// <summary>
		/// パスワード
		/// </summary>
		[Required]
		[DisplayName("パスワード")]
		public string PASSWORD { get; set; }

        #region Navigation

        /// <summary>
        /// ユーザーアカウント
        /// </summary>
        [ForeignKey("USER_ACCOUNT_ID")]
		[DisplayName("ユーザーアカウント")]
		public USER_ACCOUNT USER_ACCOUNT { get; set; }

		#endregion // Navigation


        /// <summary>
        /// パスワードのソルト
        /// </summary>
        public string PASSWORD_SALT { get; set; }

    }

}