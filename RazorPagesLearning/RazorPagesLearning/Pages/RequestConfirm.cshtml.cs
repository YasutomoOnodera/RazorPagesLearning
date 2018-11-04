using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// 追加
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using RazorPagesLearning.Data.Models;
using System.Security.Claims;
using static RazorPagesLearning.Service.User.UserService;
using RazorPagesLearning.Utility.SortableTable;
using RazorPagesLearning.Utility.Pagination;
using AutoMapper;
using RazorPagesLearning.Utility.TempDataHelper;
//using RazorEngine;
using System.Net.Mail;
//using RazorEngine.Templating;
using static RazorPagesLearning.Data.Models.DOMAIN;
using RazorPagesLearning.Service.Pages.RequestConfirmModel;

namespace RazorPagesLearning.Pages
{
    /// <summary>
    /// 作業依頼確定画面
    /// </summary>
    [Authorize(Roles = "Admin ,ShipperBrowsing ,ShipperEditing ,Worker", Policy = "PasswordExpiration")]
    public class RequestConfirmModel : PageModel
    {

        #region 画面上に表示すべき情報
        /// <summary>
        /// 画面上に表示すべき情報
        /// </summary>
        public class ViewModel : SortableTableViewModelBase<WK_REQUEST_DETAIL>
        {
            /// <summary>
            /// 集配先ワークを追跡するための情報
            /// </summary>
            public class TrackingInformation
            {
                /// <summary>
                /// 依頼内容
                /// </summary>
                public string requestKind { get; set; }

                /// <summary>
                /// 集配先ワーク追跡ID
                /// </summary>
                public Int64 wkRequestId { get; set; }
            }

            /// <summary>
            /// 集配先ワークの表示情報
            /// </summary>
            public TrackingInformation trackingInformation { get; set; }

            /// <summary>
            /// 作業依頼処理が確定したか判定する
            /// </summary>
            public bool is_confirm { get; set; }

            /// <summary>
            /// ユーザアカウント
            /// </summary>
            public USER_ACCOUNT userAccount { get; set; }

            /// <summary>
            /// 集配先ワークモデル
            /// </summary>
            public Formatted_WK_DELIVERY wkDelivery { get; set; }

            /// <summary>
            /// 作業依頼履歴
            /// </summary>
            public REQUEST_HISTORY requestHistory { get; set; }

            /// <summary>
            /// 依頼内容
            /// </summary>
            public string requestContents { get; set; }

            /// <summary>
            /// エラーメッセージ
            /// </summary>
            public string errorMessage { get; set; }

            public ViewModel()
            {
                this.trackingInformation = new TrackingInformation();
            }

        }
        #endregion

        #region サービス関係

        /// <summary>
        /// サービスセットを生成する
        /// </summary>
        /// <returns></returns>
        protected RequestConfirmServiceSet makeServiceSet()
        {
            return new RequestConfirmServiceSet(this._db,
                User,
                this._signInManager,
                this._userManager);
        }

        #endregion

        /// <summary>
        /// DBアクセスオブジェクト
        /// </summary>
        public readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;

        // コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        // コンストラクタ
        public RequestConfirmModel(RazorPagesLearning.Data.RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            // memo：Userがこのタイミングではまだnullなので、
            //       DBのインスタンスはコンストラクタでは生成しないこと。

            this._db = db;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;

            //viewモデルの更新
            this.viewModel = new ViewModel();

            //ページネーション情報
            this.paginationInfo = new PaginationInfo();
        }

        /// <summary>
        /// 画面上の表示項目と同期する情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        /// <summary>
        /// ページネーション情報
        /// </summary>
        [BindProperty]
        public PaginationInfo paginationInfo { get; set; }


        #region エラーリダイレクト関係の処理

        /// <summary>
        /// ページ間で遷移する時に受け渡しするエラー関係のリダイレクト情報
        /// </summary>
        public class ErrorHandoverData : ViewModel.TrackingInformation
        {
            /// <summary>
            /// エラーメッセージ
            /// </summary>
            public string errorMessage { get; set; }
        }

