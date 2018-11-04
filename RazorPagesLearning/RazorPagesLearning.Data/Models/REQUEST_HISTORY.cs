using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 作業依頼履歴
    /// </summary>   
    [Serializable]
    public class REQUEST_HISTORY : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
        public Int64 ID { get; set; }

		/// <summary>
		/// ユーザーアカウントID
		/// </summary>
		[Required]
		[DisplayName("ユーザーアカウントID")]
		public Int64 USER_ACCOUNTID { get; set; }

		/// <summary>
		/// 依頼日
		/// </summary>
		[DisplayName("依頼日")]
		public DateTimeOffset? REQUEST_DATE { get; set; }

		/// <summary>
		/// 受付番号
		/// </summary>
		[StringLength(8)]
		[DisplayName("受付番号")]
		public string ORDER_NUMBER { get; set; }

		/// <summary>
		/// 依頼内容
		/// DOMAIN.CODE(KIND=00020001)
		/// </summary>
		[Required]
		[StringLength(8)]
		[DisplayName("依頼内容")]
		public string REQUEST_KIND { get; set; }

		/// <summary>
		/// 明細数
		/// </summary>
		[Required]
		[DisplayName("明細数")]
		public int DETAIL_COUNT { get; set; }

        /// <summary>
        /// WMS状態
        /// DOMAIN.CODE(KIND=00090000)
        /// </summary>
        [StringLength(8)]
        [DisplayName("WMS状態")]
        public string WMS_STATUS { get; set; }

		/// <summary>
		/// 依頼数
		/// </summary>
		[Required]
		[DisplayName("依頼数")]
		public int REQUEST_COUNT { get; set; }

		/// <summary>
		/// 確定数
		/// </summary>
		[Required]
		[DisplayName("確定数")]
		public int CONFIRM_COUNT { get; set; }

        /// <summary>
        /// 集配先マスタID
        /// </summary>
        [DisplayName("集配先マスタID")]
        public Int64 DELIVERY_ADMIN_ID { get; set; }

        /// <summary>
        /// 出荷先/返却元(コード)
        /// </summary>
        [StringLength(8)]
        [DisplayName("出荷先/返却元(コード)")]
        public string SHIP_RETURN_CODE { get; set; }

        /// <summary>
        /// 出荷先/返却元(社名)
        /// </summary>
        [StringLength(72)]
        [DisplayName("出荷先/返却元(社名)")]
        public string SHIP_RETURN_COMPANY { get; set; }

        /// <summary>
        /// 出荷先/返却元(部署名)
        /// </summary>
        [StringLength(72)]
        [DisplayName("出荷先/返却元(部署名)")]
        public string SHIP_RETURN_DEPARTMENT { get; set; }

        /// <summary>
        /// 出荷先/返却元(担当者名)
        /// </summary>
        [StringLength(72)]
        [DisplayName("出荷先/返却元(担当者名)")]
        public string SHIP_RETURN_CHARGE_NAME { get; set; }

        /// <summary>
        /// 出荷先/返却元(郵便番号)
        /// </summary>
        [StringLength(8)]
        [DisplayName("出荷先/返却元(郵便番号)")]
        public string SHIP_RETURN_ZIPCODE { get; set; }

        /// <summary>
        /// 出荷先/返却元(住所)
        /// </summary>
        [StringLength(255)]
        [DisplayName("出荷先/返却元(住所)")]
        public string SHIP_RETURN_ADDRESS { get; set; }

        /// <summary>
        /// 出荷先/返却元(TEL)
        /// </summary>
        [StringLength(14)]
        [DisplayName("出荷先/返却元(TEL)")]
        public string SHIP_RETURN_TEL { get; set; }

		/// <summary>
		/// 依頼者_荷主コード
		/// </summary>
		[StringLength(3)]
		[DisplayName("依頼者_荷主コード")]
		public string OWNER_SHIPPER_CODE { get; set; }

		/// <summary>
		/// 依頼者_社名
		/// </summary>
		[StringLength(128)]
		[DisplayName("依頼者_社名")]
		public string OWNER_COMPANY { get; set; }

		/// <summary>
		/// 依頼者_部署名
		/// </summary>
		[StringLength(72)]
		[DisplayName("依頼者_部署名")]
		public string OWNER_DEPARTMENT { get; set; }

		/// <summary>
		/// 依頼者_担当者名
		/// </summary>
		[StringLength(72)]
		[DisplayName("依頼者_担当者名")]
		public string OWNER_CHARGE { get; set; }

		/// <summary>
		/// 依頼者_郵便番号
		/// </summary>
		[StringLength(8)]
		[DisplayName("依頼者_郵便番号")]
		public string OWNER_ZIPCODE { get; set; }

		/// <summary>
		/// 依頼者_住所
		/// </summary>
		[StringLength(255)]
		[DisplayName("依頼者_住所")]
		public string OWNER_ADDRESS { get; set; }

		/// <summary>
		/// 依頼者_TEL
		/// </summary>
		[StringLength(14)]
		[DisplayName("依頼者_TEL")]
		public string OWNER_TEL { get; set; }

		/// <summary>
		/// 指定日
		/// </summary>
		[DisplayName("指定日")]
		public DateTimeOffset? SPECIFIED_DATE { get; set; }

		/// <summary>
		/// 指定時間
		/// </summary>
		[StringLength(8)]
		[DisplayName("指定時間")]
		public string SPECIFIED_TIME { get; set; }

		/// <summary>
		/// 便
		/// </summary>
		[StringLength(8)]
		[DisplayName("便")]
		public string FLIGHT { get; set; }

		/// <summary>
		/// コメント
		/// </summary>
		[StringLength(100)]
		[DisplayName("コメント")]
		public string COMMENT { get; set; }

		#region Navigation

		/// <summary>
		/// 作業依頼履歴詳細
		/// </summary>
		[DisplayName("作業依頼履歴詳細")]
		public IEnumerable<REQUEST_HISTORY_DETAIL> REQUEST_HISTORY_DETAILs { get; set; }

		#endregion // Navigation

    }

}