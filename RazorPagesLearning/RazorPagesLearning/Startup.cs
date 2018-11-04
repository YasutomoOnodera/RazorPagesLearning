using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RazorPagesLearning.Authentication.Policy;
using RazorPagesLearning.Data;
using RazorPagesLearning.Data.Models;
using System;
using static RazorPagesLearning.Pages.RequestConfirmModel;

namespace RazorPagesLearning
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// AutoMapperを初期化する
        /// </summary>
        private void setUpAutoMapper()
        {
            //Mainプロジェクトのマッパーを初期化する
            Mapper.Initialize(cfg =>
            {
                //サービスプロジェクトのマッパーを初期化する
                Service.Startup.startUP(cfg);

                //現プロジェクトにおけるマッパーの設定
                cfg.CreateMap<DELIVERY_REQUEST, Pages.DeliveryModel.Formatted_DELIVERY_REQUEST>();
                cfg.CreateMap<DELIVERY_REQUEST_DETAIL, Service.DB.DeliveryDetailService.CollisionDetectable_DELIVERY_DETAIL>();
                cfg.CreateMap<Service.DB.DeliveryDetailService.CollisionDetectable_DELIVERY_DETAIL, Pages.DeliveryHelpers.TRUCK_Corrected_DELIVERY_DETAIL>();
                cfg.CreateMap<WK_REQUEST_DELIVERY, Formatted_WK_DELIVERY>();
                cfg.CreateMap<USER_ITEM, USER_ITEM>();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ///formにおけるpostの最大数
            ///https://www.billbogaiv.com/posts/submitting-large-number-of-form-values-in-aspnet-core-10
            services.Configure<FormOptions>(x => x.ValueCountLimit = 65536);

            //SQLサーバーへの接続を初期化
            services.AddDbContext<RazorPagesLearningContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            ///認証機構を初期化
            {
                //ユーザー認証機構の状態を設定する
                services.Configure<IdentityOptions>(options =>
                {
                    /*
                     ■ パスワードの使用可能文字
                     ・英数混合とする。
                     ・英字の大文字小文字の区別はしない。
                     ・記号は使用不可。
                     */

                    options.User.RequireUniqueEmail = false;

                    options.Password.RequiredLength = 2;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;

                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;


                });

                //認証機構を初期化
                services.AddIdentity<IdentityUser, IdentityRole>()
                           .AddEntityFrameworkStores<RazorPagesLearningContext>();


                //パスワードの有効期限ポリシーを追加
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("PasswordExpiration", policyBuilder =>
                    {
                        policyBuilder.Requirements.Add(new PasswordExpirationRequirement());
                    });
                });

                //DBアクセスサービスをインジェクションするのでオブジェクトのライフタイムは
                //1リクエスト毎に生成とする
                //(Singletonだとインジェクションできない)
                services.AddScoped<IAuthorizationHandler, PasswordExpirationHandler>();


                //未承認でログインした場合の遷移先パス
                services.ConfigureApplicationCookie(options =>
                {
                    options.AccessDeniedPath = "/AccessDenied";
                    options.LoginPath = "/Index";
                    options.LogoutPath = "/Index";
                });

            }

            //セッション用のキャッシュプロバイダを有効化
            services.AddDistributedMemoryCache();

            // サービスにセッションを追加
            services.AddSession(options => {
                // セッションクッキーの名前を変えるなら
                options.Cookie.Name = "mgl-session";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                //TempDataのデータ保存領域をセッションに移す
                .AddSessionStateTempDataProvider();

            //AutoMapperを初期化する
            setUpAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();

            //2.1ではuse mvcの後ろでないと動かない
            app.UseCookiePolicy();
        }
    }
}
