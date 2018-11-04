using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.SortableTable
{
    /// <summary>
    /// ソート可能テーブルにおける各行の詳細情報
    /// </summary>
    /// <typeparam name="TableDataType"></typeparam>
    public class SortableTableRowInfo<TableDataType>
    {
        //------
        ///以下は動的バインドから外す

        /// <summary>
        /// テーブルで表示するデータ種別
        /// </summary>
        public TableDataType data { get; set; }

    }
}
