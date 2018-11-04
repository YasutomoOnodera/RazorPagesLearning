using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RazorPagesLearning.Report;
using RazorPagesLearning.Service.DB;
using RazorPagesLearning.Utility.TempDataHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RazorPagesLearning.Pages
{
    /// <summary>
    /// 運転日報、配表ダウンロード用のページ処理
    /// </summary>
    [Authorize(Roles = "Admin,ShippingCompany,Worker", Policy = "PasswordExpiration")]
    public class DeliveryDownloadReportFilesModel : DeliveryHelpers.DeliveryModelBase
    {
        /// <summary>
        /// エラー情報
        /// </summary>
        [BindProperty]
        public string errorMessage { get; set; }


        public DeliveryDownloadReportFilesModel(RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            SignInManager<IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager,
            ILogger<LoginModel> ref_logger)
        {
            //引数を取り込み
            this._db = ref_db;
            this._signInManager = ref_signInManager;
            this._userManager = ref_userManager;
            this._logger = ref_logger;
        }

        public void OnGet()
        {
            //対象サービス
            var serviceSet = makeServiceSet();

            //絞り込み条件を取得
            var conditions = TempData.Get<DeliveryHelpers.ViewModelBase.FilteringConditions>("exportFileInfo");

            ////対象情報を生成
            //var cfg = makeDeliveryReportConfig(serviceSet,
            //    conditions.DELIVERY_REQUEST_NUMBER,
            //    conditions.TRANSPORT_ADMIN_CODE);
            //if (true == cfg.isSucceed)
            //{
            //    prepareDownloadExportFile(cfg.deliveryReportConfig, ExportFileOutputType.Both);
            //}
            //else
            //{
            //    this.errorMessage = cfg.errorMessage;
            //}
        }

    }
}