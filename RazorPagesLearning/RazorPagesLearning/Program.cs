using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace RazorPagesLearning
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Win上で実行した時の日本語化対応
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            //CreateWebHostBuilder(args).Build().Run();

            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<RazorPagesLearning.Data.RazorPagesLearningContext>();

                try
                {
                    //DB上に初期データを登録する
                    RazorPagesLearning.Data.SeedData.Initialize(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");

                    //どうにもならないので落とす。
                    throw ex;
                }
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            //ここで様々な初期設定を行う
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
