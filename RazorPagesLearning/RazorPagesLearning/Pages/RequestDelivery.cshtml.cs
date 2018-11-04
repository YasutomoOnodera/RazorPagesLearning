﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

// 追加
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorPagesLearning.Utility.SelectableTable;
using RazorPagesLearning.Utility.Pagination;
using System.Security.Claims;
using RazorPagesLearning.Service.DB;
//using RazorPagesLearning.Service.User;

namespace RazorPagesLearning.Pages
{
    public class RequestDeliveryModel : PageModel
    {
        // 定義
        //==============================================================================================================
        // TODO：ドメインから取得してきた依頼内容の値。どこかにまとめて定数で持つ？？
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
        /// <summary>
        /// 画面上の在庫テーブルにおけるチェックボックスのチェック状態を管理する
        /// </summary>
        class WkRequestCheckStatusManagement : CheckStatusManagement<WK_REQUEST>
        {
            public override TableSelectionSettingService.TrackingIdentifier translationRelationIdentifier(WK_REQUEST table)
            {
                return new Service.DB.TableSelectionSettingService.TrackingIdentifier
                {
                    // memo：チェックボックスの状態を取得してくるための主キーとなるもの
                    //       ユーザーアカウントIDは既に設定されているので、それ以外
                    dataId = table.ID
                };
            }

            // コンストラクタ
            public WkRequestCheckStatusManagement(RazorPagesLearning.Data.RazorPagesLearningContext db,
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
        /// 画面上に表示すべき情報
        /// </summary>
        public class ViewModel : SelectableTableViewModelBase<WK_REQUEST>
        {
            public ViewModel() :
                base(ViewTableType.WkRequest)
            {
            }

            /// <summary>
            /// 集配先ワーク情報
            /// </summary>
            public WK_REQUEST_DELIVERY wkDelivery { get; set; }

            /// <summary>
            /// ユーザー情報
            /// </summary>
            public RazorPagesLearning.Data.Models.USER_ACCOUNT USER_ACCOUNT { get; set; }

            /// <summary>
            /// 処理対象の依頼内容
            /// </summary>
            public string requestKind { get; set; }

            /// <summary>
            /// 処理対象の作業依頼ワークID
            /// </summary>
            public long wkRequestId { get; set; }

            /// <summary>
            /// 選択された集配先のID
            /// </summary>
            public long mwlDeliveryId { get; set; }

            /// <summary>
            /// 集配先選択ボタン表示有無
            /// </summary>
            public bool isDeliveryButton { get; set; }

            /// <summary>
            /// 遷移時のポップアップ表示有無
            /// </summary>
            public bool isDispPopup { get; set; }

            /// <summary>
            /// 「指定日」を設定する項目の表示文言
            /// </summary>
            public string specifiedDateString { get; set; }

            /// <summary>
            /// 「便」のセレクトボックス内容
            /// </summary>
            public List<SelectListItem> flightList { get; set; }

            /// <summary>
            /// 「便」のセレクトボックス内容
            /// </summary>
            public string selectFlight { get; set; }

            /// <summary>
            /// 「時間指定」のセレクトボックス内容
            /// </summary>
            public List<SelectListItem> targetTimeList { get; set; }

            /// <summary>
            /// 「時間指定」のセレクトボックス内容
            /// </summary>
            public string selectTargetTime { get; set; }

            /// <summary>
            /// ステータスの内容(画面表示に利用)
            /// </summary>
            public List<DOMAIN> domainStatusList { get; set; }

            /// <summary>
            /// 作業依頼内容の内容(画面表示に利用)
            /// </summary>
            public List<DOMAIN> domainRequestKindList { get; set; }
        }

        // 変数
        //==============================================================================================================
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

        #region テーブル・ページネーション情報

        /// <summary>
        /// ページネーション情報
        /// </summary>
        [BindProperty]
        public PaginationInfo paginationInfo { get; set; }

        /// <summary>
        /// テーブル上の選択状態を管理する
        /// </summary>
        private WkRequestCheckStatusManagement checkStatusManagement;

        /// <summary>
        /// テーブル・ページネーションの情報を含む
        /// </summary>
        public CheckStatusManagement<WK_REQUEST>.StateRestoreConfig stateRestoreConfig = new CheckStatusManagement<WK_REQUEST>.StateRestoreConfig();
        #endregion


        /// <summary>
        /// ユーザー情報
        /// </summary>
        public USER_ACCOUNT USER_ACCOUNT;

        // DB
        private Service.User.UserService userService;
        private Service.DB.WkRequestService wkRequestService;
        private Service.DB.CalendarAdminService calendarAdminService;
        private Service.DB.WkDeliveryService wkDeliveryService;
        private Service.DB.DomainService domainService;

        //コンストラクタ引数
        private readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        // 処理
        //==============================================================================================================
        // コンストラクタ
        public RequestDeliveryModel(RazorPagesLearning.Data.RazorPagesLearningContext db,
            SignInManager<IdentityUser> signInManager,
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

            //ポップアップ表示
            this.selectDeriveryModelViewModel = new PopUp.SelectDeriveryModel.ViewModel();
            this.selectDeriveryModel = new PopUp.SelectDeriveryModel(
                this._signInManager,
                this._logger,
                db);
        }

        // アクション関連の処理
        //==============================================================================================================
        /// <summary>
        /// indexアクション
        /// 画面遷移時処理
        /// </summary>
        /// <param name="requestKind">作業依頼内容</param>
        /// <param name="wkRequestId">作業依頼ワークID</param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync(string requestKind, long wkRequestId)
        {
            // 情報保持
            this.viewModel.requestKind = requestKind;
            this.viewModel.wkRequestId = wkRequestId;

            await this.init();

            // 初期遷移時の集配先設定
            await this.settingDispData();

            await this.view();

            return Page();
        }

        /// <summary>
        /// ページネーション時のイベント等の受け口
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            await this.init();

            await view();

            return Page();
        }

