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

        public AOULogMessage(long logTime, string logMsg)
        {
            time = AOUHelper.msToTimeSpanStr(logTime);
            message = logMsg;
            prio = 0;
            pid = 0;
        }

        public AOULogMessage(long logTime, string logMsg, int logPrio, int logProcessId)
        {
            time = AOUHelper.msToTimeSpanStr(logTime);
            message = logMsg;
            prio = logPrio;
            pid = logProcessId;
        }

        public override string ToString()
        {
            // return String.Format("{0}, {1}, {2}, {3}", time, message, prio, pid);
            return String.Format("{0}; {1}; {2}; {3}", time, message, prio, pid);   //new separator needed message can include ","
        }
    }
}
