using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RazorPagesLearning.Service.User.UserService;

namespace RazorPagesLearning.Pages
{
	/// <summary>
	/// 資材販売モデル
	/// </summary>
	public class MaterialModel : PageModel
    {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="db"></param>
		/// <param name="signInManager"></param>
		/// <param name="userManager"></param>
		/// <param name="logger"></param>
		public MaterialModel(RazorPagesLearning.Data.RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager,
			UserManager<IdentityUser> userManager,
			ILogger<LoginModel> logger)
		{
			this._db = db;
			this._signInManager = signInManager;
			this._userManager = userManager;
			this._logger = logger;

			this.userService = new Service.User.UserService(_db, User, _signInManager, _userManager);
			this.materialService = new Service.DB.MaterialService(_db, User, _signInManager, _userManager);
			this.domainService = new Service.DB.DomainService(_db, User, _signInManager, _userManager);
			this.wkRequestService = new Service.DB.WkRequestService(db, User, signInManager, userManager);

			// viewモデルの更新
			this.viewModel = new ViewModel();
		}

		/// <summary>
		/// 画面上に表示すべき情報
		/// </summary>
		public class ViewModel
        {
			/// <summary>
			/// コンストラクタ
			/// </summary>
            public ViewModel()
            {
                this.errorList = new List<string>();
            }
            #region 表示対象データ

			/// <summary>
			/// 在庫
			/// </summary>
            public List<STOCK> STOCKs { get; set; }

            #endregion // 表示対象データ
            /// <summary>
            /// エラー一覧
            /// </summary>
            public List<string> errorList { get; set; }
            #region 更新対象データ

			/// <summary>
			/// 作業依頼詳細ワーク
			/// </summary>
			public List<WK_REQUEST_DETAIL> WK_REQUEST_DETAILs { get; set; }

            #endregion // 更新対象データ
        }

        /// <summary>
        /// 画面上の表示項目と同期する情報
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

		/// <summary>
		/// ユーザー関係のサービス
		/// </summary>
		private readonly Service.User.UserService userService;

		/// <summary>
		/// 資材サービス
		/// </summary>
		private readonly Service.DB.MaterialService materialService;

		/// <summary>
		/// ドメインサービス
		/// </summary>
		private readonly Service.DB.DomainService domainService;

		/// <summary>
		/// 作業依頼一覧サービス
		/// </summary>
		private readonly WkRequestService wkRequestService;

		/// <summary>
		/// サービス情報を更新する
		/// </summary>
		private void updateService()
		{
			// PageModelのコンストラクタでは、ユーザーサービス情報は引き渡されない。
			// 実際にこれらの情報に触れるようになるのは、アクションメソッドが呼ばれたタイミングである。
			// このため、アクションメソッドが呼ばれたタイミングでユーザー情報を更新する
			this.userService.updateUser(User);
			this.materialService.updateUser(User);
			this.domainService.updateUser(User);
		}

		/// <summary>
		/// 表示情報を初期化する
		/// </summary>
		private void readViewDataFromDB()
		{
			// 在庫から、資材の一覧を検索する
			this.viewModel.STOCKs = materialService.read();

			// 資材の一覧から、作業依頼詳細ワークのひな型を作る
			this.viewModel.WK_REQUEST_DETAILs =
				this.viewModel.STOCKs.Select(e => new WK_REQUEST_DETAIL
				{
					STOCK_ID = e.ID
				}).ToList();
		}

		/// <summary>
		/// indexアクション
		/// </summary>
		public IActionResult OnGetAsync()
        {
			this.viewModel.STOCKs = materialService.read();

			return Page();
        }

        /// <summary>
		/// アクションの準備
		/// </summary>
		/// <returns></returns>
		private async Task<USER_ACCOUNT> prepare()
        {
            // サービス情報を更新
            updateService();

            // ログインユーザー情報
            var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await userService.read(principalConfig);

            return user;
        }

		/// <summary>
		/// confirmアクション
		/// </summary>
        /// <returns></returns>
		public async Task<IActionResult> OnPostConfirm()
        {
            // ユーザー情報を取得
            var user = await prepare();

            // foreach内で使用するカウンタ
            int i = 0;

            // 発注数が在庫数を超えていた場合、エラーメッセージをリストに格納
            foreach (var detail in viewModel.WK_REQUEST_DETAILs)
            { 
                if (viewModel.STOCKs[i].STOCK_COUNT < detail.REQUEST_COUNT)
                {
                    this.viewModel.errorList.Add($"{viewModel.STOCKs[i].TITLE}の発注数が在庫数を超えています。");
                }
                i++;
            }

            if (0 != viewModel.errorList.Count)
            {
                //エラー発生時
                goto ErrorEnd;
            }

            var addResult = await wkRequestService.add(
				this.viewModel.WK_REQUEST_DETAILs.Where(e => e.REQUEST_COUNT > 0).ToList(), user.ID);
			
            // データベースの追加に失敗判定
            if (addResult.succeed == false)
            {
                this.viewModel.errorList.Add("データベースへの登録に失敗しました");

                goto ErrorEnd;
            }
			
			// 表示情報を初期化する
			readViewDataFromDB();

			// 集配先選択画面へ遷移する
			var requestKind = domainService.read(new DomainService.ReadConfig
			{
				KIND = DOMAIN.Kind.REQUEST_REQUEST,
				CODE = DOMAIN.RequestRequestCode.MATERIAL
			});

			return RedirectToPage("/RequestDelivery", new
			{
				requestKind = requestKind.result.CODE,
				wkRequestId = addResult.result
			});


			// エラー発生時
			ErrorEnd:;

            this.viewModel.STOCKs = materialService.read();

			return Page();
		}
	}
}