using AutoMapper;
using RazorPagesLearning.Data;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Service.DB.Helper;
using RazorPagesLearning.Service.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using RazorPagesLearningL.Service.Utility;

namespace RazorPagesLearning.Service.User
{
    /// <summary>
    /// ユーザー関係のサービス
    /// 
    /// ユーザー関係の処理はシステム独自でユーザー情報を保持しているものと、
    /// ASP.NET core Identity側で値を持っているもの二種類が存在する。
    /// 別々で処理すると、煩雑であるため、関連処理はこのクラスにまとめる。
    /// 
    /// </summary>
    public class UserService : DB.DBServiceBase
    {

        /// <summary>
        /// セキュリティポリシーアクセスサービス
        /// </summary>
        private readonly RazorPagesLearning.Service.DB.PolicyService policyService;

        /*
        private readonly RazorPagesLearning.Data.RazorPagesLearningContext db;
        private readonly ClaimsPrincipal user;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        */
        public UserService(
            RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            ClaimsPrincipal ref_user,
            SignInManager<IdentityUser> ref_signInManager,
                UserManager<IdentityUser> ref_userManager)
            : base(ref_db, ref_user, ref_signInManager, ref_userManager)
        {

            this.db = ref_db;
            /*
            this.user = ref_user;
            this.signInManager = ref_signInManager;
            this.userManager = ref_userManager;*/

            this.policyService = new DB.PolicyService(
                ref_db,
                ref_user,
                ref_signInManager,
                ref_userManager);
        }

        /// <summary>
        /// 追加時のコンテキスト
        /// </summary>
        public class ChangeConfig
        {
            /// <summary>
            /// 管理対象テーブル
            /// </summary>
            public RazorPagesLearning.Data.Models.USER_ACCOUNT USER_ACCOUNT;

            #region 権限が荷主の場合における必須事項

            /// <summary>
            /// デフォルトのユーザー部課コード
            /// </summary>
            public RazorPagesLearning.Data.Models.USER_DEPARTMENT DEFAULT_USER_DEPARTMENT;

            /// <summary>
            /// ユーザーが所属するユーザー部課コード
            /// </summary>
            public List<RazorPagesLearning.Data.Models.USER_DEPARTMENT> belongsUSER_DEPARTMENT;

            #endregion

            #region 権限が運送会社の場合における必須項目

            /// <summary>
            /// 運送会社情報
            /// </summary>
            public TRANSPORT_ADMIN TRANSPORT_ADMIN;

            #endregion

            /// <summary>
            /// パスワード
            /// </summary>
            public string password;

            /// <summary>
            /// 登録ユーザーアカウントID
            /// </summary>
            public Int64 createdUserAccountId { get; set; }

            /// <summary>
            /// 更新ユーザーアカウントID
            /// </summary>
            public Int64 updatedUserAccountId { get; set; }

        }


        /// <summary>
        /// ユーザー情報を消去する
        /// </summary>
        /// <param name="USER_ACCOUNT"></param>
        /// <returns></returns>
        private async Task<bool> removeUser(RazorPagesLearning.Data.Models.USER_ACCOUNT USER_ACCOUNT)
        {
            var str_ID = USER_ACCOUNT.ID.ToString();

            //ToDo :
            //以下、落ちないことだけ確認された状態。
            //ちゃんと試験が必要。

            ///例外が出たらIdentity側をロールバックする
            ///ユーザーをロックアウトする
            /// 
            /// 参考
            ///https://stackoverflow.com/questions/23977036/asp-net-mvc-5-how-to-delete-a-user-and-its-related-data-in-identity-2-0
            var ui = await userManager.FindByNameAsync(str_ID);

            if (null != ui)
            {
                //var logins = ui.Logins;
                var rolesForUser = await userManager.GetRolesAsync(ui);

                using (var transaction = db.Database.BeginTransaction())
                {
#if false
                    foreach (var login in logins.ToList())
                    {
                        await _userManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                    }
#endif

                    if (rolesForUser.Count() > 0)
                    {
                        foreach (var item in rolesForUser.ToList())
                        {
                            // item should be the name of the role
                            var result = await userManager.RemoveFromRoleAsync(ui, item);
                        }
                    }

                    await userManager.DeleteAsync(ui);
                    transaction.Commit();
                }
            }

            return true;
        }

        /// <summary>
        /// ユーザー部課はデフォルトユーザー部課と
        /// 所属するユーザー部課の2個の情報が存在する。
        /// DB登録に際して、これらの情報を結合して1個の部課情報とする。
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private IEnumerable<USER_DEPARTMENT> makeBelongsDEFAULT_USER_DEPARTMENT(ChangeConfig arg)
        {
            var tl = new List<USER_DEPARTMENT> { arg.DEFAULT_USER_DEPARTMENT };
            if (null != arg.belongsUSER_DEPARTMENT)
            {
                tl.AddRange(arg.belongsUSER_DEPARTMENT);
            }

            //ユーザーIDを割り付ける
            foreach (var e in tl)
            {
                e.USER_ACCOUNT_ID = arg.USER_ACCOUNT.ID;
            }

            //ユーザーの指定方法によっては、重複して入ることもあるので、
            //重複を除外しておく
            var deduplication = tl
                .GroupBy(e => new { e.SHIPPER_CODE, e.DEPARTMENT_CODE, e.USER_ACCOUNT_ID })
                .Select(group => group.First());

            return deduplication;
        }

        /// <summary>
        /// ユーザーアカウント追加
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<Result.DefaultExecutionResult> add(ChangeConfig arg)
        {
            #region ローカル関数

            //ユーザーアカウント情報を登録
            async Task<RazorPagesLearning.Data.Models.USER_ACCOUNT> LF_addUSERACCOUNT()
            {

                {
                    #region 登録前のデータチェック

                    //同一名称のユーザー
                    if (string.Empty == arg.USER_ACCOUNT.USER_ID)
                    {
                        throw new InvalidOperationException("ユーザーIDが空です。");
                    }

                    //DB登録済みか確認する
                    #region
                    {
                        //即時更新系の重複名称確認
                        if (0 != db.USER_ACCOUNTs.Where(ele => ele.USER_ID == arg.USER_ACCOUNT.USER_ID).Count())
                        {
                            throw new InvalidOperationException("指定されたユーザーIDは登録済みです。");
                        }

                        //遅延更新系の重複名称確認
                        if (0 != db.WK_USER_ACCOUNTs.Where(ele => ele.USER_ID == arg.USER_ACCOUNT.USER_ID).Count())
                        {
                            throw new InvalidOperationException("指定されたユーザーIDは登録済みです。");
                        }
                    }
                    #endregion

                    //新規のパスワード桁数
                    {
                        #region パスワード桁数の確認
                        var d = this.policyService.read(new DB.PolicyService.ReadConfig
                        {
                            NAME = POLICY.PASSWORD_POLICY.Digit
                        });
                        if (arg.password.Length < d.result.VALUE)
                        {
                            throw new InvalidOperationException($"パスワードは{d.result.VALUE}文字以上指定する必要があります。");
                        }
                        #endregion
                    }

                    //
                    {
                        #region　権限に関係する項目のチェック

                        {
                            #region 荷主権限

                            //ナビケーションによるリレーションがないので、
                            //ここではチェックしない

                            #endregion
                        }

                        {

                            #region 運送会社権限

                            if (null != arg.USER_ACCOUNT.TRANSPORT_ADMIN)
                            {
                                throw new ApplicationException("使い処理では運送会社情報を直接指定しないでください。");
                            }

                            #endregion
                        }

                        #endregion
                    }

                    #endregion
                }

                {
                    #region パスワードをHash化して設定する

                    arg.USER_ACCOUNT.PASSWORD_SALT = HelperFunctions.getSaltString();
                    arg.USER_ACCOUNT.PASSWORD = HelperFunctions.toHashString(arg.password, arg.USER_ACCOUNT.PASSWORD_SALT);
                    arg.USER_ACCOUNT.PASSWORD_UPDATED_AT = DateTimeOffset.Now;

                    //パスワード更新履歴にデータを追加する
                    {
                        if (null == arg.USER_ACCOUNT.PASSWORD_HISTORYs)
                        {
                            arg.USER_ACCOUNT.PASSWORD_HISTORYs = new List<PASSWORD_HISTORY>();
                        }

                        arg.USER_ACCOUNT.PASSWORD_HISTORYs.Add(
                            new RazorPagesLearning.Data.Models.PASSWORD_HISTORY
                            {
                                PASSWORD = arg.USER_ACCOUNT.PASSWORD,
                                PASSWORD_SALT = arg.USER_ACCOUNT.PASSWORD_SALT,
                                CREATED_USER_ACCOUNT_ID = arg.createdUserAccountId,
                                UPDATED_USER_ACCOUNT_ID = arg.updatedUserAccountId
                            });
                    }

                    #endregion
                }


                {
                    #region 更新登録者情報を追加する

                    arg.USER_ACCOUNT.CREATED_USER_ACCOUNT_ID = arg.createdUserAccountId;
                    arg.USER_ACCOUNT.UPDATED_USER_ACCOUNT_ID = arg.updatedUserAccountId;

                    #endregion
                }
                //新規に値をセットするので0IDは0とする。
                arg.USER_ACCOUNT.ID = 0;
                db.USER_ACCOUNTs.Add(arg.USER_ACCOUNT);
                //ユーザーIDが確定する
                await db.SaveChangesAsync();

                {
                    #region 権限が荷主項目の場合の追加処理
                    if (arg.USER_ACCOUNT.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperBrowsing ||
                        arg.USER_ACCOUNT.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperEditing)
                    {
                        //権限が荷主の場合、デフォルト部課コードを追加する
                        if (null == arg.DEFAULT_USER_DEPARTMENT)
                        {
                            throw new ArgumentException("権限が荷主であるがデフォルト部課コードが設定されていません。");
                        }

                        //ユーザー部課情報をDBに追加できる状態に整理する
                        var user_department = (new Func<List<USER_DEPARTMENT>>(() =>
                       {
                           #region ユーザー部課情報をDBに追加できる状態に整理する


                           //関係する部課マスタを関連付けする
                           var departmentAdminService = new DB.DepartmentAdminService(
                              db,
                              user,
                              signInManager,
                              userManager);

                           //部課情報はデフォルトの部課情報と、
                           //ユーザーが所属する部課情報が存在する。
                           //DBへの保存時にこれらを統合、重複排除して保存できる状態とする。
                           var t_user_department = makeBelongsDEFAULT_USER_DEPARTMENT(arg);

                           return t_user_department
                           .Select((e) =>
                           {
                               #region 関係する部課マスタを関連付けする
                               var errMsg = $"DEPARTMENT_CODE={e.DEPARTMENT_CODE} かつ SHIPPER_CODE={e.SHIPPER_CODE}を満たす部課マスタ履歴が存在しません。";

                               //関係する部課情報を取得
                               var daq = departmentAdminService.read(
                                  new DB.DepartmentAdminService.ReadConfig
                                  {
                                      DEPARTMENT_CODE = e.DEPARTMENT_CODE,
                                      SHIPPER_CODE = e.SHIPPER_CODE
                                  });
                               if (false == daq.succeed)
                               {
                                   throw new ArgumentException(errMsg);
                               }
                               var da = daq.result.FirstOrDefault();
                               e.DEPARTMENT_ADMIN = da ?? throw new ArgumentException(errMsg);

                               return e;
                               #endregion
                           })
                           .ToList();

                           #endregion
                       }))();

                        //DBに登録する
                        {
                            var userDepartmentService = new DB.UserDepartmentService(
                                db,
                                user,
                                signInManager,
                                userManager);

                            await userDepartmentService.add(user_department);

                            //ユーザー部課のIDが確定する
                            await db.SaveChangesAsync();
                        }

                        //デフォルト部課情報情報をユーザーに設定する
                        arg.USER_ACCOUNT.DEFAULT_DEPARTMENT_CODE = arg.DEFAULT_USER_DEPARTMENT.DEPARTMENT_CODE;
                        arg.USER_ACCOUNT.CURRENT_SHIPPER_CODE = arg.DEFAULT_USER_DEPARTMENT.DEPARTMENT_CODE;

                        await db.SaveChangesAsync();
                    }
                    #endregion
                }

                {
                    #region 権限が運送会社の場合の追加処理
                    if (arg.USER_ACCOUNT.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany)
                    {
                        var errMsg = "運送会社権限の場合、運送会社の指定が必須です。";
                        if (null == arg.TRANSPORT_ADMIN)
                        {
                            throw new ApplicationException(errMsg);
                        }

                        var transportAdminService = new DB.TransportAdminService(
                            db,
                            user,
                            signInManager,
                            userManager);
                        {
                            var q = transportAdminService.read(arg.TRANSPORT_ADMIN.CODE);
                            if (null == q.FirstOrDefault())
                            {
                                throw new ApplicationException("DB上に存在しない運送会社コードが指定されています。");
                            }
                        }

                        //追加
                        arg.USER_ACCOUNT.TRANSPORT_ADMIN = arg.TRANSPORT_ADMIN;
                        arg.USER_ACCOUNT.TRANSPORT_ADMIN_CODE = arg.TRANSPORT_ADMIN.CODE;

                        await db.SaveChangesAsync();
                    }
                    #endregion
                }

				{
					#region 権限に関わらず必要な情報の追加処理

					// ユーザー検索条件を追加する
					var searchConditionService = new DB.SearchConditionService(db, user, signInManager, userManager);
					await searchConditionService.Add(arg.USER_ACCOUNT.ID);

					// ユーザー表示設定を追加する
					var displaySettingService = new DB.DisplaySettingService(db, user, signInManager, userManager);
					await displaySettingService.add(arg.USER_ACCOUNT.ID);

					// まとめて保存
					await db.SaveChangesAsync();

					#endregion // 権限に関わらず必要な情報の追加処理
				}

				return arg.USER_ACCOUNT;
            };

            //Identity側の情報を登録する
            async Task<Result.DefaultExecutionResult> LF_addIdentityTask
                (RazorPagesLearning.Data.Models.USER_ACCOUNT userAccount)
            {
                var str_ID = userAccount.ID.ToString();
                {
                    var r = await userManager.FindByNameAsync(str_ID);
                    if (null != r)
                    {
                        return new Result.DefaultExecutionResult
                        {
                            succeed = false,
                            errorMessages = new List<string>
                            {
                                "登録済みのユーザーです。"
                            }
                        };
                    }
                }

                {
                    //管理者ユーザーが存在しない場合追加する
                    var cr = await userManager.CreateAsync(
                        new IdentityUser
                        {
                            UserName = str_ID
                        },
                        userAccount.PASSWORD);
                    if (false == cr.Succeeded)
                    {
                        return new Result.DefaultExecutionResult
                        {
                            succeed = false,
                            errorMessages = new List<string>
                                {
                                    "ユーザー追加に失敗しました。"
                                }
                        };
                    }

                    //権限を追加
                    {
                        var ui = await userManager.FindByNameAsync(str_ID);
                        var rr = await userManager.AddToRoleAsync(ui, arg.USER_ACCOUNT.PERMISSION.ToString());
                        if (false == rr.Succeeded)
                        {
                            return new Result.DefaultExecutionResult
                            {
                                succeed = false,
                                errorMessages = new List<string>
                                    {
                                        "ユーザー権限追加に失敗しました。"
                                    }
                            };
                        }
                    }
                }

                return new Result.DefaultExecutionResult
                {
                    succeed = true
                };
            }

            #endregion

            try
            {
                //ToDO : 二つの認証機構にまたがって登録するので、ロック処理に関してちゃんと考える必要あり。
                using (var transaction = db.Database.BeginTransaction())
                {
                    //まずはtrimする。
                    arg.USER_ACCOUNT.USER_ID = arg.USER_ACCOUNT.USER_ID.Trim();

                    //USER_ACCOUNTテーブル側の値設定
                    var uid = await LF_addUSERACCOUNT();

                    //Identity側の情報を登録する
                    var ret = await LF_addIdentityTask(uid);
                    if (false == ret.succeed)
                    {
                        throw new ApplicationException(String.Join(",", ret.errorMessages.ToString()));
                    }

                    //DB保存する
                    transaction.Commit();

                    return ret;
                }
            }
            catch (Exception e)
            {
                #region 問題が出たらロールバックする

                try
                {
                    await removeUser(arg.USER_ACCOUNT);
                }
                catch
                {
                    //失敗しても無視する
                }

                #endregion

                return Result.DefaultExecutionResult.makeError(e.Message);
            }
        }


