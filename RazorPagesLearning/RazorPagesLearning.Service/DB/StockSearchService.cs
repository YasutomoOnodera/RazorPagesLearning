using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.DB.Helper;
using RazorPagesLearning.Service.Definition;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
	////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 在庫検索サービス
	/// 
	/// 在庫検索は在庫テーブル「STOCK」と在庫ワークテーブル「WK_STOCK」テーブルの
	/// 2テーブルの情報をマージして表示される。
	/// このため、在庫系の独自サービスとして定義する。
	/// </summary>
	////////////////////////////////////////////////////////////////////////////////////////////////
	public class StockSearchService : DBServiceBase
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
		public StockSearchService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
			ClaimsPrincipal ref_user,
			SignInManager<IdentityUser> ref_signInManager,
			UserManager<IdentityUser> ref_userManager)
			: base(ref_db, ref_user, ref_signInManager, ref_userManager)
		{
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
		/// 統計情報
		/// </summary>
		/// <param name="db"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public StatisticsInfo getStatistics()
        {
            return new StatisticsInfo
            {
                //ToDo
                //在庫ワークとの連結結果とする
                numbers = db.STOCKs.Count()
            };
        }

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 在庫情報検索結果
		/// 
		/// STOCKとWK_STOCKの結合結果となるため、
		/// 一度テーブルにまとめる。
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public class StockSearchResult
        {
            /// <summary>
            /// STOCKデータ抜き取り元のID
            /// </summary>
            public Int64 ID { get; set; }

            /// <summary>
            /// 倉庫管理番号
            /// </summary>
            public string STORAGE_MANAGE_NUMBER { get; set; }

			/// <summary>
			/// ステータス
			/// </summary>
			public string STATUS { get; set; }

            /// <summary>
            /// 題名
            /// </summary>
            public string TITLE { get; set; }

            /// <summary>
            /// 副題
            /// </summary>
            public string SUBTITLE { get; set; }

            /// <summary>
            /// 備考
            /// </summary>
            public string NOTE { get; set; }

            /// <summary>
            /// 荷主項目
            /// </summary>
            public string SHIPPER_NOTE { get; set; }

            /// <summary>
            /// お客様管理番号
            /// </summary>
            public string CUSTOMER_MANAGE_NUMBER { get; set; }

            /// <summary>
            /// 処理日
            /// </summary>
            public DateTimeOffset? PROCESSING_DATE { get; set; }

            /// <summary>
            /// 形状
            /// </summary>
            public string SHAPE { get; set; }

            /// <summary>
            /// Remark1
            /// </summary>
            public string REMARK1 { get; set; }

            /// <summary>
            /// Remark2
            /// </summary>
            public string REMARK2 { get; set; }

            /// <summary>
            /// 在庫数
            /// </summary>
            public Int64 STOCK_COUNT { get; set; }
        }


		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 読み取り設定
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////
		public class ReadConfig
        {
			public static class ConditionLabel
			{
				public const string STATUS					= "ステータス";
				public const string REQUEST					= "作業依頼";
				public const string CUSTOMER_MANAGE_NUMBER	= "お客様管理番号";
				public const string CUSTOMER_MANAGE_NUMBERS = "お客様管理番号(複数)";
				public const string TITLE					= "題名";
				public const string SUBTITLE				= "副題";
				public const string DEPARTMENT				= "部課";
				public const string SHAPE					= "形状";
				public const string CLASS1					= "区分1";
				public const string CLASS2					= "区分2";
				public const string STORAGE_MANAGE_NUMBER	= "倉庫管理番号";
				public const string REMARK1					= "Remark1";
				public const string REMARK2					= "Remark2";
				public const string NOTE					= "備考";
				public const string PRODUCT_DATE			= "制作日";
				public const string STORAGE_DATE			= "入庫日";
				public const string PROCESSING_DATE			= "処理日";
				public const string SCRAP_SCHEDULE_DATE		= "廃棄予定日";
				public const string SHIP_RETURN				= "出荷先/返却元";
				public const string REGIST_DATE				= "登録日";
				public const string CUSTOMER_ITEM			= "お客様項目";
				public const string SHIPPER_NOTE			= "荷主項目";
				public const string PROJECT_NO				= "ProjectNo";
				public const string COPYRIGHT				= "著作権";
				public const string CONTRACT				= "契約書";
				public const string DATA_NO					= "データNo";
				public const string PROCESS_JUDGE			= "処理判定";
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
			/// ステータス
			/// </summary>
			public List<string> status = new List<string>();

			/// <summary>
			/// 作業依頼
			/// </summary>
			public bool request;

			/// <summary>
			/// お客様管理番号(From)
			/// </summary>
			public string customerManageNumberFrom;

			/// <summary>
			/// お客様管理番号(To)
			/// </summary>
			public string customerManageNumberTo;

			/// <summary>
			/// お客様管理番号(複数)
			/// </summary>
			public string customerManageNumbers;

			/// <summary>
			/// AND/OR
			/// お客様管理番号(複数)
			/// </summary>
			public AndOr customerManageNumbers_AndOr;

			/// <summary>
			/// 題名
			/// </summary>
			public string title;

			/// <summary>
			/// 題名(AND/OR)
			/// </summary>
			public AndOr title_AndOr;

			/// <summary>
			/// 副題
			/// </summary>
			public string subtitle;

			/// <summary>
			/// 副題(AND/OR)
			/// </summary>
			public AndOr subtitle_AndOr;

			/// <summary>
			/// 部課コード
			/// </summary>
			public string departmentCode;

			/// <summary>
			/// 形状
			/// </summary>
			public string shape;

			/// <summary>
			/// 形状(AND/OR)
			/// </summary>
			public AndOr shape_AndOr;

			/// <summary>
			/// 区分1
			/// </summary>
			public List<string> class1 = new List<string>();

			/// <summary>
			/// 区分2
			/// </summary>
			public List<string> class2 = new List<string>();

			/// <summary>
			/// 倉庫管理番号(From)
			/// </summary>
			public string storageManageNumberFrom;

			/// <summary>
			/// 倉庫管理番号(To)
			/// </summary>
			public string storageManageNumberTo;

			/// <summary>
			/// Remark1
			/// </summary>
			public string remark1;

			/// <summary>
			/// Remark1(AND/OR)
			/// </summary>
			public AndOr remark1_AndOr;

			/// <summary>
			/// Remark2
			/// </summary>
			public string remark2;

			/// <summary>
			/// Remark2(AND/OR)
			/// </summary>
			public AndOr remark2_AndOr;

			/// <summary>
			/// 備考
			/// </summary>
			public string note;

			/// <summary>
			/// 備考(AND/OR)
			/// </summary>
			public AndOr note_AndOr;

			/// <summary>
			/// 制作日(From)
			/// </summary>
			public DateTimeOffset? productDateFrom;

			/// <summary>
			/// 制作日(To)
			/// </summary>
			public DateTimeOffset? productDateTo;

			/// <summary>
			/// 入庫日(From)
			/// </summary>
			public DateTimeOffset? storageDateFrom;

			/// <summary>
			/// 入庫日(To)
			/// </summary>
			public DateTimeOffset? storageDateTo;

			/// <summary>
			/// 処理日(From)
			/// </summary>
			public DateTimeOffset? processingDateFrom;

			/// <summary>
			/// 処理日(To)
			/// </summary>
			public DateTimeOffset? processingDateTo;

			/// <summary>
			/// 廃棄予定日(From)
			/// </summary>
			public DateTimeOffset? scrapScheduleDateFrom;

			/// <summary>
			/// 廃棄予定日(To)
			/// </summary>
			public DateTimeOffset? scrapScheduleDateTo;

			/// <summary>
			/// 出荷先/返却元コード
			/// </summary>
			public string shipReturnCode;

			/// <summary>
			/// 出荷先/返却元コード(AND/OR)
			/// </summary>
			public AndOr shipReturnCode_AndOr;

			/// <summary>
			/// 登録日(From)
			/// </summary>
			public DateTimeOffset? registDateFrom;

			/// <summary>
			/// 登録日(To)
			/// </summary>
			public DateTimeOffset? registDateTo;

			/// <summary>
			/// お客様項目1_コード
			/// </summary>
			public string customerItem1Code;

			/// <summary>
			/// お客様項目1_値
			/// </summary>
			public string customerItem1Value;

			/// <summary>
			/// お客様項目1(AND/OR)
			/// </summary>
			public AndOr customerItem1_AndOr;

			/// <summary>
			/// お客様項目2_コード
			/// </summary>
			public string customerItem2Code;

			/// <summary>
			/// お客様項目2_値
			/// </summary>
			public string customerItem2Value;

			/// <summary>
			/// お客様項目2(AND/OR)
			/// </summary>
			public AndOr customerItem2_AndOr;

			/// <summary>
			/// お客様項目3_コード
			/// </summary>
			public string customerItem3Code;

			/// <summary>
			/// お客様項目3_値
			/// </summary>
			public string customerItem3Value;

			/// <summary>
			/// お客様項目3(AND/OR)
			/// </summary>
			public AndOr customerItem3_AndOr;

			/// <summary>
			/// 荷主項目
			/// </summary>
			public string shipperNote;

			/// <summary>
			/// 荷主項目(AND/OR)
			/// </summary>
			public AndOr shipperNote_AndOr;

			/// <summary>
			/// ProjectNo1
			/// </summary>
			public string projectNo1;

			/// <summary>
			/// ProjectNo2
			/// </summary>
			public string projectNo2;

			/// <summary>
			/// ProjectNo(AND/OR)
			/// </summary>
			public AndOr projectNo_AndOr;

			/// <summary>
			/// 著作権1
			/// </summary>
			public string copyright1;

			/// <summary>
			/// 著作権2
			/// </summary>
			public string copyright2;

			/// <summary>
			/// 著作権(AND/OR)
			/// </summary>
			public AndOr copyright_AndOr;

			/// <summary>
			/// 契約書1
			/// </summary>
			public string contract1;

			/// <summary>
			/// 契約書2
			/// </summary>
			public string contract2;

			/// <summary>
			/// 契約書
			/// </summary>
			public AndOr contract_AndOr;

			/// <summary>
			/// データNo1
			/// </summary>
			public string dataNo1;

			/// <summary>
			/// データNo2
			/// </summary>
			public string dataNo2;

			/// <summary>
			/// データNo(AND/OR)
			/// </summary>
			public AndOr dataNo_AndOr;

			/// <summary>
			/// 処理判定1
			/// </summary>
			public string processJudge1;

			/// <summary>
			/// 処理判定2
			/// </summary>
			public string processJudge2;

			/// <summary>
			/// 処理判定(AND/OR)
			/// </summary>
			public AndOr processJudge_AndOr;

            /// <summary>
            /// 読み取り開始位置
            /// </summary>
            public int start;

            /// <summary>
            /// 取り出しデータ件数
            /// </summary>
            public int take;

            /// <summary>
            /// ソート順序
            /// </summary>
            public string sortOrder;

            /// <summary>
            /// ソート順序(昇順/降順)
            /// </summary>
            public SortDirection sortDirection;

            public ReadConfig()
            {
                ///倉庫管理番号をソート順序とする
                this.sortOrder = "STORAGE_MANAGE_NUMBER";
            }
        }

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// ソート順設定
		/// </summary>
		/// <param name="columns"></param>
		/// <param name="q"></param>
		/// <param name="direction"></param>
		/// <param name="byColumnHeader"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		public IQueryable<STOCK> setSortOrder(List<string> columns, IQueryable<STOCK> q, SortDirection direction, bool byColumnHeader)
        {
			IOrderedQueryable<STOCK> ret = null;

			for (int i = 0; i < columns.Count(); i++)
			{
				switch (columns[i].Trim())
				{
					case "STORAGE_MANAGE_NUMBER":
						if (SortDirection.ASC == direction)
						{
							ret = (0 == i) ?
								q.OrderBy(e => e.STORAGE_MANAGE_NUMBER) :
								ret.ThenBy(e => e.STORAGE_MANAGE_NUMBER);
						}
						else
						{
							ret = (0 == i) ?
								q.OrderByDescending(e => e.STORAGE_MANAGE_NUMBER) :
								ret.ThenByDescending(e => e.STORAGE_MANAGE_NUMBER);
						}
						break;

					case "STATUS":
						if (SortDirection.ASC == direction)
						{
							ret = (0 == i) ?
								q.OrderBy(e => e.STATUS) :
								ret.ThenBy(e => e.STATUS);
						}
						else
						{
							ret = (0 == i) ?
								q.OrderByDescending(e => e.STATUS) :
								ret.ThenByDescending(e => e.STATUS);
						}
						break;

					case "TITLE":
						if (SortDirection.ASC == direction)
						{
							ret = (0 == i) ?
								q.OrderBy(e => e.TITLE) :
								ret.ThenBy(e => e.TITLE);
						}
						else
						{
							ret = (0 == i) ?
								q.OrderByDescending(e => e.TITLE) :
								ret.ThenByDescending(e => e.TITLE);
						}
						break;

					case "SUBTITLE":
						if (SortDirection.ASC == direction)
						{
							ret = (0 == i) ?
								q.OrderBy(e => e.SUBTITLE) :
								ret.ThenBy(e => e.SUBTITLE);
						}
						else
						{
							ret = (0 == i) ?
								q.OrderByDescending(e => e.SUBTITLE) :
								ret.ThenByDescending(e => e.SUBTITLE);
						}
						break;

					case "NOTE":
						if (SortDirection.ASC == direction)
						{
							ret = (0 == i) ?
								q.OrderBy(e => e.NOTE) :
								ret.ThenBy(e => e.NOTE);
						}
						else
						{
							ret = (0 == i) ?
								q.OrderByDescending(e => e.NOTE) :
								ret.ThenByDescending(e => e.NOTE);
						}
						break;

					case "SHIPPER_NOTE":
						if (SortDirection.ASC == direction)
						{
							ret = (0 == i) ?
								q.OrderBy(e => e.SHIPPER_NOTE) :
								ret.ThenBy(e => e.SHIPPER_NOTE);
						}
						else
						{
							ret = (0 == i) ?
								q.OrderByDescending(e => e.SHIPPER_NOTE) :
								ret.ThenByDescending(e => e.SHIPPER_NOTE);
						}
						break;

					case "CUSTOMER_MANAGE_NUMBER":
						if (SortDirection.ASC == direction)
						{
							ret = (0 == i) ?
								q.OrderBy(e => e.CUSTOMER_MANAGE_NUMBER) :
								ret.ThenBy(e => e.CUSTOMER_MANAGE_NUMBER);
						}
						else
						{
							ret = (0 == i) ?
								q.OrderByDescending(e => e.CUSTOMER_MANAGE_NUMBER) :
								ret.ThenByDescending(e => e.CUSTOMER_MANAGE_NUMBER);
						}
						break;

					case "PROCESSING_DATE":
						if (SortDirection.ASC == direction)
						{
							ret = (0 == i) ?
								q.OrderBy(e => e.PROCESSING_DATE) :
								ret.ThenBy(e => e.PROCESSING_DATE);
						}
						else
						{
							ret = (0 == i) ?
								q.OrderByDescending(e => e.PROCESSING_DATE) :
								ret.ThenByDescending(e => e.PROCESSING_DATE);
						}
						break;

					case "SHAPE":
						if (SortDirection.ASC == direction)
						{
							ret = (0 == i) ?
								q.OrderBy(e => e.SHAPE) :
								ret.ThenBy(e => e.SHAPE);
						}
						else
						{
							ret = (0 == i) ?
								q.OrderByDescending(e => e.SHAPE) :
								ret.ThenByDescending(e => e.SHAPE);
						}
						break;

					case "REMARK1":
						if (SortDirection.ASC == direction)
						{
							ret = (0 == i) ?
								q.OrderBy(e => e.REMARK1) :
								ret.ThenBy(e => e.REMARK1);
						}
						else
						{
							ret = (0 == i) ?
								q.OrderByDescending(e => e.REMARK1) :
								ret.ThenByDescending(e => e.REMARK1);
						}
						break;

					case "REMARK2":
						if (SortDirection.ASC == direction)
						{
							ret = (0 == i) ?
								q.OrderBy(e => e.REMARK2) :
								ret.ThenBy(e => e.REMARK2);
						}
						else
						{
							ret = (0 == i) ?
								q.OrderByDescending(e => e.REMARK2) :
								ret.ThenByDescending(e => e.REMARK2);
						}
						break;

					case "STOCK_COUNT":
						if (SortDirection.ASC == direction)
						{
							ret = (0 == i) ?
								q.OrderBy(e => e.STOCK_COUNT) :
								ret.ThenBy(e => e.STOCK_COUNT);
						}
						else
						{
							ret = (0 == i) ?
								q.OrderByDescending(e => e.STOCK_COUNT) :
								ret.ThenByDescending(e => e.STOCK_COUNT);
						}
						break;

					default:
						throw new ApplicationException("想定外のソート条件です。");
				}

				if (byColumnHeader && columns[i].Trim() != "STORAGE_MANAGE_NUMBER")
				{
					ret = ret.ThenBy(e => e.STORAGE_MANAGE_NUMBER);
				}
			}

			return ret;
        }

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 指定された情報を読み取る
		/// </summary>
		/// <param name="db"></param>
		/// <param name="readConfig"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		// public IQueryable<StockSearchResult> read(ReadConfig readConfig)
		public IQueryable<STOCK> read(ReadConfig readConfig)
        {
			var q = db.STOCKs
				.Where(e => e.DELETE_FLAG == false)
				.Where(e => e.HIDE_FLAG == false)
                .Include(e => e.WK_STOCK)
                .AsQueryable();

			q = catQuerySearchCondition(q, readConfig);

			return q;
		}

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 指定された情報を読み取る(帳票出力)
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        // public IQueryable<StockSearchResult> read(ReadConfig readConfig)
        public IQueryable<STOCK> readReport(ReadConfig readConfig)
        {
            var q = db.STOCKs
                .Where(e => e.DELETE_FLAG == false)
                .Include(e => e.WK_STOCK).Include(e => e.DEPARTMENT_ADMIN)
                .AsQueryable();

            q = catQuerySearchCondition(q, readConfig);

            //Todo 
            //WK_STOCKと結合した結果を表示する
            return q;
        }

        #region 検索条件の追加

        //////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 検索条件の追加
        /// </summary>
        /// <param name="q"></param>
        /// <param name="conf"></param>
        /// <returns></returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////
        private IQueryable<STOCK> catQuerySearchCondition(IQueryable<STOCK> q, ReadConfig conf)
		{
			q = catQueryStatus(q, conf);						// ステータス
			q = catQueryRequest(q, conf);						// 作業依頼に追加した在庫を非表示にする
			q = catQueryCustomerManageNumberFromTo(q, conf);    // お客様管理番号(FromTo)
			q = catQueryCustomerManageNumbers(q, conf);         // お客様管理番号(複数)
			q = catQueryTitle(q, conf);                         // 題名
			q = catQuerySubtitle(q, conf);                      // 副題
			q = catQueryDepartmentCode(q, conf);                // 部課コード
			q = catQueryShape(q, conf);                         // 形状
			q = catQueryClass1(q, conf);                        // 区分1
			q = catQueryClass2(q, conf);                        // 区分2
			q = catQueryStorageManageNumberFromTo(q, conf);     // 倉庫管理番号(FromTo)
			q = catQueryRemark1(q, conf);                       // Remark1
			q = catQueryRemark2(q, conf);                       // Remark2
			q = catQueryNote(q, conf);                          // 備考
			q = catQueryProductDateFromTo(q, conf);             // 制作日(FromTo)
			q = catQueryStorageDateFromTo(q, conf);             // 入庫日(FromTo)
			q = catQueryProcessingDateFromTo(q, conf);          // 処理日(FromTo)
			q = catQueryScrapScheduleDateFromTo(q, conf);       // 廃棄予定日(FromTo)
			q = catQueryShipReturnCode(q, conf);                // 出荷先/返却元コード
			q = catQueryRegistDateFromTo(q, conf);              // 登録日(FromTo)
			q = catQueryCustomerItem1(q, conf);                 // お客様項目1
			q = catQueryCustomerItem2(q, conf);                 // お客様項目2
			q = catQueryCustomerItem3(q, conf);                 // お客様項目3
			q = catQueryShipperNote(q, conf);                   // 荷主項目
			q = catQueryCustomerOption(q, conf);                // 顧客専用項目

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// ステータス
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryStatus(IQueryable<STOCK> q, ReadConfig conf)
		{
			// =====================================================================================
			// 資材は資材販売画面でのみ表示するため、ここでは対象外とする
			// =====================================================================================
			q = q.Where(e =>
				e.STATUS != DOMAIN.StockStatusCode.MATERIAL
			);

			// =====================================================================================
			// 資材、ゼロ在庫を表示する 以外の条件を追加する
			// =====================================================================================
			var filter = new List<string>
			{
				DOMAIN.StockStatusCode.MATERIAL,
				DOMAIN.StockStatusCode.DISP_ZERO_STOCK
			};
			var statusList = conf.status.Where(e => !filter.Contains(e)).ToList();

			if (0 < statusList.Count)
			{
				// OR条件で連結
				q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(statusList, "STATUS"));
			}

			// =====================================================================================
			// ゼロ在庫を表示する が選択されていない場合は、0<在庫数のみを検索する
			// =====================================================================================
			if (!conf.status.Contains(DOMAIN.StockStatusCode.DISP_ZERO_STOCK))
			{
				q = q.Where(e =>
					0 < e.STOCK_COUNT
				);
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 作業依頼に追加した在庫を非表示にする
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryRequest(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (conf.request)
			{
				q = q.Where(e =>
					e.STATUS != DOMAIN.StockStatusCode.REQUEST
				);
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// お客様管理番号(FromTo)
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryCustomerManageNumberFromTo(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (string.IsNullOrEmpty(conf.customerManageNumberFrom) &&
				string.IsNullOrEmpty(conf.customerManageNumberTo))
			{
				return q;
			}

			// From、Toともに指定されている場合
			if (!string.IsNullOrEmpty(conf.customerManageNumberFrom) &&
				!string.IsNullOrEmpty(conf.customerManageNumberTo))
			{
				q = q.Where(e =>
					int.Parse(conf.customerManageNumberFrom) <= int.Parse(e.CUSTOMER_MANAGE_NUMBER) &&
					int.Parse(e.CUSTOMER_MANAGE_NUMBER) <= int.Parse(conf.customerManageNumberTo)
				);
			}
			// Fromだけ指定されている場合
			else if (string.IsNullOrEmpty(conf.customerManageNumberTo))
			{
				q = q.Where(e =>
					int.Parse(conf.customerManageNumberFrom) <= int.Parse(e.CUSTOMER_MANAGE_NUMBER)
				);
			}
			// Toだけ指定されている場合
			else
			{
				q = q.Where(e =>
					int.Parse(e.CUSTOMER_MANAGE_NUMBER) <= int.Parse(conf.customerManageNumberTo)
				);
			}

			return q;
		}

		// TODO: お客様管理番号(複数)の検索において、ANDはあり得ないので、仕様バグと思われる。
		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// お客様管理番号(複数)
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryCustomerManageNumbers(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (string.IsNullOrEmpty(conf.customerManageNumbers))
			{
				return q;
			}

			char[] delim = { ' ', '　' };
			var keys = conf.customerManageNumbers.Split(delim, StringSplitOptions.RemoveEmptyEntries);

			switch (conf.customerManageNumbers_AndOr)
			{
				// AND検索
				case ReadConfig.AndOr.And:
					foreach (var k in keys)
					{
						q = q.Where(e =>
							e.CUSTOMER_MANAGE_NUMBER == k
						);
					}
					break;

				// OR検索
				case ReadConfig.AndOr.Or:
					q = q.Where(
						QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "CUSTOMER_MANAGE_NUMBER")
					);
					break;
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 題名
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryTitle(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (string.IsNullOrEmpty(conf.title))
			{
				return q;
			}

			char[] delim = { ' ', '　' };
			var keys = conf.title.Split(delim, StringSplitOptions.RemoveEmptyEntries);

			switch (conf.title_AndOr)
			{
				// AND検索
				case ReadConfig.AndOr.And:
					foreach (var k in keys)
					{
						q = q.Where(e =>
							e.TITLE.Contains(k)
						);
					}
					break;
					
				// OR検索
				case ReadConfig.AndOr.Or:
					q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "TITLE"));
					break;
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 副題
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQuerySubtitle(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (string.IsNullOrEmpty(conf.subtitle))
			{
				return q;
			}

			char[] delim = { ' ', '　' };
			var keys = conf.subtitle.Split(delim, StringSplitOptions.RemoveEmptyEntries);

			switch (conf.subtitle_AndOr)
			{
				// AND検索
				case ReadConfig.AndOr.And:
					foreach (var k in keys)
					{
						q = q.Where(e =>
							e.SUBTITLE.Contains(k)
						);
					}
					break;

				// OR検索
				case ReadConfig.AndOr.Or:
					q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "SUBTITLE"));
					break;
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 部課コード
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryDepartmentCode(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (!string.IsNullOrEmpty(conf.departmentCode))
			{
				q = q.Where(e =>
					e.DEPARTMENT_CODE.Contains(conf.departmentCode)
				);
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 形状
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryShape(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (string.IsNullOrEmpty(conf.shape))
			{
				return q;
			}

			char[] delim = { ' ', '　' };
			var keys = conf.shape.Split(delim, StringSplitOptions.RemoveEmptyEntries);

			switch (conf.shape_AndOr)
			{
				// AND検索
				case ReadConfig.AndOr.And:
					foreach (var k in keys)
					{
						q = q.Where(e =>
							e.SHAPE.Contains(k)
						);
					}
					break;

				// OR検索
				case ReadConfig.AndOr.Or:
					q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "SHAPE"));
					break;
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 区分1
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryClass1(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (0 < conf.class1.Count)
			{
				q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(conf.class1, "CLASS1"));
			}
			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 区分2
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryClass2(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (0 < conf.class2.Count)
			{
				q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(conf.class2, "CLASS2"));
			}
			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 倉庫管理番号(FromTo)
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryStorageManageNumberFromTo(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (string.IsNullOrEmpty(conf.storageManageNumberFrom) &&
				string.IsNullOrEmpty(conf.storageManageNumberTo))
			{
				return q;
			}

			// From、Toともに指定されている場合
			if (!string.IsNullOrEmpty(conf.storageManageNumberFrom) &&
				!string.IsNullOrEmpty(conf.storageManageNumberTo))
			{
				q = q.Where(e =>
					int.Parse(conf.storageManageNumberFrom) <= int.Parse(e.STORAGE_MANAGE_NUMBER) &&
					int.Parse(e.STORAGE_MANAGE_NUMBER) <= int.Parse(conf.storageManageNumberTo)
				);
			}
			// Fromだけ指定されている場合
			else if (string.IsNullOrEmpty(conf.storageManageNumberTo))
			{
				q = q.Where(e =>
					int.Parse(conf.storageManageNumberFrom) <= int.Parse(e.STORAGE_MANAGE_NUMBER)
				);
			}
			// Toだけ指定されている場合
			else
			{
				q = q.Where(e =>
					int.Parse(e.STORAGE_MANAGE_NUMBER) <= int.Parse(conf.storageManageNumberTo)
				);
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// Remark1
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryRemark1(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (string.IsNullOrEmpty(conf.remark1))
			{
				return q;
			}

			char[] delim = { ' ', '　' };
			var keys = conf.remark1.Split(delim, StringSplitOptions.RemoveEmptyEntries);

			switch (conf.remark1_AndOr)
			{
				// AND検索
				case ReadConfig.AndOr.And:
					foreach (var k in keys)
					{
						q = q.Where(e =>
							e.REMARK1.Contains(k)
						);
					}
					break;

				// OR検索
				case ReadConfig.AndOr.Or:
					q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "REMARK1"));
					break;
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// Remark2
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryRemark2(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (string.IsNullOrEmpty(conf.remark2))
			{
				return q;
			}

			char[] delim = { ' ', '　' };
			var keys = conf.remark2.Split(delim, StringSplitOptions.RemoveEmptyEntries);

			switch (conf.remark2_AndOr)
			{
				// AND検索
				case ReadConfig.AndOr.And:
					foreach (var k in keys)
					{
						q = q.Where(e =>
							e.REMARK2.Contains(k)
						);
					}
					break;

				// OR検索
				case ReadConfig.AndOr.Or:
					q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "REMARK2"));
					break;
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 備考
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryNote(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (string.IsNullOrEmpty(conf.note))
			{
				return q;
			}

			char[] delim = { ' ', '　' };
			var keys = conf.note.Split(delim, StringSplitOptions.RemoveEmptyEntries);

			switch (conf.note_AndOr)
			{
				// AND検索
				case ReadConfig.AndOr.And:
					foreach (var k in keys)
					{
						q = q.Where(e =>
							e.NOTE.Contains(k)
						);
					}
					break;

				// OR検索
				case ReadConfig.AndOr.Or:
					q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "NOTE"));
					break;
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 制作日(FromTo)
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryProductDateFromTo(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (null == conf.productDateFrom && null == conf.productDateTo)
			{
				return q;
			}

            // ModelChange:型がDateTimeOffsetからstringに変わったため、ここの処理を修正する必要あり
			// From、Toともに指定されている場合
			//if (null != conf.productDateFrom && null != conf.productDateTo)
			//{
			//	q = q.Where(e =>
			//		conf.productDateFrom <= e.PRODUCT_DATE &&
			//		e.PRODUCT_DATE <= conf.productDateTo
			//	);
			//}
			//// Fromだけ指定されている場合
			//else if (null == conf.productDateTo)
			//{
			//	q = q.Where(e =>
			//		conf.productDateFrom <= e.PRODUCT_DATE
			//	);
			//}
			//// Toだけ指定されている場合
			//else
			//{
			//	q = q.Where(e =>
			//		e.PRODUCT_DATE <= conf.productDateTo
			//	);
			//}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 入庫日(FromTo)
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryStorageDateFromTo(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (null == conf.storageDateFrom && null == conf.storageDateTo)
			{
				return q;
			}

			// From、Toともに指定されている場合
			if (null != conf.storageDateFrom && null != conf.storageDateTo)
			{
				q = q.Where(e =>
					conf.storageDateFrom <= e.STORAGE_DATE &&
					e.STORAGE_DATE <= conf.storageDateTo
				);
			}
			// Fromだけ指定されている場合
			else if (null == conf.storageDateTo)
			{
				q = q.Where(e => conf.storageDateFrom <= e.STORAGE_DATE);
			}
			// Toだけ指定されている場合
			else
			{
				q = q.Where(e => e.STORAGE_DATE <= conf.storageDateTo);
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 処理日(FromTo)
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryProcessingDateFromTo(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (null == conf.processingDateFrom && null == conf.processingDateTo)
			{
				return q;
			}

			// From、Toともに指定されている場合
			if (null != conf.processingDateFrom && null != conf.processingDateTo)
			{
				q = q.Where(e =>
					conf.processingDateFrom <= e.PROCESSING_DATE &&
					e.PROCESSING_DATE <= conf.processingDateTo
				);
			}
			// Fromだけ指定されている場合
			else if (null == conf.processingDateTo)
			{
				q = q.Where(e => conf.processingDateFrom <= e.PROCESSING_DATE);
			}
			// Toだけ指定されている場合
			else
			{
				q = q.Where(e => e.PROCESSING_DATE <= conf.processingDateTo);
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 廃棄予定日(FromTo)
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryScrapScheduleDateFromTo(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (null == conf.scrapScheduleDateFrom && null == conf.scrapScheduleDateTo)
			{
				return q;
			}

			// From、Toともに指定されている場合
			if (null != conf.scrapScheduleDateFrom && null != conf.scrapScheduleDateTo)
			{
				q = q.Where(e =>
					conf.scrapScheduleDateFrom <= e.SCRAP_SCHEDULE_DATE &&
					e.SCRAP_SCHEDULE_DATE <= conf.scrapScheduleDateTo
				);
			}
			// Fromだけ指定されている場合
			else if (null == conf.scrapScheduleDateTo)
			{
				q = q.Where(e =>
					conf.scrapScheduleDateFrom <= e.SCRAP_SCHEDULE_DATE
				);
			}
			// Toだけ指定されている場合
			else
			{
				q = q.Where(e =>
					e.SCRAP_SCHEDULE_DATE <= conf.scrapScheduleDateTo
				);
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 出荷先/返却元コード
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryShipReturnCode(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (string.IsNullOrEmpty(conf.shipReturnCode))
			{
				return q;
			}

			char[] delim = { ' ', '　' };
			var keys = conf.shipReturnCode.Split(delim, StringSplitOptions.RemoveEmptyEntries);

			switch (conf.shipReturnCode_AndOr)
			{
				// AND検索
				case ReadConfig.AndOr.And:
					foreach (var k in keys)
					{
						q = q.Where(e => e.SHIP_RETURN_CODE == k);
					}
					break;
				
				// OR検索
				case ReadConfig.AndOr.Or:
					q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "SHIP_RETURN_CODE"));
					break;
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 登録日(FromTo)
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryRegistDateFromTo(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (null == conf.registDateFrom && null == conf.registDateTo)
			{
				return q;
			}

			// From、Toともに指定されている場合
			if (null != conf.registDateFrom && null != conf.registDateTo)
			{
				q = q.Where(e =>
					conf.registDateFrom <= e.WMS_REGIST_DATE &&
					e.WMS_REGIST_DATE <= conf.registDateTo
				);
			}
			// Fromだけ指定されている場合
			else if (null == conf.registDateTo)
			{
				q = q.Where(e => conf.registDateFrom <= e.WMS_REGIST_DATE);
			}
			// Toだけ指定されている場合
			else
			{
				q = q.Where(e => e.WMS_REGIST_DATE <= conf.registDateTo);
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// お客様項目1
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryCustomerItem1(IQueryable<STOCK> q, ReadConfig conf)
		{
			char[] delim = { ' ', '　' };

			// =====================================================================================
			// お客様項目1_コード
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.customerItem1Code))
			{
				var keys = conf.customerItem1Code.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.customerItem1_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
                            // ModelChange:USER_ITEMテーブルへ移動
                            //q = q.Where(e => e.CUSTOMER_ITEM1_CODE == k);
						}
						break;

					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "CUSTOMER_ITEM1_CODE"));
						break;
				}
			}

			// =====================================================================================
			// お客様項目1_値
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.customerItem1Value))
			{
				var keys = conf.customerItem1Value.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.customerItem1_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
                            // ModelChange:USER_ITEMテーブルへ移動
                            //q = q.Where(e => e.CUSTOMER_ITEM1_VALUE == k);
						}
						break;
					
					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "CUSTOMER_ITEM1_VALUE"));
						break;
				}
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// お客様項目2
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryCustomerItem2(IQueryable<STOCK> q, ReadConfig conf)
		{
			char[] delim = { ' ', '　' };

			// =====================================================================================
			// お客様項目2_コード
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.customerItem2Code))
			{
				var keys = conf.customerItem2Code.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.customerItem2_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
                            // ModelChange:USER_ITEMテーブルへ移動
                            //q = q.Where(e => e.CUSTOMER_ITEM2_CODE == k);
						}
						break;

					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "CUSTOMER_ITEM2_CODE"));
						break;
				}
			}

			// =====================================================================================
			// お客様項目2_値
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.customerItem2Value))
			{
				var keys = conf.customerItem2Value.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.customerItem2_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
                            // ModelChange:USER_ITEMテーブルへ移動
                            //q = q.Where(e => e.CUSTOMER_ITEM2_VALUE == k);
						}
						break;
					
					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "CUSTOMER_ITEM2_VALUE"));
						break;
				}
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// お客様項目3
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryCustomerItem3(IQueryable<STOCK> q, ReadConfig conf)
		{
			char[] delim = { ' ', '　' };

			// =====================================================================================
			// お客様項目3_コード
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.customerItem3Code))
			{
				var keys = conf.customerItem3Code.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.customerItem3_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
                            // ModelChange:USER_ITEMテーブルへ移動
                            //q = q.Where(e => e.CUSTOMER_ITEM3_CODE == k);
						}
						break;

					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "CUSTOMER_ITEM3_CODE"));
						break;
				}
			}

			// =====================================================================================
			// お客様項目3_値
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.customerItem3Value))
			{
				var keys = conf.customerItem3Value.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.customerItem3_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
                            // ModelChange:USER_ITEMテーブルへ移動
							//q = q.Where(e => e.CUSTOMER_ITEM3_VALUE == k);
						}
						break;

					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "CUSTOMER_ITEM3_VALUE"));
						break;
				}
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 荷主項目
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryShipperNote(IQueryable<STOCK> q, ReadConfig conf)
		{
			if (string.IsNullOrEmpty(conf.shipperNote))
			{
				return q;
			}

			char[] delim = { ' ', '　' };
			var keys = conf.shipperNote.Split(delim, StringSplitOptions.RemoveEmptyEntries);

			switch (conf.shipperNote_AndOr)
			{
				// AND検索
				case ReadConfig.AndOr.And:
					foreach (var k in keys)
					{
						q = q.Where(e =>
							e.SHIPPER_NOTE.Contains(k)
						);
					}
					break;

				// OR検索
				case ReadConfig.AndOr.Or:
					q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "SHIPPER_NOTE"));
					break;
			}

			return q;
		}

		#endregion // 検索条件の追加


		#region 検索条件(顧客専用項目)の追加

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加
		/// 顧客専用項目
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryCustomerOption(IQueryable<STOCK> q, ReadConfig conf)
		{
			q = catQueryProjectNo(q, conf);         // ProjectNo
			q = catQueryCopyright(q, conf);         // 著作権
			q = catQueryContract(q, conf);          // 契約書
			q = catQueryDataNo(q, conf);            // データNo
			q = catQueryProcessJudge(q, conf);		// 処理判定

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加(顧客専用項目)
		/// ProjectNo
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryProjectNo(IQueryable<STOCK> q, ReadConfig conf)
		{
			char[] delim = { ' ', '　' };

			// =====================================================================================
			// ProjectNo1
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.projectNo1))
			{
				var keys = conf.projectNo1.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.projectNo_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
							q = q.Where(e => e.PROJECT_NO1 == k);
						}
						break;
					
					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "PROJECT_NO1"));
						break;
				}
			}

			// =====================================================================================
			// ProjectNo2
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.projectNo2))
			{
				var keys = conf.projectNo2.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.projectNo_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
							q = q.Where(e => e.PROJECT_NO2 == k);
						}
						break;

					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "PROJECT_NO2"));
						break;
				}
			}
			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加(顧客専用項目)
		/// 著作権
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryCopyright(IQueryable<STOCK> q, ReadConfig conf)
		{
			char[] delim = { ' ', '　' };

			// =====================================================================================
			// 著作権1
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.copyright1))
			{
				var keys = conf.copyright1.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.copyright_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
							q = q.Where(e => e.COPYRIGHT1 == k);
						}
						break;

					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "COPYRIGHT1"));
						break;
				}
			}

			// =====================================================================================
			// 著作権2
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.copyright2))
			{
				var keys = conf.copyright2.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.copyright_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
							q = q.Where(e => e.COPYRIGHT2 == k);
						}
						break;

					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "COPYRIGHT2"));
						break;
				}
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加(顧客専用項目)
		/// 契約書
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryContract(IQueryable<STOCK> q, ReadConfig conf)
		{
			char[] delim = { ' ', '　' };

			// =====================================================================================
			// 契約書1
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.contract1))
			{
				var keys = conf.contract1.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.contract_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
							q = q.Where(e => e.CONTRACT1 == k);
						}
						break;

					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "CONTRACT1"));
						break;
				}
			}

			// =====================================================================================
			// 契約書2
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.contract2))
			{
				var keys = conf.contract2.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.contract_AndOr)
				{
					// AND条件
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
							q = q.Where(e => e.CONTRACT2 == k);
						}
						break;

					// OR条件
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "CONTRACT2"));
						break;
				}
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加(顧客専用項目)
		/// データNo
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryDataNo(IQueryable<STOCK> q, ReadConfig conf)
		{
			char[] delim = { ' ', '　' };

			// =====================================================================================
			// データNo1
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.dataNo1))
			{
				var keys = conf.dataNo1.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.dataNo_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
							q = q.Where(e => e.DATA_NO1 == k);
						}
						break;

					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "DATA_NO1"));
						break;
				}
			}

			// =====================================================================================
			// データNo2
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.dataNo2))
			{
				var keys = conf.dataNo2.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.dataNo_AndOr)
				{
					// AND条件
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
							q = q.Where(e => e.DATA_NO2 == k);
						}
						break;

					// OR条件
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "DATA_NO2"));
						break;
				}
			}

			return q;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件の追加(顧客専用項目)
		/// 処理判定
		/// </summary>
		/// <param name="q"></param>
		/// <param name="conf"></param>
		/// <returns></returns>
		////////////////////////////////////////////////////////////////////////////////////////////
		private IQueryable<STOCK> catQueryProcessJudge(IQueryable<STOCK> q, ReadConfig conf)
		{
			char[] delim = { ' ', '　' };

			// =====================================================================================
			// 処理判定1
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.processJudge1))
			{
				var keys = conf.processJudge1.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.processJudge_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
							q = q.Where(e => e.PROCESS_JUDGE1 == k);
						}
						break;
					
					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "PROCESS_JUDGE1"));
						break;
				}
			}

			// =====================================================================================
			// 処理判定2
			// =====================================================================================
			if (!string.IsNullOrEmpty(conf.processJudge2))
			{
				var keys = conf.processJudge2.Split(delim, StringSplitOptions.RemoveEmptyEntries);

				switch (conf.processJudge_AndOr)
				{
					// AND検索
					case ReadConfig.AndOr.And:
						foreach (var k in keys)
						{
							q = q.Where(e => e.PROCESS_JUDGE2 == k);
						}
						break;
					
					// OR検索
					case ReadConfig.AndOr.Or:
						q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<STOCK>(keys, "PROCESS_JUDGE2"));
						break;
				}
			}

			return q;
		}

		#endregion // 検索条件(顧客専用項目)の追加





		public void setAllIsSelectedState(bool sts)
        {
#if false // 2018/08/16 M.Hoshino del DBマイグレーション
            foreach (var ele in db.WATCHLISTs
                 .Include(e => e.WK_DSP_WATCHLIST)
                 .Where(e => null != e.WK_DSP_WATCHLIST))
            {
                ele.WK_DSP_WATCHLIST.IsSelected = sts;
            }
#else // 2018/08/16 M.Hoshino del DBマイグレーション
            throw new NotImplementedException("2018/08/16 M.Hoshino del DBマイグレーション");
#endif // 2018/08/16 M.Hoshino del DBマイグレーション
        }

    }
}
