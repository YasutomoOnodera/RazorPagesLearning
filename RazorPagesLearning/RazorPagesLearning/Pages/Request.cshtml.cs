﻿using System;
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
using RazorPagesLearning.Utility.SelectableTable;
using RazorPagesLearning.Utility.Pagination;
using System.Security.Claims;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Service.Utility.ViewHelper;
//using static RazorPagesLearning.Report.HistoryReport;

namespace RazorPagesLearning.Pages
{
    [Authorize(Roles = "PasswordExpiration, Admin, ShipperBrowsing, ShipperEditing, Worker")]
    public class RequestModel : PageModel
    {
        // 定義
        //==============================================================================================================
        // TODO：ドメインから取得してきた依頼内容の値。どこかにまとめて定数で持つ？？
        /// <summary>
        /// 全在庫
        /// </summary>
        private const string requestContents1 = "99";   // memo：これはドメインでは定義されていない
        /// <summary>
        /// 新規入庫（登録ユーザー）
        /// </summary>
        private const string requestContents2 = "10";
        /// <summary>
        /// 新規入庫
        /// </summary>
        private const string requestContents3 = "20";
        /// <summary>
        /// 出荷
        /// </summary>
        private const string requestContents4 = "30";
        /// <summary>
        /// 再入庫
        /// </summary>
        private const string requestContents5 = "40";
        /// <summary>
        /// 廃棄
        /// </summary>
        private const string requestContents6 = "50";
        /// <summary>
        /// 抹消(永久出庫)
        /// </summary>
        private const string requestContents7 = "60";
        /// <summary>
        /// 抹消(データ抹消)
        /// </summary>
        private const string requestContents8 = "70";
        /// <summary>
        /// 資材販売
        /// </summary>
        private const string requestContents9 = "80";

        // ステータス
        // TODO：ドメインから取得してきた依頼内容の値。どこかにまとめて定数で持つ？？
        private const string status10 = "10";
        private const string status20 = "20";
        private const string status30 = "30";
        private const string status40 = "40";
        private const string status50 = "50";
        private const string status60 = "60";
        private const string status70 = "70";

        // class
        //==============================================================================================================
        #region class
        /// <summary>
        /// 画面上の在庫テーブルにおけるチェックボックスのチェック状態を管理する
        /// </summary>
        class RequestListCheckStatusManagement : CheckStatusManagement<REQUEST_LIST>
        {
            public override TableSelectionSettingService.TrackingIdentifier translationRelationIdentifier(REQUEST_LIST table)
            {
                return new Service.DB.TableSelectionSettingService.TrackingIdentifier
                {
                    // memo：チェックボックスの状態を取得してくるための主キーとなるもの
                    //       ユーザーアカウントIDは既に設定されているので、それ以外
                    dataId = table.STOCK_ID
                };
            }

