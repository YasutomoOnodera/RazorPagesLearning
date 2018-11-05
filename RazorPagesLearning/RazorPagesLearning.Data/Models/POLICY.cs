using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    ///セキュリティポリシー
    /// </summary>
    [Serializable]
    public class POLICY : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// パスワードポリシー
		/// </summary>
		public enum PASSWORD_POLICY
		{
			/// <summary>
			/// パスワード桁数
			/// </summary>
			Digit = 1,

			/// <summary>
			/// パスワード変更間隔日数
			/// </summary>
			Interval = 2,

			/// <summary>
			/// パスワード変更通知日数
			/// </summary>
			Notify = 3,

			/// <summary>
			/// パスワード再利用禁止回数
			/// </summary>
			Reuse = 4
		}

		/// <summary>
		/// 名前
		/// </summary>
		[Key]
		[Required]
		[DisplayName("名前")]
		public PASSWORD_POLICY NAME { get; set; }

		/// <summary>
		/// 数値
		/// </summary>
		[Required]
		[DisplayName("数値")]
		public int VALUE { get; set; }

    }
}