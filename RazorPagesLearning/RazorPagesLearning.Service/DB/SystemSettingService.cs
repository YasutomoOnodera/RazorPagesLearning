using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Claims;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// システム設定サービス
    /// </summary>
    public class SystemSettingService : DBServiceBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// コンストラクタ
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public SystemSettingService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 読み取り設定
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////

        // 取得
        public Result.ExecutionResult<IQueryable<SYSTEM_SETTING>> read()
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<IQueryable<SYSTEM_SETTING>>((ret) =>
        {
            ret.result = db.SYSTEM_SETTINGs;
            ret.succeed = true;
        });
        }

    }
}
