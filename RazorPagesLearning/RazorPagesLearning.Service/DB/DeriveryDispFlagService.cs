using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace RazorPagesLearning.Service.DB
{
	/// ////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 集配先表示フラグマスタサービス
	/// </summary>
	/// ////////////////////////////////////////////////////////////////////////////////////////////
	public class DeriveryDispFlagService : DBServiceBase
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
		public DeriveryDispFlagService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
			ClaimsPrincipal ref_user,
			SignInManager<IdentityUser> ref_signInManager,
			UserManager<IdentityUser> ref_userManager)
			: base(ref_db, ref_user, ref_signInManager, ref_userManager)
		{
		}

		/// ////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 読み取り条件
		/// </summary>
		/// ////////////////////////////////////////////////////////////////////////////////////////
		public class ReadConfig
		{
			/// <summary>
			/// 集配先マスタID
			/// </summary>
			public Int64 deliveryAdminId;

			/// <summary>
			/// ユーザーアカウントID
			/// </summary>
			public Int64 userAccountId;
		}

		/// ////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 指定された集配先表示フラグマスタを読み取る
		/// </summary>
		/// <param name="db"></param>
		/// <param name="readConfig"></param>
		/// <returns></returns>
		/// ////////////////////////////////////////////////////////////////////////////////////////
		public Result.ExecutionResult<IQueryable<USER_DELIVERY>> read(ReadConfig conf)
		{
			return Helper.ServiceHelper.DoOperationWithErrorManagement<IQueryable<USER_DELIVERY>>((ret) =>
			{
				ret.result = db.USER_DELIVERies
								.Where(e => e.DELIVERY_ADMIN_ID == conf.deliveryAdminId)
								.Where(e => e.USER_ACCOUNT_ID == conf.userAccountId);
				ret.succeed = true;
			});
		}

		/// ////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 集配先表示フラグマスタを追加する
		/// </summary>
		/// <param name="db"></param>
		/// <param name="target"></param>
		/// ////////////////////////////////////////////////////////////////////////////////////////
		public void add(IEnumerable<USER_DELIVERY> inTarget)
		{
			db.AddRange(inTarget);
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 表示フラグを更新
		/// </summary>
		/// <param name="deliveryAdminIds"></param>
		/// <param name="userAccountId"></param>
		/// <param name="displayFlag"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<Result.DefaultExecutionResult> update(List<Int64> deliveryAdminIds, Int64 userAccountId, bool displayFlag)
		{
			async Task<Result.DefaultExecutionResult> task(Int64 deliveryAdminId)
			{
				// 集配先表示フラグを探す
				var wddf = read(new ReadConfig
				{
					deliveryAdminId = deliveryAdminId,
					userAccountId = userAccountId
				});

				if (false == wddf.succeed || 0 == wddf.result.Count())
				{
					throw new ApplicationException($"存在しない集配先マスタが指定されました。");
				}

				// 更新する
				var ddf = (USER_DELIVERY)wddf.result.First();
                // ModelChange:表示フラグ削除。表示させるデータのみDBに持たせるようにテーブルの使い方を変更
				//ddf.DISPLAY_FLAG = displayFlag;
				await setUpdateManagementInformation(ddf);

				// DBに登録
				db.USER_DELIVERies.Update(ddf);
				db.SaveChanges();

				return new Result.DefaultExecutionResult
				{
					succeed = true,
					result = ddf,
				};
			}

			try
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					// await Task.WhenAll()で、deliveryAdminIdsに対する全Taskが終わるまで待つ
					await Task.WhenAll(
						deliveryAdminIds.Select(async deliveryAdminId =>
						{
							var ret = await task(deliveryAdminId);
							if (false == ret.succeed)
							{
								throw new ApplicationException(string.Join(",", ret.errorMessages.ToString()));
							}
						})
					);

					// DB保存する
					transaction.Commit();

					return new Result.DefaultExecutionResult
					{
						succeed = true
					};
				}
			}
			catch (Exception e)
			{
				return Result.DefaultExecutionResult.makeError(e.Message);
			}
		}
	}
}
