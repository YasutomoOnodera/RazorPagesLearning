using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// ページネーション関係の設定
    /// </summary>
    public class TablePaginationSettingService : UpdatableDBServiceBase
    {
        public TablePaginationSettingService(
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
            /// ユーザーアカウントID
            /// </summary>
            public long USER_ACCOUNT_ID { get; set; }

            /// <summary>
            /// 対象テーブル種別
            /// </summary>
            public ViewTableType viewTableType { get; set; }
        }

        /// <summary>
        /// 指定された情報を読み取る
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public WK_TABLE_PAGINATION_SETTING read(ReadConfig readConfig)
        {
            return db.WK_TABLE_PAGINATION_SETTINGs                
                .Where(e=> (e.USER_ACCOUNT_ID == readConfig.USER_ACCOUNT_ID) &&
                (e.viewTableType == readConfig.viewTableType))
                .FirstOrDefault();
        }

    }
}
