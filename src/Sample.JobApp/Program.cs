using Anet.Data;
using Anet.Job;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Sample.JobApp
{
    public class Program
    {
        public static SettingsModel Settings { get; set; }

        static void Main(string[] args)
        {
            Sql.Ands(null);

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

            using (var scope = App.ServiceProvider.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("程序已启动。");

                // 1. 简单任务调度示例
                Scheduler.StartNew<MessageJob>(Settings.JobIntervalSeconds);
                logger.LogInformation("已启动消息发送任务处理程序。");

                // todo: 其它例子
            }

            // 等待程序关闭
            Scheduler.WaitForShutdown();
        }
    }
}
