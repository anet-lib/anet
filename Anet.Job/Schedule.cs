using System;
using System.Collections.Generic;
using System.Text;

namespace Anet.Job
{
    class Schedule
    {
        public Type JobType { get; internal set; }

        public TimeSpan Interval { get; internal set; }

        public DateTime NextRun { get; internal set; }
    }
}
