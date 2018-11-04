using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// ユーザー部課
    /// </summary>
    public class UserDepartmentService : DBServiceBase
    {

        public UserDepartmentService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }

        /// <summary>
        /// データを追加する
        /// 該当データがすでに登録済みの場合、登録済みのデータは無視してデータ追加を行う
        /// </summary>
        /// <param name="db"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public async Task<Result.DefaultExecutionResult> add(IEnumerable<USER_DEPARTMENT> target)
        {
            return await RazorPagesLearning.Service.DB.Helper.ServiceHelper
                .doOperationWithErrorManagementAsync(async (ret) =>
           {
               var tl = new List<USER_DEPARTMENT>();

               #region 存在していないコードを登録されているか
               foreach (var ele in target)
               {
                   //DB上に同一のユーザー部課情報が保存されているか確認する
                   var temporaryTarget = this.db.USER_DEPARTMENTs.Where(e =>
                     e.USER_ACCOUNT_ID == ele.USER_ACCOUNT_ID &&
                     e.SHIPPER_CODE == ele.SHIPPER_CODE &&
                     e.DEPARTMENT_CODE == ele.DEPARTMENT_CODE
                         ).FirstOrDefault();
                   if (null == temporaryTarget)
                   {
                       //DB上に存在しなければ、指定されたデータをDBに入れる
                       await this.setBothManagementInformation(ele);
                       tl.Add(ele);
                   }
               }
               #endregion

               //データ追加
               this.db.USER_DEPARTMENTs.AddRange(tl);

               //実行成功
               ret.succeed = true;

               return ret;

           });
        }

        /// <summary>
        /// ユーザーに属する部課情報の一覧を取得
        /// </summary>
        /// <param name="userAccountId"></param>
        /// <returns></returns>
        public List<USER_DEPARTMENT> readAtUserAccountId(long userAccountId)
        {
            return db.USER_DEPARTMENTs
                .Where(e => e.USER_ACCOUNT_ID == userAccountId)
                .Include(e => e.DEPARTMENT_ADMIN).ToList();


        }

		/// <summary>
		/// ユーザーアカウントID、荷主コードより、ユーザー部課を検索する
		/// </summary>
		/// <param name="userAccountId"></param>
		/// <param name="shipperCode"></param>
		/// <returns></returns>
		public Result.ExecutionResult<IQueryable<USER_DEPARTMENT>> read(Int64 userAccountId, string shipperCode)
		{
			return Helper.ServiceHelper.DoOperationWithErrorManagement<IQueryable<USER_DEPARTMENT>>((ret) =>
			{
				ret.result = db.USER_DEPARTMENTs.AsQueryable();
				ret.result = ret.result.Where(e => e.USER_ACCOUNT_ID == userAccountId);
				ret.result = ret.result.Where(e => e.SHIPPER_CODE == shipperCode);
				ret.succeed = true;
			});
		}
    }
}
