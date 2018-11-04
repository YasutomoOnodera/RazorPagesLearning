using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.TagHelpers
{
    /// <summary>
    /// ページネーションにおいて1ページの表示件数を選択するタグヘルパー
    /// </summary>
   // [HtmlTargetElement("pager", Attributes = "total-pages, current-page, link-url, paginationInfo")]
    public class PaginationSelectDisplayNumberTagHelper : TagHelper
    {
        public Pagination.PaginationInfo Info { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            /// asp-forで要素情報が正しく扱えないので、
            /// 現状はこのタグヘルパーは使わない事とする

            if( null != Info ){

                output.TagName = "div";
                output.Attributes.SetAttribute("class", "p-datachip");
                output.PreContent.SetHtmlContent($@"
                            <h1>
                                表示件数
                            </h1>
                            <p>
                            <select asp-for=""{Info.displayNumber}"" asp-items=""{ Info.displayNumbers}""
                                    onchange = ""submit(this.form)"" ></select>
                            </p>
                            ");                
            }
        }
    }
}
