using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Utility.SelectItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace RazorPagesLearning.Pages
{
    [Authorize(Policy = "PasswordExpiration")]
    public class AccountInfoModel : PageModel
    {
        /// <summary>
        /// 画面上に表示すべき情報
        /// </summary>
        public class ViewModel
        {
            #region 更新対象データ
            /// <summary>
            /// ユーザー情報
            /// </summary>
            public RazorPagesLearning.Data.Models.USER_ACCOUNT USER_ACCOUNT { get; set; }

            #endregion

            /// <summary>
            /// 衝突検知用のタイムスタンプ
            /// </summary>
            public string collisionDetectionTimestamp { get; set; }

            /// <summary>
            /// エラーメッセージ
            /// </summary>
            public List<string> errorMessage { get; set; }

            public ViewModel()
            {
            }
        }

        /// <summary>
        /// 画面上の表示項目と同期する情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        /// <summary>
        /// 集配先選択モーダルで画面上の要素と連動する情報
        /// </summary>
        [BindProperty]
        public RazorPagesLearning.Pages.PopUp.SelectDeriveryModel.ViewModel selectDeriveryModelViewModel { get; set; }

        /// <summary>
        /// 集配先選択画面の処理を実装するモデル動作
        /// </summary>
        public RazorPagesLearning.Pages.PopUp.SelectDeriveryModel selectDeriveryModel { get; set; }

        //コンストラクタ引数
        private readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public AccountInfoModel(RazorPagesLearning.Data.RazorPagesLearningContext db,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            this._db = db;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;

            //viewモデルの更新
            this.viewModel = new ViewModel();

            //ポップアップ表示
            this.selectDeriveryModelViewModel = new PopUp.SelectDeriveryModel.ViewModel();
            this.selectDeriveryModel = new PopUp.SelectDeriveryModel(
                this._signInManager,
                this._logger,
                db);
        }

        /// <summary>
        /// 表示情報を初期化する
        /// </summary>
        /// <param name="id"></param>
        private async Task<bool> readViewDataFromDB()
        {
            var userService =  new Service.User.UserService(_db, User, _signInManager, _userManager);

            //現在ログイン中のユーザーを取得する
            var ui = await this._userManager.GetUserAsync(User);
            if (null != ui)
            {
                var r = userService.read(Int64.Parse(ui.UserName));
                if (null != r)
                {
                    this.viewModel.USER_ACCOUNT = r;
                    this.viewModel.collisionDetectionTimestamp 
                        = r.timestampToString();
                }
            }
            else
            {
                this.viewModel.errorMessage = 
                    new List<string> { "指定されたユーザーが存在しません。" };
            }
            return true;
        }

        /// <summary>
        /// 指定されたデータを読み取る
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            await readViewDataFromDB();
            return Page();
        }

        /// <summary>
        /// ユーザー情報を読み込んで同期する
        /// </summary>
        /// <returns></returns>
        private async Task<bool> raedAndSyncUserData()
        {
            await readViewDataFromDB();
            return true;
        }

        /// <summary>
        /// 画面内の各種変更が発生した場合
        /// </summary>
        /// <param name="id"></param>
        public async Task<IActionResult> OnPost()
        {
            //データを同期する
            await raedAndSyncUserData();

            return Page();
        }

        /// <summary>
        /// 
        /// 集配先検索ページで検索させる
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostSearchSelectDeriveryAsync()
        {
            //ユーザー情報を同期する
            await raedAndSyncUserData();

            //集配先管理ページの情報を読み取る
            this.selectDeriveryModel.OnPostSearch(this.selectDeriveryModelViewModel);

            return Page();
        }

        // <summary>
        /// データをDBに登録する
        /// </summary>
        public async Task<IActionResult> OnPostSaveAsync()
        {
            #region ローカル関数

            ///バリデーション結果のうち、意図的に無視項目を除外したバリデーション結果を作成する
            List<ModelStateEntry> LF_makeIgnoredItemsFilteredModelState()
            {
                var r = ModelState.Keys
                    //パスワード列を除外する
                    .Where(e =>
                    false == System.Text.RegularExpressions.Regex.IsMatch(
                    e, @".*PASSWORD$"))
                    .Select(e => ModelState[e])
                    .ToList();

                return r;
            }

            #endregion

            //DB定義上、パスワード列は必須となっている。
            //このため、バリデーションバリデーション対象となっている。
            //このページではパスワードは更新対象とならないのでパスワード列は無視して
            //バリデーションチェックを行う
            var filteredModelState = LF_makeIgnoredItemsFilteredModelState();

            if (0 == filteredModelState.Where(e => 0 != e.Errors.Count).Count())
            {
                #region バリデーションが正しい場合
                {
                    //既存情報の更新
                    #region 既存データ更新
                    var userService = new Service.User.UserService(_db, User, _signInManager, _userManager);

                    //自分のユーザーIDで更新する
                    var nowUser = await userService.readLoggedUserInfo();
                    this.viewModel.USER_ACCOUNT.ID = nowUser.ID;

                    var r = await userService.update(
                        new Service.User.UserService.UpdateConfig {
                            updateRange = Service.User.UserService.UpdateConfig.UpdateRange.AccountInfo,
                            USER_ACCOUNT = this.viewModel.USER_ACCOUNT,
                            collisionDetectionTimestamp = this.viewModel.collisionDetectionTimestamp
                        });
                    if (false == r.succeed)
                    {
                        //エラーの場合
                        this.viewModel.errorMessage = r.errorMessages;
                    }

                    #endregion
                }
            }
            #endregion
            else
            {
                #region バリエーションに問題がある場合
                this.viewModel.errorMessage = filteredModelState
                    .SelectMany(e => e.Errors.Select(er => er.ErrorMessage)).ToList();
                #endregion
            }

            //ポップアップ表示が開いた状態となっているので、
            //一度閉じる
            this.selectDeriveryModelViewModel.isShowModal = false;

            return Page();
        }
    }
}