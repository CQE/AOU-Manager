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
        string textData;
        int textdataIndex = 0;

        public AOUFileData(AOUSettings.FileSetting fileSetting, AOUSettings.DebugMode mode = AOUSettings.DebugMode.noDebug) : base(mode)
        {
            setting = fileSetting;
            dataFile = new TextFile();
            dataFile.OpenFileIfExistAndGetText(fileSetting.FilePath);
            textData = "";
        }

        public override void UpdateData()
        {
            base.GetTextDataList();
        }

        protected override string GetTextData()
        {
            if (textData .Length == 0)
            {
                textData = dataFile.GetTextData();
                return "";
            }
            else
            {
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
                /*
                if (textData.Length > 100)
                {
                    text = textData.Substring(0, 100);
                    textData = textData.Substring(100);
                }
                else
                {
                    text = textData;
                    textData = "";
                }
                */
                return text;
            }
        }

        public override bool SendData(string data)
        {
            return true;
        }
    }
}
