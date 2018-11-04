using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// 画面表示に関する補助関数
/// </summary>
namespace RazorPagesLearning.Service.Utility.ViewHelper
{
    /// <summary>
    /// 表示補助関数
    /// </summary>
    public static class HelperFunctions
    {

        /// <summary>
        /// 日付表示形式をシステム内部で統一された形式へ変換する
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string toFormattedString(DateTimeOffset? dt, string format = "yyyy/MM/dd HH:mm")
        {
            if (false == dt.HasValue)
            {
                return "-";
            }
            return toFormattedString(dt.Value, format);
        }

        /// <summary>
        /// 日付表示形式をシステム内部で統一された形式へ変換する
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string toFormattedString(DateTimeOffset dt, string format = "yyyy/MM/dd HH:mm")
        {
            var empty = new DateTimeOffset();
            if (empty == dt)
            {
                return "-";
            }
            else
            {
                return dt.ToString(format);
            }
        }

        /// <summary>
        /// 日時文字列を日付表示形式をシステム内部で統一された形式へ変換する
        /// </summary>
        /// <param name="dtString"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string toStringReFormatted(string dtString, string format = "yyyy/MM/dd HH:mm")
        {
            if (dtString == null)
            {
                return "";
            }

            var dt = DateTimeOffset.Parse(dtString);
            return toFormattedString(dt, format);
        }

        /// <summary>
        /// DOMAINのリストから対応するCODEの値を取得する。
        /// KINDで絞り込まれたリストをあらかじめ用意しておき引数で指定する。
        /// （毎回取得するとDBアクセス増えるので）
        /// </summary>
        /// <param name="list">KINDで絞り込まれたリスト</param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string toDomainValue(List<Data.Models.DOMAIN> list, string code)
        {
            if (list == null || code == null)
            {
                return "";
            }
            string ret = "";

            if (list.Exists(e => e.CODE == code))
            {
                ret = list.Find(e => e.CODE == code).VALUE;
            }

            return ret;
        }

        /// <summary>
        /// 文字列から数値に変換する
        /// 変換に失敗した場合は、デフォルト値を返す
        /// デフォルト値の設定がない場合は、nullを返す
        /// </summary>
        /// <param name="str">対象文字列</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns></returns>
        public static int? ConvertStringToInt(string str, int? defaultValue = null)
        {
            int result;
            if (int.TryParse(str, out result))
            {
                return result;
            }
            return defaultValue;
        }
    }
}
