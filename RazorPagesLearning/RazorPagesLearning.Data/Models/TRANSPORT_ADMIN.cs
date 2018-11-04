using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 運送会社マスタ
    /// </summary>   
    [Serializable]
    public class TRANSPORT_ADMIN : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TRANSPORT_ADMIN()
		{
		}

        /// <summary>
        /// 運送会社コード
        /// </summary>
		[Key]
		[Required]
		[StringLength(128)]
		[DisplayName("運送会社コード")]
        public string CODE { get; set; }

		/// <summary>
		/// 運送会社名
		/// </summary>
		[Required]
		[StringLength(128)]
		[DisplayName("運送会社名")]
		public string NAME { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        [Required]
        [DisplayName("削除フラグ")]
        public bool DELETE_FLAG { get; set; }

        #region Navigation

        /// <summary>
        /// 車両マスタ
        /// </summary>
        [DisplayName("車両マスタ")]
		public IEnumerable<TRUCK_ADMIN> TRUCK_ADMINs { get; set; }

		/// <summary>
		/// 集配依頼
		/// </summary>
		[DisplayName("集配依頼")]
		public List<DELIVERY_REQUEST> DELIVERY_REQUESTs { get; set; }

		/// <summary>
		/// 集配依頼詳細
		/// </summary>
		[DisplayName("集配依頼詳細")]
		public List<DELIVERY_REQUEST_DETAIL> DELIVERY_REQUEST_DETAILs { get; set; }

		#endregion // Navigation

    }
}