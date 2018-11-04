﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
using RazorPagesLearning.Service.FormattedView;
using RazorPagesLearning.Utility.Pagination;
using RazorPagesLearning.Utility.SelectableTable;
using RazorPagesLearning.Utility.SelectItem;
using RazorPagesLearning.Utility.SortableTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using static RazorPagesLearning.Data.Models.USER_ACCOUNTExt;

namespace RazorPagesLearning.Pages
{
    [Authorize(Roles = "Admin", Policy = "PasswordExpiration")]
    public class AccountUserModel : PageModel
    {

        #region ページネーション
        /// <summary>
        /// ページネーション情報を初期化する
        /// </summary>
        /// <returns></returns>
        private PaginationInfo initPaginationInfo()
        {
            //ToDo : セレクト結果で表示する


            return PaginationInfo.createInstance(1);
            //return PaginationInfo.createInstance(
            //            this.userService.getStatistics().numbers);
        }

        /// <summary>
        /// ページネーション情報
        /// </summary>
        [BindProperty]
        public PaginationInfo paginationInfo { get; set; }

        #endregion

        #region 検索条件

        /// <summary>
        /// 検索条件定義
        /// </summary>
        public class SearchConditions
        {
            /// <summary>
            /// DBに対する検索条件
            /// </summary>
            public RazorPagesLearning.Service.User.UserService.ReadConfig queryConditions { get; set; }

            #region 権限関係

            /// <summary>
            /// アカウント権限を選択する
            /// </summary>
            public SelectItemSet_AccountPermissions AccountPermissions { get; set; }

            #endregion

            public SearchConditions()
            {
                this.AccountPermissions = new SelectItemSet_AccountPermissions();
                this.queryConditions = new Service.User.UserService.ReadConfig();
            }

            /// <summary>
            /// 選択要素から権限情報を取り込む
            /// </summary>
            public void reflectPermissions()
            {
                this.queryConditions.PERMISSION = this.AccountPermissions.selected.toACCOUNT_PERMISSION();
            }
        }

        /// <summary>
        /// 検索条件
        /// </summary>
        [BindProperty]
        public SearchConditions searchConditions { get; set; }

        #endregion

        #region 表示項目


        /// <summary>
        /// 画面に表示するユーザーアカウント情報
        /// </summary>
        public class ViewUSER_ACCOUNT
        {
            /// <summary>
            /// 表示するデータ
            /// </summary>
            public Formatted_USER_ACCOUNT data { get; set; }

        }

        /// <summary>
        /// 画面上に表示する項目
        /// </summary>
        public class ViewModel : SortableTableViewModelBase<ViewUSER_ACCOUNT>
        {
            public ViewModel() 
            {
            }
        }

        /// <summary>
        /// テーブル上の行情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        #endregion

        /// <summary>
        /// 指定された検索条件に従ってデータを読み取る
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Formatted_USER_ACCOUNT> doSearch()
        {
            //パスワードの有効期限を取得する
            var userService = new Service.User.UserService(db, User, signInManager, userManager);

            //セキュリティポリシー情報を取得する
            var policyService = new Service.DB.PolicyService(db, User, signInManager, userManager);

            //select要素をデータに取り込む
            this.searchConditions.reflectPermissions();

            //表示データを更新
            var r = userService.readWithPendingInfo(this.searchConditions.queryConditions);
            if (true == r.succeed)
            {
                return
                    r.result
                    .Select(e => Formatted_USER_ACCOUNT.convert(e, userService, policyService))
                    .Sort(this.viewModel.sortDirection, this.viewModel.sortColumn);
            }

            //失敗したら要素なしで返す
            return new List<Formatted_USER_ACCOUNT>();
        }

        /// <summary>
        /// 画面上のデータを更新する
        /// </summary>
        private void UpdateDate()
        {
            var searchResults = doSearch();
            var rawList = searchResults.ToList();
            this.paginationInfo.maxItems = rawList.Count();
            this.paginationInfo.movePage(this.paginationInfo.displayNextPage);

            this.viewModel.tableRows = rawList
                .Select(e => new SortableTableRowInfo<ViewUSER_ACCOUNT>
                {
                    data = new ViewUSER_ACCOUNT
                    {
                        data = e,
                    }
                })
                .Skip(this.paginationInfo.startViewItemIndex)
                .Take(this.paginationInfo.endViewItemIndex)
                .ToList();
        }

