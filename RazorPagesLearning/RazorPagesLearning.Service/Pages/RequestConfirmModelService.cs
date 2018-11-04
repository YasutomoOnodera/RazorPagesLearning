using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static RazorPagesLearning.Data.Models.DOMAIN;

/// <summary>
/// 作業依頼確定処理用のサービス
/// </summary>
namespace RazorPagesLearning.Service.Pages.RequestConfirmModel
{
    /// <summary>
    /// 使用するサービス一覧
    /// </summary>
    public class RequestConfirmServiceSet
    {
        #region 内部サービス一覧
        /// <summary>
        /// 
        /// </summary>
        public readonly Service.User.UserService userService;
        public readonly Service.DB.DomainService domainService;
        public readonly Service.DB.WkDeliveryService wkDeliveryService;
        public readonly Service.DB.WkRequestDetailService wkRequestDetailService;
        public readonly Service.DB.RequestHistoryService requestHistoryService;
        public readonly Service.DB.RequestHistoryDetailService requestHistoryDetailService;
        public readonly Service.DB.WkRequestService wkRequestService;
        public readonly Service.DB.SystemSettingService systemSettingService;
        public readonly Service.DB.MailTemplateService mailTemplateService;
        public readonly Service.DB.CalendarAdminService calendarAdminService;
        public readonly Service.DB.StockService stockService;
        public readonly RazorPagesLearning.Data.RazorPagesLearningContext db;
        #endregion

        public RequestConfirmServiceSet(RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
        SignInManager<IdentityUser> ref_signInManager,
        UserManager<IdentityUser> ref_userManager)
        {
            this.db = ref_db;

            this.userService = new Service.User.UserService
                (ref_db,
                ref_user,
                ref_signInManager,
                ref_userManager);

            this.domainService = new Service.DB.DomainService
                (ref_db,
                ref_user,
                ref_signInManager,
                ref_userManager);

            this.wkDeliveryService = new Service.DB.WkDeliveryService
                (ref_db,
                ref_user,
                ref_signInManager,
                ref_userManager);

            this.wkRequestDetailService = new Service.DB.WkRequestDetailService
                (ref_db,
                ref_user,
                ref_signInManager,
                ref_userManager);

            this.requestHistoryService = new Service.DB.RequestHistoryService
                (ref_db,
                ref_user,
                ref_signInManager,
                ref_userManager);

            this.requestHistoryDetailService = new Service.DB.RequestHistoryDetailService
                (ref_db,
                ref_user,
                ref_signInManager,
                ref_userManager);

            this.wkRequestService = new Service.DB.WkRequestService
                (ref_db,
                ref_user,
                ref_signInManager,
                ref_userManager);

            this.systemSettingService = new Service.DB.SystemSettingService
                (ref_db,
                ref_user,
                ref_signInManager,
                ref_userManager);

            this.mailTemplateService = new Service.DB.MailTemplateService
                (ref_db,
                ref_user,
                ref_signInManager,
                ref_userManager);

            this.calendarAdminService = new Service.DB.CalendarAdminService
                (ref_db,
                ref_user,
                ref_signInManager,
                ref_userManager);

            this.stockService = new Service.DB.StockService
                (ref_db,
                ref_user,
                ref_signInManager,
                ref_userManager);
        }
    }

    /// <summary>
    /// 作業依頼確定処理を実行する処理クラス
    /// </summary>
    public class RequestConfirmationProcessingHelper
    {
        /// <summary>
        /// サービスセット
        /// </summary>
        private RequestConfirmServiceSet serviceSet;

        /// <summary>
        /// 依頼ステータスを見て、依頼後の在庫ステータスを返す
        /// </summary>
        /// <param name="requestCode"></param>
        /// <returns></returns>
        private string decideStockStatusFromRequestStatus(string requestCode)
        {
            switch (requestCode)
            {
                // 新規入庫(登録ユーザ)、新規入庫
                // →集荷予定
                case DOMAIN.RequestRequestCode.NEW_USER:
                case DOMAIN.RequestRequestCode.NEW:
                    return DOMAIN.StockStatusCode.RESERVE_CARGO;

                // 出荷
                // →出荷予約
                case DOMAIN.RequestRequestCode.SHIPPING:
                    return DOMAIN.StockStatusCode.RESERVE_SHIPPING;

                // 再入庫
                // →再入庫予定
                case DOMAIN.RequestRequestCode.RESTOCK:
                    return DOMAIN.StockStatusCode.RESERVE_RESTOCK;

                // 廃棄
                // →廃棄予定
                case DOMAIN.RequestRequestCode.SCRAP:
                    return DOMAIN.StockStatusCode.RESERVE_SCRAP;

                // 抹消(永久出庫)、抹消(データ抹消)
                // →抹消予定
                case DOMAIN.RequestRequestCode.PERIPHERY:
                case DOMAIN.RequestRequestCode.PERIPHERY_DATA:
                    return DOMAIN.StockStatusCode.RESERVE_PERIPHERY;

                // その他
                case DOMAIN.RequestRequestCode.MATERIAL:
                default:
                    throw new ApplicationException("依頼内容が不正です。");
            }
        }

