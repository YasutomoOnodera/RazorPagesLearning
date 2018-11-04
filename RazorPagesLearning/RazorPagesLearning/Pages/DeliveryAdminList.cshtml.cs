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
using static RazorPagesLearning.Service.DB.DeliveryAdminService;
using static RazorPagesLearning.Service.User.UserService;

namespace RazorPagesLearning.Pages
{
	////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 集配先マスタ一覧ページモデル
	/// 
	/// [アクセス制限]
	/// ・運送会社だけ入れない
	/// 
	/// </summary>    
	////////////////////////////////////////////////////////////////////////////////////////////////
	[Authorize(Roles = "Admin,ShipperBrowsing,ShipperEditing,Worker", Policy = "PasswordExpiration")]
	public class DeliveryAdminListModel : PageModel
	{
		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="db"></param>
		/// <param name="signInManager"></param>
		/// <param name="userManager"></param>
		/// <param name="logger"></param>
		////////////////////////////////////////////////////////////////////////////////////////////
		public DeliveryAdminListModel(
			RazorPagesLearning.Data.RazorPagesLearningContext db,
			SignInManager<IdentityUser> signInManager,
			UserManager<IdentityUser> userManager,
			ILogger<LoginModel> logger)
		{
			// 各種サービス
			this.userService = new Service.User.UserService(db, User, signInManager, userManager);
			this.deliveryAdminService = new DeliveryAdminService(db, User, signInManager, userManager);
			this.deliveryDispFlagService = new DeriveryDispFlagService(db, User, signInManager, userManager);
			this.domainService = new DomainService(db, User, signInManager, userManager);
			this.wkTableSelectionSettingService = new WkTableSelectionSettingService(db, User, signInManager, userManager);

			// コンストラクタ引数
			this._signInManager = signInManager;
			this._userManager = userManager;
			this._logger = logger;

			// 画面に表示する項目
			this.paginationInfo = PaginationInfo.createInstance(0);
			this.viewModel = new ViewModel();
			this.checkStatusManagement = new DeliveryAdminCheckStatusManagement(
				db, signInManager, userManager, this.paginationInfo, User);
		}


		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 画面に表示する項目
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public class ViewModel : SelectableTableViewModelBase<DeliveryAdminSearchResult>
		{
			////////////////////////////////////////////////////////////////////////////////////////
			/// <summary>
			/// コンストラクタ
			/// </summary>
			////////////////////////////////////////////////////////////////////////////////////////
			public ViewModel() :
				base(ViewTableType.DeliveryAdmin)
			{
			}

			/// <summary>
			/// 社名
			/// </summary>
			public string company { get; set; }

			/// <summary>
			/// AND/OR
			/// 社名
			/// </summary>
			public ReadConfigBase.AndOr andOr { get; set; }

			/// <summary>
			/// 表示ON/OFF
			/// </summary>
			public string display { get; set; }

			/// <summary>
			/// セレクト項目
			/// 表示ON/OFF
			/// </summary>
			public List<SelectListItem> displays { get; set; }
		}


		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// チェック状態管理
		/// 画面上のテーブルにおけるチェックボックスのチェック状態を管理する
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		private class DeliveryAdminCheckStatusManagement : CheckStatusManagement<DeliveryAdminSearchResult>
		{
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
			public DeliveryAdminCheckStatusManagement(
				RazorPagesLearning.Data.RazorPagesLearningContext db,
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

			public override TableSelectionSettingService.TrackingIdentifier translationRelationIdentifier(DeliveryAdminSearchResult table)
			{
				return new TableSelectionSettingService.TrackingIdentifier
				{
					dataId = table.DELIVERY_ADMIN_ID
				};
			}
		}


		// コンストラクタ引数
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ILogger<LoginModel> _logger;

		// 各種サービス
		private readonly Service.User.UserService userService;
		private readonly Service.DB.DeliveryAdminService deliveryAdminService;
		private readonly Service.DB.DeriveryDispFlagService deliveryDispFlagService;
		private readonly Service.DB.DomainService domainService;
		private readonly Service.DB.WkTableSelectionSettingService wkTableSelectionSettingService;

		// 画面上のテーブルにおけるチェックボックスのチェック状態
		private DeliveryAdminCheckStatusManagement checkStatusManagement;


		#region 画面バインド情報

		/// <summary>
		/// 画面に表示する項目
		/// </summary>
		[BindProperty]
		public ViewModel viewModel { get; set; }

		/// <summary>
		/// ページネーション情報
		/// </summary>
		[BindProperty]
		public PaginationInfo paginationInfo { get; set; }

		#endregion // 画面バインド情報

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索してページを返す
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private async Task<IActionResult> searchAndReturnPage()
		{
			// サービス情報を更新
			await this.updateService();

			// チェックボックス状態情報などをDBに保存する
			await formStateSave();

			// 画面に表示する項目を作成
			this.mkViewModel();

			// 荷主が紐づくデータを検索
			await this.updateViewData();

			return Page();
			//return RedirectToAction("List");
			//return new RedirectToPageResult("index");
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 表示フラグを更新してページを返す
		/// </summary>
		/// <param name="displayFlag"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private async Task<IActionResult> chgDispAndReturnPage(bool displayFlag)
		{
			// サービス情報を更新
			var user = await this.updateService();

			// チェックボックス状態情報などをDBに保存する
			await formStateSave();

			// 選択された内容を取得
			var checkList = wkTableSelectionSettingService.getCheckStateList(user.ID, ViewTableType.DeliveryAdmin);

			// 表示フラグを更新
			var updateResult = await deliveryDispFlagService.update(checkList.Select(e => e.originalDataId).ToList(), user.ID, displayFlag);

			// 画面に表示する項目を作成
			this.mkViewModel();

			// 荷主が紐づくデータを検索
			await this.updateViewData();

			//return Page();
			return RedirectToAction("List");
			//return new RedirectToPageResult("index");
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// サービス情報を更新する
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private async Task<USER_ACCOUNT> updateService()
		{
			// PageModelのコンストラクタでは、ユーザーサービス情報は引き渡されない。
			// 実際にこれらの情報に触れるようになるのは、アクションメソッドが呼ばれたタイミングである。
			// このため、アクションメソッドが呼ばれたタイミングでユーザー情報を更新する
			this.userService.updateUser(User);
			this.deliveryAdminService.updateUser(User);
			this.deliveryDispFlagService.updateUser(User);
			this.domainService.updateUser(User);
			this.wkTableSelectionSettingService.updateUser(User);

			// ログインユーザー情報
			var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
			var user = await userService.read(principalConfig);
			return user;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// フォーム状態を保存する
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private async Task<bool> formStateSave()
		{
			// テーブルのチェック状態を記憶
			await this.checkStatusManagement.stateSave(
				new CheckStatusManagement<DeliveryAdminSearchResult>.StatePersistenceConfig
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
		/// 画面に表示する項目を作成
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		private void mkViewModel()
		{
			// 表示のセレクトボックス内容
			var domains = this.domainService.getValueList(DOMAIN.Kind.DELIVERY_DISPLAY).OrderBy(e => e.CODE);
			this.viewModel.displays =
				domains.Select(e => new SelectListItem
				{
					Value = e.CODE,
					Text = e.VALUE
				}).ToList();
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 画面上に表示されたデータ更新
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private async Task<bool> updateViewData()
		{
			// ログインユーザー情報
			var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
			var user = await userService.read(principalConfig);

			// 検索条件
			var conf = new DeliveryAdminService.ReadConfigBase
			{
				userAccountId = user.ID,
				company = this.viewModel.company,
				company_AndOr = this.viewModel.andOr,
				displayFlag = (this.viewModel.display == DOMAIN.DeliveryDisplayCode.BRANK) ?
							    DeliveryAdminService.ReadConfigBase.DisplayFlag.Na :
							  (this.viewModel.display == DOMAIN.DeliveryDisplayCode.ON) ?
							    DeliveryAdminService.ReadConfigBase.DisplayFlag.On :
							    DeliveryAdminService.ReadConfigBase.DisplayFlag.Off,
				sortOrder = this.viewModel.sortColumn,
				sortDirection = this.viewModel.sortDirection,
			};

			// ページネーション情報を設定
			paginationPreparation(conf);

			// TODO: 読み取り開始位置と、読み取りデータ数は、設定したページネーション情報より取得する？
			conf.start = this.paginationInfo.startViewItemIndex;
			conf.take = int.Parse(this.paginationInfo.displayNumber);


			// チェックボックスのチェック状態を復元しつつテーブルを読み込む
			// memo：ここで取得したデータ内容は、SelectableTableViewModelBase(viewModelの継承元)のtableRowsに格納されている
			return await this.checkStatusManagement.stateRestore(
				new CheckStatusManagement<DeliveryAdminSearchResult>.StateRestoreConfig
				{
					// 対象となるユーザーID
					userInfo = User,
					// データが格納されたビューモデル
					viewModel = this.viewModel,
					// ページネーション管理情報
					paginationInfo = this.paginationInfo,
					// DBから画面上のテーブルに表示する情報を読み取る関数
					readFunc = new Func<IQueryable<DeliveryAdminSearchResult>>(() =>
					{
						// 表示データを読み込む
						return this.deliveryAdminService.read(conf);
					})
				}
			);
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// ページネーション関連
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		private void paginationPreparation(DeliveryAdminService.ReadConfigBase conf)
		{
			// 取得するデータの最大件数を先に取得する
			this.paginationInfo.maxItems = this.deliveryAdminService.getStatistics(conf).numbers;

			// 最大件数を取得した上で、ページ情報を設定する
			this.paginationInfo.movePage(paginationInfo.displayNextPage);
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

			var ret = wkTableSelectionSettingService.getCheckStateList(user.ID, ViewTableType.DeliveryAdmin);

			return ret;
		}


		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// indexアクション
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnGetAsync()
		{
			// サービス情報を更新
			await this.updateService();

			// チェックボックス状態情報などをDBに保存する
			await formStateSave();

			// 画面に表示する項目を作成
			this.mkViewModel();

			return Page();
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// listアクション
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostListAsync()
		{
			// 検索してページを返す
			return await this.searchAndReturnPage();
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 既定のPOST要求を処理
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostAsync()
		{
			// 検索してページを返す
			return await this.searchAndReturnPage();
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// disponアクション
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostDisponAsync()
		{
			// 表示フラグを更新してページを返す
			return await this.chgDispAndReturnPage(true);
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// dispoffアクション
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostDispoffAsync()
		{
			// 表示フラグを更新してページを返す
			return await this.chgDispAndReturnPage(false);
		}
	}
}