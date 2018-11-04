using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ��ƈ˗�����
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
		/// ���[�U�[�A�J�E���gID
		/// </summary>
		[Required]
		[DisplayName("���[�U�[�A�J�E���gID")]
		public Int64 USER_ACCOUNTID { get; set; }

		/// <summary>
		/// �˗���
		/// </summary>
		[DisplayName("�˗���")]
		public DateTimeOffset? REQUEST_DATE { get; set; }

		/// <summary>
		/// ��t�ԍ�
		/// </summary>
		[StringLength(8)]
		[DisplayName("��t�ԍ�")]
		public string ORDER_NUMBER { get; set; }

		/// <summary>
		/// �˗����e
		/// DOMAIN.CODE(KIND=00020001)
		/// </summary>
		[Required]
		[StringLength(8)]
		[DisplayName("�˗����e")]
		public string REQUEST_KIND { get; set; }

		/// <summary>
		/// ���א�
		/// </summary>
		[Required]
		[DisplayName("���א�")]
		public int DETAIL_COUNT { get; set; }

        /// <summary>
        /// WMS���
        /// DOMAIN.CODE(KIND=00090000)
        /// </summary>
        [StringLength(8)]
        [DisplayName("WMS���")]
        public string WMS_STATUS { get; set; }

		/// <summary>
		/// �˗���
		/// </summary>
		[Required]
		[DisplayName("�˗���")]
		public int REQUEST_COUNT { get; set; }

		/// <summary>
		/// �m�萔
		/// </summary>
		[Required]
		[DisplayName("�m�萔")]
		public int CONFIRM_COUNT { get; set; }

        /// <summary>
        /// �W�z��}�X�^ID
        /// </summary>
        [DisplayName("�W�z��}�X�^ID")]
        public Int64 DELIVERY_ADMIN_ID { get; set; }

        /// <summary>
        /// �o�א�/�ԋp��(�R�[�h)
        /// </summary>
        [StringLength(8)]
        [DisplayName("�o�א�/�ԋp��(�R�[�h)")]
        public string SHIP_RETURN_CODE { get; set; }

        /// <summary>
        /// �o�א�/�ԋp��(�Ж�)
        /// </summary>
        [StringLength(72)]
        [DisplayName("�o�א�/�ԋp��(�Ж�)")]
        public string SHIP_RETURN_COMPANY { get; set; }

        /// <summary>
        /// �o�א�/�ԋp��(������)
        /// </summary>
        [StringLength(72)]
        [DisplayName("�o�א�/�ԋp��(������)")]
        public string SHIP_RETURN_DEPARTMENT { get; set; }

        /// <summary>
        /// �o�א�/�ԋp��(�S���Җ�)
        /// </summary>
        [StringLength(72)]
        [DisplayName("�o�א�/�ԋp��(�S���Җ�)")]
        public string SHIP_RETURN_CHARGE_NAME { get; set; }

        /// <summary>
        /// �o�א�/�ԋp��(�X�֔ԍ�)
        /// </summary>
        [StringLength(8)]
        [DisplayName("�o�א�/�ԋp��(�X�֔ԍ�)")]
        public string SHIP_RETURN_ZIPCODE { get; set; }

        /// <summary>
        /// �o�א�/�ԋp��(�Z��)
        /// </summary>
        [StringLength(255)]
        [DisplayName("�o�א�/�ԋp��(�Z��)")]
        public string SHIP_RETURN_ADDRESS { get; set; }

        /// <summary>
        /// �o�א�/�ԋp��(TEL)
        /// </summary>
        [StringLength(14)]
        [DisplayName("�o�א�/�ԋp��(TEL)")]
        public string SHIP_RETURN_TEL { get; set; }

		/// <summary>
		/// �˗���_�׎�R�[�h
		/// </summary>
		[StringLength(3)]
		[DisplayName("�˗���_�׎�R�[�h")]
		public string OWNER_SHIPPER_CODE { get; set; }

		/// <summary>
		/// �˗���_�Ж�
		/// </summary>
		[StringLength(128)]
		[DisplayName("�˗���_�Ж�")]
		public string OWNER_COMPANY { get; set; }

		/// <summary>
		/// �˗���_������
		/// </summary>
		[StringLength(72)]
		[DisplayName("�˗���_������")]
		public string OWNER_DEPARTMENT { get; set; }

		/// <summary>
		/// �˗���_�S���Җ�
		/// </summary>
		[StringLength(72)]
		[DisplayName("�˗���_�S���Җ�")]
		public string OWNER_CHARGE { get; set; }

		/// <summary>
		/// �˗���_�X�֔ԍ�
		/// </summary>
		[StringLength(8)]
		[DisplayName("�˗���_�X�֔ԍ�")]
		public string OWNER_ZIPCODE { get; set; }

		/// <summary>
		/// �˗���_�Z��
		/// </summary>
		[StringLength(255)]
		[DisplayName("�˗���_�Z��")]
		public string OWNER_ADDRESS { get; set; }

		/// <summary>
		/// �˗���_TEL
		/// </summary>
		[StringLength(14)]
		[DisplayName("�˗���_TEL")]
		public string OWNER_TEL { get; set; }

		/// <summary>
		/// �w���
		/// </summary>
		[DisplayName("�w���")]
		public DateTimeOffset? SPECIFIED_DATE { get; set; }

		/// <summary>
		/// �w�莞��
		/// </summary>
		[StringLength(8)]
		[DisplayName("�w�莞��")]
		public string SPECIFIED_TIME { get; set; }

		/// <summary>
		/// ��
		/// </summary>
		[StringLength(8)]
		[DisplayName("��")]
		public string FLIGHT { get; set; }

		/// <summary>
		/// �R�����g
		/// </summary>
		[StringLength(100)]
		[DisplayName("�R�����g")]
		public string COMMENT { get; set; }

		#region Navigation

		/// <summary>
		/// ��ƈ˗������ڍ�
		/// </summary>
		[DisplayName("��ƈ˗������ڍ�")]
		public IEnumerable<REQUEST_HISTORY_DETAIL> REQUEST_HISTORY_DETAILs { get; set; }

		#endregion // Navigation

    }

}