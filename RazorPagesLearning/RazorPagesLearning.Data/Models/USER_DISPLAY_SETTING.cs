using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ユーザー表示設定
    /// </summary>
    [Serializable]
    public class USER_DISPLAY_SETTING : COMMON_MANAGEMENT_INFORMATION
    {
		/// <summary>
		/// 画面ID
		/// </summary>
		public enum SCREEN
		{
			/// <summary>
			/// 03.検索・閲覧(検索条件指示、検索結果)
			/// </summary>
			Search = 3
		}

		/// <summary>
		/// 画面ID
		/// </summary>
		[DisplayName("画面ID")]
		public SCREEN SCREEN_ID { get; set; }

        /// <summary>
        /// ユーザーアカウントID
        /// </summary>
        [DisplayName("ユーザーアカウントID")]
		public Int64 USER_ACCOUNT_ID { get; set; }

		/// <summary>
		/// 列名(物理)
		/// </summary>
		[StringLength(128)]
		[DisplayName("列名(物理)")]
		public string PHYS_COLUMN_NAME { get; set; }

		/// <summary>
		/// 列名(論理)
		/// </summary>
		[StringLength(128)]
		[DisplayName("列名(論理)")]
		public string LOGI_COLUMN_NAME { get; set; }

		/// <summary>
		/// 選択状態
		/// </summary>
		[Required]
		[DisplayName("選択状態")]
		public bool CHECK_STATUS { get; set; }

		/// <summary>
		/// 表示順(初期値)
		/// </summary>
		[Required]
		[DisplayName("表示順(初期値)")]
		public int DEFAULT_ORDER { get; set; }

		/// <summary>
		/// 表示順
		/// </summary>
		[DisplayName("表示順")]
		public int? DISPLAY_ORDER { get; set; }

		/// <summary>
		/// ソート
		/// </summary>
		[DisplayName("ソート")]
		public int? SORT { get; set; }

		#region Navigation

		/// <summary>
		/// ユーザーアカウント
		/// </summary>
		[ForeignKey("USER_ACCOUNT_ID")]
		[DisplayName("ユーザーアカウント")]
		public USER_ACCOUNT USER_ACCOUNT { get; set; }

		#endregion // Navigation

    }

}