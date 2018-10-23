using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace Anet
{
    public class AnetGlobal
    {
        public static IServiceProvider ServiceProvider { get; internal set; }
        public static IConfiguration Configuration { get; internal set; }

        /// <summary>
        /// 初始化 ASP.NET CORE 应用
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void InitAspNetApp(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Configuration = serviceProvider.GetRequiredService<IConfiguration>();
        }

        /// <summary>
        /// 初始化 Console 应用
        /// </summary>
        /// <param name="setup"></param>
        public static void InitConsoleApp(Action<IConfiguration, ServiceCollection> setup)
        {
            // 1、Load Configurations
            // Console App 没有 ASPNETCORE_ENVIRONMENT 变量
            // var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Configuration = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                //.AddJsonFile($"appsettings.{envName}.json", optional: true)
                .Build();

            // 2、Build IoC Services
            var services = new ServiceCollection();
            services.AddSingleton(Configuration);
            services.AddLogging(b => b.AddConfiguration(Configuration.GetSection("Logging")));
            setup(Configuration, services);
            ServiceProvider = services.BuildServiceProvider();

            // 3、Config Logging
            var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
            var nlogOptions = new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true
            };
            loggerFactory.AddConsole(); // LogLevel.Debug
            loggerFactory.AddNLog(nlogOptions);
            NLog.LogManager.LoadConfiguration("nlog.config");
        }
    }
}
