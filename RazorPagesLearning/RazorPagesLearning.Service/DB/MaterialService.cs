using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
	////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 資材サービス
	/// 在庫のうち、資材について扱うサービス
	/// </summary>
	////////////////////////////////////////////////////////////////////////////////////////////////
	public class MaterialService : DBServiceBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ref_db"></param>
        /// <param name="ref_user"></param>
        /// <param name="ref_signInManager"></param>
        /// <param name="ref_userManager"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public MaterialService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 資材を検索する
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public List<STOCK> read()
        {
            var ret = db.STOCKs
				.Where(e => e.STOCK_KIND == DOMAIN.StockStockKindCode.MATERIAL)
				.Where(e => e.DELETE_FLAG == false)
				.OrderBy(e => e.STORAGE_MANAGE_NUMBER)
				.ToList();

            return ret;
        }

    }
}

