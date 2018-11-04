using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Authentication.Policy
{
    /// <summary>
    /// パスワードの有効期限を確認するハンドラ
    /// </summary>
    public class PasswordExpirationHandler : AuthorizationHandler<PasswordExpirationRequirement>
    {

        /// <summary>
        /// DBアクセスオブジェクト
        /// </summary>
        public RazorPagesLearning.Data.RazorPagesLearningContext _db;

        //コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public PasswordExpirationHandler(RazorPagesLearning.Data.RazorPagesLearningContext db,
             SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            this._db = db;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;

        }

        protected override async Task HandleRequirementAsync
            (AuthorizationHandlerContext context, PasswordExpirationRequirement requirement)
        {
            //ユーザーログインサービスを取り込む
            var userService = new RazorPagesLearning.Service.User.UserService(
                this._db,
                context.User,
                this._signInManager,
                this._userManager);

            //アカウント情報
            var userInfo = await userService.readLoggedUserInfo();
            if (null == userInfo)
            {
                //認証NG扱いとする
                context.Fail();
                return;
            }

            var userInfoPending = userService.readWithPendingInfo(userInfo.USER_ID);

            //ログイン無効化フラグが効いていたらログインを無効化する
            if (false == userInfoPending.LOGIN_ENABLE_FLAG)
            {
                //認証NG扱いとする
                context.Fail();
                return;
            }

            //パスワードの有効期限が切れているかチェックする
            var r = userService.passwordExpirationCheck(userInfoPending);
            if (true == r)
            {
                //認証NG扱いとする
                context.Fail();
                return;
            }

            //確認成功と判断する
            context.Succeed(requirement);

            return;
        }

    }
}
