﻿using RazorPagesLearning.Data.Models;
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
    /// 運送会社サービス
    /// </summary>
    public class TransportAdminService : DBServiceBase
    {

        public TransportAdminService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }


#if false

        /// <summary>
        /// 読み取り設定
        /// </summary>
        public class ReadConfig
        {
            /// <summary>
            /// 読み取り開始位置
            /// </summary>
            public int start;

            /// <summary>
            /// 取り出しデータ件数
            /// </summary>
            public int take;
        }

        /// <summary>
        /// 指定された情報を読み取る
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public static IQueryable<TRANSPORT_ADMIN> read(RazorPagesLearning.Data.RazorPagesLearningContext db, ReadConfig readConfig)
        {
            return db.TRANSPORT_ADMINs
                .Include(e => e.TRUCK_ADMINs)
                .Skip(readConfig.start)
                .Take(readConfig.take);
        }

#endif

        /// <summary>
        /// 全件読み取る
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public IQueryable<TRANSPORT_ADMIN> read()
        {
            return db.TRANSPORT_ADMINs
                .Include(e => e.TRUCK_ADMINs);
        }

        /// <summary>
        /// 運送会社コードで指定して読み取る
        /// </summary>
        /// <param name="db"></param>
        /// <param name="readConfig"></param>
        /// <returns></returns>
        public IQueryable<TRANSPORT_ADMIN> read(string refCODE)
        {
            return db.TRANSPORT_ADMINs
                .Include(e => e.TRUCK_ADMINs)
                .Where(e => e.CODE == refCODE);
        }

        /// <summary>
        /// 追加
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<Result.ExecutionResult<IEnumerable<TRANSPORT_ADMIN>>> add(IEnumerable<TRANSPORT_ADMIN> target)
        {
            #region ローカル関数

            //車両マスタが無かったら追加する
            async Task<bool> checkAndSetUpTRUCK_ADMIN(TRANSPORT_ADMIN ref_TRANSPORT_ADMIN)
            {
                var num = 0;
                if (null != ref_TRANSPORT_ADMIN.TRUCK_ADMINs)
                {
                    num = ref_TRANSPORT_ADMIN.TRUCK_ADMINs.Count();
                }
                switch (num)
                {
                    case 10:
                        {
                            //指定数存在するので何もしない
                            return true;
                        }
                    case 0:
                        {
                            var t = new List<TRUCK_ADMIN>();
                            //要素0の場合、追加する
                            foreach (int i in Enumerable.Range(1, 10))
                            {
                                //ref_TRANSPORT_ADMIN.TRUCK_ADMINs.Add();
                                t.Add(new TRUCK_ADMIN
                                {
                                    TRUCK_MANAGE_NUMBER = i,
                                    TRANSPORT_ADMIN_CODE = ref_TRANSPORT_ADMIN.CODE,
                                    TRANSPORT_ADMIN = ref_TRANSPORT_ADMIN
                                });
                            }
                            await this.setBothManagementInformation(t);

                            ref_TRANSPORT_ADMIN.TRUCK_ADMINs = t;
                            return true;
                        }
                    default:
                        {
                            //このパターンが発生した場合、
                            //途中でデータが正規の方法以外で変更されている可能性あり。
                            throw new ApplicationException("データが非正規の方法で変更されています。");
                        }
                }

            }

            #endregion

            return await RazorPagesLearning.Service.DB.Helper.ServiceHelper
                .doOperationWithErrorManagementAsync<IEnumerable<TRANSPORT_ADMIN>>(async (ret) =>
           {
               //運送会社必ず、0空10の車両マスタを持っているバス
               //存在しない場合、追加しておく
               foreach (var ele in target)
               {
                   await checkAndSetUpTRUCK_ADMIN(ele);
               }

               //データ追加
               db.TRANSPORT_ADMINs.AddRange(target);

               //実行成功
               ret.succeed = true;

               return ret;
           });
        }

    }
}
