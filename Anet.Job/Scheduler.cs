using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Anet.Job
{
    public class Scheduler
    {

        public static bool Enabled { get; private set; } = true;

        public static bool IsStopping { get; private set; }

        private static Timer _timer = new Timer(s => OnTimerCallback(), null, -1, -1);

        private static List<Schedule> _scheduleList = new List<Schedule>();

        private static Schedule _nextToRun = null;

        private static HashSet<(Schedule, Task)> _running = new HashSet<(Schedule, Task)>();

        private static void UpdateTimer()
        {
            if (!Enabled) return;

            var nextToRun = _scheduleList.OrderBy(s => s.NextRun).FirstOrDefault();
            if(nextToRun == null)
            {
                _timer.Change(-1, -1);
            }
            else
            {
                _nextToRun = nextToRun;
                var interval = nextToRun.NextRun - DateTime.Now;
                if(interval <= TimeSpan.Zero)
                {
                    OnTimerCallback();
                }
                else
                {
                    _timer.Change(interval, interval);
                }
            }
                
        }

        private static void RunJob(Schedule schedule)
        {
            schedule.NextRun = schedule.NextRun + schedule.Interval;

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
                        job.OnExceptionAsync(ex).Wait();
                    }
                    catch (Exception innerEx)
                    {
                        var logger = scope.ServiceProvider.GetService<ILogger<Scheduler>>();
                        logger?.LogError(innerEx, "任务内部处理执行异常。");
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

        private static void OnTimerCallback()
        {
            RunJob(_nextToRun);
            UpdateTimer();
        }

        /// <summary>
        /// 开启新的任务调度
        /// </summary>
        /// <param name="seconds">任务间隔秒数</param>
        public static void StartNew<T>(int seconds) where T : IJob
        {
            StartNew<T>(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// 开启新的任务调度
        /// </summary>
        /// <param name="interval">任务间隔</param>
        public static void StartNew<T>(TimeSpan interval) where T : IJob
        {
            var schedule = new Schedule()
            {
                JobType = typeof(T),
                Interval = interval,
                NextRun = DateTime.Now + interval
            };

            _scheduleList.Add(schedule);
            UpdateTimer();
        }

        
        /// <summary>
        /// 等待程序关闭
        /// </summary>
        public static void WaitForShutdown()
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => StopAllAndWait();
            Console.CancelKeyPress += (s, e) => StopAllAndWait();
            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// 停止所有任务
        /// </summary>
        public static void StopAll()
        {
            Enabled = false;
            _timer.Change(-1, -1);
        }

        /// <summary>
        /// 停止所有任务并等待
        /// </summary>
        public static void StopAllAndWait()
        {
            StopAll();

            var tasks = new Task[0];
            do
            {
                lock (_running)
                {
                    tasks = _running.Select(t => t.Item2).ToArray();
                }
                Task.WaitAll(tasks);
            } while (tasks.Any());
        }
    }
}
