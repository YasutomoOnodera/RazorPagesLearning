using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
	public class WkDeliveryService : DBServiceBase
	{
		public WkDeliveryService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
			ClaimsPrincipal ref_user,
			SignInManager<IdentityUser> ref_signInManager,
			UserManager<IdentityUser> ref_userManager)
			: base(ref_db, ref_user, ref_signInManager, ref_userManager)
		{
		}

		/// <summary>
		/// 読み取り設定
		/// </summary>
		public class ReadConfig
		{
			/// <summary>
			/// 荷主コード
			/// </summary>
			public Int64 WK_REQUEST_ID;

		}

		/// <summary>
		/// データを読み取り
		/// </summary>
		/// <param name="readConfig"></param>
		/// <returns></returns>
		public Result.ExecutionResult<WK_REQUEST_DELIVERY> read(ReadConfig readConfig)
		{
			return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<WK_REQUEST_DELIVERY>((ret) =>
			{
				ret.result = null;
				ret.result = db.WK_REQUEST_DELIVERies
					.Include(e => e.DELIVERY_ADMIN)
					.Include(e => e.WK_REQUEST)
                    .Include(e => e.DELIVERY_ADMIN.SHIPPER_ADMIN)
                    .Include(e => e.DELIVERY_ADMIN.SHIPPER_ADMIN.DEPARTMENT_ADMINs)
                    .Where(e => e.WK_REQUEST_ID == readConfig.WK_REQUEST_ID)
                    .FirstOrDefault();
				ret.succeed = true;

			});
		}

        /// <summary>
        /// 集配先マスタより集配先ワークの情報を生成する
        /// </summary>
        /// <param name="deliveryAdminId">集配先マスタID</param>
        /// <param name="wkRequestId">作業依頼ワークID</param>
        /// <returns></returns>
        public async Task addFromDeliveryAdmin(long deliveryAdminId, long wkRequestId)
        {
            var user = readLoggedUserInfo();

            var deliveryAdmin = this.db.DELIVERY_ADMINs.FirstOrDefault(e => e.ID == deliveryAdminId);

            var tmp = this.db.WK_REQUEST_DELIVERies.FirstOrDefault(e => e.WK_REQUEST_ID == wkRequestId);
            // 存在チェック
            if (tmp == null)
            {
                // 追加
                WK_REQUEST_DELIVERY wkDelivery = new WK_REQUEST_DELIVERY();
                wkDelivery.WK_REQUEST_ID = wkRequestId;
                wkDelivery.DELIVERY_ADMIN_ID = deliveryAdminId;
                wkDelivery.USER_ACCOUNT_ID = user.Result.ID;
                wkDelivery.DELIVERY_DEPARTMENT = deliveryAdmin.DEPARTMENT;
                wkDelivery.DELIVERY_CHARGE = deliveryAdmin.CHARGE_NAME;
                wkDelivery.FLIGHT = deliveryAdmin.DEFAULT_FLIGHT_CODE;

                await setCreateManagementInformation(wkDelivery);
                await setUpdateManagementInformation(wkDelivery);

                this.db.WK_REQUEST_DELIVERies.Add(wkDelivery);
            }
            else
            {
                // 更新
                tmp.WK_REQUEST_ID = wkRequestId;
                tmp.DELIVERY_ADMIN_ID = deliveryAdminId;
                tmp.USER_ACCOUNT_ID = user.Result.ID;
                tmp.DELIVERY_DEPARTMENT = deliveryAdmin.DEPARTMENT;
                tmp.DELIVERY_CHARGE = deliveryAdmin.CHARGE_NAME;
                tmp.FLIGHT = deliveryAdmin.DEFAULT_FLIGHT_CODE;

                await setUpdateManagementInformation(tmp);
            }

            await this.db.SaveChangesAsync();
        }

        /// <summary>
        /// 集配先マスタより集配先ワークの情報を生成する
        /// </summary>
        /// <param name="destCodes">集配先コード</param>
        /// <param name="wkRequestId">作業依頼ワークID</param>
        /// <returns></returns>
        public async Task addFromDeliveryAdmin(string destCode, long wkRequestId)
        {
            var user = readLoggedUserInfo();

            var deliveryAdmin = this.db.DELIVERY_ADMINs.FirstOrDefault(e => e.DELIVERY_CODE == destCode);

            var tmp = this.db.WK_REQUEST_DELIVERies.FirstOrDefault(e => e.WK_REQUEST_ID == wkRequestId);
            // 存在チェック
            if (tmp == null)
            {
                // 追加
                WK_REQUEST_DELIVERY wkDelivery = new WK_REQUEST_DELIVERY();
                wkDelivery.WK_REQUEST_ID = wkRequestId;
                wkDelivery.DELIVERY_ADMIN_ID = deliveryAdmin.ID;
                wkDelivery.USER_ACCOUNT_ID = user.Result.ID;
                wkDelivery.DELIVERY_DEPARTMENT = deliveryAdmin.DEPARTMENT;
                wkDelivery.DELIVERY_CHARGE = deliveryAdmin.CHARGE_NAME;

                await setCreateManagementInformation(wkDelivery);
                await setUpdateManagementInformation(wkDelivery);

                this.db.WK_REQUEST_DELIVERies.Add(wkDelivery);
            }
            else
            {
                // 更新
                tmp.WK_REQUEST_ID = wkRequestId;
                tmp.DELIVERY_ADMIN_ID = deliveryAdmin.ID;
                tmp.USER_ACCOUNT_ID = user.Result.ID;
                tmp.DELIVERY_DEPARTMENT = deliveryAdmin.DEPARTMENT;
                tmp.DELIVERY_CHARGE = deliveryAdmin.CHARGE_NAME;

                await setUpdateManagementInformation(tmp);
            }

            await this.db.SaveChangesAsync();
        }

        /// <summary>
        /// 集配先ワークの情報を更新する
        /// </summary>
        /// <param name="wkDelivery">更新情報</param>
        /// <param name="wkRequestId">作業依頼ワークID</param>
        /// <returns></returns>
        public async Task add(WK_REQUEST_DELIVERY wkDelivery, long wkRequestId)
        {
            var tmp = this.db.WK_REQUEST_DELIVERies.FirstOrDefault(e => e.WK_REQUEST_ID == wkRequestId);

            // 最新の情報に上書き
            tmp.DELIVERY_DEPARTMENT = wkDelivery.DELIVERY_DEPARTMENT;
            tmp.DELIVERY_CHARGE = wkDelivery.DELIVERY_CHARGE;
            tmp.SPECIFIED_DATE = wkDelivery.SPECIFIED_DATE;
            tmp.SPECIFIED_TIME = wkDelivery.SPECIFIED_TIME;
            tmp.FLIGHT = wkDelivery.FLIGHT;
            tmp.COMMENT = wkDelivery.COMMENT;

            await setUpdateManagementInformation(tmp);

            await this.db.SaveChangesAsync();
        }

    }
}

