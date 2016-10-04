﻿using System;
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

        long firstTimeStamp = -1;
        long lastTimeStamp = -1;
        long currentTimeStamp = -1;
        uint loop = 0;

        public AOUFileData(AOUSettings.FileSetting fileSetting, AOUSettings.DebugMode mode = AOUSettings.DebugMode.noDebug) : base(mode)
        {
            setting = fileSetting;
        }

        public override void Connect()
        {
            base.Connect();
            dataFile = new TextFile();
            dataFile.OpenFileIfExistAndGetText(setting.FilePath);
            textData = "";
            AddDataLogText("File data started: " + setting.FilePath);
        }

        public override void Disconnect()
        {
            base.Disconnect();
            AddDataLogText("File data stopped");
        }

        public override void UpdateData()
        {
             base.GetTextDataList();
        }

        protected override string GetTextData()
        {
            long time;

            if (textData.Length == 0)
            {
                if (!fileLoaded)
                {
                    fileLoaded = true;
                    fileData = dataFile.GetTextData();
                    dataFile = null;
                }
                else
                {
                    loop++;
                }
                textData = fileData;
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
                if (AOUTagParser.ParseLongTime(text, out time))
                {
                    if (firstTimeStamp < 0)
                    {
                        firstTimeStamp = time;
                    }
                    currentTimeStamp = time;
                    if (loop > 0)
                    {
                        long newTimeStamp = time + loop * lastTimeStamp + 10;
                        string newTime = AOUXMLCreator.CreateTimeXmlString((uint)newTimeStamp);
                        text = text.Replace("<Time>" + time + "</Time>", newTime);
                    }

                }
                if (lastTimeStamp < 0 && textData.Length == 0)
                {
                    lastTimeStamp = currentTimeStamp;
                }

                return text;
            }
       }

        public override bool SendData(string data)
        {
            return true;
        }
    }
}

