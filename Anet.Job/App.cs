using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace Anet.Job
{
    public class App
    {
        public static IServiceProvider ServiceProvider { get; internal set; }
        public static IConfiguration Configuration { get; internal set; }

        /// <summary>
        /// 初始化 Console 应用程序
        /// </summary>
        /// <param name="setup">应用配置</param>
        /// <param name="args">Main 入口函数参数</param>
        public static void Init(Action<IConfiguration, ServiceCollection> setup, string[] args = null)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args ?? new string[] { })
                .Build();

            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddNLog(Configuration);
            });

            setup(Configuration, services);

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
