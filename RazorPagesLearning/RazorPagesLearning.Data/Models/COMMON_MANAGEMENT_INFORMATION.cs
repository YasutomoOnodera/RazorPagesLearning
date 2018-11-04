using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// テーブル間で共通となる共通定義項目
    /// </summary>
    [Serializable]
    public class COMMON_MANAGEMENT_INFORMATION 
    {

		/// <summary>
		/// コンストラクタ
		/// </summary>
        public COMMON_MANAGEMENT_INFORMATION()
        {
            var now = DateTimeOffset.Now;
            CREATED_AT = now;
            UPDATED_AT = now;
        }

        #region 共通項目

        /// <summary>
        /// 登録日時
        /// </summary>
        [Required]
        [DisplayName("登録日時")]
        public DateTimeOffset CREATED_AT { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        [Required]
        [DisplayName("更新日時")]
        public DateTimeOffset UPDATED_AT { get; set; }

        #endregion // 共通項目

        /// <summary>
        /// 同期更新チェック用のタイムスタンプ列
        /// </summary>
        [Timestamp]
        public Byte[] Timestamp { get; set; }
    }
}
