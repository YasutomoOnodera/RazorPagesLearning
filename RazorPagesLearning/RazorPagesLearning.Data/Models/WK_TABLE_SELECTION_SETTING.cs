using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// テーブル上のチェックボックス選択状態管理
    /// </summary>
    [Serializable]
    public class WK_TABLE_SELECTION_SETTING : COMMON_MANAGEMENT_INFORMATION
    {
        public WK_TABLE_SELECTION_SETTING()
        {
        }

        /// <summary>
        /// ID
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
        /// 元データのID
        /// </summary>
        public Int64 originalDataId { get; set; }

        /// <summary>
        /// 要素を選択済みか
        /// </summary>
        public bool selected { get; set; }

        /// <summary>
        /// 付加情報
        /// 必要に応じて使用
        /// </summary>
        public string appendInfo { get; set; }

    }
}