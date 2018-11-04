using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static RazorPagesLearning.Service.DB.PolicyService;

namespace RazorPagesLearning.Service.DB
{
    public class CalendarAdminService : DBServiceBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// コンストラクタ
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public CalendarAdminService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 休日
        /// データ更新時の引数受け渡しに使用する
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public class Holiday
        {
            /// <summary>
            /// 休日
            /// </summary>
            public List<DateTimeOffset> HOLIDAYs{ get; set; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 休日を追加する
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<bool> Add(CALENDAR_ADMIN target)
        {
            await this.setBothManagementInformation(target);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// カレンダーを検索する
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public List<CALENDAR_ADMIN> ReadAll()
        {
            var ret = db.CALENDAR_ADMINs.ToList();
            return ret;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 指定日以降のカレンダーを検索する
        /// </summary>
        /// <param name="target">指定日</param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public List<CALENDAR_ADMIN> ReadFrom(DateTimeOffset target)
        {
            var ret = db.CALENDAR_ADMINs
                .Where(e => e.HOLIDAY >= target)
                .ToList();
            return ret;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 休日を追加
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<bool> add(List<CALENDAR_ADMIN> list)
        {
            var user = await readLoggedUserInfo();

            await this.setBothManagementInformation(list);
            this.db.CALENDAR_ADMINs.AddRange(list);

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 休日を更新する
        /// </summary>
        /// <param name="db"></param>
        /// <param name="></param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<bool> UpdateAsync(Holiday holiday)
        {           
            var user = await readLoggedUserInfo();

            var q = db.CALENDAR_ADMINs;

            // トランザクションの開始
            using (var transaction = db.Database.BeginTransaction())
            {
                // DBから休日をすべて削除
                db.CALENDAR_ADMINs.RemoveRange(q);
                await db.SaveChangesAsync();

                int elementCount = holiday.HOLIDAYs.Count();
                for (int i = 0; i < elementCount; i++)
                {
                    List<CALENDAR_ADMIN> list = new List<CALENDAR_ADMIN>();

                    // 引数の内容を設定する
                    CALENDAR_ADMIN e = new CALENDAR_ADMIN
                    {
                        HOLIDAY = holiday.HOLIDAYs[i]
                    };

                    // 追加時に付与する共通情報を追加
                    await setBothManagementInformation(e);

                    // 休日を追加する
                    db.CALENDAR_ADMINs.Add(e);

                    // 更新を実行
                    await db.SaveChangesAsync();
                }

                // 処理内容をコミットする
                transaction.Commit();
            }
            return true;
        }
    }
}