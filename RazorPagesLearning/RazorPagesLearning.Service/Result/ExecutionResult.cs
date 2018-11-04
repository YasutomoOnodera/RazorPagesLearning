using System;
using System.Collections.Generic;

namespace RazorPagesLearning.Service.Result
{
    /// <summary>
    /// 処理実行結果
    /// </summary>
    public class ExecutionResult<ValueType>
    {
        /// <summary>
        /// 実行結果
        /// </summary>
        public bool succeed;

        /// <summary>
        /// エラーメッセージ一覧
        /// </summary>
        public List<string> errorMessages;

        /// <summary>
        /// 発生した例外
        /// </summary>
        public Exception exception;

        /// <summary>
        /// 実行結果
        /// </summary>
        public ValueType result;


        /// <summary>
        /// NGパターンを生成する
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static ExecutionResult<ValueType> MakeError(string msg , Exception exp =null)
        {
            return new ExecutionResult<ValueType>
            {
                errorMessages = new List<string> { msg },
                exception = exp,
                succeed = false
            };
        }

    }
}