        /// <summary>
        /// 更新種別
        /// </summary>
        public enum UpdateOperationType
        {
            /// <summary>
            /// 後で変更を適応する必要あり
            /// </summary>
            DelayChange,
            /// <summary>
            /// 即座に変更を適応する必要あり
            /// </summary>
            ImmediateChange
        }

        #region ユーザーアカウント情報更新処理用のローカル処理群

        /// <summary>
        /// ユーザーアカウント処理を更新する関数
        /// </summary>
        internal class UpdateHelper
        {
            public UpdateHelper(UserService ref_userService,
                UpdateConfig ref_config)
            {
                this.userService = ref_userService;
                this.config = ref_config;
            }

            #region メンバー変数

            /// <summary>
            /// ユーザーサービス情報
            /// </summary>
            private readonly UserService userService;

            /// <summary>
            /// 追加する情報
            /// </summary>
            private readonly UpdateConfig config;

            #endregion

            /// <summary>
            /// チェックありでデータの読み取りを行う
            /// </summary>
            private USER_ACCOUNT readUserWithCheck()
            {
                var dbu = this.userService.read(
                    config.USER_ACCOUNT.ID
                    );
                if (null == dbu)
                {
                    throw new ApplicationException
                        ($"{config.USER_ACCOUNT.ID}のユーザーは存在しません。");
                }

                return dbu;
            }


            /// <summary>
            /// 更新種別の検出
            /// </summary>
            public UpdateOperationType updateOperationTypeDetection()
            {
                //DB上のデータを取り込む
                var dbu = readUserWithCheck();

                //条件から判定する
                #region 確認条件から判定する

                if (config.USER_ACCOUNT.USER_ID != dbu.USER_ID)
                {
                    return UpdateOperationType.DelayChange;
                }

                if (config.USER_ACCOUNT.NAME != dbu.NAME)
                {
                    return UpdateOperationType.DelayChange;
                }

                if (config.USER_ACCOUNT.KANA != dbu.KANA)
                {
                    return UpdateOperationType.DelayChange;
                }

                if (config.USER_ACCOUNT.PERMISSION != dbu.PERMISSION)
                {
                    return UpdateOperationType.DelayChange;
                }

                if (config.USER_ACCOUNT.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperBrowsing ||
                    config.USER_ACCOUNT.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperEditing)
                {
                    //部課情報が変わっていたら、
                    //遅延更新対象とする
                    if (config.USER_ACCOUNT.DEFAULT_DEPARTMENT_CODE != dbu.DEFAULT_DEPARTMENT_CODE)
                    {
                        return UpdateOperationType.DelayChange;
                    }

                    if (config.USER_ACCOUNT.CURRENT_SHIPPER_CODE != dbu.CURRENT_SHIPPER_CODE)
                    {
                        return UpdateOperationType.DelayChange;
                    }

                    {
                        //関係する部課情報が変わっていたら遅延更新とする
                        //現在のユーザー部課を主キーで検索する
                        foreach (var ele in dbu.USER_DEPARTMENTs)
                        {
                            var r = dbu.USER_DEPARTMENTs
                                    .Where(e => e.USER_ACCOUNT_ID == ele.USER_ACCOUNT_ID &&
                                    e.SHIPPER_CODE == ele.SHIPPER_CODE &&
                                    e.DEPARTMENT_CODE == ele.DEPARTMENT_CODE)
                                    .FirstOrDefault();
                            if (null == r)
                            {
                                //発見出来なかったら変化したものと判定して遅延変更対象とする
                                return UpdateOperationType.DelayChange;
                            }
                        }

                        //部課情報はデフォルトの部課情報と、
                        //ユーザーが所属する部課情報が存在する。
                        //DBへの保存時にこれらを統合、重複排除して保存できる状態とする。
                        var t_user_department =
                            userService.makeBelongsDEFAULT_USER_DEPARTMENT(config);

                        if (dbu.USER_DEPARTMENTs.Count != (t_user_department.Count() + 1))
                        {
                            //件数が違ったら遅延変更とする                            
                            return UpdateOperationType.DelayChange;
                        }
                    }
                }

                if (config.isPasswordUpdateRequired)
                {
                    return UpdateOperationType.DelayChange;
                }

                #endregion

                //即時反映
                return UpdateOperationType.ImmediateChange;
            }

            /// <summary>
            /// ユーザーアカウント情報ページにおける変更項目をDBに適応する
            /// </summary>
            public async Task<bool> updateAccountInfoPage_ImmediateChange()
            {
                //更新対象となる情報を取得する
                var user = readUserWithCheck();

                //更新対象となっている項目だけuserにコピーする
                //ここでの更新対象は以下となる
                //
                //[更新対象]
                //社名
                //部署名
                //郵便番号
                //住所1
                //住所2
                //TEL
                //FAX
                //メールアドレス
                //デフォルト集配先コード
                //メールアドレス1
                //から
                //メールアドレス10
                user.COMPANY = config.USER_ACCOUNT.COMPANY;
                user.DEPARTMENT = config.USER_ACCOUNT.DEPARTMENT;
                user.ZIPCODE = config.USER_ACCOUNT.ZIPCODE;
                user.ADDRESS1 = config.USER_ACCOUNT.ADDRESS1;
                user.ADDRESS2 = config.USER_ACCOUNT.ADDRESS2;
                user.TEL = config.USER_ACCOUNT.TEL;
                user.FAX = config.USER_ACCOUNT.FAX;
                user.MAIL = config.USER_ACCOUNT.MAIL;
                user.DEFAULT_DELIVERY_ADMIN_ID = config.USER_ACCOUNT.DEFAULT_DELIVERY_ADMIN_ID;

                //メールアドレスを更新
                user.MAIL1 = config.USER_ACCOUNT.MAIL1;
                user.MAIL2 = config.USER_ACCOUNT.MAIL2;
                user.MAIL3 = config.USER_ACCOUNT.MAIL3;
                user.MAIL4 = config.USER_ACCOUNT.MAIL4;
                user.MAIL5 = config.USER_ACCOUNT.MAIL5;
                user.MAIL6 = config.USER_ACCOUNT.MAIL6;
                user.MAIL7 = config.USER_ACCOUNT.MAIL7;
                user.MAIL8 = config.USER_ACCOUNT.MAIL8;
                user.MAIL9 = config.USER_ACCOUNT.MAIL9;
                user.MAIL10 = config.USER_ACCOUNT.MAIL10;

                //更新ユーザー情報を追加
                await userService.setUpdateManagementInformation(user);
                return true;
            }