        /// <summary>
        /// エラーリダイレクトの遷移時に引き渡す情報を設定する
        /// </summary>
        private void setErrorHandoverData()
        {
            var eInfo = new ErrorHandoverData
            {
                requestKind = this.viewModel.trackingInformation.requestKind,
                wkRequestId = this.viewModel.trackingInformation.wkRequestId,
                errorMessage = this.viewModel.errorMessage
            };
            TempData.Put("ErrorInfo", eInfo);
        }

        /// <summary>
        /// エラーリダイレクトの遷移時に引き渡す情報を読み取り現在のモデルに反映させる
        /// </summary>
        protected void readAndReflectErrorHandoverData()
        {
            //遷移元からエラーメッセージが届いていたら張り付ける
            var eInfo = TempData.Get<ErrorHandoverData>("ErrorInfo");
            if (null != eInfo)
            {
                this.viewModel.trackingInformation.wkRequestId = eInfo.wkRequestId;
                this.viewModel.trackingInformation.requestKind = eInfo.requestKind;
                this.viewModel.errorMessage = eInfo.errorMessage;
            }
        }

        /// <summary>
        /// エラー情報を保持してこのページへリダイレクトする
        /// </summary>
        /// <returns></returns>
        protected IActionResult errorRedirectToThisPage()
        {
            //TempDataに遷移関係の情報をセット
            setErrorHandoverData();
            return RedirectToPagePermanentPreserveMethod();
        }

        #endregion


        /// <summary>
        /// DBから表示に必要な情報を読み取る
        /// </summary>
        private async Task<bool> readData(RequestConfirmServiceSet serviceSet)
        {
            #region ローカル関数

            //作業依頼内容を設定する
            bool LF_setRequestContents()
            {
                const string RequestContentsKind = "00020001";
                var r = serviceSet.domainService
                    .getCodeList(RequestContentsKind)
                    .Where(e => e.CODE == this.viewModel.trackingInformation.requestKind)
                    .FirstOrDefault();
                if (null != r)
                {
                    //判定可能
                    this.viewModel.requestContents = r.VALUE;
                    return true;
                }
                else
                {
                    this.viewModel.requestContents = "-";
                    this.viewModel.errorMessage = "指定された依頼内容が不正です。";
                }

                return false;
            }

            #endregion

            // ログインユーザ情報を取得
            this.viewModel.userAccount = await serviceSet.userService.read(new ReadFromClaimsPrincipalConfig
            {
                userInfo = User
            });

            //作業依頼内容を変換する
            if (false == LF_setRequestContents())
            {
                return false;
            }

            //集配先ワーク情報を取得
            {
                var r = serviceSet.wkDeliveryService
                    .read(new Service.DB.WkDeliveryService.ReadConfig
                    {
                        WK_REQUEST_ID = this.viewModel.trackingInformation.wkRequestId
                    });
                if (false == r.succeed)
                {
                    this.viewModel.errorMessage = "指定された集配先ワークが読み取れません。";
                    return false;
                }
                var twkDelivery = r.result;
                if (null == twkDelivery)
                {
                    this.viewModel.errorMessage = "指定された集配先ワークが読み取れません。";
                    return false;
                }

                //読み取り結果を設定
                this.viewModel.wkDelivery = Formatted_WK_DELIVERY.convert(serviceSet, twkDelivery);
            }

            ///作業依頼詳細ワークを取得
            {
                var r = serviceSet.wkRequestDetailService.read(new Service.DB.WkRequestDetailService.ReadConfig
                {
                    WK_REQUEST_ID = this.viewModel.wkDelivery.WK_REQUEST_ID,
                    sortColumn = this.viewModel.sortColumn,
                    sortDirection = this.viewModel.sortDirection
                });
                if (false == r.succeed)
                {
                    this.viewModel.errorMessage = "指定された作業依頼ワークが読み取れません。";
                    return false;
                }

                //最大データ件数を指定
                this.paginationInfo.maxItems = r.result.Count();

                //該当データを取り込む
                this.viewModel.tableRows = r.result
                    .Skip(this.paginationInfo.startViewItemIndex)
                    .Take(int.Parse(this.paginationInfo.displayNumber))
                    .Select(e => new SortableTableRowInfo<WK_REQUEST_DETAIL>
                    {
                        data = e
                    })
                .ToList();
            }

            return true;
        }

