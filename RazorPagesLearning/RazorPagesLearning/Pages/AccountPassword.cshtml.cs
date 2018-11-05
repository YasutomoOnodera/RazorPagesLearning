using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RazorPagesLearning.Pages
{
    /// <summary>
    /// パスワード情報を変更するためのページモデル
    /// </summary>
    public class AccountPasswordModel : PageModel
    {
        #region 表示関係

        /// <summary>
        /// 画面表示情報
        /// </summary>
        public class ViewModel
        {
            /// <summary>
            /// 現在のパスワード
            /// </summary>
            [Required(ErrorMessage = "現在のパスワードを入力してください。")]
            public string nowPassword { get; set; }

            /// <summary>
            /// 新規パスワード
            /// </summary>
            [Required(ErrorMessage = "新しいパスワードを入力してください。")]
            public string newPassword { get; set; }

            /// <summary>
            /// 新規パスワード(確認)
            /// </summary>
            [Required(ErrorMessage = "新しいパスワード(確認)を入力してください。")]
            public string newPasswordConfirm { get; set; }

            #region パスワード設定

            /// <summary>
            /// 最小パスワード桁数
            /// </summary>
            public int minimumNumberOfPasswordDigits { get; set; }

            /// <summary>
            /// 最小パスワード　再使用禁止回数
            /// </summary>
            public int minimumNumberOfPasswordReuse { get; set; }

            #endregion

            #region 制御変数

            /// <summary>
            /// フレーム内部での表示か
            /// </summary>
            public bool isInFrame { get; set; }

            /// <summary>
            /// ユーザー名
            /// </summary>
            public string userName { get; set; }

            /// <summary>
            /// エラーメッセージリスト
            /// </summary>
            public List<string> errorMessage { get; set; }

            #endregion

            /// <summary>
            /// パスワードをトリムする
            /// </summary>
            public void trimPassword()
            {
                this.nowPassword = this.nowPassword.Trim();
                this.newPassword = this.newPassword.Trim();
                this.newPasswordConfirm = this.newPasswordConfirm.Trim();
            }

            /// <summary>
            /// パスワードが一致する事を確認
            /// </summary>
            /// <returns></returns>
            public bool passwordConfirm()
            {
                if (null == this.newPassword)
                {
                    return false;
                }
                if (null == this.newPasswordConfirm)
                {
                    return false;
                }

                if (this.newPassword == this.newPasswordConfirm)
                {
                    return true;
                }

                return false;
            }

            public ViewModel()
            {
                this.isInFrame = true;
                this.errorMessage = new List<string>();
            }

        }

        /// <summary>
        /// 画面表示情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        #endregion

        #region メンバーオブジェクト系

        /// <summary>
        /// DBアクセスオブジェクト
        /// </summary>
        public readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;

        // コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        #endregion

        public AccountPasswordModel(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            SignInManager<IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager,
            ILogger<LoginModel> ref_logger)
        {
            this._db = ref_db;
            this._signInManager = ref_signInManager;
            this._userManager = ref_userManager;
            this._logger = ref_logger;

            this.viewModel = initViewModel();
        }

        /// <summary>
        /// ビューモデルを初期化する
        /// </summary>
        private ViewModel initViewModel()
        {
            var tView = new ViewModel();

            //ポリシーオブジェクトを生成
            //セキュリティポリシー情報を取得する
            var policyService = new Service.DB.PolicyService(_db, User, _signInManager, _userManager);

            //パスワード有効桁数を取得
            tView.minimumNumberOfPasswordDigits
                = policyService.read(new Service.DB.PolicyService.ReadConfig
                {
                    NAME = POLICY.PASSWORD_POLICY.Digit
                }).result.VALUE;

            //パスワード再利用禁止回数を取得
            tView.minimumNumberOfPasswordReuse
                = policyService.read(new Service.DB.PolicyService.ReadConfig
                {
                    NAME = POLICY.PASSWORD_POLICY.Reuse
                }).result.VALUE;

            return tView;
        }

        public void OnGet(bool isInFrame, string userName)
        {
            //フレームの有り無しを判定する
            this.viewModel.isInFrame = isInFrame;
            this.viewModel.userName = userName;
        }

        /// <summary>
        /// パスワード更新送信
        /// </summary>
        public async Task<IActionResult> OnPost()
        {
            #region ローカル関数

            //対象ユーザー情報を取得する
            async Task<USER_ACCOUNT> LF_getTargetUser(Service.User.UserService userService)
            {
                if (null != this.viewModel.userName)
                {
                    //現在のユーザー情報を取得する
                    return userService.readWithPendingInfo(this.viewModel.userName);                    
                }
                else
                {
                   return await userService.readLoggedUserInfo();
                }
            }


            #endregion

            if (true == ModelState.IsValid)
            {
                //ユーザーサービス情報を追加
                var userService = new Service.User.UserService(_db, User, _signInManager, _userManager);

                //現在のユーザー情報を取得する
                var loggedUser = await LF_getTargetUser(userService);

                //パスワードをトリムする
                this.viewModel.trimPassword();

                #region 確認用パスワードが一致するか調べる
                if (false == this.viewModel.passwordConfirm())
                {
                    this.viewModel.errorMessage =
                        new List<string> {
                            "新しいパスワードと新しいパスワード（確認）の内容が一致しません。"
                        };
                    goto ERROR_END;
                }
                #endregion

                //パスワードの更新
                var r = await userService.update(
                    new Service.User.UserService.UpdateConfig
                    {
                        USER_ACCOUNT = loggedUser,
                        updateRange = Service.User.UserService.UpdateConfig.UpdateRange.ALL,
                        UpdatedUserAccountId = loggedUser.ID,
                        collisionDetectionTimestamp = loggedUser.timestampToString(),
                        isPasswordUpdateRequired = true,
                        password = this.viewModel.newPassword
                    });
                if (false == r.succeed)
                {
                    r.errorMessages =
                        this.viewModel.errorMessage = r.errorMessages;
                    goto ERROR_END;
                }

                if (true == this.viewModel.isInFrame)
                {
                    //成功したらHoemに移動
                    return RedirectToPage("Home");
                }
                else
                {
					//ログイン画面から来ていたらログイン
					var login = await userService.login(new Service.User.UserService.LoginConfig
					{
						password = this.viewModel.newPassword,
						userName = this.viewModel.userName
					});
					if (true == login)
					{
						return RedirectToPage("Home");
					}
                }
            }
            else
            {
                #region バリエーションに問題がある場合
                this.viewModel.errorMessage =
                    ModelState.Values.SelectMany(e => e.Errors.Select(er => er.ErrorMessage)).ToList();
                #endregion
                goto ERROR_END;
            }

            //エラーの場合の処理結果
            ERROR_END:
            return Page();
        }

    }
}