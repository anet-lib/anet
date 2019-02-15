using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anet.Job
{
    public class Scheduler
    {
        public static bool IsStopping { get; private set; }

        private static volatile int _runningTaskCount;

        public static void IncreTaskCount()
        {
            Interlocked.Increment(ref _runningTaskCount);
        }

        public static void DecreTaskCount()
        {
            Interlocked.Decrement(ref _runningTaskCount);
        }

        /// <summary>
        /// 开启新的任务调度
        /// </summary>
        /// <param name="seconds">任务间隔秒数</param>
        public static Task StartNew<T>(int seconds) where T : IJob
        {
            return StartNew<T>(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// 开启新的任务调度
        /// </summary>
        /// <param name="intervel">任务间隔</param>
        public static Task StartNew<T>(TimeSpan intervel) where T : IJob
        {
            return Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (IsStopping) break;
                    IncreTaskCount();

                    var scope = App.ServiceProvider.CreateScope();
                    var job = scope.ServiceProvider.GetRequiredService<T>();
                    try
                    {
                        job.ExecuteAsync().GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            job.OnExceptionAsync(ex);
                        }
                        catch (Exception innerEx)
                        {
                            var logger = scope.ServiceProvider.GetService<ILogger<Scheduler>>();
                            logger?.LogError(innerEx, "任务内部处理执行异常。");
                        }
                    }
                    finally
                    {
                        DecreTaskCount();
                        scope.Dispose();
                    }
                    Thread.Sleep(intervel);
                }
            }, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// 等待程序关闭
        /// </summary>
        public static void WaitForShutdown()
        {
            void Shutdown()
            {
                Console.WriteLine("程序正在关闭，请稍后...");
                StopAll();
            }

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Shutdown();
            Console.CancelKeyPress += (s, e) => Shutdown();
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// 停止所有任务
        /// </summary>
        private static void StopAll()
        {
            if (IsStopping) return;
            IsStopping = true;
            while (_runningTaskCount > 0) ;
        }
    }
}