        /// <summary>
        /// Get処理
        /// </summary>
        /// <param name="requestKind"></param>
        /// <param name="wkRequestId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync(string requestKind, long wkRequestId)
        {
            //サービス一覧を取得
            var serviceSet = makeServiceSet();

            //表示対象情報の追跡IDを保存
            {
                this.viewModel.trackingInformation.requestKind = requestKind;
                this.viewModel.trackingInformation.wkRequestId = wkRequestId;
            }

            // ViewModelへの設定
            this.viewModel.is_confirm = false;

            //画面表示データを読み取る
            await this.readData(serviceSet);

            return Page();
        }


        /// <summary>
        /// ページネーション遷移時
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            //サービス一覧を取得
            var serviceSet = makeServiceSet();

            //エラー情報が届いていたらエラー情報を反映させる
            readAndReflectErrorHandoverData();

            //画面表示データを読み取る
            await this.readData(serviceSet);

            // 最大件数を取得した上で、ページ情報を設定する
            this.paginationInfo.movePage(paginationInfo.displayNextPage);

            return Page();
        }

        /// <summary>
        /// 確認チェック結果
        /// </summary>
        private class ConfirmChecker
        {
            /// <summary>
            /// 実行結果
            /// </summary>
            public class Result
            {
                /// <summary>
                /// エラーを検知したか
                /// </summary>
                public bool hasError;

                /// <summary>
                /// エラー行
                /// </summary>
                public class ErrorRow
                {
                    /// <summary>
                    /// 作業依頼詳細情報
                    /// </summary>
                    public WK_REQUEST_DETAIL wk_request_detail;

                    /// <summary>
                    /// エラーメッセージ
                    /// </summary>
                    public string errorMessage;

                }

                /// <summary>
                /// エラー行情報
                /// </summary>
                public List<ErrorRow> errorRows;

                public Result()
                {
                    this.hasError = false;
                    this.errorRows = new List<ErrorRow>();
                }
            }

            /// <summary>
            /// 便情報のチェックを行う
            /// </summary>
            /// <param name="result"></param>
            private void doCheckFlight(Result result)
            {
                #region 便のチェックを行う

                var r = REQUEST_HELPER.functions.checkFlight
                    (this.viewModel.wkDelivery.SPECIFIED_DATE,
                    this.viewModel.wkDelivery.SPECIFIED_TIME,
                    this.viewModel.wkDelivery.FLIGHT,
                    this.serviceSet.calendarAdminService
                    );
                if (false == r.Item1)
                {
                    this.viewModel.errorMessage = r.Item2;
                    result.hasError = true;
                }

                #endregion
            }

            /// <summary>
            /// ステータスのチェックを行う
            /// </summary>
            /// <param name="result"></param>
            private void doCheckStatus(Result result)
            {
                #region ステータスをチェックする

                var r = REQUEST_HELPER.functions.checkStatus
                    (this.viewModel.wkDelivery.WK_REQUEST.ID,
                    this.viewModel.userAccount.ID,
                    this.viewModel.trackingInformation.requestKind,
                    this.serviceSet.wkRequestService
                    );
                if (true == r.hasError)
                {
                    this.viewModel.errorMessage = r.errorMessage;
                    result.hasError = true;
                    if (null != r.errors)
                    {
                        //ドメインからステータスを引く
                        var stsList = this.serviceSet.domainService.getValueList("00010004");

                        //詳細行の情報を持っていたら変換する
                        result.errorRows.AddRange(
                            r.errors.Select(e =>
                            {
                                //ステータスを変換する
                                var s = stsList.FirstOrDefault(in_e => e.Item2.STOCK.STATUS == in_e.CODE);
                                var strSts = null == s ? $"未定義({e.Item2.STOCK.STATUS})" : s.VALUE;
                                return new Result.ErrorRow
                                {
                                    wk_request_detail = e.Item2,
                                    errorMessage = $"在庫のステータス:{ strSts }は想定外のステータスです。"
                                };
                            }));
                    }
                }

                #endregion
            }

