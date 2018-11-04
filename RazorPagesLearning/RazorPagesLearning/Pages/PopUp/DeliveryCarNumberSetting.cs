using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static RazorPagesLearning.Service.DB.TruckAdminService;

namespace RazorPagesLearning.Pages.PopUp
{
    /// <summary>
    /// 車番号設定画面のview model
    /// </summary>
    public class DeliveryCarNumberSetting
    {
        /// <summary>
        /// 運送会社コード(検索のキーとして使用する)
        /// </summary>
        [BindProperty]
        public string TRANSPORT_ADMIN_CODE { get; set; }

        /// <summary>
        /// 画面上で決定が押下された時に使用されるヘッダー名
        /// </summary>
        public string onDecisionHeaderName { get; set; }

        /// <summary>
        /// 表示用のビューモデル
        /// </summary>
        public class ViewModel
        {
            /// <summary>
            /// 表示する車両マスタ情報
            /// </summary>
            public List<CollisionDetectableTRUCK_ADMIN> TRUCK_ADMINs { get; set; }

            /// <summary>
            /// エラーメッセージ
            /// </summary>
            public string errorMessage { get; set; }

            /// <summary>
            /// ステータス的に編集可能な状態か判定する
            /// </summary>
            public bool isNotEditable { get; set; }

            /// <summary>
            /// モーダル画面を表示中か
            /// </summary>
            [BindProperty]
            public bool isShowModal { get; set; }

            public ViewModel()
            {
                isShowModal = false;
                errorMessage = "";
                this.isNotEditable = false;
            }
        }

        /// <summary>
        /// 表示データ
        /// </summary>
        public ViewModel viewModel { get; set; }

        /// <summary>
        /// コンストラクタで渡される引数
        /// </summary>
        private RazorPagesLearning.Data.RazorPagesLearningContext _db;
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;
        private ILogger<LoginModel> _logger;

        /// <summary>
        /// DI使う関係上、コンストラクタは引数なしで実行する必要あり
        /// </summary>
        public DeliveryCarNumberSetting()
        {
            this.viewModel = new ViewModel();
        }

        /// <summary>
        /// 内部情報初期化関数
        /// </summary>
        /// <param name="ref_db"></param>
        /// <param name="ref_signInManager"></param>
        /// <param name="ref_userManager"></param>
        /// <param name="ref_logger"></param>
        public void init(RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            SignInManager<IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager,
            ILogger<LoginModel> ref_logger,
            string onDecisionHeaderName)
        {
            //引数を取り込み
            this._db = ref_db;
            this._signInManager = ref_signInManager;
            this._userManager = ref_userManager;
            this._logger = ref_logger;

            this.onDecisionHeaderName = onDecisionHeaderName;

        }


        /// <summary>
        /// 使用するサービス一覧
        /// </summary>
        internal class ServiceSet
        {
            /// <summary>
            /// 車両マスタサービス
            /// </summary>
            public readonly Service.DB.TruckAdminService truckAdminService;

            public readonly Service.DB.DomainService domainService;

            public readonly Service.DB.DeliveryRequestService deliveryService;

            public ServiceSet(RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
                ClaimsPrincipal ref_user,
            SignInManager<Microsoft.AspNetCore.Identity.IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager)
            {
                this.truckAdminService = new Service.DB.TruckAdminService
                    (ref_db,
                    ref_user,
                    ref_signInManager,
                    ref_userManager);


                this.domainService = new Service.DB.DomainService
                    (ref_db,
                    ref_user,
                    ref_signInManager,
                    ref_userManager);

                this.deliveryService = new Service.DB.DeliveryRequestService
                    (ref_db,
                    ref_user,
                    ref_signInManager,
                    ref_userManager);

            }
        }

        /// <summary>
        /// サービスセットを生成する
        /// </summary>
        /// <returns></returns>
        private ServiceSet makeServiceSet(ClaimsPrincipal User)
        {
            return new ServiceSet(this._db,
                User,
                this._signInManager,
                this._userManager);
        }


        /// <summary>
        /// 表示するデータを読み取る
        /// </summary>
        public void readDate(ClaimsPrincipal User, string DELIVERY_REQUEST_NUMBER)
        {
            //サービス一覧を作成する
            var serviceSet = makeServiceSet(User);

            //ユーザー一覧を読み取る
            //表示時には、車両管理番号でソートして表示する
            this.viewModel.TRUCK_ADMINs =
                serviceSet.truckAdminService
                .read(this.TRANSPORT_ADMIN_CODE)
                .OrderBy(e => e.TRUCK_MANAGE_NUMBER)
                .Select(e => CollisionDetectableTRUCK_ADMIN.convert(e))
                .ToList();

            //対象となる集配依頼情報を読み取る
            var target = getTargetDELIVERY(serviceSet, DELIVERY_REQUEST_NUMBER);

            //編集可能か判定する
            this.viewModel.isNotEditable = !isEditable(serviceSet, target);

        }

