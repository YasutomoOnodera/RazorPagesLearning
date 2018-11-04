using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Utility.Pagination;
using RazorPagesLearning.Utility.SelectableTable;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RazorPagesLearning.Service.User.UserService;

namespace RazorPagesLearning.Pages
{
    /// <summary>
    /// 作業依頼履歴(一覧)ページモデル
    /// 
    /// [アクセス制限]
    /// ・運送会社だけ入れない
    /// </summary>
    [Authorize(Roles = "Admin,ShipperBrowsing,ShipperEditing,Worker", Policy = "PasswordExpiration")]
	public class RequestHistoryModel : PageModel
    {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="ref_db"></param>
		/// <param name="ref_signInManager"></param>
		/// <param name="ref_userManager"></param>
		/// <param name="ref_logger"></param>
		public RequestHistoryModel(RazorPagesLearning.Data.RazorPagesLearningContext db,
			SignInManager<IdentityUser> signInManager,
			UserManager<IdentityUser> userManager,
			ILogger<LoginModel> logger)
		{
			// 各種サービス
			this.userService = new Service.User.UserService(db, User, signInManager, userManager);
			this.domainService = new Service.DB.DomainService(db, User, signInManager, userManager);
			this.requestHistoryService = new Service.DB.RequestHistoryService(db, User, signInManager, userManager);

			// コンストラクタ引数
			this._signInManager = signInManager;
			this._userManager = userManager;
			this._logger = logger;

			// 画面に表示する項目
			this.paginationInfo = initPaginationInfo();
			this.viewModel = new ViewModel();

			// DOMAINのコードを事前取得
			//this.viewModel.requestKind = domainService.getValueList(DOMAIN.Kind.REQUEST_REQUEST).ToList();
			//this.viewModel.wmsStatus = domainService.getValueList(DOMAIN.Kind.WMS_).ToList();

			//ポップアップ表示
			this.selectDeriveryModelViewModel = new PopUp.SelectDeriveryModel.ViewModel();
			this.selectDeriveryModel = new PopUp.SelectDeriveryModel(
				signInManager,
				logger,
				db);
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
            //            this.userService.getStatistics().numbers);
        }

        /// <summary>
        /// ページネーション情報
        /// </summary>
        [BindProperty]
        public PaginationInfo paginationInfo { get; set; }

		#endregion // ページネーション情報

		
		#region 表示項目

		/// <summary>
		/// 画面に表示する作業依頼履歴
		/// </summary>
		public class ViewREQUEST_HISTORY
        {
            /// <summary>
            /// 表示するデータ
            /// </summary>
            public RazorPagesLearning.Data.Models.REQUEST_HISTORY data { get; set; }
        }

		/// <summary>
		/// 画面上に表示する項目
		/// </summary>
		public class ViewModel : SelectableTableViewModelBase<ViewREQUEST_HISTORY>
        {
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public ViewModel() :
				base(ViewTableType.Search)
			{
			}

			/// <summary>
			/// 検索条件
			/// </summary>
			public SearchCondition searchCondition { get; set; }

			/// <summary>
			/// 依頼内容：DOMAIN(KIND=00020001)
			/// </summary>
			public List<DOMAIN> requestKind { get; set; }

            /// <summary>
            /// WMS状態：DOMAIN(KIND=00090000)のコード一覧
            /// </summary>
            public List<DOMAIN> wmsStatus { get; set; }
        }

        /// <summary>
        /// テーブル上の行情報
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

		#endregion // 表示項目


		#region 検索条件

		public class ReadConfig
		{
			public static class ConditionLabel
			{
				public const string REQUEST_HISTORY = "依頼履歴";
				public const string STATUS = "状態";
				public const string REQUEST_DATE = "依頼日";
				public const string ORDER_NUMBER = "受付番号";
				public const string DEST_CODE = "集配先コード";
				public const string DEST_NAME = "集配先名";
				public const string OWNER = "依頼者";
			}

			/// <summary>
			/// AND/OR条件
			/// </summary>
			public enum AndOr
			{
				/// <summary>
				/// OR条件
				/// </summary>
				Or,

				/// <summary>
				/// AND条件
				/// </summary>
				And
			}
		}

		/// <summary>
		/// 検索条件のタイプ
		/// </summary>
		public class CondType
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public CondType()
			{
				this.checkedDic = new Dictionary<string, bool>();
			}

			/// <summary>
			/// 検索条件のタイプ
			/// </summary>
			public enum SearchType
			{
				/// <summary>
				/// チェックボックス2列表示
				/// </summary>
				CheckBox_2Row,

				/// <summary>
				/// チェックボックス
				/// </summary>
				CheckBox,

				/// <summary>
				///  テキスト入力(FromTo)
				/// </summary>
				Text_FromTo,

				/// <summary>
				/// テキスト入力(AND/OR)
				/// </summary>
				Text_AndOr,

				/// <summary>
				/// テキスト入力
				/// </summary>
				Text
			}

