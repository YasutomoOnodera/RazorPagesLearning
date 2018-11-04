using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Pages.DeliveryHelpers;
using RazorPagesLearning.Report;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Utility.SelectItem;
using RazorPagesLearning.Utility.TempDataHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace RazorPagesLearning.Pages
{
    /// <summary>
    /// ルート入力画面
    /// </summary>
    [Authorize(Roles = "Admin,ShippingCompany,Worker", Policy = "PasswordExpiration")]
    public class DeliveryRouteModel : DeliveryHelpers.DeliveryDetailBase<DeliveryRouteModel.DeliveryDetailViewModel>
    {
        /// <summary>
        /// 画面上に表示する項目
        /// </summary>
        public class DeliveryDetailViewModel : DeliveryHelpers.ViewModelBase
        {
            /// <summary>
            /// 車番号
            /// </summary>
            public SelectItemSet selectedCar { get; set; }

            /// <summary>
            /// RazorPagesLearningへの送信完了後ステータス実行状態
            /// </summary>
            public bool onPostProcessingStatus { get; set; }

            public DeliveryDetailViewModel()
            {
                onPostProcessingStatus = false;
            }

        }

        public DeliveryRouteModel(RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
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
            this.viewModel = new DeliveryDetailViewModel();
        }

        /// <summary>
        /// 車番の絞り込みを行う
        /// </summary>
        /// <param name="serviceSet"></param>
        private void setUpSelectedCar(ServiceSet serviceSet)
        {
            //車番の一覧を作成する
            var trucks = serviceSet.truckAdminService.read(this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE);
            var cars = this.viewModel.tableRows
                .Select(e => e.data.info.TRUCK_MANAGE_NUMBER)
                .Distinct()
                .Select(e =>
                trucks.Where(in_e => in_e.TRUCK_MANAGE_NUMBER == e).FirstOrDefault()
                )
                .Where(e => e != null);

            //選択情報を作成する
            if (null == this.viewModel.selectedCar)
            {
                this.viewModel.selectedCar = new SelectItemSet();
            }
            this.viewModel.selectedCar.display = cars.Select(e =>
                new SelectListItem
                {
                    Value = e.TRUCK_MANAGE_NUMBER.ToString(),
                    Text = e.NUMBER
                })
                .ToList();

            //postで値が戻ってきていたらその値を採用する
            if (null == this.viewModel.selectedCar.selected)
            {
                //先頭を選択する
                this.viewModel.selectedCar.selected = this.viewModel.selectedCar.display.FirstOrDefault()?.Value;
            }
        }

        /// <summary>
        /// ユーザーが選択した車番で表示内容をフィルタする
        /// </summary>
        private void filterBySelectedCarNumber()
        {
            //サービス一覧
            var serviceSet = makeServiceSet();

            //車番の絞り込みセレクトボックスの絞り込みを行う
            setUpSelectedCar(serviceSet);

            //作成済みの表示情報を車番情報でフィルタリングする
            this.viewModel.tableRows =
                this.viewModel.tableRows.Where(e => e.data.info.TRUCK_MANAGE_NUMBER == int.Parse(this.viewModel.selectedCar.selected))
                .ToList();
        }

        /// <summary>
        /// 画面表示時
        /// </summary>
        /// <param name="DELIVERY_DETAIL_NUMBER"></param>
        public override void OnGet(string DELIVERY_REQUEST_NUMBER, string TRANSPORT_ADMIN_CODE)
        {
            //サービス一覧
            var serviceSet = makeServiceSet();

            //親クラスで一括して読み取りする
            base.OnGet(DELIVERY_REQUEST_NUMBER, TRANSPORT_ADMIN_CODE);

            //ユーザーが選択した車番号でフィルタする
            filterBySelectedCarNumber();

        }

        /// <summary>
        /// 入力済みデータを保持して復元処理込みで再描画用のデータを準備する
        /// </summary>
        protected override void dataPreparationWithInputDataRestoration(ServiceSet serviceSet = null)
        {
            if (null == serviceSet)
            {
                serviceSet = makeServiceSet();
            }

            //入力中のデータが変更してしまうので、
            //画面上のデータを一度退避しておく
            var orgData = this.viewModel.tableRows.Select(e => e.data.info);

            //該当データの読み込みを行う
            readDataFromDB();

            //ユーザーが選択した車番号でフィルタする
            filterBySelectedCarNumber();

            //取得しておいた入力済みデータを復元する
            foreach (var ele in this.viewModel.tableRows)
            {
                var src = orgData.Where(e => e.ID == ele.data.info.ID &&
                e.TRANSPORT_ADMIN_CODE == ele.data.info.TRANSPORT_ADMIN_CODE &&
                e.DELIVERY_REQUEST_NUMBER == ele.data.info.DELIVERY_REQUEST_NUMBER).FirstOrDefault();
                if (null != src)
                {
                    ele.data.info.ROUTE = src.ROUTE;
                }
            }
        }

        /// <summary>
        /// 画面の内容が更新された時
        /// </summary>
        public void OnPost()
        {
            //前頁から引継ぎ情報が来ていたら反映させる。
            readAndReflectErrorHandoverData();

            //入力済みデータの復元処理込みでデータを準備を行う。
            dataPreparationWithInputDataRestoration();

        }

        /// <summary>
        /// DBにデータを保存する
        /// </summary>
        /// <returns></returns>
        protected override async Task<DoSaveToDBResult> doSaveToDB()
        {
            //入力エラーチェック
            var jr = DeliveryHelpers.RouteHelperFunctions.hasConfigurationError(this.viewModel
                    .tableRows.Select(e => e.data.info)
                    .Cast<DELIVERY_REQUEST_DETAIL>());
            if (false == jr.Item1)
            {
                //ルート情報が全て入力されている

                //サービス一覧
                var serviceSet = makeServiceSet();

                //データをDBに保存する
                await serviceSet.deliveryDetailService
                    .updateOnlyROUTEInfo(this.viewModel.tableRows.Select(e => e.data.info));

                await this._db.SaveChangesAsync();

                //該当データの読み込みを行う
                readDataFromDB();

                //ユーザーが選択した車番号でフィルタする
                filterBySelectedCarNumber();

                return DoSaveToDBResult.Success;
            }
            else
            {
                //未入力のルート情報が存在する
                this.viewModel.errorMessage = jr.Item2;

                //入力済みデータの復元処理込みでデータを準備を行う。
                dataPreparationWithInputDataRestoration();

                return DoSaveToDBResult.Failure;
            }
        }

        /// <summary>
        /// データを保存する
        /// </summary>
        public async Task<IActionResult> OnPostSaveAsync()
        {
            var r = await doSaveToDB();
            switch (r)
            {
                case DoSaveToDBResult.Success:
                    {
                        return RedirectToPagePermanentPreserveMethod("DeliveryRoute");
                    }
                case DoSaveToDBResult.Failure:
                    {
                        return errorRedirectToThisPage();
                    }
                default:
                    {
                        throw new ApplicationException("DB保存処理が想定外の動作を行いました。");
                    }
            }

            throw new ApplicationException("DB保存処理が想定外の動作を行いました。");
        }

        /// <summary>
        /// 車番入力画面に遷移する
        /// </summary>
        public IActionResult OnPostMoveToCar()
        {
            //入力エラーチェック
            var jr = DeliveryHelpers.RouteHelperFunctions.hasConfigurationError(this.viewModel
                    .tableRows.Select(e => e.data.info)
                    .Cast<DELIVERY_REQUEST_DETAIL>());
            if (false == jr.Item1)
            {
                //入力エラーなしなので遷移可能

                //車番設定があるので、車番設定画面へ飛ばす
                return RedirectToPage("DeliveryCar",
                    new
                    {
                        DELIVERY_REQUEST_NUMBER = this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER,
                        TRANSPORT_ADMIN_CODE = this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE
                    });
            }
            else
            {
                //入力エラー有り

                //未入力のルート情報が存在する
                this.viewModel.errorMessage = jr.Item2;

                //入力済みデータの復元処理込みでデータを準備を行う。
                dataPreparationWithInputDataRestoration();

                return Page();
            }
        }

        /// <summary>
        /// RazorPagesLearningに送信可能な状態か判定する
        /// </summary>
        /// <returns></returns>
        private async Task< IActionResult > checkDataCanSendToRazorPagesLearning(ServiceSet serviceSet)
        {
            var tragetData = readDeliveryDetail(serviceSet);

            //車番が未設定の項目が存在したらエラーとする
            //このページに来る前にエラーチェックではじかれている想定だが念のためチェックする
            {
                var e = DeliveryHelpers.CarHelperFunctions.hasConfigurationError(tragetData);
                if (true == e.Item1)
                {
                    //入力内容に問題があったのでエラーとする
                    this.viewModel.errorMessage = e.Item2;
                    //入力済みデータの復元処理込みでデータを準備を行う。
                    dataPreparationWithInputDataRestoration();
                    return errorRedirectToThisPage();
                }
            }

            //現在の画面の入力済み項目に関してチェック
            {
                var r = await doSaveToDB();
                if (r == DoSaveToDBResult.Failure)
                {
                    //この時点で入力内容に問題があったらエラー扱いとする
                    return errorRedirectToThisPage();
                }
            }

            //問題なければ、他の車に関してもDB上から値をとってきてチェックを行う
            {
                var e = DeliveryHelpers.RouteHelperFunctions
                    .hasConfigurationErrorForAllTruck(serviceSet, tragetData, this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE);
                if (true == e.Item1)
                {
                    //未入力のルート情報が存在する
                    this.viewModel.errorMessage = e.Item2;

                    //入力済みデータの復元処理込みでデータを準備を行う。
                    dataPreparationWithInputDataRestoration();

                    //他の履歴にも問題がある場合
                    return errorRedirectToThisPage();
                }
            }

            return null;
        }


        /// <summary>
        /// RazorPagesLearningへ送信する
        /// </summary>
        public async Task<IActionResult> OnPostSendToRazorPagesLearningAsync()
        {
            var serviceSet = makeServiceSet();

            //現在の入力済み情報に関して、チェック、保存する
            using (var transaction = this._db.Database.BeginTransaction())
            {

                //入力状態チェック
                var check = await checkDataCanSendToRazorPagesLearning(serviceSet);
                if (null != check)
                {
                    return check;
                }

                //ステータスを変更する
                {
                    var r = await serviceSet.deliveryService.statusUpdate(
                         new Service.DB.DeliveryRequestService.StatusUpdateConfig
                         {
                             DELIVERY_REQUEST_NUMBER = this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER,
                             TRANSPORT_ADMIN_CODE = this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE,
                             status = Service.DB.DeliveryRequestService.DeliveryStatus.確定済み
                         });
                    if (r.succeed == false)
                    {
                        this.viewModel.errorMessage = String.Join("<br>", r.errorMessages);
                    }

                    //確定保存する
                    await serviceSet.deliveryService.db.SaveChangesAsync();
                }

                //-- -- -- --
                //[ToDo]
                //Web API でRazorPagesLearningへ送信する
                //-- -- -- --

                //DBへ確定する
                transaction.Commit();
            }
            //全てOKだったら、運転日報、はい表を作成する
            {
                //遷移先ページでダウンロードさせる
                var exportFileInfo = new DeliveryHelpers.ViewModelBase.FilteringConditions
                {
                    DELIVERY_REQUEST_NUMBER = this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER,
                    TRANSPORT_ADMIN_CODE = this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE
                };
                TempData.Put("exportFileInfo", exportFileInfo);
            }

            //次の処理としては、配表/日報処理をダウンロートさせつつ、
            //一覧ページへ遷移させる。
            //ダウンロードとページ遷移を並行して実施する事はできないため、
            //ダウンロード用の別画面を開いてそちらでダウンロードさせる。
            //
            //このような動作を実現するために、
            //完了状態になった事をpage上に記録して、
            //クライアント側に返して、クライアント側で並行動作させる。
            //
            //完了状態に切り替える
            this.viewModel.onPostProcessingStatus = true;

            return Page();

        }


        ///// <summary>
        ///// エクスポート対象のファイルをダウンロードする
        ///// </summary>
        ///// <param name="DELIVERY_REQUEST_NUMBER"></param>
        ///// <param name="TRANSPORT_ADMIN_CODE"></param>
        //public async Task<IActionResult> OnPostExportedDilesAsync()
        //{
        //    //対象サービス
        //    var serviceSet = makeServiceSet();

        //    //入力状態チェック
        //    var check = await checkDataCanSendToRazorPagesLearning(serviceSet);
        //    if (null != check)
        //    {
        //        return check;
        //    }

        //    ////対象情報を生成
        //    //var cfg = makeDeliveryReportConfig(serviceSet,
        //    //    this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER,
        //    //    this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE);
        //    //if (true == cfg.isSucceed)
        //    //{
        //    //    prepareDownloadExportFile(cfg.deliveryReportConfig, ExportFileOutputType.onlyExcel);
        //    //    return RedirectToPagePermanentPreserveMethod();
        //    //}
        //    //else
        //    //{
        //    //    //エラー情報を表示する
        //    //    this.viewModel.errorMessage = cfg.errorMessage;
        //    //    return errorRedirectToThisPage();
        //    //}
        //}

    }
}