        /// <summary>
        /// 対象となる集配依頼情報を読み取る
        /// </summary>
        /// <param name="serviceSet"></param>
        /// <param name="DELIVERY_REQUEST_NUMBER"></param>
        /// <returns></returns>
        private DELIVERY_REQUEST getTargetDELIVERY(ServiceSet serviceSet, string DELIVERY_REQUEST_NUMBER)
        {
            //該当レコードのステータスを取得する
            var tr = serviceSet.deliveryService.read(new Service.DB.DeliveryRequestService.ReadConfig
            {
                DELIVERY_REQUEST_NUMBER = DELIVERY_REQUEST_NUMBER,
                TRANSPORT_ADMIN_CODE = this.TRANSPORT_ADMIN_CODE
            });
            if (tr.succeed == false)
            {
                throw new ApplicationException(String.Join("<br>", tr.errorMessages));
            }

            //必ずデータが存在するはず
            var target = tr.result.First();

            return target;
        }

        /// <summary>
        /// 編集可能な状態か判定する
        /// </summary>
        /// <param name="serviceSet"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool isEditable(ServiceSet serviceSet, DELIVERY_REQUEST target)
        {
            //ドメインから値を取得
            var domeis = serviceSet.domainService.getValue("00100000", target.STATUS);
            if (domeis.VALUE == "入力可能")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 決定動作実行結果
        /// </summary>
        public enum OnDecisionResult
        {
            /// <summary>
            /// 処理に成功した場合
            /// </summary>
            Success,
            /// <summary>
            /// 処理に失敗して再度同一モーダル画面を描画する必要がある場合
            /// </summary>
            Failure
        }

        /// <summary>
        /// 表示されているデータを保存する
        /// </summary>
        public async Task<OnDecisionResult> onDecision(PageModel parentPage, string DELIVERY_REQUEST_NUMBER)
        {
            //使用するサービスを作成
            var serviceSet = makeServiceSet(parentPage.User);

            //対象となる集配依頼情報を読み取る
            var target = getTargetDELIVERY(serviceSet, DELIVERY_REQUEST_NUMBER);

            //ドメインから値を取得
            if(true == isEditable(serviceSet , target))
            {
                //入力データをチェックする
                #region 入力値チェック
                {
                    //車番号と担当者で対になっていない項目を探す
                    {
                        #region 車番号と担当者で対になっていない項目を探す

                        var cars = this.viewModel.TRUCK_ADMINs.Where(e => e.NUMBER != null && e.NUMBER != String.Empty);
                        var charges = this.viewModel.TRUCK_ADMINs.Where(e => e.CHARGE != null && e.CHARGE != String.Empty);

                        if (cars.Count() != charges.Count())
                        {
                            //担当者の入力列にずれがある
                            this.viewModel.errorMessage = "車番がされているが担当者が入力されていない　もしく　担当者が入力されているが車番が入力されていない列があります。";
                            return OnDecisionResult.Failure;
                        }

                        #endregion
                    }

                    //車番号の割り当てなし
                    {
                        #region 車番号と担当者で対になっていない項目を探す

                        var cars = this.viewModel.TRUCK_ADMINs.Where(e => e.NUMBER != null && e.NUMBER != String.Empty).Distinct();
                        if (0 == cars.Count())
                        {
                            this.viewModel.errorMessage = "1件以上入力してください。";
                            return OnDecisionResult.Failure;
                        }

                        #endregion
                    }

                    //車番号が重複している
                    {
                        var cars = this.viewModel.TRUCK_ADMINs.Where(e => e.NUMBER != null && e.NUMBER != String.Empty);
                        var deduplication = cars.Distinct();
                        if (cars.Count() != deduplication.Count())
                        {
                            this.viewModel.errorMessage = "車番が重複して入力されています。";
                            return OnDecisionResult.Failure;
                        }
                    }
                }
                #endregion

                //対象となる運送会社番号を指定する
                foreach ( var ele in this.viewModel.TRUCK_ADMINs)
                {
                    ele.TRANSPORT_ADMIN_CODE = this.TRANSPORT_ADMIN_CODE;
                }

                //問題ないのでDBに保存する
                {
                    var r = await serviceSet.truckAdminService.update(
                        this.viewModel.TRUCK_ADMINs
                        );

                    await this._db.SaveChangesAsync();
                    switch (r)
                    {
                        case UpdateResults.Success:
                            {
                                return OnDecisionResult.Success;
                            }
                        case UpdateResults.RelatedInformationClear:
                            {
                                this.viewModel.errorMessage = "車番、担当者が消去されました。関連する入力済みの車番、ルートを消去しました。";
                                return OnDecisionResult.Failure;
                            }
                    }
                }

                return OnDecisionResult.Success;
            }
            else
            {
                //画面を閉じる
                return OnDecisionResult.Success;
            }

        }



    }
}
