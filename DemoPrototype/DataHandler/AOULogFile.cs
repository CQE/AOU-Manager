using System;
using System.Collections.Generic;
using System.IO;
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

        public AOULogFile(DateTime StartTime, string name)
        {
            aouLogFile = new TextFile();
            this.startTime = StartTime;
            curAOULogFileName = name + "-" + startTime.ToString("yyMMdd-hhmmss") + ".txt";
        }

        public void AddLog(long time, string text)
        {
            aouLogFile.AddToFile(subPath, curAOULogFileName, ">" + time + "," + text);
        }

        public void DeleteLogFiles(long date)
        {
            //we want to delete all files older than date
            DirectoryInfo folder = new DirectoryInfo(subPath);
            FileInfo[] files = folder.GetFiles();
            long folderSize = files.Sum(fi => fi.Length);
            long folderSizeLimit = 1000;
            long amountToDelete = 1;

            if (folderSize > folderSizeLimit)
            {
                // Sort the list of files with the oldest first.
                Array.Sort(files,
                           (fi1, fi2) => fi1.CreationTime.CompareTo(fi2.CreationTime));

                long amountDeleted = 0L;

                foreach (FileInfo file in files)
                {
                    amountDeleted += file.Length;
                    AppHelper.ShowMessageBox("You are about to delete 1 file");
                    file.Delete();

                    if (amountDeleted >= amountToDelete)
                    {
                        break;
                    }

                }
            }
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