        /// <summary>
        /// 集配先選択ボタン
        /// ポップアップで集配先を選択された後のイベント
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeliveryAsync()
        {
            await this.init();

            var deliveryAdminId = this.viewModel.mwlDeliveryId;

            // 便の指定日・指定時間を取得する






            // 取得してきた内容を保存する
            await this.wkDeliveryService.addFromDeliveryAdmin(deliveryAdminId, this.viewModel.wkRequestId);

            this.selectDeriveryModelViewModel.isShowModal = false;

            return RedirectToPagePermanentPreserveMethod("RequestDelivery");
        }

        /// <summary>
        /// 確定ボタン
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostConfirmAsync()
        {
            await this.init();

            // 確定処理

            // 入力情報のバリデーション
            // TODO

            var checkFlight = REQUEST_HELPER.functions.checkFlight(this.viewModel.wkDelivery.SPECIFIED_DATE, this.viewModel.selectTargetTime, this.viewModel.selectFlight, this.calendarAdminService);
            if (checkFlight.Item1 == false)
            {
                // TODO：エラー情報設定

                // OnPostAsyncを実行して画面再描画
                return RedirectToPagePermanentPreserveMethod("RequestDelivery");
            }

            var checkStatus = REQUEST_HELPER.functions.checkStatus(this.viewModel.wkRequestId, this.USER_ACCOUNT.ID, this.viewModel.requestKind, this.wkRequestService);
            if (checkStatus.hasError == true)
            {
                // TODO：エラー情報設定

                return RedirectToPagePermanentPreserveMethod("RequestDelivery");
            }

            // 最終データ保存
            this.viewModel.wkDelivery.FLIGHT = this.viewModel.selectFlight;
            this.viewModel.wkDelivery.SPECIFIED_TIME = this.viewModel.selectTargetTime;
            await this.wkDeliveryService.add(this.viewModel.wkDelivery, this.viewModel.wkRequestId);

            // 確認画面へ遷移する。パラメータは、requestKind と requestId
            return RedirectToPage("/RequestConfirm", new { this.viewModel.requestKind, this.viewModel.wkRequestId });
        }

