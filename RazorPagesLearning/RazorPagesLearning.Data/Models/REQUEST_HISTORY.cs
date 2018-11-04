using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ìÆËð
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
		/// [U[AJEgID
		/// </summary>
		[Required]
		[DisplayName("[U[AJEgID")]
		public Int64 USER_ACCOUNTID { get; set; }

		/// <summary>
		/// Ëú
		/// </summary>
		[DisplayName("Ëú")]
		public DateTimeOffset? REQUEST_DATE { get; set; }

		/// <summary>
		/// ótÔ
		/// </summary>
		[StringLength(8)]
		[DisplayName("ótÔ")]
		public string ORDER_NUMBER { get; set; }

		/// <summary>
		/// Ëàe
		/// DOMAIN.CODE(KIND=00020001)
		/// </summary>
		[Required]
		[StringLength(8)]
		[DisplayName("Ëàe")]
		public string REQUEST_KIND { get; set; }

		/// <summary>
		/// ¾×
		/// </summary>
		[Required]
		[DisplayName("¾×")]
		public int DETAIL_COUNT { get; set; }

        /// <summary>
        /// WMSóÔ
        /// DOMAIN.CODE(KIND=00090000)
        /// </summary>
        [StringLength(8)]
        [DisplayName("WMSóÔ")]
        public string WMS_STATUS { get; set; }

		/// <summary>
		/// Ë
		/// </summary>
		[Required]
		[DisplayName("Ë")]
		public int REQUEST_COUNT { get; set; }

		/// <summary>
		/// mè
		/// </summary>
		[Required]
		[DisplayName("mè")]
		public int CONFIRM_COUNT { get; set; }

        /// <summary>
        /// Wzæ}X^ID
        /// </summary>
        [DisplayName("Wzæ}X^ID")]
        public Int64 DELIVERY_ADMIN_ID { get; set; }

        /// <summary>
        /// o×æ/Ôp³(R[h)
        /// </summary>
        [StringLength(8)]
        [DisplayName("o×æ/Ôp³(R[h)")]
        public string SHIP_RETURN_CODE { get; set; }

        /// <summary>
        /// o×æ/Ôp³(Ð¼)
        /// </summary>
        [StringLength(72)]
        [DisplayName("o×æ/Ôp³(Ð¼)")]
        public string SHIP_RETURN_COMPANY { get; set; }

        /// <summary>
        /// o×æ/Ôp³(¼)
        /// </summary>
        [StringLength(72)]
        [DisplayName("o×æ/Ôp³(¼)")]
        public string SHIP_RETURN_DEPARTMENT { get; set; }

        /// <summary>
        /// o×æ/Ôp³(SÒ¼)
        /// </summary>
        [StringLength(72)]
        [DisplayName("o×æ/Ôp³(SÒ¼)")]
        public string SHIP_RETURN_CHARGE_NAME { get; set; }

        /// <summary>
        /// o×æ/Ôp³(XÖÔ)
        /// </summary>
        [StringLength(8)]
        [DisplayName("o×æ/Ôp³(XÖÔ)")]
        public string SHIP_RETURN_ZIPCODE { get; set; }

        /// <summary>
        /// o×æ/Ôp³(Z)
        /// </summary>
        [StringLength(255)]
        [DisplayName("o×æ/Ôp³(Z)")]
        public string SHIP_RETURN_ADDRESS { get; set; }

        /// <summary>
        /// o×æ/Ôp³(TEL)
        /// </summary>
        [StringLength(14)]
        [DisplayName("o×æ/Ôp³(TEL)")]
        public string SHIP_RETURN_TEL { get; set; }

		/// <summary>
		/// ËÒ_×åR[h
		/// </summary>
		[StringLength(3)]
		[DisplayName("ËÒ_×åR[h")]
		public string OWNER_SHIPPER_CODE { get; set; }

		/// <summary>
		/// ËÒ_Ð¼
		/// </summary>
		[StringLength(128)]
		[DisplayName("ËÒ_Ð¼")]
		public string OWNER_COMPANY { get; set; }

		/// <summary>
		/// ËÒ_¼
		/// </summary>
		[StringLength(72)]
		[DisplayName("ËÒ_¼")]
		public string OWNER_DEPARTMENT { get; set; }

		/// <summary>
		/// ËÒ_SÒ¼
		/// </summary>
		[StringLength(72)]
		[DisplayName("ËÒ_SÒ¼")]
		public string OWNER_CHARGE { get; set; }

		/// <summary>
		/// ËÒ_XÖÔ
		/// </summary>
		[StringLength(8)]
		[DisplayName("ËÒ_XÖÔ")]
		public string OWNER_ZIPCODE { get; set; }

		/// <summary>
		/// ËÒ_Z
		/// </summary>
		[StringLength(255)]
		[DisplayName("ËÒ_Z")]
		public string OWNER_ADDRESS { get; set; }

		/// <summary>
		/// ËÒ_TEL
		/// </summary>
		[StringLength(14)]
		[DisplayName("ËÒ_TEL")]
		public string OWNER_TEL { get; set; }

		/// <summary>
		/// wèú
		/// </summary>
		[DisplayName("wèú")]
		public DateTimeOffset? SPECIFIED_DATE { get; set; }

		/// <summary>
		/// wèÔ
		/// </summary>
		[StringLength(8)]
		[DisplayName("wèÔ")]
		public string SPECIFIED_TIME { get; set; }

		/// <summary>
		/// Ö
		/// </summary>
		[StringLength(8)]
		[DisplayName("Ö")]
		public string FLIGHT { get; set; }

		/// <summary>
		/// Rg
		/// </summary>
		[StringLength(100)]
		[DisplayName("Rg")]
		public string COMMENT { get; set; }

		#region Navigation

		/// <summary>
		/// ìÆËðÚ×
		/// </summary>
		[DisplayName("ìÆËðÚ×")]
		public IEnumerable<REQUEST_HISTORY_DETAIL> REQUEST_HISTORY_DETAILs { get; set; }

		#endregion // Navigation

    }

}