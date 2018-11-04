using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// テーブルページネーション設定情報
    /// </summary>
    [Serializable]
    public class WK_TABLE_PAGINATION_SETTING : COMMON_MANAGEMENT_INFORMATION
    {
        public WK_TABLE_PAGINATION_SETTING()
        {
        }

        /// <summary>
        ///ID
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 ID { get; set; }

        /// <summary>
        /// 関連付けされたユーザーアカウントID
        /// </summary>
        public Int64 USER_ACCOUNT_ID { get; set; }

        /// <summary>
        /// 対象となるテーブル
        /// </summary>
        public ViewTableType viewTableType { get; set; }

        /// <summary>
        /// 全要素を選択済みか
        /// </summary>
        public bool checkAllPage { get; set; }

    }
}