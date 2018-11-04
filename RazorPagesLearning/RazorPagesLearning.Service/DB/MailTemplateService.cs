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
    /// メールテンプレートサービス
    /// </summary>
    public class MailTemplateService : DBServiceBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// コンストラクタ
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public MailTemplateService(
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

        public Result.ExecutionResult<MAIL_TEMPLATE> read(string code)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<MAIL_TEMPLATE>((ret) =>
            {
                //ToDo : 現状では1件だけと仮定して進める
                ret.result = db.MAIL_TEMPLATEs.First();
                ret.succeed = true;
            });
        }

    }
}
