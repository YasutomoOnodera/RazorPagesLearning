using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using static RazorPagesLearning.Service.User.UserService;

namespace RazorPagesLearning.Pages
{
	/// <summary>
	/// ログインページモデル
	/// </summary>
    public class IndexModel : PageModel
    {
		#region 画面とバインディングする情報

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 画面上に表示する項目
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public class ViewModel 
        {
            /// <summary>
            /// ユーザー名
            /// </summary>
            [BindProperty]
            [Required(ErrorMessage = "{0}に値を入力してください。")]
            public string userName { get; set; }

            /// <summary>
            /// パスワード
            /// </summary>
            [BindProperty]
            [Required]
            public string password { get; set; }

            /// <summary>
            /// エラー一覧
            /// </summary>
            public List<string> errorList { get; set; }

			/// <summary>
			/// ユーザ通知領域のメッセージ
			/// </summary>
			public String message { get; set; }

            public ViewModel()
            {
                this.errorList = new List<string>();
            }
        }

        /// <summary>
        /// 画面と共用するデータ
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        #endregion

        //コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        private readonly RazorPagesLearning.Service.User.UserService userService;
		private readonly RazorPagesLearning.Service.DB.MessageAdminService messageAdminService;

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="signInManager"></param>
		/// <param name="userManager"></param>
		/// <param name="logger"></param>
		/// <param name="db"></param>
		////////////////////////////////////////////////////////////////////////////////////////////
		public IndexModel(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger,
            RazorPagesLearning.Data.RazorPagesLearningContext db)
        {
            //DI情報の取り込み
            this._signInManager = signInManager;
            this._logger = logger;

            this.userService = new Service.User.UserService(db, User, signInManager, userManager);
			this.messageAdminService = new Service.DB.MessageAdminService(db, User, signInManager, userManager);

			this.viewModel = new ViewModel();

            //テスト用
            {
                this.viewModel.userName = "MWLSystemAdmin";
                this.viewModel.password = "MWLSystemAdmin2018";
            }
        }

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// indexアクション
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public IActionResult OnGetAsync()
		{
			var messageAdmin = messageAdminService.Read(new Service.DB.MessageAdminService.ReadConfig
			{
				KIND = Data.Models.MESSAGE_ADMIN.MESSAGE_KIND.Login
			});

			viewModel.message = messageAdmin.result.MESSAGE;

			return Page();
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// loginアクション
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostLoginAsync()
        {

            #region ローカル関数

            //ユーザー情報を読み取る
            Tuple<bool, HoldsPendingInformation_USER_ACCOUNT> readUserInfo()
            {
                try
                {
                    return Tuple.Create(true,
                        this.userService.readWithPendingInfo(this.viewModel.userName));
                }
                catch
                {
                }
                return Tuple.Create<bool, HoldsPendingInformation_USER_ACCOUNT>(false, null);
            }

            //ユーザー更新が必要か
            bool needUpdating(HoldsPendingInformation_USER_ACCOUNT tuser)
            {
                return this.userService.checkPasswordUpdateRequest(tuser);
            }

            #endregion

            //ログイン処理を行う
            if (ModelState.IsValid)
            {
                #region バリデーションが正しい

                //1. 該当ユーザー名が存在するか確認する
                var tUI = readUserInfo();
                if (false == tUI.Item1)
                {
                    goto ErrorEnd;
                }

                if (false == tUI.Item2.LOGIN_ENABLE_FLAG)
                {
                    //ログインが無効化されている
                    goto ErrorEnd;
                }

                //2. 該当ユーザーに対して、パスワードの更新が必要な状態であるか判定する
                if (true == needUpdating(tUI.Item2))
                {
                    //パスワード更新画面へ遷移する
                    return RedirectToPage("./AccountPassword", new { isInFrame = "false" , userName = this.viewModel.userName });
                }

                //3.問題ないので通常ログインを行う
                var r = await userService.login(new Service.User.UserService.LoginConfig
                {
                    password = this.viewModel.password,
                    userName = this.viewModel.userName
                });
                if (true == r)
                {
                    //ログインに成功したのでリダイレクトさせる
                    return RedirectToPage("./Home");
                }

                goto ErrorEnd;

                #endregion
            }

            //エラー発生時
            ErrorEnd:;

            this.viewModel.errorList.Add("ユーザーID　または　パスワードが正しくありません。");

            return Page();
        }
    }
}
