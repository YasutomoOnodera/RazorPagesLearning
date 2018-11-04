using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Utility.TempDataHelper
{
    /// <summary>
    /// TempDataに型情報を出し入れするための変換関数群
    /// 
    /// [参考]
    /// https://beachside.hatenablog.com/entry/2018/03/05/180000
    /// 
    /// </summary>
    public static class TempDataDictionaryExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            tempData.TryGetValue(key, out var obj);
            return obj == null ? null : JsonConvert.DeserializeObject<T>(obj.ToString());
        }
    }
}