        /// <summary>
        /// 戻るボタン
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostBack()
        {
            // 依頼内容によって、戻る画面を変える
            if (this.viewModel.requestKind == requestContents9)
            {
                // 資材販売画面へ遷移する
                return RedirectToPage("/Material", new { requestKind = this.viewModel.requestKind, wkRequestId = this.viewModel.wkRequestId });
            }
            else
            {
                // 作業依頼画面へ遷移する
                return RedirectToPage("/Request", new { requestKind = this.viewModel.requestKind, wkRequestId = this.viewModel.wkRequestId });

            }
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
            this.wkRequestService = new Service.DB.WkRequestService(this._db, User, this._signInManager, this._userManager);
            this.calendarAdminService = new Service.DB.CalendarAdminService(this._db, User, this._signInManager, this._userManager);
            this.wkDeliveryService = new Service.DB.WkDeliveryService(this._db, User, this._signInManager, this._userManager);
            this.domainService = new Service.DB.DomainService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);
            this.USER_ACCOUNT = user;

            // チェックボックス関連(一覧を出すのに使用)
            this.checkStatusManagement = new WkRequestCheckStatusManagement(this._db, this._signInManager, this._userManager, this.paginationInfo, User);

            // 「集配先選択」ボタン表示判定
            if (this.viewModel.requestKind == requestContents7 || this.viewModel.requestKind == requestContents8)
            {
                // 廃棄と抹消(データ抹消)のときは、集配先はRazorPagesLearning固定となるので、ボタンは表示させない
                this.viewModel.isDeliveryButton = false;
            }
            this.viewModel.isDeliveryButton = true;


            // 「指定日」を設定する項目の表示文言を設定
            string specifiedDateString = "";
            switch (this.viewModel.requestKind)
            {
                case requestContents2:
                case requestContents3:
                case requestContents5:
                    // 新規入庫（登録ユーザー）
                    // 新規入庫
                    // 再入庫
                    specifiedDateString = "引取日";
                    break;
                case requestContents4:
                case requestContents7:
                case requestContents9:
                    // 出荷
                    // 抹消(永久出庫)
                    // 資材販売
                    specifiedDateString = "出荷日";
                    break;
                case requestContents6:
                case requestContents8:
                    // 廃棄
                    // 抹消(データ抹消)
                    specifiedDateString = "処理日";
                    break;
                default:

                    break;
            }
            this.viewModel.specifiedDateString = specifiedDateString;

            // 便情報
            List<DOMAIN> flightList = this.domainService.getCodeList(DOMAIN.Kind.DELIVERY_FLIGHT).ToList();
            this.viewModel.flightList = flightList.Select(e => new SelectListItem() { Value = e.CODE, Text = e.VALUE }).ToList();

            // 時間指定
            List<DOMAIN> targetTimeList = this.domainService.getCodeList(DOMAIN.Kind.DELIVERY_DATETIME).ToList();
            this.viewModel.targetTimeList = targetTimeList.Select(e => new SelectListItem() { Value = e.CODE, Text = e.VALUE }).ToList();

            // 画面での文言表示用
            this.viewModel.domainStatusList = this.domainService.getCodeList(DOMAIN.Kind.STCOK_STATUS).ToList();
            this.viewModel.domainRequestKindList = this.domainService.getCodeList(DOMAIN.Kind.REQUEST_REQUEST).ToList();

            // ポップアップは出さない
            this.selectDeriveryModelViewModel.isShowModal = false;
        }

