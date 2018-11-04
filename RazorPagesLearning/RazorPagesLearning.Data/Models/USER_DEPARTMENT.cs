using RazorPagesLearning.Data.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ユーザー部課
    /// </summary>   
    [Serializable]
    public class USER_DEPARTMENT : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public USER_DEPARTMENT()
		{
		}

		/// <summary>
		/// ユーザーアカウントID
		/// </summary>
		[DisplayName("ユーザーアカウントID")]
		public Int64 USER_ACCOUNT_ID { get; set; }

		/// <summary>
		/// 荷主コード
		/// </summary>
		//[Key, Column(Order = 1)]
		//[Required]
		[StringLength(3)]
		[DisplayName("荷主コード")]
		public string SHIPPER_CODE { get; set; }

		/// <summary>
		/// 部課コード
		/// </summary>
		[StringLength(128)]
		[DisplayName("部課コード")]
		public string DEPARTMENT_CODE { get; set; }

        #region Navigation

        /// <summary>
        /// ユーザーアカウント
        /// </summary>
        [ForeignKey("USER_ACCOUNT_ID")]
		[DisplayName("ユーザーアカウント")]
		public USER_ACCOUNT USER_ACCOUNT { get; set; }

		/// <summary>
		/// 部課マスタ
		/// </summary>
		[DisplayName("部課マスタ")]
		public DEPARTMENT_ADMIN DEPARTMENT_ADMIN { get; set; }

		#endregion // Navigation


    }

}