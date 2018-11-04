using RazorPagesLearning.Service.Definition;
using RazorPagesLearning.Utility.SortableTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.SelectableTable
{
    /// <summary>
    /// 選択可能な表を表示するページのview モデル規定
    /// </summary>
    public class SelectableTableViewModelBase<TableDataType>
        : TableViewModelBase<RowInfo< TableDataType >>
    {

        /// <summary>
        /// チェック情報を保存するテーブル種別
        /// </summary>
        public Data.Models.ViewTableType viewTableType { get; set; }


        public SelectableTableViewModelBase(Data.Models.ViewTableType refViewTableType)
        {
            this.viewTableType = refViewTableType;
        }

    }
}
