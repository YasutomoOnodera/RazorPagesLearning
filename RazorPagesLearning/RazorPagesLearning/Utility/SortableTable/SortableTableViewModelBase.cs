using RazorPagesLearning.Service.Definition;
using RazorPagesLearning.Utility.SelectableTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.SortableTable
{
    /// <summary>
    /// ソート可能なテーブル列を持つView Modelの基底
    /// </summary>
    /// <typeparam name="TableDataType"></typeparam>
    public class SortableTableViewModelBase<TableDataType>
        : TableViewModelBase< SortableTableRowInfo<TableDataType> >
    {
        public SortableTableViewModelBase()
        {
            this.tableRows = new List<SortableTableRowInfo<TableDataType>>();
        }
    }
}
