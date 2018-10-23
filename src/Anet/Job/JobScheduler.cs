using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anet.Job
{
    public class JobScheduler
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
        /// <param name="intervel">任务间隔</param>
        public static Task StartNew<T>(TimeSpan intervel) where T : IJob
        {
            return Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (IsStopping) break;
                    IncreTaskCount();

                    var scope = AnetGlobal.ServiceProvider.CreateScope();
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
                            var logger = scope.ServiceProvider.GetService<ILogger<JobScheduler>>();
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
            // 程序结束事件
            var done = new ManualResetEventSlim(false);
            // 附加 Ctrl+C 终止程序
            AttachCtrlcSigtermShutdown(done);
            // 程序保活
            while (!IsStopping || _runningTaskCount > 0)
            {
                Thread.Sleep(0);
            }
            // 程序正常结束
            done.Set();
        }

        private static void AttachCtrlcSigtermShutdown(ManualResetEventSlim doneEvent)
        {
            void Shutdown()
            {
                Console.WriteLine("程序正在关闭，请稍后...");
                IsStopping = true;
                // 等待程序执行结束
                doneEvent.Wait();
            };

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => Shutdown();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Shutdown();
                // 不要立即关闭, 等待主线程退出
                eventArgs.Cancel = true;
            };
        }
    }
}
