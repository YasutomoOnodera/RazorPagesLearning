using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Pages.DeliveryHelpers;
using RazorPagesLearning.Report;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Service.Definition;
using RazorPagesLearning.Service.Utility;
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
using static RazorPagesLearning.Pages.DeliveryHelpers.DeliveryModelBase;
using static RazorPagesLearning.Service.DB.DeliveryDetailService;

namespace RazorPagesLearning.Pages
{

    /// <summary>
    /// 自社便画面で共通に使用出来る補助的な関数、オブジェクト群を定義
    /// </summary>
    namespace DeliveryHelpers
    {
        /// <summary>
        /// ルート入力処理における補助関数群
        /// </summary>
        static class RouteHelperFunctions
        {
            /// <summary>
            /// 指定されたルート情報に問題がないか確認する
            /// </summary>
            /// <returns></returns>
            public static Tuple<bool, string> hasConfigurationError(IEnumerable<DELIVERY_REQUEST_DETAIL> targetList)
            {
                #region ローカル関数

                ///未入力のルート情報が存在するか判定する
                bool LF_hasNosettedRoutes()
                {
                    //未入力件数を取得
                    var nonInput = targetList
                        .Where(e => null == e.ROUTE
                        ).Count();

                    if (0 != nonInput)
                    {
                        //未選択の項目がある
                        return true;
                    }

                    return false;
                }

                ///ルートの重複を検査する
                ///この関数はNULL状態のデータが存在しないことを前提としている。
                Tuple<bool, string> LF_hasOverlappedRoute()
                {
                    //集計済みのルートを数える
                    var rours = targetList
                        .Select(e => e.ROUTE)
                        .GroupBy(e => e.Value)
                        .Select(c => new { route = c.Key, Count = c.Count() });

                    //重複しているレコードを抽出する
                    var overlap = rours.Select(e => e.Count)
                        .Where(e => e != 1);
                    if (0 == overlap.Count())
                    {
                        //重複入力なし
                        return Tuple.Create<bool, string>(false, "");
                    }
                    else
                    {
                        //重複入力あり
                        var overlapRoutes = rours.Where(e => e.Count != 1)
                            .Select(e => e.route);
                        var eMsg = $"ルート{String.Join(",", overlapRoutes)}が重複して入力されています。";
                        return Tuple.Create<bool, string>(true, eMsg);
                    }
                }

                //ルート入力結果が連番になっていないか確認する
                Tuple<bool, string> LF_isNotSequenceNumber()
                {
                    //対象ルート
                    var rours = targetList
                        .Select(e => e.ROUTE)
                        .OrderBy(e => e.Value);

                    //ルートは1から連番になっている事
                    if (1 != rours.First())
                    {
                        var eMsg = $"ルートは1から連番で指定する必要があります。";
                        return Tuple.Create<bool, string>(true, eMsg);
                    }

                    //ループでルートの連番性を検証ずる
                    {
                        int? pre = null;
                        foreach (var ele in rours)
                        {
                            if (true == pre.HasValue)
                            {
                                if (pre.Value + 1 != ele.Value)
                                {
                                    //連番規則に乗っ取っていないので
                                    //エラー扱いとする

                                    var eMsg = $"ルート{pre.Value}とルート{ele.Value}が連番になっていません。";
                                    return Tuple.Create<bool, string>(true, eMsg);
                                }
                            }

                            //次に送る
                            pre = ele;
                        }
                    }

                    return Tuple.Create<bool, string>(false, "");
                }


                #endregion

                //ルート設定を確認
                if (true == LF_hasNosettedRoutes())
                {
                    return Tuple.Create<bool, string>
                        (true, "未入力のルートが存在します。");
                }

                //ルートが重複しているか判定する
                {
                    var r = LF_hasOverlappedRoute();
                    if (true == r.Item1)
                    {
                        return r;
                    }
                }

                //入力データが連番になっているか確認する
                return LF_isNotSequenceNumber();

            }

