using Microsoft.AspNetCore.Identity;
using RazorPagesLearning.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// ポリシーサービス
    /// </summary>
    public class PolicyService : DBServiceBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// コンストラクタ
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public PolicyService(
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
            /// 名前
            /// </summary>
            public POLICY.PASSWORD_POLICY NAME;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 種別とメッセージ
        /// データ更新時の引数受け渡しに使用する
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public class NameValue
        {
            /// <summary>
            /// 名前
            /// </summary>
            public POLICY.PASSWORD_POLICY NAME { get; set; }

            /// <summary>
            /// 数値
            /// </summary>
            public int VALUE { get; set; }
        }

        // 取得
        public Result.ExecutionResult<POLICY> read(ReadConfig readConfig)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<POLICY>((ret) =>
            {
                ret.result = db.POLICies
                 .Where(e => e.NAME == (POLICY.PASSWORD_POLICY)readConfig.NAME)
                 .FirstOrDefault();
                ret.succeed = true;
            });
        }

        /// <summary>
        /// セキュリティポリシーを追加する
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public async Task<bool> add(POLICY target)
        {
            await this.setBothManagementInformation(target);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// セキュリティポリシーを検索する
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public List<POLICY> ReadAll()
        {
            var ret = db.POLICies.ToList();
            return ret;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// セキュリティポリシーを更新する
        /// </summary>
        /// <param name="db"></param>
        /// <param name="></param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<bool> UpdateAsync(NameValue namevalue)
        {
            //更新前の数値を取得
            var policy = db.POLICies.SingleOrDefault(e => e.NAME == namevalue.NAME);
            //throw new NotImplementedException();

            if (null != policy)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    // 指定された数値に更新する
                    policy.VALUE = namevalue.VALUE;

                    // 更新時に付与する共通情報を追加
                    await setUpdateManagementInformation(policy);

                    // 更新を実行
                    await db.SaveChangesAsync();
                    transaction.Commit();
                }
            }

            return true;
        }
    }
}