            /// <summary>
            /// 全体更新　即時系
            /// </summary>
            public async Task<bool> updateALL_ImmediateChange()
            {
                //更新対象となる情報を取得する
                var user = readUserWithCheck();

                //更新対象となっている項目だけuserにコピーする
                //ここでの更新対象は以下となる
                //
                //[更新対象]
                //社名
                //部署名
                //郵便番号
                //住所1
                //住所2
                //TEL
                //FAX
                //メールアドレス
                //
                //パスワードを無期限
                //次回ログイン時にパスワード変更を指定
                //運送会社
                //確認ダイアログを表示しない

                user.COMPANY = config.USER_ACCOUNT.COMPANY;
                user.DEPARTMENT = config.USER_ACCOUNT.DEPARTMENT;
                user.ZIPCODE = config.USER_ACCOUNT.ZIPCODE;
                user.ADDRESS1 = config.USER_ACCOUNT.ADDRESS1;
                user.ADDRESS2 = config.USER_ACCOUNT.ADDRESS2;
                user.TEL = config.USER_ACCOUNT.TEL;
                user.FAX = config.USER_ACCOUNT.FAX;
                user.MAIL = config.USER_ACCOUNT.MAIL;
                user.DEFAULT_DEPARTMENT_CODE = config.USER_ACCOUNT.DEFAULT_DEPARTMENT_CODE;
                user.LOGIN_ENABLE_FLAG = config.USER_ACCOUNT.LOGIN_ENABLE_FLAG;
                user.EXPIRE_FLAG = config.USER_ACCOUNT.EXPIRE_FLAG;
                user.PASSWORD_CHANGE_REQUEST = config.USER_ACCOUNT.PASSWORD_CHANGE_REQUEST;

                if ((user.PERMISSION == config.USER_ACCOUNT.PERMISSION) &&
                    (config.USER_ACCOUNT.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany))
                {
                    var transportAdminService = new DB.TransportAdminService(userService.db,
                            userService.user,
                            userService.signInManager,
                            userService.userManager);

                    if (config.USER_ACCOUNT.TRANSPORT_ADMIN_CODE == string.Empty)
                    {
                        #region 運送会社コードが存在するものか判定する

                        var transportAdmin =
                            transportAdminService
                            .read(config.USER_ACCOUNT.TRANSPORT_ADMIN_CODE)
                            .FirstOrDefault();
                        if (null == transportAdmin)
                        {
                            throw new ArgumentException("指定された運送会社が存在しません。");
                        }

                        #endregion
                    }

                    if (null != config.TRANSPORT_ADMIN)
                    {
                        var transportAdmin =
                            transportAdminService
                            .read(config.TRANSPORT_ADMIN.CODE)
                            .FirstOrDefault();
                        if (null == transportAdmin)
                        {
                            throw new ArgumentException("指定された運送会社が存在しません。");
                        }
                        user.TRANSPORT_ADMIN = config.TRANSPORT_ADMIN;
                        user.TRANSPORT_ADMIN_CODE = config.USER_ACCOUNT.TRANSPORT_ADMIN_CODE;
                    }
                }

                user.CONFIRM_FLAG = config.USER_ACCOUNT.CONFIRM_FLAG;

                //更新ユーザー情報を追加
                await userService.setUpdateManagementInformation(user);
                return true;
            }

            /// <summary>
            /// 全体更新　遅延更新
            /// </summary>
            public async Task<bool> updateALL_DelayChange()
            {

                //更新対象となる情報を取得する
                var user = readUserWithCheck();

                //遅延更新用のデータを取得
                var wk_user = (new Func<WK_USER_ACCOUNT>(() =>
                {
                    //存在したら存在する値を更新
                    if (null == user.WK_USER_ACCOUNT)
                    {
                        var t = new WK_USER_ACCOUNT();
                        user.WK_USER_ACCOUNT = t;

                        //遅延更新系の重複名称確認
                        if (0 != userService.
                            db.WK_USER_ACCOUNTs
                            .Where(ele => ele.USER_ID == config.USER_ACCOUNT.USER_ID)
                            .Count())
                        {
                            throw new InvalidOperationException("指定されたユーザーIDは登録済みです。");
                        }

                        return t;
                    }
                    else
                    {
                        //遅延更新系の重複名称確認
                        if (0 != userService.
                            db.WK_USER_ACCOUNTs
                            .Where(ele => ele.USER_ID == config.USER_ACCOUNT.USER_ID)
                            .Where(ele => ele.ID != user.WK_USER_ACCOUNT.ID)
                            .Count())
                        {
                            throw new InvalidOperationException("指定されたユーザーIDは登録済みです。");
                        }

                        return user.WK_USER_ACCOUNT;
                    }
                }))();

                //更新対象データをコピーする
                {
                    #region 必要データをコピーする
                    wk_user.USER_ACCOUNT_ID = readUserWithCheck().ID;
                    wk_user.USER_ID = config.USER_ACCOUNT.USER_ID;
                    wk_user.NAME = config.USER_ACCOUNT.NAME;
                    wk_user.KANA = config.USER_ACCOUNT.KANA;
                    wk_user.PERMISSION = config.USER_ACCOUNT.PERMISSION;

                    #endregion

                    await userService.db.SaveChangesAsync();
                }

                //パスワード指定が有ったらパスワード更新する
                if (true == config.isPasswordUpdateRequired)
                {
                    #region パスワード更新に関してデータを投入する
                    //ハッシュ文字列化されたパスワード
                    //入力されたパスワード文字列をhash形式に変換
                    var hashedPassword =
                        HelperFunctions.toHashString(config.password,
                        this.userService.db.USER_ACCOUNTs
                        .Where(e => e.ID == config.USER_ACCOUNT.ID)
                        .First().PASSWORD_SALT);

                    //入力されたパスワードがセキュリティポリシーに合致するか確認する
                    {
                        var r = userService.passwordPolicyCheck(new PasswordUpdateConfig
                        {
                            updatePassword = config.password,
                            USER_ACCOUNT = config.USER_ACCOUNT
                        }, hashedPassword);
                        if (null != r)
                        {
                            //パスワードチェックに失敗
                            throw new ApplicationException(r);
                        }
                    }

                    //更新用のパスワード情報を書き込む
                    wk_user.PASSWORD = hashedPassword;
                    wk_user.IS_NEED_PASSWORD_UPDATE = true;
                    #endregion

                    await userService.db.SaveChangesAsync();
                }

                {
                    #region ユーザー権限に応じた付加処理を実施する

                    {
                        #region 権限が荷主項目の場合の追加処理
                        if (config.USER_ACCOUNT.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperBrowsing ||
                            config.USER_ACCOUNT.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperEditing)
                        {
                            //権限が荷主の場合、デフォルト部課コードを追加する
                            if (null == config.DEFAULT_USER_DEPARTMENT)
                            {
                                throw new ArgumentException("権限が荷主であるがデフォルト部課コードが設定されていません。");
                            }

                            //ユーザー部課情報をDBに追加できる状態に整理する
                            var user_department = (new Func<IEnumerable<WK_USER_DEPARTMENT>>(() =>
                            {
                                #region ユーザー部課情報をDBに追加できる状態に整理する

                                //関係する部課マスタを関連付けする
                                var departmentAdminService = new DB.DepartmentAdminService(
                                   userService.db,
                                   userService.user,
                                   userService.signInManager,
                                   userService.userManager);

                                //部課情報はデフォルトの部課情報と、
                                //ユーザーが所属する部課情報が存在する。
                                //DBへの保存時にこれらを統合、重複排除して保存できる状態とする。
                                var t_user_department = userService.makeBelongsDEFAULT_USER_DEPARTMENT(config);

                                //ユーザー部課テーブルのワークテーブルを作成する
                                return t_user_department
                                .Select((e) =>
                                {
                                    #region 関係する部課マスタを関連付けする
                                    var errMsg = $"DEPARTMENT_CODE={e.DEPARTMENT_CODE} かつ SHIPPER_CODE={e.SHIPPER_CODE}を満たす部課マスタ履歴が存在しません。";

                                    //関係する部課情報を取得
                                    var daq = departmentAdminService.read(
                                            new DB.DepartmentAdminService.ReadConfig
                                            {
                                                DEPARTMENT_CODE = e.DEPARTMENT_CODE,
                                                SHIPPER_CODE = e.SHIPPER_CODE
                                            });
                                    if (false == daq.succeed)
                                    {
                                        throw new ArgumentException(errMsg);
                                    }
                                    var da = daq.result.FirstOrDefault();
                                    e.DEPARTMENT_ADMIN = da ?? throw new ArgumentException(errMsg);

                                    return e;
                                    #endregion
                                })
                                .Select(e =>
                                {
                                    return new WK_USER_DEPARTMENT
                                    {
                                        USER_ACCOUNT_ID = wk_user.ID,
                                        SHIPPER_CODE = e.SHIPPER_CODE,
                                        DEPARTMENT_CODE = e.DEPARTMENT_CODE
                                    };
                                });

                                #endregion
                            }))();

                            //DBに登録する
                            {
                                if (null == wk_user.USER_DEPARTMENTs)
                                {
                                    wk_user.USER_DEPARTMENTs = new List<WK_USER_DEPARTMENT>();
                                }

                                //基本情報を設定
                                await userService.setBothManagementInformation(user_department);

                                //基本上書きとするので一度DB上の値は消去する
                                this.userService.db.WK_USER_DEPARTMENTs.RemoveRange(
                                        this.userService.db.WK_USER_DEPARTMENTs.Where(e => e.USER_ACCOUNT_ID == wk_user.ID)
                                        );
                                //消去を確定する
                                await userService.db.SaveChangesAsync();

                                //作成されたユーザー情報を消去する
                                wk_user.USER_DEPARTMENTs.AddRange(user_department);

                                //ユーザー部課のIDが確定する
                                await userService.db.SaveChangesAsync();
                            }

                            //デフォルト部課情報情報をユーザーに設定する
                            wk_user.DEFAULT_DEPARTMENT_CODE = config.DEFAULT_USER_DEPARTMENT.DEPARTMENT_CODE;
                            wk_user.CURRENT_SHIPPER_CODE = config.DEFAULT_USER_DEPARTMENT.SHIPPER_CODE;

                            await userService.db.SaveChangesAsync();
                        }
                        #endregion
                    }

                    {
                        #region 権限が運送会社の場合の追加処理
                        if (config.USER_ACCOUNT.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany)
                        {
                            var errMsg = "運送会社権限の場合、運送会社の指定が必須です。";
                            if (null == config.TRANSPORT_ADMIN)
                            {
                                throw new ApplicationException(errMsg);
                            }

                            var transportAdminService = new DB.TransportAdminService(
                                userService.db,
                                userService.user,
                                userService.signInManager,
                                userService.userManager);
                            {
                                var q = transportAdminService.read(config.TRANSPORT_ADMIN.CODE);
                                if (null == q.FirstOrDefault())
                                {
                                    throw new ApplicationException("DB上に存在しない運送会社コードが指定されています。");
                                }
                            }

                            //追加
                            wk_user.TRANSPORT_ADMIN_CODE = config.TRANSPORT_ADMIN.CODE;

                            await userService.db.SaveChangesAsync();
                        }
                        #endregion
                    }

                    #endregion
                }

                //更新ユーザー情報を追加
                await userService.setUpdateManagementInformation(user);
                return true;
            }

            /// <summary>
            /// 変更検知
            /// </summary>
            public void collisionDetection()
            {
                var user = readUserWithCheck();
                if (config.collisionDetectionTimestamp != user.timestampToString())
                {
                    throw new ApplicationException
                        ("他のユーザーによる変更を検知しました。ページを再ロードして最新の情報に更新してください。");
                }
            }
        }

        #endregion

        /// <summary>
        /// 更新処理用の設定一覧
        /// </summary>
        public class UpdateConfig : ChangeConfig
        {
            /// <summary>
            /// 更新範囲
            /// </summary>
            public enum UpdateRange
            {
                /// <summary>
                /// アカウント情報ページにおける更新範囲を更新
                /// </summary>
                AccountInfo,
                /// <summary>
                /// 全体の更新を行う
                /// </summary>
                ALL
            }

            /// <summary>
            /// 更新範囲
            /// </summary>
            public UpdateRange updateRange;

            /// <summary>
            /// パスワード更新が必要
            /// </summary>
            public bool isPasswordUpdateRequired;

            /// <summary>
            /// 衝突検知用のタイムスタンプ
            /// </summary>
            public string collisionDetectionTimestamp;

        }

