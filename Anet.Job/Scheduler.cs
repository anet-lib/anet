using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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
            var nextRun = GetNextTimeToRun(schedule.NextRun, schedule.Interval);
            if (nextRun != null)
                schedule.NextRun = nextRun.Value;
            else
                _scheduleList.Remove(schedule);

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
        /// <typeparam name="T">用于调度的 <see cref="IJob"/> 服务</typeparam>
        /// <param name="seconds">任务间隔秒数</param>
        [Obsolete("请用 StartNewAt<T>(DateTime, TimeSpan) 代替")]
        public static void StartNew<T>(int seconds) where T : IJob
        {
            StartNewAt<T>(DateTime.Now, TimeSpan.FromSeconds(seconds), true);
        }

        /// <summary>
        /// 开启新的任务调度
        /// </summary>
        /// <typeparam name="T">用于调度的 <see cref="IJob"/> 服务</typeparam>
        /// <param name="interval">任务间隔</param>
        [Obsolete("请用 StartNewAt<T>(DateTime, TimeSpan) 代替")]
        public static void StartNew<T>(TimeSpan interval) where T : IJob
        {
            StartNewAt<T>(DateTime.Now, interval, true);
        }

        /// <summary>
        /// 开启新的任务调度
        /// </summary>
        /// <typeparam name="T">用于调度的 <see cref="IJob"/> 服务</typeparam>
        /// <param name="startTime">任务开始时间</param>
        /// <param name="interval">任务间隔; 当值为 <see cref="TimeSpan.Zero"/> 时, 指定任务为一次性任务</param>
        /// <param name="executeAtStartTime">指定任务是否在<paramref name="startTime"/>时执行</param>
        public static void StartNewAt<T>(DateTime startTime, TimeSpan interval, bool executeAtStartTime = true) where T : IJob
        {
            var schedule = new Schedule()
            {
                JobType = typeof(T),
                Interval = interval,
                NextRun = startTime
            };

            if (!executeAtStartTime)
                schedule.NextRun = GetNextTimeToRun(startTime, interval) ?? throw new ArgumentException("任务无法调度");

            _scheduleList.Add(schedule);
            UpdateTimer();
        }

        private static DateTime? GetNextTimeToRun(DateTime date, TimeSpan interval)
        {
            var span = (DateTime.Now - date).Ticks;
            if (span < 0) return date;
            if (interval <= TimeSpan.Zero) return null;
            return new DateTime(date.Ticks + span + interval.Ticks - span % interval.Ticks, date.Kind);
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
            if (IsStopping) return;
            IsStopping = true;

            Enabled = false;
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
                IsStopping = false;
            });
        }
    }
}
