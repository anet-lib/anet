using Anet.Job;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Job
{
    public class MessageJob : IJob
    {
        private readonly ILogger<MessageJob> _logger;
        public MessageJob(ILogger<MessageJob> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync()
        {
            return Task.Run(() =>
            {
                // 模拟发送消息
                _logger.LogInformation("正在发送消息...");
                Thread.Sleep(3000);
                _logger.LogInformation("消息发送成功。");
            });
        }

        public Task OnExceptionAsync(Exception ex)
        {
            _logger.LogError(ex,"发送消息出错。");
            return Task.FromResult(0);
        }
    }
}
