using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.User;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Utility.Pagination;
using RazorPagesLearning.Utility.SelectableTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using static RazorPagesLearning.Pages.WatchlistModel.ViewModel;
//using static RazorPagesLearning.Report.WatchListExcel;
//using static RazorPagesLearning.Report.WatchListReport;

namespace RazorPagesLearning.Pages
{
    [Authorize(Roles = "Admin", Policy = "PasswordExpiration")]
    public class WatchlistModel : PageModel
    {
        /// <summary>
        /// サービス
        /// </summary>
        private RazorPagesLearning.Service.User.UserService userService;
        private RazorPagesLearning.Service.DB.WkStockService WkStockService;
        private RazorPagesLearning.Service.DB.StockService StockService;
        private RazorPagesLearning.Service.DB.RequestListService RequestListService;
        private RazorPagesLearning.Service.DB.WatchlistService WatchlistService;
        private RazorPagesLearning.Service.DB.RequestHistoryDetailService RequestHistoryDetailService;
        private RazorPagesLearning.Service.DB.WkTableSelectionSettingService wkTableSelectionSettingService;

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
        public WatchlistModel(RazorPagesLearning.Data.RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            this._db = db;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;

            this.viewModel = new ViewModel();
            this.paginationInfo = new PaginationInfo();
        }

        /// <summary>
        /// 画面上の在庫テーブルにおけるチェックボックスのチェック状態を管理する
        /// </summary>
        class WacthListCheckStatusManagement : CheckStatusManagement<WATCHLIST>
        {
            public override TableSelectionSettingService.TrackingIdentifier translationRelationIdentifier(WATCHLIST table)
            {
                return new Service.DB.TableSelectionSettingService.TrackingIdentifier
                {
                    // memo：チェックボックスの状態を取得してくるための主キーとなるもの
                    //       ユーザーアカウントIDは既に設定されているので、それ以外
                    dataId = table.STOCK_ID
                };
            }

            // コンストラクタ
            public WacthListCheckStatusManagement(RazorPagesLearning.Data.RazorPagesLearningContext db,
                SignInManager<IdentityUser> signInManager,
                UserManager<IdentityUser> userManager,
                PaginationInfo paginationInfo,
                ClaimsPrincipal ref_user) : base(
                    db,
                    signInManager,
                    userManager,
                    paginationInfo,
                    ref_user
                    )
            {
            }
        }

        public class ViewModel : SelectableTableViewModelBase<WATCHLIST>
        {
            #region 更新対象データ
            /// <summary>
            /// 作業依頼一覧テーブル
            /// </summary>
            public RazorPagesLearning.Data.Models.REQUEST_LIST RequestList { get; set; }

            /// <summary>
            /// ウォッチリスト用
            /// </summary>
            public List<val_WACTHLIST> val_WACTHLISTs { get; set; }

            /// <summary>
            /// エラー情報格納用
            /// </summary>
            public class error_WacthList
            {
                /// <summary>
                /// エラーMWL在庫ID
                /// </summary>
                public long ERROR_MWL_STOCK_ID { get; set; }

                /// <summary>
                /// エラー倉庫管理番号
                /// </summary>
                public string ERROR_STORAGE_MANAGE_NUMBER { get; set; }

                /// <summary>
                /// エラーメッセージ
                /// </summary>
                public string ERROR_MESSAGE { get; set; }
            }

            public class val_WACTHLIST
            {
                /// <summary>
                /// ウォッチリストテーブル
                /// </summary>
                public RazorPagesLearning.Data.Models.WATCHLIST WacthList { get; set; }

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

            #region 在庫詳細ポップアップ情報

            /// <summary>
            /// 顧客専用項目表示有無
            /// </summary>
            public bool popStockDetailCutsomerOnlyFlag { get; set; }

            /// <summary>
            /// 著作権セレクトボックス一覧
            /// </summary>
            public List<SelectListItem> popStockDetailSelectCopyRightList { get; set; }

            /// <summary>
            /// 契約書セレクトボックス一覧
            /// </summary>
            public List<SelectListItem> popStockDetailSelectContractList { get; set; }

            /// <summary>
            /// 処理判定セレクトボックス一覧
            /// </summary>
            public List<SelectListItem> popStockDetailSelectProcessJudgeList { get; set; }