            // コンストラクタ
            public RequestListCheckStatusManagement(RazorPagesLearning.Data.RazorPagesLearningContext db,
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


        /// <summary>
        /// 画面と連動する情報
        /// DataTablesを利用する場合は、SelectableTableViewModelBaseを継承する。<>には対象の型を指定。
        /// </summary>
        public class ViewModel : SelectableTableViewModelBase<REQUEST_LIST>
        {
            public ViewModel() :
                base(ViewTableType.Request)
            {
            }

            /// <summary>
            /// 依頼内容一覧
            /// </summary>
            public List<SelectListItem> requestContents { get; set; }

            /// <summary>
            /// 選択中の依頼内容
            /// </summary>
            public string selectRequest { get; set; }

            /// <summary>
            /// 作業依頼ワークテーブルのID
            /// </summary>
            public long? requestId { get; set; }

            /// <summary>
            /// ステータスの内容(画面表示に利用)
            /// </summary>
            public List<DOMAIN> domainStatusList { get; set; }

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
        }
        #endregion

        // 変数
        //==============================================================================================================
        /// <summary>
        /// 画面上の表示項目と同期する情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        #region テーブル・ページネーション情報

        /// <summary>
        /// ページネーション情報
        /// </summary>
        [BindProperty]
        public PaginationInfo paginationInfo { get; set; }

        /// <summary>
        /// テーブル上の選択状態を管理する
        /// </summary>
        private RequestListCheckStatusManagement checkStatusManagement;

        /// <summary>
        /// テーブル・ページネーションの情報を含む
        /// </summary>
        public CheckStatusManagement<REQUEST_LIST>.StateRestoreConfig stateRestoreConfig = new CheckStatusManagement<REQUEST_LIST>.StateRestoreConfig();
        #endregion

        /// <summary>
        /// 検索条件
        /// </summary>
        public RequestListService.SearchConfig searchConfig;

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
        private Service.DB.RequestListService requestListService;
        private Service.DB.WkRequestService wkRequestService;
        private Service.DB.WkRequestDetailService wkRequestDetailService;
        private Service.DB.WkTableSelectionSettingService wkTableSelectionSettingService;
        private RazorPagesLearning.Service.DB.RequestHistoryDetailService RequestHistoryDetailService;

        // コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        // 処理
        //==============================================================================================================
        // コンストラクタ
        public RequestModel(RazorPagesLearning.Data.RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager,
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
            this.paginationInfo = new PaginationInfo();
        }

        // アクション関連の処理
        //==============================================================================================================
        /// <summary>
        /// indexアクション
        /// 画面遷移時処理
        /// </summary>
        /// <param name="requestKind">作業依頼内容</param>
        /// <param name="wkRequestId">作業依頼ワークID</param>
        public async Task<IActionResult> OnGet(string requestKind, long? wkRequestId)
        {
            await this.init(requestKind);

            // 作業依頼ワークIDが無い場合は、フレームのメニューからの遷移とみなして、
            // 作業依頼ワークテーブルに入っている自分のデータを削除する
            if(wkRequestId == null)
            {
                // 削除対象のwkRequestIdを取得
                var targetWkRequest = this.wkRequestService.read(this.USER_ACCOUNT.ID);
                if (targetWkRequest.result != null)
                {
                    // 作業依頼ワークテーブルから、情報を削除する
                    this.wkRequestService.delete((long)targetWkRequest.result.ID);
                }
                // チェックボックスの情報も初期化する。
                this.deleteCheckInfoAll();
            }
            else
            {
                // 確保
                this.viewModel.requestId = wkRequestId;
            }

            if(requestKind != null)
            {
                // 集配先選択画面より戻った場合、一覧を再現
                this.viewModel.selectRequest = requestKind;
                await this.view();
            }

            return Page();
        }

        /// <summary>
        /// 画面表示アクション
        /// 基本的に、ここを経由して画面を表示する
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            // 初期処理
            await this.init(this.viewModel.selectRequest);

            // 依頼内容が選択されていない状態でPOSTされたら、何もしない
            if (this.viewModel.selectRequest == null)
            {
                return Page();
            }

            // 画面描画処理
            await this.view();

            return Page();
        }

        /// <summary>
        /// 依頼内容変更アクション
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostChgRequestAsync()
        {
            // 初期処理
            await this.init(this.viewModel.selectRequest);

            // 依頼内容変更時に、チェックボックスの内容を初期化する
            this.deleteCheckInfoAll();

            // 依頼内容が変わるので、入力情報を引き継がないように初期化する？
            this.viewModel.tableRows = new List<RowInfo<REQUEST_LIST>>();

            // 作業依頼内容が変更になったため、保持していた作業依頼情報は消す
            if (this.viewModel.requestId != null)
            {
                // 作業依頼ワークテーブルから、情報を削除する
                this.wkRequestService.delete((long)this.viewModel.requestId);

                this.viewModel.requestId = null;
            }

            // OnPostAsyncを実行して画面再描画
            return RedirectToPagePermanentPreserveMethod("Request");
        }

        /// <summary>
        /// printアクション
        /// 印刷ボタン押下イベント
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostPrintAsync()
        {
            // 初期処理
            await this.init(this.viewModel.selectRequest);

            // 読み取り情報
            var readConfig = new Service.DB.RequestListService.ReadConfig
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

            //    // 内容を全件取得
            //    var allGetOption = true;

            //    Utility.ReportHelper.HelperFunctions.writePdf
            //        (new Utility.ReportHelper.HelperFunctions.WriteConfig
            //        {
            //            fileName = $"Request_{nowStr}.pdf",
            //            report = new RazorPagesLearning.Report.RequestReport(
            //                new RazorPagesLearning.Report.RequestReport.ReportConfig
            //                {
            //                    targetInformation = requestListService.read(readConfig, this.searchConfig, allGetOption),
            //                    targetRequestHistoryDetail = RequestHistoryDetailService.readRequestHistoryDetail(this.USER_ACCOUNT.ID).First()
            //                }),
            //            target = this.Response
            //        });
            //}

            // OnPostAsyncを実行して画面再描画
            return RedirectToPagePermanentPreserveMethod("Request");
        }


