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

        public AOULogMessage(long logTime, string logMsg, int logPrio, int logProcessId)
        {
            time = AOUHelper.msToTimeSpanStr(logTime);
            message = logMsg;
            prio = logPrio;
            pid = logProcessId;
        }

        public AOULogMessage(long logTime, string logMsg, int logPrio) : this(logTime, logMsg, logPrio, 0)
        {
        }

        public AOULogMessage(long logTime, string logMsg) : this(logTime, logMsg, 0, 0)
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
