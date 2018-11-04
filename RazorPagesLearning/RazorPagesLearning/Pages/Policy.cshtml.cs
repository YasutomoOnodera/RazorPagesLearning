using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RazorPagesLearning.Pages
{
    public class PolicyModel : PageModel
    {
        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 画面上に表示すべき情報
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public class ViewModel
        {
            #region 表示対象データ

            public List<POLICY> POLICies { get; set; }

            #endregion // 表示対象データ


            #region 更新対象データ

            public UpdateModel updateModel { get; set; }

            #endregion // 更新対象データ

        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 更新対象データ
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public class UpdateModel
        {
            /// <summary>
            /// パスワード桁数
            /// </summary>
            [Range(2,99, ErrorMessage="2以上99以下で入力してください。")]
            [DisplayName("パスワード桁数")]
            public int policyDigit { get; set; }

            /// <summary>
            /// パスワード変更間隔日数
            /// </summary>
            [Range(1, 999, ErrorMessage = "1以上999以下で入力してください。")]
            [DisplayName("パスワード変更間隔日数")]
            public int policyInterval { get; set; }

            /// <summary>
            /// パスワード変更通知日数
            /// </summary>
            [Range(1, 99, ErrorMessage = "1以上99以下で入力してください。")]
            [DisplayName("パスワード変更通知日数")]
            public int policyNotify { get; set; }

            /// <summary>
            /// パスワード再利用禁止回数
            /// </summary>
            [Range(0, 9, ErrorMessage = "0以上9以下で入力してください。")]
            [DisplayName("パスワード再利用禁止回数")]
            public int policyReuse { get; set; }
        }

        /// <summary>
        /// 画面上の表示項目と同期する情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        /// <summary>
        /// DBアクセスオブジェクト
        /// </summary>
        public readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;

        // コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        private readonly Service.DB.PolicyService _service;

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="db"></param>
        /// <param name="signInManager"></param>
        /// <param name="userManager"></param>
        /// <param name="logger"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public PolicyModel(RazorPagesLearning.Data.RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            this._db = db;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;

            this._service = new Service.DB.PolicyService(_db, User, _signInManager, _userManager);

            // viewモデルの更新
            this.viewModel = new ViewModel();

        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Indexアクション
        /// メッセージマスタ画面の表示
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> OnGetAsync()
        {
            await ReadViewDataFromDB();
            return Page();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 表示情報を初期化する
        /// </summary>
        /// <param name="id"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////
        private async Task<bool> ReadViewDataFromDB()
        {
            // 現在ログイン中のユーザーを取得する
            var ui = await this._userManager.GetUserAsync(User);
            if (null != ui)
            {
                viewModel.POLICies = _service.ReadAll();
            }
            else
            {
                //TODO: エラーメッセージを出力するように修正する
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saveアクション
        /// メッセージを更新
        /// </summary>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> OnPostSaveAsync()
        {
            // =====================================================================================
            // パスワード桁数の更新
            // =====================================================================================
            // サービスに渡す値をまとめる
            var NameValue = new RazorPagesLearning.Service.DB.PolicyService.NameValue
            {
                NAME = POLICY.PASSWORD_POLICY.Digit,
                VALUE = this.viewModel.updateModel.policyDigit
            };

            // パスワード桁数を更新する
            await _service.UpdateAsync(NameValue);

            // =====================================================================================
            // パスワード変更間隔日数の更新
            // =====================================================================================
            // サービスに渡す値をまとめる
            NameValue = new RazorPagesLearning.Service.DB.PolicyService.NameValue
            {
                NAME = POLICY.PASSWORD_POLICY.Interval,
                VALUE = this.viewModel.updateModel.policyInterval
            };

            // パスワード変更間隔日数を更新する
            await _service.UpdateAsync(NameValue);

            // =====================================================================================
            // パスワード変更通知日数の更新
            // =====================================================================================
            // サービスに渡す値をまとめる
            NameValue = new RazorPagesLearning.Service.DB.PolicyService.NameValue
            {
                NAME = POLICY.PASSWORD_POLICY.Notify,
                VALUE = this.viewModel.updateModel.policyNotify
            };

            // パスワード変更通知日数を更新する
            await _service.UpdateAsync(NameValue);

            // =====================================================================================
            // パスワード再利用禁止回数の更新
            // =====================================================================================
            // サービスに渡す値をまとめる
            NameValue = new RazorPagesLearning.Service.DB.PolicyService.NameValue
            {
                NAME = POLICY.PASSWORD_POLICY.Reuse,
                VALUE = this.viewModel.updateModel.policyReuse
            };

            // パスワード再利用禁止回数を更新する
            await _service.UpdateAsync(NameValue);

            // =====================================================================================
            // 更新後の値を再検索
            // =====================================================================================
            await ReadViewDataFromDB();

            return Page();
        }

    }

    
}