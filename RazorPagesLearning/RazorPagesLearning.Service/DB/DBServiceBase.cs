using Microsoft.AspNetCore.Identity;
using RazorPagesLearning.Data.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using static RazorPagesLearning.Service.User.UserService;

namespace RazorPagesLearning.Service.DB
{
    public static class DBServiceBaseFunctions
    {
        /// <summary>
        /// 衝突検知用タイムスタンプを文字列に変換する
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string timestampToString(this RazorPagesLearning.Data.Models.COMMON_MANAGEMENT_INFORMATION target)
        {
            if (null != target.Timestamp)
            {            // byte型配列を16進数の文字列に変換
                System.Text.StringBuilder result = new System.Text.StringBuilder();
                foreach (byte b in target.Timestamp)
                {
                    result.Append(b.ToString("x2"));
                }

                return result.ToString();
            }
            else
            {
                return "";
            }
        }

    }

    /// <summary>
    /// DBアクセスサービスの基底クラス
    /// </summary>
    public abstract class DBServiceBase
    {
        /// <summary>
        /// DBアクセスコンテクスト
        /// </summary>
        public RazorPagesLearning.Data.RazorPagesLearningContext db { get; protected set; }

        /// <summary>
        /// ログイン中のユーザー情報
        /// </summary>
        public ClaimsPrincipal user { get; private set; }


        /// <summary>
        /// サービスの構築時にuserがnullの場合がある。
        /// (PageModelのコンストラクト時。
        /// これは、ASP側の仕様。)
        ///
        /// 意図的に更新をかける。
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void updateUser(ClaimsPrincipal ref_user) {
            this.user = ref_user;
        }

        /// <summary>
        /// ログイン情報マネージャー
        /// </summary>
        public SignInManager<IdentityUser> signInManager { get; private set; }

        /// <summary>
        /// ユーザー情報マネージャー
        /// </summary>
        public UserManager<IdentityUser> userManager { get; private set; }

