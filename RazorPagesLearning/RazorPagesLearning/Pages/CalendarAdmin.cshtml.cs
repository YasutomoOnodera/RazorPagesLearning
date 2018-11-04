using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RazorPagesLearning.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using static RazorPagesLearning.Pages.MessageAdminModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using RazorPagesLearning.Service.DB;

namespace RazorPagesLearning.Pages
{
    public class CalendarAdminModel : PageModel
    {
        /// <summary>
        /// 画面上の表示項目と同期する情報
        /// </summary>
        [BindProperty]
        public ViewModel viewModel { get; set; }

        /// <summary>
        /// DBアクセスオブジェクト
        /// </summary>
        public readonly RazorPagesLearning.Data.RazorPagesLearningContext _db;

        // コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        private readonly Service.DB.CalendarAdminService _service;

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="db"></param>
        /// <param name="signInManager"></param>
        /// <param name="userManager"></param>
        /// <param name="logger"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public CalendarAdminModel(RazorPagesLearning.Data.RazorPagesLearningContext db, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<LoginModel> logger)
        {
            this._db = db;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._logger = logger;

            this._service = new Service.DB.CalendarAdminService(_db, User, _signInManager, _userManager);

            // viewモデルの更新
            this.viewModel = new ViewModel();

        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 画面上に表示すべき情報
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public class ViewModel
        {
            ////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// コンストラクタ
            /// </summary>
            ////////////////////////////////////////////////////////////////////////////////////////
            public ViewModel()
            {
                updateModel = new UpdateModel();
            }

            #region 表示対象データ

            public List<CALENDAR_ADMIN> CALENDAR_ADMINs { get; set; }

            #endregion // 表示対象データ

            #region 更新対象データ

            public UpdateModel updateModel { get; set; }

            #endregion // 更新対象データ

            /// <summary>
            /// 月初の曜日
            /// </summary>
            public List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();

            /// <summary>
            /// 当月の日数
            /// </summary>
            public List<int> numberOfDays = new List<int>();

            /// <summary>
            /// 描画対象の月
            /// </summary>
            public List<int> month = new List<int>();

            /// <summary>
            /// 西暦
            /// </summary>
            public List<string> year = new List<string>();
           
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 更新対象データ
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public class UpdateModel
        {
            ////////////////////////////////////////////////////////////////////////////////////////
            /// <summary>
            /// コンストラクタ
            /// </summary>
            ////////////////////////////////////////////////////////////////////////////////////////
            public UpdateModel()
            {
                checkBoxes = new Dictionary<string, bool>();
            }

            /// <summary>
            ///休日
            /// </summary>
            [Key]
            [Required]
            [DisplayName("休日")]
            public List<DateTimeOffset> HOLIDAYs { get; set; }

            /// <summary>
            /// チェックボックス
            /// </summary>
            public Dictionary<string, bool> checkBoxes { get; set; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Indexアクション
        /// メッセージマスタ画面の表示
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> OnGetAsync()
        {
            // 休日リストを取得
            await ReadViewDataFromDB();

            // 現在の年月日を取得
            DateTime Now = DateTime.Now;

            // 月初を取得
            DateTime beginOfMonth = Now.AddDays(-Now.Day + 1);
                         
            // セレクトボックスに表示する期
            mkSelectCaption(beginOfMonth);

            // 3期分(18ヶ月)を取得
            monthList(beginOfMonth);

            return Page();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 表示情報を初期化する
        /// </summary>
        /// <param name="id"></param>
        ////////////////////////////////////////////////////////////////////////////////////////////
        private async Task<bool> ReadViewDataFromDB()
        {
            // 現在ログイン中のユーザーを取得する
            var ui = await this._userManager.GetUserAsync(User);
            if (null != ui)
            {
                viewModel.CALENDAR_ADMINs = _service.ReadAll();
            }
            else
            {
                //TODO: エラーメッセージを出力するように修正する
            }
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Saveアクション
        /// 休日を更新
        /// </summary>
        /// <returns></returns>
        ////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> OnPostSaveAsync()
        {
            this.viewModel.updateModel.HOLIDAYs = new List<DateTimeOffset>();

            foreach (string Key in viewModel.updateModel.checkBoxes.Keys)
            {
                string f;
                DateTime dt;

                f = "yyyy/MM/dd";
                dt = DateTime.ParseExact(Key, f, null);

                viewModel.updateModel.HOLIDAYs.Add(dt);
            }

            // =====================================================================================
            // 休日の更新
            // =====================================================================================
            // サービスに渡す値をまとめる
            var holiday = new RazorPagesLearning.Service.DB.CalendarAdminService.Holiday
            {
                HOLIDAYs = this.viewModel.updateModel.HOLIDAYs
            };

            // 休日を更新する
            await _service.UpdateAsync(holiday);

            // =====================================================================================
            // 更新後の値を再検索
            // =====================================================================================
            await ReadViewDataFromDB();

            this.viewModel.updateModel = new UpdateModel();

            DateTime Now = DateTime.Now;

            //// 月初を取得
            DateTime beginOfMonth = Now.AddDays(-Now.Day + 1);

            //// セレクトボックスに表示する期
            mkSelectCaption(beginOfMonth);

            //// 3期分(18ヶ月)を取得
            monthList(beginOfMonth);

            //// 休日リストを取得
            await ReadViewDataFromDB();

            return Page();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 18ヶ月分の月、月初曜日、日数を取得
        /// </summary>
        /// <param name="beginOfMonth">月初となる日時</param>
        ////////////////////////////////////////////////////////////////////////////////////////////
        private void monthList(DateTime beginOfMonth)
        {
            // 起点となる月を判定
            if (4 <= beginOfMonth.Month && beginOfMonth.Month <= 9)
            {
                beginOfMonth = beginOfMonth.AddMonths(- beginOfMonth.Month + 4);
            }
            else if (10 <= beginOfMonth.Month && beginOfMonth.Month <= 12)
            {
                beginOfMonth = beginOfMonth.AddMonths(- beginOfMonth.Month + 10);
            }
            else
            {
                beginOfMonth = beginOfMonth.AddYears(-1).AddMonths(- beginOfMonth.Month + 10);
            }

            // 3期分(18ヶ月)を取得
            const int PRIOD = 18;
            for (int i = 0; i < PRIOD; i++)
            {
                // 月を取得
                int month = beginOfMonth.Month;

                // その月の日数を取得
                int endOfMonth = beginOfMonth.AddMonths(1).AddDays(-1).Day;

                // 曜日を取得
                DayOfWeek dayOfWeek = beginOfMonth.DayOfWeek;

                // リストに各月を格納
                this.viewModel.month.Add(month);

                // リストに各月初曜日を格納
                this.viewModel.dayOfWeeks.Add(dayOfWeek);

                // リストに各月の日数を格納
                this.viewModel.numberOfDays.Add(endOfMonth);

                // 当月の日数を集める
                for (int j = 0; j < endOfMonth; j++)
                {
                    // 年月日をintに代入
                    int y = beginOfMonth.Year;
                    int m = beginOfMonth.Month;
                    int d = j + 1;

                    bool isHoliday = viewModel.CALENDAR_ADMINs.Exists((e) =>
                    {
                        return (e.HOLIDAY.Year == y && e.HOLIDAY.Month == m && e.HOLIDAY.Day == d);
                    });

                    // 年月日をstring型で連結する
                    string ymd = string.Format("{0:D4}/{1:D2}/{2:D2}", y, m, d);

                    // checkBoxesに年月日を格納する
                    this.viewModel.updateModel.checkBoxes.Add(ymd, isHoliday);
                }

                // 1ヶ月進める
                beginOfMonth = beginOfMonth.AddMonths(1);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// セレクトボックスに表示する期
        /// </summary>
        /// <param name="beginOfMonth">月初となる日時</param>
        ////////////////////////////////////////////////////////////////////////////////////////////
        private void mkSelectCaption(DateTime beginOfMonth)
        {
            // 1月から3月の場合は前年を起点にする
            if (1 <= beginOfMonth.Month && beginOfMonth.Month <= 3)
            {
                beginOfMonth = beginOfMonth.AddYears(-1);
            }

            for (int i = 0; i < 3; i++)
            {
                // 4月から9月の場合
                if (4 <= beginOfMonth.Month && beginOfMonth.Month <= 9)
                {
                    string year = beginOfMonth.ToString("yyyy");

                    if (i == 0)
                    {
                        year = year + "上期";
                    }
                    else if (i == 1)
                    {
                        year = year + "下期";
                    }
                    else
                    {
                        year = beginOfMonth.AddYears(1).ToString("yyyy");
                        year = year + "上期";
                    }

                    this.viewModel.year.Add(year);
                }
                else
                {
                    string year = beginOfMonth.ToString("yyyy");

                    if (i == 0)
                    {
                        year = year + "下期";
                    }
                    else if (i == 1)
                    {
                        year = beginOfMonth.AddYears(1).ToString("yyyy");
                        year = year + "上期";
                    }
                    else
                    {
                        year = beginOfMonth.AddYears(1).ToString("yyyy");
                        year = year + "下期";
                    }

                    this.viewModel.year.Add(year);
                }
            }
        }
    }   
}