            #endregion

            public ViewModel() :
                base(ViewTableType.Watchlist)
            {
                this.val_WACTHLISTs = new List<val_WACTHLIST>();
            }
        }

        /// <summary>
        /// 画面上の表示項目と同期する情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        #region ページネーション
        /// <summary>
        /// ページネーション情報
        /// </summary>
        [BindProperty]
        public PaginationInfo paginationInfo { get; set; }

        /// <summary>
        /// テーブル上の選択状態を管理する
        /// </summary>
        private WacthListCheckStatusManagement checkStatusManagement;

        /// <summary>
        /// テーブル・ページネーションの情報を含む
        /// </summary>
        public CheckStatusManagement<WATCHLIST>.StateRestoreConfig stateRestoreConfig = new CheckStatusManagement<WATCHLIST>.StateRestoreConfig();

        /// <summary>
        /// チェックボックスの内容初期化
        /// </summary>
        private async Task deleteCheckInfoAll()
        {
            // サービスのインスタンス作成
            this.wkTableSelectionSettingService = new Service.DB.WkTableSelectionSettingService(this._db, User, this._signInManager, this._userManager);
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            this.wkTableSelectionSettingService.deleteCheckInfoAll(user.ID, ViewTableType.Watchlist);
        }

        /// <summary>
        /// 現在のチェックボックスの状態を取得
        /// </summary>
        /// <returns></returns>
        private async Task<List<WK_TABLE_SELECTION_SETTING>> getCheckInfo()
        {
            // サービスのインスタンス作成
            this.wkTableSelectionSettingService = new Service.DB.WkTableSelectionSettingService(this._db, User, this._signInManager, this._userManager);
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            // memo：チェックボックスの状態を先に反映させる必要があるので、formStateSave をまずは行う。
            await this.formStateSave();
            return this.wkTableSelectionSettingService.getCheckStateList(user.ID, ViewTableType.Watchlist);
        }

        /// <summary>
        /// 表示件数変更、ページネーション、ソート変更の際に呼ばれるアクション
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            await this.paginationPreparation();

            await dispWacthList();

            return Page();
        }

        #endregion

        /// <summary>
        /// 画面遷移処理
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync()
        {
            await getStockDetailInfo();

            await this.deleteCheckInfoAll();

            await this.paginationPreparation();

            await dispWacthList();

            return Page();
        }

        /// <summary>
        /// ウォッチリストのデータを表示する。
        /// </summary>
        /// <returns></returns>
        private async Task dispWacthList()
        {
            await this.formStateSave();

            await updateViewData();
        }


        /// <summary>
        /// 削除
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDelete()
        {
            await getStockDetailInfo();

            // サービスのインスタンス作成
            this.WatchlistService = new Service.DB.WatchlistService(this._db, User, this._signInManager, this._userManager);
            this.StockService = new Service.DB.StockService(this._db, User, this._signInManager, this._userManager);
            this.RequestListService = new Service.DB.RequestListService(this._db, User, this._signInManager, this._userManager);
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            var wacthlist = new WATCHLIST();
            var result = new List<WATCHLIST>();

            // チェックされたウォッチリストを取得
            var wacthCheckResult = await this.getCheckInfo();

            if (wacthCheckResult.Count == 0)
            {
                //依頼が選択されていない。
            }
            else
            {

                using (var transaction = _db.Database.BeginTransaction())
                {
                    foreach (var wacthResult in wacthCheckResult)
                    {

                        // 反映準備
                        await WatchlistService.delete(wacthResult.originalDataId);

                        // 反映
                        await WatchlistService.db.SaveChangesAsync();

                    }
                    transaction.Commit();
                }
            }

            // チェックボックスを初期化
            await this.deleteCheckInfoAll();

            // ページネーション情報
            await this.paginationPreparation();

            // 画面表示用
            await dispWacthList();

            return Page();
        }

