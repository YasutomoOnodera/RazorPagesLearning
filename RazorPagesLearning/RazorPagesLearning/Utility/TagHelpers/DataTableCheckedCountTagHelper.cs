using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.TagHelpers
{
    /// <summary>
    /// データテーブル上における選択数カウント
    /// </summary>
   // [HtmlTargetElement("pager", Attributes = "total-pages, current-page, link-url, paginationInfo")]
    public class DataTableCheckedCountTagHelper : TagHelper
    {
        /// <summary>
        /// サーバー上のチェック済み件数
        /// </summary>
        public int checkedCountOnServer { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
                output.TagName = "p";
                output.PreContent.SetHtmlContent($@"
<input type=""hidden""  id=""dataTables-checkedCountOnServer"" value=""{checkedCountOnServer}"" />
<span id = ""dataTables-checkedCount"" > 0 </span >
<span > 件選択しました。</span >
                                ");
                output.Attributes.Clear();
        }
    }
}
