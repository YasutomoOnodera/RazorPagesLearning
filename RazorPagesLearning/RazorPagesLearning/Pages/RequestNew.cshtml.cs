using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// 追加
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RazorPagesLearning.Pages
{
    [Authorize(Roles = "PasswordExpiration, Admin, ShipperEditing, Worker")]
    public class RequestNewModel : PageModel
    {
        // 定義
        //==============================================================================================================
        // TODO：ドメインから取得してくる値。どこかにまとめて定数で持つ？？
        private const string domainKindClass1 = "00010002";
        private const string domainKindClass2 = "00010003";

        /// <summary>
        /// モード情報
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// 新規登録
            /// </summary>
            create = 0,

            /// <summary>
            /// 編集
            /// </summary>
            update = 1
        }

        // class
        //==============================================================================================================
        #region class
        /// <summary>
        /// 画面と連動する情報
        /// </summary>
        public class ViewModel
        {
            // エラーメッセージ格納用
            public List<string> ErrorMesList { get; set; }

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

            /// <summary>
            /// 入力データ
            /// </summary>
            public WK_STOCK inputData { get; set; }

            /// <summary>
            /// 部課一覧
            /// </summary>
            public List<SelectListItem> userDepartmentList { get; set; }

            /// <summary>
            /// 部課選択値
            /// </summary>
            public string selectedUserDepartment { get; set; }

            /// <summary>
            /// 区分1
            /// </summary>
            public List<SelectListItem> class1List { get; set; }

            /// <summary>
            /// 区分1選択値
            /// </summary>
            public string selectedClass1 { get; set; }

            /// <summary>
            /// 区分2
            /// </summary>
            public List<SelectListItem> class2List { get; set; }

            /// <summary>
            /// 区分2選択値
            /// </summary>
            public string selectedClass2 { get; set; }

            /// <summary>
            /// 編集モード情報
            /// </summary>
            public RequestNewModel.Mode Mode { get; set; }

            /// <summary>
            /// 編集時に選択されている行情報
            /// </summary>
            public int selectDataNo { get; set; }

            /// <summary>
            /// 編集時に選択されている在庫ワークID情報
            /// </summary>
            public int selectDataId { get; set; }


        }
        #endregion

        // 変数
        //==============================================================================================================
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
        /// DBアクセスオブジェクト
        /// </summary>
        public readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;

        // DB
        private Service.User.UserService userService;
        private Service.DB.DomainService domainService;
        private Service.DB.UserDepartmentService userDepartmentService;
        private Service.DB.WkStockService wkStockService;
        private Service.DB.RequestListService requestListService;
        private Service.DB.StockService stockService;

        // コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        // 処理
        //==============================================================================================================
        // コンストラクタ
        public RequestNewModel(RazorPagesLearning.Data.RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            // memo：Userがこのタイミングではまだnullなので、
            //       DBのインスタンスはコンストラクタでは生成しないこと。

            this._db = db;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;

            // BindPropertyのものはここで初期化
            this.viewModel = new ViewModel();
        }

        // アクション関連の処理
        //==============================================================================================================
        /// <summary>
        /// indexアクション
        /// 画面遷移時処理
        /// </summary>
        public async Task<IActionResult> OnGetAsync()
        {
            await this.init();

            this.viewModel.Mode = RequestNewModel.Mode.create;

            this.viewModel.inputData = new WK_STOCK();

            await this.view();

            return Page();
        }

        /// <summary>
        /// clearアクション
        /// モード初期化処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostClearAsync()
        {
            // モード初期化
            this.viewModel.Mode = RequestNewModel.Mode.create;

            await this.init();

            // 入力データをクリアする
            this.viewModel.inputData = new WK_STOCK();
            // セレクトボックスの内容をクリア
            this.ModelState["viewModel.selectedUserDepartment"].RawValue = "";
            this.ModelState["viewModel.selectedClass1"].RawValue = "";
            this.ModelState["viewModel.selectedClass2"].RawValue = "";

            await this.view();

            return Page();
        }

        /// <summary>
        /// createアクション
        /// 新規登録、更新処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostCreateAsync()
        {
            await this.init();

            // TODO：バリデーションを書く
            try
            {
                this.viewModel.inputData.DEPARTMENT_CODE = this.viewModel.selectedUserDepartment;
                this.viewModel.inputData.CLASS1 = this.viewModel.selectedClass1;
                this.viewModel.inputData.CLASS2 = this.viewModel.selectedClass2;

                if (this.viewModel.Mode == RequestNewModel.Mode.create)
                {
                    // 新規登録
                    await this.wkStockService.add(
                        this.viewModel.inputData,
                        "10",   // 処理種別
                        "10");  // 登録待ちステータス
                }
                else
                {
                    // 更新
                    await this.wkStockService.update(
                        this.viewModel.selectDataId,
                        this.viewModel.inputData,
                        "10",
                        "10");
                }

                // 成功したら、初期化する
                this.viewModel.inputData = new WK_STOCK();
                this.viewModel.selectDataId = 0;
                this.viewModel.selectDataNo = 0;
                this.viewModel.Mode = RequestNewModel.Mode.create;
                // セレクトボックスの内容をクリア
                this.ModelState["viewModel.selectedUserDepartment"].RawValue = "";
                this.ModelState["viewModel.selectedClass1"].RawValue = "";
                this.ModelState["viewModel.selectedClass2"].RawValue = "";

            }
            catch (Exception)
            {
                // エラーならエラーメッセージを取得して終了
                //this.viewModel.ErrorMesList.Add("データの登録に失敗しました。");
                this.viewModel.ErrorMesList = new List<string> { "データの登録に失敗しました。" };
            }


            await this.view();

            return Page();
        }

        /// <summary>
        /// updateアクション
        /// 新規登録データの編集処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            this.viewModel.Mode = RequestNewModel.Mode.update;

            await this.init();

            try
            {
                // 該当するデータの取得
                var DataQuery = await this.wkStockService.read("10");   // TODO：定数
                var target = DataQuery.result.FirstOrDefault(e => e.ID == this.viewModel.selectDataId);

                // 値のセット
                this.viewModel.inputData = target;
                this.viewModel.selectedUserDepartment = target.DEPARTMENT_CODE;
                this.viewModel.selectedClass1 = target.CLASS1;
                this.viewModel.selectedClass2 = target.CLASS2;

                // memo：↑のviewModelの値を書き換えるだけでは、選択状態が再現されなかったため、ここの情報も更新をかける
                //       それによって、セレクトボックスの状態が再現されることを確認
                this.ModelState["viewModel.selectedUserDepartment"].RawValue = target.DEPARTMENT_CODE;
                this.ModelState["viewModel.selectedClass1"].RawValue = target.CLASS1;
                this.ModelState["viewModel.selectedClass2"].RawValue = target.CLASS1;
            }
            catch (Exception)
            {
                // err
                // データ取得失敗
            }

            await this.view();

            return Page();
        }

        /// <summary>
        /// deleteアクション
        /// 新規登録データの削除処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            await this.init();

            int deleteCount = 0;
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var x in this.viewModel.val_WK_STOCKs)
                    {
                        if (x.selected == true)
                        {
                            // 削除
                            await this.wkStockService.deleteWkStockAndStock(x.WkStockList.ID, "10");

                            deleteCount++;
                        }
                    }

                    if (deleteCount > 0)
                    {
                        // 適応
                        transaction.Commit();
                    }
                    else
                    {
                        // 削除対象なし
                        // エラー内容確保
                    }
                }
                catch (Exception)
                {
                    // エラーならエラーメッセージを取得して終了
                    //this.viewModel.ErrorMesList.Add("データの削除に失敗しました。");
                    this.viewModel.ErrorMesList = new List<string> { "データの削除に失敗しました。" };
                    transaction.Rollback();
                }
            }

            await this.view();

            return Page();
        }

        /// <summary>
        /// requestアクション
        /// 作業依頼指示への追加処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostRequestAsync()
        {
            await this.init();

            // トランザクション開始
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    int checkCount = 0;

                    // チェックされた対象の在庫ワークIDを取得
                    foreach (var x in this.viewModel.val_WK_STOCKs)
                    {
                        //チェックされたデータ
                        if (x.selected == true)
                        {
                            // データ更新
                            await this.requestListService.addDataAndDeleteWkStock(x.WkStockList.ID);

                            checkCount++;
                        }
                    }
                    if (checkCount > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        // 対象なし
                        // エラー内容確保
                    }
                }
                catch (Exception)
                {
                    // エラーメッセージを取得して終了
                    //this.viewModel.ErrorMesList.Add("データの登録に失敗しました。");
                    this.viewModel.ErrorMesList = new List<string> { "データの登録に失敗しました。" };
                    transaction.Rollback();
                }
            }

            // 画面表示用
            await view();

            return Page();
        }

        // 実装処理
        //==============================================================================================================
        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <returns></returns>
        public async Task init()
        {
            // インスタンス生成
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            this.domainService = new Service.DB.DomainService(this._db, User, this._signInManager, this._userManager);
            this.userDepartmentService = new Service.DB.UserDepartmentService(this._db, User, this._signInManager, this._userManager);
            this.wkStockService = new Service.DB.WkStockService(this._db, User, this._signInManager, this._userManager);
            this.requestListService = new Service.DB.RequestListService(this._db, User, this._signInManager, this._userManager);
            this.stockService = new Service.DB.StockService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);
            this.USER_ACCOUNT = user;

            // ユーザーの部課情報を取得
            List<USER_DEPARTMENT> userDepartmentList = this.userDepartmentService.readAtUserAccountId(user.ID);
            this.viewModel.userDepartmentList = userDepartmentList.Select(e => new SelectListItem() { Value = e.DEPARTMENT_CODE, Text = e.DEPARTMENT_ADMIN.DEPARTMENT_NAME }).ToList();

            // 区分1を取得
            List<DOMAIN> class1List = this.domainService.getCodeList(RequestNewModel.domainKindClass1).ToList();
            this.viewModel.class1List = class1List.Select(e => new SelectListItem() { Value = e.CODE, Text = e.VALUE }).ToList();

            // 区分2を取得
            List<DOMAIN> class2List = this.domainService.getCodeList(RequestNewModel.domainKindClass2).ToList();
            this.viewModel.class2List = class2List.Select(e => new SelectListItem() { Value = e.CODE, Text = e.VALUE }).ToList();

            // 
            this.viewModel.ErrorMesList = new List<string>();

        }

        /// <summary>
        /// 一覧表示データ取得処理
        /// </summary>
        /// <returns></returns>
        public async Task view()
        {
            var wkstkDisp = await this.wkStockService.read("10");

            // チェックボックスと一覧のデータを表示する。
            this.viewModel.val_WK_STOCKs = wkstkDisp.result
                .Select(e =>
                new ViewModel.val_WK_STOCK
                {
                    selected = false,
                    WkStockList = e
                })
                .ToList();
        }

    }
}