        /// <summary>
        /// 作業依頼追加
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostRequest()
        {
            await getStockDetailInfo();

            // サービスのインスタンス作成
            this.WkStockService = new Service.DB.WkStockService(this._db, User, this._signInManager, this._userManager);
            this.StockService = new Service.DB.StockService(this._db, User, this._signInManager, this._userManager);
            this.RequestListService = new Service.DB.RequestListService(this._db, User, this._signInManager, this._userManager);
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            // 作業依頼リスト用
            var req = new REQUEST_LIST();
            var ReqList = new List<REQUEST_LIST>();

            // 重複エラー確認用
            var requestErrorList = new List<error_WacthList>();
            int errorCount = 0;

            // チェックされたウォッチリストを取得
            var wacthCheckResult = await this.getCheckInfo();

            if (wacthCheckResult.Count == 0)
            {
                //依頼が選択されていない。
            }
            else
            {

                using (var transaction = _db.Database.BeginTransaction())
                {
                    foreach (var wacthResult in wacthCheckResult)
                    {

                        req.STOCK_ID = wacthResult.originalDataId;
                        req.USER_ACCOUNT_ID = wacthResult.USER_ACCOUNT_ID;

                        // すでに存在する作業依頼か確認
                        var checkDuplication = await this.RequestListService.Read(req.USER_ACCOUNT_ID, req.STOCK_ID);
                        if (checkDuplication.result == null)
                        {
                            ReqList.Add(req);

                            // 反映準備
                            await RequestListService.add(ReqList);

                            // 反映
                            await RequestListService.db.SaveChangesAsync();
                        }
                        else
                        {
                            // 重複しているので追加しない。
                            //作業依頼追加の際に、既に追加依頼されているものはエラーとなる。
                            //作業依頼追加できなかったものに関しては、エラー理由ファイルをCSV形式でダウンロードする。
                            // CSV出力の準備
                            var requestError = new error_WacthList();

                            requestError.ERROR_MWL_STOCK_ID = req.STOCK_ID;
                            requestError.ERROR_MESSAGE = "選択した依頼はすでに作業依頼に存在しているため、追加できませんでした。";

                            requestErrorList.Add(requestError);
                            errorCount++;
                        }
                    }
                    // エラーがあった場合
                    if (errorCount > 0)
                    {
                        ////エラーメッセージをbodyに吐き出す
                        //RazorPagesLearning.Utility.ReportHelper.HelperFunctions.writeErrorCsvFile
                        //    (new Utility.ReportHelper.HelperFunctions.ErrorCsvConfig
                        //    {
                        //        fileName = "error.csv",
                        //        target = this.Response,
                        //        writerOperation = new Action<System.IO.StreamWriter>((csv) =>
                        //        {
                        //            // header
                        //            csv.WriteLine("倉庫管理番号,エラー内容");

                        //            // body
                        //            for (int i = 0; i < requestErrorList.Count(); i++)
                        //            {
                        //                csv.WriteLine("{0},{1}", requestErrorList[i].ERROR_MWL_STOCK_ID, requestErrorList[i].ERROR_MESSAGE);
                        //                csv.Flush();
                        //            }
                        //            csv.Flush();

                        //        })
                        //    });
                    }
                    transaction.Commit();
                }
            }

            // チェックボックスを初期化
            await this.deleteCheckInfoAll();

            // ページネーション情報
            await this.paginationPreparation();

            // 画面表示用
            await dispWacthList();

            return Page();
        }

        /// <summary>
        /// 印刷(PDF)
        /// </summary>
        /// <returns></returns>
        public async Task OnPostPrint(string page)
        {
            await getStockDetailInfo();

            // サービスのインスタンス作成
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            this.WatchlistService = new Service.DB.WatchlistService(this._db, User, this._signInManager, this._userManager);
            this.RequestHistoryDetailService = new Service.DB.RequestHistoryDetailService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            // 読み取り情報
            var readConfig = new Service.DB.WatchlistService.ReadConfig
            {
                start = this.paginationInfo.startViewItemIndex,
                take = int.Parse(this.paginationInfo.displayNumber),
                sortOrder = this.viewModel.sortColumn,
                sortDirection = this.viewModel.sortDirection
            };

            ////レポートをエクスポートする
            //{
            //    var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
            //        DateTimeOffset.Now, "yyyyMMddHHmmss");

            //    Utility.ReportHelper.HelperFunctions.writePdf
            //        (new Utility.ReportHelper.HelperFunctions.WriteConfig
            //        {
            //            fileName = $"WatchList_{nowStr}.pdf",
            //            report = new RazorPagesLearning.Report.WatchListReport(
            //                new ReportConfig
            //                {
            //                    targetInformation = WatchlistService.dispWatchList(readConfig, user.ID, true),
            //                    targetRequestHistoryDetail = RequestHistoryDetailService.readRequestHistoryDetail(user.ID).First()
            //                }),
            //            target = this.Response
            //        });
            //}
        }

