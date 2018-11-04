using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using static RazorPagesLearning.Service.User.UserService;

namespace RazorPagesLearning.Pages
{
	////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 集配先マスタ新規登録ページモデル
	/// 
	/// [アクセス制限]
	/// ・運送会社だけ入れない
	/// 
	/// </summary>    
	////////////////////////////////////////////////////////////////////////////////////////////////
	[Authorize(Roles = "Admin,ShipperBrowsing,ShipperEditing,Worker", Policy = "PasswordExpiration")]
	public class DeliveryAdminEditModel : PageModel
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
		public DeliveryAdminEditModel(
			RazorPagesLearning.Data.RazorPagesLearningContext db,
			SignInManager<IdentityUser> signInManager,
			UserManager<IdentityUser> userManager,
			ILogger<LoginModel> logger)
		{
			// 各種サービス
			this.userService = new Service.User.UserService(db, User, signInManager, userManager);
			this.deliveryAdminService = new Service.DB.DeliveryAdminService(db, User, signInManager, userManager);
			this.domainService = new Service.DB.DomainService(db, User, signInManager, userManager);

			// コンストラクタ引数
			this._signInManager = signInManager;
			this._userManager = userManager;
			this._logger = logger;

			// 画面に表示する項目
			this.viewModel = new ViewModel();
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 画面に表示する項目
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public class ViewModel
		{
			////////////////////////////////////////////////////////////////////////////////////////
			/// <summary>
			/// コンストラクタ
			/// </summary>
			////////////////////////////////////////////////////////////////////////////////////////
			public ViewModel()
			{
			}

			/// <summary>
			/// 集配先マスタ
			/// </summary>
			public DELIVERY_ADMIN DELIVERY_ADMIN { get; set; }

			/// <summary>
			/// セレクト項目
			/// 便
			/// </summary>
			public List<SelectListItem> flights { get; set; }

			/// <summary>
			/// モード情報
			/// </summary>
			public Mode mode { get; set; }

			/// <summary>
			/// エラーメッセージ
			/// </summary>
			public List<string> errorMessage { get; set; }
		}

		/// <summary>
		/// モード情報
		/// </summary>
		public enum Mode
		{
			/// <summary>
			/// 新規登録
			/// </summary>
			Create = 0,

			/// <summary>
			/// 照会
			/// </summary>
			Update = 1
		}

		// コンストラクタ引数
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ILogger<LoginModel> _logger;

		// 各種サービス
		private readonly Service.User.UserService userService;
		private readonly Service.DB.DeliveryAdminService deliveryAdminService;
		private readonly Service.DB.DomainService domainService;


		/// <summary>
		/// 画面に表示する項目
		/// </summary>
		[BindProperty]
		public ViewModel viewModel { get; set; }

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
			this.domainService.updateUser(User);

			// ログインユーザー情報
			var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
			var user = await userService.read(principalConfig);
			return user;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 表示情報を初期化する
		/// </summary>
		/// <param name="deliveryAdminId"></param>
		////////////////////////////////////////////////////////////////////////////////////////////
		private void readViewDataFromDB(Int64? deliveryAdminId)
		{
			try
			{
				// 便のセレクトボックス内容
				var domains = this.domainService.getValueList(DOMAIN.Kind.DELIVERY_FLIGHT).OrderBy(e => e.CODE);
				this.viewModel.flights =
					domains.Select(e => new SelectListItem
					{
						Value = e.CODE,
						Text = e.VALUE
					}).ToList();

				// 登録されている集配先マスタ情報
				if (null != deliveryAdminId)
				{
					this.viewModel.DELIVERY_ADMIN = this.deliveryAdminService.readById((Int64)deliveryAdminId).result;
				}
				else
				{
					// 初期値はRazorPagesLearningを選択状態とする
					this.viewModel.DELIVERY_ADMIN = new DELIVERY_ADMIN
					{
						DEFAULT_FLIGHT_CODE = DOMAIN.DeliverlyFlightCode.RazorPagesLearning
					};
				}
			}
			catch (Exception e)
			{
				this.viewModel.errorMessage = new List<string> { string.Join(",", e.Message.ToString()) };
			}
		}


		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// indexアクション
		/// </summary>
		/// <param name="id"></param>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnGetAsync(Int64? id)
        {
			this.viewModel.mode = (null == id) ? Mode.Create : Mode.Update;

			// サービス情報を更新
			var user = await updateService();

			// 表示情報を初期化
			this.readViewDataFromDB(id);

			return Page();
        }

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// createアクション
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostCreateAsync()
		{
			// サービス情報を更新
			var user = await updateService();

			// 集配先マスタ情報を登録
			await this.deliveryAdminService.add(this.viewModel.DELIVERY_ADMIN, user);

			return RedirectToPage("/DeliveryAdminList");
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// updateアクション
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<IActionResult> OnPostUpdateAsync()
		{
			// サービス情報を更新
			var user = await updateService();

			// 集配先マスタ情報を更新
			await this.deliveryAdminService.update(this.viewModel.DELIVERY_ADMIN);

			return RedirectToPage("/DeliveryAdminList");
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// backアクション
		/// </summary>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public IActionResult OnPostBackAsync()
		{
			return RedirectToPage("/DeliveryAdminList");
		}
	}
}