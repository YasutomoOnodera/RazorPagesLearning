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
using RazorPagesLearning.Service.User;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Service.Utility.ViewHelper;

namespace RazorPagesLearning.Pages
{
    [Authorize(Policy = "PasswordExpiration")]
    public class RequestImportModel : PageModel
    {
        
        /// <summary>
        /// 各種サービス
        /// </summary>
        private RazorPagesLearning.Service.DB.WkStockService wkStockService;
        private RazorPagesLearning.Service.DB.StockService stockService;
        private RazorPagesLearning.Service.DB.RequestListService requestListService;
        private Service.User.UserService userService;
        private Service.DB.DomainService domainService;

        /// <summary>
        /// DBアクセスオブジェクト
        /// </summary>
        public readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;

        /// <summary>
        /// コンストラクタ引数
        /// </summary>
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RequestImportModel(RazorPagesLearning.Data.RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager,
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
            #region ファイルアップロード
            /// <summary>
            /// ファイルアップロード
            /// </summary>
            [Required]
            public IFormFile UploadFile { get; set; }

            /// <summary>
            /// フォーマット選択(新規入庫・その他依頼)
            /// </summary>
            [Required]
            public SelectItemSet SetItem { get; set; }
            #endregion


            #region 取り込みモード
            /// <summary>
            /// 取り込みモード
            /// </summary>
            public string Selected { get; set; }
            #endregion


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
            /// 作業依頼一覧テーブル
            /// </summary>
            public RazorPagesLearning.Data.Models.REQUEST_LIST RequestList { get; set; }

            /// <summary>
            /// エクセル取り込み結果
            /// </summary>
            public List<RazorPagesLearning.Service.ExcelAnalysis.wkstock_Result> Result { get; set; }

            #endregion


            #region ドメイン情報
            /// <summary>
            /// ドメインテーブル
            /// </summary>
            public RazorPagesLearning.Data.Models.DOMAIN Domain { get; set; }

            #endregion

            /// <summary>
            /// VieeModel
            /// </summary>
            public ViewModel()
            {
                // アイテム
                this.SetItem = new SelectItemSet();
                this.Result = new List<wkstock_Result>();

                this.SetItem.display = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "新規入庫",
                        Text = "新規入庫"
                    },
                    new SelectListItem
                    {
                        Value = "その他依頼",
                        Text = "その他依頼"
                    }
                };
            }

        }

        /// <summary>
        /// 画面上の表示項目と同期する情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        /// <summary>
        /// ユーザー情報
        /// </summary>
        public USER_ACCOUNT USER_ACCOUNT;


        /// <summary>
        /// indexアクション
        /// </summary>
        public async Task OnGetAsync()
        {
            await this.init();

            var wkstkDisp = await wkStockService.read("20");

            // 倉庫管理番号で新規入庫・その他依頼か判別
            var typeCheck = wkstkDisp.result.FirstOrDefault();
            if (typeCheck == null)
            {
                // 何もしない
            }
            else if (typeCheck.STORAGE_MANAGE_NUMBER == "")
            {
                this.viewModel.SetItem.selected = "新規入庫";
            }
            else
            {
                this.viewModel.SetItem.selected = "その他依頼";
            }

            this.viewModel.WkStockList = wkstkDisp.result.ToList();
        }

        /// <summary>
        /// createアクション
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostCreateAsync()
        {
            await this.init();

            // 取り込んだファイルを一時ファイルに
            var filePath = System.IO.Path.GetTempFileName();

            try
            {
                // WK_STOCKのデータを削除する
                await WK_STOCK_Clear();

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    if (viewModel.UploadFile == null)
                    {
                        // 失敗しても無視する
                    }
                    else
                    {
                        await viewModel.UploadFile.CopyToAsync(fileStream);
                    }
                }
                // 新規入庫かその他依頼かで分岐
                if (viewModel.SetItem.selected == "新規入庫")
                {
                    // エクセルシート解析(新規入庫)
                    var newana = new NewFormatAnalysis();
                    var newFormatResult = newana.Analysis(filePath);
                                        
                    this.viewModel.Result = newFormatResult;

                    using (var transaction = _db.Database.BeginTransaction())
                    {
                        var wkStockList  = this.wkStockService.addNewWkstock(newFormatResult, this.USER_ACCOUNT.ID);
                        
                        // 画面表示用
                        this.viewModel.WkStockList = wkStockList.Result;
                        transaction.Commit();
                    }
                }
                else
                {
                    // エクセルシート解析(その他依頼)
                    var ana = new OtherFormatAnalysis(this.userService, this.stockService);
                    var otherFormatResult = ana.Analysis(filePath);

                    this.viewModel.Result = otherFormatResult;

                    using (var transaction = _db.Database.BeginTransaction())
                    {
                        var wkStockList = this.wkStockService.addOtherWkstock(otherFormatResult, this.USER_ACCOUNT.ID);                      

                        // 画面表示用
                        this.viewModel.WkStockList = wkStockList.Result;
                        transaction.Commit();
                    }

                }
            }

            finally
            {
                // 問答無用で消す
                try
                {
                    // 一時ファイルを削除
                    System.IO.File.Delete(filePath);
                }
                catch
                {
                    // 失敗しても無視する
                }
            }

            return Page();
        }

        /// <summary>
        /// clearアクション
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostClear()
        {
            await this.init();

            using (var transaction = _db.Database.BeginTransaction())
            {
                // WK_STOCKのデータを削除する
                await WK_STOCK_Clear();

                transaction.Commit();
            }
            return Page();
        }

        private async Task WK_STOCK_Clear()
        {

            // WK_STOCKの中身を削除
            await wkStockService.delete("20");

        }

        /// <summary>
        /// requestアクション
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostRequest()
        {
            await this.init();

            using (var transaction = _db.Database.BeginTransaction())
            {
                var requestList = await requestListService.addRequest(this.USER_ACCOUNT.ID, viewModel.SetItem.selected, this.wkStockService, this.stockService);

                var requestListEroor = requestList.FirstOrDefault();

                if(requestListEroor == null)
                {
                    // 追加成功
                    this.viewModel.Result = requestList;
                    transaction.Commit();
                }
                else
                {
                    // 追加失敗
                    this.viewModel.Result = requestList;
                    transaction.Rollback();
                }              
            }
            await OnGetAsync();
            return Page();
        }

        ///// <summary>
        ///// 新規フォーマットダウンロード
        ///// </summary>
        ///// <returns></returns>
        //public async Task<IActionResult> OnPostNewFormatAsync()
        //{
        //    #region ローカル関数

        //    //エクセルテンプレートファイルを生成する
        //    async Task<string> LF_createTemplateFile(string workPath)
        //    {
        //        await this.init();

        //        // ユーザー情報の取得
        //        var user = await userService.read(new
        //            UserService.ReadFromClaimsPrincipalConfig
        //        { userInfo = User });

        //        //var maker = new RazorPagesLearning.Report.RequestImportNewFormat
        //        //    (new RazorPagesLearning.Report.RequestImportNewFormat.RequestImportConfig
        //        //    {
        //        //        domainService = domainService,
        //        //        user_account = user
        //        //    },
        //        //    workPath);

        //        ////テンプレートファイル生成
        //        //return maker.doCreateTemplate();
        //    }

        //    #endregion

        //    ////一時ディレクトリの中で作業する
        //    //await RazorPagesLearning.Utility.ReportHelper.HelperFunctions.TemporaryDirectoryAccompanyBlock
        //    //    (async (workPath) =>
        //    //    {
        //    //        //フォーマットファイルを生成する
        //    //        var targetFilePath = await LF_createTemplateFile(workPath);

        //    //        //HTTP応答に書き出す
        //    //        RazorPagesLearning.Utility.ReportHelper.HelperFunctions.writeExistingFile(
        //    //            new Utility.ReportHelper.HelperFunctions.WriteExistingFileConfig
        //    //            {
        //    //                ContentType = "application/excel",
        //    //                fileName = "NewInportFormat.xlsx",
        //    //                target = this.Response,
        //    //                targetFilePath = targetFilePath
        //    //            });

        //    //        return true;
        //    //    });

        //    return Page();
        //}

        ///// <summary>
        ///// その他フォーマットダウンロード
        ///// </summary>
        ///// <returns></returns>
        //public IActionResult OnPostOtherFormat()
        //{
        //    //System.Net.WebClient wc = new System.Net.WebClient();
        //    //wc.DownloadFile("C:\\Users\\y-asa\\Documents\\test\\ot.xlsx", @"その他依頼フォーマット");
        //    //wc.Dispose();
        //    return RedirectToPage("../Format/その他依頼フォーマット.xlsx");

        //}

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <returns></returns>
        public async Task init()
        {
            // インスタンス生成
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            this.domainService = new Service.DB.DomainService(this._db, User, this._signInManager, this._userManager);
            this.wkStockService = new Service.DB.WkStockService(this._db, User, this._signInManager, this._userManager);
            this.requestListService = new Service.DB.RequestListService(this._db, User, this._signInManager, this._userManager);
            this.stockService = new Service.DB.StockService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);
            this.USER_ACCOUNT = user;
        }
    }
}