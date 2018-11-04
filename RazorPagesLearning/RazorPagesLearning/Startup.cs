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
        /// AutoMapper������������
        /// </summary>
        private void setUpAutoMapper()
        {
            //Main�v���W�F�N�g�̃}�b�p�[������������
            Mapper.Initialize(cfg =>
            {
                //�T�[�r�X�v���W�F�N�g�̃}�b�p�[������������
                Service.Startup.startUP(cfg);

                //���v���W�F�N�g�ɂ�����}�b�p�[�̐ݒ�
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

            ///form�ɂ�����post�̍ő吔
            ///https://www.billbogaiv.com/posts/submitting-large-number-of-form-values-in-aspnet-core-10
            services.Configure<FormOptions>(x => x.ValueCountLimit = 65536);

            //SQL�T�[�o�[�ւ̐ڑ���������
            services.AddDbContext<RazorPagesLearningContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            ///�F�؋@�\��������
            {
                //���[�U�[�F�؋@�\�̏�Ԃ�ݒ肷��
                services.Configure<IdentityOptions>(options =>
                {
                    /*
                     �� �p�X���[�h�̎g�p�\����
                     �E�p�������Ƃ���B
                     �E�p���̑啶���������̋�ʂ͂��Ȃ��B
                     �E�L���͎g�p�s�B
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

                //�F�؋@�\��������
                services.AddIdentity<IdentityUser, IdentityRole>()
                           .AddEntityFrameworkStores<RazorPagesLearningContext>();


                //�p�X���[�h�̗L�������|���V�[��ǉ�
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("PasswordExpiration", policyBuilder =>
                    {
                        policyBuilder.Requirements.Add(new PasswordExpirationRequirement());
                    });
                });

                //DB�A�N�Z�X�T�[�r�X���C���W�F�N�V��������̂ŃI�u�W�F�N�g�̃��C�t�^�C����
                //1���N�G�X�g���ɐ����Ƃ���
                //(Singleton���ƃC���W�F�N�V�����ł��Ȃ�)
                services.AddScoped<IAuthorizationHandler, PasswordExpirationHandler>();


                //�����F�Ń��O�C�������ꍇ�̑J�ڐ�p�X
                services.ConfigureApplicationCookie(options =>
                {
                    options.AccessDeniedPath = "/AccessDenied";
                    options.LoginPath = "/Index";
                    options.LogoutPath = "/Index";
                });

            }

            //�Z�b�V�����p�̃L���b�V���v���o�C�_��L����
            services.AddDistributedMemoryCache();

            // �T�[�r�X�ɃZ�b�V������ǉ�
            services.AddSession(options => {
                // �Z�b�V�����N�b�L�[�̖��O��ς���Ȃ�
                options.Cookie.Name = "mgl-session";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                //TempData�̃f�[�^�ۑ��̈���Z�b�V�����Ɉڂ�
                .AddSessionStateTempDataProvider();

            //AutoMapper������������
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

            //2.1�ł�use mvc�̌��łȂ��Ɠ����Ȃ�
            app.UseCookiePolicy();
        }
    }
}
