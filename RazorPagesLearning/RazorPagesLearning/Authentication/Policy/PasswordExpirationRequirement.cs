using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Authentication.Policy
{
    /// <summary>
    /// パスワードの有効期限確認用の要件オブジェクト
    /// </summary>
    public class PasswordExpirationRequirement : IAuthorizationRequirement
    {
    }
}