        /// <summary>
        /// excelアクション
        /// Excelボタン押下イベント
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostExcelAsync()
        {
            // 初期処理
            await this.init(this.viewModel.selectRequest);

            // 読み取り情報
            var readConfig = new Service.DB.RequestListService.ReadConfig
            {
                start = this.paginationInfo.startViewItemIndex,
                take = int.Parse(this.paginationInfo.displayNumber),
                sortOrder = this.viewModel.sortColumn,
                sortDirection = this.viewModel.sortDirection
            };

            //レポートをエクスポートする
            //{
            //    var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
            //        DateTimeOffset.Now, "yyyyMMddHHmmss");
            //    // 内容を全件取得
            //    var allGetOption = true;

            //    Utility.ReportHelper.HelperFunctions.writeExcel
            //        (new Utility.ReportHelper.HelperFunctions.WriteConfig
            //        {
            //            fileName = $"Request_{nowStr}.xlsx",
            //            report = new RazorPagesLearning.Report.RequestReportExcel(
            //                new RazorPagesLearning.Report.RequestReportExcel.ReportConfigExcel
            //                {
            //                    targetInformation = requestListService.read(readConfig, this.searchConfig, allGetOption),
            //                    targetRequestHistoryDetail = RequestHistoryDetailService.readRequestHistoryDetail(this.USER_ACCOUNT.ID).First()
            //                }),
            //            target = this.Response
            //        });
            //}

            // OnPostAsyncを実行して画面再描画
            return RedirectToPagePermanentPreserveMethod("Request");
        }

        /// <summary>
        /// csvアクション
        /// CSVボタン押下イベント
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostCsvAsync()
        {
            // 初期処理
            await this.init(this.viewModel.selectRequest);

            // 読み取り情報
            var readConfig = new Service.DB.RequestListService.ReadConfig
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
            //    //内容を全件取得
            //    var allGetOption = true;

            //    Utility.ReportHelper.HelperFunctions.writeCsv
            //        (new Utility.ReportHelper.HelperFunctions.WriteConfig
            //        {
            //            fileName = $"Request_{nowStr}.csv",
            //            report = new RazorPagesLearning.Report.RequestReportExcel(
            //                new RazorPagesLearning.Report.RequestReportExcel.ReportConfigExcel
            //                {
            //                    targetInformation = requestListService.read(readConfig, this.searchConfig, allGetOption),
            //                    targetRequestHistoryDetail = RequestHistoryDetailService.readRequestHistoryDetail(this.USER_ACCOUNT.ID).First()
            //                }),
            //            target = this.Response
            //        });
            //}

            // OnPostAsyncを実行して画面再描画
            return RedirectToPagePermanentPreserveMethod("Request");
        }

