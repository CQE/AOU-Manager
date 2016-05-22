using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    class AOULogFile
    {
        public const string subPath = "AOU-Logs";
        private TextFile aouLogFile;
        private string curAOULogFileName = "";
        private DateTime startTime;

        public AOULogFile(DateTime StartTime)
        {
            aouLogFile = new TextFile();
            this.startTime = StartTime;
            curAOULogFileName = "AOULog-" + startTime.ToString("yyMMdd-hhmmss") + ".txt";
        }

        public void AddLogMessages(AOULogMessage[] logs)
        {
            var newTime = DateTime.Now;
            if (newTime.Day != startTime.Day)
            {
                // ToDo: New File ?
            }

            foreach (var log in logs)
            {
                if (log == null)
                {
                    aouLogFile.AddToFile(subPath, curAOULogFileName, "NULL");
                }
                else
                {
                    aouLogFile.AddToFile(subPath, curAOULogFileName, log.ToString());
                }
            }
                
        }

    }
}
