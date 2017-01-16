using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        private int curSizeOfFolder = 0;
        private int maxSizeOfFolder = 1000000; //just testing

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

        public ulong CheckSize2()
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

        public int CheckSize()
        {
           StorageFolder dataFolder = KnownFolders.PicturesLibrary;
         //   StorageFolder dataSubFolder = KnownFolders.PicturesLibrary.GetFolderAsync("AOU-Logs").AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            DirectoryInfo folder = new DirectoryInfo(subPath);
            IReadOnlyList<StorageFile> filesInFolder2 = dataFolder.GetFilesAsync(CommonFileQuery.OrderByDate).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            // Size of Folder in bytes
            IReadOnlyList<StorageFile> fif = dataFolder.GetFilesAsync(CommonFileQuery.OrderByDate).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

            int fileCount = 0;
            Task tmpTask = Task.Run(() => Parallel.ForEach(fif, async (currentFile) =>
            {
                //    Windows.Storage.FileProperties.BasicProperties bps = currentFile.GetBasicPropertiesAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                Windows.Storage.FileProperties.BasicProperties bps = await currentFile.GetBasicPropertiesAsync();

                Interlocked.Add(ref fileCount, (int)bps.Size);
            }));

            tmpTask.Wait();
            return fileCount;
        }






        public void DeleteLogFiles(long date = 0)
        {
            StorageFolder dataFolder = KnownFolders.PicturesLibrary;
            //we want to delete all files older than date
            IReadOnlyList<StorageFile> fif = dataFolder.GetFilesAsync(CommonFileQuery.OrderByDate).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();

             // Delete files from oldest onwards
            int nFiles = fif.Count;
            for (int fidx = nFiles - 1; fidx >= 0; fidx--)
            {
                Windows.Storage.FileProperties.BasicProperties tps = fif.ElementAt(fidx).GetBasicPropertiesAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
                if (tps.Size > 0UL && fif.ElementAt(fidx).Name == "junk.txt")
                    fif.ElementAt(fidx).DeleteAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
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
