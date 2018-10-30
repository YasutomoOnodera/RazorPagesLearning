using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// データを変更したユーザー情報
    /// </summary>
    [Serializable]
    public class MODIFY_USER_INFORMATION : COMMON_MANAGEMENT_INFORMATION
    {
        /// <summary>
        /// 登録ユーザーアカウントID
        /// </summary>
        [Required]
        [DisplayName("登録ユーザーアカウントID")]
        public Int64 CREATED_USER_ACCOUNT_ID { get; set; }

        /// <summary>
        /// 更新ユーザーアカウントID
        /// </summary>
        [Required]
        [DisplayName("更新ユーザーアカウントID")]
        public Int64 UPDATED_USER_ACCOUNT_ID { get; set; }

    }
}
