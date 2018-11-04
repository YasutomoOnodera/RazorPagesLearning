using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.ExcelAnalysis
{ 
    /// <summary>
    /// エクセル取り込み インターフェースクラス
    /// </summary>
    interface InterfaceExcelAnalysis
    {
        List<wkstock_Result> Analysis(string fileName);
    }
}
