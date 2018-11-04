using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesLearning.Data.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using RazorPagesLearning.Service.Utility.ViewHelper;

namespace RazorPagesLearning.Pages
{
    public class JsonModel : PageModel
    {
        #region 画面上に表示すべき情報
        /// <summary>
        /// 画面上に表示すべき情報
        /// </summary>
        public class ViewModel
        {
            /// <summary>
            /// 会社名
            /// </summary>
            public string company { get; set; }

            /// <summary>
            /// AND／OR
            /// </summary>
            public int condition { get; set; }

            /// <summary>
            /// 全件表示
            /// </summary>
            public bool dispCond { get; set; }

            /// <summary>
            /// 表示件数
            /// </summary>
            public int viewCount { get; set; }

            /// <summary>
            /// ページ番号
            /// </summary>
            public int pageNumber { get; set; }

            /// <summary>
            /// オーダーキー
            /// </summary>
            public int orderKey { get; set; }

            /// <summary>
            /// 昇順／降順
            /// </summary>
            public int order { get; set; }

        }
        #endregion

        #region 集配先選択で返却する情報
        public class ResSelectDeliveryViewModel
        {
            /// <summary>
            /// 全件
            /// </summary>
            public int count { get; set; }

            /// <summary>
            /// ページ内のアイテム数
            /// </summary>
            public int itemCount { get; set; }

            /// <summary>
            /// テーブル表示HTML
            /// </summary>
            public string tableStr { get; set; }

        }
        #endregion

        #region 在庫詳細照会（ステータス）で返却する情報
        public class ResStockDetailViewModel
        {
            public STOCK stock { get; set; }
            public USER_ITEM user_item { get; set; }
            public REQUEST_HISTORY_DETAIL requestHistoryDetail { get; set; }
        }
        #endregion

        #region 履歴詳細照会（入出庫履歴）で返却する情報
		/// <summary>
		/// 履歴詳細照会（入出庫履歴）で返却する情報
		/// </summary>
        public class ResStockHistoryViewModel
        {
			/// <summary>
			/// 倉庫管理番号
			/// </summary>
			public string storageManageNumber { get; set; }

			/// <summary>
			/// お客様管理番号
			/// </summary>
			public string customerManageNumber { get; set; }

			/// <summary>
			/// 題名
			/// </summary>
			public string title { get; set; }

			/// <summary>
			/// 副題
			/// </summary>
			public string subtitle { get; set; }

			/// <summary>
			/// Remark1
			/// </summary>
			public string remark1 { get; set; }

			/// <summary>
			/// Remark2
			/// </summary>
			public string remark2 { get; set; }

			/// <summary>
			/// 形状
			/// </summary>
			public string shape { get; set; }

			/// <summary>
			/// 時間
			/// </summary>
			public string time { get; set; }

			/// <summary>
			/// 廃棄予定日
			/// </summary>
			public string scrapScheduleDate { get; set; }

			/// <summary>
			/// 入庫日
			/// </summary>
			public string storageDate { get; set; }

			/// <summary>
			/// 制作日
			/// </summary>
			public string productDate { get; set; }

			/// <summary>
			/// 一覧表示のHTML
			/// </summary>
            public string tableStr { get; set; }
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

        // DB
        private Service.DB.DeliveryAdminService deliveryAdminService;
        private Service.DB.StockService stockService;
        private Service.DB.RequestHistoryDetailService requestHistoryAdminService;
        //private Service.DB.WmsRequestHistoryDetailService wmsRequestHistoryAdminService;

        // コンストラクタ
        public JsonModel(RazorPagesLearning.Data.RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager,
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
            this.resSelectDeliveryViewModel = new ResSelectDeliveryViewModel();
            this.resStockDetailViewModel = new ResStockDetailViewModel();
            this.resStockHistoryViewModel = new ResStockHistoryViewModel();
        }

        /// <summary>
        /// 画面上の表示項目と同期する情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        public ResSelectDeliveryViewModel resSelectDeliveryViewModel { get; set; }
        public ResStockDetailViewModel resStockDetailViewModel { get; set; }
        public ResStockHistoryViewModel resStockHistoryViewModel { get; set; }

