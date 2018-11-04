using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
    ////////////////////////////////////////////////////////////////////////////////////////////////
    /// メッセージマスタサービス
    /// 
    /// ユーザー関係の処理はシステム独自でユーザー情報を保持しているものと、
    /// ASP.NET core Identity側で値を持っているもの二種類が存在する。
    /// 別々で処理すると、煩雑であるため、関連処理はこのクラスにまとめる。
    /// 
    ////////////////////////////////////////////////////////////////////////////////////////////////
    public class MessageAdminService : DBServiceBase
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
        public MessageAdminService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 読み取り設定
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public class ReadConfig
        {
            /// <summary>
            /// 種別
            /// </summary>
            public MESSAGE_ADMIN.MESSAGE_KIND KIND;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 種別とメッセージ
        /// データ更新時の引数受け渡しに使用する
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public class KindMessage
        {
            /// <summary>
            /// 種別
            /// </summary>
            public MESSAGE_ADMIN.MESSAGE_KIND kind { get; set; }

            /// <summary>
            /// メッセージ
            /// </summary>
            public string message { get; set; }
        }

        // 取得
        public Result.ExecutionResult<MESSAGE_ADMIN> Read(ReadConfig readConfig)
        {
            return Helper.ServiceHelper.DoOperationWithErrorManagement<MESSAGE_ADMIN>((ret) =>
            {
                ret.result = db.MESSAGE_ADMINs
                 .Where(e => e.KIND == readConfig.KIND)
                 .FirstOrDefault();
                ret.succeed = true;
            });
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// メッセージを検索する
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public List<MESSAGE_ADMIN> ReadAll()
        {
            var ret = db.MESSAGE_ADMINs.ToList();
            return ret;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// メッセージを更新する
        /// </summary>
        /// <param name="db"></param>
        /// <param name="kindMessage"></param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<bool> Update(KindMessage kindMessage)
        {
            // 更新前のメッセージを取得
            var messageAdmin = db.MESSAGE_ADMINs.SingleOrDefault(e => e.KIND == kindMessage.kind);

            if (null != messageAdmin)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    // 指定されたメッセージに更新する
                    messageAdmin.MESSAGE = kindMessage.message;

                    // 更新時に付与する共通情報を追加
                    await setUpdateManagementInformation(messageAdmin);

                    // 更新を実行
                    await db.SaveChangesAsync();
                    transaction.Commit();
                }
            }
            else
            {
                throw new InvalidOperationException("メッセージマスタが初期化されていない状態で更新が行われました。");
            }

            return true;
        }
    }
}
