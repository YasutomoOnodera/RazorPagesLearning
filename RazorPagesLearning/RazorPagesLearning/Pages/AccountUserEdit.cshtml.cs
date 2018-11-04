using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Utility.SelectItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RazorPagesLearning.Pages
{
    [Authorize(Roles = "Admin", Policy = "PasswordExpiration")]
    public class AccountUserEditModel : PageModel
    {
        /// <summary>
        /// 画面上に表示すべき情報
        /// </summary>
        public class ViewModel
        {
            #region 更新対象データ
            /// <summary>
            /// ユーザー情報
            /// </summary>
            public RazorPagesLearning.Data.Models.USER_ACCOUNT USER_ACCOUNT { get; set; }

            /// <summary>
            /// 衝突検知用のタイムスタンプ
            /// </summary>
            public string collisionDetectionTimestamp { get; set; }

            /// <summary>
            /// 新規テータか判定する
            /// </summary>
            public bool isNewData { get; set; }

            #region パスワード情報


            /// <summary>
            /// パスワード情報
            /// </summary>
            public class PasswordInfo
            {
                /// <summary>
                /// 管理者パスワード
                /// </summary>
                public string administratorPassword { get; set; }

                /// <summary>
                /// パスワード
                /// </summary>
                public string password { get; set; }

                /// <summary>
                /// パスワード　確認
                /// </summary>
                public string passwordConfirmation { get; set; }

                /// <summary>
                /// 確認用で入力されたパスワードが正しいか確認する
                /// </summary>
                /// <returns></returns>
                public bool confirmation()
                {
                    if ((null != this.password) && (null != this.passwordConfirmation))
                    {
                        if (this.password.Trim() == this.passwordConfirmation.Trim())
                        {
                            return true;
                        }
                    }
                    return false;
                }

                /// <summary>
                /// データを持っているか判定する
                /// </summary>
                /// <returns></returns>
                public bool hasData()
                {
                    if ((null != this.administratorPassword) &&
                        (null != this.password) &&
                        (null != this.passwordConfirmation))
                    {
                        return true;
                    }

                    return false;
                }

                /// <summary>
                /// 更新指定
                /// </summary>
                public bool update { get; set; }


                public PasswordInfo()
                {
                    this.update = true;
                }
            }

            /// <summary>
            /// パスワード更新情報
            /// </summary>
            public PasswordInfo passwordInfo { get; set; }


            #endregion

            #endregion

            /// <summary>
            /// エラーメッセージ
            /// </summary>
            public List<string> errorMessage { get; set; }

            #region 荷主関係

            /// <summary>
            /// 荷主コード
            /// </summary>
            public string SHIPPER_CODE { get; set; }

            /// <summary>
            /// 荷主名
            /// </summary>
            public string SHIPPER_NAME { get; set; }

            #endregion

            #region 選択項目制御

            #region 権限関係

            /// <summary>
            /// アカウント権限を選択する
            /// </summary>
            public SelectItemSet_AccountPermissions accountPermissions { get; set; }

            #endregion


            #region ログイン有効

            /// <summary>
            /// ログイン有効指定
            /// </summary>
            public class SelectItemSet_LOGIN_ENABLE : RazorPagesLearning.Utility.SelectItem.SelectItemSet
            {
                public void init()
                {
                    this.display = new List<SelectListItem> {
                    new SelectListItem
                    {
                        Value = "true",
                        Text = "有効"
                    },
                    new SelectListItem
                    {
                        Value = "false",
                        Text = "無効"
                    }
                };
                    this.selected = display[0].Value.ToString();
                }


                /// <summary>
                /// 選択されている要素をDB上の要素に変換する
                /// </summary>
                /// <param name="db"></param>
                /// <returns></returns>
                public bool getSelectedElement()
                {
                    try
                    {
                        return bool.Parse(this.selected);
                    }
                    catch
                    {
                        return false;
                    }
                }

                public SelectItemSet_LOGIN_ENABLE()
                {
                    this.init();
                }
            }

            /// <summary>
            /// ログイン有効指定
            /// </summary>
            public SelectItemSet_LOGIN_ENABLE LOGIN_ENABLE { get; set; }

            #endregion


            #region 無期限パスワード

            /// <summary>
            /// パスワード無期限フラグ指定選択アイテム
            /// </summary>
            public class SelectItemSet_EXPIRE_FLAG : RazorPagesLearning.Utility.SelectItem.SelectItemSet
            {
                public void init()
                {
                    this.display = new List<SelectListItem> {
                    new SelectListItem
                    {
                        Value = "true",
                        Text = "はい"
                    },
                    new SelectListItem
                    {
                        Value = "false",
                        Text = "いいえ"
                    }
                };
                    this.selected = display[0].Value.ToString();
                }

                /// <summary>
                /// 選択されている要素をDB上の要素に変換する
                /// </summary>
                /// <param name="db"></param>
                /// <returns></returns>
                public bool getSelectedElement()
                {
                    try
                    {
                        return bool.Parse(this.selected);
                    }
                    catch
                    {
                        return false;
                    }
                }

                public SelectItemSet_EXPIRE_FLAG()
                {
                    this.init();
                }
            }

            /// <summary>
            /// 期限パスワード指定
            /// </summary>
            public SelectItemSet_EXPIRE_FLAG EXPIRE_FLAG { get; set; }

            #endregion


            #region 運送会社関係

            /// <summary>
            /// 運送会社情報
            /// </summary>
            public class SelectItemSet_TRANSPORT_ADMIN : RazorPagesLearning.Utility.SelectItem.SelectItemSet
            {
                public void init(RazorPagesLearning.Service.DB.TransportAdminService transportAdminService)
                {
                    this.display = transportAdminService.read()
                        .Select(e =>
                            new SelectListItem
                            {
                                Value = e.CODE.ToString(),
                                Text = e.NAME
                            }).ToList();
                    if (0 <= this.display.Count)
                    {
                        this.selected = this.display[0].Text;
                    }
                }


                /// <summary>
                /// 選択されている要素をDB上の要素に変換する
                /// </summary>
                /// <param name="db"></param>
                /// <returns></returns>
                public TRANSPORT_ADMIN getSelectedElement(RazorPagesLearning.Service.DB.TransportAdminService transportAdminService)
                {
                    try
                    {
                        return transportAdminService.read(this.selected).FirstOrDefault();
                    }
                    catch
                    {
                        return null;
                    }
                }

                public SelectItemSet_TRANSPORT_ADMIN()
                {
                    this.display = new List<SelectListItem>();
                    this.selected = "";
                }
            }

            /// <summary>
            /// 運送会社情報
            /// </summary>
            public SelectItemSet_TRANSPORT_ADMIN TRANSPORT_ADMIN { get; set; }

            #endregion

            #region 部課一覧

            /// <summary>
            /// 部課情報一覧
            /// </summary>
            public class SelectItemSet_DEPARTMENT_ADMIN : RazorPagesLearning.Utility.SelectItem.SelectItemSet
            {
                public void init(RazorPagesLearning.Service.DB.DepartmentAdminService departmentAdminService, string SHIPPER_CODE)
                {
                    this.display = departmentAdminService.read(
                        new RazorPagesLearning.Service.DB.DepartmentAdminService.ReadConfig
                        {
                            SHIPPER_CODE = SHIPPER_CODE
                        })
                        .result
                        .Select(e =>
                            new SelectListItem
                            {
                                Value = e.DEPARTMENT_CODE.ToString(),
                                Text = e.DEPARTMENT_NAME
                            })
                        .ToList();
                    if (0 < this.display.Count)
                    {
                        this.selected = this.display[0].Text;
                    }
                }

                /// <summary>
                /// 選択されている要素をDB上の要素に変換する
                /// </summary>
                /// <param name="db"></param>
                /// <returns></returns>
                public USER_DEPARTMENT getSelectedElement
                    (RazorPagesLearning.Service.DB.DepartmentAdminService departmentAdminService,
                    string SHIPPER_CODE)
                {
                    try
                    {
                        var r = departmentAdminService.read(
                            new DepartmentAdminService.ReadConfig
                            {
                                DEPARTMENT_CODE = this.selected,
                                SHIPPER_CODE = SHIPPER_CODE
                            });
                        if (true == r.succeed)
                        {
                            var rr = r.result.FirstOrDefault();
                            if (null != rr)
                            {
                                return new USER_DEPARTMENT
                                {
                                    DEPARTMENT_CODE = rr.DEPARTMENT_CODE,
                                    SHIPPER_CODE = rr.SHIPPER_CODE
                                };
                            }
                        }

                        throw new ApplicationException("指定された条件に合致する部課コードが取得できません。");
                    }
                    catch
                    {
                        return null;
                    }
                }

                public SelectItemSet_DEPARTMENT_ADMIN()
                {
                    this.display = new List<SelectListItem>();
                    this.selected = "";
                }
            }

            /// <summary>
            /// デフォルト部課
            /// </summary>
            public SelectItemSet_DEPARTMENT_ADMIN DEFAULT_DEPARTMENT_ADMIN { get; set; }

            #endregion

            #endregion

            #region 部課一覧

            /// <summary>
            /// ユーザーが所属する部課一覧
            /// </summary>
            public class Belongs_DEPARTMENT
            {
                /// <summary>
                /// 選択されている
                /// </summary>
                public bool selected { get; set; }


                #region postで戻ってきたときの追跡用識別子

                /// <summary>
                /// 荷主コード
                /// </summary>
                public string SHIPPER_CODE { get; set; }

                /// <summary>
                /// 部課コード
                /// </summary>
                public string DEPARTMENT_CODE { get; set; }

                #endregion

                /// <summary>
                /// 部課情報
                /// </summary>
                public DEPARTMENT_ADMIN DEPARTMENT_ADMIN { get; set; }

                public Belongs_DEPARTMENT()
                {
                    this.DEPARTMENT_ADMIN = new DEPARTMENT_ADMIN();
                }
            }

            /// <summary>
            /// ユーザーが所属する部課情報一覧
            /// </summary>
            public List<Belongs_DEPARTMENT> Belongs_DEPARTMENTs { get; set; }

            /// <summary>
            /// ユーザー部課一覧へ変換
            /// </summary>
            /// <returns></returns>
            public IEnumerable<USER_DEPARTMENT> toUSER_DEPARTMENTs
                (RazorPagesLearning.Service.DB.DepartmentAdminService departmentAdminService,
                    string SHIPPER_CODE)
            {
                //選択されている項目をユーザー選択部課へ変換する
                if (null != this.Belongs_DEPARTMENTs)
                {
                    return this.Belongs_DEPARTMENTs
                    .Where(e => e.selected == true)
                    .Select(e =>
                    {
                        //部課情報に変換する
                        var r = departmentAdminService.read(new DepartmentAdminService.ReadConfig
                        {
                            SHIPPER_CODE = SHIPPER_CODE,
                            DEPARTMENT_CODE = e.DEPARTMENT_CODE
                        });
                        if (true == r.succeed)
                        {
                            var rr = r.result.FirstOrDefault();
                            if (null != rr)
                            {
                                return rr;
                            }
                        }
                        return new DEPARTMENT_ADMIN
                        {
                            SHIPPER_CODE = "-1",
                            DEPARTMENT_CODE = "-1"
                        };
                    })
                    .Select(e =>
                    //ユーザー部課情報に変換する
                        new USER_DEPARTMENT
                        {
                            SHIPPER_CODE = e.SHIPPER_CODE,
                            DEPARTMENT_CODE = e.DEPARTMENT_CODE
                        }
                    );
                }
                else
                {
                    return null;
                }
            }

            #endregion

            /// <summary>
            /// 画面上で選択された情報をDBデータに反映できるように変換する
            /// 
            /// 権限毎に異なるデータ
            /// 
            /// </summary>
            public Tuple<bool,CONFIG_TYPE> makeAddContext<CONFIG_TYPE>(
                RazorPagesLearning.Service.DB.TransportAdminService transportAdminService,
                RazorPagesLearning.Service.DB.DepartmentAdminService departmentAdminService)
                where CONFIG_TYPE : Service.User.UserService.ChangeConfig, new()
            {
                #region ローカル関数

                //データの内容を確認する
                bool LF_verifyContentsData(CONFIG_TYPE target)
                {
                    if (this.USER_ACCOUNT.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperBrowsing ||
                        this.USER_ACCOUNT.PERMISSION == USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperEditing)
                    {
                        //荷主権限の時に部課情報をチェック

                        //部課に所属しているか確認する
                        {
                            var selected = this.Belongs_DEPARTMENTs.Where(e => e.selected == true);
                            if (0 == selected.Count())
                            {
                                this.errorMessage = new List<string>() {
                            "所属部課が指定されていません。"
                        };
                                return false;
                            }
                        }

                        //デフォルト部課が所属部課として選択されているか確認する
                        {
                            var selected = target.belongsUSER_DEPARTMENT.Where(e =>
                            e.DEPARTMENT_CODE == target.DEFAULT_USER_DEPARTMENT.DEPARTMENT_CODE &&
                            e.SHIPPER_CODE == target.DEFAULT_USER_DEPARTMENT.SHIPPER_CODE
                            );
                            if (0 == selected.Count())
                            {
                                this.errorMessage = new List<string>() {
                                    "デフォルト部課コードが所属部課コードとして選択されていません。"
                                };
                                return false;
                            }

                        }
                    }

                    return true;
                }

                #endregion

                var adc = new CONFIG_TYPE()
                {
                    password = passwordInfo.password,
                    USER_ACCOUNT = this.USER_ACCOUNT
                };

                //権限を変換
                adc.USER_ACCOUNT.PERMISSION = this.accountPermissions.selected.toACCOUNT_PERMISSION();

                //権限固有の情報を取り込む
                switch (adc.USER_ACCOUNT.PERMISSION)
                {
                    case Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperBrowsing:
                    case Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperEditing:
                        {
                            //取り込まないといけないのは

                            #region 作業者関連情報の取り込み

                            //デフォルト部課
                            adc.DEFAULT_USER_DEPARTMENT =
                                this.DEFAULT_DEPARTMENT_ADMIN?.getSelectedElement
                                (departmentAdminService, this.SHIPPER_CODE)
                                ;

                            //関連部課
                            adc.belongsUSER_DEPARTMENT =
                                this.toUSER_DEPARTMENTs(departmentAdminService, this.SHIPPER_CODE)?.ToList();

                            #endregion
                            break;
                        }
                    case USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany:
                        {
                            #region 運送会社情報の取り込み
                            adc.TRANSPORT_ADMIN =
                                this.TRANSPORT_ADMIN?.getSelectedElement(transportAdminService);

                            #endregion
                            break;
                        }
                }

                //全権限で共通の情報を取り込む
                {
                    #region 全権限で共通の情報を取り込む

                    adc.USER_ACCOUNT.LOGIN_ENABLE_FLAG = this.LOGIN_ENABLE.getSelectedElement();
                    adc.USER_ACCOUNT.EXPIRE_FLAG = this.EXPIRE_FLAG.getSelectedElement();

                    #endregion
                }

                {
                    #region 更新ユーザー情報を追加する

                    adc.createdUserAccountId = USER_ACCOUNT.ID;
                    adc.updatedUserAccountId = USER_ACCOUNT.ID;
                    adc.USER_ACCOUNT.PASSWORD_UPDATED_AT = USER_ACCOUNT.PASSWORD_UPDATED_AT;

                    #endregion
                }

                //登録された内容をチェックする
                if (false == LF_verifyContentsData(adc))
                {
                    return Tuple.Create<bool , CONFIG_TYPE>(false, null);
                }

                return Tuple.Create<bool,CONFIG_TYPE> (true,adc);
            }


            public ViewModel()
            {
                this.isNewData = false;
                this.passwordInfo = new PasswordInfo();

                //固定値系
                this.accountPermissions = new SelectItemSet_AccountPermissions();
                this.LOGIN_ENABLE = new SelectItemSet_LOGIN_ENABLE();
                this.EXPIRE_FLAG = new SelectItemSet_EXPIRE_FLAG();

                this.SHIPPER_CODE = "";
                this.SHIPPER_NAME = "";

                //DBから読み取った値で表示する
                this.TRANSPORT_ADMIN = new SelectItemSet_TRANSPORT_ADMIN();
                this.DEFAULT_DEPARTMENT_ADMIN = new SelectItemSet_DEPARTMENT_ADMIN();
                this.Belongs_DEPARTMENTs = new List<Belongs_DEPARTMENT>();
            }
        }

        /// <summary>
        /// 画面上の表示項目と同期する情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        //コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;


        public readonly RazorPagesLearning.Service.User.UserService userService;
        public readonly RazorPagesLearning.Service.DB.PolicyService policyService;
        private readonly RazorPagesLearning.Service.DB.ShipperAdminService shipperAdminService;

        private readonly RazorPagesLearning.Service.DB.TransportAdminService transportAdminService;
        private readonly RazorPagesLearning.Service.DB.DepartmentAdminService departmentAdminService;


        public AccountUserEditModel(RazorPagesLearning.Data.RazorPagesLearningContext db,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;

            this.userService = new RazorPagesLearning.Service.User.UserService(db, User, signInManager, userManager);
            this.policyService = new RazorPagesLearning.Service.DB.PolicyService(db, User, signInManager, userManager);

            this.shipperAdminService = new RazorPagesLearning.Service.DB.ShipperAdminService(db, User, signInManager, userManager);
            this.transportAdminService = new RazorPagesLearning.Service.DB.TransportAdminService(db, User, signInManager, userManager);
            this.departmentAdminService = new RazorPagesLearning.Service.DB.DepartmentAdminService(db, User, signInManager, userManager);

            //viewモデルの更新
            this.viewModel = new ViewModel();
            this.viewModel.TRANSPORT_ADMIN.init(this.transportAdminService);

        }

        /// <summary>
        /// サービス情報を更新する
        /// </summary>
        private void updateService()
        {
            //PageModelのコンストラクタでは、ユーザーサービス情報は引き渡されない。
            //実際にこれらの情報に触れるようになるのは、アクションメソッドが呼ばれたタイミングである。
            //このため、アクションメソッドが呼ばれたタイミングでユーザー情報を更新する
            this.userService.updateUser(User);
            this.policyService.updateUser(User);
            this.shipperAdminService.updateUser(User);
            this.transportAdminService.updateUser(User);
            this.departmentAdminService.updateUser(User);
        }

        /// <summary>
        /// 表示情報を初期化する
        /// </summary>
        /// <param name="USER_ID"></param>
        private void initViewData(string USER_ID)
        {
            if (null != USER_ID)
            {
                var r = userService.readWithPendingInfo(USER_ID);
                if (null != r)
                {
                    this.viewModel = new ViewModel();
                    this.viewModel.USER_ACCOUNT = r;
                    this.viewModel.isNewData = false;
                    this.viewModel.collisionDetectionTimestamp =
                        this.viewModel.USER_ACCOUNT.timestampToString();

                    //権限を更新する
                    this.viewModel.accountPermissions.selected =
                        this.viewModel.USER_ACCOUNT.PERMISSION.ToString();

                    //荷主コードを取り込み(関連付け用)
                    this.viewModel.SHIPPER_CODE =
                        this.viewModel.USER_ACCOUNT.CURRENT_SHIPPER_CODE;

                    //ログイン許可を関連付けする
                    this.viewModel.LOGIN_ENABLE.selected
                        = r.LOGIN_ENABLE_FLAG.ToString();

                    //パスワード無期限フラグを有効化する
                    this.viewModel.EXPIRE_FLAG.selected
                        = r.EXPIRE_FLAG.ToString();

                    //関連付けが必要な情報の読み取りを行う
                    reloadRequiredItems();
                }
            }
            else
            {
                //ID指定が無ければ新規登録扱いとする
                this.viewModel = new ViewModel();
                this.viewModel.USER_ACCOUNT = new Data.Models.USER_ACCOUNT();
                this.viewModel.isNewData = true;
            }
        }

        public void OnGet(string user)
        {
            updateService();
            this.initViewData(user);
            this.viewModel.isNewData = false;
            this.viewModel.passwordInfo.update = false;
        }

        /// <summary>
        /// 指定されたIDのデータをコピーして処理する場合
        /// </summary>
        /// <param name="id"></param>
        public void OnGetCopy(string user)
        {
            updateService();
            this.initViewData(user);
            this.viewModel.isNewData = true;
            this.viewModel.passwordInfo.update = true;

            //登録訂正画面では、下記項目はクリアした状態で表示する。																															
            //ユーザーID
            //ユーザー名
            //ユーザー名(カナ)
            //パスワード
            this.viewModel.USER_ACCOUNT.USER_ID = "";
            this.viewModel.USER_ACCOUNT.NAME = "";
            this.viewModel.USER_ACCOUNT.KANA = "";
            this.viewModel.passwordInfo.administratorPassword = "";
            this.viewModel.passwordInfo.password = "";
            this.viewModel.passwordInfo.passwordConfirmation = "";
        }

        /// <summary>
        /// ページ遷移ごとに再度読み取りが必要な項目を読み取る
        /// </summary>
        private void reloadRequiredItems()
        {
            //ユーザー種別が荷主の場合、
            //荷主コードでデータを引っ張ってきて表示内容を修正する
            switch (this.viewModel.accountPermissions.selected.toACCOUNT_PERMISSION())
            {
                case Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperBrowsing:
                case Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperEditing:
                    {
                        //入力途中で遷移したときにnull参照を防ぐ
                        if (null != this.viewModel.SHIPPER_CODE)
                        {
                            #region 部課情報の取り込み
                            var r = this.shipperAdminService.read(new Service.DB.ShipperAdminService.ReadConfig
                            {
                                SHIPPER_CODE = this.viewModel.SHIPPER_CODE.Trim()
                            });
                            if (null == r.result)
                            {
                                this.viewModel.errorMessage = new List<string> {
                                $"荷主コード{this.viewModel.SHIPPER_CODE}に関連する情報は登録されていません。"
                            };
                                return;
                            }
                            else
                            {
                                //荷主コードに関係する情報を読み取る
                                this.viewModel.SHIPPER_NAME = r.result.SHIPPER_NAME;

                                {
                                    #region デフォルト部課
                                    if (null == this.viewModel.DEFAULT_DEPARTMENT_ADMIN)
                                    {
                                        this.viewModel.DEFAULT_DEPARTMENT_ADMIN = new ViewModel.SelectItemSet_DEPARTMENT_ADMIN();
                                    }
                                    this.viewModel.DEFAULT_DEPARTMENT_ADMIN.init(this.departmentAdminService,
                                        this.viewModel.SHIPPER_CODE.Trim());
                                    #endregion
                                }

                                {
                                    #region ユーザーが所属する部課一覧
                                    this.viewModel.Belongs_DEPARTMENTs = departmentAdminService.read(
                                        new Service.DB.DepartmentAdminService.ReadConfig
                                        {
                                            SHIPPER_CODE = this.viewModel.SHIPPER_CODE.Trim()
                                        })
                                        .result
                                        .Select(e =>
                                        new ViewModel.Belongs_DEPARTMENT
                                        {
                                            selected = true,
                                            DEPARTMENT_ADMIN = e,
                                            DEPARTMENT_CODE = e.DEPARTMENT_CODE,
                                            SHIPPER_CODE = e.SHIPPER_CODE
                                        })
                                        .ToList();

                                    if (null != this.viewModel.USER_ACCOUNT)
                                    {
                                        if (null != this.viewModel.USER_ACCOUNT.USER_ID)
                                        {
                                            var tu = this.userService
                                                .readWithPendingInfo(this.viewModel.USER_ACCOUNT.USER_ID);
                                            if (null != tu)
                                            {
                                                //DB上のチェック状態を反映させる
                                                this.viewModel.Belongs_DEPARTMENTs.ForEach(e =>
                                                {
                                                    if (null == tu.USER_DEPARTMENTs
                                                     .Where(in_e =>
                                                     in_e.DEPARTMENT_CODE == e.DEPARTMENT_CODE &&
                                                     in_e.SHIPPER_CODE == e.SHIPPER_CODE
                                                     )
                                                     .FirstOrDefault())
                                                    {
                                                        e.selected = false;
                                                    }
                                                    else
                                                    {
                                                        e.selected = true;
                                                    }
                                                });
                                            }
                                        }
                                    }

                                    #endregion
                                }
                            }
                        }

                        #endregion
                        break;
                    }
                case USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany:
                    {
                        #region 運送会社情報を取り込み

                        this.viewModel.TRANSPORT_ADMIN.init(this.transportAdminService);

                        #endregion
                        break;
                    }
            }

            //以下情報はWebページから帰ってこないので、
            //IDをキーとしてDB上から再度読み取りを行う
            //
            //最終ログイン日時、最終パスワード更新日時
            //アカウント状態、初回登録日時、最終更新日時、最終更新者
            {
                var uinfo = this.userService.read(this.viewModel.USER_ACCOUNT.ID);
                if (null != uinfo)
                {
                    this.viewModel.USER_ACCOUNT.LOGINED_AT = uinfo.LOGINED_AT;
                    this.viewModel.USER_ACCOUNT.PASSWORD_UPDATED_AT = uinfo.PASSWORD_UPDATED_AT;

                    this.viewModel.USER_ACCOUNT.CREATED_AT = uinfo.CREATED_AT;
                    this.viewModel.USER_ACCOUNT.UPDATED_AT = uinfo.UPDATED_AT;
                    this.viewModel.USER_ACCOUNT.CREATED_USER_ACCOUNT_ID = uinfo.CREATED_USER_ACCOUNT_ID;
                    this.viewModel.USER_ACCOUNT.UPDATED_USER_ACCOUNT_ID = uinfo.UPDATED_USER_ACCOUNT_ID;
                }
            }
        }

        /// <summary>
        /// 画面内の各種変更が発生した場合
        /// </summary>
        /// <param name="user"></param>
        public void OnPost(string user)
        {
            updateService();
            reloadRequiredItems();
        }

        /// <summary>
        /// データをDBに登録する
        /// </summary>
        public async Task<IActionResult> OnPostSaveAsync()
        {

            #region ローカル関数

            ///バリデーション結果のうち、意図的に無視項目を除外したバリデーション結果を作成する
            List<ModelStateEntry> LF_makeIgnoredItemsFilteredModelState()
            {
                var r = ModelState.Keys
                    //パスワード列を除外する
                    .Where(e =>
                    false == System.Text.RegularExpressions.Regex.IsMatch(
                    e, @".*PASSWORD$"))
                    //ユーザー所属部署リストの中身はpostされてこないので、
                    //バリデーション結果から除外する
                    .Where(e =>
                    false == System.Text.RegularExpressions.Regex.IsMatch(
                    e, @".*DEPARTMENT_NAME$"))
                    .Select(e => ModelState[e])
                    .ToList();


                return r;
            }

            //このページを再読み込みする
            PageResult LF_reloadThisPage()
            {
                reloadRequiredItems();
                return Page();
            }

            #endregion

            updateService();

            //DB定義上、パスワード列は必須となっているが、
            //パスワード自体はHash化された値を登録するために、
            //画面上では入力されない。
            //このため、ここではパスワードに関するエラーチェックだけは無視する事とする。
            var filteredModelState = LF_makeIgnoredItemsFilteredModelState();

            //ログイン処理を行う
            if (0 == filteredModelState.Where(e => 0 != e.Errors.Count).Count())
            {
                #region バリデーションが正しい場合

                #region パスワード指定がある場合、指定された管理者パスワードが正しいか確認する
                //ログインユーザーのパスワードを確認する
                if ((true == this.viewModel.passwordInfo.update) && (true == this.viewModel.passwordInfo.hasData()))
                {
                    //指定された管理者パスワードがログイン中のユーザーパスワードと一致するか判定する
                    var r = await userService.checkLoginUserPassword(new Service.User.UserService.CheckLoginUserPasswordConfig
                    {
                        password = this.viewModel.passwordInfo.administratorPassword,
                        userInfo = User,
                        userManager = this._userManager
                    });
                    if (false == r)
                    {
                        this.viewModel.errorMessage = new List<string>() {
                            "入力された管理者パスワードが正しくありません。"
                        };
                        goto FILED;
                    }
                }

                #endregion


                //新規登録指定ならば
                if (true == this.viewModel.isNewData)
                {
                    #region 新規登録
                    #region 条件確認
                    //新規登録の場合、パスワード指定は必須とする。
                    if ((false == this.viewModel.passwordInfo.update) && (false == this.viewModel.passwordInfo.hasData()))
                    {
                        this.viewModel.errorMessage = new List<string>() {
                            "新規登録の場合、パスワードを指定してください。"
                        };
                        goto FILED;
                    }

                    //パスワードが正しいか確認
                    if (false == this.viewModel.passwordInfo.confirmation())
                    {
                        this.viewModel.errorMessage = new List<string>() {
                            "入力されたパスワードと確認用パスワードが一致しません。"
                        };
                        goto FILED;
                    }
                    #endregion

                    //画面上で選択された各種一時データをDB保存用の構造体にまとめる
                    var adc = this.viewModel.makeAddContext
                        <Service.User.UserService.ChangeConfig>
                        (transportAdminService, departmentAdminService);
                    if (false == adc.Item1)
                    {
                        //処理失敗
                        goto FILED;
                    }

                    //新規登録する新規登録するデータ
                    var r = await userService.add(adc.Item2);
                    if (false == r.succeed)
                    {
                        this.viewModel.errorMessage = r.errorMessages;
                        goto FILED;
                    }
                    else
                    {
                        //成功したら一覧ページに飛ばす
                        return RedirectToPage("AccountUser");
                    }

                    #endregion
                }
                else
                {
                    //既存情報の更新
                    #region 既存データ更新

                    //更新用のコンテクストを作成する
                    var adc = this.viewModel.makeAddContext
                        <Service.User.UserService.UpdateConfig>
                        (transportAdminService, departmentAdminService);
                    if (false == adc.Item1)
                    {
                        //処理失敗
                        goto FILED;
                    }

                    adc.Item2.updateRange = Service.User.UserService.UpdateConfig.UpdateRange.ALL;

                    //更新の場合、同時更新チェックが必要
                    adc.Item2.collisionDetectionTimestamp = this.viewModel.collisionDetectionTimestamp;

                    //パスワードをチェックする
                    #region パスワードチェック
                    if (true == this.viewModel.passwordInfo.update)
                    {
                        if (false == this.viewModel.passwordInfo.hasData())
                        {
                            this.viewModel.errorMessage = new List<string>() {
                            "新規登録の場合、パスワードを指定してください。"
                        };
                            goto FILED;
                        }

                        //パスワードが一致するか確認する
                        if (false == this.viewModel.passwordInfo.confirmation())
                        {
                            this.viewModel.errorMessage = new List<string>() {
                            "入力されたパスワードと確認用パスワードが一致しません。"
                        };
                            goto FILED;
                        }

                        //パスワードの更新指定を追加する
                        adc.Item2.password = this.viewModel.passwordInfo.password;
                        adc.Item2.isPasswordUpdateRequired = this.viewModel.passwordInfo.update;

                    }
                    #endregion

                    //更新命令を発行する
                    {
                        #region 更新命令

                        //更新処理をかける
                        var r = await userService.update(adc.Item2);
                        if (false == r.succeed)
                        {
                            this.viewModel.errorMessage = r.errorMessages;
                            goto FILED;
                        }
                        else
                        {
                            //成功したら一覧ページに飛ばす
                            return RedirectToPage("AccountUser");
                        }

                        #endregion
                    }

                    #endregion
                }
                #endregion
            }
            else
            {
                #region バリエーションに問題がある場合
                this.viewModel.errorMessage = filteredModelState
                    .SelectMany(e => e.Errors.Select(er => er.ErrorMessage)).ToList();
                #endregion
            }

            FILED:

            //ページ内で再読み込みが必要なデータを読み込む
            return LF_reloadThisPage();
        }
    }
}