using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 集配依頼詳細
    /// </summary>
    [Serializable]
    public class DELIVERY_REQUEST_DETAIL : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
		public Int64 ID { get; set; }

		/// <summary>
		/// 運送会社コード
		/// </summary>
		[Required]
		[StringLength(128)]
		[DisplayName("運送会社コード")]
		public string TRANSPORT_ADMIN_CODE { get; set; }

        /// <summary>
        /// 集配依頼番号
        /// </summary>
        [Required]
        [StringLength(128)]
        [DisplayName("集配依頼番号")]
        public string DELIVERY_REQUEST_NUMBER { get; set; }

        /// <summary>
        /// 集配依頼枝番
        /// </summary>
        [Required]
        [StringLength(128)]
        [DisplayName("集配依頼枝番")]
        public string DELIVERY_REQUEST_DETAIL_NUMBER { get; set; }

        /// <summary>
        /// 車両管理番号
        /// 車番1～10のどこに表示するかを指定する番号
        /// </summary>
        [DisplayName("車両管理番号")]
        public int? TRUCK_MANAGE_NUMBER { get; set; }

		/// <summary>
		/// 車番
		/// </summary>
		[StringLength(4)]
		[DisplayName("車番")]
		public string TRUCK_NUMBER { get; set; }

		/// <summary>
		/// 担当者
		/// </summary>
		[StringLength(20)]
		[DisplayName("担当者")]
		public string TRUCK_CHARGE { get; set; }

		/// <summary>
		/// ルート
		/// </summary>
		[DisplayName("ルート")]
		public int? ROUTE { get; set; }

		/// <summary>
		/// 集配先会社名
		/// </summary>
		[StringLength(128)]
		[DisplayName("集配先会社名")]
		public string COMPANY { get; set; }

		/// <summary>
		/// 配達品名数量
		/// </summary>
		[StringLength(128)]
		[DisplayName("配達品名数量")]
		public string DELIVERY_TITLE { get; set; }

		/// <summary>
		/// 配達備考
		/// </summary>
		[StringLength(128)]
		[DisplayName("配達備考")]
		public string DELIVERY_NOTE { get; set; }

		/// <summary>
		/// 配達アカウント先
		/// </summary>
		[StringLength(128)]
		[DisplayName("配達アカウント先")]
		public string DELIVERY_AC { get; set; }

		/// <summary>
		/// 集荷品名数量
		/// </summary>
		[StringLength(128)]
		[DisplayName("集荷品名数量")]
		public string CARGO_TITLE { get; set; }

		/// <summary>
		/// 集荷備考
		/// </summary>
		[StringLength(128)]
		[DisplayName("集荷備考")]
		public string CARGO_NOTE { get; set; }

		/// <summary>
		/// 集荷アカウント先
		/// </summary>
		[StringLength(128)]
		[DisplayName("集荷アカウント先")]
		public string CARGO_AC { get; set; }

        /// <summary>
        /// バーコード
        /// </summary>
        [StringLength(128)]
        [DisplayName("バーコード")]
        public string BARCODE { get; set; }

        #region Navigation

        /// <summary>
        /// 運送会社マスタ
        /// </summary>
        [ForeignKey("TRANSPORT_ADMIN_CODE")]
		[DisplayName("運送会社マスタ")]
		public TRANSPORT_ADMIN TRANSPORT_ADMIN { get; set; }

		/// <summary>
		/// 集配依頼
		/// </summary>
		[DisplayName("集配依頼")]
		public DELIVERY_REQUEST DELIVERY_REQUEST { get; set; }

		#endregion // Navigation

	}
}
