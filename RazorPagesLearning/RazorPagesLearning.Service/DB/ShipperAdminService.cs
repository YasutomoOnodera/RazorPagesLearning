using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorPagesLearning.Service.DB
{
    /// <summary>
    /// 荷主マスタサービス
    /// </summary>
    public class ShipperAdminService : DBServiceBase
    {
        public ShipperAdminService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }

        public ShipperAdminService(DBServiceBase ref_base) :
            base(ref_base.db, ref_base.user , ref_base.signInManager , ref_base.userManager)
        {
        }

        /// <summary>
        /// 読み取り設定
        /// </summary>
        public class ReadConfig
        {
            /// <summary>
            /// 荷主コード
            /// </summary>
            public string SHIPPER_CODE;
        }

        /// <summary>
        /// 指定された情報を読み取る
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public Result.ExecutionResult<SHIPPER_ADMIN> read(ReadConfig readConfig)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<SHIPPER_ADMIN>((ret) =>
          {
              ret.result = db.SHIPPER_ADMINs
               .Where(e => e.SHIPPER_CODE == readConfig.SHIPPER_CODE)
               .FirstOrDefault();
              ret.succeed = true;
          });
        }

        /// <summary>
        ///データを追加する
        /// </summary>
        /// <param name="db"></param>
        /// <param name="target"></param>
        public void add(IEnumerable<SHIPPER_ADMIN> target)
        {
            db.AddRange(target);
        }

        /// <summary>
        /// 荷主全件を取得
        /// </summary>
        /// <returns></returns>
        public IQueryable<SHIPPER_ADMIN> readList()
        {
            return db.SHIPPER_ADMINs.Where(x => x.DELETE_FLAG != true);
        }

    }
}
