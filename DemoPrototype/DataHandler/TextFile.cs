using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace DemoPrototype
{
    // Store data from one sensor in one file, named with date, change file every day
    public class TextFile
    {
        StorageFolder dataFolder;

        private string logStr = "";

        private string StrData = "";

        public void AddLog(string logText)
        {
            // Just for having a breakpoint here at the moment
            logStr += logText + "\r\n";
        }

        public bool IsDataAvailable()
        {
            return StrData.Length > 0;
        }

        public string GetTextData()
        {
            string text = StrData;
            return text;
        }

        public string GetLogText()
        {
            string text = logStr;
            logStr = "";
            return text;
        }

        public TextFile()
        {
            /*  Most safe to use this. Belongs to User and not App.
                Capabilities for Pictures Library must be enabled in project properties
            */
            dataFolder = KnownFolders.PicturesLibrary;
            StrData = "";
        }

        private async void SaveToFileAsync(StorageFile f, string content)
        {
            try
            {
                using (StorageStreamTransaction transaction = await f.OpenTransactedWriteAsync())
                {

                    using (DataWriter dataWriter = new DataWriter(transaction.Stream))
                    {
                        dataWriter.WriteString(content);
                        transaction.Stream.Size = await dataWriter.StoreAsync(); // reset stream size to override the file
                        await transaction.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog("AddToFileAsync Error: " + ex.Message);
            }
        }

        private async void AddToFileAsync(StorageFile f, string lineOfText)
        {
            try
            {
                await FileIO.AppendTextAsync(f, lineOfText + Environment.NewLine);
            }
            catch (Exception ex)
            {
                AddLog("AddToFileAsync Error: " + ex.Message);
            }
        }


        private async void CreateFileIfNotExistAndAddTextLine(string subPath, string fileName, string lineOfText)
        {
            try
            {
                var folder = await dataFolder.CreateFolderAsync(subPath, CreationCollisionOption.OpenIfExists);

                StorageFile f = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                AddToFileAsync(f, lineOfText);
            }
            catch (Exception ex)
            {
                AddLog("CreateFileIfNotExistAndAddTextLine Error: " + ex.Message);
            }
        }


        private async void CreateFileIfNotExistAndReplaceText(string subPath, string fileName, string text)
        {
            try
            {
                var folder = await dataFolder.CreateFolderAsync(subPath, CreationCollisionOption.OpenIfExists);

                StorageFile f = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                SaveToFileAsync(f, text);
            }
            catch (Exception ex)
            {
                AddLog("CreateFileIfNotExistAndReplaceText Error: " + ex.Message);
            }
        }



        /***********************/
        /* public from here    */
        /***********************/
        public async void OpenFileIfExistAndGetText(string fileName)
        {
            try
            {
                if (fileName[0] == '\\') fileName = fileName.Substring(1);
                StorageFile f = await dataFolder.GetFileAsync(fileName);
                StrData = await FileIO.ReadTextAsync(f);
                AddLog("Filedata loaded from " + fileName + ", " + StrData.Length + " lines");
            }
            catch (Exception ex)
            {
                AppHelper.ShowMessageBox("OpenFileIfExistAndGetText "+ fileName + " Error: " + ex.Message);
            }
        }

        public void SaveToFile(string subPath, string fileName, string textContent)
        {
            CreateFileIfNotExistAndReplaceText(subPath, fileName, textContent);
        }

        public void AddToFile(string subPath, string fileName, string textLine)
        {
            CreateFileIfNotExistAndAddTextLine(subPath, fileName, textLine);
        }

    }
}
