using RazorPagesLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 部課マスタ
    /// </summary>   
    [Serializable]
    public class DEPARTMENT_ADMIN : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DEPARTMENT_ADMIN()
		{
		}

        /// <summary>
        /// 荷主コード
        /// </summary>
		[StringLength(3)]
		[DisplayName("荷主コード")]
        public string SHIPPER_CODE { get; set; }

        /// <summary>
        /// 部課コード
        /// </summary>
		[StringLength(128)]
		[DisplayName("部課コード")]
		public string DEPARTMENT_CODE { get; set; }

        /// <summary>
        /// 部課名
        /// </summary>
        [Required]
		[StringLength(72)]
		[DisplayName("部課名")]
		public string DEPARTMENT_NAME { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        [Required]
        [DisplayName("削除フラグ")]
        public bool DELETE_FLAG { get; set; }

        #region Navigation

        /// <summary>
        /// 荷主マスタ
        /// </summary>
        [ForeignKey("SHIPPER_CODE")]
		[DisplayName("荷主マスタ")]
		public SHIPPER_ADMIN SHIPPER_ADMIN { get; set; }

		/// <summary>
		/// ユーザー部課
		/// </summary>
		[DisplayName("ユーザー部課")]
		public List<USER_DEPARTMENT> USER_DEPARTMENTs { get; set; }

		/// <summary>
		/// 在庫
		/// </summary>
		[DisplayName("在庫")]
		public List<STOCK> STOCKs { get; set; }

		#endregion // Navigation

    }

}