        /// <summary>
        /// confirmアクション
        /// 確定ボタン押下イベント
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostConfirmAsync()
        {
            // 初期処理
            await this.init(this.viewModel.selectRequest);

            // チェックボックスが入った内容を取得する
            var checklist = await this.getCheckInfo();
            int detailCount = checklist.Count();
            if (detailCount <= 0)
            {
                // TODO：エラー内容を保持

                // OnPostAsyncを実行して画面再描画
                return RedirectToPagePermanentPreserveMethod("Request");
            }

            // 取得したデータで、依頼数が入っていない情報があった場合はエラー
            // 同時に依頼数の合計も取得する
            int requestCount = 0;
            foreach (var elm in checklist)
            {
                if(elm.appendInfo == null || elm.appendInfo == "")
                {
                    // err
                    // TODO：エラー内容を保持
                    // 依頼数に入力されていない
                }
                else
                {
                    int? tmpCount = HelperFunctions.ConvertStringToInt(elm.appendInfo);
                    if(tmpCount == null)
                    {
                        // err
                        // TODO：エラー内容を保持
                        // 数値じゃない
                    }
                    else
                    {
                        requestCount += (int)tmpCount;
                    }
                }
            }
            if(false) // TODO：エラー内容があるかどうかのチェックを入れる
            {
                //// 対象をチェックし終えた上で、エラー確認

                //// OnPostAsyncを実行して画面再描画
                //return RedirectToPagePermanentPreserveMethod("Request");
            }

            long requestId = 0;
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    if (this.viewModel.requestId != null)
                    {
                        requestId = (long)this.viewModel.requestId;

                        // 作業依頼ワークIDが存在している場合
                        // 作業依頼ワークテーブルの内容は更新、作業依頼詳細ワークテーブルは、全削除して、新しく追加

                        // 更新
                        var wkRequestData = this.wkRequestService.read(this.USER_ACCOUNT.ID, requestId);
                        wkRequestData.result.REQUEST_KIND = this.viewModel.selectRequest;
                        wkRequestData.result.REQUEST_COUNT_SUM = requestCount;
                        wkRequestData.result.DETAIL_COUNT = detailCount;

                        // 現在の情報を一度、全削除
                        this._db.WK_REQUEST_DETAILs.RemoveRange(wkRequestData.result.WK_REQUEST_DETAILs);

                        // 追加情報取得
                        List<WkRequestDetailService.AddConfig> wkRequestDetailAddConfigList = new List<WkRequestDetailService.AddConfig>();
                        foreach (var elm in checklist)
                        {
                            WkRequestDetailService.AddConfig wkRequestDetailAddConfig = new WkRequestDetailService.AddConfig();
                            wkRequestDetailAddConfig.mwlStockId = elm.originalDataId;
                            wkRequestDetailAddConfig.processingDate = DateTimeOffset.Now;
                            wkRequestDetailAddConfig.requestCount = (int)HelperFunctions.ConvertStringToInt(elm.appendInfo);

                            wkRequestDetailAddConfigList.Add(wkRequestDetailAddConfig);
                        }

                        // 新しく追加する
                        var addList = await this.wkRequestDetailService.getAddList(wkRequestDetailAddConfigList);
                        wkRequestData.result.WK_REQUEST_DETAILs.AddRange(addList);

                    }
                    else
                    {
                        // 作業依頼ワークIDが存在していない場合は、追加処理
                        // 作業依頼ワークテーブルと、作業依頼詳細ワークテーブル
                        WkRequestService.AddConfig wkRequestAddConfig = new WkRequestService.AddConfig();
                        wkRequestAddConfig.userAccountId = this.USER_ACCOUNT.ID;
                        wkRequestAddConfig.requestKind = this.viewModel.selectRequest;
                        wkRequestAddConfig.requestCountSum = requestCount;
                        wkRequestAddConfig.detailCount = detailCount;

                        wkRequestAddConfig.detailConfig = new List<WkRequestDetailService.AddConfig>();
                        foreach (var elm in checklist)
                        {
                            WkRequestDetailService.AddConfig wkRequestDetailAddConfig = new WkRequestDetailService.AddConfig();
                            wkRequestDetailAddConfig.mwlStockId = elm.originalDataId;
                            wkRequestDetailAddConfig.processingDate = DateTimeOffset.Now;
                            wkRequestDetailAddConfig.requestCount = (int)HelperFunctions.ConvertStringToInt(elm.appendInfo);

                            wkRequestAddConfig.detailConfig.Add(wkRequestDetailAddConfig);
                        }

                        // 追加した作業依頼ワークのIDを取得しておき、次の画面へ連携する
                        requestId = await this.wkRequestService.add(wkRequestAddConfig);
                    }

                    // DB更新
                    this._db.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // データ更新中エラー

                    transaction.Rollback();

                    // OnPostAsyncを実行して画面再描画
                    return RedirectToPagePermanentPreserveMethod("Request");
                }
            }

            // debug なので、自分へ返す
            // OnPostAsyncを実行して画面再描画
            //return RedirectToPagePermanentPreserveMethod("Request");

            // 集配先選択画面へ遷移する。パラメータは、requestKind と requestId
            return RedirectToPage("/RequestDelivery", new { requestKind = this.viewModel.selectRequest, wkRequestId = requestId });
        }

