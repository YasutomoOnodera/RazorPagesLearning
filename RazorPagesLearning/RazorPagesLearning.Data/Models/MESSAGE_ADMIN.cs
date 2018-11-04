using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ���b�Z�[�W�}�X�^
    /// </summary>
    [Serializable]
    public class MESSAGE_ADMIN : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// ���
		/// </summary>
		public enum MESSAGE_KIND
		{
			/// <summary>
			/// ���O�C��
			/// </summary>
			Login = 1,

			/// <summary>
			/// �z�[��
			/// </summary>
			Home = 2
		}


		/// <summary>
		/// ���
		/// </summary>
		[Key]
		[Required]
		[DisplayName("���")]
		public MESSAGE_KIND KIND { get; set; }

		/// <summary>
		/// ���b�Z�[�W
		/// </summary>
		[DisplayName("���b�Z�[�W")]
		public string MESSAGE { get; set; }

    }

}