			/// <summary>
			/// id属性値
			/// </summary>
			public string id { get; set; }

			/// <summary>
			/// 検索条件のタイプ
			/// </summary>
			public SearchType type { get; set; }

			/// <summary>
			/// チェックボックス項目 (DOMAIN.CODE, DOMAIN.VALUE)
			/// </summary>
			public SortedDictionary<string, string> checkBoxes { get; set; }


			#region POSTパラメータ取得用

			/// <summary>
			/// テキスト入力(From)
			/// </summary>
			public string textFrom { get; set; }

			/// <summary>
			/// テキスト入力(To)
			/// </summary>
			public string textTo { get; set; }

			/// <summary>
			/// テキスト入力
			/// </summary>
			public string text { get; set; }

			/// <summary>
			/// チェックボックス
			/// 選択されたチェックボックスについて(value属性値, true)を取得
			/// </summary>
			public Dictionary<string, bool> checkedDic { get; set; }

			/// <summary>
			/// ラジオボタン(AndOr)
			/// </summary>
			public ReadConfig.AndOr radio { get; set; }

			/// <summary>
			/// セレクトボックス
			/// </summary>
			public string selected { get; set; }

			#endregion // POSTパラメータ取得用
		}

		/// <summary>
		/// 検索条件
		/// </summary>
		public class SearchCondition
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public SearchCondition()
			{
				dispSetting = new Dictionary<string, CondType>();
			}