        /// <summary>
        /// お試し通信 TODO あとで消すこと
        /// </summary>
        /// <returns></returns>
        public ActionResult OnGet()
        {
            //string json = "{\"status\":\"OK\"}";

            WK_REQUEST_DELIVERY wkDelivery = new WK_REQUEST_DELIVERY();

            this.resSelectDeliveryViewModel.tableStr = "<tr><td style = 'text-align: center;'><a style='color:white;' name='select'>選択</ a></ td><td></td><td></td><td></td><td></td><td></td><td></td></tr>";
            //this.viewModel.table = "<tr>< td >< a > 選択 </ a ></ td >< td ></ td >< td ></ td >< td ></ td >< td ></ td >< td ></ td >< td ></ td ></ tr >";

            var json = JsonConvert.SerializeObject(this.resSelectDeliveryViewModel);

            //return
            return Content(json, "application/json");

        }

        /// <summary>
        /// 集配先選択の検索通信用
        /// </summary>
        /// <param name="companyName"></param>
        /// <param name="andCondition"></param>
        /// <param name="dispCondition"></param>
        /// <param name="viewCount"></param>
        /// <param name="pageNumber"></param>
        /// <param name="orderKey"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ActionResult OnGetSelectDelivery(string companyName, int andCondition, int dispCondition,
                int viewCount, int pageNumber, int orderKey, int order)
        {
            this.deliveryAdminService = new Service.DB.DeliveryAdminService(this._db, User, this._signInManager, this._userManager);

            //// 送信されてきた条件でデータ検索
            //Service.DB.DeliveryAdminService.ReadConfig readConfig = new Service.DB.DeliveryAdminService.ReadConfig();
            //readConfig.companyName = companyName;
            //readConfig.andCondtion = andCondition;
            //readConfig.dispCondition = dispCondition;

            //this.resSelectDeliveryViewModel.count = this.deliveryAdminService.count(readConfig);

            //readConfig.viewCount = viewCount;
            //readConfig.pageNumber = pageNumber;
            //readConfig.orderKey = orderKey;
            //readConfig.order = order;


            //List<DELIVERY_ADMIN> deliveryAdmins = this.deliveryAdminService.read(readConfig).result;

            //string tableTags = "";
            //this.resSelectDeliveryViewModel.itemCount = 0;
            //foreach (var deliveryAdmin in deliveryAdmins)
            //{
            //    tableTags += string.Format("<tr><td style = 'text-align: center;'><a style='color:white;' name='select'>選択</a></td><td name='delivery_code'>{0}</td><td name='company'>{1}</td><td name='department'>{2}</td><td name='charge'>{3}</td><td name='address'>{4}</td><td name='tel'>{5}</td><input type='hidden' name='zipcode' value='{6}'><input type='hidden' name='mwlDeliveryId' value='{7}'></tr>",
            //        deliveryAdmin.DELIVERY_CODE, deliveryAdmin.COMPANY, deliveryAdmin.DEPARTMENT, deliveryAdmin.CHARGE_NAME, (deliveryAdmin.ADDRESS1 + deliveryAdmin.ADDRESS2), deliveryAdmin.TEL, deliveryAdmin.ZIPCODE, deliveryAdmin.DELIVERY_ADMIN_ID);
            //    this.resSelectDeliveryViewModel.itemCount++;
            //}


            //this.resSelectDeliveryViewModel.tableStr = tableTags;
            //var json = JsonConvert.SerializeObject(this.resSelectDeliveryViewModel);

            //return Content(json, "application/json");



            // 送信されてきた条件でデータ検索
            Service.DB.DeliveryAdminService.ReadConfigBase readConfig = new Service.DB.DeliveryAdminService.ReadConfigBase();
            readConfig.company = companyName;
            readConfig.company_AndOr = (Service.DB.DeliveryAdminService.ReadConfigBase.AndOr)andCondition;
            readConfig.displayFlag = (Service.DB.DeliveryAdminService.ReadConfigBase.DisplayFlag)dispCondition;
            readConfig.userAccountId = 1;

            //this.resSelectDeliveryViewModel.count = this.deliveryAdminService.count(readConfig);

            //readConfig.viewCount = viewCount;
            //readConfig.pageNumber = pageNumber;
            //readConfig.orderKey = orderKey;
            //readConfig.order = order;


            //List<DELIVERY_ADMIN> deliveryAdmins = this.deliveryAdminService.read(readConfig).result;
            IQueryable<Service.DB.DeliveryAdminService.DeliveryAdminSearchResult> deliveryAdmins = this.deliveryAdminService.read(readConfig, true);

            string tableTags = "";
            this.resSelectDeliveryViewModel.itemCount = 0;
            foreach (var deliveryAdmin in deliveryAdmins)
            {
                tableTags += string.Format("<tr><td style = 'text-align: center;'><a style='color:white;' name='select'>選択</a></td><td name='delivery_code'>{0}</td><td name='company'>{1}</td><td name='department'>{2}</td><td name='charge'>{3}</td><td name='address'>{4}</td><td name='tel'>{5}</td><input type='hidden' name='mwlDeliveryId' value='{6}'></tr>",
                    deliveryAdmin.DELIVERY_CODE, deliveryAdmin.COMPANY, deliveryAdmin.DEPARTMENT, deliveryAdmin.CHARGE_NAME, (deliveryAdmin.ADDRESS1 + deliveryAdmin.ADDRESS2), deliveryAdmin.TEL, deliveryAdmin.DELIVERY_ADMIN_ID);
                this.resSelectDeliveryViewModel.itemCount++;
            }


            this.resSelectDeliveryViewModel.tableStr = tableTags;
            var json = JsonConvert.SerializeObject(this.resSelectDeliveryViewModel);

            return Content(json, "application/json");

        }

