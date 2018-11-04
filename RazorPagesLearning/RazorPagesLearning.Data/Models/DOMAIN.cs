using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// ドメイン
    /// </summary>
    [Serializable]
    public class DOMAIN : MODIFY_USER_INFORMATION
    {
		/// <summary>
		/// 種別
		/// </summary>
		public static class Kind
		{
			#region // STOCK系

			/// <summary>
			/// 依頼内容
			/// </summary>
			//public const string STOCK_REQUEST = "00010001";   // 2018/09/12 delete

			/// <summary>
			/// 区分1
			/// </summary>
			public const string STOCK_CLASS1 = "00010002";

			/// <summary>
			/// 区分2
			/// </summary>
			public const string STOCK_CLASS2 = "00010003";

			/// <summary>
			/// ステータス
			/// </summary>
			public const string STCOK_STATUS = "00010004";

			/// <summary>
			/// 著作権
			/// </summary>
			public const string STOCK_COPYRIGHT = "00010005";

			/// <summary>
			/// 契約書
			/// </summary>
			public const string STOCK_CONTRACT = "00010006";

			/// <summary>
			/// 処理判定
			/// </summary>
			public const string STOCK_PROCESS_JUDGE = "00010007";

			/// <summary>
			/// 在庫種別
			/// </summary>
			public const string STOCK_STOCK_KIND = "00010008";

			/// <summary>
			/// 在庫ステータス
			/// </summary>
			public const string STOCK_STOCK_STATUS = "00010009";

			/// <summary>
			/// 処理種別
			/// </summary>
			public const string STOCK_PROCESS_KIND = "00010010";

			#endregion // STOCK系

			#region // REQUEST系

			/// <summary>
			/// 依頼内容
			/// </summary>
			public const string REQUEST_REQUEST = "00020001";

			#endregion // REQUEST系

			#region // 集配先系

			/// <summary>
			/// 便
			/// </summary>
			public const string DELIVERY_FLIGHT = "00080001";

			/// <summary>
			/// 指定日時(集配先の便)
			/// </summary>
			public const string DELIVERY_DATETIME = "00080002";

			/// <summary>
			/// 表示
			/// </summary>
			public const string DELIVERY_DISPLAY = "00080003";

			#endregion // 集配先系

			#region // WMS系

			/// <summary>
			/// WMS状態
			/// </summary>
			public const string WMS_ = "00090000";

			#endregion // WMS系

			#region // 自社便系

			/// <summary>
			/// ステータス(集配依頼)
			/// </summary>
			public const string FLIGHT_STAUS = "00100000";

			#endregion // 自社便系
		}

		#region コード：STOCK系

		/// <summary>
		/// STOCK系 在庫ステータスのDOMAIN.CODE
		/// </summary>
		public static class StockStatusCode
		{
			/// <summary>
			/// 登録待
			/// </summary>
			public const string REGIST_WAITING = "10";

			/// <summary>
			/// 在庫中
			/// </summary>
			public const string STOCK = "20";

			/// <summary>
			/// 出荷中
			/// </summary>
			public const string SHIPPING = "30";

			/// <summary>
			/// 廃棄済
			/// </summary>
			public const string SCRAP = "40";

			/// <summary>
			/// 抹消済
			/// </summary>
			public const string PERIPHERY = "50";

			/// <summary>
			/// 依頼中
			/// </summary>
			public const string REQUEST = "60";

			/// <summary>
			/// 複数品
			/// </summary>
			public const string MULTIPLE = "70";

			/// <summary>
			/// 資材
			/// </summary>
			public const string MATERIAL = "80";

			/// <summary>
			/// ゼロ在庫を表示する
			/// </summary>
			public const string DISP_ZERO_STOCK = "90";

			// -----------------------------------------------------
			// TODO: 在庫ステータスのコードをWMSに合わせて整理する
			/// <summary>
			/// 集荷予定
			/// </summary>
			public const string RESERVE_CARGO = "39";

			/// <summary>
			/// 出荷予約
			/// </summary>
			public const string RESERVE_SHIPPING = "41";

			/// <summary>
			/// 再入庫予定
			/// </summary>
			public const string RESERVE_RESTOCK = "42";

			/// <summary>
			/// 廃棄予定
			/// </summary>
			public const string RESERVE_SCRAP = "43";

			/// <summary>
			/// 抹消予定
			/// </summary>
			public const string RESERVE_PERIPHERY = "44";
			// -----------------------------------------------------
		}

		/// <summary>
		/// STOCK系 在庫種別のDOMAIN.CODE
		/// </summary>
		public static class StockStockKindCode
		{
			/// <summary>
			/// 単品
			/// </summary>
			public const string SINGLE = "10";

			/// <summary>
			/// 複数品
			/// </summary>
			public const string MULTIPLE = "20";

			/// <summary>
			/// 資材
			/// </summary>
			public const string MATERIAL = "30";
		}

		#endregion // コード：STOCK系

		#region コード：REQUEST系

		/// <summary>
		/// REQUEST系 依頼内容のDOMAIN.CODE
		/// </summary>
		public static class RequestRequestCode
		{
			/// <summary>
			/// 新規入庫(登録ユーザ)
			/// </summary>
			public const string NEW_USER = "10";

			/// <summary>
			/// 新規入庫
			/// </summary>
			public const string NEW = "20";

			/// <summary>
			/// 出荷
			/// </summary>
			public const string SHIPPING = "30";

			/// <summary>
			/// 再入庫
			/// </summary>
			public const string RESTOCK = "40";

			/// <summary>
			/// 廃棄
			/// </summary>
			public const string SCRAP = "50";

			/// <summary>
			/// 抹消(永久出庫)
			/// </summary>
			public const string PERIPHERY = "60";

			/// <summary>
			/// 抹消(データ抹消)
			/// </summary>
			public const string PERIPHERY_DATA = "70";

			/// <summary>
			/// 資材販売
			/// </summary>
			public const string MATERIAL = "80";
		}

		#endregion // コード：REQUEST系

		#region コード：集配系

		/// <summary>
		/// 集配系 便のDOMAIN.CODE
		/// </summary>
		public static class DeliverlyFlightCode
		{
			/// <summary>
			/// 依頼主様取引
			/// </summary>
			public const string SHIPPER = "A";

			/// <summary>
			/// バイク便(TNL手配)
			/// </summary>
			public const string BIKE_TNL = "B";

			/// <summary>
			/// バイク便(依頼主様手配)
			/// </summary>
			public const string BIKE_SHIPPER = "C";

			/// <summary>
			/// ヤマト代引
			/// </summary>
			public const string YAMATO_DAIHIKI = "D";

			/// <summary>
			/// RazorPagesLearning便
			/// </summary>
			public const string RazorPagesLearning = "G";

			/// <summary>
			/// 近鉄物流
			/// </summary>
			public const string KINTETSU = "K";

			/// <summary>
			/// チャーター便(TNL手配)
			/// </summary>
			public const string CHARTER = "M";

			/// <summary>
			/// 佐川急便
			/// </summary>
			public const string SAGAWA = "S";

			/// <summary>
			/// ヤマト運輸
			/// </summary>
			public const string YAMATO = "Y";
		}

		/// <summary>
		/// 集配系 表示のDOMAIN.CODE
		/// </summary>
		public static class DeliveryDisplayCode
		{
			/// <summary>
			/// 未指定
			/// </summary>
			public const string BRANK = "0";

			/// <summary>
			/// ON
			/// </summary>
			public const string ON = "10";

			/// <summary>
			/// OFF
			/// </summary>
			public const string OFF = "20";
		}

		#endregion // コード：集配系

		/// <summary>
		/// 種別
		/// </summary>
		[StringLength(8)]
		[DisplayName("種別")]
		public string KIND { get; set; }

        /// <summary>
        /// 日本語の種別名
        /// </summary>
        public string JAPANESE_KIND { get; set; }

		/// <summary>
		/// コード
		/// </summary>
		[StringLength(8)]
		[DisplayName("コード")]
		public string CODE { get; set; }

		/// <summary>
		/// 値
		/// </summary>
		[Required]
		[StringLength(128)]
		[DisplayName("値")]
		public string VALUE { get; set; }

		/// <summary>
		/// 有効フラグ
		/// </summary>
		[Required]
		public bool VALID_FLAG { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        [Required]
        [DisplayName("削除フラグ")]
        public bool DELETE_FLAG { get; set; }
    }
}