        /// <summary>
        /// 画面表示処理
        /// </summary>
        private async Task view()
        {
            this.paginationPreparation();

            // 集配先情報を取得する
            Service.DB.WkDeliveryService.ReadConfig deliveryReadConfig = new WkDeliveryService.ReadConfig();
            deliveryReadConfig.WK_REQUEST_ID = this.viewModel.wkRequestId;
            this.viewModel.wkDelivery = this.wkDeliveryService.read(deliveryReadConfig).result;

            if (this.viewModel.wkDelivery == null)
            {
                // 初期化
                this.viewModel.wkDelivery = new WK_REQUEST_DELIVERY();
                this.viewModel.wkDelivery.DELIVERY_ADMIN = new DELIVERY_ADMIN();
            }
            else
            {
                // 便の選択情報
                this.viewModel.selectFlight = this.viewModel.wkDelivery.FLIGHT;
                this.viewModel.selectTargetTime = this.viewModel.wkDelivery.SPECIFIED_TIME;
            }

            // 一覧データ取得
            await this.updateViewData();
        }

        /// <summary>
        /// 初回遷移時の集配先情報を設定
        /// </summary>
        /// <returns></returns>
        private async Task settingDispData()
        {
            this.selectDeriveryModelViewModel.isShowModal = false;

            // WK_REQUEST_DELIVERY テーブルにデータがあるかチェック
            var wkRequest = this.wkRequestService.read(this.USER_ACCOUNT.ID, this.viewModel.wkRequestId);
            if(wkRequest.result.WK_REQUEST_DELIVERY == null)
            {
                // 初期化
                this.viewModel.wkDelivery = new WK_REQUEST_DELIVERY();
                this.viewModel.wkDelivery.DELIVERY_ADMIN = new DELIVERY_ADMIN();
            }

            if(wkRequest.result.WK_REQUEST_DELIVERY != null)
            {
                // DBにある値が最優先なので、何も設定しない
            }
            else if (this.viewModel.requestKind == requestContents7 || this.viewModel.requestKind == requestContents8)
            {
                // 依頼内容で集配先が決定する
                // TODO:集配先マスタデータのRazorPagesLearningを集配先ワークへ入れる
                await this.wkDeliveryService.addFromDeliveryAdmin(1/*RazorPagesLearningのIDを入れる*/, this.viewModel.wkRequestId);

            }
            else if(this.USER_ACCOUNT.DEFAULT_DELIVERY_ADMIN_ID != null)
            {
                // デフォルト集配先が設定されている
                // 集配先マスタデータを集配先ワークへ入れる
                await this.wkDeliveryService.addFromDeliveryAdmin(this.USER_ACCOUNT.DEFAULT_DELIVERY_ADMIN_ID, this.viewModel.wkRequestId);
            }
            else
            {
                // 集配先ポップアップを表示する
                this.selectDeriveryModelViewModel.isShowModal = true;
            }
        }


        // ↓↓↓一覧のテーブルに必要なもの
        /// <summary>
        /// 画面上に表示するデータを取得
        /// ページネーションやチェックボックスの内容も考慮している
        /// </summary>
        private async Task<bool> updateViewData()
        {
            // memo：ここで取得したデータ内容は、SelectableTableViewModelBase(viewModelの継承元)のtableRowsに格納されている

            return await this.checkStatusManagement.stateRestore(
                new CheckStatusManagement<WK_REQUEST>.StateRestoreConfig
                {
                    //対象となるユーザーID
                    userInfo = User,
                    //データが格納されたビューモデル
                    viewModel = this.viewModel,
                    //ページネーション管理情報
                    paginationInfo = this.paginationInfo,
                    //DBから表に表示する情報を読み取る関数
                    readFunc = new Func<IQueryable<WK_REQUEST>>(() =>
                    {
                        //表示データを読み込む
                        return this.wkRequestService.read(
                        new Service.DB.WkRequestService.ReadConfig
                        {
                            start = this.paginationInfo.startViewItemIndex,
                            take = int.Parse(this.paginationInfo.displayNumber),
                            sortOrder = this.viewModel.sortColumn,
                            sortDirection = this.viewModel.sortDirection
                        }, this.viewModel.wkRequestId, false);
                    }),
                    //DB上の最大レコード件数
                    maxRecords = this.wkRequestService.getStatistics(this.viewModel.wkRequestId).numbers
                });
        }