        /// <summary>
        /// 確定処理結果をDBに帆損する
        /// </summary>
        /// <param name="targetWK_REQUEST_DETAIL"></param>
        /// <param name="config"></param>
        /// <param name="isNew">新規入庫の場合true</param>
        private void saveData
            (WK_REQUEST_DETAIL targetWK_REQUEST_DETAIL,
            ConfirmConfig config,
            bool isNew)
        {
            //データを更新する
            this.serviceSet.stockService.saveRequestConfirm
               (targetWK_REQUEST_DETAIL.STOCK_ID,
                (up) =>
                {
                    //DBカラムを更新
                    up.ID = targetWK_REQUEST_DETAIL.STOCK_ID;
                    up.PROCESSING_DATE = DateTimeOffset.Now;
                    up.STATUS =
                    decideStockStatusFromRequestStatus(targetWK_REQUEST_DETAIL.WK_REQUEST.REQUEST_KIND);
                    up.ORDER_NUMBER = config.request_history.ORDER_NUMBER;
                    up.SHIP_RETURN_CODE = config.wk_request_delivery.DELIVERY_ADMIN.DELIVERY_CODE;
                    up.SHIP_RETURN_COMPANY = config.wk_request_delivery.DELIVERY_ADMIN.COMPANY;
                    up.SHIP_RETURN_DEPARTMENT = config.wk_request_delivery.DELIVERY_ADMIN.DEPARTMENT;
                    up.SHIP_RETURN_CHARGE_NAME = config.wk_request_delivery.DELIVERY_ADMIN.CHARGE_NAME;
                    up.SCRAP_SCHEDULE_DATE = config.wk_request_delivery.SPECIFIED_DATE;
                    up.ARRIVAL_TIME = config.wk_request_delivery.SPECIFIED_TIME;

                    //新しい在庫数は依頼数を減じて処理する
                    {
                        var orgSTOCK_COUNT = up.STOCK_COUNT;
                        up.STOCK_COUNT = up.STOCK_COUNT - up.WK_REQUEST_DETAILs.Sum(e => e.REQUEST_COUNT);
                        if (0 > up.STOCK_COUNT)
                        {
                            throw new ApplicationException
                            ($"在庫数が正常値域を外れています。在庫数 : {orgSTOCK_COUNT} , 依頼数{up.WK_REQUEST_DETAILs.Sum(e => e.REQUEST_COUNT)}");
                        }
                    }

                    //新規登録の場合、入庫日を設定する
                    if (true == isNew)
                    {
                        up.STORAGE_DATE = config.request_history.REQUEST_DATE;
                    }
                });
        }

        /// <summary>
        /// 確定設定
        /// </summary>
        public class ConfirmConfig
        {
            //対象となる作業依頼詳細ワーク
            public WK_REQUEST_DELIVERY wk_request_delivery;

            /// <summary>
            /// 作業依頼履歴情報
            /// </summary>
            public REQUEST_HISTORY request_history;
        }

        /// <summary>
        /// 確定
        /// </summary>
        public void confirm(ConfirmConfig confirmConfig)
        {
            //対象の在庫一覧を作成する
            var wk_request_details = confirmConfig
                .wk_request_delivery
                .WK_REQUEST
                .WK_REQUEST_DETAILs;

            //関係する在庫毎に判定を行う
            foreach (var ele in wk_request_details)
            {
                switch (ele.STOCK.STATUS)
                {
                    case StockStatusCode.REGIST_WAITING:
                        {
                            //登録待ち状態の確定更新処理を行う
                            //[対象ステータス]
                            //登録待
                            saveData(ele, confirmConfig, true);
                            break;
                        }
                    case StockStatusCode.STOCK:
                    case StockStatusCode.SHIPPING:
                    case StockStatusCode.MULTIPLE:
                        {
                            //登録済み更新処理
                            //[対象ステータス]
                            //在庫中
                            //出荷中
                            //複数品
                            saveData(ele, confirmConfig, false);
                            break;
                        }
                    case StockStatusCode.MATERIAL:
                        {
                            //資材

                            //ToDo: 資材の処理を行う。詳細が決まり次第実装する。
                            throw new NotImplementedException("資材処理の詳細未確定。");
                        }
                    default:
                        {
                            //ここに来るパターンはおかしい
                            throw new ApplicationException
                                ($"作業依頼確定処理においてステータス{ele.STOCK.STATUS}は想定外のステータスです。");
                        }
                }
            }

            //更新処理を確定
            this.serviceSet.db.SaveChanges();
        }

        public RequestConfirmationProcessingHelper(RequestConfirmServiceSet ref_serviceSet)
        {
            this.serviceSet = ref_serviceSet;
        }
    }

}
