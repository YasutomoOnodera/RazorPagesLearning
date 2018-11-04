using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RazorPagesLearning.Pages
{
    public class LogOutModel : PageModel
    {

        //コンストラクタ引数
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LogOutModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            //DI情報の取り込み
            this._signInManager = signInManager;
            this._logger = logger;
        }


        public async Task<IActionResult> OnGetAsync()
        {
            await this._signInManager.SignOutAsync();
            return RedirectToPage("./Index");
        }
    }
}