using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.SelectItem
{
    /// <summary>
    /// 要素選択用のアイテム一覧
    /// </summary>
    public class SelectItemSet
    {

        /// <summary>
        /// 表示用の権限一覧
        /// </summary>
        public List<SelectListItem> display { get; set; }

        /// <summary>
        /// 選択されたアカウント権限
        /// </summary>
        public string selected { get; set; }

    }
}