        /// <summary>
        /// 在庫詳細照会（ステータス）のデータ取得通信用
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public ActionResult OnGetStockDetail(long stockId)
        {
            this.stockService = new Service.DB.StockService(this._db, User, this._signInManager, this._userManager);
            this.requestHistoryAdminService = new Service.DB.RequestHistoryDetailService(this._db, User, this._signInManager, this._userManager);

            var stockInfo = this.stockService.readWithUserItemByStockId(stockId);

            this.resStockDetailViewModel.stock = stockInfo.Item1;
            this.resStockDetailViewModel.user_item = stockInfo.Item2;
            //循環参照しているとjson化できないのでnullでつぶす
            if (null != this.resStockDetailViewModel.user_item)
            {
                this.resStockDetailViewModel.user_item.USER_ACCOUNT = null;
                this.resStockDetailViewModel.user_item.STOCK = null;
            }

            var json = JsonConvert.SerializeObject(this.resStockDetailViewModel);

            return Content(json, "application/json");

        }

        /// <summary>
        /// 在庫詳細照会（ステータス）の更新用
        /// </summary>
        /// <param name="stockId"></param>
        /// <param name="customerItem1Code"></param>
        /// <param name="customerItem1Value"></param>
        /// <param name="customerItem2Code"></param>
        /// <param name="customerItem2Value"></param>
        /// <param name="customerItem3Code"></param>
        /// <param name="customerItem3Value"></param>
        /// <param name="projectNo1"></param>
        /// <param name="projectNo2"></param>
        /// <param name="copyright1"></param>
        /// <param name="copyright2"></param>
        /// <param name="contract1"></param>
        /// <param name="contract2"></param>
        /// <param name="dataNo1"></param>
        /// <param name="dataNo2"></param>
        /// <param name="processJudge1"></param>
        /// <param name="processJudge2"></param>
        /// <returns></returns>
        public async Task<ActionResult> OnGetStockDetailUpdate(long stockId, string customerItem1Code, string customerItem1Value, string customerItem2Code, string customerItem2Value, string customerItem3Code, string customerItem3Value,
            string projectNo1, string projectNo2, string copyright1, string copyright2, string contract1, string contract2, string dataNo1, string dataNo2, string processJudge1, string processJudge2)
        {
            this.stockService = new Service.DB.StockService(this._db, User, this._signInManager, this._userManager);


            // TODO AjaxでのPOST通信がうまくできないため、GETでまずは対応する
            STOCK stock = new STOCK();
            stock.ID = stockId;

            var ui = await stockService.readLoggedUserInfo();

            //ユーザー項目を追加する
            stock.USER_ITEMs = new List<USER_ITEM>()
            {
                new USER_ITEM
                {
                    USER_ACCOUNT_ID = ui.ID,
                    STOCK_ID = stockId,
                    ITEM1_CODE = customerItem1Code,
                    ITEM1_VALUE = customerItem1Value,
                    ITEM2_CODE = customerItem2Code,
                    ITEM2_VALUE = customerItem2Value,
                    ITEM3_CODE = customerItem3Code,
                    ITEM3_VALUE = customerItem3Value
                }
            };

            stock.PROJECT_NO1 = projectNo1;
            stock.PROJECT_NO2 = projectNo2;
            stock.COPYRIGHT1 = copyright1;
            stock.COPYRIGHT2 = copyright2;
            stock.CONTRACT1 = contract1;
            stock.CONTRACT2 = contract2;
            stock.DATA_NO1 = dataNo1;
            stock.DATA_NO2 = dataNo2;
            stock.PROCESS_JUDGE1 = processJudge1;
            stock.PROCESS_JUDGE2 = processJudge2;

            await this.stockService.add(stock);

            var json = "1";
            return Content(json, "application/json");
        }
        /// <summary>
        /// 在庫詳細照会（ステータス）のデータ取得通信用
        /// </summary>
        /// <returns></returns>
        public ActionResult OnPostStockDetailUpdate(long stockId)
        {
            //this.stockService = new Service.DB.StockService(this._db, User, this._signInManager, this._userManager);
            //this.requestHistoryAdminService = new Service.DB.RequestHistoryDetailService(this._db, User, this._signInManager, this._userManager);

            //this.resStockDetailViewModel.stock = this.stockService.readByStockId(stockId);
            //this.resStockDetailViewModel.requestHistoryDetail = this.requestHistoryAdminService.readByStockId(stockId);

            //var json = JsonConvert.SerializeObject(this.resStockDetailViewModel);

            var json = "1";
            return Content(json, "application/json");

        }

