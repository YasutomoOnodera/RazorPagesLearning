// 手動追加 using
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.DB.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// 自動追加 using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
	////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 集配先マスタサービス
	/// </summary>
	////////////////////////////////////////////////////////////////////////////////////////////////
	public class DeliveryAdminService : DBServiceBase
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
		public DeliveryAdminService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 読み取り条件
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public class ReadConfigBase
		{
			////////////////////////////////////////////////////////////////////////////////////////
			/// <summary>
			/// コンストラクタ
			/// </summary>
			////////////////////////////////////////////////////////////////////////////////////////
			public ReadConfigBase()
			{
				this.company_AndOr = AndOr.Or;
				this.displayFlag = DisplayFlag.Off;
				this.sortDirection = Definition.SortDirection.ASC;
			}

			/// <summary>
			/// AND/OR条件
			/// </summary>
			public enum AndOr
			{
				/// <summary>
				/// OR条件
				/// </summary>
				Or,

				/// <summary>
				/// AND条件
				/// </summary>
				And
			}

			/// <summary>
			/// 12.集配先選択   : 表示ON/OFF
			/// 13.集配先マスタ : 非表示設定分も表示する
			/// </summary>
			public enum DisplayFlag
			{
				/// <summary>
				/// OFF
				/// </summary>
				Off = 0,

				/// <summary>
				/// ON
				/// </summary>
				On = 1,

				/// <summary>
				/// 未指定
				/// </summary>
				Na = 2
			}

			/// <summary>
			/// ユーザーアカウントID
			/// </summary>
			public Int64 userAccountId;

			/// <summary>
			/// 社名
			/// </summary>
			public string company;

			/// <summary>
			/// AND/OR
			/// 社名
			/// </summary>
			public AndOr company_AndOr;

			/// <summary>
			/// 表示ON/OFF
			/// </summary>
			public DisplayFlag displayFlag;

			/// <summary>
			/// ソート順序
			/// </summary>
			public string sortOrder;

			/// <summary>
			/// ソート方向
			/// </summary>
			public Service.Definition.SortDirection sortDirection;

			/// <summary>
			/// 読み取り開始位置
			/// </summary>
			public int start;

			/// <summary>
			/// 取り出しデータ件数
			/// </summary>
			public int take;
		}


		/// ////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 読み取り条件
		/// </summary>
		/// ////////////////////////////////////////////////////////////////////////////////////////
		public class ReadConfig
        {
            /// <summary>
            /// ユーザID
            /// </summary>
            public USER_ACCOUNT USER_ACCOUNT;

            /// <summary>
            /// AND/OR
            /// </summary>
            public int andCondtion;

            /// <summary>
            /// 会社名
            /// </summary>
            public string companyName;

            /// <summary>
            /// 表示フラグ
            /// </summary>
            public int dispCondition;

            /// <summary>
            /// 表示件数
            /// </summary>
            public int viewCount;

            /// <summary>
            /// ページ番号
            /// </summary>
            public int pageNumber;

            /// <summary>
            /// ソート順
            /// </summary>
            public int orderKey;

            /// <summary>
            /// 昇順・降順
            /// </summary>
            public int order;

            /// <summary>
            /// 
            /// </summary>
            public ReadConfig()
            {
                dispCondition = 1;
            }
        }

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 集配先マスタ検索結果
		/// DELIVERY_ADMINとUSER_DELIVERYを結合した結果をまとめる
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public class DeliveryAdminSearchResult
		{
			/// <summary>
			/// ユーザーアカウントID
			/// </summary>
			public Int64 USER_ACCOUNT_ID { get; set; }

			/// <summary>
			/// 集配先マスタID
			/// </summary>
			public Int64 DELIVERY_ADMIN_ID { get; set; }

			/// <summary>
			/// 集配先コード
			/// </summary>
			public string DELIVERY_CODE { get; set; }

			/// <summary>
			/// 荷主コード
			/// </summary>
			public string SHIPPER_CODE { get; set; }

			/// <summary>
			/// 社名
			/// </summary>
			public string COMPANY { get; set; }

			/// <summary>
			/// 部署名
			/// </summary>
			public string DEPARTMENT { get; set; }

			/// <summary>
			/// 担当者名
			/// </summary>
			public string CHARGE_NAME { get; set; }

			/// <summary>
			/// 住所1
			/// </summary>
			public string ADDRESS1 { get; set; }

			/// <summary>
			/// 住所2
			/// </summary>
			public string ADDRESS2 { get; set; }

			/// <summary>
			/// TEL
			/// </summary>
			public string TEL { get; set; }

			/// <summary>
			/// 表示フラグ
			/// </summary>
			public bool DISPLAY_FLAG { get; set; }
		}


		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 統計情報
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public class StatisticsInfo
		{
			/// <summary>
			/// データ件数
			/// </summary>
			public int numbers;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 統計情報の取得
		/// </summary>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public StatisticsInfo getStatistics(ReadConfigBase conf)
		{
			return new StatisticsInfo
			{
				// 表示する一覧の全件数を返す
				numbers = read(conf, true).Count()
			};
		}


		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件で取得できる総数
		/// </summary>
		/// <param name="readConfig"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public int count(ReadConfig readConfig)
        {
            int count = db.DELIVERY_ADMINs
                // TODO 条件の設定
                .Count();



            return count;
        }


		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// データを読み取り
		/// </summary>
		/// <param name="readConfig"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public Result.ExecutionResult<List<DELIVERY_ADMIN>> read(ReadConfig readConfig)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<List<DELIVERY_ADMIN>>((ret) =>
            {
                int startPosition = (1 + ((readConfig.pageNumber - 1) * readConfig.viewCount)) - 1;
                // TODO 対象ユーザで絞る
                var query = db.DELIVERY_ADMINs
                    .Include(e => e.USER_DELIVERies)
                    .Skip(startPosition)
                    .Take(readConfig.viewCount)
                    .ToList();
                // TODO 表示フラグで絞る
                // TODO 会社名ごとにでLIKEで絞る

                ret.result = query.ToList();
                ret.succeed = true;
            });
        }

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// データを読み取り
		/// </summary>
		/// <param name="conf"></param>
		/// <param name="allGet">true:全件取得する、false:skip,takeを指定して取得する</param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public IQueryable<DeliveryAdminSearchResult> read(ReadConfigBase conf, bool allGet = false)
		{
			#region ローカル関数

			IOrderedQueryable<DeliveryAdminSearchResult> LF_setSortOrder(IQueryable<DeliveryAdminSearchResult> _q)
			{
				string column = conf.sortOrder;
				// デフォルトソート順序
				if (string.IsNullOrEmpty(column))
				{
					column = "DELIVERY_CODE";
				}

				switch (column)
				{
					case "DELIVERY_CODE":
						if (Service.Definition.SortDirection.ASC == conf.sortDirection)
						{
							return _q.OrderBy(e => e.DELIVERY_CODE);
						}
						else
						{
							return _q.OrderByDescending(e => e.DELIVERY_CODE);
						}

					case "COMPANY":
						if (Service.Definition.SortDirection.ASC == conf.sortDirection)
						{
							return _q.OrderBy(e => e.COMPANY);
						}
						else
						{
							return _q.OrderByDescending(e => e.COMPANY);
						}

					case "DEPARTMENT":
						if (Service.Definition.SortDirection.ASC == conf.sortDirection)
						{
							return _q.OrderBy(e => e.DEPARTMENT);
						}
						else
						{
							return _q.OrderByDescending(e => e.DEPARTMENT);
						}

					case "CHARGE_NAME":
						if (Service.Definition.SortDirection.ASC == conf.sortDirection)
						{
							return _q.OrderBy(e => e.CHARGE_NAME);
						}
						else
						{
							return _q.OrderByDescending(e => e.CHARGE_NAME);
						}

					case "ADDRESS":
						if (Service.Definition.SortDirection.ASC == conf.sortDirection)
						{
							return _q.OrderBy(e => e.ADDRESS1).ThenBy(e => e.ADDRESS2);
						}
						else
						{
							return _q.OrderByDescending(e => e.ADDRESS1).ThenByDescending(e => e.ADDRESS2);
						}

					case "TEL":
						if (Service.Definition.SortDirection.ASC == conf.sortDirection)
						{
							return _q.OrderBy(e => e.TEL);
						}
						else
						{
							return _q.OrderByDescending(e => e.TEL);
						}

					case "DISPLAY_FLAG":
						if (Service.Definition.SortDirection.ASC == conf.sortDirection)
						{
							return _q.OrderBy(e => e.DISPLAY_FLAG);
						}
						else
						{
							return _q.OrderByDescending(e => e.DISPLAY_FLAG);
						}

					default:
						throw new ArgumentException($"指定されたソート条件[{column}]が不正です。");
				}
			};

			#endregion // ローカル関数

			// ユーザーアカウントを調べる
			var userService = new User.UserService(db, user, signInManager, userManager);
			var userAccount = userService.read(conf.userAccountId)
				?? throw new ArgumentException($"ID:{conf.userAccountId}のユーザ情報がありません。");

			// 作業中の荷主コードで絞り込んで基本クエリを作成
			var q = db.DELIVERY_ADMINs
				.AsQueryable()
				.Include(e => e.USER_DELIVERies)
				.Where(e => e.DELETE_FLAG == false)
				.Where(e => e.SHIPPER_CODE == userAccount.CURRENT_SHIPPER_CODE);

			// =================================================================================
			// 検索条件を連結
			// =================================================================================
			// 社名
			if (!string.IsNullOrEmpty(conf.company))
			{
				char[] delim = { ' ', '　' };
				var keys = conf.company.Split(delim, StringSplitOptions.RemoveEmptyEntries);
				switch (conf.company_AndOr)
				{
					// AND検索
					case ReadConfigBase.AndOr.And:
						foreach (var k in keys)
						{
							q = q.Where(e => e.COMPANY.Contains(k));
						}
						break;

					// OR検索
					case ReadConfigBase.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<DELIVERY_ADMIN>(keys, "COMPANY"));
						break;
				}
			}

			// 表示
			IQueryable<DeliveryAdminSearchResult> joinQ = q.GroupJoin(
				db.USER_DELIVERies,
				da => da.ID,
				ud => ud.DELIVERY_ADMIN_ID,
				(da, ud) => new { da, ud }
			)
			.SelectMany(
				x => x.ud.DefaultIfEmpty(),
				(x, ud) => new DeliveryAdminSearchResult
				{
					// USER_DELIVERYが見つからなくても、検索結果を表示する必要があるため、
					// USER_ACCOUNT_IDにダミー値(-1)を設定する
					USER_ACCOUNT_ID = (ud != null) ? ud.USER_ACCOUNT_ID : -1,
					DELIVERY_ADMIN_ID = x.da.ID,
					DELIVERY_CODE = x.da.DELIVERY_CODE,
					SHIPPER_CODE = x.da.SHIPPER_CODE,
					COMPANY = x.da.COMPANY,
					DEPARTMENT = x.da.DEPARTMENT,
					CHARGE_NAME = x.da.CHARGE_NAME,
					ADDRESS1 = x.da.ADDRESS1,
					ADDRESS2 = x.da.ADDRESS2,
					TEL = x.da.TEL,
					DISPLAY_FLAG = (ud != null) ? true : false
				}
			);

			switch (conf.displayFlag)
			{
				// 表示ON
				case ReadConfigBase.DisplayFlag.On:
					joinQ = joinQ
						.Where(e => e.DISPLAY_FLAG == true)
						.Where(e => e.USER_ACCOUNT_ID == userAccount.ID);
					break;

				// 表示OFF
				case ReadConfigBase.DisplayFlag.Off:
					joinQ = joinQ
						.Where(e => e.DISPLAY_FLAG == false);
					break;

				// 指定なし
				case ReadConfigBase.DisplayFlag.Na:
				default:
					joinQ = joinQ
						.Where(e => e.USER_ACCOUNT_ID == userAccount.ID || e.USER_ACCOUNT_ID == -1);
					break;
			}

			// ---------------------------------------------------------------------------------
			// ソート順、取得件数を指定
			// ---------------------------------------------------------------------------------
			joinQ = LF_setSortOrder(joinQ);

			// 必要な部分のみ抽出して返す
			return (true == allGet) ? joinQ : joinQ.Skip(conf.start).Take(conf.take);
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// IDを指定してデータを読み取り
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public Result.ExecutionResult<DELIVERY_ADMIN> readById(Int64 id)
		{
			return ServiceHelper.DoOperationWithErrorManagement<DELIVERY_ADMIN>((ret) =>
			{
				var q = db.DELIVERY_ADMINs.Where(e => e.ID == id).First()
							?? throw new ArgumentOutOfRangeException("指定されたIDが集配先マスタにありません。");

				ret.result = q;
				ret.succeed = true;
			});
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 集配先マスタ情報を登録
		/// </summary>
		/// <param name="deliveryAdmin"></param>
		/// <param name="userAccount"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<Result.DefaultExecutionResult> add(DELIVERY_ADMIN deliveryAdmin, USER_ACCOUNT userAccount)
		{
			async Task<Result.DefaultExecutionResult> task()
			{
				// -----------------------------------------
				// 集配先マスタに登録
				// -----------------------------------------
				deliveryAdmin.SHIPPER_CODE = userAccount.CURRENT_SHIPPER_CODE;
				await setBothManagementInformation(deliveryAdmin);

				// DBに登録
				db.DELIVERY_ADMINs.Add(deliveryAdmin);
				db.SaveChanges();

				// -----------------------------------------
				// 全ユーザーの集配先表示フラグに登録
				// -----------------------------------------
				var deliveryDispFlags = db.USER_ACCOUNTs.Select(ua => new USER_DELIVERY
				{
					DELIVERY_ADMIN_ID = deliveryAdmin.ID,
					USER_ACCOUNT_ID = ua.ID
#if false // 20180925_DBModel修正 (DELIVERY_DISP_FLAG.DISPLAY_FLAGは廃止)
					DISPLAY_FLAG = true,
					DELETE_FLAG = false
#endif // 20180925_DBModel修正 (DELIVERY_DISP_FLAG.DISPLAY_FLAGは廃止)
				});
				await setBothManagementInformation(deliveryDispFlags);

				// DBに登録
				db.USER_DELIVERies.AddRange(deliveryDispFlags);
				db.SaveChanges();

				return new Result.DefaultExecutionResult
				{
					succeed = true
				};
			};

			try
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					var ret = await task();
					if (false == ret.succeed)
					{
						throw new ApplicationException(string.Join(",", ret.errorMessages.ToString()));
					}

					// DB保存する
					transaction.Commit();

					return ret;
				}
			}
			catch (Exception e)
			{
				return Result.DefaultExecutionResult.makeError(e.Message);
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 集配先マスタ情報を更新
		/// </summary>
		/// <param name="deliveryAdmin"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public async Task<Result.DefaultExecutionResult> update(DELIVERY_ADMIN deliveryAdmin)
		{
			async Task<Result.DefaultExecutionResult> task()
			{
				// -----------------------------------------
				// 集配先マスタを更新
				// -----------------------------------------
				var record = db.DELIVERY_ADMINs
					.Where(e => e.ID == deliveryAdmin.ID)
					.FirstOrDefault() ?? throw new ArgumentException("集配先マスタ情報が存在しません。");

				record.COMPANY = deliveryAdmin.COMPANY;
				record.DEPARTMENT = deliveryAdmin.DEPARTMENT;
				record.CHARGE_NAME = deliveryAdmin.CHARGE_NAME;
				record.ZIPCODE = deliveryAdmin.ZIPCODE;
				record.ADDRESS1 = deliveryAdmin.ADDRESS1;
				record.ADDRESS2 = deliveryAdmin.ADDRESS2;
				record.TEL = deliveryAdmin.TEL;
				record.FAX = deliveryAdmin.FAX;
				record.DEFAULT_FLIGHT_CODE = deliveryAdmin.DEFAULT_FLIGHT_CODE;
				record.MAIL = deliveryAdmin.MAIL;
				record.MAIL1 = deliveryAdmin.MAIL1;
				record.MAIL2 = deliveryAdmin.MAIL2;
				record.MAIL3 = deliveryAdmin.MAIL3;
				record.MAIL4 = deliveryAdmin.MAIL4;
				record.MAIL5 = deliveryAdmin.MAIL5;
				record.MAIL6 = deliveryAdmin.MAIL6;
				record.MAIL7 = deliveryAdmin.MAIL7;
				record.MAIL8 = deliveryAdmin.MAIL8;
				record.MAIL9 = deliveryAdmin.MAIL9;
				record.MAIL10 = deliveryAdmin.MAIL10;
				await setUpdateManagementInformation(record);

				// DBに登録
				db.DELIVERY_ADMINs.Update(record);
				db.SaveChanges();

				return new Result.DefaultExecutionResult
				{
					succeed = true
				};
			};

			try
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					var ret = await task();
					if (false == ret.succeed)
					{
						throw new ApplicationException(string.Join(",", ret.errorMessages.ToString()));
					}

					// DB保存する
					transaction.Commit();

					return ret;
				}
			}
			catch (Exception e)
			{
				return Result.DefaultExecutionResult.makeError(e.Message);
			}
		}
	}
}