        /// <summary>
        /// ページネーション関連
        /// </summary>
        private void paginationPreparation()
        {
            // 取得するデータの最大件数を先に取得する
            this.paginationInfo.maxItems = this.wkRequestService.getStatistics(this.viewModel.wkRequestId).numbers;

            // 最大件数を取得した上で、ページ情報を設定する
            this.paginationInfo.movePage(paginationInfo.displayNextPage);
        }
    }

    namespace REQUEST_HELPER
    {
        // 共通関数
        public static class functions
        {
            // TODO：ドメインから取得してきた依頼内容の値。どこかにまとめて定数で持つ？？
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

            /// <summary>
            /// 便チェック
            /// 便の指定日時と指定時間が問題ないかチェックを行う
            /// </summary>
            /// <param name="targetDate">検査対象日時（WK_REQUEST_DELIVERY.SPECIFIED_DATE）</param>
            /// <param name="targetTime">検査対象時刻(午前、午後、指定なし)（WK_REQUEST_DELIVERY.SPECIFIED_TIME）</param>
            /// <param name="targetFlight">検査対象便（WK_REQUEST_DELIVERY.FLIGHT）</param>
            /// <param name="calendarAdminService">カレンダーマスターのサービス</param>
            /// <returns>結果とエラーメッセージを返す。trueのときはエラーメッセージはnull</returns>
            public static Tuple<bool, string> checkFlight(DateTimeOffset? targetDate, string targetTime, string targetFlight, Service.DB.CalendarAdminService calendarAdminService)
            {
                // TODO：エラーメッセージ確認
                var errString = "指定した日付・時間帯は対象外です。他の日付・時間帯を指定してください。";

                // 現在のサーバの情報を取得する
                DateTimeOffset dateNow = DateTimeOffset.Now;

                // カレンダーマスタから休日情報を取得する(条件：当日以降)
                var calenderData = calendarAdminService.ReadFrom(dateNow.Date);

                // １．当日以降の未来の日付が指定されているか
				if (targetDate.HasValue == false)
				{
					// TODO: 指定日が未指定の時にエラーにする
				}
                else if (targetDate <= dateNow.Date)
                {
                    // error
                    // 過去の日付が設定されている
                    return Tuple.Create<bool, string>(false, errString);
                }

                // ２．指定された日付は、RazorPagesLearning稼働日か
                var targetCalenderDate = calenderData.FirstOrDefault(e => e.HOLIDAY == dateNow.Date);
                if (targetCalenderDate != null)
                {
                    // カレンダーマスタにその日が存在していたら、error
                    return Tuple.Create<bool, string>(false, errString);
                }

                var ret = false;

                // 時刻取得
                TimeSpan nowTime = dateNow.TimeOfDay;
                // 比較用の時刻
                TimeSpan check1Start = new TimeSpan();
                TimeSpan check1End = new TimeSpan();
                TimeSpan check2Start = new TimeSpan();
                TimeSpan check2End = new TimeSpan();
                // 選択されている便によって、処理分ける
                switch (targetFlight)
                {
                    case "G":
                        // ルート便
                        // ・RazorPagesLearning便

                        // 現在のサーバ時刻の時間で処理を分ける
                        check1Start = TimeSpan.Parse("0:00");
                        check1End = TimeSpan.Parse("12:00");
                        check2Start = TimeSpan.Parse("12:01");
                        check2End = TimeSpan.Parse("18:00");

                        if (check1Start <= nowTime && nowTime <= check1End) // 0:00-12:00
                        {
                            // 当日の午後以降であればOK
                            // memo：休みの日は、事前にエラー検出しているので、ここでは当日午前でなければOKという認識
                            if (targetDate == dateNow.Date
                                && targetTime == "2")  // TODO:domain
                            {
                                // error
                                ret = false;
                            }
                            else
                            {
                                // true
                                ret = true;
                            }
                        }
                        else if (check2Start <= nowTime && nowTime <= check2End) // 12:01-18:00
                        {
                            // 翌稼働日の午前以降であればOK
                            // memo：休みの日は、事前にエラー検出しているので、ここでは当日でなければOKという認識
                            if (targetDate == dateNow)
                            {
                                // error
                                ret = false;
                            }
                            else
                            {
                                // true
                                ret = true;
                            }
                        }
                        else // 18:01-23:59
                        {
                            // 当日はNG
                            if (targetDate == dateNow)
                            {
                                // error
                                ret = false;
                            }
                            else
                            {
                                // 翌稼働日を見つける
                                int count = 0;
                                do
                                {
                                    // 1日ずつずらしていき、取得できない日が稼働日
                                    count++;
                                    targetCalenderDate = calenderData.OrderBy(e => e.HOLIDAY).FirstOrDefault(e => e.HOLIDAY == dateNow.AddDays(count).Date);
                                }
                                while (targetCalenderDate == null);

                                // 翌稼働日の午後以降であればOK
                                if (targetDate == dateNow.AddDays(count).Date
                                    && targetTime == "2")   // TODO:domain
                                {
                                    // error
                                    ret = false;
                                }
                                else
                                {
                                    // true
                                    // ここまできたら問題なし
                                    ret = true;
                                }
                            }
                        }
                        break;

                    case "D":
                    case "K":
                    case "S":
                    case "Y":
                        // 路線便
                        // ・ヤマト代引き
                        // ・近鉄物流
                        // ・佐川急便
                        // ・ヤマト運輸

                        check1Start = TimeSpan.Parse("0:00");
                        check1End = TimeSpan.Parse("16:00");

                        ret = checkFunc(dateNow, check1Start, check1End);
                        break;

                    case "A":
                    case "B":
                    case "C":
                    case "M":
                        // チャーター、バイク、引取
                        // ・依頼主様取引
                        // ・バイク便（TNL手配）
                        // ・バイク便（依頼主様手配）
                        // ・チャーター便（TNL手配）

                        check1Start = TimeSpan.Parse("0:00");
                        check1End = TimeSpan.Parse("18:00");

                        ret = checkFunc(dateNow, check1Start, check1End);
                        break;

                    default:
                        // error
                        // 未選択
                        break;
                }

                if (ret == true)
                {
                    errString = "";
                }

                return Tuple.Create<bool, string>(ret, errString);

                /// 【ローカル関数】
                /// 路線便、チャーター便、バイク便、
                /// now : 当日日時情報
                /// startTime : 範囲の開始時刻
                /// endTime : 範囲の終了時刻
                bool checkFunc(DateTimeOffset now, TimeSpan startTime, TimeSpan endTime)
                {
                    TimeSpan time = now.TimeOfDay;
                    if (startTime <= time && time <= endTime)
                    {
                        // 当日以降であればOK
                        return true;
                    }
                    else
                    {
                        // 翌稼働日であればOK
                        // memo：休みの日は、事前にエラー検出しているので、ここでは当日でなければOKという認識
                        if (targetDate == now)
                        {
                            // error
                            return false;
                        }
                        else
                        {
                            // true
                            return true;
                        }
                    }
                }
            }

