using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Claims;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// チェックボックスの状態を持つサービス
    /// </summary>
    public class WkTableSelectionSettingService : DBServiceBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// コンストラクタ
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public WkTableSelectionSettingService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {

        }


        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// チェックがついたチェックボックスの状態を取得
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type">画面種別</param>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public List<WK_TABLE_SELECTION_SETTING> getCheckStateList(long userId, ViewTableType type)
        {
            return db.WK_TABLE_SELECTION_SETTINGs
                .Where(e => e.USER_ACCOUNT_ID == userId)
                .Where(e => e.viewTableType == type)
                .Where(e => e.selected == true).ToList();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// チェックの情報を初期化する
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type">画面種別</param>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public void deleteCheckInfoAll(long userId, ViewTableType type)
        {
            var tmp = db.WK_TABLE_SELECTION_SETTINGs
                .Where(e => e.USER_ACCOUNT_ID == userId)
                .Where(e => e.viewTableType == type).ToList();
            db.WK_TABLE_SELECTION_SETTINGs.RemoveRange(tmp);
			db.SaveChanges();
        }
    }
}
