using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// �p�X���[�h����
    /// </summary>   
    [Serializable]
    public class PASSWORD_HISTORY : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		public PASSWORD_HISTORY()
		{
			var now = DateTimeOffset.Now;
			this.CREATED_AT = now;
			this.UPDATED_AT = now;
		}

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
		public Int64 USER_ACCOUNT_ID { get; set; }

		/// <summary>
		/// �p�X���[�h
		/// </summary>
		[Required]
		[DisplayName("�p�X���[�h")]
		public string PASSWORD { get; set; }

        #region Navigation

        /// <summary>
        /// ���[�U�[�A�J�E���g
        /// </summary>
        [ForeignKey("USER_ACCOUNT_ID")]
		[DisplayName("���[�U�[�A�J�E���g")]
		public USER_ACCOUNT USER_ACCOUNT { get; set; }

		#endregion // Navigation


        /// <summary>
        /// �p�X���[�h�̃\���g
        /// </summary>
        public string PASSWORD_SALT { get; set; }

    }

}