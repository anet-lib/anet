using Anet.Job;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Sample.Job
{
    public class Program
    {
        public static SettingsModel Settings { get; set; }

        static void Main(string[] args)
        {
            Console.Title = "Aet 示例 - 简单任务调度程序";
            System.Net.ServicePointManager.DefaultConnectionLimit = 10;

            // 初始化应用
            App.Init((config, services) =>
            {
                // 绑定配置
                Settings = new SettingsModel();
                config.Bind(Settings);

                // 注册服务
                services.AddTransient<MessageJob>();
            });

            var logger = App.ServiceProvider.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("程序已启动。");

            // 启动定时轮循任务
            Scheduler.StartNew<MessageJob>(Settings.JobIntervalSeconds);

            logger.LogInformation("已启动消息发送任务处理程序。");

            // 等待程序关闭
            Scheduler.WaitForShutdown();
        }
    }
}
