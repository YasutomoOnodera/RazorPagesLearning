using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    ///�Z�L�����e�B�|���V�[
    /// </summary>
    [Serializable]
    public class POLICY : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// �p�X���[�h�|���V�[
		/// </summary>
		public enum PASSWORD_POLICY
		{
			/// <summary>
			/// �p�X���[�h����
			/// </summary>
			Digit = 1,

			/// <summary>
			/// �p�X���[�h�ύX�Ԋu����
			/// </summary>
			Interval = 2,

			/// <summary>
			/// �p�X���[�h�ύX�ʒm����
			/// </summary>
			Notify = 3,

			/// <summary>
			/// �p�X���[�h�ė��p�֎~��
			/// </summary>
			Reuse = 4
		}

		/// <summary>
		/// ���O
		/// </summary>
		[Key]
		[Required]
		[DisplayName("���O")]
		public PASSWORD_POLICY NAME { get; set; }

		/// <summary>
		/// ���l
		/// </summary>
		[Required]
		[DisplayName("���l")]
		public int VALUE { get; set; }

    }
}