        private readonly RazorPagesLearning.Data.RazorPagesLearningContext db;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;


        //★
        private Service.DB.StockService stockService;
        //private Service.DB.WmsRequestHistoryDetailService wmsRequestHistoryAdminService;
        private Service.DB.RequestHistoryService requestHistoryService;
        private Service.DB.RequestHistoryDetailService requestHistoryDetailService;
        //★

        public AccountUserModel(RazorPagesLearning.Data.RazorPagesLearningContext ref_db,
            SignInManager<IdentityUser> ref_signInManager,
            UserManager<IdentityUser> ref_userManager,
            ILogger<LoginModel> ref_logger)
        {
            this.db = ref_db;
            this.signInManager = ref_signInManager;
            this.userManager = ref_userManager;


            this.paginationInfo = initPaginationInfo();
            this.searchConditions = new SearchConditions();

            this.viewModel = new ViewModel();
        }

        public void OnPost(string page)
        {
            //画面更新
            this.UpdateDate();
        }

        public void OnPostPdf(string page)
        {
            // 15.在庫詳細照会のレポート出力 >>
            this.stockService = new Service.DB.StockService(this.db, User, this.signInManager, this.userManager);
            this.requestHistoryDetailService = new Service.DB.RequestHistoryDetailService(this.db, User, this.signInManager, this.userManager);
            this.requestHistoryService = new Service.DB.RequestHistoryService(this.db, User, this.signInManager, this.userManager);
            // 仮のMWL在庫ID
            //long id = 1;
     
//            //レポートをエクスポートする
//            {
//                var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
//                    DateTimeOffset.Now, "yyyyMMddHHmmss");
//#if false // 20180925_DBModel修正 (STOCKから取れるようにしたため、本関数は不要)
//				RazorPagesLearning.Data.Models.REQUEST_HISTORY_DETAIL targetHistoryDetail = this.requestHistoryDetailService.readByMwlStockIdOrderByCreatedAt(id).result;
//				RazorPagesLearning.Data.Models.REQUEST_HISTORY targetHistory = this.requestHistoryService.readById(targetHistoryDetail.REQUEST_HISTORY_ID).result;
//#endif // 20180925_DBModel修正 (STOCKから取れるようにしたため、本関数は不要)

//                Utility.ReportHelper.HelperFunctions.writePdf
//                   (new Utility.ReportHelper.HelperFunctions.WriteConfig
//                   {
//                       fileName = $"Stock_{nowStr}.pdf",
//                       report = new RazorPagesLearning.Report.StockReport(
//                           new Report.StockReport.ReportConfig
//                           {
//                               targetStock = this.stockService.readByStockId(id).result,
//#if false // 20180925_DBModel修正 (STOCKから取れるようにしたため、不要)
//							   targetHistoryDetail = targetHistoryDetail,
//							   targetHistory = targetHistory
//#endif // 20180925_DBModel修正 (STOCKから取れるようにしたため、不要)
//                           }),
//                       target = this.Response
//                   });
//            }
            // << 15.在庫詳細照会のレポート出力
        }

        ///// <summary>
        ///// Excel形式でエクスポートする
        ///// </summary>
        ///// <param name="page"></param>
        //public void OnPostExcel(string page)
        //{
        //    //レポートをエクスポートする
        //    {
        //        var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
        //            DateTimeOffset.Now, "yyyyMMddHHmmss");

        //        Utility.ReportHelper.HelperFunctions.writeExcel
        //           (new Utility.ReportHelper.HelperFunctions.WriteConfig
        //            {
        //                fileName = $"AccountUser_{nowStr}.xlsx",
        //                report = new RazorPagesLearning.Report.AccountUserReport(doSearch()),
        //                target = this.Response
        //            });
        //    }
        //}

