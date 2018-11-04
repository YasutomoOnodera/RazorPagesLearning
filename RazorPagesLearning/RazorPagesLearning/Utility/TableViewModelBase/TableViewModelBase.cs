using RazorPagesLearning.Service.Definition;
using RazorPagesLearning.Utility.SelectableTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.SortableTable
{
    /// <summary>
    /// テーブル列を持つView Modelの基底
    /// </summary>
    /// <typeparam name="TableDataType"></typeparam>
    public class TableViewModelBase<RowInfoType>
    {
        /// <summary>
        /// ソート順序
        /// </summary>
        public string sortColumn { get; set; }

        /// <summary>
        /// ソート方向
        /// </summary>
        public SortDirection sortDirection { get; set; }

        /// <summary>
        /// 画面に表示する行情報
        /// </summary>
        public List<RowInfoType> tableRows { get; set; }

        /// <summary>
        /// サーバー上で管理されているチェック済みのデータ件数
        /// </summary>
        public int checkedCountOnServer { get; set; }

        /// <summary>
        /// 指定された列がソート選択状態であるか判定する
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string getTargetSortSign(string type)
        {
            if (null != type)
            {
                if (type == sortColumn)
                {
                    switch (this.sortDirection)
                    {
                        case SortDirection.ASC:
                            {
                                return "▲";
                            }
                        case SortDirection.DES:
                            {
                                return "▼";
                            }
                    }
                }
            }
            return "●";
        }

        public TableViewModelBase()
        {
            this.tableRows = new List<RowInfoType>();
        }
    }
}
