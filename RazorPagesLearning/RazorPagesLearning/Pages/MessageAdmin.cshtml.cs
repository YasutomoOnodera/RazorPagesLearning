using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RazorPagesLearning.Pages
{
    public class MessageAdminModel : PageModel
    {
        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 画面上に表示すべき情報
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public class ViewModel
        {
            #region 表示対象データ

            public List<MESSAGE_ADMIN> MESSAGE_ADMINs { get; set; }

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
            /// ログインメッセージ
            /// </summary>
            public string messageLogin { get; set; }

            /// <summary>
            /// ホームメッセージ
            /// </summary>
            public string messageHome { get; set; }
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

        private readonly Service.DB.MessageAdminService _service;

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="db"></param>
        /// <param name="signInManager"></param>
        /// <param name="userManager"></param>
        /// <param name="logger"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public MessageAdminModel(RazorPagesLearning.Data.RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            this._db = db;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;

            this._service = new Service.DB.MessageAdminService(_db, User, _signInManager, _userManager);

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
                viewModel.MESSAGE_ADMINs = _service.ReadAll();
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
            // ログイン画面に表示するメッセージの更新
            // =====================================================================================
            // サービスに渡す値をまとめる
            var kindMessage = new RazorPagesLearning.Service.DB.MessageAdminService.KindMessage
            {
                kind = MESSAGE_ADMIN.MESSAGE_KIND.Login,
                message = this.viewModel.updateModel.messageLogin
            };

            // メッセージを更新する
            await _service.Update(kindMessage);

            // =====================================================================================
            // ホーム画面に表示するメッセージの更新
            // =====================================================================================
            // サービスに渡す値をまとめる
            kindMessage = new RazorPagesLearning.Service.DB.MessageAdminService.KindMessage
            {
                kind = MESSAGE_ADMIN.MESSAGE_KIND.Home,
                message = this.viewModel.updateModel.messageHome
            };

            // メッセージを更新する
            await _service.Update(kindMessage);

            // =====================================================================================
            // 更新後の値を再検索
            // =====================================================================================
            await ReadViewDataFromDB();

            return Page();
        }

    }
}