﻿// 手動追加 using
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

namespace RazorPagesLearning.Pages
{
    public class SelectDeriveryModel : PageModel
    {

#region 画面とバインディングする情報

        /// <summary>
        /// 検索文字列
        /// </summary>
        [BindProperty]
        [Required]
        public string mSearchStr { get; set; }

        /// <summary>
        /// 検索方法ANDまたはOR
        /// </summary>
        [BindProperty]
        [Required]
        public string mSearchAndOr { get; set; }

		/*
                /// <summary>
                /// エラー一覧
                /// </summary>
                public List<string> errorList { get; set; }
        */

		#endregion


#if false // 2018/08/16 M.Hoshino del DBマイグレーション
		#region WMS集配先情報

		/// <summary>
		/// WMS集配先情報
		/// </summary>
		[BindProperty]
        public IList<WMS_DERIVERY_ADMIN> WMS_DERIVERY_ADMINs { get; set; }

		#endregion
#endif // 2018/08/16 M.Hoshino del DBマイグレーション

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

/*
            //内部変数の初期化
            this.errorList = new List<string>();

            //テスト用
            {
                userName = "MWLSystemAdmin";
                password = "MWLSystemAdmin2018";
            }
*/
        }

        /// <summary>
        /// WMS集配先マスタを検索する
        /// </summary>
        public void OnPostSearch(string page)
        {
			throw new NotImplementedException("2018/08/16 M.Hoshino del DBマイグレーション");
#if false // 2018/08/16 M.Hoshino del DBマイグレーション
			// WMS集配先マスタ検索
			var wReadWmsDeriveryAdminService = RazorPagesLearning.Service.DB.WmsDeriveryAdminService.Search(
                _db,
                new Service.DB.WmsDeriveryAdminService.SearchConfig
                {
                    mSearchStr = this.mSearchStr,
                    mSearchAndOr = this.mSearchAndOr
                });
            if (true == wReadWmsDeriveryAdminService.succeed)
            {
                this.WMS_DERIVERY_ADMINs = wReadWmsDeriveryAdminService.result.ToList();
            }
#endif // 2018/08/16 M.Hoshino del DBマイグレーション
		}
	}
}
