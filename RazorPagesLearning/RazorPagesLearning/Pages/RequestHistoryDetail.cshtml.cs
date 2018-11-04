using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
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
//using static RazorPagesLearning.Report.RequestHistoryReport;
//using static RazorPagesLearning.Report.RequestHistoryReportExcel;
using static RazorPagesLearning.Service.User.UserService;

namespace RazorPagesLearning.Pages
{
	////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 作業依頼履歴(詳細)ページモデル
	/// 
	/// [アクセス制限]
	/// ・運送会社だけ入れない
	/// 
	/// </summary>
	////////////////////////////////////////////////////////////////////////////////////////////////
	[Authorize(Roles = "Admin,ShipperBrowsing,ShipperEditing,Worker", Policy = "PasswordExpiration")]
    public class RequestHistoryDetailModel : PageModel
    {
		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="ref_db"></param>
		/// <param name="ref_signInManager"></param>
		/// <param name="ref_userManager"></param>
		/// <param name="ref_logger"></param>
		////////////////////////////////////////////////////////////////////////////////////////////
		public RequestHistoryDetailModel(
			RazorPagesLearning.Data.RazorPagesLearningContext db,
			SignInManager<IdentityUser> signInManager,
			UserManager<IdentityUser> userManager,
			ILogger<LoginModel> logger)
		{
			// 各種サービス
			this.userService = new Service.User.UserService(db, User, signInManager, userManager);
			this.domainService = new Service.DB.DomainService(db, User, signInManager, userManager);
			this.requestHistoryService = new Service.DB.RequestHistoryService(db, User, signInManager, userManager);
			this.requestHistoryDetailService = new Service.DB.RequestHistoryDetailService(db, User, signInManager, userManager);
			this.requestListService = new Service.DB.RequestListService(db, User, signInManager, userManager);

            // コンストラクタ引数
            this._db = db;
			this._signInManager = signInManager;
			this._userManager = userManager;
			this._logger = logger;

			// 画面に表示する項目
			this.paginationInfo = initPaginationInfo();
			this.searchConditions = new SearchConditions(0);
			//this.searchConditions = new SearchConditions();
			this.viewModel = new ViewModel();

			// DOMAINのコードを事前取得
			this.viewModel.requestKind = domainService.getValueList(DOMAIN.Kind.REQUEST_REQUEST).ToList();
			this.viewModel.wmsStatus = domainService.getValueList(DOMAIN.Kind.WMS_).ToList();
			this.viewModel.flight = domainService.getValueList(DOMAIN.Kind.DELIVERY_FLIGHT).ToList();
			this.viewModel.class1 = domainService.getValueList(DOMAIN.Kind.STOCK_CLASS1).ToList();
			this.viewModel.class2 = domainService.getValueList(DOMAIN.Kind.STOCK_CLASS2).ToList();
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 画面上に表示する項目
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public class ViewModel : SelectableTableViewModelBase<ViewREQUEST_HISTORY_DETAIL>
		{
			////////////////////////////////////////////////////////////////////////////////////////
			/// <summary>
			/// コンストラクタ
			/// </summary>
			////////////////////////////////////////////////////////////////////////////////////////
			public ViewModel() :
				base(ViewTableType.Search)
			{
			}

			/// <summary>
			/// 作業依頼履歴
			/// </summary>
			public RazorPagesLearning.Data.Models.REQUEST_HISTORY requestHistory { get; set; }

			/// <summary>
			/// 依頼内容：DOMAIN(KIND=00020001)
			/// </summary>
			public List<DOMAIN> requestKind;

			/// <summary>
			/// WMS状態：DOMAIN(KIND=00090000)のコード一覧
			/// </summary>
			public List<DOMAIN> wmsStatus;

			/// <summary>
			/// 便：DOMAIN(KIND=00080001)のコード一覧
			/// </summary>
			public List<DOMAIN> flight;

			/// <summary>
			/// 区分1：DOMAIN(KIND=00010002)のコード一覧
			/// </summary>
			public List<DOMAIN> class1;

			/// <summary>
			/// 区分２：DOMAIN(KIND=00010003)のコード一覧
			/// </summary>
			public List<DOMAIN> class2;

			/// <summary>
			/// MWL在庫ID
			/// </summary>
			public List<Int64> mwlStockIds { get; set; }

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

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 画面に表示する作業依頼履歴詳細一覧
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public class ViewREQUEST_HISTORY_DETAIL
		{
			/// <summary>
			/// 表示するデータ
			/// </summary>
			public RazorPagesLearning.Data.Models.REQUEST_HISTORY_DETAIL data { get; set; }
		}

		#region ページネーション情報

		/// <summary>
		/// ページネーション情報を初期化する
		/// </summary>
		/// <returns></returns>
		private PaginationInfo initPaginationInfo()
        {
			//ToDo : セレクト結果で表示する
			return PaginationInfo.createInstance(1);
			//return PaginationInfo.createInstance(
			//			this.userService.getStatistics().numbers);
		}

        /// <summary>
        /// ページネーション情報
        /// </summary>
        [BindProperty]
        public PaginationInfo paginationInfo { get; set; }

		#endregion // ページネーション情報


		#region 検索条件

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件定義
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public class SearchConditions
        {
			////////////////////////////////////////////////////////////////////////////////////////
			/// <summary>
			/// コンストラクタ
			/// </summary>
			////////////////////////////////////////////////////////////////////////////////////////
			public SearchConditions()
			{
				//this.queryConditions = new RequestHistoryDetailService.ReadConfig();
			}

			////////////////////////////////////////////////////////////////////////////////////////
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="id"></param>
			////////////////////////////////////////////////////////////////////////////////////////
			public SearchConditions(int id)
			{
				queryConditions = new RequestHistoryDetailService.ReadConfig(id);
				queryConditions.start = 0;
				queryConditions.take = 100;
			}

			public RazorPagesLearning.Service.DB.RequestHistoryDetailService.ReadConfig queryConditions { get; set; }
        }

        /// <summary>
        /// 検索条件
        /// </summary>
        [BindProperty]
        public SearchConditions searchConditions { get; set; }

        #endregion // 検索条件


        /// <summary>
        /// テーブル上の行情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        /// <summary>
        /// DBアクセスオブジェクト
        /// </summary>
        public readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;

        // コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ILogger<LoginModel> _logger;

		// 各種サービス
		private readonly Service.User.UserService userService;
		private readonly Service.DB.DomainService domainService;
        private readonly Service.DB.RequestHistoryService requestHistoryService;
        private readonly Service.DB.RequestHistoryDetailService requestHistoryDetailService;
		private readonly Service.DB.RequestListService requestListService;


		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// サービス情報を更新する
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		private void updateService()
		{
			// PageModelのコンストラクタでは、ユーザーサービス情報は引き渡されない。
			// 実際にこれらの情報に触れるようになるのは、アクションメソッドが呼ばれたタイミングである。
			// このため、アクションメソッドが呼ばれたタイミングでユーザー情報を更新する
			this.userService.updateUser(User);
			this.domainService.updateUser(User);
			this.requestHistoryService.updateUser(User);
			this.requestHistoryDetailService.updateUser(User);
			this.requestListService.updateUser(User);
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// indexアクション
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnGetAsync(int id, int? start, int? take)
        {
            await getStockDetailInfo();

            // =====================================================================================
            // 概要を検索
            // =====================================================================================
            this.viewModel.requestHistory = this.requestHistoryService.readById(id).result;

			// =====================================================================================
			// 詳細を検索
			// =====================================================================================
			// 検索条件を作る
			this.searchConditions.queryConditions.id = id;
			this.searchConditions.queryConditions.start = (null != start) ? (int)start : 0;
			this.searchConditions.queryConditions.take = (null != take) ? (int)take : 20;

			// 検索
			this.viewModel.tableRows =
				requestHistoryDetailService.read(this.searchConditions.queryConditions)
				.Select(e => new RowInfo<ViewREQUEST_HISTORY_DETAIL>
				{
					data = new ViewREQUEST_HISTORY_DETAIL
					{
						data = e,
					}
				}).ToList();

			// 作業依頼追加のキーにするため、MWL在庫IDのリストを保持
			this.viewModel.mwlStockIds = this.viewModel.tableRows
				.Select(e => e.data.data.STOCK_ID).ToList();

            return Page();
        }

		//////////////////////////////////////////////////////////////////////////////////////////////
		///// <summary>
		///// 印刷(PDF)
		///// </summary>
		///// <returns></returns>
		//////////////////////////////////////////////////////////////////////////////////////////////
		//public void OnGetPdf(int id)
  //      {

  //          //レポートをエクスポートする
  //          var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
  //                  DateTimeOffset.Now, "yyyyMMddHHmmss");

  //          this.searchConditions.queryConditions.id = id;
  //          Utility.ReportHelper.HelperFunctions.writePdf
  //              (new Utility.ReportHelper.HelperFunctions.WriteConfig
  //              {
  //                  fileName = $"RequestHistoryDetail_{nowStr}.pdf",
  //                  report = new RazorPagesLearning.Report.RequestHistoryReport(
  //                      new ReportConfig
  //                      {
  //                          targetRequestHistory = this.requestHistoryService.readById(id).result,
  //                          targetInformation = this.requestHistoryDetailService.readAll(this.searchConditions.queryConditions)
  //                      }),
  //                  target = this.Response
  //              });
  //      }

		//////////////////////////////////////////////////////////////////////////////////////////////
		///// <summary>
		///// Excelダウンロード
		///// </summary>
		///// <returns></returns>
		//////////////////////////////////////////////////////////////////////////////////////////////
		//public void OnGetExcel(int id)
  //      {
  //          //レポートをエクスポートする
  //          var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
  //                  DateTimeOffset.Now, "yyyyMMddHHmmss");

  //          this.searchConditions.queryConditions.id = id;
  //          Utility.ReportHelper.HelperFunctions.writeExcel
  //              (new Utility.ReportHelper.HelperFunctions.WriteConfig
  //              {
  //                  fileName = $"RequestHistoryDetail_{nowStr}.xlsx",
  //                  report = new RazorPagesLearning.Report.RequestHistoryReportExcel(
  //                      new ReportConfigExcel
  //                      {
  //                          targetRequestHistory = this.requestHistoryService.readById(id).result,
  //                          targetInformation = this.requestHistoryDetailService.readAll(this.searchConditions.queryConditions)
  //                      }),
  //                  target = this.Response
  //              });
  //      }

		//////////////////////////////////////////////////////////////////////////////////////////////
		///// <summary>
		///// CSVダウンロード
		///// </summary>
		///// <returns></returns>
		//////////////////////////////////////////////////////////////////////////////////////////////
		//public void OnGetCsv(int id)
  //      {
  //          //レポートをエクスポートする
  //          var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
  //                  DateTimeOffset.Now, "yyyyMMddHHmmss");

  //          this.searchConditions.queryConditions.id = id;
  //          Utility.ReportHelper.HelperFunctions.writeCsv
  //              (new Utility.ReportHelper.HelperFunctions.WriteConfig
  //              {
  //                  fileName = $"RequestHistoryDetail_{nowStr}.csv",
  //                  report = new RazorPagesLearning.Report.RequestHistoryReportExcel(
  //                      new ReportConfigExcel
  //                      {
  //                          targetRequestHistory = this.requestHistoryService.readById(id).result,
  //                          targetInformation = this.requestHistoryDetailService.readAll(this.searchConditions.queryConditions)
  //                      }),
  //                  target = this.Response
  //              });
  //      }

		public async Task<IActionResult> OnGetIndexAsync(int id, int? start, int? take)
		{
			return await this.OnGetAsync(id, start, take);
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// requestアクション
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostRequestAsync()
		{
            await getStockDetailInfo();

            // サービス情報を更新
            updateService();

			// ログインユーザー情報
			var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
			var user = await userService.read(principalConfig);

			// 作業依頼に追加
			var addResult = this.requestListService.add(this.viewModel.mwlStockIds, user.ID);


			return RedirectToPage("/RequestHistoryDetail", new { id = 1 });
			//return RedirectToPage("/RequestHistoryDetail",
			//	new
			//	{
			//		id = this.searchConditions.queryConditions.id,
			//		start = this.searchConditions.queryConditions.start,
			//		take = this.searchConditions.queryConditions.take
			//	});
			//return RedirectToAction("index", "RequestHistoryDetail",
			//	new
			//	{
			//		id = this.searchConditions.queryConditions.id,
			//		start = this.searchConditions.queryConditions.start,
			//		take = this.searchConditions.queryConditions.take
			//	});
			//return Page();
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