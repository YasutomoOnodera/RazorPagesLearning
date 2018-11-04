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
    /// <summary>
    /// Domainのコード関連のサービスクラス
    /// </summary>
    public class DomainService : DBServiceBase
    {
        public DomainService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }

        /// <summary>
        /// 読み取り設定
        /// </summary>
        public class ReadConfig
        {
            /// <summary>
            /// 種別
            /// </summary>
            public string KIND;

            /// <summary>
            /// コード
            /// </summary>
            public string CODE;
        }

        /// <summary>
        /// KINDからコード一覧を取得
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public IEnumerable<DOMAIN> getValueList(String kind)
        {

            return db.DOMAINs
                .Where(e => e.KIND == kind)
                .ToList();

        }

        /// <summary>
        /// KINDとコードから値を取得
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public DOMAIN getValue(String kind, String code)
        {

            return db.DOMAINs
                .Where(e => e.KIND == kind)
                .Where(e => e.CODE == code)
                .FirstOrDefault();

        }
        
        /// <summary>
        /// 取得
        /// </summary>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public Result.ExecutionResult<DOMAIN> read(ReadConfig readConfig)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<DOMAIN>((ret) =>
            {
                ret.result = db.DOMAINs
                 .Where(e => e.KIND == readConfig.KIND)
                 .Where(e => e.CODE == readConfig.CODE)
                 .FirstOrDefault();
                ret.succeed = true;
            });
        }

        /// <summary>
        /// 種別の一覧を取得
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public IQueryable<DOMAIN> getCodeList(string kind)
        {
            return db.DOMAINs.Where(e => e.KIND == kind)
                .Where(e => e.VALID_FLAG == true);
        }
        
    }
}