			/// <summary>
			/// 検索条件名毎の表示設定
			/// </summary>
			public Dictionary<string, CondType> dispSetting { get; set; }
		}

		#endregion // 検索条件


		// コンストラクタ引数
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ILogger<LoginModel> _logger;

		// 各種サービス
		private readonly Service.User.UserService userService;
        private readonly Service.DB.DomainService domainService;
        private readonly Service.DB.RequestHistoryService requestHistoryService;


		/// <summary>
		/// サービス情報を更新する
		/// </summary>
		private void updateService()
		{
			// PageModelのコンストラクタでは、ユーザーサービス情報は引き渡されない。
			// 実際にこれらの情報に触れるようになるのは、アクションメソッドが呼ばれたタイミングである。
			// このため、アクションメソッドが呼ばれたタイミングでユーザー情報を更新する
			this.userService.updateUser(User);
			this.domainService.updateUser(User);
			this.requestHistoryService.updateUser(User);
		}

		/// <summary>
		/// 画面上に表示されたデータ更新
		/// </summary>
		private void updateViewData()
        {
			//this.viewModel.tableRows = requestHistoryService.read(this.searchConditions.queryConditions)
			//		.Select(e => new RowInfo<ViewREQUEST_HISTORY>
			//		{
			//			data = new ViewREQUEST_HISTORY
			//			{
			//				data = e,
			//			}
			//		}).ToList();

			this.viewModel.tableRows = requestHistoryService.read(new Service.DB.RequestHistoryService.ReadConfig
			{
				start = 0,
				take = 20
			})
			.Select(e => new RowInfo<ViewREQUEST_HISTORY>
			{
				data = new ViewREQUEST_HISTORY
				{
					data = e,
				}
			}).ToList();


		}

		/// <summary>
		/// アクションの準備
		/// </summary>
		/// <returns></returns>
		private async Task<USER_ACCOUNT> prepare()
		{
			// サービス情報を更新
			this.updateService();

			// DOMAINの値
			viewModel.requestKind = domainService.getCodeList(DOMAIN.Kind.REQUEST_REQUEST).ToList();
			viewModel.wmsStatus = domainService.getCodeList(DOMAIN.Kind.WMS_).ToList();

			// ログインユーザー情報
			var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
			var user = await userService.read(principalConfig);

			return user;
		}


		/// <summary>
		/// 検索条件のセットアップ
		/// </summary>
		/// <returns></returns>
		private void setupSearchCondition()
		{
			this.viewModel.searchCondition = new SearchCondition();

			// 依頼履歴
			this.viewModel.searchCondition.dispSetting.Add(
				ReadConfig.ConditionLabel.REQUEST_HISTORY,
				new CondType
				{
					id = "search_cond_request_history",
					type = CondType.SearchType.CheckBox_2Row,
					checkBoxes = new SortedDictionary<string, string>(
						domainService.getValueList(DOMAIN.Kind.REQUEST_REQUEST)
						.Where(e => e.CODE != DOMAIN.RequestRequestCode.MATERIAL)
						.OrderBy(e => e.CODE)
						.ToDictionary(e => e.CODE, e => e.VALUE)
					)
				}
			);

			// 状態
			this.viewModel.searchCondition.dispSetting.Add(
				ReadConfig.ConditionLabel.STATUS,
				new CondType
				{
					id = "search_cond_status",
					type = CondType.SearchType.CheckBox,
					checkBoxes = new SortedDictionary<string, string>(
						domainService.getValueList(DOMAIN.Kind.WMS_)
						.OrderBy(e => e.CODE)
						.ToDictionary(e => e.CODE, e => e.VALUE)
					),
				}
			);

			// 依頼日
			this.viewModel.searchCondition.dispSetting.Add(
				ReadConfig.ConditionLabel.REQUEST_DATE,
				new CondType
				{
					id = "search_cond_request_date",
					type = CondType.SearchType.Text_FromTo
				}
			);

			// 受付番号
			this.viewModel.searchCondition.dispSetting.Add(
				ReadConfig.ConditionLabel.ORDER_NUMBER,
				new CondType
				{
					id = "search_cond_order_number",
					type = CondType.SearchType.Text_FromTo
				}
			);

			// 集配先コード
			this.viewModel.searchCondition.dispSetting.Add(
				ReadConfig.ConditionLabel.DEST_CODE,
				new CondType
				{
					id = "search_cond_dest_code",
					type = CondType.SearchType.Text
				}
			);

			// 集配先名
			this.viewModel.searchCondition.dispSetting.Add(
				ReadConfig.ConditionLabel.DEST_NAME,
				new CondType
				{
					id = "searchDestCodeValue",
					type = CondType.SearchType.Text_AndOr
				}
			);

			// 依頼者
			this.viewModel.searchCondition.dispSetting.Add(
				ReadConfig.ConditionLabel.OWNER,
				new CondType
				{
					id = "search_cond_owner",
					type = CondType.SearchType.Text,
				}
			);
		}



		/// <summary>
		/// indexアクション
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<IActionResult> OnGetAsync(int? id)
        {
			// アクションの準備
			var user = await this.prepare();

			// 検索条件のセットアップ
			this.setupSearchCondition();


            ////ページ指定が来ていたら取り込む
            //this.paginationInfo.maxItems = this.requestHistoryService.getStatistics().numbers;
            //if (true == id.HasValue)
            //{
            //    this.paginationInfo.movePage(id.Value);
            //}
            //updateViewData();
            return Page();
        }

		///// <summary>
		///// 検索
		///// </summary>
		///// <param name="id"></param>
		///// <returns></returns>
		//public async Task<IActionResult> OnPostAsync(int? id)
  //      {
  //          //// TODO:ここは検索のやつできたら見直し
  //          ////ページ指定が来ていたら取り込む
  //          //this.paginationInfo.maxItems = this.requestHistoryService.getStatistics().numbers;
  //          //if (true == id.HasValue)
  //          //{
  //          //    this.paginationInfo.movePage(id.Value);
  //          //}
  //          //this.searchConditions.queryConditions.deliveryShipperCode = "002";

  //          //updateViewData();
  //          //Console.WriteLine(this.searchConditions.queryConditions.deliveryShipperCode);
  //          return this.Page();
  //      }

		public async Task<IActionResult> OnPostListAsync()
		{
			// アクションの準備
			await prepare();

			// 検索実行
			updateViewData();

			return Page();
		}

		/// <summary>
		/// 検索条件クリア
		/// </summary>
		/// <returns></returns>
		public IActionResult OnPostClear()
        {
            //// TODO:ここは検索のやつできたら見直し
            //this.searchConditions.queryConditions.start = 0;
            //this.searchConditions.queryConditions.start = 20;
            //this.searchConditions.queryConditions.orderNumberStart = "";
            //this.searchConditions.queryConditions.orderNumberEnd = "";

            //updateViewData();
            return Page();
        }

		/// <summary>
		/// Excelダウンロード
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> OnPostExcel(string page)
        {
			// サービス情報を更新
			this.updateService();

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            // 読み取り情報
            var readConfig = new Service.DB.RequestHistoryService.ReadConfig
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
            //            fileName = $"RequestHistory_{nowStr}.xlsx",
            //            report = new RazorPagesLearning.Report.RequestHistorySearchReport(
            //                new RazorPagesLearning.Report.RequestHistorySearchReport.ReportConfig
            //                {
            //                    targetInformation = requestHistoryService.read(readConfig)
            //                }),
            //            target = this.Response
            //        });
            //}
            // OnPostAsyncを実行して画面再描画
            return RedirectToPagePermanentPreserveMethod("RequestHisory");
        }

		/// <summary>
		/// CSVダウンロード
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> OnPostCsv(string page)
        {
			// サービス情報を更新
			this.updateService();

			// ユーザー情報の取得
			Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);

            // 読み取り情報
            var readConfig = new Service.DB.RequestHistoryService.ReadConfig
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
            //            fileName = $"RequestHistory_{nowStr}.xlsx",
            //            report = new RazorPagesLearning.Report.RequestHistorySearchReport(
            //                new RazorPagesLearning.Report.RequestHistorySearchReport.ReportConfig
            //                {
            //                    targetInformation = requestHistoryService.read(readConfig)
            //                }),
            //            target = this.Response
            //        });
            //}
            // OnPostAsyncを実行して画面再描画
            return RedirectToPagePermanentPreserveMethod("RequestHisory");
        }
    }
}