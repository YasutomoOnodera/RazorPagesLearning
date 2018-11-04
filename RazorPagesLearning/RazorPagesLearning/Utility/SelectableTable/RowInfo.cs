using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Utility.SortableTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.SelectableTable
{
    /// <summary>
    /// 選択可能テーブルにおける各行の付帯情報
    /// </summary>
    public class RowInfo<TableDataType> : SortableTableRowInfo<TableDataType>
    {
        /// <summary>
        /// 要素のindex
        /// </summary>
        public TableSelectionSettingService.TrackingIdentifier trackingIdentifier { get; set; }

        /// <summary>
        /// 要素が選択済みか
        /// </summary>
        public bool isSelected { get; set; }

        /// <summary>
        /// 付加情報
        /// 適宜、必要な画面で使いたいときに使う
        /// </summary>
        public string appendInfo { get; set; }

    }
}