        /// <summary>
        /// deleteアクション
        /// 削除ボタン押下イベント
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            // 初期処理
            await this.init(this.viewModel.selectRequest);

            // チェックボックスが入った内容を取得する
            var checklist = await this.getCheckInfo();
            if (checklist.Count() <= 0)
            {
                // TODO：エラー内容を保持

                // OnPostAsyncを実行して画面再描画
                return RedirectToPagePermanentPreserveMethod("Request");
            }

            // 削除
            List<long> idList = checklist.Select(e => e.originalDataId).ToList();
            this.requestListService.delete(this.USER_ACCOUNT, idList, ViewTableType.Request);

            // OnPostAsyncを実行して画面再描画
            return RedirectToPagePermanentPreserveMethod("Request");
        }

        // 実装処理
        //==============================================================================================================
        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="requestKind">対象の作業依頼内容</param>
        /// <returns></returns>
        private async Task init(string requestKind)
        {
            await getStockDetailInfo();

            // インスタンス生成
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            this.domainService = new Service.DB.DomainService(this._db, User, this._signInManager, this._userManager);
            this.requestListService = new Service.DB.RequestListService(this._db, User, this._signInManager, this._userManager);
            this.wkRequestService = new Service.DB.WkRequestService(this._db, User, this._signInManager, this._userManager);
            this.wkRequestDetailService = new Service.DB.WkRequestDetailService(this._db, User, this._signInManager, this._userManager);
            this.wkTableSelectionSettingService = new Service.DB.WkTableSelectionSettingService(this._db, User, this._signInManager, this._userManager);
            this.RequestHistoryDetailService = new Service.DB.RequestHistoryDetailService(this._db, User, this._signInManager, this._userManager);

            // チェックボックス関連
            this.checkStatusManagement = new RequestListCheckStatusManagement(this._db, this._signInManager, this._userManager, 　this.paginationInfo, User);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);
            this.USER_ACCOUNT = user;

            // 検索条件の初期化
            this.searchConfig = new RequestListService.SearchConfig() { searchStatus1 = null, seatchStatus2 = null, kind = requestKind, userAccount = user };

            // 作業依頼内容を取得
            List<DOMAIN> requestList = this.domainService.getCodeList(DOMAIN.Kind.REQUEST_REQUEST).ToList();
            this.setDisplayRequestContentItems(requestList, requestKind);

            if(requestKind != null)
            {
                // 作業依頼の対象となるステータスの在庫情報を取得する
                getSearchStatus(requestKind);
            }

            // 画面での文言表示用
            this.viewModel.domainStatusList = this.domainService.getCodeList(DOMAIN.Kind.STCOK_STATUS).ToList();

        }

        /// <summary>
        /// 検索条件となるステータス情報の取得
        /// </summary>
        /// <param name="requestKind"></param>
        private void getSearchStatus(string requestKind)
        {
            string status1 = null;
            string status2 = null;

            switch (requestKind)
            {
                // 全在庫
                case RequestModel.requestContents1:
                    // 全ステータスの情報を取得する
                    status1 = null;
                    status2 = null;
                    break;

                // 新規入庫(登録ユーザー)
                // 新規入庫
                case RequestModel.requestContents2:
                case RequestModel.requestContents3:
                    // 単品：登録待ち
                    // 複数品：登録待ち
                    status1 = RequestModel.status10;
                    status2 = null;
                    break;

                // 出荷
                case RequestModel.requestContents4:
                    // 単品：在庫中
                    // 複数品：複数品
                    status1 = RequestModel.status20;
                    status2 = RequestModel.status70;
                    break;

                // 再入庫
                case RequestModel.requestContents5:
                    // 単品：出荷中
                    // 複数品：複数品
                    status1 = RequestModel.status30;
                    status2 = RequestModel.status70;
                    break;

                // 廃棄
                case RequestModel.requestContents6:
                    // 単品：在庫中
                    // 複数品：複数品
                    status1 = RequestModel.status20;
                    status2 = RequestModel.status70;
                    break;

                // 抹消(永久出庫)
                case RequestModel.requestContents7:
                    // 単品：在庫中
                    // 複数品：-
                    status1 = RequestModel.status20;
                    status2 = null;
                    break;

                // 抹消(データ抹消)
                case RequestModel.requestContents8:
                    // 単品：出荷中
                    // 複数品：-
                    status1 = RequestModel.status30;
                    status2 = null;
                    break;

            }
            this.searchConfig.searchStatus1 = status1;
            this.searchConfig.seatchStatus2 = status2;
        }

        /// <summary>
        /// 作業依頼内容項目を設定する
        /// </summary>
        /// <param name="contents">作業依頼内容の項目一覧</param>
        /// <param name="selected">選択中の項目値</param>
        public void setDisplayRequestContentItems(List<DOMAIN> contents, string selected)
        {
            this.viewModel.requestContents = new List<SelectListItem>();
            if (selected == null)
            {
                // 依頼内容が無い場合は、空白の選択肢を入れる
                this.viewModel.requestContents.Add(new SelectListItem() { Value = "", Text = "" });
            }
            // 全在庫表示を追加する
            this.viewModel.requestContents.Add(new SelectListItem() { Value = RequestModel.requestContents1, Text = "全在庫表示" });
            foreach (var content in contents.OrderBy(e => e.CODE))
            {
                // 資材販売は除く
                if (content.CODE != requestContents9)
                {
                    this.viewModel.requestContents.Add(new SelectListItem() { Value = content.CODE, Text = content.VALUE });
                }
            }

            this.viewModel.selectRequest = selected;
        }

        /// <summary>
        /// 画面表示処理
        /// </summary>
        private async Task view()
        {
            this.paginationPreparation();

            await this.formStateSave();

            // 一覧データ取得
            await this.updateViewData();
        }

        /// <summary>
        /// チェックボックスの内容初期化
        /// </summary>
        private void deleteCheckInfoAll()
        {
            this.wkTableSelectionSettingService.deleteCheckInfoAll(this.USER_ACCOUNT.ID, ViewTableType.Request);
            this._db.SaveChanges();
        }

        /// <summary>
        /// 現在のチェックボックスの状態を取得
        /// </summary>
        /// <returns></returns>
        private async Task<List<WK_TABLE_SELECTION_SETTING>> getCheckInfo()
        {
            // memo：チェックボックスの状態を先に反映させる必要があるので、formStateSave をまずは行う。
            await this.formStateSave();
            return this.wkTableSelectionSettingService.getCheckStateList(this.USER_ACCOUNT.ID, ViewTableType.Request);
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

            return await this.checkStatusManagement.stateRestore(
                new CheckStatusManagement<REQUEST_LIST>.StateRestoreConfig
                {
                    //対象となるユーザーID
                     userInfo = User,
                    //データが格納されたビューモデル
                    viewModel = this.viewModel,
                    //ページネーション管理情報
                    paginationInfo = this.paginationInfo,
                    //DBから表に表示する情報を読み取る関数
                    readFunc = new Func<IQueryable<REQUEST_LIST>>(() =>
                    {
                        bool allGetOption = false;
                        //表示データを読み込む
                        return this.requestListService.read(
                        new Service.DB.RequestListService.ReadConfig
                        {
                            start = this.paginationInfo.startViewItemIndex,
                            take = int.Parse(this.paginationInfo.displayNumber),
                            sortOrder = this.viewModel.sortColumn,
                            sortDirection = this.viewModel.sortDirection
                        }, this.searchConfig, allGetOption);
                    }),
                    //DB上の最大レコード件数
                    maxRecords = this.requestListService.getStatistics(this.searchConfig).numbers
                });
        }

        /// <summary>
        /// フォーム状態を保存する
        /// </summary>
        private async Task<bool> formStateSave()
        {
            return await this.checkStatusManagement.stateSave(
                new CheckStatusManagement<REQUEST_LIST>.StatePersistenceConfig
                {
                    userInfo = User,
                    viewModel = this.viewModel,
                    paginationInfo = this.paginationInfo,

                });
        }

        /// <summary>
        /// ページネーション関連
        /// </summary>
        private void paginationPreparation()
        {
            // 取得するデータの最大件数を先に取得する
            this.paginationInfo.maxItems = this.requestListService.getStatistics(this.searchConfig).numbers;

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
            Service.User.UserService userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
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