            /// <summary>
            /// 在庫数と依頼数をチェックする
            /// </summary>
            private void doCheckRequestedNumber(Result result)
            {
                //該当する在庫一覧を取得して

                var r = serviceSet.wkRequestDetailService.read(new Service.DB.WkRequestDetailService.ReadConfig
                {
                    WK_REQUEST_ID = this.viewModel.wkDelivery.WK_REQUEST_ID,
                    sortColumn = this.viewModel.sortColumn,
                    sortDirection = this.viewModel.sortDirection
                });
                if (false == r.succeed)
                {
                    this.viewModel.errorMessage = "指定された作業依頼ワークが読み取れません。";
                    result.hasError = true;
                    return;
                }

                //STOCK.STOCK_COUNT とWK_REQUEST_DETAIL.REQUEST_COUNTをチェックする。
                //両者の件数を比較して、WK_REQUEST_DETAIL.REQUEST_COUNTのほうが多くなっていたら、
                //エラー扱いとする。
                result.errorRows.AddRange(
                r.result
                    .Where(e => e.STOCK.STOCK_COUNT < e.REQUEST_COUNT)
                    .Select(e => new Result.ErrorRow
                    {
                        wk_request_detail = e,
                        errorMessage =
                        $"在庫数以上の依頼数が指定されています。在庫数は{e.STOCK.STOCK_COUNT}ですが、依頼数は{e.REQUEST_COUNT}を指定されています。"
                    }));
                if (0 != result.errorRows.Count)
                {
                    this.viewModel.errorMessage = "在庫数以上の依頼数が指定されています。　CSVファイルを確認してください。";
                }
            }

            /// <summary>
            /// チェックを行う
            /// </summary>
            /// <returns></returns>
            public Result doCheck()
            {
                var result = new Result();

                //便情報のチェックを行う
                doCheckFlight(result);

                //ステータスのチェックを行う
                doCheckStatus(result);

                //依頼数と在庫数をチェックする
                doCheckRequestedNumber(result);

                return result;
            }

            /// <summary>
            /// 表示対象モデル
            /// </summary>
            private ViewModel viewModel;

            /// <summary>
            /// サービス一覧
            /// </summary>
            private RequestConfirmServiceSet serviceSet;

            /// <summary>
            /// チェックを行う
            /// </summary>
            /// <param name="ref_serviceSet"></param>
            /// <param name="ref_viewModel"></param>
            public ConfirmChecker(RequestConfirmServiceSet ref_serviceSet, ViewModel ref_viewModel)
            {
                this.serviceSet = ref_serviceSet;
                this.viewModel = ref_viewModel;
            }
        }

