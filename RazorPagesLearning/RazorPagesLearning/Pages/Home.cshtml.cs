using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// 追加
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace RazorPagesLearning.Pages
{
    [Authorize(Roles = "PasswordExpiration, Admin, ShipperBrowsing, ShipperEditing, ShippingCompany, Worker")]
    public class HomeModel : PageModel
    {
        #region 画面上に表示すべき情報
        /// <summary>
        /// 画面上に表示すべき情報
        /// </summary>
        public class ViewModel
        {
            /// <summary>
            /// 表示名
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// 荷主一覧情報
            /// </summary>
            public List<SHIPPER_ADMIN> SHIPPER_ADMINs { get; set; }

            /// <summary>
            /// メッセージ情報
            /// </summary>
            public MESSAGE_ADMIN MESSAGE_ADMIN { get; set; }

            /// <summary>
            /// パスワード変更日数メッセージ表示フラグ
            /// </summary>
            public bool changePasswordMessageFlag { get; set; }

            /// <summary>
            /// パスワード変更までの残り日数
            /// </summary>
            public int changePasswordIntervalDate { get; set; }
        }
        #endregion

        /// <summary>
        /// 画面上の表示項目と同期する情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        /// <summary>
        /// ユーザー情報
        /// </summary>
        public USER_ACCOUNT USER_ACCOUNT { get; set; }

        /// <summary>
        /// DBアクセスオブジェクト
        /// </summary>
        public readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;

        // DB
        private Service.User.UserService userService;
        private Service.DB.ShipperAdminService shipperAdminService;
        private Service.DB.MessageAdminService messageAdminService;
        private Service.DB.PolicyService policyService;

        // コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        // コンストラクタ
        public HomeModel(RazorPagesLearning.Data.RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            // memo：Userがこのタイミングではまだnullなので、
            //       DBのインスタンスはコンストラクタでは生成しないこと。

            this._db = db;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;

            //viewモデルの更新
            this.viewModel = new ViewModel();
        }

        /// <summary>
        /// 画面初期表示、荷主選択時
        /// </summary>
        /// <param name="shipper">荷主コード</param>
        public async Task OnGetAsync(string shipper)
        {
            this.userService = new Service.User.UserService(this._db, User, this._signInManager, this._userManager);
            this.shipperAdminService = new Service.DB.ShipperAdminService(this._db, User, this._signInManager, this._userManager);
            this.messageAdminService = new Service.DB.MessageAdminService(this._db, User, this._signInManager, this._userManager);
            this.policyService = new Service.DB.PolicyService(this._db, User, this._signInManager, this._userManager);

            // ユーザー情報の取得
            Service.User.UserService.ReadFromClaimsPrincipalConfig principalConfig = new Service.User.UserService.ReadFromClaimsPrincipalConfig() { userInfo = User };
            var user = await this.userService.read(principalConfig);
            this.USER_ACCOUNT = user;

            // 画面表示処理
            await this.initViewData(shipper);

        }


        #region private関数

        /// <summary>
        /// 画面表示
        /// </summary>
        /// <param name="shipper">荷主コード</param>
        private async Task initViewData(string shipper)
        {
            SHIPPER_ADMIN data;
            string nowShipperCode = null;

            this.viewModel.name = this.USER_ACCOUNT.NAME;

            // 権限
            if ((true == User.IsInRole(RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperEditing.ToString()))
                || (true == User.IsInRole(RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShipperBrowsing.ToString()))
                || (true == User.IsInRole(RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany.ToString()))
                )
            {
                #region 荷主（編集・閲覧）、運送会社の権限

                // メッセージデータを取得
                Service.DB.MessageAdminService.ReadConfig readconfig = new Service.DB.MessageAdminService.ReadConfig() { KIND = MESSAGE_ADMIN.MESSAGE_KIND.Login };
                this.viewModel.MESSAGE_ADMIN = this.messageAdminService.Read(readconfig).result;
                #endregion
            }
            else
            {
                #region 管理者、作業者の権限

                // 荷主一覧の取得、情報確保
                this.viewModel.SHIPPER_ADMINs = this.shipperAdminService.readList().ToList();

                nowShipperCode = this.USER_ACCOUNT.CURRENT_SHIPPER_CODE;
                if(shipper != null)
                {
                    // 選択された荷主コードの方が優先度高
                    nowShipperCode = shipper;
                }

                // 対象の荷主コードチェック
                if (nowShipperCode != null)
                {
                    // 一覧の中の対象の荷主コードのデータを取得
                    data = this.viewModel.SHIPPER_ADMINs.FirstOrDefault(x => x.SHIPPER_CODE == nowShipperCode);
                    if (data == null)
                    {
                        // 存在しない荷主を送信された場合は、一覧の先頭の荷主とする
                        data = this.viewModel.SHIPPER_ADMINs.OrderBy(x => x.SHIPPER_CODE).FirstOrDefault();
                    }
                }
                else
                {
                    // 初回遷移時の処理
                    // 一覧の中の最初のデータを取得
                    data = this.viewModel.SHIPPER_ADMINs.OrderBy(x => x.SHIPPER_CODE).FirstOrDefault();
                }

                // 確定したデータを保持
                nowShipperCode = data.SHIPPER_CODE;
                #endregion
            }

            // 選択した荷主の情報を、DBへ保持する
            if ((true != User.IsInRole(RazorPagesLearning.Data.Models.USER_ACCOUNT.ACCOUNT_PERMISSION.ShippingCompany.ToString())))
            {
                // memo：運送会社は保持しない
                await this.userService.updateCurrentShipperCode(nowShipperCode);
            }

            // パスワード変更文言表示チェック
            this.checkChangePasswordDate();
        }

        /// <summary>
        /// パスワード変更期限の日数をチェック
        /// </summary>
        private void checkChangePasswordDate()
        {
            // ユーザー情報の「パスワード無期限フラグ」をチェック
            // trueの場合は、表示フラグをfalseにして戻る
            if(this.USER_ACCOUNT.EXPIRE_FLAG == true)
            {
                this.viewModel.changePasswordMessageFlag = false;
            }
            else
            {
                // 上記以外の場合は、変更までの日数を求める

                // セキュリティポリシー情報の取得(間隔日数、通知日数)
                // パスワード変更間隔日数
                Service.DB.PolicyService.ReadConfig po_readconfig = new Service.DB.PolicyService.ReadConfig() { NAME = POLICY.PASSWORD_POLICY.Interval };
                int interval = this.policyService.read(po_readconfig).result.VALUE;

                // パスワード変更通知日数
                po_readconfig.NAME = POLICY.PASSWORD_POLICY.Notify;
                int notify = this.policyService.read(po_readconfig).result.VALUE;

                // １．現在パスワードの有効期限日を求める
                DateTimeOffset passwordUpdatedAt = (DateTimeOffset)this.USER_ACCOUNT.PASSWORD_UPDATED_AT;
                DateTimeOffset limitDate = passwordUpdatedAt.AddDays(interval);

                // ２．現在日が、有効期限日から何日前か求める
                DateTimeOffset now = DateTimeOffset.Now;
                TimeSpan subDate = limitDate - now;

                // ３．何日前の値 <= パスワード変更間隔日数
                if (subDate.Days <= notify)
                {
                    // 条件に一致したら、対象の日数で、メッセージを表示
                    this.viewModel.changePasswordMessageFlag = true;
                    this.viewModel.changePasswordIntervalDate = subDate.Days;
                }
                else
                {
                    // 一致しなかったら、メッセージを非表示
                    this.viewModel.changePasswordMessageFlag = false;
                }
            }
        }

        #endregion

    }
}