        /// <summary>
        /// ユーザー情報を更新する
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<Result.ExecutionResult<UpdateOperationType>> update(UpdateConfig arg)
        {
            return await RazorPagesLearning.Service.DB.Helper.ServiceHelper
                .doOperationWithErrorManagementAsync<UpdateOperationType>(async (ret) =>
          {
              using (var transaction = db.Database.BeginTransaction())
              {
                  var helper = new UpdateHelper(this, arg);

                  UpdateOperationType typeOperation = UpdateOperationType.ImmediateChange;

                  //更新衝突検知
                  helper.collisionDetection();

                  //1. 即時反映系の反映を行う
                  #region 即時反映が必要な処理を適応する
                  {
                      switch (arg.updateRange)
                      {
                          case UpdateConfig.UpdateRange.AccountInfo:
                              {
                                  //ユーザーアカウント情報ページに関係する情報を更新する
                                  await helper.updateAccountInfoPage_ImmediateChange();
                                  break;
                              }
                          case UpdateConfig.UpdateRange.ALL:
                              {
                                  typeOperation = helper.updateOperationTypeDetection();
                                  //全体変更 即時更新
                                  await helper.updateALL_ImmediateChange();
                                  break;
                              }
                      }
                      await db.SaveChangesAsync();
                  }
                  #endregion

                  //2. 逐次更新系の要求が来ていたら更新する
                  if (UpdateOperationType.DelayChange == typeOperation)
                  {
                      //全体変更 遅延更新を準備
                      await helper.updateALL_DelayChange();
                      await db.SaveChangesAsync();
                  }

                  //確定
                  transaction.Commit();

                  //結果を保存
                  ret.result = typeOperation;
                  ret.succeed = true;

                  return ret;
              }
          });
        }


        #region  ユーザーアカウントに関して適応待機中の変更を適応する

        /// <summary>
        /// 変更待機中の情報適応補助関数
        /// </summary>
        internal class ReflectPendingChangesHelper
        {
            public ReflectPendingChangesHelper(UserService ref_userService,
                string targetUSER_ID)
            {
                this.userService = ref_userService;
                this.targetUSER_ID = targetUSER_ID;
            }

            #region メンバー関数

            /// <summary>
            /// ユーザーサービス情報
            /// </summary>
            private readonly UserService userService;

            /// <summary>
            /// 捜査対象となるユーザーID
            /// </summary>
            private readonly string targetUSER_ID;

            #endregion


            /// <summary>
            /// Identityの権限を変更する
            /// </summary>
            /// <returns></returns>
            public async Task<bool> changeIdentityRole(USER_ACCOUNT target)
            {
                //現在のユーザー情報を取得する
                var ui = await userService.userManager.FindByNameAsync(target.ID.ToString());

                //既存のユーザーを消去する
                {
                    //現在の権限一覧
                    var rs = await userService.userManager.GetRolesAsync(ui);

                    var r = await userService.userManager.RemoveFromRolesAsync(ui, rs);
                    if (false == r.Succeeded)
                    {
                        throw new ApplicationException(
                            String.Join("<br>",
                            r.Errors.Select(e => $"Identity:権限消去に失敗 {e.Code} : {e.Description}")));
                    }
                }

                //ユーザーを追加する
                {
                    var r = await userService.userManager.AddToRoleAsync(ui, target.PERMISSION.ToString());
                    if (false == r.Succeeded)
                    {
                        throw new ApplicationException(
                            String.Join("<br>",
                            r.Errors.Select(e => $"Identity:権限追加に失敗 {e.Code} : {e.Description}")));
                    }
                }

                return true;
            }


            /// <summary>
            /// 操作が必要か判定する
            /// </summary>
            /// <returns></returns>
            public Tuple<bool, USER_ACCOUNT> isOperationRequired()
            {
                //ユーザー名が変更されていない場合、現在のユーザー名で取得する
                var userNow = this.userService.read(targetUSER_ID);
                if (null != userNow)
                {
                    if (null != userNow.WK_USER_ACCOUNT)
                    {
                        return Tuple.Create(true, userNow);
                    }
                    else
                    {
                        return Tuple.Create<bool, USER_ACCOUNT>(false, null);
                    }
                }
                else
                {
                    //更新後のユーザーIDを
                    var userNew = this.userService.db
                        .WK_USER_ACCOUNTs.Where(e => e.USER_ID == targetUSER_ID).FirstOrDefault();
                    if (null != userNew)
                    {
                        var tu = this.userService.db
                            .USER_ACCOUNTs.Where(e => e.ID == userNew.USER_ACCOUNT_ID).FirstOrDefault();
                        if (null == tu)
                        {
                            throw new ApplicationException($"{userNew.USER_ACCOUNT_ID}と合致するユーザーアカウントが見つかりません。");
                        }
                        return Tuple.Create(true, tu);
                    }
                    else
                    {
                        throw new ArgumentException("入力されたユーザー名は登録されていません。");
                    }
                }
            }

            /// <summary>
            /// 基本的な項目を反映させる
            /// </summary>
            public async Task<bool> reflectBasicItems(USER_ACCOUNT target)
            {
                var wk_user = target.WK_USER_ACCOUNT;

                //ユーザーIDはシステム内部でユニークであるため、
                //IDチェックを行う
                {
                    var overlap = this.userService.db
                        .USER_ACCOUNTs.Where(e => e.USER_ID == target.USER_ID)
                        .Where(e => e.ID != target.WK_USER_ACCOUNT.USER_ACCOUNT_ID)
                        ;

                    if (0 != overlap.Count())
                    {
                        throw new ApplicationException
                            ($"ユーザーID : {wk_user.USER_ID}はすでに使用済みです。");
                    }
                }

                #region 必要データをコピーする
                target.USER_ID = wk_user.USER_ID;
                target.NAME = wk_user.NAME;
                target.KANA = wk_user.KANA;
                target.PERMISSION = wk_user.PERMISSION;
                #endregion

                await userService.db.SaveChangesAsync();

                return true;
            }

            /// <summary>
            /// ユーザー権限毎に固有の付加情報を反映する
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            public async Task<bool> reflectPrivilegeSpecializationInformation(USER_ACCOUNT target)
            {
                #region ローカル関数

                //部課情報を適応
                async Task<bool> LF_ReflectShipper()
                {
                    //関係する部課マスタを関連付けする
                    var departmentAdminService = new DB.DepartmentAdminService(
                       userService.db,
                       userService.user,
                       userService.signInManager,
                       userService.userManager);

                    {
                        #region 値を反映

                        var twk_user = target.WK_USER_ACCOUNT;
                        if (null != twk_user.USER_DEPARTMENTs)
                        {
                            //現在の部課情報を消去する
                            {
                                userService.db.USER_DEPARTMENTs
                                    .RemoveRange(target.USER_DEPARTMENTs);
                                target.DEFAULT_DEPARTMENT_CODE = "";
                                target.CURRENT_SHIPPER_CODE = "";

                                await userService.db.SaveChangesAsync();
                            }

                            //部課情報の指定が有ったら適応する
                            foreach (var ele in twk_user.USER_DEPARTMENTs)
                            {
                                var r = departmentAdminService.read(new DB.DepartmentAdminService.ReadConfig
                                {
                                    DEPARTMENT_CODE = ele.DEPARTMENT_CODE,
                                    SHIPPER_CODE = ele.SHIPPER_CODE
                                });
                                if (false == r.succeed)
                                {
                                    throw new ApplicationException(string.Join("<br>", r.errorMessages));
                                }
                                var t = r.result.FirstOrDefault();
                                if (null == t)
                                {
                                    throw new ApplicationException
                                        ($"DEPARTMENT_CODE:{ele.DEPARTMENT_CODE},SHIPPER_CODE:{ele.SHIPPER_CODE}は存在しません。");
                                }
                                target.USER_DEPARTMENTs.Add(
                                    new USER_DEPARTMENT
                                    {
                                        DEPARTMENT_CODE = ele.DEPARTMENT_CODE,
                                        SHIPPER_CODE = ele.SHIPPER_CODE
                                    });
                            }

                            //デフォルトコードの張替えを行う
                            target.DEFAULT_DEPARTMENT_CODE = twk_user.DEFAULT_DEPARTMENT_CODE;
                            target.CURRENT_SHIPPER_CODE = twk_user.CURRENT_SHIPPER_CODE;
                        }

                        await userService.db.SaveChangesAsync();

                        #endregion
                    }

                    {
                        #region 値を確認

                        //デフォルト部課が存在するか確認する
                        {
                            #region  デフォルト部課が存在するか確認する
                            if (1 != target.USER_DEPARTMENTs.Where(e =>
                          e.DEPARTMENT_CODE == target.DEFAULT_DEPARTMENT_CODE &&
                          e.SHIPPER_CODE == target.CURRENT_SHIPPER_CODE
                            ).Count())
                            {
                                throw new ApplicationException
                                    ($"デフォルト指定荷主として指定されたDEPARTMENT_CODE:{target.DEFAULT_DEPARTMENT_CODE},SHIPPER_CODE:{target.CURRENT_SHIPPER_CODE}はDB上に存在しません。");
                            }
                            #endregion
                        }

                        {
                            #region 指定された部課情報がDB上に存在するか確認する

                            foreach (var ele in target.USER_DEPARTMENTs)
                            {
                                var r = departmentAdminService.read(new DB.DepartmentAdminService.ReadConfig
                                {
                                    DEPARTMENT_CODE = ele.DEPARTMENT_CODE,
                                    SHIPPER_CODE = ele.SHIPPER_CODE
                                });
                                if (false == r.succeed)
                                {
                                    throw new ApplicationException(string.Join("<br>", r.errorMessages));
                                }
                                var t = r.result.FirstOrDefault();
                                if (null == t)
                                {
                                    throw new ApplicationException
                                        ($"DEPARTMENT_CODE:{ele.DEPARTMENT_CODE},SHIPPER_CODE:{ele.SHIPPER_CODE}は存在しません。");
                                }
                            }

                            #endregion
                        }

                        #endregion
                    }

                    await userService.db.SaveChangesAsync();

                    return true;
                }

                //運送会社情報を適応
                async Task<bool> LF_ReflectShippingCompany()
                {
                    var transportAdminService = new DB.TransportAdminService(
                        userService.db,
                        userService.user,
                        userService.signInManager,
                        userService.userManager);

                    {
                        #region 値を反映
                        var twk_user = target.WK_USER_ACCOUNT;
                        if (null != twk_user.TRANSPORT_ADMIN_CODE)
                        {
                            if (string.Empty != twk_user.TRANSPORT_ADMIN_CODE)
                            {
                                //運送会社情報の指定が来ていたら更新する
                                var t = transportAdminService.read(twk_user.TRANSPORT_ADMIN_CODE)
                                    .FirstOrDefault();
                                if (null == t)
                                {
                                    throw new ApplicationException("DB上に存在しない運送会社コードが指定されています。");
                                }

                                //情報を反映させる
                                target.TRANSPORT_ADMIN = t;
                                target.TRANSPORT_ADMIN_CODE = t.CODE;
                                await userService.db.SaveChangesAsync();
                            }
                        }
                        #endregion
                    }

                    {
                        #region 必須項目なので存在しているかチェックする

                        if (null == target.TRANSPORT_ADMIN_CODE)
                        {
                            throw new ApplicationException("運送会社コードが指定されていません。");
                        }

                        if (string.Empty == target.TRANSPORT_ADMIN_CODE)
                        {
                            throw new ApplicationException("運送会社コードが指定されていません。");
                        }

                        //運送会社情報の指定が来ていたら更新する
                        var t = transportAdminService.read(target.TRANSPORT_ADMIN_CODE)
                            .FirstOrDefault();
                        if (null == t)
                        {
                            throw new ApplicationException("DB上に存在しない運送会社コードが指定されています。");
                        }

                        #endregion
                    }

                    await userService.db.SaveChangesAsync();

                    return true;
                }

                #endregion

                //適応保留中の情報を取得
                var wk_user = target.WK_USER_ACCOUNT;

                if (wk_user.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperBrowsing ||
                    wk_user.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperEditing)
                {
                    //権限が荷主の場合
                    await LF_ReflectShipper();
                }
                else if (wk_user.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany)
                {
                    //権限が運送会社の場合
                    await LF_ReflectShippingCompany();
                }

                return true;
            }

            /// <summary>
            /// パスワードを更新する
            /// </summary>
            /// <returns></returns>
            public async Task<bool> passwordUpdate(USER_ACCOUNT target)
            {
                var wk_user = target.WK_USER_ACCOUNT;
                if (true == wk_user.IS_NEED_PASSWORD_UPDATE)
                {
                    //パスワード更新サービスを呼び出す
                    await userService.passwordUpdate(
                        new PasswordUpdateConfig
                        {
                            updatePassword = wk_user.PASSWORD,
                            USER_ACCOUNT = target,
                            nowPassword = target.PASSWORD
                        });

                    await userService.db.SaveChangesAsync();
                }

                return true;
            }

