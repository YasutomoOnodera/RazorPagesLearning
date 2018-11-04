// 手動追加 using
#if false // 2018/08/16 M.Hoshino del DBマイグレーション
using static RazorPagesLearning.Data.Models.WMS_DERIVERY_ADMIN;
#endif // 2018/08/16 M.Hoshino del DBマイグレーション
using RazorPagesLearning.Data.Models;

// 自動追加 using
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RazorPagesLearning.Pages.PopUp
{
    public class SelectDeriveryModel 
    {
        /// <summary>
        /// 表示用のビューモデル
        /// </summary>
        public class ViewModel
        {
            /// <summary>
            /// 検索で使用する会社名
            /// </summary>
            [BindProperty]
            public string companyName { get; set; }

            /// <summary>
            /// モーダル画面を表示中か
            /// </summary>
            [BindProperty]
            public bool isShowModal { get; set; }

#if false // 2018/08/16 M.Hoshino del DBマイグレーション
            /// <summary>
            /// ユーザーアカウント情報
            /// </summary>
            [BindProperty]
            public IList<WMS_DERIVERY_ADMIN> WMS_DERIVERY_ADMINs { get; set; }

            public ViewModel()
            {
                this.companyName = "";
                this.WMS_DERIVERY_ADMINs = new List<WMS_DERIVERY_ADMIN>();
                isShowModal = false;
            }
#else // 2018/08/16 M.Hoshino del DBマイグレーション
			/// <summary>
			/// 集配先マスタ
			/// </summary>
			[BindProperty]
			public IList<DELIVERY_ADMIN> DELIVERY_ADMINs { get; set; }

			public ViewModel()
			{
				this.companyName = "";
				this.DELIVERY_ADMINs = new List<DELIVERY_ADMIN>();
				isShowModal = false;
			}
#endif // 2018/08/16 M.Hoshino del DBマイグレーション

            /// <summary>
            /// Json通信する用
            /// </summary>
			[BindProperty]
            public JsonModel.ViewModel json { get; set; }

		}

		//コンストラクタ引数
		private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        /// <summary>
        /// DBアクセスオブジェクト
        /// </summary>
        private readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;

        public SelectDeriveryModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, RazorPagesLearning.Data.RazorPagesLearningContext db)
        {
            //DI情報の取り込み
            this._signInManager = signInManager;
            this._logger = logger;
            this._db = db;
        }

        /// <summary>
        /// 集配先マスタを検索する
        /// </summary>
        public void OnPostSearch(ViewModel viewModel)
        {





		}
	}
}
