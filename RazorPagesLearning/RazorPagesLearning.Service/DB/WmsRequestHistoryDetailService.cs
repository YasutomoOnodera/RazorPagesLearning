// 手動追加 using
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

// 自動追加 using
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static RazorPagesLearning.Service.User.UserService;

namespace RazorPagesLearning.Service.DB
{
	/// <summary>
	/// WMS入出庫実績サービス
	/// </summary>
    public class WmsRequestHistoryDetailService : DBServiceBase
    {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="ref_db"></param>
		/// <param name="ref_user"></param>
		/// <param name="ref_signInManager"></param>
		/// <param name="ref_userManager"></param>
        public WmsRequestHistoryDetailService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }


        /// <summary>
        /// 作業依頼履歴詳細の取得
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public IQueryable<WMS_RESULT_HISTORY> readWmsRequestHistoryDetail(string storageNumber)
        {
            //WMS 作業依頼履歴詳細取得
            var wmsRequestHistoryDetail = db.WMS_RESULT_HISTORies
            .Where(e => e.STORAGE_MANAGE_NUMBER == storageNumber);

            return wmsRequestHistoryDetail;
        }



        /// <summary>
        /// 作業依頼履歴詳細の取得
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public async Task<IQueryable<WMS_RESULT_HISTORY>> read(Int64 stockId)
        {
			// ログインユーザー情報を確認
			var userService = new User.UserService(this.db, this.user, this.signInManager, this.userManager);
			var principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = this.user };
			var user = await userService.read(principalConfig);

			// 在庫情報を確認
			var stockService = new StockService(this.db, this.user, this.signInManager, this.userManager);
			var stock = stockService.readByStockId(stockId).result;

			IOrderedQueryable<WMS_RESULT_HISTORY> q = null;
			if (stock.STOCK_KIND == DOMAIN.StockStockKindCode.MATERIAL)
			{
				// 資材の場合は、作業依頼履歴の荷主コードも見る
				q = db.WMS_RESULT_HISTORies
					.AsQueryable()
					.Include(e => e.REQUEST_HISTORY_DETAIL)
					.ThenInclude(e => e.REQUEST_HISTORY)
					.Where(e => e.STOCK_ID == stockId)
					.Where(e => e.REQUEST_HISTORY_DETAIL.REQUEST_HISTORY.OWNER_SHIPPER_CODE == user.CURRENT_SHIPPER_CODE)
					.OrderBy(e => e.ID);
			}
			else
			{
				q = db.WMS_RESULT_HISTORies
					.AsQueryable()
					.Where(e => e.STOCK_ID == stockId)
					.OrderBy(e => e.ID);
			}

			return q;
        }

    }
}
