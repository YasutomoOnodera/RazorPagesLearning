using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.TagHelpers
{
    /// <summary>
    /// ページャーのタグヘルパー
    /// </summary>
    [HtmlTargetElement("pagination-pager")]
    public class PaginationPagerTagHelper : TagHelper
    {
        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        public ModelExpression For { get; set; }

        /// <summary>
        /// ページネーション情報
        /// </summary>
        //public Pagination.PaginationInfo Info { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            #region  エラーチェック
            if (null == For)
            {
                output.PreContent.SetHtmlContent(@"<span class=""alert alert-danger"">For指定が正しくありません。</span>");
                output.Attributes.Clear();
                return;
            }
            if (null == For.Model)
            {
                output.PreContent.SetHtmlContent(@"<span class=""alert alert-danger"">指定されたモデルがnullです。</span>");
                output.Attributes.Clear();
            }
            #endregion

            //表示対象データを取り出し
            var info = For.Model as Pagination.PaginationInfo;
            if (null == info)
            {
                output.PreContent.SetHtmlContent(@"<span class=""alert alert-danger"">For式で渡されているオブジェクト種別が想定外です。</span>");
                output.Attributes.Clear();
            }


            //次のページ要素が隠されているID
            var hiddenID = $"hidden-pagination-{For.Name}";

            output.TagName = "article";
            StringBuilder sb = new StringBuilder("");

            //現在のページ情報をhidden情報に隠して保存しておく
            sb.Append($@"
<input type=""hidden"" id=""{hiddenID}"" name=""{For.Name}.displayNextPage"" value=""{info.displayNextPage}"">
");


            sb.Append(
                $@"                      <ul class=""p-pager"">
                            <!-- ページネーションはpaginationメソッドにindexを指定する事で実現する -->
                            <li>
                                <button onclick='VegaUtility.StaticFunctions.PaginationHelper.movePage(""{hiddenID}"",0)'>
                                    <<
                                </button>
                            </li>
                            <li>
                                <button onclick='VegaUtility.StaticFunctions.PaginationHelper.movePage(""{hiddenID}"",{info.getPreviousPage()})' >
                                    <
                                </button >
                            </li >
");


            foreach (var index in info.getNeighborPages())
            {
                sb.Append(
$@" 
                                <li>
                                    <button onclick='VegaUtility.StaticFunctions.PaginationHelper.movePage(""{hiddenID}"",{index})' >
                                        {index}
                                    </button>
                                </li>
                                ");
            }

            sb.Append(
$@" 
                            <li>
                                <button onclick='VegaUtility.StaticFunctions.PaginationHelper.movePage(""{hiddenID}"",{info.getNextPage()})' >
                                    >
                                </button>
                            </li>
                            <li>
                                <button onclick='VegaUtility.StaticFunctions.PaginationHelper.movePage(""{hiddenID}"",{info.getMaxPages()})' >
                                    >>
                                </button>
                            </li>
                        </ul>

");

            output.PreContent.SetHtmlContent(sb.ToString());
            output.Attributes.Clear();
        }
    }
}