            //全ての車に関して入力状態のチェックを行う
            public static Tuple<bool, string> hasConfigurationErrorForAllTruck
                (ServiceSet ref_serviceSet, IEnumerable<DELIVERY_REQUEST_DETAIL> targetList, string TRANSPORT_ADMIN_CODE)
            {
                //車番が入力されていない行が無いか確認する
                {
                    var noTrunks = targetList.Select(e => e.TRUCK_MANAGE_NUMBER)
                        .Where(e => false == e.HasValue);
                    if (0 != noTrunks.Count())
                    {
                        return Tuple.Create(true, "車番が入力されていない項目があります。");
                    }
                }

                //台車番号でフィルタする
                var trunks = targetList.Select(e => e.TRUCK_MANAGE_NUMBER)
                    .Where(e => true == e.HasValue)
                    .GroupBy(e => e.Value)
                    .Select(c => c.Key);


                //車番要素ごとにルートの入力条件を確認する
                foreach (var ele in trunks)
                {
                    //指定された台車番号を持つ情報を抽出する
                    var td = targetList
                        .Where(e => e.TRUCK_MANAGE_NUMBER == ele);

                    //入力エラーチェックを行う
                    var jr = DeliveryHelpers.RouteHelperFunctions.hasConfigurationError(td);
                    if (true == jr.Item1)
                    {
                        //車番が現在表示ているものと異なるため
                        //情報を追加する
                        var truckInfo = ref_serviceSet.truckAdminService.read
                            (TRANSPORT_ADMIN_CODE,
                             ele)
                             .First();

                        return Tuple.Create(true, $"車番 : {truckInfo.NUMBER}において、{jr.Item2}");
                    }
                }
                return Tuple.Create<bool, string>(false, null);
            }

        }

        /// <summary>
        /// 車番入力において、問題が無いか確認する関数
        /// </summary>
        static class CarHelperFunctions
        {
            public static Tuple<bool, string> hasConfigurationError(IEnumerable<DELIVERY_REQUEST_DETAIL> targetList)
            {
                var notSet = targetList
                    .Where(e => e.TRUCK_MANAGE_NUMBER == null);

                if (0 != notSet.Count())
                {
                    return Tuple.Create<bool, string>(true, "車番が未選択の項目があります。");
                }

                return Tuple.Create<bool, string>(false, "");
            }
        }


        /// <summary>
        /// 車番情報に関して関係テーブルからのデータ表示に対応した、
        /// 集配詳細情報
        /// </summary>
        public class TRUCK_Corrected_DELIVERY_DETAIL : CollisionDetectable_DELIVERY_DETAIL
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

            public string formattedTRUCK_NUMBER()
            {
                if (true == TRUCK_MANAGE_NUMBER.HasValue)
                {

                    //該当情報をDBから取得する
                    var t = serviceSet.truckAdminService.read(this.TRANSPORT_ADMIN_CODE, TRUCK_MANAGE_NUMBER.Value)
                        .FirstOrDefault();
                    if (null != t)
                    {
                        return t.NUMBER;
                    }
                }

                return "-";
            }

            /// <summary>
            /// DBから取得される形式から表示可能な形式に変換する
            /// </summary>
            /// <param name=""></param>
            /// <returns></returns>
            internal static TRUCK_Corrected_DELIVERY_DETAIL convert(ServiceSet ref_serviceSet, CollisionDetectable_DELIVERY_DETAIL org)
            {
                //データ形式を変換する
                var m = Mapper.Map<TRUCK_Corrected_DELIVERY_DETAIL>(org);
                m.init(ref_serviceSet);
                return m;
            }

        }


        /// <summary>
        /// 画面上に表示するレコード情報
        /// </summary>
        public class ViewableRecord
        {
            /// <summary>
            /// 選択可能な車一覧
            /// </summary>
            public SelectItemSet selectableCars { get; set; }

            /// <summary>
            /// 表示対象の情報
            /// </summary>
            public TRUCK_Corrected_DELIVERY_DETAIL info { get; set; }

        }

        /// <summary>
        /// ページ間で遷移する時に受け渡しするエラー関係のリダイレクト情報
        /// </summary>
        public class ErrorHandoverData : DeliveryHelpers.ViewModelBase.FilteringConditions
        {
            /// <summary>
            /// エラーメッセージ
            /// </summary>
            public string errorMessage { get; set; }
        }

        /// <summary>
        /// 画面上に表示する項目
        /// </summary>
        public class ViewModelBase : SortableTableViewModelBase<ViewableRecord>
        {
            /// <summary>
            /// レコード絞り込み条件
            /// </summary>
            public class FilteringConditions
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
            /// 表示レコードをフィルタリングする条件
            /// </summary>
            public FilteringConditions filteringConditions { get; set; }

            /// <summary>
            /// エラーメッセージ
            /// </summary>
            public string errorMessage { get; set; }

