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
	/// 検索条件サービス
	/// </summary>
	////////////////////////////////////////////////////////////////////////////////////////////////
	public class SearchConditionService :DBServiceBase
	{
		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="ref_db"></param>
		/// <param name="ref_user"></param>
		/// <param name="ref_signInManager"></param>
		/// <param name="ref_userManager"></param>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public SearchConditionService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
			ClaimsPrincipal ref_user,
			SignInManager<IdentityUser> ref_signInManager,
			UserManager<IdentityUser> ref_userManager)
			: base(ref_db, ref_user, ref_signInManager, ref_userManager)
		{
			this.db = ref_db;
			/*
            this.user = ref_user;
            this.signInManager = ref_signInManager;
            this.userManager = ref_userManager;*/
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件を検索
		/// </summary>
		/// <param name="userAccountId"></param>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<Result.DefaultExecutionResult> Read(Int64 userAccountId)
		{
			try
			{
				var record = db.USER_SEARCH_CONDITIONs
					.AsQueryable()
					.Where(e => e.USER_ACCOUNT_ID == userAccountId)
					.FirstOrDefault() ?? throw new ApplicationException("検索条件がありません。");

				return new Result.DefaultExecutionResult
				{
					succeed = true,
					result = record
				};
			}
			catch (Exception e)
			{
				return Result.DefaultExecutionResult.makeError(e.Message);
			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件を追加
		/// </summary>
		/// <param name="userAccountId"></param>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<Result.DefaultExecutionResult> Add(Int64 userAccountId)
		{
			return await RazorPagesLearning.Service.DB.Helper.ServiceHelper.doOperationWithErrorManagementAsync(async (ret) =>
			{
				var record = new USER_SEARCH_CONDITION
				{
					USER_ACCOUNT_ID = userAccountId,
					CLASS1 = false,
					CLASS2 = false,
					STORAGE_MANAGE_NUMBER = false,
					REMARK1 = false,
					REMARK2 = false,
					NOTE = false,
					PRODUCT_DATE = false,
					STORAGE_DATE = false,
					PROCESSING_DATE = false,
					SCRAP_SCHEDULE_DATE = false,
					SHIPPER_RETURN = false,
					REGIST_DATE = false,
					CUSTOMER_ITEM = false
				};
				await setBothManagementInformation(record);

				// DBに登録
				db.USER_SEARCH_CONDITIONs.Add(record);

				// 実行成功
				ret.succeed = true;

				return ret;
			});
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件を更新
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<Result.DefaultExecutionResult> Update(USER_SEARCH_CONDITION record)
		{
			try
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					// 一旦DBから読み込む
					var r = (USER_SEARCH_CONDITION)(await Read(record.USER_ACCOUNT_ID)).result;
					r.CLASS1 = record.CLASS1;
					r.CLASS2 = record.CLASS2;
					r.STORAGE_MANAGE_NUMBER = record.STORAGE_MANAGE_NUMBER;
					r.REMARK1 = record.REMARK1;
					r.REMARK2 = record.REMARK2;
					r.NOTE = record.NOTE;
					r.PRODUCT_DATE = record.PRODUCT_DATE;
					r.STORAGE_DATE = record.STORAGE_DATE;
					r.PROCESSING_DATE = record.PROCESSING_DATE;
					r.SCRAP_SCHEDULE_DATE = record.SCRAP_SCHEDULE_DATE;
					r.SHIPPER_RETURN = record.SHIPPER_RETURN;
					r.REGIST_DATE = record.REGIST_DATE;
					r.CUSTOMER_ITEM = record.CUSTOMER_ITEM;
					await setUpdateManagementInformation(r);

					// DBに反映
					db.USER_SEARCH_CONDITIONs.Update(r);
					db.SaveChanges();
					transaction.Commit();

					return new Result.DefaultExecutionResult
					{
						succeed = true,
						result = r
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
