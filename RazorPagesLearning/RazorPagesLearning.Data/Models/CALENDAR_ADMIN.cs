using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    ///カレンダーマスタ
    /// </summary>
    [Serializable]
    public class CALENDAR_ADMIN : MODIFY_USER_INFORMATION
    {
		/// <summary>
		///休日
		/// </summary>
		[Key]
		[Required]
		[DisplayName("休日")]
		public DateTimeOffset HOLIDAY { get; set; }

    }
}