        /// <summary>
        /// 15.在庫詳細照会のレポート用
        /// Excel形式でエクスポートする
        /// </summary>
        /// <param name="page"></param>
        public void OnPostExcel3(string page)
        {
            this.stockService = new Service.DB.StockService(this.db, User, this.signInManager, this.userManager);
            this.requestHistoryDetailService = new Service.DB.RequestHistoryDetailService(this.db, User, this.signInManager, this.userManager);
            this.requestHistoryService = new Service.DB.RequestHistoryService(this.db, User, this.signInManager, this.userManager);
            // 仮のMWL在庫ID
            //long id = 1;

//            //レポートをエクスポートする
//            {
//                var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
//                    DateTimeOffset.Now, "yyyyMMddHHmmss");
//#if false // 20180925_DBModel修正 (STOCKから取れるようにしたため、本関数は不要)
//				RazorPagesLearning.Data.Models.REQUEST_HISTORY_DETAIL targetHistoryDetail = this.requestHistoryDetailService.readByMwlStockIdOrderByCreatedAt(id).result;
//				RazorPagesLearning.Data.Models.REQUEST_HISTORY targetHistory = this.requestHistoryService.readById(targetHistoryDetail.REQUEST_HISTORY_ID).result;
//#endif // 20180925_DBModel修正 (STOCKから取れるようにしたため、本関数は不要)


//                Utility.ReportHelper.HelperFunctions.writeExcel
//                   (new Utility.ReportHelper.HelperFunctions.WriteConfig
//                   {
//                       fileName = $"Stock_{nowStr}.xlsx",
//                       report = new RazorPagesLearning.Report.StockReportExcel(
//                           new Report.StockReportExcel.ReportConfigExcel
//                           {
//                               targetStock = this.stockService.readByStockId(id).result,
//#if false // 20180925_DBModel修正 (STOCKから取れるようにしたため、不要)
//							   targetHistoryDetail = targetHistoryDetail,
//							   targetHistory = targetHistory
//#endif // 20180925_DBModel修正 (STOCKから取れるようにしたため、不要)
//                           }),
//                       target = this.Response
//                   });
//            }
        }


        /// <summary>
        /// 15.在庫詳細照会のレポート用
        /// Excel形式でエクスポートする
        /// </summary>
        /// <param name="page"></param>
        public void OnPostCsv3(string page)
        {
            this.stockService = new Service.DB.StockService(this.db, User, this.signInManager, this.userManager);
            this.requestHistoryDetailService = new Service.DB.RequestHistoryDetailService(this.db, User, this.signInManager, this.userManager);
            this.requestHistoryService = new Service.DB.RequestHistoryService(this.db, User, this.signInManager, this.userManager);
            // 仮のMWL在庫ID
            //long id = 1;

//            //レポートをエクスポートする
//            {
//                var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
//                    DateTimeOffset.Now, "yyyyMMddHHmmss");
//#if false // 20180925_DBModel修正 (STOCKから取れるようにしたため、本関数は不要)
//				RazorPagesLearning.Data.Models.REQUEST_HISTORY_DETAIL targetHistoryDetail = this.requestHistoryDetailService.readByMwlStockIdOrderByCreatedAt(id).result;
//				RazorPagesLearning.Data.Models.REQUEST_HISTORY targetHistory = this.requestHistoryService.readById(targetHistoryDetail.REQUEST_HISTORY_ID).result;
//#endif // 20180925_DBModel修正 (STOCKから取れるようにしたため、本関数は不要)


//                Utility.ReportHelper.HelperFunctions.writeCsv
//                   (new Utility.ReportHelper.HelperFunctions.WriteConfig
//                   {
//                       fileName = $"Stock_{nowStr}.csv",
//                       report = new RazorPagesLearning.Report.StockReportExcel(
//                           new Report.StockReportExcel.ReportConfigExcel
//                           {
//                               targetStock = this.stockService.readByStockId(id).result,
//#if false // 20180925_DBModel修正 (STOCKから取れるようにしたため、不要)
//							   targetHistoryDetail = targetHistoryDetail,
//							   targetHistory = targetHistory
//#endif // 20180925_DBModel修正 (STOCKから取れるようにしたため、不要)
//                           }),
//                       target = this.Response
//                   });
//            }
        }


