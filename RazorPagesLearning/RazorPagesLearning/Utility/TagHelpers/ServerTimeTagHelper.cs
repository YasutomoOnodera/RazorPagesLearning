using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.TagHelpers
{
    public class ServerTimeTagHelper : TagHelper
    {

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";

            //現在時間を取得する
            var dt = DateTimeOffset.Now;
            var jpString = dt.ToString("yyyy/M/d(ddd) H:m", new CultureInfo("en-US"));

            // DateTime型→ISO8601形式の文字列
            string ISO8601String = dt.ToString("yyyyMMddTHHmmss") + dt.ToString("zzz").Replace(":", "");

            output.PreContent.SetHtmlContent($@"
                    <img src = ""../img/clock.svg"" alt = """" width = ""19"" height=""19"" >
                    <span id=""server-time-on-when-page-rendered-view"">{jpString}</span>
                    <span hidden id=""server-time-on-when-page-rendered-src"">{ISO8601String}</span>
                                ");
            output.Attributes.Clear();
        }
    }
}
