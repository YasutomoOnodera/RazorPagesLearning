using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesLearning.Data.Models
{
    /// <summary>
    /// 表示するテーブル種別
    /// </summary>
    public enum ViewTableType
    {
        // memo：チェックボックスのチェック内容を保存したい画面を、種別として増やす必要あり
        //       追加は一番下に行い、値は変更しないこと。

        /// <summary>
        /// 検索テーブル
        /// </summary>
        Search = 1,

        /// <summary>
        /// ウォッチリスト
        /// </summary>
        Watchlist = 2,

        /// <summary>
        /// 作業依頼指示
        /// </summary>
        Request = 3,

		/// <summary>
		/// 集配先マスタ
		/// </summary>
		DeliveryAdmin = 4,





        /// <summary>
        /// 集配先選択
        /// 一覧はあるが、チェックボックスがないので、ここに必要かどうかは実装してから決めるので99としておく
        /// </summary>
        WkRequest = 99,

    }
}
