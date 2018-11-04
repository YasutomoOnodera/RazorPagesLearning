using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Pages.DeliveryHelpers;
using RazorPagesLearning.Pages.PopUp;
using RazorPagesLearning.Report;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Utility.SelectItem;
using RazorPagesLearning.Utility.SortableTable;
using RazorPagesLearning.Utility.TempDataHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace RazorPagesLearning.Pages
{
    /// <summary>
    /// 自社便画面で共通に使用出来る補助的な関数、オブジェクト群を定義
    /// </summary>
    namespace DeliveryHelpers
    {
        /// <summary>
        /// 自社便関係のページで共通に使用できる基底クラス
        /// </summary>
        public abstract class DeliveryModelBase : PageModel
        {
            /// <summary>
            /// コンストラクタで渡される引数
            /// </summary>
            internal RazorPagesLearning.Data.RazorPagesLearningContext _db;
            internal SignInManager<IdentityUser> _signInManager;
            internal UserManager<IdentityUser> _userManager;
            internal ILogger<LoginModel> _logger;

            /// <summary>
            /// 使用するサービス一覧
            /// </summary>
            public class ServiceSet
            {
                #region 内部サービス一覧
                /// <summary>
                /// 
                /// </summary>
                public readonly Service.User.UserService userService;

                public readonly Service.DB.TransportAdminService transportAdminService;

                public readonly Service.DB.DeliveryRequestService deliveryService;

                public readonly Service.DB.DeliveryDetailService deliveryDetailService;

                public readonly Service.DB.DomainService domainService;

                public readonly Service.DB.TruckAdminService truckAdminService;

                #endregion

                public ServiceSet(RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
                    ClaimsPrincipal ref_user,
                SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
                {
                    this.userService = new Service.User.UserService
                        (ref_db,
                        ref_user,
                        ref_signInManager,
                        ref_userManager);

                    this.transportAdminService = new Service.DB.TransportAdminService
                        (ref_db,
                        ref_user,
                        ref_signInManager,
                        ref_userManager);

                    this.deliveryService = new Service.DB.DeliveryRequestService
                        (ref_db,
                        ref_user,
                        ref_signInManager,
                        ref_userManager);

                    this.deliveryDetailService = new Service.DB.DeliveryDetailService
                        (ref_db,
                        ref_user,
                        ref_signInManager,
                        ref_userManager);

                    this.domainService = new Service.DB.DomainService
                        (ref_db,
                        ref_user,
                        ref_signInManager,
                        ref_userManager);

                    this.truckAdminService = new Service.DB.TruckAdminService
                        (ref_db,
                        ref_user,
                        ref_signInManager,
                        ref_userManager);
                }
            }

            /// <summary>
            /// サービスセットを生成する
            /// </summary>
            /// <returns></returns>
            protected ServiceSet makeServiceSet()
            {
                return new ServiceSet(this._db,
                    User,
                    this._signInManager,
                    this._userManager);
            }

            /// <summary>
            /// 車番号設定画面
            /// </summary>
            [BindProperty]
            public DeliveryCarNumberSetting deliveryCarNumberSetting { get; set; }

            public DeliveryModelBase()
            {
                deliveryCarNumberSetting = new DeliveryCarNumberSetting();
            }

            /// <summary>
            /// モーダル画面の表示準備を行う
            /// </summary>
            protected void preparationForModalShow(string TRANSPORT_ADMIN_CODE,
                string DELIVERY_REQUEST_NUMBER,
                string onDecisionHeaderName)
            {
                //モーダル用のデータ読み取り
                this.deliveryCarNumberSetting.init(
                    _db,
                    _signInManager,
                    _userManager,
                    _logger,
                    onDecisionHeaderName);

                //対象となる運送会社を指定
                this.deliveryCarNumberSetting.TRANSPORT_ADMIN_CODE = TRANSPORT_ADMIN_CODE;

                //モーダル表示用の情報を用意する
                this.deliveryCarNumberSetting.readDate(User, DELIVERY_REQUEST_NUMBER);
            }

            ///// <summary>
            ///// 出力種別
            ///// </summary>
            //protected enum ExportFileOutputType
            //{
            //    /// <summary>
            //    /// PDFのみ出力
            //    /// </summary>
            //    onlyPDF,
            //    /// <summary>
            //    /// Excelのみ出力
            //    /// </summary>
            //    onlyExcel,
            //    /// <summary>
            //    /// 両方出力
            //    /// </summary>
            //    Both
            //}

            ///// <summary>
            ///// ダウンロード用のファイルを準備する
            ///// </summary>
            //protected void prepareDownloadExportFile(DeliveryReportConfig reportConfig, ExportFileOutputType type)
            //{
            //    #region ローカル関数

            //    //出力対象のファイルを生成する
            //    void LF_makeFiles(string zipPath, string nowStr)
            //    {
            //        //サービスセット
            //        var serviceSet = makeServiceSet();

            //        #region エクスポート処理
            //        var exportTasks = new List<Task>();

            //        //運転日報を生成する
            //        if (type == ExportFileOutputType.Both ||
            //            type == ExportFileOutputType.onlyExcel)
            //        {
            //            exportTasks.Add(Task.Run(() =>
            //            {
            //                #region 運転日報の生成
            //                var export = new DeliveryDailyReport(reportConfig
            //                        , Path.Combine(zipPath, $"運転日報_{nowStr}.xlsx"),
            //                        serviceSet.truckAdminService);
            //                var r = export.exportExcel();

            //                #endregion
            //            }));
            //        }

            //        //はい表を生成する
            //        if (type == ExportFileOutputType.Both ||
            //            type == ExportFileOutputType.onlyPDF)
            //        {
            //            exportTasks.Add(Task.Run(() =>
            //            {
            //                #region 運転日報の生成
            //                var exporter = new RazorPagesLearning.Report.Export();
            //                var rpt = new Report.DeliveryDistributionReport(reportConfig);
            //                exporter.exportPdf(rpt, Path.Combine(zipPath, $"はい表_{nowStr}.pdf"));

            //                #endregion
            //            }));
            //        }

            //        foreach (var t in exportTasks)
            //        {
            //            t.Wait();
            //        }

            //        #endregion
            //    }

            //    #endregion

            //    //一時ディレクトリの中で作業する
            //    RazorPagesLearning.Utility.ReportHelper.HelperFunctions.TemporaryDirectoryAccompanyBlock
            //        (async (workPath) =>
            //        {
            //            //zipとして圧縮されるフォルダを生成する
            //            var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
            //                DateTimeOffset.Now, "yyyyMMddHHmmss");
            //            var zipPath = Path.Combine(workPath, $"運転日報_はい表_{nowStr}");
            //            Directory.CreateDirectory(zipPath);

            //            //pdf、excelファイルを生成する
            //            LF_makeFiles(zipPath, nowStr);

            //            //ファイルをzip圧縮する
            //            var zipFileName = zipPath + ".zip";
            //            ZipFile.CreateFromDirectory(zipPath, zipFileName);

            //            //HTTP応答に書き出す
            //            RazorPagesLearning.Utility.ReportHelper.HelperFunctions.writeExistingFile(
            //                new Utility.ReportHelper.HelperFunctions.WriteExistingFileConfig
            //                {
            //                    ContentType = "application/zip",
            //                    fileName = $"export_{nowStr}.zip",
            //                    target = this.Response,
            //                    targetFilePath = zipFileName
            //                });

            //            //関数使用上awaitが必要なので待機させる
            //            await Task.CompletedTask;

            //            return true;
            //        }).Wait();
            //}

            ///// <summary>
            ///// レポート設定の生成結果
            ///// </summary>
            //protected class MakeDeliveryReportConfigResult
            //{
            //    public DeliveryReportConfig deliveryReportConfig;

            //    public bool isSucceed;

            //    public string errorMessage;
            //}

            ///// <summary>
            ///// レポート設定を生成する
            ///// </summary>
            ///// <param name="serviceSet"></param>
            ///// <param name="DELIVERY_REQUEST_NUMBER"></param>
            ///// <param name="TRANSPORT_ADMIN_CODE"></param>
            ///// <returns></returns>
            //protected MakeDeliveryReportConfigResult makeDeliveryReportConfig
            //    (ServiceSet serviceSet,
            //    string DELIVERY_REQUEST_NUMBER,
            //    string TRANSPORT_ADMIN_CODE)
            //{
            //    //選択条件より対象データを読み取る
            //    var delivery = serviceSet.deliveryService.read(new DeliveryRequestService.ReadConfig
            //    {
            //        DELIVERY_REQUEST_NUMBER = DELIVERY_REQUEST_NUMBER,
            //        TRANSPORT_ADMIN_CODE = TRANSPORT_ADMIN_CODE
            //    });
            //    if (false == delivery.succeed)
            //    {
            //        return new MakeDeliveryReportConfigResult
            //        {
            //            isSucceed = false,
            //            errorMessage = "指定された集配依頼情報が読み取れませんでした。"
            //        };
            //    }
            //    var deliveryTarget = delivery.result.FirstOrDefault();
            //    if (null == deliveryTarget)
            //    {
            //        return new MakeDeliveryReportConfigResult
            //        {
            //            isSucceed = false,
            //            errorMessage = "指定された集配依頼情報が読み取れませんでした。"
            //        };
            //    }

            //    var deliveryDetails = serviceSet.deliveryDetailService.read(new DeliveryDetailService.ReadConfig
            //    {
            //        DELIVERY_REQUEST_NUMBER = DELIVERY_REQUEST_NUMBER,
            //        TRANSPORT_ADMIN_CODE = TRANSPORT_ADMIN_CODE
            //    });

            //    if (false == deliveryDetails.succeed)
            //    {
            //        return new MakeDeliveryReportConfigResult
            //        {
            //            isSucceed = false,
            //            errorMessage = "指定された集配依頼詳細情報が読み取れませんでした。"
            //        };
            //    }

            //    return new MakeDeliveryReportConfigResult
            //    {
            //        deliveryReportConfig = new DeliveryReportConfig
            //        {
            //            deliveryRequest = deliveryTarget,
            //            targetInformation = deliveryDetails.result
            //        },
            //        isSucceed = true
            //    };
            //}
        }
    }


    /// <summary>
    /// 自社便選択画面
    /// 
    /// [使用できるユーザー権限]
    /// ・管理者
    /// ・作業者
    /// ・運送会社
    /// 
    /// </summary>
    [Authorize(Roles = "Admin,ShippingCompany,Worker", Policy = "PasswordExpiration")]
    public class DeliveryModel : DeliveryHelpers.DeliveryModelBase
    {
        /// <summary>
        /// 画面上に表示する項目
        /// </summary>
        public class ViewModel : SortableTableViewModelBase<Formatted_DELIVERY_REQUEST>
        {
            /// <summary>
            /// エラーメッセージ
            /// </summary>
            public string errorMessage { get; set; }

            /// <summary>
            /// 運送会社コード一覧
            /// </summary>
            public SelectItemSet transportAdmins { get; set; }

            /// <summary>
            /// クリックされた情報
            /// </summary>
            public class ClickedInformation
            {
                /// <summary>
                /// 集配依頼番号
                /// </summary>
                public string DELIVERY_REQUEST_NUMBER { get; set; }
                /// <summary>
                /// 運送会社コード
                /// </summary>
                public string TRANSPORT_ADMIN_CODE { get; set; }
            }

            /// <summary>
            /// 選択済みの情報
            /// </summary>
            public ClickedInformation clickedInformation { get; set; }

            public ViewModel()
            {
                this.transportAdmins = new SelectItemSet();
                this.clickedInformation = new ClickedInformation();
            }
        }

        /// <summary>
        /// 画面上の表示情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        public DeliveryModel(RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            SignInManager<IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager,
            ILogger<LoginModel> ref_logger)
        {
            //引数を取り込み
            this._db = ref_db;
            this._signInManager = ref_signInManager;
            this._userManager = ref_userManager;
            this._logger = ref_logger;

            //必要メンバーの更新
            this.viewModel = new ViewModel();
        }

        ///該当する運送会社一覧を取得する
        async Task<SelectItemSet> LF_readTransportAdmin(ServiceSet ref_serviceSet)
        {
            var ret = new SelectItemSet();

            //選択されていた場合、現在の値を保存しておく
            var nowSelected = this.viewModel?.transportAdmins?.selected;

            //ログイン中のユーザーアカウントを取得
            var now_u = await ref_serviceSet.userService.readLoggedUserInfo();
            switch (now_u.PERMISSION)
            {
                case USER_ACCOUNT.ACCOUNT_PERMISSION.Admin:
                case USER_ACCOUNT.ACCOUNT_PERMISSION.Worker:
                    {
                        //この権限の場合、全部の運送会社を表示する
                        ret.display =
                            ref_serviceSet.transportAdminService
                            .read()
                            .Select(e =>
                            new SelectListItem
                            {
                                Text = $"{e.CODE}:{e.NAME}",
                                Value = e.CODE
                            }).ToList();
                        //先頭要素を選択済みとする
                        ret.selected = ret.display.First().Value;
                        break;
                    }
                case USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany:
                    {
                        //この要素の場合、ユーザーが持っているIDのみを選択する
                        //DB存在チェック
                        var t = ref_serviceSet.transportAdminService
                            .read(now_u.TRANSPORT_ADMIN_CODE).FirstOrDefault();
                        if (null == t)
                        {
                            throw new ApplicationException($"運送会社コード : {now_u.TRANSPORT_ADMIN_CODE}は存在しません。");
                        }

                        ret.display = new List<SelectListItem> {
                                new SelectListItem{
                                    Text = $"{t.CODE}:{t.NAME}",
                                    Value = now_u.TRANSPORT_ADMIN_CODE
                                }
                            };
                        ret.selected = ret.display.First().Value;
                        break;
                    }
                default:
                    {
                        throw new ApplicationException("");
                    }
            }

            //選択済みだったらその値を再利用する
            if (null != nowSelected)
            {
                ret.selected = nowSelected;
            }

            return ret;
        }

        /// <summary>
        /// 画面上に表示する情報を読み取る
        /// </summary>
        private async Task<bool> readData()
        {
            //サービス一覧を取得
            var serviceSet = makeServiceSet();

            //ログイン中のユーザー権限で判定

            #region 運送会社コードの取り込み
            {
                this.viewModel.transportAdmins = await LF_readTransportAdmin(serviceSet);
            }
            #endregion

            //表示データの取り込み
            {
                var t = serviceSet
                   .deliveryService
                   .read(new Service.DB.DeliveryRequestService.ReadConfig
                   {
                       TRANSPORT_ADMIN_CODE = this.viewModel.transportAdmins.selected,
                       sortDirection = this.viewModel.sortDirection,
                       sortColumn = this.viewModel.sortColumn
                   });
                if (false == t.succeed)
                {
                    throw new ApplicationException("集配先依頼の取得処理失敗しました。");
                }
                this.viewModel.tableRows = t.result
                    .Select(e => new Utility.SortableTable.SortableTableRowInfo<Formatted_DELIVERY_REQUEST>
                    {
                        data = Formatted_DELIVERY_REQUEST.convert(serviceSet, e)
                    })
                    .ToList();
            }

            return true;
        }

        /// <summary>
        /// 最初の画面読み込み時
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync()
        {
            //データ更新
            await readData();

            return Page();
        }

        /// <summary>
        /// 画面情報更新時
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            //セレクトボックスにある運送会社コードを取り込み
            this.viewModel.clickedInformation.TRANSPORT_ADMIN_CODE = this.viewModel.transportAdmins.selected;



            //エラー情報がセットされていたら取り込む
            {
                #region エラー情報取り込み
                //遷移元からエラーメッセージが届いていたら張り付ける
                var eInfo = TempData.Get<ErrorHandoverData>("ErrorInfo");
                if (null != eInfo)
                {
                    this.viewModel.clickedInformation.DELIVERY_REQUEST_NUMBER = eInfo.DELIVERY_REQUEST_NUMBER;
                    this.viewModel.clickedInformation.TRANSPORT_ADMIN_CODE = eInfo.TRANSPORT_ADMIN_CODE;
                    this.viewModel.errorMessage = eInfo.errorMessage;
                }
                #endregion
            }

            //データ更新
            await readData();

            //モーダルの表示指定が来ていたら表示する
            {
                #region モーダル情報の更新
                //遷移元からエラーメッセージが届いていたら張り付ける
                var eInfo = TempData.Get<ErrorHandoverData>("ModalInfo");
                if (null != eInfo)
                {
                    //必要データをコピー
                    this.viewModel.clickedInformation.DELIVERY_REQUEST_NUMBER = eInfo.DELIVERY_REQUEST_NUMBER;
                    //運送会社情報はpostで来た値が正しい。
                    //ここに来るタイミングでは画面上の値がまだ反映されていいないため。                    
                    //this.viewModel.clickedInformation.TRANSPORT_ADMIN_CODE = eInfo.TRANSPORT_ADMIN_CODE;

                    //モーダル情報の更新
                    {
                        //モーダル用のデータ読み取り
                        this.deliveryCarNumberSetting.init(
                            _db,
                            _signInManager,
                            _userManager,
                            _logger,
                            "OnModalDecision");

                        //対象となる運送会社を指定
                        this.deliveryCarNumberSetting.TRANSPORT_ADMIN_CODE
                            = this.viewModel.clickedInformation.TRANSPORT_ADMIN_CODE;

                        this.deliveryCarNumberSetting
                            .readDate(User, this.viewModel.clickedInformation.DELIVERY_REQUEST_NUMBER);
                        this.deliveryCarNumberSetting.viewModel.errorMessage = eInfo.errorMessage;

                    }

                    //車番設定が無いので車番設定ポップアップ表示を行う
                    //モーダルの表示状態に切り替える
                    this.deliveryCarNumberSetting.viewModel.isShowModal = true;
                }
                else
                {
                    //モーダル表示不要なので閉じる
                    this.deliveryCarNumberSetting.viewModel.isShowModal = false;
                }
                #endregion
            }

            return Page();
        }


        /// <summary>
        /// 車番号表示用のポップアップ表示
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostOnStatusClick()
        {

            #region ローカル関数

            //車番情報を保持しているか判定
            bool hasCarNmber()
            {
                //サービス一覧を取得
                var serviceSet = makeServiceSet();
                var trucks = serviceSet.truckAdminService.read(this.viewModel.transportAdmins.selected);
                var t = trucks.Where(e =>
                e.NUMBER != null &&
                e.NUMBER != string.Empty &&
                e.CHARGE != null &&
                 e.CHARGE != string.Empty
                 );
                var hh = t.ToList();

                if (0 == t.Count())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            #endregion

            //該当運送会社の車両マスタ情報をとってくる。
            //車両マスタに車番と担当者がともに0件の状態が存在するか確認する
            if (true == hasCarNmber())
            {
                //車番設定があるので、車番設定画面へ飛ばす
                return RedirectToPage("./DeliveryCar",
                    new
                    {
                        DELIVERY_REQUEST_NUMBER = this.viewModel.clickedInformation.DELIVERY_REQUEST_NUMBER,
                        TRANSPORT_ADMIN_CODE = this.viewModel.clickedInformation.TRANSPORT_ADMIN_CODE
                    });
            }
            else
            {
                //ヘッダーを消すために一度postを通す
                var modalInfo = new ErrorHandoverData
                {
                    DELIVERY_REQUEST_NUMBER = this.viewModel.clickedInformation.DELIVERY_REQUEST_NUMBER,
                    TRANSPORT_ADMIN_CODE = this.viewModel.clickedInformation.TRANSPORT_ADMIN_CODE
                };
                TempData.Put("ModalInfo", modalInfo);

                return RedirectToPagePermanentPreserveMethod();
            }
        }

        /// <summary>
        /// モーダル画面で決定を押下した場合
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostOnModalDecision()
        {
            //モーダル用のデータ読み取り
            this.deliveryCarNumberSetting.init(
                _db,
                _signInManager,
                _userManager,
                _logger,
                "OnModalDecision");

            //モーダルオブジェクトに委任する
            var r = await this.deliveryCarNumberSetting.onDecision(this, this.viewModel.clickedInformation.DELIVERY_REQUEST_NUMBER);
            if (r == DeliveryCarNumberSetting.OnDecisionResult.Success)
            {
                //成功したら車番選択に移動
                return RedirectToPage("./DeliveryCar",
                    new
                    {
                        DELIVERY_REQUEST_NUMBER = this.viewModel.clickedInformation.DELIVERY_REQUEST_NUMBER,
                        TRANSPORT_ADMIN_CODE = this.viewModel.clickedInformation.TRANSPORT_ADMIN_CODE
                    });
            }
            else
            {
                //ヘッダーを消すために一度postを通す
                var modalInfo = new ErrorHandoverData
                {
                    DELIVERY_REQUEST_NUMBER = this.viewModel.clickedInformation.DELIVERY_REQUEST_NUMBER,
                    TRANSPORT_ADMIN_CODE = this.viewModel.clickedInformation.TRANSPORT_ADMIN_CODE,
                    errorMessage = this.deliveryCarNumberSetting.viewModel.errorMessage
                };
                TempData.Put("ModalInfo", modalInfo);

                return RedirectToPagePermanentPreserveMethod();
            }
        }

        ///// <summary>
        ///// エクスポート対象のファイルをダウンロードする
        ///// </summary>
        ///// <param name="DELIVERY_REQUEST_NUMBER"></param>
        ///// <param name="TRANSPORT_ADMIN_CODE"></param>
        //public IActionResult OnPostExportedDiles()
        //{
        //    //対象サービス
        //    var serviceSet = makeServiceSet();

        //    //対象情報を生成
        //    var cfg = makeDeliveryReportConfig(serviceSet,
        //        this.viewModel.clickedInformation.DELIVERY_REQUEST_NUMBER,
        //        this.viewModel.clickedInformation.TRANSPORT_ADMIN_CODE);
        //    if (true == cfg.isSucceed)
        //    {
        //        prepareDownloadExportFile(cfg.deliveryReportConfig, ExportFileOutputType.Both);
        //        return Page();
        //    }
        //    else
        //    {
        //        //エラー情報をセットする
        //        var exportFileInfo = new ErrorHandoverData
        //        {
        //            DELIVERY_REQUEST_NUMBER = this.viewModel.clickedInformation.DELIVERY_REQUEST_NUMBER,
        //            TRANSPORT_ADMIN_CODE = this.viewModel.clickedInformation.TRANSPORT_ADMIN_CODE,
        //            errorMessage = cfg.errorMessage
        //        };
        //        TempData.Put("ErrorInfo", exportFileInfo);

        //        return RedirectToPagePermanentPreserveMethod();
        //    }
        //}


        #region フォーマット済みの定義

        /// <summary>
        /// フォーマット済みの集配情報
        /// </summary>
        public class Formatted_DELIVERY_REQUEST : DELIVERY_REQUEST
        {
            /// <summary>
            /// サービスセット
            /// </summary>
            private ServiceSet serviceSet;

            /// <summary>
            /// AutoMapper使うとコンストラクタ使えないので
            /// 外しておく
            /// </summary>
            /// <param name="ref_serviceSet"></param>
            internal void init(ServiceSet ref_serviceSet)
            {
                this.serviceSet = ref_serviceSet;
            }

            /// <summary>
            /// フォーマット済みステータス情報
            /// </summary>
            public class FormattedSTATUSInfo
            {
                /// <summary>
                /// 画面に表示するcssクラス情報
                /// </summary>
                public string classString;

                /// <summary>
                /// 画面に表示する文字列
                /// </summary>
                public string viewString;

                //ステータス定義
                public Service.DB.DeliveryRequestService.DeliveryStatus deliveryStatus;

                /// <summary>
                ///データを生成する
                /// </summary>
                /// <param name="ref_serviceSet"></param>
                /// <param name="ref_code"></param>
                /// <returns></returns>
                internal static FormattedSTATUSInfo make(ServiceSet ref_serviceSet, string ref_code)
                {
                    #region ローカル関数

                    //CSSのクラスを取得する
                    string LF_getCSSClass(string code)
                    {
                        switch (code)
                        {
                            case "確定済み":
                                {
                                    return "button__state done";
                                }
                            case "入力可能":
                                {
                                    return "button__state base";
                                }
                            case "削除":
                                {
                                    return "button__state del";
                                }
                            case "修正依頼中":
                                {
                                    return "button__state modi";
                                }
                            default:
                                {
                                    throw new ApplicationException($"CODE : {code}は存在しません");
                                }
                        }
                    }

                    #endregion

                    const string KIND = "00100000";

                    //DBからドメイン情報を取得する
                    var domainInfo = ref_serviceSet.domainService.getValue(KIND, ref_code);
                    if (null == domainInfo)
                    {
                        throw new ApplicationException($"ドメイン情報が見つかりません。 KIND : {KIND} , CODE : {ref_code}");
                    }

                    return new FormattedSTATUSInfo
                    {
                        classString = LF_getCSSClass(domainInfo.VALUE),
                        viewString = domainInfo.VALUE,
                        deliveryStatus = DeliveryRequestService.toDeliveryStatus(domainInfo.VALUE)
                    };
                }
            }


            public FormattedSTATUSInfo formattedSTATUS()
            {
                return FormattedSTATUSInfo.make(this.serviceSet, this.STATUS);
            }

            /// <summary>
            /// フォーマット済みの集配日情報
            /// </summary>
            /// <returns></returns>
            public string FormattedDELIVERY_DATE()
            {
                return false == DELIVERY_DATE.HasValue ?
                    "-"
                    : DELIVERY_DATE.Value.ToString("yyyy/MM/dd tt", CultureInfo.InvariantCulture);
            }

            /// <summary>
            /// フォーマット済みの確定日情報
            /// </summary>
            /// <returns></returns>
            public string FormattedCONFIRM_DATETIME()
            {
                if (true == this.CONFIRM_DATETIME.HasValue)
                {
                    return this.CONFIRM_DATETIME.Value.ToString("yyyy/MM/dd HH:mm");
                }
                else
                {
                    return "-";
                }
            }

            /// <summary>
            /// フォーマット済みの実績修正日
            /// </summary>
            /// <returns></returns>
            public string FormattedCORRECTION_DATETIME()
            {

                if (true == this.CORRECTION_DATETIME.HasValue)
                {
                    return this.CORRECTION_DATETIME.Value.ToString("yyyy/MM/dd HH:mm");
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
            internal static Formatted_DELIVERY_REQUEST convert(ServiceSet ref_serviceSet, DELIVERY_REQUEST org)
            {
                //データ形式を変換する
                var m = Mapper.Map<Formatted_DELIVERY_REQUEST>(org);
                m.init(ref_serviceSet);
                return m;
            }
        }

        #endregion
    }

    #region 補助関数

    /// <summary>
    /// 実装補助関数群
    /// </summary>
    namespace Helper
    {
        /// <summary>
        /// ディレクトリ操作補助
        /// 
        /// [参考]
        /// http://jeanne.wankuma.com/tips/csharp/directory/deletesurely.html
        ///  
        /// </summary>
        static class DirectoryHelper
        {
            /// ----------------------------------------------------------------------------
            /// <summary>
            ///     指定したディレクトリをすべて削除します。</summary>
            /// <param name="stDirPath">
            ///     削除するディレクトリのパス。</param>
            /// ----------------------------------------------------------------------------
            public static void DeleteDirectory(string stDirPath)
            {
                DeleteDirectory(new System.IO.DirectoryInfo(stDirPath));
            }


            /// ----------------------------------------------------------------------------
            /// <summary>
            ///     指定したディレクトリをすべて削除します。</summary>
            /// <param name="hDirectoryInfo">
            ///     削除するディレクトリの DirectoryInfo。</param>
            /// ----------------------------------------------------------------------------
            public static void DeleteDirectory(System.IO.DirectoryInfo hDirectoryInfo)
            {
                // すべてのファイルの読み取り専用属性を解除する
                foreach (System.IO.FileInfo cFileInfo in hDirectoryInfo.GetFiles())
                {
                    if ((cFileInfo.Attributes & System.IO.FileAttributes.ReadOnly) == System.IO.FileAttributes.ReadOnly)
                    {
                        cFileInfo.Attributes = System.IO.FileAttributes.Normal;
                    }
                }

                // サブディレクトリ内の読み取り専用属性を解除する (再帰)
                foreach (System.IO.DirectoryInfo hDirInfo in hDirectoryInfo.GetDirectories())
                {
                    DeleteDirectory(hDirInfo);
                }

                // このディレクトリの読み取り専用属性を解除する
                if ((hDirectoryInfo.Attributes & System.IO.FileAttributes.ReadOnly) == System.IO.FileAttributes.ReadOnly)
                {
                    hDirectoryInfo.Attributes = System.IO.FileAttributes.Directory;
                }

                // このディレクトリを削除する
                hDirectoryInfo.Delete(true);
            }

        }


    }

    #endregion

}