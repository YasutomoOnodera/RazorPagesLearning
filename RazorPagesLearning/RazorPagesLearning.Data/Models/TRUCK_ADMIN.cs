using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// 車両マスタ
    /// </summary>   
    ////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class TRUCK_ADMIN : MODIFY_USER_INFORMATION
    {
		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// コンストラクタ
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public TRUCK_ADMIN()
		{
		}

		/// <summary>
		/// 運送会社コード
		/// </summary>
		[StringLength(128)]
		[DisplayName("運送会社コード")]
		public string TRANSPORT_ADMIN_CODE { get; set; }

		/// <summary>
		/// 車両管理番号
		/// 車番1～10のどこに表示するかを指定する番号
		/// </summary>
		[DisplayName("車両管理番号")]
		public int TRUCK_MANAGE_NUMBER { get; set; }

		/// <summary>
		/// 車番
		/// </summary>
		[StringLength(4)]
		[DisplayName("車番")]
		public string NUMBER { get; set; }

		/// <summary>
		/// 担当者
		/// </summary>
		[StringLength(20)]
		[DisplayName("担当者")]
		public string CHARGE { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        [Required]
        [DisplayName("削除フラグ")]
        public bool DELETE_FLAG { get; set; }

		#region Navigation

		/// <summary>
		/// 運送会社マスタ
		/// </summary>
		[ForeignKey("TRANSPORT_ADMIN_CODE")]
		[DisplayName("運送会社マスタ")]
		public TRANSPORT_ADMIN TRANSPORT_ADMIN { get; set; }

		#endregion // Navigation

    }

}