        /// <summary>
        /// Excelダウンロード
        /// </summary>
        /// <returns></returns>
        public async Task OnPostExcel(string page)
        {
            await getStockDetailInfo();

            // サービスのインスタンス作成
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            this.WatchlistService = new Service.DB.WatchlistService(this._db, User, this._signInManager, this._userManager);
            this.RequestHistoryDetailService = new Service.DB.RequestHistoryDetailService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            // 読み取り情報
            var readConfig = new Service.DB.WatchlistService.ReadConfig
            {
                start = this.paginationInfo.startViewItemIndex,
                take = int.Parse(this.paginationInfo.displayNumber),
                sortOrder = this.viewModel.sortColumn,
                sortDirection = this.viewModel.sortDirection
            };

            ////レポートをエクスポートする
            //{
            //    var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
            //        DateTimeOffset.Now, "yyyyMMddHHmmss");

            //    Utility.ReportHelper.HelperFunctions.writeExcel
            //        (new Utility.ReportHelper.HelperFunctions.WriteConfig
            //        {
            //            fileName = $"WatchList_{nowStr}.xlsx",
            //            report = new RazorPagesLearning.Report.WatchListExcel(
            //                new ReportConfigExcel
            //                {
            //                    targetInformation = WatchlistService.dispWatchList(readConfig, user.ID, true),
            //                    targetRequestHistoryDetail = RequestHistoryDetailService.readRequestHistoryDetail(user.ID).First()
            //                }),
            //            target = this.Response
            //        });
            //}
        }

        /// <summary>
        /// CSVダウンロード
        /// </summary>
        /// <returns></returns>
        public async Task OnPostCsv(string page)
        {
            await getStockDetailInfo();

            // サービスのインスタンス作成
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            this.WatchlistService = new Service.DB.WatchlistService(this._db, User, this._signInManager, this._userManager);
            this.RequestHistoryDetailService = new Service.DB.RequestHistoryDetailService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            // 読み取り情報
            var readConfig = new Service.DB.WatchlistService.ReadConfig
            {
                start = this.paginationInfo.startViewItemIndex,
                take = int.Parse(this.paginationInfo.displayNumber),
                sortOrder = this.viewModel.sortColumn,
                sortDirection = this.viewModel.sortDirection
            };

            ////レポートをエクスポートする
            //{
            //    var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
            //        DateTimeOffset.Now, "yyyyMMddHHmmss");

            //    Utility.ReportHelper.HelperFunctions.writeCsv
            //        (new Utility.ReportHelper.HelperFunctions.WriteConfig
            //        {
            //            fileName = $"WatchList_{nowStr}.csv",
            //            report = new RazorPagesLearning.Report.WatchListExcel(
            //                new ReportConfigExcel
            //                {
            //                    targetInformation = WatchlistService.dispWatchList(readConfig, user.ID, true),
            //                    targetRequestHistoryDetail = RequestHistoryDetailService.readRequestHistoryDetail(user.ID).First()
            //                }),
            //            target = this.Response
            //        });
            //}
        }

        // ↓↓↓一覧のテーブルに必要なもの

        /// <summary>
        /// 画面上に表示するデータを取得
        /// ページネーションやチェックボックスの内容も考慮している
        /// </summary>
        private async Task<bool> updateViewData()
        {
            // チェックボックスのチェック状態を復元しつつテーブルを読み込む
            // memo：ここで取得したデータ内容は、SelectableTableViewModelBase(viewModelの継承元)のtableRowsに格納されている

            // サービスのインスタンス作成
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            this.WatchlistService = new Service.DB.WatchlistService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);
            // チェックボックス関連
            this.checkStatusManagement = new WacthListCheckStatusManagement(this._db, this._signInManager, this._userManager, this.paginationInfo, User);

