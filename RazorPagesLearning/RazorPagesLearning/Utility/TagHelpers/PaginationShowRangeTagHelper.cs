using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.TagHelpers
{
    /// <summary>
    /// ページネーションにおいて表示しているページ範囲を表示するタグヘルパー
    /// </summary>
   // [HtmlTargetElement("pager", Attributes = "total-pages, current-page, link-url, paginationInfo")]
    public class PaginationShowRangeTagHelper : TagHelper
    {
        public Pagination.PaginationInfo Info { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if( null != Info ){

                output.TagName = "p";
                output.PreContent.SetHtmlContent($@"
                                <span>{Info.startViewItemIndex + 1}~{Info.endViewItemIndex}</span>
                                /
                                {Info.maxItems} 件
                                </span>");
                output.Attributes.Clear();
            }
        }
    }
}