            public ViewModelBase()
            {
                this.filteringConditions = new FilteringConditions();
                this.errorMessage = "";
            }
        }

        /// <summary>
        /// 集配依頼詳細ページ　基底となるページ
        /// </summary>
        public abstract class DeliveryDetailBase<ViewModelType>
            : DeliveryHelpers.DeliveryModelBase
             where ViewModelType : ViewModelBase
        {

            /// <summary>
            /// 画面上の表示情報
            /// </summary>
            [BindProperty]
            public ViewModelType viewModel { get; set; }

            /// <summary>
            /// データを表示可能な形式にフォーマットする
            /// </summary>
            protected void formDisplayableForm(ServiceSet serviceSet, IEnumerable<DELIVERY_REQUEST_DETAIL> srcData)
            {
                #region ローカル関数

                ///選択可能な車一覧を取得する
                SelectItemSet makeSelectableTRUCK(ServiceSet ref_serviceSet)
                {
                    //登録済みの車両マスターを読み込む
                    var TRUCKs = ref_serviceSet.truckAdminService
                        .read(this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE)
                        .Where(e => e.NUMBER != null);

                    //登録済み車両を選択可能な形式へ変換する
                    var selectableTRUCKs = TRUCKs
                        .OrderBy(e => e.TRUCK_MANAGE_NUMBER)
                        .Select(e => new SelectListItem
                        {
                            Value = e.TRUCK_MANAGE_NUMBER.ToString(),
                            Text = e.NUMBER
                        })
                    .ToList();

                    //先頭に空要素を入れておく
                    selectableTRUCKs.Insert(0, new SelectListItem());

                    return new SelectItemSet
                    {
                        display = selectableTRUCKs,
                        selected = selectableTRUCKs.FirstOrDefault()?.Value
                    };
                }

                ///選択済みの台車情報を取得する
                string getSelectedTRUCKInfo(SelectItemSet ref_selectItemSet, DELIVERY_REQUEST_DETAIL delivery_detail)
                {
                    if (true == delivery_detail.TRUCK_MANAGE_NUMBER.HasValue)
                    {
                        var dbSelected = ref_selectItemSet.display
                        .Where(in_e => in_e.Value == delivery_detail.TRUCK_MANAGE_NUMBER.Value.ToString())
                        .FirstOrDefault();
                        if (null != dbSelected)
                        {
                            return dbSelected.Value;
                        }
                    }
                    else
                    {
                        //確定状態の場合、TRUCK_MANAGE_NUMBERはクリアされているので、
                        //コピーされているTRUCK_NUMBER側の値を採用する
                        if (null != delivery_detail.TRUCK_NUMBER &&
                           String.Empty != delivery_detail.TRUCK_NUMBER)
                        {
                            var t = ref_selectItemSet.display.Where(e => e.Text == delivery_detail.TRUCK_NUMBER)
                                .FirstOrDefault();
                            if (null != t)
                            {
                                return t.Value;
                            }
                        }
                    }

                    //値が存在しない場合、未選択としておく
                    return "";
                }

                //ソートをかける
                IEnumerable<DELIVERY_REQUEST_DETAIL> setSortOrder
                    (string column, SortDirection sortDirection, IEnumerable<DELIVERY_REQUEST_DETAIL> q)
                {

                    #region ローカル関数

                    //主条件でソートする
                    IOrderedEnumerable<DELIVERY_REQUEST_DETAIL> LF_sortByPrimaryKey()
                    {
                        //デフォルトソート順序
                        if (null == column)
                        {
                            column = "TRUCK_MANAGE_NUMBER";
                        }

                        switch (column.Trim())
                        {
                            // 車番
                            case "TRUCK_MANAGE_NUMBER":
                                {
                                    switch (sortDirection)
                                    {
                                        case SortDirection.ASC:
                                            {
                                                return q.OrderBy(e => e.TRUCK_MANAGE_NUMBER);
                                            }
                                        case SortDirection.DES:
                                            {
                                                return q.OrderByDescending(e => e.TRUCK_MANAGE_NUMBER);
                                            }
                                    }
                                    break;
                                }
                            // カラムの表示情報
                            case "ROUTE":
                                {
                                    switch (sortDirection)
                                    {
                                        case SortDirection.ASC:
                                            {
                                                return q.OrderBy(e => e.ROUTE);
                                            }
                                        case SortDirection.DES:
                                            {
                                                return q.OrderByDescending(e => e.ROUTE);
                                            }
                                    }
                                    break;
                                }
                            // 集配先会社
                            case "COMPANY":
                                {
                                    switch (sortDirection)
                                    {
                                        case SortDirection.ASC:
                                            {
                                                return q.OrderBy(e => e.COMPANY);
                                            }
                                        case SortDirection.DES:
                                            {
                                                return q.OrderByDescending(e => e.COMPANY);
                                            }
                                    }
                                    break;
                                }
                            // 配達品名数量
                            case "DELIVERY_NOTE":
                                {
                                    switch (sortDirection)
                                    {
                                        case SortDirection.ASC:
                                            {
                                                return q.OrderBy(e => e.DELIVERY_NOTE);
                                            }
                                        case SortDirection.DES:
                                            {
                                                return q.OrderByDescending(e => e.DELIVERY_NOTE);
                                            }
                                    }
                                    break;
                                }
                            // 配達A/C先
                            case "DELIVERY_AC":
                                {
                                    switch (sortDirection)
                                    {
                                        case SortDirection.ASC:
                                            {
                                                return q.OrderBy(e => e.DELIVERY_AC);
                                            }
                                        case SortDirection.DES:
                                            {
                                                return q.OrderByDescending(e => e.DELIVERY_AC);
                                            }
                                    }
                                    break;
                                }
                            // 集荷品名数量
                            case "CARGO_TITLE":
                                {
                                    switch (sortDirection)
                                    {
                                        case SortDirection.ASC:
                                            {
                                                return q.OrderBy(e => e.CARGO_TITLE);
                                            }
                                        case SortDirection.DES:
                                            {
                                                return q.OrderByDescending(e => e.CARGO_TITLE);
                                            }
                                    }
                                    break;
                                }
                            // 集荷備考
                            case "CARGO_NOTE":
                                {
                                    switch (sortDirection)
                                    {
                                        case SortDirection.ASC:
                                            {
                                                return q.OrderBy(e => e.CARGO_NOTE);
                                            }
                                        case SortDirection.DES:
                                            {
                                                return q.OrderByDescending(e => e.CARGO_NOTE);
                                            }
                                    }
                                    break;
                                }
                            // 集荷A/C先
                            case "CARGO_AC":
                                {
                                    switch (sortDirection)
                                    {
                                        case SortDirection.ASC:
                                            {
                                                return q.OrderBy(e => e.CARGO_AC);
                                            }
                                        case SortDirection.DES:
                                            {
                                                return q.OrderByDescending(e => e.CARGO_AC);
                                            }
                                    }
                                    break;
                                }
                        }
                        throw new ApplicationException("想定外のソート条件です。");
                    }

                    #endregion

                    return LF_sortByPrimaryKey().ThenBy(e => e.TRUCK_MANAGE_NUMBER);

                }

                #endregion

                //車一覧を作成する
                var selectableTRUCK = makeSelectableTRUCK(serviceSet);

                //表示可能な形式へ変換する
                this.viewModel.tableRows = setSortOrder(this.viewModel.sortColumn, this.viewModel.sortDirection, srcData)
                    .ToList()
                    .Select(e =>
                    {
                        //確定画面の場合
                        return new SortableTableRowInfo<ViewableRecord>
                        {
                            data = new ViewableRecord
                            {
                                info = TRUCK_Corrected_DELIVERY_DETAIL.convert(serviceSet,
                                CollisionDetectable_DELIVERY_DETAIL.convert(e)),
                                //別べの選択要素となるのでコピーしておく
                                selectableCars = new SelectItemSet
                                {
                                    display = selectableTRUCK.display,
                                    //DB上にすでに選択済みの項目があったらそちらを選択した状態にする
                                    selected = getSelectedTRUCKInfo(selectableTRUCK, e)
                                }
                            }
                        };
                    })
                    .ToList();
            }

            /// <summary>
            /// DBからデータの読み取り
            /// </summary>
            /// <param name="serviceSet"></param>
            /// <returns></returns>
            protected IQueryable<DELIVERY_REQUEST_DETAIL> readDeliveryDetail(ServiceSet serviceSet)
            {
                //DBから該当条件のデータを読み取って、
                //表示可能な形式に変換する
                var tr = serviceSet.deliveryDetailService.read(new Service.DB.DeliveryDetailService.ReadConfig
                {
                    TRANSPORT_ADMIN_CODE = this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE,
                    DELIVERY_REQUEST_NUMBER = this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER
                });
                if (false == tr.succeed)
                {
                    throw new ApplicationException("集配先依頼詳細情報の読み取りに失敗しました。");
                }

                return tr.result;

            }

            /// <summary>
            /// DBからデータの読み込みを行う
            /// </summary>
            protected void readDataFromDB()
            {
                //サービス一覧
                var serviceSet = makeServiceSet();

                //DBからデータの読み取り
                var d = readDeliveryDetail(serviceSet);

                //画面上に表示可能な形式に変換する
                formDisplayableForm(serviceSet, d);
            }


            /// <summary>
            /// エラーリダイレクトの遷移時に引き渡す情報を設定する
            /// </summary>
            protected void setErrorHandoverData()
            {
                var eInfo = new ErrorHandoverData
                {
                    DELIVERY_REQUEST_NUMBER = this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER,
                    TRANSPORT_ADMIN_CODE = this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE,
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
                    this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER = eInfo.DELIVERY_REQUEST_NUMBER;
                    this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE = eInfo.TRANSPORT_ADMIN_CODE;
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

            /// <summary>
            /// モーダル画面で決定を押下した場合
            /// </summary>
            /// <returns></returns>
            public async Task<IActionResult> OnPostOnModalDecisionAsync()
            {
                //モーダル用のデータ読み取り
                this.deliveryCarNumberSetting.init(
                    _db,
                    _signInManager,
                    _userManager,
                    _logger,
                    "OnModalDecision");

                //モーダルオブジェクトに委任する
                var r = await this.deliveryCarNumberSetting.onDecision(this, this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER);
                if (r == PopUp.DeliveryCarNumberSetting.OnDecisionResult.Success)
                {
                    this.deliveryCarNumberSetting.viewModel.isShowModal = false;
                }
                else
                {
                    this.deliveryCarNumberSetting.viewModel.isShowModal = true;
                }

                //再表示用のデータの取り込み
                {
                    //該当データの読み込みを行う
                    readDataFromDB();

                    //モーダル表示用にデータを準備しておく
                    preparationForModalShow(this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE,
                        this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER,
                        "OnModalDecision");
                }

                return Page();
            }


            /// <summary>
            /// 画面表示時
            /// </summary>
            /// <param name="DELIVERY_DETAIL_NUMBER"></param>
            public virtual void OnGet(string DELIVERY_REQUEST_NUMBER, string TRANSPORT_ADMIN_CODE)
            {
                //対象レコードの絞り込み条件を決定
                this.viewModel.filteringConditions = new DeliveryHelpers.ViewModelBase.FilteringConditions
                {
                    DELIVERY_REQUEST_NUMBER = DELIVERY_REQUEST_NUMBER,
                    TRANSPORT_ADMIN_CODE = TRANSPORT_ADMIN_CODE
                };

                //該当データの読み込みを行う
                readDataFromDB();

            }

            /// <summary>
            /// 入力中のデータを復元可能な形に変換しつつデータの再読み込みを行う
            /// </summary>
            protected abstract void dataPreparationWithInputDataRestoration(ServiceSet serviceSet = null);

            /// <summary>
            /// RazorPagesLearningに対して修正依頼をかける
            /// </summary>
            /// <returns></returns>
            public async Task<IActionResult> OnPostModificationRequest()
            {
                using (var transaction = this._db.Database.BeginTransaction())
                {
                    var serviceSet = makeServiceSet();

                    //送信対象データ
                    var tragetData = readDeliveryDetail(serviceSet);

                    //送信可能な状態かエラーチェックする
                    {
                        #region エラーチェックする
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
                        #endregion
                    }

                    {
                        #region 未選択項目をDBに保存する
                        //未選択の項目あもあるので、一度DBに保存する
                        var r = await doSaveToDB();
                        if (r == DoSaveToDBResult.Failure)
                        {
                            return errorRedirectToThisPage();
                        }
                        #endregion
                    }

                    {
                        #region ステータスを変更する
                        var r = await serviceSet.deliveryService.statusUpdate(
                             new Service.DB.DeliveryRequestService.StatusUpdateConfig
                             {
                                 DELIVERY_REQUEST_NUMBER = this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER,
                                 TRANSPORT_ADMIN_CODE = this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE,
                                 status = Service.DB.DeliveryRequestService.DeliveryStatus.修正依頼中
                             });
                        if (r.succeed == false)
                        {
                            this.viewModel.errorMessage = String.Join("<br>", r.errorMessages);
                        }

                        //確定保存する
                        await serviceSet.deliveryService.db.SaveChangesAsync();
                        #endregion
                    }

                    //ToDo : 
                    //API送信

                    transaction.Commit();

                    //一覧画面へ遷移する
                    return RedirectToPage("Delivery");
                }
            }

            /// <summary>
            /// DB保存処理の処理結果
            /// </summary>
            protected enum DoSaveToDBResult
            {
                /// <summary>
                /// 処理成功
                /// </summary>
                Success,
                /// <summary>
                ///　処理失敗
                /// </summary>
                Failure
            }

            /// <summary>
            /// DBにデータを保存する
            /// </summary>
            /// <returns></returns>
            protected abstract Task<DoSaveToDBResult> doSaveToDB();
        }
    }

    /// <summary>
    /// 車番入力画面
    /// </summary>
    [Authorize(Roles = "Admin,ShippingCompany,Worker", Policy = "PasswordExpiration")]
    public class DeliveryCarModel : DeliveryHelpers.DeliveryDetailBase<DeliveryHelpers.ViewModelBase>
    {

        public DeliveryCarModel(RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
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
            this.viewModel = new DeliveryHelpers.ViewModelBase();
        }

        private bool? _confirmedStatus;

        /// <summary>
        /// 確定済み状態か判定する
        /// </summary>
        /// <returns></returns>
        public bool isConfirmedStatus(ServiceSet ref_serviceSet = null)
        {
            #region ローカル関数

            bool readStstus(ServiceSet serviceSet)
            {
                ///対象となる集配依頼情報を取得
                var d = serviceSet.deliveryService.read(new Service.DB.DeliveryRequestService.ReadConfig
                {
                    DELIVERY_REQUEST_NUMBER = this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER,
                    TRANSPORT_ADMIN_CODE = this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE
                });
                if (false == d.succeed)
                {
                    throw new ApplicationException("対象となる集配依頼履歴を読み取れませんでした。");
                }
                var delivery = d.result.FirstOrDefault();
                if (null == delivery)
                {
                    throw new ApplicationException("対象となる集配依頼履歴を読み取れませんでした。");
                }

                //集配依頼情報かを変換
                const string KIND = "00100000";
                //DBからドメイン情報を取得する
                var domainInfo = serviceSet.domainService.getValue(KIND, delivery.STATUS);
                if (null == domainInfo)
                {
                    throw new ApplicationException($"ドメイン情報が見つかりません。 KIND : {KIND} , CODE : {delivery.STATUS}");
                }

                if (Service.DB.DeliveryRequestService.toDeliveryStatus(domainInfo.VALUE) == Service.DB.DeliveryRequestService.DeliveryStatus.確定済み)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            #endregion

            if (null == ref_serviceSet)
            {
                ref_serviceSet = makeServiceSet();
            }

            //同一の値を何回も読み込む必要はないので、キャッシュする
            if (false == _confirmedStatus.HasValue)
            {
                _confirmedStatus = readStstus(ref_serviceSet);
            }

            return _confirmedStatus.Value;
        }

        /// <summary>
        /// 画面表示時
        /// </summary>
        /// <param name="DELIVERY_DETAIL_NUMBER"></param>
        public override void OnGet(string DELIVERY_REQUEST_NUMBER, string TRANSPORT_ADMIN_CODE)
        {
            //親クラスで一括して読み取りする
            base.OnGet(DELIVERY_REQUEST_NUMBER, TRANSPORT_ADMIN_CODE);

            //モーダル表示用にデータを準備しておく
            preparationForModalShow(TRANSPORT_ADMIN_CODE,
                DELIVERY_REQUEST_NUMBER,
                "OnModalDecision");
        }

        /// <summary>
        /// 選択されていない要素を確認する
        /// </summary>
        /// <returns></returns>
        private bool hasNoSelectionItems()
        {
            //未入力件数を取得
            var nonInput = this.viewModel
                .tableRows
                .Where(e => null == e.data.selectableCars.selected ||
                String.Empty == e.data.selectableCars.selected
                ).Count();

            if (0 != nonInput)
            {
                //未選択の項目がある
                return true;
            }

            return false;
        }

        /// <summary>
        /// 入力済みデータを保持して復元処理込みで再描画用のデータを準備する
        /// </summary>
        protected override void dataPreparationWithInputDataRestoration(ServiceSet serviceSet = null)
        {

            #region ローカル関数

            //DB上の追跡情報を取得する
            RazorPagesLearning.Service.DB.DeliveryDetailService.ReadConfig LF_idCheck(ServiceSet ref_serviceSet)
            {
                //postされて来た情報から追跡番号を取得する
                var TRANSPORT_ADMIN_CODEs = this.viewModel.tableRows.Select(e => e.data.info.TRANSPORT_ADMIN_CODE).Distinct();
                var DELIVERY_REQUEST_NUMBERs = this.viewModel.tableRows.Select(e => e.data.info.DELIVERY_REQUEST_NUMBER).Distinct();

                //二つのコードに関しては、同じデータレコードが1件存在する。
                //それ以外の場合はエラーとする。
                if (true == ((1 != TRANSPORT_ADMIN_CODEs.Count()) &&
                    (1 != DELIVERY_REQUEST_NUMBERs.Count())))
                {
                    throw new ApplicationException();
                }

                //追跡情報を取得する
                return new RazorPagesLearning.Service.DB.DeliveryDetailService.ReadConfig
                {
                    TRANSPORT_ADMIN_CODE = TRANSPORT_ADMIN_CODEs.First(),
                    DELIVERY_REQUEST_NUMBER = DELIVERY_REQUEST_NUMBERs.First()
                };
            }

            //セレクトボックスで選択された情報からDB上の定義情報を復元する
            IEnumerable<DELIVERY_REQUEST_DETAIL> LF_makeRestored
                (ServiceSet ref_serviceSet, RazorPagesLearning.Service.DB.DeliveryDetailService.ReadConfig ref_ids)
            {
                //トラック情報を読み取る
                var truckInfo = ref_serviceSet.truckAdminService
                     .read(this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE);

                //DBから詳細一覧を取得する
                var dbDETAILs = ref_serviceSet.deliveryDetailService.read(ref_ids);
                if (false == dbDETAILs.succeed)
                {
                    throw new ApplicationException(String.Join("<br>", dbDETAILs.errorMessages));
                }
                var dbList = dbDETAILs.result.ToList();

                //画面上で選択された情報でデータを復元する
                var restored = this.viewModel.tableRows.Select(e =>
                {
                    //該当する詳細情報を取得
                    var delivery = dbList
                    .Where(in_e => in_e.ID == e.data.info.ID)
                    .First();

                    if (null != e.data.selectableCars.selected)
                    {
                        //選択項目でセットされた値を復元
                        var selectedTruck = truckInfo
                         .Where(in_e => in_e.TRUCK_MANAGE_NUMBER == int.Parse(e.data.selectableCars.selected))
                         .FirstOrDefault();
                        if (null != selectedTruck)
                        {
                            delivery.TRUCK_MANAGE_NUMBER = selectedTruck.TRUCK_MANAGE_NUMBER;
                            return delivery;
                        }
                    }

                    //途中で変わってしまった、もとから値がないので値を指定する。                  
                    delivery.TRUCK_MANAGE_NUMBER = -1;

                    return delivery;
                });

                return restored;
            }

            #endregion

            if (null == serviceSet)
            {
                serviceSet = makeServiceSet();
            }

            //追跡番号を取得
            var ids = LF_idCheck(serviceSet);

            //[Memo]
            //
            //ソート処理が絡むのでここの部分の動作は少し異なる。
            //編集中の情報も更新する必要があるため、
            //ソートに関してはDB上では無く、メモリ上で行う。
            //対象データ件数に関しては、それほどの規模がない(数百件程度)ため問題ないものと想定する。
            //
            //1. postされてきた画面上の追跡情報から表示するデータに関してDBから復元をかける。
            //2. 指定されたカラムでソートする。
            //ここでのソート処理はDB内部ではなく、C#上で実施する。

            var dbDatas = LF_makeRestored(serviceSet, ids);

            //該当データの読み込みを行う
            formDisplayableForm(serviceSet, dbDatas);

        }

        /// <summary>
        /// 画面の内容が更新された時
        /// </summary>
        public void OnPost()
        {
            //サービス一覧
            var serviceSet = makeServiceSet();

            //前頁から引継ぎ情報が来ていたら反映させる。
            readAndReflectErrorHandoverData();

            //入力中のデータを復元できる形でデータの取り込みを行う
            dataPreparationWithInputDataRestoration(serviceSet);

            //モーダル表示用にデータを準備しておく
            preparationForModalShow(this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE,
                this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER,
                "OnModalDecision");
        }

        /// <summary>
        /// DBにデータを保存する
        /// </summary>
        /// <returns></returns>
        protected override async Task<DoSaveToDBResult> doSaveToDB()
        {
            #region ローカル関数

            /// <summary>
            /// 選択済みの車情報をDBにview modelに反映させる
            /// </summary>
            void LF_reflectSelectedCar(ServiceSet ref_serviceSet)
            {
                //DBから必要な情報を取得して反映する
                foreach (var ele in this.viewModel.tableRows)
                {
                    //選択済みの配車番号
                    var selected = ele.data.selectableCars.selected;

                    //該当車両の担当者情報を取得する
                    var tInfo = ref_serviceSet.truckAdminService.read(this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE)
                        .Where(e => e.TRUCK_MANAGE_NUMBER == int.Parse(selected))
                        .FirstOrDefault();
                    if (null != tInfo)
                    {
                        //TRUCK_NUMBER、TRUCK_CHARGEは確定状態となるまで設定しない。
                        ele.data.info.TRUCK_MANAGE_NUMBER = tInfo.TRUCK_MANAGE_NUMBER;
                    }
                }
            }

            #endregion

            if (false == hasNoSelectionItems())
            {
                //入力状態に問題がない場合

                //サービス一覧
                var serviceSet = makeServiceSet();

                //selectionの形式をview model形式に変換する
                LF_reflectSelectedCar(serviceSet);

                //データをDBに保存する
                await serviceSet.deliveryDetailService
                    .updateOnlyTRUCKInfo(this.viewModel.tableRows.Select(e => e.data.info));

                await this._db.SaveChangesAsync();

                //DBからデータの再読み込み
                readDataFromDB();

                return DoSaveToDBResult.Success;
            }
            else
            {
                //選択内容に問題がある場合

                //再描画用にデータをリロードする
                this.OnPost();
                this.deliveryCarNumberSetting.viewModel.isShowModal = false;

                this.viewModel.errorMessage = "車番が未選択の項目があります。";

                //入力状態に問題があるのでエラー表示する
                return DoSaveToDBResult.Failure;
            }
        }

        /// <summary>
        /// データを保存する
        /// </summary>
        public async Task<IActionResult> OnPostSave()
        {
            //モーダル画面を表示するか
            this.deliveryCarNumberSetting.viewModel.isShowModal = false;

            var r = await doSaveToDB();
            switch (r)
            {
                case DoSaveToDBResult.Success:
                    {
                        return RedirectToPagePermanentPreserveMethod("DeliveryCar");
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
        /// ルート入力画面に移動する
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostMoveToRouteEdit()
        {
            if (false == hasNoSelectionItems())
            {
                //車番設定があるので、車番設定画面へ飛ばす
                return RedirectToPage("./DeliveryRoute",
                    new
                    {
                        DELIVERY_REQUEST_NUMBER = this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER,
                        TRANSPORT_ADMIN_CODE = this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE
                    });
            }
            else
            {
                //選択内容に問題がある場合

                //再描画用にデータをリロードする
                this.OnPost();
                this.deliveryCarNumberSetting.viewModel.isShowModal = false;

                this.viewModel.errorMessage = "車番が未選択の項目があります。";

                //入力状態に問題があるのでエラー表示する
                return Page();
            }
        }

        ///// <summary>
        ///// エクスポート対象のファイルをダウンロードする
        ///// </summary>
        ///// <param name="DELIVERY_REQUEST_NUMBER"></param>
        ///// <param name="TRANSPORT_ADMIN_CODE"></param>
        //public IActionResult OnPostExportedDiles()
        //{
        //    //モーダル画面を表示するか
        //    this.deliveryCarNumberSetting.viewModel.isShowModal = false;

        //    //対象サービス
        //    var serviceSet = makeServiceSet();

        //    //対象情報を生成
        //    var cfg = makeDeliveryReportConfig(serviceSet,
        //        this.viewModel.filteringConditions.DELIVERY_REQUEST_NUMBER,
        //        this.viewModel.filteringConditions.TRANSPORT_ADMIN_CODE);
        //    if (true == cfg.isSucceed)
        //    {
        //        prepareDownloadExportFile(cfg.deliveryReportConfig, ExportFileOutputType.Both);
        //        return Page();
        //    }
        //    else
        //    {
        //        //エラー情報をセットする
        //        setErrorHandoverData();
        //        return RedirectToPagePermanentPreserveMethod();
        //    }

        //}
    }
}