        /// <summary>
        /// 作業確定処理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostConfirmAsync(int id)
        {
            #region ローカル関数

            //入力値をチェックする
            bool LF_checkTargetData(RequestConfirmServiceSet ref_serviceSet)
            {
                //エラーチェックする
                var r = new ConfirmChecker(ref_serviceSet, viewModel).doCheck();
                if (true == r.hasError)
                {
                    if (null != r.errorRows)
                    {
                        if (0 != r.errorRows.Count)
                        {
                            ////エラー詳細行があるので、
                            ////該当行をCSVで出力する
                            ////エラーメッセージをbodyに吐き出す
                            //RazorPagesLearning.Utility.ReportHelper.HelperFunctions.writeErrorCsvFile
                            //    (new Utility.ReportHelper.HelperFunctions.ErrorCsvConfig
                            //    {
                            //        fileName = "error.csv",
                            //        target = this.Response,
                            //        writerOperation = new Action<System.IO.StreamWriter>((csv) =>
                            //        {
                            //            // header
                            //            csv.WriteLine("倉庫管理番号,題名,エラー内容");

                            //            // body
                            //            foreach (var item in r.errorRows)
                            //            {
                            //                csv.WriteLine
                            //                ($"{item.wk_request_detail.STOCK.STORAGE_MANAGE_NUMBER},{item.wk_request_detail.STOCK.TITLE},{item.errorMessage}");
                            //            }
                            //            csv.Flush();
                            //        })
                            //    });

                            return false;
                        }
                    }
                }

                return true;
            }


            //DBに値を書き出す
            async Task<bool> LF_saveDB(RequestConfirmServiceSet ref_serviceSet)
            {
                // 作業依頼履歴を保存
                this.viewModel.requestHistory = await ref_serviceSet.requestHistoryService.save(
                    new Service.DB.RequestHistoryService.WriteConfig
                    {
                        wkDelivery = this.viewModel.wkDelivery,
                        userAccount = this.viewModel.userAccount,
                        REQUEST_KIND = this.viewModel.trackingInformation.requestKind
                    });

                //作業依頼履歴を確定してIDを決定する
                await _db.SaveChangesAsync();

                //確定処理で在庫情報を更新する
                {
                    #region 在庫情報を確定情報で更新する
                    (new RequestConfirmationProcessingHelper(ref_serviceSet))
                        .confirm(new RequestConfirmationProcessingHelper.ConfirmConfig
                        {
                            request_history = this.viewModel.requestHistory,
                            wk_request_delivery = this.viewModel.wkDelivery
                        });
                    #endregion
                }

                // 作業依頼履歴詳細を保存
                {
                    #region 作業依頼履歴詳細を保存

                    //作業依頼確定処理で決定された最新の値で表示するためにデータを取り直す
                    var r = ref_serviceSet.wkRequestDetailService
                        .read(new Service.DB.WkRequestDetailService.ReadConfig
                        {
                            WK_REQUEST_ID = this.viewModel.wkDelivery.WK_REQUEST_ID,
                            sortColumn = this.viewModel.sortColumn,
                            sortDirection = this.viewModel.sortDirection
                        });
                    if (false == r.succeed)
                    {
                        this.viewModel.errorMessage = "指定された作業依頼ワークが読み取れません。";
                        return false;
                    }

                    //DB上の値を更新する
                    await ref_serviceSet.requestHistoryDetailService
                         .save(new Service.DB.RequestHistoryDetailService.WriteConfig
                         {
                             requestHistory = this.viewModel.requestHistory,
                             //DBで取得した値で更新する
                             wkRequestHistoryDetail = r.result
                             .ToList()
                         });
                    #endregion
                }

                //DBに保存する
                await _db.SaveChangesAsync();

                return true;
            }

            //DB上の一時テーブルを消去する
            async Task<bool> LF_removeWorkDBTable(RequestConfirmServiceSet ref_serviceSet)
            {
                //作業依頼ワークからたどって削除する

                //作業依頼一覧を消去する
                this._db.REQUEST_LISTs.RemoveRange(
                                    viewModel.wkDelivery.WK_REQUEST.
                                    WK_REQUEST_DETAILs
                                    .Select(e => e.STOCK)
                                    .Select(e => this._db.REQUEST_LISTs.Where(in_e => in_e.STOCK_ID == e.ID &&
                                     in_e.USER_ACCOUNT_ID == e.CREATED_USER_ACCOUNT_ID))
                                    .SelectMany(e => e)
                    );

                //作業依頼詳細ワーク
                this._db.WK_REQUEST_DETAILs.RemoveRange(
                                    viewModel.wkDelivery.WK_REQUEST.
                                    WK_REQUEST_DETAILs
                    );

                //作業依頼ワークを消去
                this._db.WK_REQUESTs.Remove(
                                    viewModel.wkDelivery.WK_REQUEST
                    );

                //集配先ワーク
                this._db.WK_REQUEST_DELIVERies.RemoveRange(
                                    //view modelが持っているviewModel.wkDeliveryは、
                                    //一度データ変換を経ているため正しくDB上の情報を追跡できない。
                                    //wkDeliveryに関してはDBから再度引き直す。
                                    _db.WK_REQUEST_DELIVERies.Where(e => e.ID == this.viewModel.trackingInformation.wkRequestId)
                    );

                //DBに保存する
                await _db.SaveChangesAsync();

                return true;
            }

            #endregion

            try
            {
                // 保存処理
                using (var transaction = _db.Database.BeginTransaction())
                {
                    //サービス一覧を取得
                    var serviceSet = makeServiceSet();

                    //表示対象情報を読み込む
                    if (false == await this.readData(serviceSet))
                    {
                        //入力内容問題があった場合
                        return errorRedirectToThisPage();
                    }

                    //DB内容をチェックする
                    if (false == LF_checkTargetData(serviceSet))
                    {
                        //入力内容問題があった場合
                        return errorRedirectToThisPage();
                    }

                    //データを保存する
                    if (false == await LF_saveDB(serviceSet))
                    {
                        //入力内容問題があった場合
                        return errorRedirectToThisPage();
                    }

                    //メールを送信する
                    {
                        #region メールを送信する
                        var sender = new NotificationEmailSender(serviceSet, this.viewModel);
                        var r = sender.doSend();
                        if (false == r.Item1)
                        {
                            //送信処理に問題があった場合
                            this.viewModel.errorMessage = r.Item2;
                            return errorRedirectToThisPage();
                        }
                        #endregion
                    }

                    //ワークテーブルを消去する
                    if (false == await LF_removeWorkDBTable(serviceSet))
                    {
                        //入力内容問題があった場合
                        return errorRedirectToThisPage();
                    }

                    //登録完了状態に切り替える
                    this.viewModel.is_confirm = true;

                    //結果を反映させる
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                this.viewModel.errorMessage = e.Message;
                return errorRedirectToThisPage();
            }

            return Page();
        }

        #region フォーマット済みの定義

        /// <summary>
        /// フォーマット済みの作業依頼ワーク
        /// </summary>
        public class Formatted_WK_DELIVERY : WK_REQUEST_DELIVERY
        {
            /// <summary>
            /// サービスセット
            /// </summary>
            private RequestConfirmServiceSet serviceSet;

            /// <summary>
            /// AutoMapper使うとコンストラクタ使えないので
            /// 外しておく
            /// </summary>
            /// <param name="ref_serviceSet"></param>
            internal void init(RequestConfirmServiceSet ref_serviceSet)
            {
                this.serviceSet = ref_serviceSet;
            }

            /// <summary>
            /// フォーマット済みの指定時間
            /// </summary>
            /// <returns></returns>
            public string FormattedSPECIFIED_TIME()
            {
                //指定時間はDB上に 午前/午後で入っているので、
                //その値を変換して表示する
                const string RequestContentsKind = "00080002";
                var r = serviceSet.domainService
                    .getCodeList(RequestContentsKind)
                    .Where(e => e.CODE == this.SPECIFIED_TIME)
                    .FirstOrDefault();
                if (null != r)
                {
                    //判定可能
                    return r.VALUE;
                }
                else
                {
                    return "-";
                }
            }

            /// <summary>
            /// DBから取得される形式から表示可能な形式に変換する
            /// </summary>
            /// <param name=""></param>
            /// <returns></returns>
            internal static Formatted_WK_DELIVERY convert(RequestConfirmServiceSet ref_serviceSet, WK_REQUEST_DELIVERY org)
            {
                //データ形式を変換する
                var m = Mapper.Map<Formatted_WK_DELIVERY>(org);
                m.init(ref_serviceSet);
                return m;
            }
        }

        #endregion


        #region メール送信関係

        /// <summary>
        /// 確定状態通知メール送信者
        /// 
        /// [Mail送信処理の参考情報]
        /// 
        /// メールの送信
        /// https://smdn.jp/programming/netfx/tips/send_mail/
        /// 
        /// SmtpClientクラスを使ってメールを送信する
        /// https://dobon.net/vb/dotnet/internet/smtpclient.html
        /// 
        /// [メールテンプレートエンジンの参考]
        /// 
        /// RazorEngine
        /// https://antaris.github.io/RazorEngine/index.html
        /// 
        /// 
        /// 
        /// </summary>
        public class NotificationEmailSender
        {
            /// <summary>
            /// サービス情報
            /// </summary>
            public RequestConfirmServiceSet serviceSet;

            /// <summary>
            /// メール本文への埋め込み対象情報
            /// </summary>
            public ViewModel viewModel;

            public NotificationEmailSender(RequestConfirmServiceSet ref_serviceSet, ViewModel ref_viewModel)
            {
                this.serviceSet = ref_serviceSet;
                this.viewModel = ref_viewModel;
            }

            /// <summary>
            /// 本文を生成する
            /// </summary>
            /// <returns></returns>
            private MailMessage makeText()
            {
                var mailMessage = new MailMessage();

                {
                    #region メール件名を生成

                    mailMessage.Subject =
                        $"{this.viewModel.userAccount.COMPANY}様 {this.viewModel.requestContents}のご連絡";

                    #endregion
                }

                {
                    #region メール本文を生成
                    //テンプレート本文を読み込む
                    var template = this.serviceSet.mailTemplateService.read("1");
                    if (false == template.succeed)
                    {
                        throw new ApplicationException("作業依頼確定用のメールテンプレートが読み取れませんでした。");
                    }

                    ////テンプレートに対して埋め込みを行う
                    //mailMessage.Body = Razor.Parse(template.result.TEXT, this.viewModel);
                    //mailMessage.Body =
                    //    Engine.Razor.RunCompile(template.result.TEXT, "templateKey", null, this.viewModel);

                    #endregion
                }

                return mailMessage;
            }

            /// <summary>
            /// メールの送信先情報を設定する
            /// </summary>
            /// <param name="mailMessage"></param>
            private void setMailSubmitInformation(MailMessage ref_mailMessage, SYSTEM_SETTING systemSetting)
            {
                #region ローカル関数

                ///メールアドレス形式に乗っ取っているか判定する
                bool LF_isSendableMailAddress(string address)
                {
                    if (string.IsNullOrEmpty(address))
                    {
                        return false;
                    }

                    try
                    {
                        System.Net.Mail.MailAddress a =
                            new System.Net.Mail.MailAddress(address);
                    }
                    catch (FormatException)
                    {
                        //FormatExceptionがスローされた時は、正しくない
                        return false;
                    }

                    return true;
                }

                //送信可能なアドレスだったらリストに保存する
                void LF_setAddressIfSendable(List<string> destination, string address)
                {
                    if (true == LF_isSendableMailAddress(address))
                    {
                        destination.Add(address);
                    }
                }

                #endregion

                //データを送信する

                //Toアドレスの一覧
                var toList = new List<string>();

                //CCアドレスの一覧
                var ccList = new List<string>();

                //BCCアドレスの一覧
                var bccList = new List<string>();

                {
                    #region ユーザーアカウントから設定する

                    LF_setAddressIfSendable(toList, this.viewModel.userAccount.MAIL);

                    LF_setAddressIfSendable(ccList, this.viewModel.userAccount.MAIL1);
                    LF_setAddressIfSendable(ccList, this.viewModel.userAccount.MAIL2);
                    LF_setAddressIfSendable(ccList, this.viewModel.userAccount.MAIL3);
                    LF_setAddressIfSendable(ccList, this.viewModel.userAccount.MAIL4);
                    LF_setAddressIfSendable(ccList, this.viewModel.userAccount.MAIL5);
                    LF_setAddressIfSendable(ccList, this.viewModel.userAccount.MAIL6);
                    LF_setAddressIfSendable(ccList, this.viewModel.userAccount.MAIL7);
                    LF_setAddressIfSendable(ccList, this.viewModel.userAccount.MAIL8);
                    LF_setAddressIfSendable(ccList, this.viewModel.userAccount.MAIL9);
                    LF_setAddressIfSendable(ccList, this.viewModel.userAccount.MAIL10);

                    #endregion
                }

                {
                    #region 集配先マスタから設定する

                    LF_setAddressIfSendable(toList, this.viewModel.wkDelivery.DELIVERY_ADMIN.MAIL);

                    LF_setAddressIfSendable(ccList, this.viewModel.wkDelivery.DELIVERY_ADMIN.MAIL1);
                    LF_setAddressIfSendable(ccList, this.viewModel.wkDelivery.DELIVERY_ADMIN.MAIL2);
                    LF_setAddressIfSendable(ccList, this.viewModel.wkDelivery.DELIVERY_ADMIN.MAIL3);
                    LF_setAddressIfSendable(ccList, this.viewModel.wkDelivery.DELIVERY_ADMIN.MAIL4);
                    LF_setAddressIfSendable(ccList, this.viewModel.wkDelivery.DELIVERY_ADMIN.MAIL5);
                    LF_setAddressIfSendable(ccList, this.viewModel.wkDelivery.DELIVERY_ADMIN.MAIL6);
                    LF_setAddressIfSendable(ccList, this.viewModel.wkDelivery.DELIVERY_ADMIN.MAIL7);
                    LF_setAddressIfSendable(ccList, this.viewModel.wkDelivery.DELIVERY_ADMIN.MAIL8);
                    LF_setAddressIfSendable(ccList, this.viewModel.wkDelivery.DELIVERY_ADMIN.MAIL9);
                    LF_setAddressIfSendable(ccList, this.viewModel.wkDelivery.DELIVERY_ADMIN.MAIL10);

                    #endregion
                }

                {
                    #region 管理者情報を設定する

                    LF_setAddressIfSendable(bccList, systemSetting.ADMIN_MAIL);

                    #endregion

                }

                //メールアドレスの重複結果を排除して設定する
                {
                    #region 重複を除外して設定する
                    foreach (var ele in toList.Distinct())
                    {
                        ref_mailMessage.To.Add(ele);
                    }

                    foreach (var ele in ccList.Distinct())
                    {
                        ref_mailMessage.CC.Add(ele);
                    }

                    foreach (var ele in bccList.Distinct())
                    {
                        ref_mailMessage.Bcc.Add(ele);
                    }
                    #endregion
                }

                //送信元メールアドレスを設定する
                {
                    if (false == LF_isSendableMailAddress(systemSetting.ADMIN_MAIL))
                    {
                        throw new ApplicationException("メールの送信元アドレスが未設定です。");
                    }
                    ref_mailMessage.From = new MailAddress(systemSetting.ADMIN_MAIL);
                }

            }

            /// <summary>
            /// E-Mailを送信する
            /// </summary>
            private void sendEmail(MailMessage message)
            {
                //DBからメールサーバー情報を取得する
                var sys = this.serviceSet.systemSettingService.read().result.FirstOrDefault();
                if (null != sys)
                {
                    //送信先情報を設定
                    setMailSubmitInformation(message, sys);

                    //メール設定が存在する場合にメール送信する
                    using (var client = new SmtpClient(sys.MAIL_SERVER, sys.MAIL_PORT))
                    {
                        //メール送信
                        client.Send(message);
                    }
                }
                else
                {
                    //メール設定が存在しな場合、
                    //エラー扱いとせずそのまま処理を進める。
                    //エラー扱いにする場合、状況に応じて修正が必要。
                }
            }

            /// <summary>
            /// ロール送信する
            /// </summary>
            public Tuple<bool, string> doSend()
            {
                try
                {
                    //本文を作成する
                    var body = makeText();

                    //メール送信する
                    sendEmail(body);

                    //処理結果を返す
                    return Tuple.Create<bool, string>(true, null);
                }
                catch (Exception e)
                {
                    //処理結果を返す
                    return Tuple.Create<bool, string>(false, e.Message);
                }
            }

        }

        #endregion

    }
}