        ///// <summary>
        ///// CSV形式でエクスポートする
        ///// </summary>
        ///// <param name="page"></param>
        //public void OnPostCsv(string page)
        //{
        //    //レポートをエクスポートする
        //    {
        //        var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
        //            DateTimeOffset.Now, "yyyyMMddHHmmss");

        //        Utility.ReportHelper.HelperFunctions.writeCsv
        //            (new Utility.ReportHelper.HelperFunctions.WriteConfig
        //            {
        //                fileName = $"AccountUser_{nowStr}.csv",
        //                report = new RazorPagesLearning.Report.AccountUserReport(doSearch()),
        //                target = this.Response
        //            });
        //    }
        //}




  //      /// <summary>
  //      /// Excel形式でエクスポートする
  //      /// </summary>
  //      /// <param name="page"></param>
  //      public async Task<IActionResult> OnPostExcel2(string page)
  //      {
  //          //レポートをエクスポートする
  //          {
  //              var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
  //                  DateTimeOffset.Now, "yyyyMMddHHmmss");

  //              //★
  //              this.stockService = new Service.DB.StockService(this.db, User, this.signInManager, this.userManager);
  //              this.wmsRequestHistoryAdminService = new Service.DB.WmsRequestHistoryDetailService(this.db, User, this.signInManager, this.userManager);
  //              //★


  //              ////               Utility.ReportHelper.HelperFunctions.writeExcel
  //              ////★
  //              //Utility.ReportHelper.HelperFunctions.writePdf
  //              ////★
  //              //   (new Utility.ReportHelper.HelperFunctions.WriteConfig
  //              //   {
  //              //       //fileName = $"AccountUser_{nowStr}.xlsx",
  //              //       //                        report = new RazorPagesLearning.Report.AccountUserReport(doSearch()),
  //              //       //★
  //              //       //fileName = $"HistoryReport_{nowStr}.xlsx",
  //              //       //report = new RazorPagesLearning.Report.HistoryReportExcel(
  //              //       //    new RazorPagesLearning.Report.HistoryReportExcel.ReportConfigExcel
  //              //       //    {
  //              //       //        targetInformation = this.wmsRequestHistoryAdminService.read(1)
  //              //       //    }),
  //              //       fileName = $"HistoryReport_{nowStr}.pdf",
  //              //       report = new RazorPagesLearning.Report.HistoryReport(
  //              //            new RazorPagesLearning.Report.HistoryReport.ReportConfig
  //              //            {
  //              //                targetInformation = await this.wmsRequestHistoryAdminService.read(1)
  //              //            }),
  //              //       //★
  //              //       target = this.Response
  //              //   });
  //          }

		//	return RedirectToPagePermanentPreserveMethod("AccountUser");
		//}

  //      /// <summary>
  //      /// CSV形式でエクスポートする
  //      /// </summary>
  //      /// <param name="page"></param>
  //      public async Task<IActionResult> OnPostCsv2(string page)
  //      {
  //          //レポートをエクスポートする
  //          {
  //              var nowStr = RazorPagesLearning.Service.Utility.ViewHelper.HelperFunctions.toFormattedString(
  //                  DateTimeOffset.Now, "yyyyMMddHHmmss");


  //              //★
  //              this.stockService = new Service.DB.StockService(this.db, User, this.signInManager, this.userManager);
  //              this.wmsRequestHistoryAdminService = new Service.DB.WmsRequestHistoryDetailService(this.db, User, this.signInManager, this.userManager);
  //              //★


  //              Utility.ReportHelper.HelperFunctions.writeCsv
  //                  (new Utility.ReportHelper.HelperFunctions.WriteConfig
  //                  {
  //                      fileName = $"AccountUser_{nowStr}.csv",
  //                      //report = new RazorPagesLearning.Report.AccountUserReport(doSearch()),
  //                      //★
  //                      report = new RazorPagesLearning.Report.HistoryReportExcel(
  //                          new RazorPagesLearning.Report.HistoryReportExcel.ReportConfigExcel
  //                          {
  //                              targetInformation = await this.wmsRequestHistoryAdminService.read(1)
  //                          }),
  //                      //★
  //                      target = this.Response
  //                  });
  //          }

		//	return RedirectToPagePermanentPreserveMethod("AccountUser");
		//}




    }
}