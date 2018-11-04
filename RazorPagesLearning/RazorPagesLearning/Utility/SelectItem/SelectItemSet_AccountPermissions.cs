using RazorPagesLearning.Data.Models;
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
    public class SelectItemSet_AccountPermissions : SelectItemSet
    {
        public void init()
        {
            var l = new List<RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION>();
            foreach (RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION ele in Enum.GetValues(typeof(RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION)))
            {
                l.Add(ele);
            }

            this.display = l.Select(e => new SelectListItem
            {
                Value = e.ToString(),
                Text = e.DisplayName()
            }).ToList();

            this.selected = this.display[0].Value;
        }


        public SelectItemSet_AccountPermissions()
        {
            this.init();
        }
    }
}
