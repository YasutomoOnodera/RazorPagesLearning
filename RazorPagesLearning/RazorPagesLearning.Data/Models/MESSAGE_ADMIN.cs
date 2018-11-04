using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// メッセージマスタ
    /// </summary>
    [Serializable]
    public class MESSAGE_ADMIN : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// 種別
		/// </summary>
		public enum MESSAGE_KIND
		{
			/// <summary>
			/// ログイン
			/// </summary>
			Login = 1,

			/// <summary>
			/// ホーム
			/// </summary>
			Home = 2
		}


		/// <summary>
		/// 種別
		/// </summary>
		[Key]
		[Required]
		[DisplayName("種別")]
		public MESSAGE_KIND KIND { get; set; }

		/// <summary>
		/// メッセージ
		/// </summary>
		[DisplayName("メッセージ")]
		public string MESSAGE { get; set; }

    }

}