        public DBServiceBase(RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> signInManager,
                UserManager<IdentityUser> userManager)
        {
            this.db = ref_db;
            this.user = ref_user;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        /// <summary>
        /// コンストラクタで生成すると、継承元で継承先のインスタンスを起こすために、
        /// 関数呼び出しが無限ループしてしまう。
        /// このため、必要な時にインスタンスを生成するように修正した。
        /// </summary>
        private User.UserService _userServiceCache;

        /// <summary>
        /// ユーザーサービス情報
        /// </summary>
        private User.UserService userService
        {
            get
            {
                if (null != this.db &&
                    null != this.user &&
                    null != this.signInManager &&
                    null != userManager)
                {
                    if (null == _userServiceCache)
                    {
                        this._userServiceCache = new User.UserService(
                            this.db,
                            this.user,
                            this.signInManager,
                            this.userManager);
                    }
                    return _userServiceCache;
                }
                else
                {

                    return null;
                }
            }
        }

        /// <summary>
        /// ログイン中のユーザー情報を読み取る
        /// </summary>
        /// <returns></returns>
        public async Task<USER_ACCOUNT> readLoggedUserInfo()
        {
            if (null != this.user)
            {
                //ユーザー情報を取り出す
                return await this.userService?.read(new ReadFromClaimsPrincipalConfig
                {
                    userInfo = this.user
                });
            }
            return null;
        }

        /// <summary>
        /// ログイン中のユーザーIDを読み取る
        /// </summary>
        /// <returns></returns>
        private async Task<Int64> readLoggedUserId()
        {
            var r = await readLoggedUserInfo();
            if (null != r)
            {
                return r.ID;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 生成時に付与される共通情報を追加する
        /// </summary>
        /// <param name="target"></param>
        public async Task<bool> setCreateManagementInformation(RazorPagesLearning.Data.Models.COMMON_MANAGEMENT_INFORMATION target)
        {
            //ユーザー情報を取り出す
            target.CREATED_AT = DateTimeOffset.Now;

            {
                var u = target as MODIFY_USER_INFORMATION;
                if (null != u)
                {
                    u.CREATED_USER_ACCOUNT_ID = await readLoggedUserId();
                }
            }

            return true;
        }

        /// <summary>
        /// 生成時に付与される共通情報を追加する
        /// </summary>
        /// <param name="target"></param>
        public async Task<bool> setCreateManagementInformation(IEnumerable<RazorPagesLearning.Data.Models.COMMON_MANAGEMENT_INFORMATION> targets)
        {
            var now = DateTimeOffset.Now;
            var uId = await readLoggedUserId();

            foreach (var ele in targets)
            {
                ele.CREATED_AT = now;

                {
                    var u  = ele as MODIFY_USER_INFORMATION;
                    if (null != u)
                    {
                        u.CREATED_USER_ACCOUNT_ID = uId;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 更新時に付与される共通情報を追加する
        /// </summary>
        /// <param name="target"></param>
        public async Task<bool> setUpdateManagementInformation(RazorPagesLearning.Data.Models.COMMON_MANAGEMENT_INFORMATION target)
        {
            target.UPDATED_AT = DateTimeOffset.Now;

            {
                var u = target as MODIFY_USER_INFORMATION;
                if (null != u)
                {
                    u.UPDATED_USER_ACCOUNT_ID = await readLoggedUserId();
                }
            }

            return true;
        }

        /// <summary>
        /// 更新時に付与される共通情報を追加する
        /// </summary>
        /// <param name="target"></param>
        public async Task<bool> setUpdateManagementInformation(IEnumerable<RazorPagesLearning.Data.Models.COMMON_MANAGEMENT_INFORMATION> targets)
        {
            var now = DateTimeOffset.Now;
            var uId = await readLoggedUserId();

            foreach (var ele in targets)
            {
                ele.UPDATED_AT = now;

                {
                    var u = ele as MODIFY_USER_INFORMATION;
                    if (null != u)
                    {
                        u.UPDATED_USER_ACCOUNT_ID = uId;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 新規生成時に、生成時用、更新時に付与される共通情報を追加する
        /// </summary>
        /// <param name="target"></param>
        public async Task setBothManagementInformation(RazorPagesLearning.Data.Models.COMMON_MANAGEMENT_INFORMATION target)
        {
            var now = DateTimeOffset.Now;
            var uId = await readLoggedUserId();

            target.UPDATED_AT = now;
            target.CREATED_AT = now;
            {
                var u = target as MODIFY_USER_INFORMATION;
                if (null != u)
                {
                    u.UPDATED_USER_ACCOUNT_ID = uId;
                    u.CREATED_USER_ACCOUNT_ID = uId;
                }
            }
        }

        /// <summary>
        /// 新規生成時に、生成時用、更新時に付与される共通情報を追加する
        /// </summary>
        /// <param name="target"></param>
        public async Task setBothManagementInformation(IEnumerable<RazorPagesLearning.Data.Models.COMMON_MANAGEMENT_INFORMATION> targets)
        {
            var now = DateTimeOffset.Now;
            var uId = await readLoggedUserId();

            foreach (var ele in targets)
            {
                ele.UPDATED_AT = now;
                ele.CREATED_AT = now;
                {
                    var u = ele as MODIFY_USER_INFORMATION;
                    if (null != u)
                    {
                        u.UPDATED_USER_ACCOUNT_ID = uId;
                        u.CREATED_USER_ACCOUNT_ID = uId;
                    }
                }
            }
        }

    }

    /// <summary>
    ///　DBアクセス規定サービスにおいて、データの更新をメソッドを定義する
    /// </summary>
    public abstract class UpdatableDBServiceBase : DBServiceBase
    {
        public UpdatableDBServiceBase(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {
        }

        public void updateRazorPagesLearningContext(RazorPagesLearning.Data.RazorPagesLearningContext ref_db)
        {
            this.db = ref_db;
        }

    }
}