            /// <summary>
            /// ワークテーブルを消去する
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            public async Task<bool> removeWorkTable(USER_ACCOUNT target)
            {
                var wk_user = target.WK_USER_ACCOUNT;
                if (null != wk_user.USER_DEPARTMENTs)
                {
                    userService.db.WK_USER_DEPARTMENTs.RemoveRange(wk_user.USER_DEPARTMENTs);
                }

                userService.db.WK_USER_ACCOUNTs.Remove(wk_user);

                await userService.db.SaveChangesAsync();

                return true;
            }

        }

        #endregion

        /// <summary>
        /// ユーザーアカウントに関して適応待機中の変更を適応する
        /// </summary>
        /// <param name="USER_ID"></param>
        /// <returns></returns>
        public async Task<Result.ExecutionResult<bool>> reflectPendingChanges
            (string targetUSER_ID, IDbContextTransaction transaction)
        {
            targetUSER_ID = targetUSER_ID.Trim();

            return await RazorPagesLearning.Service.DB.Helper.ServiceHelper
                .doOperationWithErrorManagementAsync<bool>(async (ret) =>
                {
                    //操作補助クラスを生成する
                    var helper = new ReflectPendingChangesHelper(this, targetUSER_ID);

                    //DBからユーザーを取り込む
                    var oi = helper.isOperationRequired();
                    if (true == oi.Item1)
                    {
                        //基本的な適応項目を適応する
                        await helper.reflectBasicItems(oi.Item2);

                        //ユーザー権限毎の付加情報を追加する
                        await helper.reflectPrivilegeSpecializationInformation(oi.Item2);

                        //パスワード変更の場合パスワード更新を適応する
                        await helper.passwordUpdate(oi.Item2);

                        //Identity側の権限を変更する
                        await helper.changeIdentityRole(oi.Item2);

                        //元情報を消去する
                        await helper.removeWorkTable(oi.Item2);
                    }

                    //結果を適応
                    ret.succeed = true;
                    ret.result = true;

                    return ret;
                });
        }

        /// <summary>
        /// パスワード更新設定
        /// </summary>
        public class PasswordUpdateConfig
        {
            /// <summary>
            /// 管理対象テーブル
            /// </summary>
            public RazorPagesLearning.Data.Models.USER_ACCOUNT USER_ACCOUNT;

            /// <summary>
            /// 更新パスワード
            /// </summary>
            public string updatePassword;

            /// <summary>
            /// 該当ユーザーの現在のパスワード
            /// </summary>
            public string nowPassword;

        }

        /// <summary>
        /// パスワードポリシーをチェックする
        /// </summary>
        /// <param name="ref_hashedUpdatePassword"></param>
        /// <returns></returns>
        private string passwordPolicyCheck(PasswordUpdateConfig config, string ref_hashedUpdatePassword)
        {
            #region パスワードポリシーを確認する

            //新規のパスワード桁数
            {
                #region パスワード桁数の確認
                var d = this.policyService.read(new DB.PolicyService.ReadConfig
                {
                    NAME = POLICY.PASSWORD_POLICY.Digit
                });
                if (config.updatePassword.Length < d.result.VALUE)
                {
                    return $"パスワードは{d.result.VALUE}文字以上指定する必要があります。";
                }
                #endregion
            }

            //パスワードの再利用禁止履歴数の確認
            {
                #region パスワード再利用禁止履歴の確認
                var d = this.policyService.read(new DB.PolicyService.ReadConfig
                {
                    NAME = POLICY.PASSWORD_POLICY.Reuse
                });

                //DBからパスワード履歴を取得
                //更新日付順で指定された件数だけパスワード履歴を取得
                //[memo]
                //configで渡されたUSER_ACCOUNTはDBから来ていないものもあるので、
                //一度値を取り直す。
                var hq = this.db.USER_ACCOUNTs
                    .Where(e => e.USER_ID == config.USER_ACCOUNT.USER_ID)
                    .Where(e => false == e.DELETE_FLAG)
                    .OrderByDescending(e => e.UPDATED_AT)
                    .Take(d.result.VALUE);

                //合致する文字列があるか確認する
                if (0 != hq.Where(e => e.PASSWORD == ref_hashedUpdatePassword).Count())
                {
                    return $"指定されたパスワードは過去に使用されています。";
                }

                #endregion
            }

            return null;
            #endregion
        }

        /// <summary>
        /// パスワードを更新する
        /// </summary>
        internal async Task<Result.DefaultExecutionResult> passwordUpdate(PasswordUpdateConfig config)
        {
            #region ローカル関数

            //パスワード履歴を更新する
            async Task<bool> updatePASSWORD_HISTORY(string ref_hashedPassword)
            {
                #region パスワード履歴テーブルを更新
                var td = new PASSWORD_HISTORY
                {
                    PASSWORD = ref_hashedPassword
                };
                await this.setBothManagementInformation(td);
                config.USER_ACCOUNT.PASSWORD_HISTORYs.Add(td);

                //指定数以上の履歴が有ったら消去する
                {
                    var d = this.policyService.read(new DB.PolicyService.ReadConfig
                    {
                        NAME = POLICY.PASSWORD_POLICY.Reuse
                    });
                    //データを投入した直後なので、規定数よりも1個多くデータが入っているはず
                    var nc = config.USER_ACCOUNT.PASSWORD_HISTORYs.Count;
                    if (nc > d.result.VALUE + 1)
                    {
                        //削除対象は古いほうから順番に
                        var hq = config.USER_ACCOUNT.PASSWORD_HISTORYs
                            .OrderBy(e => e.UPDATED_AT)
                            .Take(nc - d.result.VALUE);

                        //削除する
                        foreach (var ele in hq)
                        {
                            config.USER_ACCOUNT.PASSWORD_HISTORYs.Remove(ele);
                        }
                    }
                }

                return true;
                #endregion
            }

            //該当ユーザーの現在のパスワードをチェック
            Tuple<bool, string> checkCurrentPassword(string ref_hashedNowPassword)
            {
                if (ref_hashedNowPassword != config.USER_ACCOUNT.PASSWORD)
                {
                    return Tuple.Create<bool, string>(false,
                            "指定された現在のパスワードが一致しません。");
                }

                return Tuple.Create<bool, string>(true,
                            null);
            }

            #endregion

#if false

            //ハッシュ文字列化されたパスワード
            //入力されたパスワード文字列をhash形式に変換
            var hashedNowPassword =
                HelperFunctions.toHashString(config.nowPassword, config.USER_ACCOUNT.PASSWORD_SALT);
            var hashedUpdatePassword =
                HelperFunctions.toHashString(config.updatePassword, config.USER_ACCOUNT.PASSWORD_SALT);

#endif

            var hashedNowPassword = config.nowPassword;
            var hashedUpdatePassword = config.updatePassword;


            //入力されたパスワードがセキュリティポリシーに合致するか確認する
            #region 条件チェック
            {
                {
                    var r = passwordPolicyCheck(config, hashedUpdatePassword);
                    if (null != r)
                    {
                        return new Result.DefaultExecutionResult
                        {
                            succeed = false,
                            errorMessages = new List<string> { r }
                        };
                    }
                }

                {
                    //現在のパスワードが一致するか確認
                    var r = checkCurrentPassword(hashedNowPassword);
                    if (false == r.Item1)
                    {
                        //現在のパスワードが一致していない
                        return new Result.DefaultExecutionResult
                        {
                            succeed = false,
                            errorMessages = new List<string> { r.Item2 }
                        };

                    }
                }
            }
            #endregion

            //フレームワーク側のパスワードを更新する
            {
                #region
                var identityuser = await this.userManager
                    .FindByNameAsync(config.USER_ACCOUNT.ID.ToString());

                var r = await this.userManager.ChangePasswordAsync
                    (identityuser, config.USER_ACCOUNT.PASSWORD, hashedUpdatePassword);
                if (false == r.Succeeded)
                {
                    throw new ApplicationException(
                        String.Join("<br>", r.Errors.Select(e => e.Description).ToList())
                        );
                }
                #endregion
            }

            //パスワード履歴テーブルを更新する
            await updatePASSWORD_HISTORY(hashedUpdatePassword);

            //ユーザーアカウントテーブル上のパスワード履歴を更新する
            config.USER_ACCOUNT.PASSWORD = hashedUpdatePassword;
            config.USER_ACCOUNT.PASSWORD_UPDATED_AT = DateTimeOffset.Now;

            //パスワード変更したので、更新要求はつぶしておく
            config.USER_ACCOUNT.PASSWORD_CHANGE_REQUEST = false;

            return new Result.DefaultExecutionResult
            {
                succeed = true
            };
        }

        /// <summary>
        /// 関連テーブルを全て連結した状態で基礎的なクエリを生成する
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private IQueryable<USER_ACCOUNT> makeBasicQueryUSER_ACCOUNT()
        {
            return db.USER_ACCOUNTs
                .AsQueryable()
                .Include(e => e.TRANSPORT_ADMIN)
                .Include(e => e.USER_DEPARTMENTs)
                .Include(e => e.PASSWORD_HISTORYs)
                .Include(e => e.WK_TABLE_SELECTION_SETTINGs)
                .Include(e => e.WK_TABLE_PAGINATION_SETTINGs)
                .Include(e => e.WK_USER_ACCOUNT)
                .Include(e => e.WK_USER_ACCOUNT.USER_DEPARTMENTs);
        }

        /// <summary>
        /// 検索条件
        /// </summary>
        public class ReadConfig
        {
            /// <summary>
            /// ユーザーID
            /// </summary>
            public string USER_ID { get; set; }

            /// <summary>
            /// ユーザーIDをand条件で結合するか
            /// </summary>
            public bool isAnd_USER_ID { get; set; }

            /// <summary>
            /// ユーザー名称(カナ)
            /// </summary>
            public string USER_NAME_KANA { get; set; }

            /// <summary>
            /// ユーザー名称(カナ)をand条件で結合するか
            /// </summary>
            public bool isAnd_USER_NAME_KANA { get; set; }

            /// <summary>
            /// 会社名
            /// </summary>
            public string COMPANY { get; set; }

            /// <summary>
            /// 会社名をand条件で結合するか
            /// </summary>
            public bool isAnd_COMPANY { get; set; }

            /// <summary>
            /// 荷主コード
            /// </summary>
            public string SHIPPER_CODE { get; set; }

            /// <summary>
            /// アカウント権限
            /// </summary>
            public Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION PERMISSION { get; set; }

        }

        /// <summary>
        /// ユーザー情報テーブルは即時反映する情報と、
        /// 遅延反映する情報を合わせて持っている。
        /// このクラスでは、透過的に必要な情報にアクセスできるようにする。
        /// </summary>
        public class HoldsPendingInformation_USER_ACCOUNT : RazorPagesLearning.Data.Models.USER_ACCOUNT
        {

            public UserService userService;

            //遅延更新される可能性があるものに関しては、
            //遅延更新系のデータを持っていたら、そちら側を採用する

