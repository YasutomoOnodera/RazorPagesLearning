using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    ///�J�����_�[�}�X�^
    /// </summary>
    [Serializable]
    public class CALENDAR_ADMIN : MODIFY_USER_INFORMATION
    {
		/// <summary>
		///�x��
		/// </summary>
		[Key]
		[Required]
		[DisplayName("�x��")]
		public DateTimeOffset HOLIDAY { get; set; }

    }
}