            /// <summary>
            /// ステータスチェック実行結果
            /// </summary>
            public class CheckStatusResult
            {
                /// <summary>
                /// エラー検知
                /// </summary>
                public bool hasError;

                /// <summary>
                /// エラー情報一覧
                /// </summary>
                public IEnumerable< Tuple<string, WK_REQUEST_DETAIL> > errors;

                /// <summary>
                /// エラーメッセージ
                /// </summary>
                public string errorMessage;
            }

            #region 実装補助クラス

            /// <summary>
            /// 状態チェック補助クラス
            /// </summary>
            internal class CheckStatusHelper
            {
                /// <summary>
                /// 確認ステータス一覧
                /// </summary>
                private class TargetStatus
                {
                    public string status1;
                    public string status2;
                }

                /// <summary>
                /// チェック対象のステータスを決める
                /// </summary>
                /// <param name="requestKind"></param>
                /// <returns></returns>
                private TargetStatus decideTargetStatus(string requestKind)
                {
                    var ret = new TargetStatus();

                    // 対象ステータスを取得
                    //var status1 = "";
                    //var status2 = "";
                    switch (requestKind)
                    {
                        // 新規入庫(登録ユーザー)
                        // 新規入庫
                        case requestContents2:
                        case requestContents3:
                            // 単品：登録待ち
                            // 複数品：登録待ち
                            ret.status1 = status10;
                            ret.status2 = null;
                            break;

                        // 出荷
                        case requestContents4:
                            // 単品：在庫中
                            // 複数品：複数品
                            ret.status1 = status20;
                            ret.status2 = status70;
                            break;

                        // 再入庫
                        case requestContents5:
                            // 単品：出荷中
                            // 複数品：複数品
                            ret.status1 = status30;
                            ret.status2 = status70;
                            break;

                        // 廃棄
                        case requestContents6:
                            // 単品：在庫中
                            // 複数品：複数品
                            ret.status1 = status20;
                            ret.status2 = status70;
                            break;

                        // 抹消(永久出庫)
                        case requestContents7:
                            // 単品：在庫中
                            // 複数品：-
                            ret.status1 = status20;
                            ret.status2 = null;
                            break;

                        // 抹消(データ抹消)
                        case requestContents8:
                            // 単品：出荷中
                            // 複数品：-
                            ret.status1 = status30;
                            ret.status2 = null;
                            break;
                    }

                    return ret;
                }

