using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.ExcelAnalysis;
using RazorPagesLearning.Utility.SelectItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using RazorPagesLearning.Utility.SelectableTable;
using static RazorPagesLearning.Service.DB.StockSearchService;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Utility.Pagination;
using System.Security.Claims;

namespace RazorPagesLearning.Pages
{
    [Authorize(Policy = "PasswordExpiration")]
    public class RequestDirectModel : PageModel
    {
        // サービス
        private RazorPagesLearning.Service.DB.WkStockService WkStockService;
        private RazorPagesLearning.Service.DB.StockService StockService;
        private RazorPagesLearning.Service.DB.RequestListService RequestListService;
        private Service.User.UserService userService;
        private RazorPagesLearning.Service.DB.RequestDirectService requestDirectService;

        /// <summary>
        /// DBアクセスオブジェクト
        /// </summary>
        public readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;

        // コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RequestDirectModel(RazorPagesLearning.Data.RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            this._db = db;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;

            this.viewModel = new ViewModel();
        }

        public class ViewModel
        {
            #region 更新対象データ
            /// <summary>
            /// 在庫テーブル
            /// </summary>
            public RazorPagesLearning.Data.Models.STOCK Stock { get; set; }

            /// <summary>
            /// 在庫ワークテーブル
            /// </summary>
            public List<RazorPagesLearning.Data.Models.WK_STOCK> WkStockList { get; set; }

            /// <summary>
            /// エクセル取り込み結果
            /// </summary>
            public List<RazorPagesLearning.Service.ExcelAnalysis.wkstock_Result> Result { get; set; }

            // バーコード用
            public string Barcode { get; set; }

            // ダイレクト入庫エラーメッセージ
            public string createErrorMessage { get; set; }

            /// <summary>
            /// 在庫ワーク用
            /// </summary>
            public List<val_WK_STOCK> val_WK_STOCKs { get; set; }

            public class val_WK_STOCK
            {
                /// <summary>
                /// 在庫ワークテーブル
                /// </summary>
                public RazorPagesLearning.Data.Models.WK_STOCK WkStockList { get; set; }

                /// <summary>
                /// 選択されている
                /// </summary>
                public bool selected { get; set; }

            }

            #endregion

            #region ドメイン情報
            /// <summary>
            /// ドメインテーブル
            /// </summary>
            public RazorPagesLearning.Data.Models.DOMAIN Domain { get; set; }

            #endregion

            public ViewModel()
            {
                this.Result = new List<wkstock_Result>();
                this.val_WK_STOCKs = new List<val_WK_STOCK>();
                this.Barcode = "";
            }

        }

        /// <summary>
        /// 画面上の表示項目と同期する情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        /// <summary>
        /// 画面遷移処理
        /// </summary>
        /// <returns></returns>
        public async Task OnGetAsync()
        {
            await dispWkStock();
        }

