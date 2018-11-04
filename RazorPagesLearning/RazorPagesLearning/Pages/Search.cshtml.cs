using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
using static RazorPagesLearning.Pages.SearchModel.ViewModel;
using static RazorPagesLearning.Service.DB.StockSearchService;
using static RazorPagesLearning.Service.User.UserService;
using static RazorPagesLearning.Service.DB.RequestHistoryDetailService;

namespace RazorPagesLearning.Pages
{
	///////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 検索ページモデル
	/// 
	/// [アクセス制限]
	/// ・運送会社だけ入れない
	/// 
	/// </summary>    
	///////////////////////////////////////////////////////////////////////////////////////////////////
	[Authorize(Roles = "Admin,ShipperBrowsing,ShipperEditing,Worker", Policy = "PasswordExpiration")]
    public class SearchModel : PageModel
    {
		#region ページネーション情報

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// ページネーション情報を初期化する
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private PaginationInfo initPaginationInfo()
        {
            return PaginationInfo.createInstance(
            stockSearchService.getStatistics().numbers);
        }

        /// <summary>
        /// ページネーション情報
        /// </summary>
        [BindProperty]
        public PaginationInfo paginationInfo { get; set; }

		#endregion


		#region 表示項目

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 画面上に表示する項目
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public class ViewModel : SelectableTableViewModelBase<RazorPagesLearning.Service.DB.StockSearchService.StockSearchResult>
        {
			/// <summary>
			/// 検索条件
			/// </summary>
			public SearchCondition searchCondition { get; set; }

			/// <summary>
			/// 表示設定(項目の並び替え設定)
			/// </summary>
			public List<USER_DISPLAY_SETTING> colOrders { get; set; }

			/// <summary>
			/// ステータスの値
			/// </summary>
			public List<DOMAIN> statusDomains { get; set; }

			/// <summary>
			/// 処理結果メッセージ
			/// </summary>
			public string resultMessage { get; set; }

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


            ////////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// コンストラクタ
            /// </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////
            public ViewModel() :
                base(ViewTableType.Search)
            {
				searchCondition = new SearchCondition();
            }
        }

        /// <summary>
        /// テーブル上の行情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

		#endregion // 表示項目


		#region 表示計算処理

