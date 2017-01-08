using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace DemoPrototype
{
    class AOULogFile
    {
        public const string subPath = "AOU-Logs";
        private TextFile aouLogFile;
        private string curAOULogFileName = "";
        private DateTime startTime;
        private ulong curSizeOfFolder = 0;
        private ulong maxSizeOfFolder = 1000000; //just testing

        public AOULogFile(DateTime StartTime)
        {
           // curSizeOfFolder = CheckSize();
            if (curSizeOfFolder > maxSizeOfFolder)
                DeleteLogFiles();
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

        public ulong CheckSize()
        {
           ulong totSize;
            ulong subTotSize;
            StorageFolder dataFolder = KnownFolders.PicturesLibrary;
            StorageFolder dataSubFolder = KnownFolders.PicturesLibrary.GetFolderAsync("AOU-Logs").AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            DirectoryInfo folder = new DirectoryInfo(subPath);
            IReadOnlyList<StorageFile> filesInFolder2 = dataFolder.GetFilesAsync(CommonFileQuery.OrderByDate).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            // Size of Folder in bytes
            IReadOnlyList<StorageFile> filesInFolder = dataFolder.GetFilesAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            totSize = 0;
            foreach (StorageFile ff in filesInFolder)
            {
                Windows.Storage.FileProperties.BasicProperties bp = ff.GetBasicPropertiesAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                totSize = totSize + bp.Size;
            }

            // Size of Sub folder in bytes
            IReadOnlyList<StorageFile> filesInSubFolder = dataSubFolder.GetFilesAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            subTotSize = 0;
            foreach (StorageFile ffs in filesInSubFolder)
            {
                Windows.Storage.FileProperties.BasicProperties bps = ffs.GetBasicPropertiesAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                subTotSize += bps.Size;
            }
            return totSize + subTotSize;
        }




        public void DeleteLogFiles(long date = 0)
        {
            //we want to delete all files older than date
            DirectoryInfo folder = new DirectoryInfo(subPath);
            IReadOnlyList<StorageFile> filesInFolder = dataFolder.GetFilesAsync(CommonFileQuery.OrderByDate).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            return;
            /*
            FileInfo[] files = folder.GetFiles();
           // FileInfo[] files2 = await dataFolder.GetFilesAsync();
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
            }*/
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
