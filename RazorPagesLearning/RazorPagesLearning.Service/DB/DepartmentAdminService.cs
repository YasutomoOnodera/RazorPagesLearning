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
    ///　部課マスタサービス
    /// </summary>
    public class DepartmentAdminService : DBServiceBase
    {
        private ShipperAdminService shipperAdminService;

        public DepartmentAdminService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
            this.shipperAdminService = new ShipperAdminService(this);
        }

        /// <summary>
        /// データを追加する
        /// </summary>
        /// <param name="db"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public Result.DefaultExecutionResult add(IEnumerable<DEPARTMENT_ADMIN> target)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.doOperationWithErrorManagement((ret) =>
            {
                #region 存在していないコードを登録されているか
                foreach (var ele in target)
                {
                    var r = this.shipperAdminService.read(new ShipperAdminService.ReadConfig
                    {
                        SHIPPER_CODE = ele.SHIPPER_CODE
                    });
                    if (false == r.succeed)
                    {
                        ret.succeed = false;
                        ret.errorMessages = r.errorMessages;
                        ret.exception = r.exception;
                        goto FUNC_END;
                    }
                    else
                    {
                        if (null == r.result)
                        {
                            //存在しないコードだったので終了とする
                            ret.succeed = false;
                            ret.errorMessages = new List<string>();
                            ret.errorMessages.Add("荷主マスタに存在しない荷主コードが指定されています。");
                            goto FUNC_END;
                        }
                    }
                }
                #endregion

                //データ追加
                this.db.DEPARTMENT_ADMINs.AddRange(target);

                //実行成功
                ret.succeed = true;

                FUNC_END:;

            });
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

            /// <summary>
            /// 部課コード
            /// </summary>
            public string DEPARTMENT_CODE;
        }

        /// <summary>
        /// データを読み取り
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public Result.ExecutionResult<IQueryable<DEPARTMENT_ADMIN>> read(ReadConfig readConfig)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.DoOperationWithErrorManagement<IQueryable<DEPARTMENT_ADMIN>>((ret) =>
            {
                ret.result = db.DEPARTMENT_ADMINs.AsQueryable();
                if (null != readConfig.SHIPPER_CODE)
                {
                    ret.result = ret.result.Where(e => e.SHIPPER_CODE == readConfig.SHIPPER_CODE);
                }
                if (null != readConfig.DEPARTMENT_CODE)
                {
                    ret.result = ret.result.Where(e => e.DEPARTMENT_CODE == readConfig.DEPARTMENT_CODE);
                }
                ret.succeed = true;
            });
        }

		/// <summary>
		/// 荷主コード、部課コードより、部課マスタを検索する
		/// </summary>
		/// <param name="shipperCode"></param>
		/// <param name="departmentCodes"></param>
		/// <returns></returns>
		public Result.ExecutionResult<IQueryable<DEPARTMENT_ADMIN>> read(string shipperCode, List<String>departmentCodes)
		{
			return Helper.ServiceHelper.DoOperationWithErrorManagement<IQueryable<DEPARTMENT_ADMIN>>((ret) =>
			{
				ret.result = db.DEPARTMENT_ADMINs.AsQueryable();
				ret.result = ret.result.Where(e => e.SHIPPER_CODE == shipperCode);
				ret.result = ret.result.Where(Helper.QueryHelper.makeQueryOfKeywordJoinedByOr<DEPARTMENT_ADMIN>(departmentCodes, "DEPARTMENT_CODE"));
				ret.succeed = true;
			});
		}


        /// <summary>
        /// データを読み取り
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public Result.ExecutionResult<DEPARTMENT_ADMIN> read(Int64 refId)
        {
#if false // 2018/08/16 M.Hoshino del DBマイグレーション
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.doOperationWithErrorManagement<DEPARTMENT_ADMIN>((ret) =>
            {
                ret.result = db.DEPARTMENT_ADMINs.Where(e=>e.ID == refId).FirstOrDefault();
                ret.succeed = true;
            });
#else // 2018/08/16 M.Hoshino del DBマイグレーション
            throw new NotImplementedException("2018/08/16 M.Hoshino del DBマイグレーション");
#endif // 2018/08/16 M.Hoshino del DBマイグレーション
        }

        /// <summary>
        /// データを読み取り
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public Result.ExecutionResult<IQueryable<DEPARTMENT_ADMIN>> read(IEnumerable<Int64> refId)
        {
#if false // 2018/08/16 M.Hoshino del DBマイグレーション
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper.doOperationWithErrorManagement<IQueryable<DEPARTMENT_ADMIN>>((ret) =>
            {
                ret.result = db.DEPARTMENT_ADMINs.Where(e => refId.Contains(e.ID) );
                ret.succeed = true;
            });
#else // 2018/08/16 M.Hoshino del DBマイグレーション
            throw new NotImplementedException("2018/08/16 M.Hoshino del DBマイグレーション");
#endif // 2018/08/16 M.Hoshino del DBマイグレーション
        }


    }
}