                /// <summary>
                /// チェックを実行する
                /// </summary>
                public CheckStatusResult doCkeck(long wkRequestId, long userAccountId, 
                    string requestKind, Service.DB.WkRequestService wkRequestService)
                {
                    //チェック対象のリクエストを探す
                    var targetRequest = wkRequestService.read(userAccountId, wkRequestId);
                    if (false == targetRequest.succeed)
                    {
                        return new CheckStatusResult
                        {
                            hasError = true,
                            errorMessage = "作業依頼ワークテーブルの読み取りに失敗しました。"
                        };
                    }

                    //チェック対象のステータスを決める
                    var targetStatus = decideTargetStatus(requestKind);

                    //エラーを持つ要素一覧を作成する
                    var hasErrorElements = targetRequest.result.WK_REQUEST_DETAILs
                         .Where(elm =>
                         {
                             if (elm.STOCK.STATUS == targetStatus.status1)
                             {
                                 return false;
                             }

                             if (targetStatus.status2 != null && elm.STOCK.STATUS == targetStatus.status2)
                             {
                                 return false;
                             }

                             //ステータス条件に合致しない場合、エラーとする
                             return true;
                         });

                    //エラー一覧をエラー形式に変換する
                    if (0 == hasErrorElements.Count())
                    {
                        //エラーなし
                        return new CheckStatusResult {
                            hasError = false
                        };
                    }
                    else
                    {
                        //エラー検知
                        return new CheckStatusResult
                        {
                            hasError = true,
                            errors = hasErrorElements.Select(e=> 
                            Tuple.Create<string, WK_REQUEST_DETAIL>(
                                "",e
                            ))
                        };
                    }
                }
            }

            #endregion

            /// <summary>
            /// ステータスチェック
            /// 依頼内容の一覧を取得してきて、すべてが対応するステータスであるかどうかチェックを行う
            /// </summary>
            /// <param name="wkRequestId">作業依頼ワークID（WK_REQUEST.ID）</param>
            /// <param name="userAccountId">ユーザーアカウントID（USER_ACCOUNT.ID）</param>
            /// <param name="requestKind">作業依頼内容（WK_REQUEST.REQUEST_KIND）</param>
            /// <param name="wkRequestService">作業依頼ワークのサービス</param>
            /// <returns>結果とエラーメッセージを返す。trueのときはエラーメッセージはnull</returns>
            public static CheckStatusResult checkStatus(long wkRequestId, long userAccountId, string requestKind, Service.DB.WkRequestService wkRequestService)
            {
                //補助クラスで実行する
                return (new CheckStatusHelper())
                    .doCkeck( wkRequestId,  userAccountId,  requestKind,  wkRequestService);
            }
        }
    }

}