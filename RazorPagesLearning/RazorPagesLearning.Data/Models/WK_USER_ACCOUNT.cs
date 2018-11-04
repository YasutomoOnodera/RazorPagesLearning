using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 適応保留中のユーザーアカウント情報
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
        /// 更新対象となるユーザーアカウント情報
        /// </summary>
        [Required]
        public Int64 USER_ACCOUNT_ID { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
		[StringLength(40, MinimumLength = 1, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[DisplayName("ユーザーID")]
		public string USER_ID { get; set; }

		/// <summary>
		/// パスワード
        /// 
        /// [Memo]
        /// 更新保留中の場合、
        /// パスワードの更新を伴わない場合もあるため、
        /// パスワード要求は必須としない。
        /// 
		/// </summary>
		public string PASSWORD { get; set; }

        /// <summary>
        /// USER_ACCOUNTテーブルとの同期時にパスワードの更新も必要
        /// </summary>
        public bool IS_NEED_PASSWORD_UPDATE { get; set; }

        /// <summary>
        /// 権限
        /// </summary>
        [DisplayName("権限")]
        public USER_ACCOUNT.ACCOUNT_PERMISSION PERMISSION { get; set; }

        /// <summary>
        /// 選択中荷主コード
        /// </summary>
        [StringLength(3)]
        [DisplayName("選択中荷主コード")]
        public string CURRENT_SHIPPER_CODE { get; set; }

        /// <summary>
        /// デフォルト部課コード
        /// </summary>
        [StringLength(128)]
        [DisplayName("デフォルト部課コード")]
        public string DEFAULT_DEPARTMENT_CODE { get; set; }

        /// <summary>
        /// 運送会社コード
        /// </summary>
        [StringLength(128)]
        [DisplayName("運送会社コード")]
        public string TRANSPORT_ADMIN_CODE { get; set; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        [Required(ErrorMessage = "{0}に値を入力してください。")]
		[StringLength(72, MinimumLength = 1, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[DisplayName("ユーザー名")]
		public string NAME { get; set; }

		/// <summary>
		/// ユーザー名(カナ)
		/// </summary>
		[Required(ErrorMessage = "{0}に値を入力してください。")]
		[StringLength(50, MinimumLength = 1, ErrorMessage = "{0}は{1}文字以上{2}文字以内で入力してください。")]
		[DisplayName("ユーザー名(カナ)")]
		public string KANA { get; set; }

        #region Navigation

		/// <summary>
		/// ユーザーが所属する部課名
		/// </summary>
		[DisplayName("ユーザー部課")]        
		public List<WK_USER_DEPARTMENT> USER_DEPARTMENTs { get; set; }

		#endregion // Navigation

    }
}