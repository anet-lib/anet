using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Anet.Job
{
    public class Scheduler
    {
        public static bool _enabled = true;
        public static bool _issStopping = false;
        private static Schedule _nextToRun = null;
        private static readonly Timer _timer = new Timer(s => OnTimerCallback(), null, -1, -1);
        private static readonly List<Schedule> _scheduleList = new List<Schedule>();
        private static readonly HashSet<(Schedule, Task)> _running = new HashSet<(Schedule, Task)>();

        private static void UpdateTimer()
        {
            if (!_enabled) return;

            var nextToRun = _scheduleList.OrderBy(s => s.NextRunTime).FirstOrDefault();
            if(nextToRun == null)
            {
                _timer.Change(-1, -1);
                return;
            }
            
            _nextToRun = nextToRun;

            var interval = nextToRun.NextRunTime - DateTime.Now;
            if (interval <= TimeSpan.Zero)
            {
                OnTimerCallback();
            }
            else
            {
                _timer.Change(interval, interval);
            }
        }

        private static void RunJob(Schedule schedule)
        {
            if (schedule.Interval <= TimeSpan.Zero)
            {
                _scheduleList.Remove(schedule);
            }
            else
            {
                schedule.NextRunTime = schedule.NextRunTime.Add(schedule.Interval);
            }

            lock (_running)
            {
                if (_running.Any(t => ReferenceEquals(t.Item1, schedule)))
                    return;
            }

            (Schedule, Task) tuple = (null, null);

            var task = new Task(() => 
            {
                var scope = App.ServiceProvider.CreateScope();
                var job = scope.ServiceProvider.GetRequiredService(schedule.JobType) as IJob;
                try
                {
                    job.ExecuteAsync().Wait();
                }
                catch (Exception ex)
                {
                    try
                    {
                        job.OnExceptionAsync(GetInnerException(ex)).Wait();
                    }
                    catch (Exception innerEx)
                    {
                        var logger = scope.ServiceProvider.GetService<ILogger<Scheduler>>();
                        logger?.LogError(innerEx, "执行任务异常处理发生错误。");
                    }
                }
                finally
                {
                    lock (_running)
                    {
                        _running.Remove(tuple);
                    }
                    scope.Dispose();
                }
            }, TaskCreationOptions.PreferFairness);

            tuple = (schedule, task);

            lock(_running)
            {
                _running.Add(tuple);
            }

            task.Start();
        }

        private static Exception GetInnerException(Exception ex)
        {
            if (ex.InnerException == null)
                return ex;
            return GetInnerException(ex.InnerException);
        }

        private static void OnTimerCallback()
        {
            RunJob(_nextToRun);
            UpdateTimer();
        }

        /// <summary>
        /// 开启一个新的任务调度
        /// </summary>
        /// <typeparam name="T">用于调度的 <see cref="IJob"/> 服务</typeparam>
        /// <param name="intervalSeconds">任务间隔秒数；当值为 0 时, 任务只执行一次</param>
        /// <param name="immediate">是否立即执行</param>
        public static void StartNew<T>(uint intervalSeconds, bool immediate = true) where T : IJob
        {
            StartNew<T>(TimeSpan.FromSeconds(intervalSeconds), immediate);
        }

        /// <summary>
        /// 开启一个新的任务调度
        /// </summary>
        /// <typeparam name="T">用于调度的 <see cref="IJob"/> 服务</typeparam>
        /// <param name="interval">任务间隔；当值为 <see cref="TimeSpan.Zero"/> 时, 任务只执行一次</param>
        /// <param name="immediate">是否立即执行</param>
        public static void StartNew<T>(TimeSpan interval, bool immediate = true) where T : IJob
        {
            var firstRunTime = immediate ? DateTime.Now : DateTime.Now.Add(interval);
            StartNewAt<T>(firstRunTime, interval);
        }

        /// <summary>
        /// 开启一个新的任务调度
        /// </summary>
        /// <typeparam name="T">用于调度的 <see cref="IJob"/> 服务</typeparam>
        /// <param name="startTime">任务开始时间（小于或等于当前时间会立即执行）</param>
        /// <param name="interval">任务间隔，当值为 <see cref="TimeSpan.Zero"/> 时, 任务只执行一次</param>
        public static void StartNewAt<T>(DateTime startTime, TimeSpan interval) where T : IJob
        {
            var schedule = new Schedule()
            {
                JobType = typeof(T),
                Interval = interval,
                NextRunTime = startTime
            };
            _scheduleList.Add(schedule);
            UpdateTimer();
        }

        /// <summary>
        /// 等待程序关闭
        /// </summary>
        public static void WaitForShutdown()
        {
            AppDomain.CurrentDomain.ProcessExit += async (s, e) => await StopAll();
            Console.CancelKeyPress += async (s, e) => await StopAll();
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// 停止所有任务
        /// </summary>
        public async static Task StopAll()
        {
            if (_issStopping) return;
            _issStopping = true;

            _enabled = false;
            _timer.Change(-1, -1);

            await Task.Run(() =>
            {
                var tasks = new Task[0];
                do
                {
                    lock (_running)
                    {
                        tasks = _running.Select(t => t.Item2).ToArray();
                    }
                    Task.WaitAll(tasks);
                } while (tasks.Any());
                _issStopping = false;
            });
        }
    }
}
