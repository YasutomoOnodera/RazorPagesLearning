using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ��ƈ˗������ڍ�
    /// </summary>   
    [Serializable]
    public class REQUEST_HISTORY_DETAIL : MODIFY_USER_INFORMATION
    {
        /// <summary>
        /// ID
        /// </summary>
		[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
        public Int64 ID { get; set; }

		/// <summary>
		/// ��ƈ˗�����ID
		/// </summary>
		[Required]
		[DisplayName("��ƈ˗�����ID")]
		public Int64 REQUEST_HISTORY_ID { get; set; }

        /// <summary>
        /// ��t�ԍ�
        /// </summary>
        [StringLength(8)]
        [DisplayName("��t�ԍ�")]
        public string ORDER_NUMBER { get; set; }

        /// <summary>
        /// �`�[�ԍ�
        /// </summary>
        [StringLength(128)]
        [DisplayName("�`�[�ԍ�")]
        public string SLIP_NUMBER { get; set; }

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
        [DisplayName("�˗���")]
        public int? REQUEST_COUNT { get; set; }

        /// <summary>
        /// �m�萔
        /// </summary>
        [DisplayName("�m�萔")]
        public int? CONFIRM_COUNT { get; set; }

        /// <summary>
        /// �݌�ID
        /// </summary>
        [Required]
		[DisplayName("�݌�ID")]
		public Int64 STOCK_ID { get; set; }

        /// <summary>
        /// �q�ɊǗ��ԍ�
        /// </summary>
        [StringLength(30)]
        [DisplayName("�q�ɊǗ��ԍ�")]
        public string STORAGE_MANAGE_NUMBER { get; set; }

        /// <summary>
        /// �X�e�[�^�X
        /// </summary>
        [StringLength(8)]
        [DisplayName("�X�e�[�^�X")]
        public string STATUS { get; set; }

        /// <summary>
        /// ���q�l�Ǘ��ԍ�
        /// </summary>
        [StringLength(30)]
        [DisplayName("���q�l�Ǘ��ԍ�")]
        public string CUSTOMER_MANAGE_NUMBER { get; set; }

        /// <summary>
        /// �薼
        /// </summary>
        [StringLength(200)]
		[DisplayName("�薼")]
		public string TITLE { get; set; }

		/// <summary>
		/// ����
		/// </summary>
		[StringLength(200)]
		[DisplayName("����")]
		public string SUBTITLE { get; set; }

        /// <summary>
        /// �׎�R�[�h
        /// </summary>
        [StringLength(3)]
        [DisplayName("�׎�R�[�h")]
        public string SHIPPER_CODE { get; set; }

        /// <summary>
        /// ���ۃR�[�h
        /// </summary>
        [StringLength(128)]
        [DisplayName("���ۃR�[�h")]
        public string DEPARTMENT_CODE { get; set; }

        /// <summary>
        /// �`��
        /// </summary>
        [StringLength(20)]
        [DisplayName("�`��")]
        public string SHAPE { get; set; }

        /// <summary>
        /// �敪1
        /// DOMAIN.CODE(KIND=00010002)
        /// </summary>
        [StringLength(8)]
        [DisplayName("�敪1")]
        public string CLASS1 { get; set; }

        /// <summary>
        /// �敪2
        /// DOMAIN.CODE(KIND=00010003)
        /// </summary>
        [StringLength(8)]
        [DisplayName("�敪2")]
        public string CLASS2 { get; set; }

        /// <summary>
        /// Remark1
        /// </summary>
        [StringLength(72)]
        [DisplayName("Remark1")]
        public string REMARK1 { get; set; }

        /// <summary>
        /// Remark2
        /// </summary>
        [StringLength(72)]
        [DisplayName("Remark2")]
        public string REMARK2 { get; set; }

        /// <summary>
        /// ���l
        /// </summary>
        [StringLength(200)]
        [DisplayName("���l")]
        public string NOTE { get; set; }

        /// <summary>
        /// �׎區��
        /// </summary>
        [StringLength(2000)]
		[DisplayName("�׎區��")]
		public string SHIPPER_NOTE { get; set; }

		/// <summary>
		/// �����
		/// </summary>
		[StringLength(10)]
		[DisplayName("�����")]
        public string PRODUCT_DATE { get; set; }

		/// <summary>
		/// ���ɓ�
		/// </summary>
		[DisplayName("���ɓ�")]
		public DateTimeOffset? STORAGE_DATE { get; set; }

		/// <summary>
		/// ������
		/// </summary>
		[DisplayName("������")]
        public DateTimeOffset? PROCESSING_DATE { get; set; }

        /// <summary>
        /// �p���\���
        /// </summary>
        [DisplayName("�p���\���")]
        public DateTimeOffset? SCRAP_SCHEDULE_DATE { get; set; }

		/// <summary>
		/// ����1
		/// </summary>
		[StringLength(3)]
		[DisplayName("����1")]
		public string TIME1{ get; set; }

		/// <summary>
		/// ����2
		/// </summary>
		[StringLength(3)]
		[DisplayName("����2")]
		public string TIME2 { get; set; }

		/// <summary>
		/// �݌ɐ�
		/// </summary>
		[DisplayName("�݌ɐ�")]
		public int? STOCK_COUNT { get; set; }

		/// <summary>
		/// ���o�ɓ�
		/// </summary>
		[DisplayName("���o�ɓ�")]
		public DateTimeOffset? STORAGE_RETRIEVAL_DATE { get; set; }

		/// <summary>
		/// ������
		/// </summary>
		[StringLength(128)]
		[DisplayName("������")]
		public string ARRIVAL_TIME { get; set; }

		/// <summary>
		/// �o�[�R�[�h
		/// </summary>
		[StringLength(9)]
		[DisplayName("�o�[�R�[�h")]
		public string BARCODE { get; set; }

		/// <summary>
		/// �݌Ɏ��
		/// DOMAIN.CODE(KIND=00010008)
		/// </summary>
		[StringLength(8)]
        [DisplayName("�݌Ɏ��")]
        public string STOCK_KIND { get; set; }

        /// <summary>
        /// �P��
        /// ���ސ�p����
        /// </summary>
        [DisplayName("�P��")]
        public int? UNIT { get; set; }

        /// <summary>
        /// WMS�o�^��
        /// </summary>
        [DisplayName("WMS�o�^��")]
		public DateTimeOffset? WMS_REGIST_DATE{ get; set; }

        /// <summary>
        /// WMS�X�V��
        /// </summary>
        [DisplayName("WMS�X�V��")]
		public DateTimeOffset? WMS_UPDATE_DATE { get; set; }

		/// <summary>
		/// ProjectNo1
		/// �ڋq��p����
		/// </summary>
		[StringLength(20)]
		[DisplayName("ProjectNo1")]
		public string PROJECT_NO1 { get; set; }

		/// <summary>
		/// ProjectNo2
		/// �ڋq��p����
		/// </summary>
		[StringLength(50)]
		[DisplayName("ProjectNo2")]
		public string PROJECT_NO2 { get; set; }

		/// <summary>
		/// ���쌠1
		/// �ڋq��p����
		/// DOMAIN.CODE(KIND=00010005)
		/// </summary>
		[StringLength(8)]
		[DisplayName("���쌠1")]
		public string COPYRIGHT1 { get; set; }

		/// <summary>
		/// ���쌠2
		/// �ڋq��p����
		/// </summary>
		[StringLength(50)]
		[DisplayName("���쌠2")]
		public string COPYRIGHT2 { get; set; }

		/// <summary>
		/// �_��1
		/// �ڋq��p����
		/// DOMAIN.CODE(KIND=00010006)
		/// </summary>
		[StringLength(8)]
		[DisplayName("�_��1")]
		public string CONTRACT1 { get; set; }

		/// <summary>
		/// �_��2
		/// �ڋq��p����
		/// </summary>
		[StringLength(50)]
		[DisplayName("�_��2")]
		public string CONTRACT2 { get; set; }

		/// <summary>
		/// �f�[�^NO1
		/// �ڋq��p����
		/// </summary>
		[StringLength(20)]
		[DisplayName("�f�[�^NO1")]
		public string DATA_NO1 { get; set; }

		/// <summary>
		/// �f�[�^NO2
		/// �ڋq��p����
		/// </summary>
		[StringLength(50)]
		[DisplayName("�f�[�^NO2")]
		public string DATA_NO2 { get; set; }

		/// <summary>
		/// ��������1
		/// �ڋq��p����
		/// DOMAIN.CODE(KIND=00010007)
		/// </summary>
		[StringLength(8)]
		[DisplayName("��������1")]
		public string PROCESS_JUDGE1 { get; set; }

		/// <summary>
		/// ��������2
		/// �ڋq��p����
		/// </summary>
		[StringLength(50)]
		[DisplayName("��������2")]
		public string PROCESS_JUDGE2 { get; set; }

        #region Navigation

        /// <summary>
        /// ��ƈ˗�����
        /// </summary>
        [ForeignKey("REQUEST_HISTORY_ID")]
		[DisplayName("��ƈ˗�����")]
		public REQUEST_HISTORY REQUEST_HISTORY { get; set; }

		/// <summary>
		/// WMS_���o�Ɏ���
		/// </summary>
		[DisplayName("WMS_���o�Ɏ���")]
		public WMS_RESULT_HISTORY WMS_RESULT_HISTORY { get; set; }

		#endregion // Navigation

    }

}