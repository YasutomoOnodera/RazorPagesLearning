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
	///////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 表示設定サービス
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////
	public class DisplaySettingService : DBServiceBase
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
		public DisplaySettingService(
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
		/// 追加時のコンテキスト
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public class ChangeConfig
		{
			/// <summary>
			/// 管理対象テーブル
			/// </summary>
			public USER_DISPLAY_SETTING DISPLAY_SETTING;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 表示設定を検索
		/// </summary>
		/// <param name="userAccountId"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public List<USER_DISPLAY_SETTING> read(Int64 userAccountId)
		{
			var records = db.USER_DISPLAY_SETTINGs
				.AsQueryable()
				.Where(e => e.USER_ACCOUNT_ID == userAccountId)
				.OrderBy(e => e.DEFAULT_ORDER)
				.ToList();

			return records;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 表示設定を検索
		/// </summary>
		/// <param name="userAccountId"></param>
		/// <param name="screenId"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public List<USER_DISPLAY_SETTING> read(Int64 userAccountId, USER_DISPLAY_SETTING.SCREEN screenId)
		{
			var records = db.USER_DISPLAY_SETTINGs
				.AsQueryable()
				.Where(e => e.SCREEN_ID == screenId)
				.Where(e => e.USER_ACCOUNT_ID == userAccountId)
				.ToList();

			return records;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 表示設定を追加
		/// </summary>
		/// <param name="arg"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<Result.DefaultExecutionResult> add(Int64 userAccountId)
		{
			return await RazorPagesLearning.Service.DB.Helper.ServiceHelper.doOperationWithErrorManagementAsync(async (ret) =>
			{
				var records = new List<USER_DISPLAY_SETTING>();

				// 列名
				var colNames = new Service.Utility.Generic.OrderedDictionary<string, string>
				{
					["STORAGE_MANAGE_NUMBER"] = "倉庫管理番号",
					["STATUS"] = "ステータス",
					["TITLE"] = "題名",
					["SUBTITLE"] = "副題",
					["NOTE"] = "備考",
					["SHIPPER_NOTE"] = "荷主項目",
					["CUSTOMER_MANAGE_NUMBER"] = "お客様管理番号",
					["PROCESSING_DATE"] = "処理日",
					["SHAPE"] = "形状",
					["REMARK1"] = "Remark1",
					["REMARK2"] = "Remark2",
					["STOCK_COUNT"] = "在庫数"
				};

				foreach (var (name, index) in colNames.Select((name, index) => (name, index)))
				{
					records.Add(new USER_DISPLAY_SETTING
					{
						SCREEN_ID = USER_DISPLAY_SETTING.SCREEN.Search,
						USER_ACCOUNT_ID = userAccountId,
						PHYS_COLUMN_NAME = name.Key,
						LOGI_COLUMN_NAME = name.Value,
						CHECK_STATUS = true,
						DEFAULT_ORDER = index + 1,
						DISPLAY_ORDER = index + 1,
						SORT = index + 1
					});
				}
				await setBothManagementInformation(records);

				// DBに登録
				db.USER_DISPLAY_SETTINGs.AddRange(records);

				// 実行成功
				ret.succeed = true;

				return ret;
			});
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 表示設定を更新
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<Result.DefaultExecutionResult> update(List<USER_DISPLAY_SETTING> list)
		{
			try
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					// 一旦DBから読み込む
					var records = read(list.First().USER_ACCOUNT_ID);

					// 読み込んだ物に対して、更新箇所を反映することで、Modifiedマークを付ける
					foreach (var r in records)
					{
						var tmp = list.Single(e => e.SCREEN_ID == r.SCREEN_ID && e.PHYS_COLUMN_NAME == r.PHYS_COLUMN_NAME);
						r.CHECK_STATUS = tmp.CHECK_STATUS;
						r.DISPLAY_ORDER = tmp.CHECK_STATUS ? tmp.DISPLAY_ORDER : null;
						r.SORT = tmp.CHECK_STATUS ? tmp.SORT : null;
						await setUpdateManagementInformation(r);
					}

					// DBに反映
					db.USER_DISPLAY_SETTINGs.UpdateRange(records);
					db.SaveChanges();
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
