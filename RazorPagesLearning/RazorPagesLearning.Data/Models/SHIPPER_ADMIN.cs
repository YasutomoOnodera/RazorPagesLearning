using RazorPagesLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 荷主マスタ
    /// </summary>
    [Serializable]
    public class SHIPPER_ADMIN : MODIFY_USER_INFORMATION
    {
        /// <summary>
        /// 荷主コード
        /// </summary>
		[Key]
		[Required]
		[StringLength(3)]
		[DisplayName("荷主コード")]
        public string SHIPPER_CODE { get; set; }

		/// <summary>
		/// 荷主名
		/// </summary>
		[Required]
		[StringLength(128)]
		[DisplayName("荷主名")]
		public string SHIPPER_NAME { get; set; }

		/// <summary>
		/// 午後設定
		/// </summary>
		[Required]
		[DisplayName("午後指定")]
		public bool AFTERNOON_FLAG { get; set; }

		/// <summary>
		/// パスワード有効期限
		/// </summary>
		[Required]
		[DisplayName("パスワード有効期限")]
		public bool PASSWORD_FLAG { get; set; }

		/// <summary>
		/// 顧客専用項目
		/// </summary>
		[Required]
		[DisplayName("顧客専用項目")]
		public bool CUSTOMER_ONLY_FLAG { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        [Required]
        [DisplayName("削除フラグ")]
        public bool DELETE_FLAG { get; set; }

        #region Navigation

        /// <summary>
        /// 部課マスタ
        /// </summary>
        [DisplayName("部課マスタ")]
		public List<DEPARTMENT_ADMIN> DEPARTMENT_ADMINs { get; set; }

		/// <summary>
		/// 集配先マスタ
		/// </summary>
		[DisplayName("集配先マスタ")]
		public List<DELIVERY_ADMIN> DELIVERY_ADMINs { get; set; }

		#endregion // Navigation
    }

}