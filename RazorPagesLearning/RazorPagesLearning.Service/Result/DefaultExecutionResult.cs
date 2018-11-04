using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.Result
{
    /// <summary>
    /// 標準の実行結果
    /// (戻り値なし)
    /// </summary>
    public class DefaultExecutionResult : ExecutionResult<Object>
    {

        /// <summary>
        /// NGパターンを生成する
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static new DefaultExecutionResult makeError(string msg, Exception exp = null)
        {
            return new DefaultExecutionResult
            {
                errorMessages = new List<string> { msg },
                exception = exp,
                succeed = false
            };
        }

    }
}