        /// <summary>
        ///検索
        /// </summary>
        /// <returns></returns>
        private IQueryable<STOCK> makeReadQuery()
        {
            // 検索条件を作る
            StockSearchService.ReadConfig config = new StockSearchService.ReadConfig();
            mkReadConfigStatus(config);

            // ソート条件を作る
            List<string> order = viewModel.colOrders.OrderBy(e => e.SORT).Select(e => e.PHYS_COLUMN_NAME).ToList();
            Service.Definition.SortDirection direction = Service.Definition.SortDirection.ASC;
            bool byColumnHeader = false;

            if (!string.IsNullOrEmpty(viewModel.sortColumn))
            {
                order = new List<string> { viewModel.sortColumn };
                direction = viewModel.sortDirection;
                byColumnHeader = true;
            }

            // 検索
            var searchResults = stockSearchService.read(config);
            searchResults = stockSearchService.setSortOrder(order, searchResults, direction, byColumnHeader);

            return searchResults;
        }
		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 画面上に表示されたデータ更新
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		private async Task<bool> updateViewData()
        {
			// =====================================================================================
			// 検索
			// =====================================================================================
			// 検索
            var searchResults = makeReadQuery();
			// ページネーション情報を設定
			paginationInfo.maxItems = searchResults.Count();
			paginationInfo.movePage(paginationInfo.displayNextPage);

			// =====================================================================================
			// チェックボックスのチェック状態を復元しつつテーブルを読み込む
			// =====================================================================================
			return await this.checkStatusManagement.stateRestore(
                new CheckStatusManagement<StockSearchResult>.StateRestoreConfig
                {
                    // 対象となるユーザーID
                    userInfo = User,
                    // データが格納されたビューモデル
                    viewModel = this.viewModel,
                    // ページネーション管理情報
                    paginationInfo = this.paginationInfo,
					// DBから表に表示する情報を読み取る関数
					readFunc = new Func<IQueryable<StockSearchResult>>(() =>
					{
						return searchResults.Select(e => new StockSearchResult
						{
							ID = e.ID,
							STORAGE_MANAGE_NUMBER = e.STORAGE_MANAGE_NUMBER,
							STATUS = e.STATUS,
							TITLE = e.TITLE,
							SUBTITLE = e.SUBTITLE,
							NOTE = e.NOTE,
							SHIPPER_NOTE = e.SHIPPER_NOTE,
							CUSTOMER_MANAGE_NUMBER = e.CUSTOMER_MANAGE_NUMBER,
							PROCESSING_DATE = e.PROCESSING_DATE,
							SHAPE = e.SHAPE,
							REMARK1 = e.REMARK1,
							REMARK2 = e.REMARK2,
							STOCK_COUNT = (Int64)e.STOCK_COUNT
						})
						.Skip(paginationInfo.startViewItemIndex)
						.Take(paginationInfo.endViewItemIndex);
					}),
					// DB上の最大レコード件数
					maxRecords = stockSearchService.getStatistics().numbers
                });
        }

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 画面上の在庫テーブルにおけるチェックボックスのチェック状態を管理する
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		class StockTableCheckStatusManagement : CheckStatusManagement<StockSearchResult>
        {
            public override TableSelectionSettingService.TrackingIdentifier translationRelationIdentifier(StockSearchResult table)
            {
                return new Service.DB.TableSelectionSettingService.TrackingIdentifier
                {
                    dataId = table.ID
                };
            }

			////////////////////////////////////////////////////////////////////////////////////////
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="db"></param>
			/// <param name="signInManager"></param>
			/// <param name="userManager"></param>
			/// <param name="paginationInfo"></param>
			/// <param name="ref_user"></param>
			////////////////////////////////////////////////////////////////////////////////////////
			public StockTableCheckStatusManagement(RazorPagesLearning.Data.RazorPagesLearningContext db,
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

		#endregion // 表示計算処理

        #region 帳票出力用
        /// <summary>
        ///検索用クエリを作成する
        /// </summary>
        /// <returns></returns>
        private IQueryable<STOCK> makeReadQueryReport()
        {
            // =====================================================================================
            // 検索
            // =====================================================================================
            // 検索条件を作る
            StockSearchService.ReadConfig config = new StockSearchService.ReadConfig();
            mkReadConfigStatus(config);

            // ソート条件を作る
            List<string> order = viewModel.colOrders.OrderBy(e => e.SORT).Select(e => e.PHYS_COLUMN_NAME).ToList();
            Service.Definition.SortDirection direction = Service.Definition.SortDirection.ASC;
            bool byColumnHeader = false;

            if (!string.IsNullOrEmpty(viewModel.sortColumn))
            {
                order = new List<string> { viewModel.sortColumn };
                direction = viewModel.sortDirection;
                byColumnHeader = true;
            }

            // 検索
            var searchResults = stockSearchService.readReport(config);
            searchResults = stockSearchService.setSortOrder(order, searchResults, direction, byColumnHeader);

            return searchResults;
        }

        #endregion // 帳票出力用

		#region 検索条件

		//////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件のタイプ
		/// </summary>
		//////////////////////////////////////////////////////////////////////////////////////////////////
		public class CondType
		{
			////////////////////////////////////////////////////////////////////////////////////////
			/// <summary>
			/// コンストラクタ
			/// </summary>
			////////////////////////////////////////////////////////////////////////////////////////
			public CondType()
			{
				textList1 = new List<string>();
				textList2 = new List<string>();
				checkedDic = new Dictionary<string, bool>();
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
				/// テキスト入力(FromTo)
				/// </summary>
				Text_FromTo,

				/// <summary>
				/// テキスト入力(AND/OR)
				/// </summary>
				Text_AndOr,

				/// <summary>
				/// テキスト入力(AND/OR)
				/// [テキスト][テキスト] AND/OR
				/// </summary>
				Text2_AndOr,

				/// <summary>
				/// セレクトボックス
				/// </summary>
				Select,

				/// <summary>
				/// セレクトボックス テキストボックス (AND/OR)
				/// [セレクト][テキスト] AND/OR
				/// </summary>
				SelectText_AndOr,

				/// <summary>
				/// 倉庫管理番号
				/// [1桁][1桁][5桁][3桁]～[1桁][1桁][5桁][3桁]
				/// </summary>
				StorageManageNumber,

				/// <summary>
				/// お客様項目
				/// [テキスト][テキスト] AND/OR
				/// [テキスト][テキスト] AND/OR
				/// [テキスト][テキスト] AND/OR
				/// </summary>
				CustomerItem
			}

			/// <summary>
			/// 有効 or 無効
			/// </summary>
			public bool enable { get; set; }

			/// <summary>
			/// 切り替え可能
			/// </summary>
			public bool switchable { get; set; }

			/// <summary>
			/// id属性値
			/// </summary>
			public string id { get; set; }

			/// <summary>
			/// 検索条件のタイプ
			/// </summary>
			public SearchType type { get; set; }

			/// <summary>
			/// セレクト項目 (DOMAIN.CODE, DOMAIN.VALUE)
			/// </summary>
#if false // セレクトボックス
			public List<SelectListItem> selects { get; set; }
#else
			public SortedDictionary<string, string> selects { get; set; }
#endif

			/// <summary>
			/// チェックボックス項目 (DOMAIN.CODE, DOMAIN.VALUE)
			/// </summary>
			public SortedDictionary<string, string> checkBoxes { get; set; }

			/// <summary>
			/// カラム名
			/// </summary>
			public string colName { get; set; }


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
			/// テキスト入力(複数1)
			/// </summary>
			public List<string> textList1 { get; set; }

			/// <summary>
			/// テキスト入力(複数2)
			/// </summary>
			public List<string> textList2 { get; set; }

			/// <summary>
			/// テキスト入力(複数1)
			/// </summary>
			public string text1 { get; set; }

			/// <summary>
			/// テキスト入力(複数2)
			/// </summary>
			public string text2 { get; set; }

			/// <summary>
			/// テキスト入力(複数3)
			/// </summary>
			public string text3 { get; set; }

			/// <summary>
			/// テキスト入力(複数4)
			/// </summary>
			public string text4 { get; set; }

			/// <summary>
			/// テキスト入力(複数5)
			/// </summary>
			public string text5 { get; set; }

			/// <summary>
			/// テキスト入力(複数6)
			/// </summary>
			public string text6 { get; set; }

			/// <summary>
			/// チェックボックス
			/// 選択されたチェックボックスについて(value属性値, true)を取得
			/// </summary>
			public Dictionary<string, bool> checkedDic { get; set; }

			/// <summary>
			/// ラジオボタン1(AndOr)
			/// </summary>
			public StockSearchService.ReadConfig.AndOr radio1 { get; set; }

			/// <summary>
			/// ラジオボタン2(AndOr)
			/// </summary>
			public StockSearchService.ReadConfig.AndOr radio2 { get; set; }

			/// <summary>
			/// ラジオボタン3(AndOr)
			/// </summary>
			public StockSearchService.ReadConfig.AndOr radio3 { get; set; }

			/// <summary>
			/// セレクトボックス
			/// </summary>
			public string selected { get; set; }

			#endregion // POSTパラメータ取得用
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public class SearchCondition
		{
			////////////////////////////////////////////////////////////////////////////////////////
			/// <summary>
			/// コンストラクタ
			/// </summary>
			////////////////////////////////////////////////////////////////////////////////////////
			public SearchCondition()
			{
				dispSetting = new Dictionary<string, CondType>();
			}

			/// <summary>
			/// 検索条件名毎の表示設定
			/// </summary>
			public Dictionary<string, CondType> dispSetting { get; set; }
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の作成
		/// POSTパラメータより、DB検索用の読み取り設定を作成する
		/// </summary>
		/// <param name="conf"></param>
		////////////////////////////////////////////////////////////////////////////////////////////
		private void mkReadConfigStatus(StockSearchService.ReadConfig conf)
		{
			char[] delim = { ' ', '　' };
			DateTimeOffset dto;

			// ステータス
			conf.status = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.STATUS].checkedDic.Keys.ToList();

			// 作業依頼に追加した在庫を非表示にする
			conf.request = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.REQUEST].checkedDic.Count() == 0 ? false : true;

			// お客様管理番号
			conf.customerManageNumberFrom = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_MANAGE_NUMBER].textFrom;
			conf.customerManageNumberTo = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_MANAGE_NUMBER].textTo;

			// お客様管理番号(複数)
			conf.customerManageNumbers = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_MANAGE_NUMBERS].text1;
			conf.customerManageNumbers_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_MANAGE_NUMBERS].radio1;

