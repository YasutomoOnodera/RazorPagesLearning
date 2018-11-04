﻿// 手動追加 using
using Microsoft.AspNetCore.Identity;

// 自動追加 using
using System.Security.Claims;

namespace RazorPagesLearning.Service.DB
{

    /// ////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// WMS集配先マスタサービス
    /// </summary>
    /// ////////////////////////////////////////////////////////////////////////////////////////////
    public class WmsDeriveryAdminService : DBServiceBase
    {
        public WmsDeriveryAdminService(
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
            /// 集配先コード
            /// </summary>
            public string _DestCode;
        }

#if false // 2018/08/16 M.Hoshino del DBマイグレーション
        /// ////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 指定されたWMS集配先マスタを読み取る
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        /// ////////////////////////////////////////////////////////////////////////////////////////
        public static Result.ExecutionResult<WMS_DERIVERY_ADMIN> Read(
            RazorPagesLearning.Data.RazorPagesLearningContext db, ReadConfig inReadConfig
            )
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.doOperationWithErrorManagement<WMS_DERIVERY_ADMIN>((ret) =>
            {
                ret.result = db.WMS_DERIVERY_ADMINs
                 .Where(e => e.DEST_CODE == inReadConfig._DestCode)
                 .FirstOrDefault();
                ret.succeed = true;
            });
        }
#endif // 2018/08/16 M.Hoshino del DBマイグレーション

#if false // 2018/08/16 M.Hoshino del DBマイグレーション
		/// ////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// WMS集配先マスタを追加する
		/// </summary>
		/// <param name="db"></param>
		/// <param name="target"></param>
		/// ////////////////////////////////////////////////////////////////////////////////////////
		public static void add(RazorPagesLearning.Data.RazorPagesLearningContext inDB, IEnumerable<WMS_DERIVERY_ADMIN> inTarget)
        {
            inDB.AddRange(inTarget);
        }
#endif // 2018/08/16 M.Hoshino del DBマイグレーション


		/// ////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 検索条件
		/// </summary>
		/// ////////////////////////////////////////////////////////////////////////////////////////
		public class SearchConfig
        {
            /// <summary>
            /// 検索文字列
            /// </summary>
            public string mSearchStr;

            /// <summary>
            /// 検索方法ANDまたはOR
            /// </summary>
            public string mSearchAndOr;
        }


#if false // 2018/08/16 M.Hoshino del DBマイグレーション
        /// ////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// WMS集配先マスタ一覧を取得する
        /// </summary>
        /// <param name="db"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        /// ////////////////////////////////////////////////////////////////////////////////////////
        public static Result.ExecutionResult<IQueryable<WMS_DERIVERY_ADMIN>> Search(RazorPagesLearning.Data.RazorPagesLearningContext db, SearchConfig config)
        {

            //======================================================================================
            // クエリの作成
            //======================================================================================
            var query = from wda in db.WMS_DERIVERY_ADMINs
                        join wdf in db.DERIVERY_DISP_FLAGs
                        on   wda.ID equals wdf.WMS_DERIVERY_ADMIN.ID
                        where wdf.DISPLAY_FLAG == true
                        select wda;

#if false
                        var
                            query = 
                                db.WMS_DERIVERY_ADMINs
                                .Join(
                                    db.DERIVERY_DISP_FLAGs,
                                    (i) => i.ID,
                                    (o) => o.WMS_DERIVERY_ADMIN.ID,
                                    (i, o) => i
                                );
#endif // false

            //======================================================================================
            // 動的検索条件：会社名の条件追加
            //======================================================================================
            //--------------------------------------------------------------------------------------
            // 検索文字列が設定されていない場合は、検索条件を設定しない
            //--------------------------------------------------------------------------------------
            if (config.mSearchStr == string.Empty)
            {

            }
            //--------------------------------------------------------------------------------------
            //検索文字列が設定されている場合は、検索条件を設定する
            //--------------------------------------------------------------------------------------
            else
            {
                //スペース区切りで連結して設定されているので、
                //分割して使用する
                char[] wDelimiterChars = { ' ', '　' };
                var wKeys = config.mSearchStr.Split(wDelimiterChars);

                if (config.mSearchAndOr == "AND")
                {
                    foreach (var wKey in wKeys)
                    {
                        query = query.Where(e => e.COMPANY.Contains(wKey));
                    }
                }
                else
                {
                    //orの場合unionでつなげる
                    //参考
                    //【LINQ】複数キーワードでAND検索・OR検索をする方法
                    //https://qiita.com/kuzira_mood/items/982bbc16bcb6eb1ecaa3
                    //【LINQ】２つの要素一覧を結合（和集合）するには（Unionセット演算子）
                    //https://keibalight.wordpress.com/2011/07/27/%E3%80%90linq%E3%80%91%EF%BC%92%E3%81%A4%E3%81%AE%E8%A6%81%E7%B4%A0%E4%B8%80%E8%A6%A7%E3%82%92%E7%B5%90%E5%90%88%EF%BC%88%E5%92%8C%E9%9B%86%E5%90%88%EF%BC%89%E3%81%99%E3%82%8B%E3%81%AB%E3%81%AF/
                    var wLp = 0;
                    foreach (var wKey in wKeys)
                    {
                        if (wLp == 0)
                        {
                            query = query.Where(e => e.COMPANY.Contains(wKey));
                        }
                        else
                        {
                            query = query.Union(query.Where(e => e.COMPANY.Contains(wKey)));
                        }

                        wLp++;
                    }
                }

            }

            //======================================================================================
            // 検索して成功を返す
            //======================================================================================
            return new Result.ExecutionResult<IQueryable<Data.Models.WMS_DERIVERY_ADMIN>>
            {
                result = query,
                succeed = true
            };
        }
#endif // 2018/08/16 M.Hoshino del DBマイグレーション

	}
}