            return await this.checkStatusManagement.stateRestore(
                new CheckStatusManagement<WATCHLIST>.StateRestoreConfig
                {
                    //対象となるユーザーID
                    userInfo = User,
                    //データが格納されたビューモデル
                    viewModel = this.viewModel,
                    //ページネーション管理情報
                    paginationInfo = this.paginationInfo,
                    //DBから表に表示する情報を読み取る関数
                    readFunc = new Func<IQueryable<WATCHLIST>>(() =>
                    {
                        //表示データを読み込む
                        return this.WatchlistService.dispWatchList(
                        new Service.DB.WatchlistService.ReadConfig
                        {
                            start = this.paginationInfo.startViewItemIndex,
                            take = int.Parse(this.paginationInfo.displayNumber),
                            sortOrder = this.viewModel.sortColumn,
                            sortDirection = this.viewModel.sortDirection
                        }, user.ID, false);
                    }),
                    //DB上の最大レコード件数
                    maxRecords = this.WatchlistService.getStatistics(user.ID).numbers
                });
        }

        /// <summary>
        /// フォーム状態を保存する
        /// </summary>
        private async Task<bool> formStateSave()
        {
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            this.WatchlistService = new Service.DB.WatchlistService(this._db, User, this._signInManager, this._userManager);

            // チェックボックス関連
            this.checkStatusManagement = new WacthListCheckStatusManagement(this._db, this._signInManager, this._userManager, this.paginationInfo, User);

            return await this.checkStatusManagement.stateSave(
                new CheckStatusManagement<WATCHLIST>.StatePersistenceConfig
                {
                    userInfo = User,
                    viewModel = this.viewModel,
                    paginationInfo = this.paginationInfo,

                });
        }

        /// <summary>
        /// ページネーション関連
        /// </summary>
        private async Task paginationPreparation()
        {
            // サービスのインスタンス作成
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            this.checkStatusManagement = new WacthListCheckStatusManagement(this._db, this._signInManager, this._userManager, this.paginationInfo, User);
            this.WatchlistService = new Service.DB.WatchlistService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            // 取得するデータの最大件数を先に取得する
            this.paginationInfo.maxItems = this.WatchlistService.getStatistics(user.ID).numbers;

            // 最大件数を取得した上で、ページ情報を設定する
            this.paginationInfo.movePage(paginationInfo.displayNextPage);
        }

        /// <summary>
        /// 在庫詳細照会ポップアップに必要な情報を設定
        /// </summary>
        /// <returns></returns>
        private async Task getStockDetailInfo()
        {
            // サービスのインスタンス作成
            UserService userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            ShipperAdminService shipperAdminService = new Service.DB.ShipperAdminService(this._db, User, this._signInManager, this._userManager);
            DomainService domainService = new Service.DB.DomainService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await userService.read(principalConfig);

            // 荷主マスタの情報を取得
            Service.DB.ShipperAdminService.ReadConfig readConfig = new Service.DB.ShipperAdminService.ReadConfig() { SHIPPER_CODE = user.CURRENT_SHIPPER_CODE };
            var shipperInfo = shipperAdminService.read(readConfig);

            // 顧客専用項目
            this.viewModel.popStockDetailCutsomerOnlyFlag = shipperInfo.result.CUSTOMER_ONLY_FLAG;

            // DOMAINから一覧を取得
            // 著作権
            List<DOMAIN> copyRightList = domainService.getValueList(DOMAIN.Kind.STOCK_COPYRIGHT).ToList();
            this.viewModel.popStockDetailSelectCopyRightList = copyRightList.Select(e => new SelectListItem() { Value = e.CODE, Text = e.VALUE }).ToList();

            // 契約書
            List<DOMAIN> contractList = domainService.getValueList(DOMAIN.Kind.STOCK_CONTRACT).ToList();
            this.viewModel.popStockDetailSelectContractList = contractList.Select(e => new SelectListItem() { Value = e.CODE, Text = e.VALUE }).ToList();

            // 処理判定
            List<DOMAIN> processJudgeList = domainService.getValueList(DOMAIN.Kind.STOCK_PROCESS_JUDGE).ToList();
            this.viewModel.popStockDetailSelectProcessJudgeList = processJudgeList.Select(e => new SelectListItem() { Value = e.CODE, Text = e.VALUE }).ToList();
        }
    }
}