		/// <summary>
		/// 履歴詳細照会（入出庫履歴）のデータ取得通信用
		/// </summary>
		/// <param name="stockId"></param>
		/// <param name="orderKey"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		public async Task<ActionResult> OngetStockHistory(Int64 stockId, int orderKey, int order)
        {
			// WMS入出庫実績を取得
			// TODO: ソートする
			var wmsResultHistoryService = new Service.DB.WmsRequestHistoryDetailService(this._db, User, this._signInManager, this._userManager);
			var wmsResultHistories = await wmsResultHistoryService.read(stockId);

			// DOMAINを取得
			var domainService = new Service.DB.DomainService(this._db, User, this._signInManager, this._userManager);
			var domains = domainService.getValueList(DOMAIN.Kind.STCOK_STATUS).ToList();

			string tableTags = "";
			var first = wmsResultHistories.FirstOrDefault();
			if (null != first)
			{
				// サマリ部分を、1件目から取得
				this.resStockHistoryViewModel.storageManageNumber = first.STORAGE_MANAGE_NUMBER;
				this.resStockHistoryViewModel.customerManageNumber = first.CUSTOMER_MANAGE_NUMBER;
				this.resStockHistoryViewModel.title = first.TITLE;
				this.resStockHistoryViewModel.subtitle = first.SUBTITLE;
				this.resStockHistoryViewModel.remark1 = first.REMARK1;
				this.resStockHistoryViewModel.remark2 = first.REMARK2;
				this.resStockHistoryViewModel.shape = first.SHAPE;
				this.resStockHistoryViewModel.time = first.TIME1 + first.TIME2;
				this.resStockHistoryViewModel.scrapScheduleDate = HelperFunctions.toFormattedString(first.SCRAP_SCHEDULE_DATE, "yyyy/MM/dd");
				this.resStockHistoryViewModel.storageDate = HelperFunctions.toFormattedString(first.STORAGE_DATE, "yyyy/MM/dd");
				this.resStockHistoryViewModel.productDate = first.PRODUCT_DATE;

				// テーブル部分を組み立てる
				foreach (var item in wmsResultHistories)
				{
					tableTags +=
						$"<tr>" +
						$"  <td name='processingDate'>{HelperFunctions.toFormattedString(item.PROCESSING_DATE, "yyyy/MM/dd")}</td>" +
						$"  <td name='slipNumber'>{item.SLIP_NUMBER}</td>" +
						$"  <td name='orderNumber'>{item.ORDER_NUMBER}</td>" +
						$"  <td name='status'>{HelperFunctions.toDomainValue(domains, item.STATUS)}</td>" +
						$"  <td name='storageRetrievalDate'>{HelperFunctions.toFormattedString(item.STORAGE_RETRIEVAL_DATE, "yyyy/MM/dd")}</td>" +
						$"  <td name='arrivalTime'>{item.ARRIVAL_TIME}</td>" +
						$"  <td name='shipCompany'>{item.SHIP_COMPANY}</td>" +
						$"  <td name='requestCount'>{item.REQUEST_COUNT}</td>" +
						$"  <td name='confirmCount'>{item.CONFIRM_COUNT}</td>" +
						$"</tr>";
				}
			}

			this.resStockHistoryViewModel.tableStr = tableTags;

            var json = JsonConvert.SerializeObject(this.resStockHistoryViewModel);

            return Content(json, "application/json");
        }
    }
}