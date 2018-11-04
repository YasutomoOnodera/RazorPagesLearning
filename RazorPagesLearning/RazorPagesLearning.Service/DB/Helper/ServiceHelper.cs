using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB.Helper
{
    /// <summary>
    /// サービスの補助
    /// </summary>
    public static class ServiceHelper
    {
        /// <summary>
        /// 例外の後処理付きでデータ処理を行う
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static Result.ExecutionResult<T> DoOperationWithErrorManagement<T>(Action<Result.ExecutionResult<T>> operation)
        {
            var r = new Result.ExecutionResult<T>
            {
                succeed = false
            };
            try
            {
                operation(r);
            }
            catch (Exception e)
            {
                r.succeed = false;
                r.exception = e;
                if (null == r.errorMessages)
                {
                    r.errorMessages = new List<string>();
                }
                r.errorMessages.Add(e.Message);
            }

            return r;
        }


        /// <summary>
        /// 非同期版
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static async Task<Result.ExecutionResult<T>> doOperationWithErrorManagementAsync<T>
            (Func<Result.ExecutionResult<T> ,Task<Result.ExecutionResult<T>>> operation)
        {
            var r = new Result.ExecutionResult<T>
            {
                succeed = false
            };
            try
            {
                await operation(r);
            }
            catch (Exception e)
            {
                r.succeed = false;
                r.exception = e;
                if (null == r.errorMessages)
                {
                    r.errorMessages = new List<string>();
                }
                r.errorMessages.Add(e.Message);
            }

            return r;
        }

        public static Result.DefaultExecutionResult doOperationWithErrorManagement
            (Action<Result.DefaultExecutionResult> operation)
        {
            var r = new Result.DefaultExecutionResult
            {
                succeed = false
            };
            try
            {
                operation(r);
            }
            catch (Exception e)
            {
                r.succeed = false;
                r.exception = e;
                if (null == r.errorMessages)
                {
                    r.errorMessages = new List<string>();
                }
                r.errorMessages.Add(e.Message);
            }

            return r;
        }


        public static async Task<Result.DefaultExecutionResult>
            doOperationWithErrorManagementAsync
            (Func<Result.DefaultExecutionResult,Task<Result.DefaultExecutionResult>> operation)
        {
            var r = new Result.DefaultExecutionResult
            {
                succeed = false
            };
            try
            {
               await  operation(r);
            }
            catch (Exception e)
            {
                r.succeed = false;
                r.exception = e;
                if (null == r.errorMessages)
                {
                    r.errorMessages = new List<string>();
                }
                r.errorMessages.Add(e.Message);
            }

            return r;
        }

    }
}
