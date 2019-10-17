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
        /// 初始化 Console 应用。
        /// 项目下必须包含 appsettings.json 和 nlog.config 文件。
        /// </summary>
        /// <param name="setup"></param>
        public static void Init(Action<IConfiguration, ServiceCollection> setup)
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
            var loggingBuilder = ServiceProvider.GetRequiredService<ILoggingBuilder>();
            loggingBuilder.AddConsole(); // LogLevel.Debug
            loggingBuilder.AddNLog();
            //NLog.LogManager.LoadConfiguration("nlog.config");
        }
    }
}
