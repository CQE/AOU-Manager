using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    class AOUFileData : AOUData
    {
        private TextFile dataFile;
        private AOUSettings.FileSetting setting;

        string fileData = "";
        string textData;

        bool fileLoaded = false;
        int textdataIndex = 0;

        // ToDo. Go on time in file and add time when reading more times from file
        private int counter;
        private const int count_max = 2;


        public AOUFileData(AOUSettings.FileSetting fileSetting, AOUSettings.DebugMode mode = AOUSettings.DebugMode.noDebug) : base(mode)
        {
            setting = fileSetting;
            dataFile = new TextFile();
            dataFile.OpenFileIfExistAndGetText(fileSetting.FilePath);
            textData = "";
            counter = count_max;
        }

        public override void UpdateData()
        {
            base.GetTextDataList();
        }

        protected override string GetTextData()
        {
            if (counter <= 0)
            {
                if (textData.Length == 0)
                {
                    if (!fileLoaded)
                    {
                        fileLoaded = true;
                        fileData = dataFile.GetTextData();
                    }
                    textData = fileData;
                    return "";
                }
                else
                {
                    counter = count_max;
                    string text = "";
                    int pos = textData.IndexOf('\n');
                    if (pos > 0 && pos < textData.Length)
                    {
                        text = textData.Substring(0, pos + 1);
                        textData = textData.Substring(pos + 1);
                    }
                    else
                    {
                        text = textData;
                        textData = "";
                    }
                    return text;
                }
            }
            else
            {
                counter--;
                return "";
            }
        }

        public override bool SendData(string data)
        {
            return true;
        }
    }
}

