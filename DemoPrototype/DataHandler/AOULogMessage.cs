using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{

    public class AOULogMessage
    {
        public string time
        {
            get; set;
        }

        public long time_ms
        {
            get; set;
        }

        public string message
        {
            get; set;
        }

        public int pid
        {
            get; set;
        }

        public int prio
        {
            get; set;
        }

        public AOULogMessage(long logTime_ms, string logMsg, int logPrio, int logProcessId)
        {
            time_ms = logTime_ms;
            time = AOUHelper.msToTimeSpanStr(logTime_ms);
            message = logMsg;
            prio = logPrio;
            pid = logProcessId;
        }

        public AOULogMessage(long logTime_ms, string logMsg, int logPrio) : this(logTime_ms, logMsg, logPrio, 0)
        {
        }

        public AOULogMessage(long logTime_ms, string logMsg) : this(logTime_ms, logMsg, 0, 0)
        {
        }

        public string ToString(char sep)
        {
            return String.Format("{0}{4} {1}{4} {2}{4} {3}", time, message, prio, pid, sep);
        }

        public override string ToString()
        {
            return ToString(';');
        }
    }
}