        /// <summary>
        /// 処理種別 再入庫ダイレクトのデータを表示する。
        /// </summary>
        /// <returns></returns>
        private async Task dispWkStock()
        {
            this.WkStockService = new Service.DB.WkStockService(this._db, User, this._signInManager, this._userManager);
            var wkstkDisp = await WkStockService.read("30");

            // チェックボックスと再入庫ダイレクトのデータを表示する。
            this.viewModel.val_WK_STOCKs = wkstkDisp.result
                .Select(e =>
                new ViewModel.val_WK_STOCK
                {
                    selected = false,
                    WkStockList = e
                })
                .ToList();
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDelete()
        {
            // サービスのインスタンス作成
            this.WkStockService = new Service.DB.WkStockService(this._db, User, this._signInManager, this._userManager);
            this.StockService = new Service.DB.StockService(this._db, User, this._signInManager, this._userManager);
            this.RequestListService = new Service.DB.RequestListService(this._db, User, this._signInManager, this._userManager);
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            this.requestDirectService = new Service.DB.RequestDirectService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            var wkstk = new WK_STOCK();
            var result = new List<wkstock_Result>();
            var wk = new wkstock_Result();
            int checkCount = 0;

            using (var transaction = _db.Database.BeginTransaction())
            {
                foreach (var wkStock in this.viewModel.val_WK_STOCKs)
                {
                    //チェックされたデータ
                    if (wkStock.selected == true)
                    {
                        //var deleteWkStock = await requestDirectService.deleteDirect(wkStock.
                        //    , this.WkStockService);

                        wkstk.STOCK_ID = wkStock.WkStockList.STOCK_ID;

                        // 反映準備
                        await WkStockService.delete(wkstk.STOCK_ID, "30");

                        // 反映
                        await WkStockService.db.SaveChangesAsync();

                        checkCount++;
                    }
                }
                if (checkCount == 0)
                {
                    wk.NG_IMPORT_ERROR_MESSAGE = "依頼が選択されていません。";
                    result.Add(wk);
                    this.viewModel.Result = result;
                }
                transaction.Commit();
            }

            // 画面表示用
            await dispWkStock();

            return RedirectToPage("RequestDirect");
        }

        /// <summary>
        /// 作業依頼追加
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostRequest()
        {
            // サービスのインスタンス作成
            this.WkStockService = new Service.DB.WkStockService(this._db, User, this._signInManager, this._userManager);
            this.StockService = new Service.DB.StockService(this._db, User, this._signInManager, this._userManager);
            this.RequestListService = new Service.DB.RequestListService(this._db, User, this._signInManager, this._userManager);
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            var req = new REQUEST_LIST();
            var ReqList = new List<REQUEST_LIST>();
            var result = new List<wkstock_Result>();
            var wk = new wkstock_Result();
            int checkCount = 0;

            using (var transaction = _db.Database.BeginTransaction())
            {
                foreach (var wkStock in this.viewModel.val_WK_STOCKs)
                {
                    //チェックされたデータ
                    if (wkStock.selected == true)
                    {
                        req.STOCK_ID = wkStock.WkStockList.STOCK_ID;
                        req.USER_ACCOUNT_ID = user.ID;

                        ReqList.Add(req);

                        // 反映準備
                        await RequestListService.add(ReqList);

                        // 反映
                        await RequestListService.db.SaveChangesAsync();

                        //WK_STOCKのデータを削除する
                        await WkStockService.delete(req.STOCK_ID, "30");

                        // 反映
                        await WkStockService.db.SaveChangesAsync();

                        checkCount++;
                    }
                }
                if (checkCount == 0)
                {
                    wk.NG_IMPORT_ERROR_MESSAGE = "依頼が選択されていません。";
                    result.Add(wk);
                    this.viewModel.Result = result;
                }
                transaction.Commit();
            }

            // 画面表示用
            await dispWkStock();

            return RedirectToPage("RequestDirect");
        }

        /// <summary>
        /// ダイレクト入庫
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostCreate()
        {
            var WkStockList = new List<WK_STOCK>();
            var wkstk = new WK_STOCK();
            var result = new List<wkstock_Result>();
            var wk = new wkstock_Result();

            // サービスのインスタンス作成
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            this.WkStockService = new Service.DB.WkStockService(this._db, User, this._signInManager, this._userManager);
            this.StockService = new Service.DB.StockService(this._db, User, this._signInManager, this._userManager);
            this.requestDirectService = new Service.DB.RequestDirectService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            using (var transaction = _db.Database.BeginTransaction())
            {

                // バーコードから在庫を取得する
                var barcodeStock = await requestDirectService.createDirect(viewModel.Barcode, this.StockService, this.WkStockService, user.ID);

                // 再入庫エラーがあった場合
                if (barcodeStock.NG_IMPORT_ERROR_MESSAGE != null)
                {
                    this.viewModel.createErrorMessage = barcodeStock.NG_IMPORT_ERROR_MESSAGE;
                }
                else
                {
                    // 追加に成功したので バーコード入力欄の値を消す。
                    viewModel.Barcode = string.Empty;
                    transaction.Commit();
                }
            }
            // 画面表示用
            await dispWkStock();

            return Page();
        }
    }
}