            public override string USER_ID
            {
                get
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        return this.WK_USER_ACCOUNT.USER_ID;
                    }
                    else
                    {
                        return base.USER_ID;
                    }
                }
                set
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        this.WK_USER_ACCOUNT.USER_ID = value;
                    }
                    else
                    {
                        base.USER_ID = value;
                    }
                }
            }

            public override string NAME
            {
                get
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        return this.WK_USER_ACCOUNT.NAME;
                    }
                    else
                    {
                        return base.NAME;
                    }
                }
                set
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        this.WK_USER_ACCOUNT.NAME = value;
                    }
                    else
                    {
                        base.NAME = value;
                    }
                }
            }

            public override string KANA
            {
                get
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        return this.WK_USER_ACCOUNT.KANA;
                    }
                    else
                    {
                        return base.KANA;
                    }
                }
                set
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        this.WK_USER_ACCOUNT.KANA = value;
                    }
                    else
                    {
                        base.KANA = value;
                    }
                }
            }

            public override ACCOUNT_PERMISSION PERMISSION
            {
                get
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        return this.WK_USER_ACCOUNT.PERMISSION;
                    }
                    else
                    {
                        return base.PERMISSION;
                    }
                }
                set
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        this.WK_USER_ACCOUNT.PERMISSION = value;
                    }
                    else
                    {
                        base.PERMISSION = value;
                    }
                }
            }


            public override string DEFAULT_DEPARTMENT_CODE
            {
                get
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        return this.WK_USER_ACCOUNT.DEFAULT_DEPARTMENT_CODE;
                    }
                    else
                    {
                        return base.DEFAULT_DEPARTMENT_CODE;
                    }
                }
                set
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        this.WK_USER_ACCOUNT.DEFAULT_DEPARTMENT_CODE = value;
                    }
                    else
                    {
                        base.DEFAULT_DEPARTMENT_CODE = value;
                    }
                }
            }

            public override string CURRENT_SHIPPER_CODE
            {
                get
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        return this.WK_USER_ACCOUNT.CURRENT_SHIPPER_CODE;
                    }
                    else
                    {
                        return base.CURRENT_SHIPPER_CODE;
                    }
                }
                set
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        this.WK_USER_ACCOUNT.CURRENT_SHIPPER_CODE = value;
                    }
                    else
                    {
                        base.CURRENT_SHIPPER_CODE = value;
                    }
                }
            }

            /// <summary>
            /// ユーザーが所属する部課名
            /// </summary>
            public override List<USER_DEPARTMENT> USER_DEPARTMENTs
            {
                get
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        if (null == this.WK_USER_ACCOUNT.USER_DEPARTMENTs)
                        {
                            return null;
                        }
                        else
                        {
                            return this.WK_USER_ACCOUNT
                                .USER_DEPARTMENTs
                                .Select(e =>
                                //まだ存在しないユーザー情報なので、仮想的な情報を返す
                                new USER_DEPARTMENT
                                {
                                    DEPARTMENT_CODE = e.DEPARTMENT_CODE,
                                    SHIPPER_CODE = e.SHIPPER_CODE,
                                    USER_ACCOUNT_ID = this.WK_USER_ACCOUNT.USER_ACCOUNT_ID
                                })
                                .ToList();
                        }
                    }
                    else
                    {
                        return base.USER_DEPARTMENTs;
                    }
                }
                set
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        this.WK_USER_ACCOUNT.USER_DEPARTMENTs =
                            value.Select(e =>
                            new WK_USER_DEPARTMENT
                            {
                                SHIPPER_CODE = e.SHIPPER_CODE,
                                DEPARTMENT_CODE = e.DEPARTMENT_CODE,
                                USER_ACCOUNT_ID = WK_USER_ACCOUNT.USER_ACCOUNT_ID
                            })
                            .ToList();
                    }
                    else
                    {
                        base.USER_DEPARTMENTs = value;
                    }
                }
            }


            public override string TRANSPORT_ADMIN_CODE
            {
                get
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        return this.WK_USER_ACCOUNT.TRANSPORT_ADMIN_CODE;
                    }
                    else
                    {
                        return base.TRANSPORT_ADMIN_CODE;
                    }
                }
                set
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        this.WK_USER_ACCOUNT.TRANSPORT_ADMIN_CODE = value;
                    }
                    else
                    {
                        base.TRANSPORT_ADMIN_CODE = value;
                    }
                }
            }

            public override TRANSPORT_ADMIN TRANSPORT_ADMIN
            {
                get
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        if (null != this.WK_USER_ACCOUNT.TRANSPORT_ADMIN_CODE)
                        {
                            if (String.Empty != this.WK_USER_ACCOUNT.TRANSPORT_ADMIN_CODE)
                            {
                                //補助テーブルに入っている値に変換する
                                var tadmin = new TransportAdminService(
                                    userService.db,
                                    userService.user,
                                    userService.signInManager,
                                    userService.userManager
                                    );
                                return tadmin.read()
                                    .Where(e => e.CODE == this.WK_USER_ACCOUNT.TRANSPORT_ADMIN_CODE)
                                    .First();
                            }
                        }
                        return null;
                    }
                    else
                    {
                        return base.TRANSPORT_ADMIN;
                    }
                }
                set
                {
                    if (null != this.WK_USER_ACCOUNT)
                    {
                        this.WK_USER_ACCOUNT.DEFAULT_DEPARTMENT_CODE = value.CODE;
                    }
                    else
                    {
                        base.TRANSPORT_ADMIN = value;
                    }
                }
            }

        }

        /// <summary>
        /// ユーザー情報を即時反映系の情報を含めた形式にマッピングする
        /// </summary>
        /// <param name="ref_org"></param>
        /// <returns></returns>
        private HoldsPendingInformation_USER_ACCOUNT convert(USER_ACCOUNT ref_org)
        {
            //DB上の状況が変化しないように深いコピーを行う
            var org = ref_org.DeepCopy();

            //今回のオブジェクトでは、
            //内部でWK_USER_ACCOUNTのオブジェクトへのアクセスをラッピングしている。
            //コピー時にそちらへのアクセスがおかしくならないよに、
            //一度参照を外してからコピーする
            var wk = org.WK_USER_ACCOUNT;
            org.WK_USER_ACCOUNT = null;

            //マッピングをかける
            var m = Mapper.Map<HoldsPendingInformation_USER_ACCOUNT>(org);
            m.userService = this;

            //参照を戻す
            m.WK_USER_ACCOUNT = wk;

            return m;
        }

        /// <summary>
        /// 変更保留中テーブルからユーザー一覧を取得する
        /// </summary>
        /// <param name="db"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        internal Result.ExecutionResult<IQueryable<RazorPagesLearning.Data.Models.WK_USER_ACCOUNT>> readFrom_WK_USER_ACCOUNT(ReadConfig config)
        {
            var q = (IQueryable<RazorPagesLearning.Data.Models.WK_USER_ACCOUNT>)db.WK_USER_ACCOUNTs
                .AsQueryable()
                .Include(e => e.USER_DEPARTMENTs);
            return conditionalSearch(config, q);
        }

        /// <summary>
        /// 変更保留中テーブルからユーザー一覧を取得する
        /// </summary>
        /// <param name="db"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        internal Result.ExecutionResult<IQueryable<RazorPagesLearning.Data.Models.WK_USER_ACCOUNT>>
            conditionalSearch(ReadConfig config, IQueryable<RazorPagesLearning.Data.Models.WK_USER_ACCOUNT> q)
        {
            //権限は全要素共通
            q = q.Where(e => e.PERMISSION == config.PERMISSION);

            if (null != config)
            {
                //権限は必ずあるので指定する
                q = q.Where(e => e.PERMISSION == config.PERMISSION);

                if (null != config.USER_ID)
                {
                    //空文字の時は無条件検索とする
                    if (string.Empty != config.USER_ID)
                    {
                        #region ユーザーIDによる絞り込み
                        //スペース区切りで連結して設定されているので、
                        //分割して使用する
                        char[] delimiterChars = { ' ', '　' };
                        var keys = config.USER_ID.Split(delimiterChars);
                        if (0 != keys.Length)
                        {
                            if (true == config.isAnd_USER_ID)
                            {
                                foreach (var k in keys)
                                {
                                    q = q.Where(e => e.USER_ID.Contains(k));
                                }
                            }
                            else
                            {
                                q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<WK_USER_ACCOUNT>(keys, "USER_ID"));
                            }
                        }
                        #endregion
                    }
                }

                if (null != config.USER_NAME_KANA)
                {
                    #region ユーザー名(カナ)により絞り込み
                    if (string.Empty != config.USER_NAME_KANA)
                    {
                        //スペース区切りで連結して設定されているので、
                        //分割して使用する
                        char[] delimiterChars = { ' ', '　' };
                        var keys = config.USER_NAME_KANA.Split(delimiterChars);
                        if (0 != keys.Length)
                        {
                            if (true == config.isAnd_USER_NAME_KANA)
                            {
                                foreach (var k in keys)
                                {
                                    q = q.Where(e => e.KANA.Contains(k));
                                }
                            }
                            else
                            {
                                q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<WK_USER_ACCOUNT>(keys, "KANA"));
                            }
                        }
                    }
                    #endregion
                }

                if (null != config.SHIPPER_CODE)
                {
                    #region 荷主コードによる絞り込み

                    q = q.Where(e =>
                    null != e.USER_DEPARTMENTs
                    .Where(ine => ine.SHIPPER_CODE == config.SHIPPER_CODE)
                    .FirstOrDefault()
                    );

                    #endregion
                }
            }

            //ユーザー部課に応じて重複する場合があるので、
            //重複排除をかける
            q = q.Distinct();

            ///成功を返す
            return new Result.ExecutionResult<IQueryable<Data.Models.WK_USER_ACCOUNT>>
            {
                result = q,
                succeed = true
            };
        }

        /// <summary>
        /// 更新保留中の情報を含めて読み取る
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public Result.ExecutionResult<IEnumerable<HoldsPendingInformation_USER_ACCOUNT>>
            readWithPendingInfo(ReadConfig config)
        {
            return RazorPagesLearning.Service.DB.Helper.ServiceHelper
                .DoOperationWithErrorManagement<IEnumerable<HoldsPendingInformation_USER_ACCOUNT>>((ret) =>
                {
                    //正規のデータを取り込み
                    var regular = (new Func<List<HoldsPendingInformation_USER_ACCOUNT>>(() =>
             {
                 //通常のデータ
                 var t_regular = read(config).result;

                 //正規データの中で変更待機状態にあるデータ
                 var tc = t_regular
                 .Where(e => null != e.WK_USER_ACCOUNT)
                 .Select(e => e.WK_USER_ACCOUNT);

                 //変換手順
                 //1. 通常データから変更待機状態にあるデータを取り除く
                 //2. 変更待機状態のデータのうち変更待機状態の値が条件に合致するデータは許可する

                 //1.通常データから変更待機状態にあるデータを取り除く
                 var ids = tc.Select(e => e.USER_ACCOUNT_ID);
                 var f = t_regular
                 .Where(e => false == ids.Contains(e.ID))
                 .ToList()
                 .Select(e =>
                 {
                     //更新待機中のオブジェクトもあるため、
                     //透過オブジェクトに変換
                     return convert(e);
                 })
         .ToList();

                 //変更待機状態を検索して対象となるデータか確認する。
                 //(変更待機状態が正となる。)
                 var cahnged = conditionalSearch(config, tc)
          .result;

                 //結合する
                 f.AddRange(
              cahnged
              .ToList()
              .Select(e =>
              {
                  //親となるユーザーアカウントを探す
                  var p = read(e.USER_ACCOUNT_ID);
                  return convert(p);
              })
              .ToList()
              );

                 return f;
             }))();

                    //変更保留中のデータを読み取る
                    var pending = readFrom_WK_USER_ACCOUNT(config)
            .result
            .ToList()
            .Select(e =>
            {
                //親となるユーザーアカウントを探す
                var p = read(e.USER_ACCOUNT_ID);
                return convert(p);
            });

                    //全体をまとめて一つのデータとする
                    var r = new List<HoldsPendingInformation_USER_ACCOUNT>();
                    r.AddRange(regular);
                    r.AddRange(pending);

                    //ユーザーの指定方法によっては、重複して入ることもあるので、
                    //重複を除外しておく
                    var deduplication = r
                .GroupBy(e => new { e.ID })
                .Select(group => group.First());

                    //ソートをかける
                    ret.result = deduplication.OrderBy(e => e.USER_ID);
                    ret.succeed = true;
                });
        }

        internal Result.ExecutionResult<IQueryable<RazorPagesLearning.Data.Models.USER_ACCOUNT>> read(ReadConfig config)
        {
            var q = makeBasicQueryUSER_ACCOUNT();
            return conditionalSearch(config, q);
        }

        /// <summary>
        /// ユーザー一覧を取得する
        /// </summary>
        /// <param name="db"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        internal Result.ExecutionResult<IQueryable<RazorPagesLearning.Data.Models.USER_ACCOUNT>> conditionalSearch
            (ReadConfig config, IQueryable<USER_ACCOUNT> q)
        {
            //権限は全要素共通
            q = q.Where(e => e.PERMISSION == config.PERMISSION);

            if (null != config)
            {
                //権限は必ずあるので指定する
                q = q.Where(e => e.PERMISSION == config.PERMISSION);

                if (null != config.USER_ID)
                {
                    //空文字の時は無条件検索とする
                    if (string.Empty != config.USER_ID)
                    {
                        #region ユーザーIDによる絞り込み
                        //スペース区切りで連結して設定されているので、
                        //分割して使用する
                        char[] delimiterChars = { ' ', '　' };
                        var keys = config.USER_ID.Split(delimiterChars);
                        if (0 != keys.Length)
                        {
                            if (true == config.isAnd_USER_ID)
                            {
                                foreach (var k in keys)
                                {
                                    q = q.Where(e => e.USER_ID.Contains(k));
                                }
                            }
                            else
                            {
                                q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<USER_ACCOUNT>(keys, "USER_ID"));
                            }
                        }
                        #endregion
                    }
                }

                if (null != config.USER_NAME_KANA)
                {
                    #region ユーザー名(カナ)により絞り込み
                    if (string.Empty != config.USER_NAME_KANA)
                    {
                        //スペース区切りで連結して設定されているので、
                        //分割して使用する
                        char[] delimiterChars = { ' ', '　' };
                        var keys = config.USER_NAME_KANA.Split(delimiterChars);
                        if (0 != keys.Length)
                        {
                            if (true == config.isAnd_USER_NAME_KANA)
                            {
                                foreach (var k in keys)
                                {
                                    q = q.Where(e => e.KANA.Contains(k));
                                }
                            }
                            else
                            {
                                q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<USER_ACCOUNT>(keys, "KANA"));
                            }
                        }
                    }
                    #endregion
                }

                if (null != config.COMPANY)
                {
                    #region 会社名により絞り込み
                    if (string.Empty != config.COMPANY)
                    {
                        //スペース区切りで連結して設定されているので、
                        //分割して使用する
                        char[] delimiterChars = { ' ', '　' };
                        var keys = config.COMPANY.Split(delimiterChars);
                        if (0 != keys.Length)
                        {
                            if (true == config.isAnd_COMPANY)
                            {
                                foreach (var k in keys)
                                {
                                    q = q.Where(e => e.COMPANY.Contains(k));
                                }
                            }
                            else
                            {
                                q = q.Where(QueryHelper.makeQueryOfKeywordJoinedByOr<USER_ACCOUNT>(keys, "COMPANY"));
                            }
                        }
                    }
                    #endregion
                }

                if (null != config.SHIPPER_CODE)
                {
                    #region 荷主コードによる絞り込み

                    q = q.Where(e =>
                    null != e.USER_DEPARTMENTs
                    .Where(ine => ine.SHIPPER_CODE == config.SHIPPER_CODE)
                    .FirstOrDefault()
                    );

                    #endregion
                }
            }

            //ユーザー部課に応じて重複する場合があるので、
            //重複排除をかける
            q = q.Distinct();

            ///成功を返す
            return new Result.ExecutionResult<IQueryable<Data.Models.USER_ACCOUNT>>
            {
                result = q,
                succeed = true
            };
        }

        /// <summary>
        /// ユーザー情報を検索する
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public USER_ACCOUNT read(string USER_ID)
        {
            return makeBasicQueryUSER_ACCOUNT()
                .Where(e => e.USER_ID == USER_ID).FirstOrDefault();
        }

        /// <summary>
        /// ユーザが所属する部課マスタ一覧を検索する
        /// RazorPagesLearning社員の場合は、選択中荷主の全部課マスタ一覧を検索する
        /// </summary>
        /// <param name="userAccountId"></param>
        /// <returns></returns>
        public IQueryable<DEPARTMENT_ADMIN> readDEPARTMENT_ADMINs(Int64 userAccountId)
        {
            // ユーザーアカウントを調べる
            var userAccount = read(userAccountId)
                ?? throw new ArgumentException($"ID:{userAccountId}のユーザ情報がありません。");

            // 部課マスタを調べる
            Result.ExecutionResult<IQueryable<DEPARTMENT_ADMIN>> departmentAdmins = null;
            var departmentAdminService = new DepartmentAdminService(db, user, signInManager, userManager);

            switch (userAccount.PERMISSION)
            {
                // RazorPagesLearning社員(管理者、作業者)の場合は、荷主コードで部課一覧を特定する
                // ※RazorPagesLearning社員の場合は荷主の全部課が見えることになる
                case USER_ACCOUNT.ACCOUNT_PERMISSION.Admin:
                case USER_ACCOUNT.ACCOUNT_PERMISSION.Worker:
                    departmentAdmins = departmentAdminService.read(new DepartmentAdminService.ReadConfig
                    {
                        SHIPPER_CODE = userAccount.CURRENT_SHIPPER_CODE
                    });
                    break;

                // 上記以外の場合は、ユーザーアカウントID、荷主コードで部課一覧を特定する
                case USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperBrowsing:
                case USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperEditing:
                case USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany:
                    var userDepartmentService = new UserDepartmentService(db, user, signInManager, userManager);
                    var userDepartments = userDepartmentService.read(userAccount.ID, userAccount.CURRENT_SHIPPER_CODE);
                    departmentAdmins = departmentAdminService.read(
                        userAccount.CURRENT_SHIPPER_CODE,
                        userDepartments.result.Select(e => e.DEPARTMENT_CODE).ToList()
                    );
                    break;
            }

            return departmentAdmins.result;
        }

        /// <summary>
        /// パスワードの有効期限チェック
        /// </summary>
        /// <returns></returns>
        public bool passwordExpirationCheck(HoldsPendingInformation_USER_ACCOUNT tuser)
        {
            //更新間隔を取得
            var interval = this.policyService.read(
                new PolicyService.ReadConfig
                {
                    NAME = POLICY.PASSWORD_POLICY.Interval
                });
            if (false == interval.succeed)
            {
                throw new ApplicationException("パスワードの更新間隔情報の取得に失敗しました。");
            }

            //現在時刻
            var nowTime = DateTimeOffset.Now;

            //1. 遅延更新情報において、パスワード更新要求が来ているか確認
            //この場合、こちら側を優先して判断する
            if (null != tuser.WK_USER_ACCOUNT)
            {
                #region 遅延更新情報
                if (true == tuser.WK_USER_ACCOUNT.IS_NEED_PASSWORD_UPDATE)
                {
                    var diff = (nowTime - tuser.WK_USER_ACCOUNT.UPDATED_AT).TotalDays;
                    if (interval.result.VALUE < diff)
                    {
                        //遅延更新情報でパスワードの変更後に指定時間を経過してしまった場合
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                #endregion
            }

            //通常のパスワード更新情報
            {
                #region 通常の更新情報

				var diff = (nowTime - (DateTimeOffset)tuser.PASSWORD_UPDATED_AT).TotalDays;
                if (interval.result.VALUE < diff)
                {
                    //パスワード更新期間を超えている
                    return true;
                }
                else
                {
                    return false;
                }

                #endregion
            }
        }

        /// <summary>
        /// パスワードの更新要求をチェックする
        /// </summary>
        /// <returns></returns>
        public bool checkPasswordUpdateRequest(HoldsPendingInformation_USER_ACCOUNT tuser)
        {
            #region ローカル関数

            //パスワード変更要求の確認
            bool check_PASSWORD_CHANGE_REQUEST()
            {
                if (true == tuser.PASSWORD_CHANGE_REQUEST)
                {
                    //遅延更新が来ているか確認する
                    if (null != tuser.WK_USER_ACCOUNT)
                    {
                        if (true == tuser.WK_USER_ACCOUNT.IS_NEED_PASSWORD_UPDATE)
                        {
                            //更新要求が来ているので、更新したものと判定する
                            return false;
                        }
                        else
                        {
                            //更新要求が無いので、更新が必要
                            return true;
                        }
                    }

                    //更新した形跡がないので、更新が必要
                    return true;
                }

                return false;
            }

            #endregion

            #region 入力値チェック
            if (null == tuser)
            {
                throw new ArgumentException("引数tuserがnullです。");
            }
            #endregion

            //-- -- -- -- --
            //-- Main処理

            //1. パスワード無期限フラグが立っていたら更新の必要なし
            if (true == tuser.EXPIRE_FLAG)
            {
                //変更不要
                return false;
            }

            //2. パスワード変更要求フラグが立っていたら変更指示
            if (true == check_PASSWORD_CHANGE_REQUEST())
            {
                //変更が必要
                return true;
            }

            //3. パスワード有効期限が切れていたら変更する
            if (true == passwordExpirationCheck(tuser))
            {
                return true;
            }

            //それ以外の場合、パスワードの更新は不要であると判断する

            return false;
        }


        /// <summary>
        /// 更新待機中の情報を含めて検索してユーザー情報を取得する
        /// </summary>
        /// <param name="USER_ID"></param>
        /// <returns></returns>
        public HoldsPendingInformation_USER_ACCOUNT readWithPendingInfo(string USER_ID)
        {
            //ユーザーIDを読み取る
            var tUid = USER_ID.Trim();
            {
                #region ユーザー名が変更されている場合
                //1. 指定されたユーザー名が適応待機中の項目に存在するか確認する
                var uq = this.db.WK_USER_ACCOUNTs.Where(e => e.USER_ID == tUid);
                switch (uq.Count())
                {
                    case 0:
                        {
                            //適応不要
                            break;
                        }
                    case 1:
                        {
                            //存在した値を返す
                            var wk_a = uq.First();
                            var a = this.makeBasicQueryUSER_ACCOUNT().Where(e => e.ID == wk_a.USER_ACCOUNT_ID).First();
                            return convert(a);
                        }
                    default:
                        {
                            //想定外パターン
                            throw new ApplicationException("システムエラー : ユーザー名称が重複して登録されています。");
                        }
                }
                #endregion
            }

            {
                #region それ以外の項目が変更されていて、ワークテーブルが存在する場合

                //ユーザー情報を取得する
                var uq = this.makeBasicQueryUSER_ACCOUNT().Where(e => e.USER_ID == tUid);
                switch (uq.Count())
                {
                    case 0:
                        {
                            //ユーザー不明
                            return null;
                        }
                    case 1:
                        {
                            //遅延適応が存在する場合、適応する
                            return convert(uq.First());
                        }
                    default:
                        {
                            //想定外パターン
                            throw new ApplicationException("システムエラー : ユーザー名称が重複して登録されています。");
                        }
                }

                #endregion
            }

            //ユーザーIDが見つからない
            throw new ApplicationException("指定されたユーザーIDが存在しません。");
        }

        /// <summary>
        /// ID指定でユーザー情報を取り込む
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public USER_ACCOUNT read(Int64 ID)
        {
            return makeBasicQueryUSER_ACCOUNT()
                .Where(e => e.ID == ID).FirstOrDefault();
        }


        /// <summary>
        /// ログイン中のユーザー情報から取得できる
        /// </summary>
        public class ReadFromClaimsPrincipalConfig
        {
            /// <summary>
            /// ユーザー情報
            /// </summary>
            public ClaimsPrincipal userInfo;

        }

        /// <summary>
        /// 現在のログイン中のユーザー情報を取得する
        /// </summary>
        /// <param name="db"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<USER_ACCOUNT> read(ReadFromClaimsPrincipalConfig config)
        {
            //現在のユーザー情報を読み取る
            var u = await userManager.GetUserAsync(config.userInfo);
            if (null != u)
            {
                //ユーザー情報を読み取る
                var uac = read(Int64.Parse(u.UserName));
                if (null != uac)
                {
                    return uac;
                }
            }

            return null;
        }

        /// <summary>
        /// ログインユーザーパスワード設定
        /// </summary>
        public class CheckLoginUserPasswordConfig
        {

            /// <summary>
            /// パスワード
            /// </summary>
            public string password;

            /// <summary>
            /// パスワードのソルト
            /// </summary>
            public string passwordSalt;

            /// <summary>
            /// ユーザー情報
            /// </summary>
            public ClaimsPrincipal userInfo;

            /// <summary>
            /// ユーザーマネージャー
            /// </summary>
            public UserManager<IdentityUser> userManager;

        }

        /// <summary>
        /// ログイン中のユーザーのパスワードを判定する
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<bool> checkLoginUserPassword(CheckLoginUserPasswordConfig config)
        {
            //現在のユーザー情報を読み取る
            var u = await config.userManager.GetUserAsync(config.userInfo);
            if (null != u)
            {
                //ユーザー情報を読み取る
                var uac = read(Int64.Parse(u.UserName));
                if (null != uac)
                {
                    if (null != uac.PASSWORD)
                    {
                        //指定されたパスワードが一致するか判定する
                        if (HelperFunctions.toHashString(config.password, uac.PASSWORD_SALT) == uac.PASSWORD)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 統計情報
        /// </summary>
        public class StatisticsInfo
        {
            /// <summary>
            /// データ件数
            /// </summary>
            public int numbers;
        }

        /// <summary>
        /// 統計情報
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public StatisticsInfo getStatistics()
        {
            return new StatisticsInfo
            {
                numbers = db.USER_ACCOUNTs.Count()
            };
        }

        /// <summary>
        /// ログイン設定
        /// </summary>
        public class LoginConfig
        {
            /// <summary>
            /// ユーザー名
            /// </summary>
            public string userName;

            /// <summary>
            /// パスワード
            /// </summary>
            public string password;

        }

        /// <summary>
        /// システムにログインする
        /// </summary>
        /// <param name="login"></param>
        public async Task<bool> login(LoginConfig config)
        {
            #region ローカル関数

            //適応待機中の更新項目を適応する
            async Task<bool> LF_reflectPendingChanges(IDbContextTransaction transaction)
            {
                //ユーザーIDを読み取る
                var tUid = config.userName.Trim();

                {
                    #region ユーザー名が変更されている場合
                    //1. 指定されたユーザー名が適応待機中の項目に存在するか確認する
                    var uq = this.db.WK_USER_ACCOUNTs.Where(e => e.USER_ID == tUid);
                    switch (uq.Count())
                    {
                        case 0:
                            {
                                //適応不要
                                break;
                            }
                        case 1:
                            {
                                //適応耐久の項目があるので適応する
                                var r = await this.reflectPendingChanges(tUid, transaction);
                                return r.succeed;
                            }
                        default:
                            {
                                //想定外パターン
                                throw new ApplicationException("システムエラー : ユーザー名称が重複して登録されています。");
                            }
                    }
                    #endregion
                }

                {
                    #region それ以外の項目が変更されていて、ワークテーブルが存在する場合

                    //ユーザー情報を取得する
                    var uq = this.db.USER_ACCOUNTs.Where(e => e.USER_ID == tUid);
                    switch (uq.Count())
                    {
                        case 0:
                            {
                                //適応不要
                                throw new ApplicationException("指定されたユーザーIDが存在しません。");
                            }
                        case 1:
                            {
                                //適応耐久の項目があるので適応する
                                var u = uq.First();
                                if (null != u.WK_USER_ACCOUNT)
                                {
                                    //遅延適応が存在する場合、適応する
                                    var r = await this.reflectPendingChanges(tUid, transaction);
                                    return r.succeed;
                                }
                                break;
                            }
                        default:
                            {
                                //想定外パターン
                                throw new ApplicationException("システムエラー : ユーザー名称が重複して登録されています。");
                            }
                    }

                    #endregion
                }

                return true;
            }

            #endregion

            try
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    //ユーザー情報に関して、適応更新待機中の項目が有ったら適応する
                    {
                        var r = await LF_reflectPendingChanges(transaction);
                        if (false == r)
                        {
                            throw new ApplicationException("遅延中のデータ適応に失敗");
                        }
                    }

                    //ユーザーIDを読み取る
                    var tUid = config.userName.Trim();
                    var tUSER_ACCOUNT = await
                        this.db.USER_ACCOUNTs.Where(e => e.USER_ID == tUid).FirstOrDefaultAsync();
                    if (null != tUSER_ACCOUNT)
                    {
                        //ログイン有効化をチェック
                        if (false == tUSER_ACCOUNT.LOGIN_ENABLE_FLAG)
                        {
                            return false;
                        }
#if false

                        if (true == this.checkPasswordUpdateRequest(convert(tUSER_ACCOUNT)))
                        {
                            //パスワード更新要求が来ていたらログインさせない
                            return false;
                        }
#endif

                        //ハッシュ化されたパスワード文字列を取得する
                        var hashedPassword =
                            HelperFunctions.toHashString(config.password,
                            tUSER_ACCOUNT.PASSWORD_SALT);

                        var r = await this.signInManager.PasswordSignInAsync
                            (tUSER_ACCOUNT.ID.ToString(), hashedPassword, true, false);
                        if (true == r.Succeeded)
                        {
                            var ui = read(config.userName);
                            if (null != ui)
                            {
                                //ログイン時間を記録する
                                ui.LOGINED_AT = DateTimeOffset.Now;

                                // 管理者、作業者権限のみ、選択中荷主コードの内容を初期化する
                                if (ui.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.Admin
                                    || ui.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.Worker)
                                {
                                    ui.CURRENT_SHIPPER_CODE = null;
                                }

                                // TODO：ログイン前に削除したいデータの処理をここに書く >>>>>                                

                                // 在庫ワークにある情報を削除する
                                // 新規登録の情報、再入庫ダイレクトの情報、データ取込の情報
                                this.db.WK_STOCKs.RemoveRange(
                                    this.db.WK_STOCKs
                                    .Where(e => e.USER_ACOUNT_ID == ui.ID)
                                );

                                // <<<<< TODO：ログイン前に削除したいデータの処理をここに書く

                                await db.SaveChangesAsync();
                                transaction.Commit();

                                return true;
                            }
                        }
                        else
                        {
                            throw new ApplicationException("ログイン処理に失敗しました。");
                        }
                    }
                }
            }
            catch (Exception)
            {
                //処理に失敗したにログイン失敗扱いとする
            }

            return false;
        }

        /// <summary>
        /// パスワード更新期限切れか判定する
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="user"></param>
        /// <returns>true : 更新期限切れ、　false : 更新期限内</returns>
        public bool isPasswordExpiration(int interval, USER_ACCOUNT user)
        {
            if (true == user.EXPIRE_FLAG)
            {
                //有効期限無視フラグが立っていたら常に有効期限切れにならない
                return false;
            }
            else
            {
                //パスワード更新情報が無かったらエラー扱いとする
                if (true == user.PASSWORD_UPDATED_AT.HasValue)
                {
                    var diff = DateTimeOffset.Now - user.PASSWORD_UPDATED_AT.Value;
                    if (interval > diff.Days)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //必要に応じて以下関数を使用する
#if false
        /// <summary>
        /// パスワードが期限切れか判定する
        /// </summary>
        /// <returns></returns>
        public bool isPasswordExpiration(Int64 userId)
        {
            var d = this.policyService.read(new DB.PolicyService.ReadConfig
            {
                NAME = POLICY.PASSWORD_POLICY.Interval
            });
            if (true == d.succeed)
            {
                var u = read(userId);
                return isPasswordExpiration(d.result.VALUE, u);
            }
            else
            {
                throw new ApplicationException
                    ("セキュリティポリシーポリシーからパスワード更新間隔が取得できません。");
            }
        }
#endif

        /// <summary>
        /// フレームに表示する荷主（運送会社）名を取得
        /// </summary>
        /// <returns></returns>
        public async Task<string> getShipperNameForFrame()
        {
            Service.DB.ShipperAdminService shipperAdminService = new Service.DB.ShipperAdminService(this.db, this.user, this.signInManager, this.userManager);
            Service.DB.TransportAdminService transportAdminService = new Service.DB.TransportAdminService(this.db, this.user, this.signInManager, this.userManager);

            string retName = "";

            ReadFromClaimsPrincipalConfig principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = this.user };
            var userData = await this.read(principalConfig);

            if ((true == this.user.IsInRole(RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperEditing.ToString()))
                || (true == this.user.IsInRole(RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperBrowsing.ToString())))
            {
                // 荷主権限の場合は、ユーザーが属する荷主の名前
                Service.DB.ShipperAdminService.ReadConfig sa_readconfig = new Service.DB.ShipperAdminService.ReadConfig() { SHIPPER_CODE = userData.CURRENT_SHIPPER_CODE };
                retName = shipperAdminService.read(sa_readconfig).result.SHIPPER_NAME;
            }
            else if ((true == this.user.IsInRole(RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany.ToString())))
            {
                // 運送会社権限の場合は、運送会社名
                var ta = transportAdminService.read(userData.TRANSPORT_ADMIN_CODE);
                retName = ta.First().NAME;
            }
            else
            {
                // 管理者、作業者権限の場合は、選択された荷主の名前
                // memo：ホーム画面の処理より後に呼ばれるため、CURRENT_SHIPPER_CODEのみ参照すればよい。
                //       CURRENT_SHIPPER_CODEは必ずデータが入っているもの

                // 荷主一覧の取得、情報確保
                Service.DB.ShipperAdminService.ReadConfig sa_readconfig = new Service.DB.ShipperAdminService.ReadConfig() { SHIPPER_CODE = userData.CURRENT_SHIPPER_CODE };
                retName = shipperAdminService.read(sa_readconfig).result.SHIPPER_NAME;
            }

            return retName;
        }

        /// <summary>
        /// 顧客専用項目
        /// </summary>
        /// <returns>true:あり、false:なし</returns>
        public async Task<bool> customerOnlyFlag()
        {
            ReadFromClaimsPrincipalConfig principalConfig = new ReadFromClaimsPrincipalConfig() { userInfo = this.user };
            var userData = await this.read(principalConfig);

			var shipperAdmin = db.SHIPPER_ADMINs
				.AsQueryable()
				.Where(e => e.SHIPPER_CODE == userData.CURRENT_SHIPPER_CODE)
				.FirstOrDefault() ?? throw new NullReferenceException("荷主マスタが存在しません。");

            return shipperAdmin.CUSTOMER_ONLY_FLAG;
        }


        /// <summary>
        /// 選択中荷主コードの更新
        /// </summary>
        /// <param name="shipperCode">更新対象の荷主コード</param>
        /// <returns></returns>
        public async Task updateCurrentShipperCode(string shipperCode)
        {
            var user = await readLoggedUserInfo();
            var target = this.db.USER_ACCOUNTs.First(e => e.ID == user.ID);

            target.CURRENT_SHIPPER_CODE = shipperCode;
            await this.db.SaveChangesAsync();
        }


        #region 実装補助

        /// <summary>
        /// 実装補助関数
        /// </summary>
        internal static class HelperFunctions
        {
            /// <summary>
            /// パスワード用のソルトを生成する
            /// </summary>
            /// <returns></returns>
            public static string getSaltString()
            {
                // generate a 128-bit salt using a secure PRNG
                byte[] salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
                return Convert.ToBase64String(salt);
            }


            /// <summary>
            /// ハッシュ文字列に変換する
            /// </summary>
            /// <param name="src"></param>
            /// <returns></returns>
            public static string toHashString(string src, string salt)
            {
                if (null == src)
                {
                    throw new ArgumentException("srcがnullです。");
                }
                if (null == salt)
                {
                    throw new ArgumentException("saltがnullです。");
                }

                // 入力したパスワードを文字列をbyte型配列に変換
                byte[] data = System.Text.Encoding.UTF8.GetBytes(src + salt);

                //CryptoServiceProviderオブジェクトを作成
                using (var cp = new System.Security.Cryptography.SHA512CryptoServiceProvider())
                {
                    // ハッシュ値を計算
                    byte[] bs = cp.ComputeHash(data);

                    // リソースを解放
                    cp.Clear();

                    // byte型配列を16進数の文字列に変換
                    System.Text.StringBuilder result = new System.Text.StringBuilder();
                    foreach (byte b in bs)
                    {
                        result.Append(b.ToString("x2"));
                    }

                    return result.ToString();
                }
            }
        }

        #endregion

    }
}