			// 題名
			conf.title = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.TITLE].text1;
			conf.title_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.TITLE].radio1;

			// 副題
			conf.subtitle = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.SUBTITLE].text1;
			conf.subtitle_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.SUBTITLE].radio1;

			// 部課
			conf.departmentCode = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.DEPARTMENT].selected;

			// 形状
			conf.shape = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.SHAPE].text1;
			conf.shape_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.SHAPE].radio1;

			// 区分1
			conf.class1 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CLASS1].checkedDic.Keys.ToList();

			// 区分2
			conf.class2 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CLASS2].checkedDic.Keys.ToList();

			// 倉庫管理番号
			conf.storageManageNumberFrom = string.Join("", viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.STORAGE_MANAGE_NUMBER].textList1);
			conf.storageManageNumberTo = string.Join("", viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.STORAGE_MANAGE_NUMBER].textList2);

			// Remark1
			conf.remark1 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.REMARK1].text1;
			conf.remark1_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.REMARK1].radio1;

			// Remark2
			conf.remark2 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.REMARK2].text1;
			conf.remark2_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.REMARK2].radio1;

			// 備考
			conf.note = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.NOTE].text1;
			conf.note_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.NOTE].radio1;

			// 制作日
			conf.productDateFrom = DateTimeOffset.TryParse(viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PRODUCT_DATE].textFrom, out dto) ? dto : (DateTimeOffset?)null;
			conf.productDateTo = DateTimeOffset.TryParse(viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PRODUCT_DATE].textTo, out dto) ? dto : (DateTimeOffset?)null;

			// 入庫日
			conf.storageDateFrom = DateTimeOffset.TryParse(viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.STORAGE_DATE].textFrom, out dto) ? dto : (DateTimeOffset?)null;
			conf.storageDateTo = DateTimeOffset.TryParse(viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.STORAGE_DATE].textTo, out dto) ? dto : (DateTimeOffset?)null;

			// 処理日
			conf.processingDateFrom = DateTimeOffset.TryParse(viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PROCESSING_DATE].textFrom, out dto) ? dto : (DateTimeOffset?)null;
			conf.processingDateTo = DateTimeOffset.TryParse(viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PROCESSING_DATE].textTo, out dto) ? dto : (DateTimeOffset?)null;

			// 廃棄予定日
			conf.scrapScheduleDateFrom = DateTimeOffset.TryParse(viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.SCRAP_SCHEDULE_DATE].textFrom, out dto) ? dto : (DateTimeOffset?)null;
			conf.scrapScheduleDateTo = DateTimeOffset.TryParse(viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.SCRAP_SCHEDULE_DATE].textTo, out dto) ? dto : (DateTimeOffset?)null;

			// 出荷先/返却元
			conf.shipReturnCode = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.SHIP_RETURN].text1;
			conf.shipReturnCode_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.SHIP_RETURN].radio1;

			// 登録日
			conf.registDateFrom = DateTimeOffset.TryParse(viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.REGIST_DATE].textFrom, out dto) ? dto : (DateTimeOffset?)null;
			conf.registDateTo = DateTimeOffset.TryParse(viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.REGIST_DATE].textTo, out dto) ? dto : (DateTimeOffset?)null;

			// お客様項目
			// -- 1 --
			conf.customerItem1Code = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_ITEM].text1;
			conf.customerItem1Value = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_ITEM].text2;
			conf.customerItem1_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_ITEM].radio1;
			// -- 2 --
			conf.customerItem2Code = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_ITEM].text3;
			conf.customerItem2Value = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_ITEM].text4;
			conf.customerItem2_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_ITEM].radio2;
			// -- 3 --
			conf.customerItem3Code = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_ITEM].text5;
			conf.customerItem3Value = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_ITEM].text6;
			conf.customerItem3_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_ITEM].radio3;

			// 荷主項目
			conf.shipperNote = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.SHIPPER_NOTE].text1;
			conf.shipperNote_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.SHIPPER_NOTE].radio1;

			// ProjectNo
			conf.projectNo1 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PROJECT_NO].text1;
			conf.projectNo2 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PROJECT_NO].text2;
			conf.projectNo_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PROJECT_NO].radio1;

			// 著作権
			conf.copyright1 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.COPYRIGHT].selected;
			conf.copyright2 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.COPYRIGHT].text1;
			conf.copyright_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.COPYRIGHT].radio1;

			// 契約書
			conf.contract1 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CONTRACT].selected;
			conf.contract2 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CONTRACT].text1;
			conf.contract_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CONTRACT].radio1;

			// データNo
			conf.dataNo1 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.DATA_NO].text1;
			conf.dataNo2 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.DATA_NO].text2;
			conf.dataNo_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.DATA_NO].radio1;

			// 処理判定
			conf.processJudge1 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PROCESS_JUDGE].selected;
			conf.processJudge2 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PROCESS_JUDGE].text1;
			conf.processJudge_AndOr = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PROCESS_JUDGE].radio1;
		}

		#endregion // 検索条件


		#region // 各種サービス

		/// <summary>
		/// ユーザー関係のサービス
		/// </summary>
		private readonly Service.User.UserService userService;

		/// <summary>
		/// 在庫情報サービス
		/// </summary>
		private readonly StockSearchService stockSearchService;

		/// <summary>
		/// 在庫サービス
		/// </summary>
		private readonly StockService stockService;

		/// <summary>
		/// 検索条件サービス
		/// </summary>
		private readonly SearchConditionService searchConditionService;

		/// <summary>
		/// 表示設定サービス
		/// </summary>
		private readonly DisplaySettingService displaySettingService;

		/// <summary>
		/// 作業依頼一覧サービス
		/// </summary>
		private readonly RequestListService requestListService;

        /// <summary>
        /// 作業依頼履歴詳細サービス
        /// </summary>
        private RazorPagesLearning.Service.DB.RequestHistoryDetailService requestHistoryDetailService;

        /// <summary>
        /// WMS作業依頼履歴詳細サービス
        /// </summary>
        private RazorPagesLearning.Service.DB.WmsRequestHistoryDetailService wmsRequestHistoryDetailService;

		/// <summary>
		/// ウォッチリストサービス
		/// </summary>
		private readonly WatchlistService watchlistService;

		/// <summary>
		/// DOMAINサービス
		/// </summary>
		private readonly DomainService domainService;

		/// <summary>
		/// チェックボックスの状態を持つサービス
		/// </summary>
		private readonly WkTableSelectionSettingService wkTableSelectionSettingService;

        #endregion // 各種サービス

        /// <summary>
        /// DBアクセスオブジェクト
        /// </summary>
        public readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;

        //コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        //テーブル上の選択状態を管理する
        private StockTableCheckStatusManagement checkStatusManagement;

		//////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="db"></param>
		/// <param name="signInManager"></param>
		/// <param name="userManager"></param>
		/// <param name="logger"></param>
		//////////////////////////////////////////////////////////////////////////////////////////////////
        public SearchModel(
			RazorPagesLearning.Data.RazorPagesLearningContext db,
			SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
			// 各種サービス
			this.userService = new Service.User.UserService(db, User, signInManager, userManager);
			this.stockSearchService = new StockSearchService(db, User, signInManager, userManager);
			this.stockService = new StockService(db, User, signInManager, userManager);
			this.searchConditionService = new SearchConditionService(db, User, signInManager, userManager);
			this.displaySettingService = new DisplaySettingService(db, User, signInManager, userManager);
			this.requestListService = new RequestListService(db, User, signInManager, userManager);
            this.requestHistoryDetailService = new Service.DB.RequestHistoryDetailService(db, User, this._signInManager, this._userManager);
            this.wmsRequestHistoryDetailService = new Service.DB.WmsRequestHistoryDetailService(db, User, this._signInManager, this._userManager);
			this.watchlistService = new WatchlistService(db, User, signInManager, userManager);
			this.domainService = new DomainService(db, User, signInManager, userManager);
			this.wkTableSelectionSettingService = new WkTableSelectionSettingService(db, User, signInManager, userManager);

            this._db = db;
			this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;
            this.paginationInfo = initPaginationInfo();
            this.viewModel = new ViewModel();

            this.checkStatusManagement = new StockTableCheckStatusManagement
                (db,
                signInManager,
                userManager,
                paginationInfo,
                User);
        }

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
			this.stockSearchService.updateUser(User);
			this.stockService.updateUser(User);
			this.searchConditionService.updateUser(User);
			this.displaySettingService.updateUser(User);
			this.requestListService.updateUser(User);
			this.watchlistService.updateUser(User);
			this.domainService.updateUser(User);
			this.wkTableSelectionSettingService.updateUser(User);
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// フォーム状態を保存する
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		private async Task<bool> formStateSave()
        {
			// ログインユーザー情報
			var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
			var user = await userService.read(principalConfig);

			// 検索条件を更新
			await searchConditionService.Update(new USER_SEARCH_CONDITION
			{
				USER_ACCOUNT_ID = user.ID,
				CLASS1 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CLASS1].enable,
				CLASS2 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CLASS2].enable,
				STORAGE_MANAGE_NUMBER = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.STORAGE_MANAGE_NUMBER].enable,
				REMARK1 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.REMARK1].enable,
				REMARK2 = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.REMARK2].enable,
				NOTE = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.NOTE].enable,
				PRODUCT_DATE = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PRODUCT_DATE].enable,
				STORAGE_DATE = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.STORAGE_DATE].enable,
				PROCESSING_DATE = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PROCESSING_DATE].enable,
				SCRAP_SCHEDULE_DATE = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.SCRAP_SCHEDULE_DATE].enable,
				SHIPPER_RETURN = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.SHIP_RETURN].enable,
				REGIST_DATE = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.REGIST_DATE].enable,
				CUSTOMER_ITEM = viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_ITEM].enable,
			});

			// 表示設定を更新
			await displaySettingService.update(viewModel.colOrders);

			// テーブルのチェック状態を記憶
            await this.checkStatusManagement.stateSave(
				new CheckStatusManagement<StockSearchResult>.StatePersistenceConfig
                {
                    userInfo = User,
                    viewModel = this.viewModel,
                    paginationInfo = this.paginationInfo,

                }
			);

			return true;
        }

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// チェックボックスの内容を初期化
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private async Task<bool> deleteCheckInfoAll()
		{
			// ログインユーザー情報
			var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
			var user = await userService.read(principalConfig);

			wkTableSelectionSettingService.deleteCheckInfoAll(user.ID, ViewTableType.Search);

			return true;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 現在のチェックボックスの状態を取得
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private async Task<List<WK_TABLE_SELECTION_SETTING>> getCheckInfo()
		{
			// ログインユーザー情報
			var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
			var user = await userService.read(principalConfig);

			var ret = wkTableSelectionSettingService.getCheckStateList(user.ID, ViewTableType.Search);

			return ret;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件のセットアップ
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		private async Task<bool> SetupSearchCondition()
		{
			// サービス情報を更新
			updateService();

			// ログインユーザー情報
			var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
			var user = await userService.read(principalConfig);

			// ステータスの値
			viewModel.statusDomains = domainService.getCodeList(DOMAIN.Kind.STCOK_STATUS).ToList();

			#region // 検索条件

			// =====================================================================================
			// 検索条件
			// =====================================================================================
			viewModel.searchCondition = new SearchCondition();

            // 検索条件の有効/無効を読み込む
            var condEnable = (USER_SEARCH_CONDITION)(await searchConditionService.Read(user.ID)).result;

			// -------------------------------------------------------------------------------------
			// ステータス
			//   資材以外をステータスのチェックボックス表示対象とする
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.STATUS,
				new CondType
				{
					enable = true,
					id = "search_cond_status",
					type = CondType.SearchType.CheckBox_2Row,
					checkBoxes = new SortedDictionary<string, string>(
						domainService.getValueList(DOMAIN.Kind.STCOK_STATUS)
						.Where(e => e.CODE != DOMAIN.StockStatusCode.MATERIAL)
						.OrderBy(e => e.CODE)
						.ToDictionary(e => e.CODE, e => e.VALUE)
					),
					colName = "STATUS",
				}
			);

			// -------------------------------------------------------------------------------------
			// 作業依頼に追加した在庫を非表示にする
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.REQUEST,
				new CondType
				{
					enable = true,
					id = "search_cond_request",
					type = CondType.SearchType.CheckBox,
					checkBoxes = new SortedDictionary<string, string>()
					{
						// "10"に意味はない。チェックボックス選択されたことが分かれば良い。
						{ "10", "作業依頼に追加した在庫を非表示にする" }
					},
					colName = ""
				}
			);

			// -------------------------------------------------------------------------------------
			// お客様管理番号
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_MANAGE_NUMBER,
				new CondType
				{
					enable = true,
					id = "search_cond_customer_manage_number",
					type = CondType.SearchType.Text_FromTo,
					colName = "CUSTOMER_MANAGE_NUMBER"
				}
			);

			// -------------------------------------------------------------------------------------
			// "お客様管理番号(複数)"
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_MANAGE_NUMBERS,
				new CondType
				{
					enable = true,
					id = "search_cond_customer_manage_number2",
					type = CondType.SearchType.Text_AndOr,
					colName = "CUSTOMER_MANAGE_NUMBER"
				}
			);

			// -------------------------------------------------------------------------------------
			// 題名
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.TITLE,
				new CondType
				{
					enable = true,
					id = "search_cond_title",
					type = CondType.SearchType.Text_AndOr,
					colName = "TITLE"
				}
			);

			// -------------------------------------------------------------------------------------
			// 副題
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.SUBTITLE,
				new CondType
				{
					enable = true,
					id = "search_cond_subtitle",
					type = CondType.SearchType.Text_AndOr,
					colName = "SUBTITLE"
				}
			);

			// -------------------------------------------------------------------------------------
			// 部課
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.DEPARTMENT,
				new CondType
				{
					enable = true,
					id = "search_cond_department",
					type = CondType.SearchType.Select,
					selects = new SortedDictionary<string, string>(
						userService.readDEPARTMENT_ADMINs(user.ID)
						.ToDictionary(e => e.DEPARTMENT_CODE, e => e.DEPARTMENT_NAME)
					),
					colName = "DEPARTMENT_CODE"
				}
			);

			viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.DEPARTMENT].selects.Add("", "");

			// -------------------------------------------------------------------------------------
			// 形状
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.SHAPE,
				new CondType
				{
					enable = true,
					id = "search_cond_shape",
					type = CondType.SearchType.Text_AndOr,
					colName = "SHAPE"
				}
			);

			// -------------------------------------------------------------------------------------
			// 区分1
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.CLASS1,
				new CondType
				{
					enable = condEnable.CLASS1,
					switchable = true,
					id = "search_cond_class1",
					type = CondType.SearchType.CheckBox_2Row,
					checkBoxes = new SortedDictionary<string, string>(
						domainService.getValueList(DOMAIN.Kind.STOCK_CLASS1)
						.OrderBy(e => e.CODE)
						.ToDictionary(e => e.CODE, e => e.VALUE)
					),
					colName = "CLASS1"
				}
			);

			// -------------------------------------------------------------------------------------
			// 区分2
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.CLASS2,
				new CondType
				{
					enable = condEnable.CLASS2,
					switchable = true,
					id = "search_cond_class2",
					type = CondType.SearchType.CheckBox_2Row,
					checkBoxes = new SortedDictionary<string, string>(
						domainService.getValueList(DOMAIN.Kind.STOCK_CLASS2)
						.OrderBy(e => e.CODE)
						.ToDictionary(e => e.CODE, e => e.VALUE)
					),
					colName = "CLASS2"
				}
			);

			// -------------------------------------------------------------------------------------
			// 倉庫管理番号
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.STORAGE_MANAGE_NUMBER,
				new CondType
				{
					enable = condEnable.STORAGE_MANAGE_NUMBER,
					switchable = true,
					id = "search_cond_storage_manage_number",
					type = CondType.SearchType.StorageManageNumber,
					colName = "STORAGE_MANAGE_NUMBER"
				}
			);

			// -------------------------------------------------------------------------------------
			// Remark1
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.REMARK1,
				new CondType
				{
					enable = condEnable.REMARK1,
					switchable = true,
					id = "search_cond_remark1",
					type = CondType.SearchType.Text_AndOr,
					colName = "REMARK1"
				}
			);

			// -------------------------------------------------------------------------------------
			// Remark2
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.REMARK2,
				new CondType
				{
					enable = condEnable.REMARK2,
					switchable = true,
					id = "search_cond_remark2",
					type = CondType.SearchType.Text_AndOr,
					colName = "REMARK2"
				}
			);

			// -------------------------------------------------------------------------------------
			// 備考
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.NOTE,
				new CondType
				{
					enable = condEnable.NOTE,
					switchable = true,
					id = "search_cond_note",
					type = CondType.SearchType.Text_AndOr,
					colName = "NOTE"
				}
			);

			// -------------------------------------------------------------------------------------
			// 制作日
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.PRODUCT_DATE,
				new CondType
				{
					enable = condEnable.PRODUCT_DATE,
					switchable = true,
					id = "search_cond_product_date",
					type = CondType.SearchType.Text_FromTo,
					colName = "PRODUCT_DATE"
				}
			);

			// -------------------------------------------------------------------------------------
			// 入庫日
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.STORAGE_DATE,
				new CondType
				{
					enable = condEnable.STORAGE_DATE,
					switchable = true,
					id = "search_cond_storage_date",
					type = CondType.SearchType.Text_FromTo,
					colName = "STORAGE_DATE"
				}
			);

			// -------------------------------------------------------------------------------------
			// 処理日
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.PROCESSING_DATE,
				new CondType
				{
					enable = condEnable.PROCESSING_DATE,
					switchable = true,
					id = "search_cond_processing_date",
					type = CondType.SearchType.Text_FromTo,
					colName = "PROCESSING_DATE"
				}
			);

			// -------------------------------------------------------------------------------------
			// 廃棄予定日
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.SCRAP_SCHEDULE_DATE,
				new CondType
				{
					enable = condEnable.SCRAP_SCHEDULE_DATE,
					switchable = true,
					id = "search_cond_scrap_schedule_date",
					type = CondType.SearchType.Text_FromTo,
					colName = "SCRAP_SCHEDULE_DATE"
				}
			);

			// -------------------------------------------------------------------------------------
			// 出荷先/返却元
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.SHIP_RETURN,
				new CondType
				{
					enable = condEnable.SHIPPER_RETURN,
					switchable = true,
					id = "search_cond_ship_return_code",
					type = CondType.SearchType.Text_AndOr,
					colName = "SHIP_RETURN_CODE"
				}
			);

			// -------------------------------------------------------------------------------------
			// 登録日
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.REGIST_DATE,
				new CondType
				{
					enable = condEnable.REGIST_DATE,
					switchable = true,
					id = "search_cond_regist_date",
					type = CondType.SearchType.Text_FromTo,
					colName = "REGIST_DATE"
				}
			);

			// -------------------------------------------------------------------------------------
			// お客様項目
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.CUSTOMER_ITEM,
				new CondType
				{
					enable = condEnable.CUSTOMER_ITEM,
					switchable = true,
					id = "search_cond_customer_item",
					type = CondType.SearchType.CustomerItem,
					colName = "CUSTOMER_ITEM"
				}
			);

			// -------------------------------------------------------------------------------------
			// 荷主項目
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.SHIPPER_NOTE,
				new CondType
				{
					enable = true,
					id = "search_cond_shipper_note",
					type = CondType.SearchType.Text_AndOr,
					colName = "SHIPPER_NOTE"
				}
			);

			#region // 検索条件(顧客専用項目)

			bool customerOnlyFlag = await userService.customerOnlyFlag();

			// -------------------------------------------------------------------------------------
			// ProjectNo
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.PROJECT_NO,
				new CondType
				{
					enable = customerOnlyFlag,
					id = "search_cond_project_no",
					type = CondType.SearchType.Text2_AndOr,
					colName = "PROJECT_NO"
				}
			);

			// -------------------------------------------------------------------------------------
			// 著作権
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.COPYRIGHT,
				new CondType
				{
					enable = customerOnlyFlag,
					id = "search_cond_copyright",
					type = CondType.SearchType.SelectText_AndOr,
					selects = new SortedDictionary<string, string>(
						domainService.getValueList(DOMAIN.Kind.STOCK_COPYRIGHT)
						.ToDictionary(e => e.CODE, e => e.VALUE)
					),
					colName = "COPYRIGHT"
				}
			);

			viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.COPYRIGHT].selects.Add("", "");

			// -------------------------------------------------------------------------------------
			// 契約書
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.CONTRACT,
				new CondType
				{
					enable = customerOnlyFlag,
					id = "search_cond_contract",
					type = CondType.SearchType.SelectText_AndOr,
					selects = new SortedDictionary<string, string>(
						domainService.getValueList(DOMAIN.Kind.STOCK_CONTRACT)
						.ToDictionary(e => e.CODE, e => e.VALUE)
					),
					colName = "CONTRACT"
				}
			);

			viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.CONTRACT].selects.Add("", "");

			// -------------------------------------------------------------------------------------
			// データNo
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.DATA_NO,
				new CondType
				{
					enable = customerOnlyFlag,
					id = "search_cond_data_no",
					type = CondType.SearchType.Text2_AndOr,
					colName = "DATA_NO"
				}
			);

			// -------------------------------------------------------------------------------------
			// 処理判定
			// -------------------------------------------------------------------------------------
			viewModel.searchCondition.dispSetting.Add(
				StockSearchService.ReadConfig.ConditionLabel.PROCESS_JUDGE,
				new CondType
				{
					enable = customerOnlyFlag,
					id = "search_cond_process_judge",
					type = CondType.SearchType.SelectText_AndOr,
					selects = new SortedDictionary<string, string>(
						domainService.getValueList(DOMAIN.Kind.STOCK_PROCESS_JUDGE)
						.ToDictionary(e => e.CODE, e => e.VALUE)
					),
					colName = "PROCESS_JUDGE"
				}
			);

			viewModel.searchCondition.dispSetting[StockSearchService.ReadConfig.ConditionLabel.PROCESS_JUDGE].selects.Add("", "");

			#endregion // 検索条件(顧客専用項目)

			#endregion // 検索条件

			// =====================================================================================
			// 表示設定
			// =====================================================================================
			viewModel.colOrders = displaySettingService.read(user.ID);

			return true;
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
            var user = await this.userService.read(principalConfig);

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


        /// <summary>
        /// アクションの準備
        /// </summary>
        /// <returns></returns>
        private async Task<USER_ACCOUNT> prepare()
		{
            await getStockDetailInfo();

            // サービス情報を更新
            updateService();

			// ステータスの値
			viewModel.statusDomains = domainService.getCodeList(DOMAIN.Kind.STCOK_STATUS).ToList();

			// ログインユーザー情報
			var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
			var user = await userService.read(principalConfig);

			return user;
		}


		#region // アクション

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// indexアクション
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnGetAsync()
		{
			// アクションの準備
			await prepare();

			// チェックボックスの内容を初期化
			await deleteCheckInfoAll();

			// 検索条件のセットアップ
			await SetupSearchCondition();

			return Page();
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostAsync(int? id)
        {
			// アクションの準備
			await prepare();

			// チェックボックス状態情報などをDBに保存する
			await formStateSave();

			//ページ指定が来ていたら取り込む
			this.paginationInfo.maxItems = stockSearchService.getStatistics().numbers;
            if (null != paginationInfo)
            {
                this.paginationInfo.movePage(paginationInfo.displayNextPage);
            }
            await updateViewData();
            return Page();
        }

		///////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索アクション
		/// </summary>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostListAsync()
		{
			// アクションの準備
			await prepare();

			// チェックボックス状態情報などをDBに保存する
			await formStateSave();

			// 検索実行
			await updateViewData();

			return Page();
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// クリアアクション
		/// </summary>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostClearAsync()
		{
			// アクションの準備
			await prepare();

			// formの値をクリア
			viewModel = new ViewModel();
			ModelState.Clear();

			// 検索条件をDBから読み直すことでクリアする
			await SetupSearchCondition();

			return Page();
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 印刷アクション
		/// </summary>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostPrintAsync()
		{
            // ログインユーザー情報
            var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await userService.read(principalConfig);

            ////レポートをエクスポートする
            //await Task.Run(() =>
            //{
            //    var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
            //        DateTimeOffset.Now, "yyyyMMddHHmmss");


            //    Utility.ReportHelper.HelperFunctions.writePdf
            //        (new Utility.ReportHelper.HelperFunctions.WriteConfig
            //        {
            //            fileName = $"Search_{nowStr}.pdf",
            //            report = new RazorPagesLearning.Report.SearchReport(
            //                new RazorPagesLearning.Report.SearchReport.ReportConfig
            //                {
            //                    targetInformation = makeReadQueryReport().ToList(),
            //                    wmsRequestHistoryDetailService = this.wmsRequestHistoryDetailService
            //                }),
            //            target = this.Response
            //        });
            //});

            // OnPostAsyncを実行して画面再描画
            return RedirectToPagePermanentPreserveMethod("Search");
		}

		/////////////////////////////////////////////////////////////////////////////////////////////
		///// <summary>
		///// EXCELアクション
		///// </summary>
		///// <returns></returns>
		/////////////////////////////////////////////////////////////////////////////////////////////
		//public async Task<IActionResult> OnPostExcelAsync()
		//{
  //          // ログインユーザー情報
  //          var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
  //          var user = await userService.read(principalConfig);

  //          //レポートをエクスポートする
  //          await Task.Run(() =>
  //          {
  //              var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
  //                  DateTimeOffset.Now, "yyyyMMddHHmmss");

  //              Utility.ReportHelper.HelperFunctions.writeExcel
  //                  (new Utility.ReportHelper.HelperFunctions.WriteConfig
  //                  {
  //                      fileName = $"Search_{nowStr}.xlsx",
  //                      report = new RazorPagesLearning.Report.SearchReportExcel(
  //                          new RazorPagesLearning.Report.SearchReportExcel.ReportConfigExcel
  //                          {
  //                              targetInformation = makeReadQueryReport().ToList(),
  //                              wmsRequestHistoryDetailService = this.wmsRequestHistoryDetailService
  //                          }),
  //                      target = this.Response
  //                  });
  //          });

  //          // OnPostAsyncを実行して画面再描画
  //          return RedirectToPagePermanentPreserveMethod("Search");
		//}

		/////////////////////////////////////////////////////////////////////////////////////////////
		///// <summary>
		///// CSVアクション
		///// </summary>
		///// <returns></returns>
		/////////////////////////////////////////////////////////////////////////////////////////////
		//public async Task<IActionResult> OnPostCsvAsync()
		//{
  //          // ログインユーザー情報
  //          var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
  //          var user = await userService.read(principalConfig);

  //          //レポートをエクスポートする
  //          await Task.Run(() =>
  //          {
  //              var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
  //                  DateTimeOffset.Now, "yyyyMMddHHmmss");

  //              Utility.ReportHelper.HelperFunctions.writeCsv
  //                  (new Utility.ReportHelper.HelperFunctions.WriteConfig
  //                  {
  //                      fileName = $"Search_{nowStr}.csv",
  //                      report = new RazorPagesLearning.Report.SearchReportExcel(
  //                          new RazorPagesLearning.Report.SearchReportExcel.ReportConfigExcel
  //                          {
  //                              targetInformation = makeReadQueryReport().ToList(),
  //                              wmsRequestHistoryDetailService = this.wmsRequestHistoryDetailService
  //                          }),
  //                      target = this.Response
  //                  });
  //          });

  //          // OnPostAsyncを実行して画面再描画
  //          return RedirectToPagePermanentPreserveMethod("Search");
		//}



		///////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 作業依頼追加アクション
		/// </summary>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostRequestAsync()
		{
			// アクションの準備
			var user = await prepare();

            // チェックボックス状態情報などをDBに保存する
			await formStateSave();

			// 選択された内容を取得
			var checkList = await getCheckInfo();

			// 作業依頼追加できるか確認
			var ok = new List<StockService.RequestableJudgement>();
			var ng = new List<StockService.RequestableJudgement>();
			foreach (var e in checkList)
			{
				var requestable = await stockService.isRequestable(e.originalDataId, user.ID);
				if (false == requestable.succeed)
				{
					ng.Add(requestable.result);
				}
				else
				{
					ok.Add(requestable.result);
				}
			}

			// 作業依頼に追加
			var addResult = await requestListService.add(ok.Select(e => e.stock.ID).ToList(), user.ID);

			// エラーCSV出力
			if (0 < ng.Count())
			{
                ////エラーメッセージをbodyに吐き出す
                //RazorPagesLearning.Utility.ReportHelper.HelperFunctions.writeErrorCsvFile
                //    ( new Utility.ReportHelper.HelperFunctions.ErrorCsvConfig {
                //        fileName = "error.csv",
                //        target = this.Response,
                //        writerOperation = new Action<System.IO.StreamWriter>( (csv)=> {

                //            // header
                //            csv.WriteLine("MWL在庫ID,倉庫管理番号,題名,エラー内容");

                //            // body
                //            foreach (var item in ng)
                //            {
                //                if (null != item.stock)
                //                {
                //                    csv.WriteLine($"{item.stock.ID},{item.stock.STORAGE_MANAGE_NUMBER},{item.stock.TITLE},{item.errorMessage}");
                //                }
                //                else
                //                {
                //                    csv.WriteLine($",,,{item.errorMessage}");
                //                }
                //            }
                //            csv.Flush();

                //        } )
                //    });
			}

			// 選択されたもののチェック状態はクリアしない
			//await deleteCheckInfoAll();

			// 処理結果メッセージを設定
			viewModel.resultMessage = $"{ok.Count()}件作業依頼追加しました。";

			// 検索実行
			await updateViewData();

			return Page();
		}

		///////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// ウォッチリスト追加アクション
		/// </summary>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostWatchlistAsync()
		{
			// アクションの準備
			var user = await prepare();

			// チェックボックス状態情報などをDBに保存する
			await formStateSave();

			// 選択された内容を取得
			var checkList = await getCheckInfo();
			var mwlStockIds = checkList.Select(e => e.originalDataId).ToList();

			// ウォッチリストに追加
			var addResult = await watchlistService.add(mwlStockIds, user.ID);
			var records = (List<WATCHLIST>)addResult.result;

			// 選択されたもののチェック状態はクリアしない
			//await deleteCheckInfoAll();

			// 処理結果メッセージを設定
			viewModel.resultMessage = $"{records.Count()}件ウォッチリスト追加しました。";

			// 検索実行
			await updateViewData();

			return Page();
		}

		#endregion // アクション
	}
}