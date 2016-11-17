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
        string textData = "";

        bool fileLoaded = false;

        uint lastTimeStamp = uint.MaxValue;
        uint lastTimeInFile = 0;
        uint loop = 0;

        private List<string> commandsToReplyList;

        public AOUFileData(AOUSettings.FileSetting fileSetting, AOUSettings.DebugMode mode = AOUSettings.DebugMode.noDebug) : base(mode)
        {
            setting = fileSetting;
            commandsToReplyList = new List<string>();
        }

        public override void Connect()
        {
            fileLoaded = false;
            textData = "";
            fileData = "";
            lastTimeStamp = uint.MaxValue;
            lastTimeInFile = 0;
            loop = 0;
            dataFile = new TextFile();
            dataFile.OpenFileIfExistAndGetText(setting.FilePath);
            // Wait until async open file operation is Ready
            // var t = Task.Run(() => dataFile.FileIsReady()); // Test
            Task.Delay(200); // Test. Not start until whole file is read
            base.Connect();
        }

        public override void Disconnect()
        {
            dataFile = null;
            base.Disconnect();
            AddDataLogText("File data stopped");
        }

        public override void UpdateData()
        {
            if (dataFile != null)
            {
                string filelog = dataFile.GetLogText();
                if (filelog.Length > 0)
                {
                    AddDataLogText(filelog);
                    if (dataFile.IsDataAvailable())
                    {
                        AddDataLogText("File data started: " + setting.FilePath);
                    }
                    else
                    {
                        AddDataLogText("File data not started: " + setting.FilePath); 
                    }
                }
            }
            base.GetTextDataList();
        }

        private string handleCmdList(uint time)
        {
            string retStr = "";
            string tag = "";
            string content = "";
            string value = "";
            int cmdPos = 0;
            foreach (var txt in commandsToReplyList)
            {
                if (AOUTagParser.FindTagAndExtractText("cmd", txt, out content, out cmdPos))
                {
                    int tagEndPos;
                    if (AOUTagParser.GetTagAndContent(content, out tag, out value, out tagEndPos))
                    {
                        if (value.Length == 0)
                        {
                            value = ValueGenerator.GetStaticCommandValue(tag).ToString();
                        }

                        retStr += AOURandomData.CreateCmdRetXMLString(time, tag, value);
                    }

                }
            }
            commandsToReplyList.Clear(); // Remove all when handled
            return retStr;
        }

        private string HandleNewLoop()
        {
            if (dataFile != null)
            {
                if (!fileLoaded)
                {
                    // First time: Load text from file to string fileData and close the file
                    fileLoaded = true;
                    fileData = dataFile.GetTextData();
                    dataFile = null;
                }
                else
                {
                    // Next time: Save lastTimeStamp for filedata time span. update loop counter
                    if (lastTimeInFile == 0)
                    {
                        lastTimeInFile = lastTimeStamp;
                    }
                    loop++;
                }
            }
            // Move fileData to textData to work on
            textData = fileData;
            return ""; 
        }

        private string GetNextLineInFile(out uint fileTime, out uint newTimeStamp)
        {
            fileTime = 0;
            newTimeStamp = 0;

            string text = "";
            string newTextData = "";
            string tst = "";
            int pos = textData.IndexOf('\n');
            
            if (pos > 0 && pos < textData.Length)
            {
                // Get next textline in file to text
                text = textData.Substring(0, pos + 1);
                newTextData = textData.Substring(pos + 1); // Next lines in file
            }
            else
            {
                // Use rest of file if '\n' is missing in last line of file
                text = textData;
            }

            if (loop == 0 && AOUTagParser.ParseString("Msg", text, out tst))
            {
                // Delete first startup messages for next loops in file (fileData)
                if (tst == "setup() ready")
                {
                    string search = "<log><Time>42</Time><Msg>setup() ready</Msg></log>";
                    int filePos = fileData.IndexOf(search);
                    if (filePos > 1)
                    {
                        fileData = fileData.Substring(filePos + search.Length).TrimStart();
                    }
                }
            }

            uint timeAdd = 0;
            if (loop > 0)
            {
                /*
                    If repeat reading from file take last timestamp in file and multiply with 
                    current repeated loop count + 1 sek extra time (deci seconds)
                */
                timeAdd = loop * (lastTimeInFile + 10);
            }

            if (AOUTagParser.ParseUInt("Time", text, out fileTime))
            {
                if (lastTimeStamp == uint.MaxValue)
                {
                    // First time in file to get last time time value from. Always 0 ?
                    textData = newTextData;
                    lastTimeStamp = fileTime;
                }
                else 
                {
                    newTimeStamp = timeAdd + fileTime;
                    long realTime = GetAOUTime_ms() / 100;

                    // Only ready if real time >= simulated time
                    if (realTime >= newTimeStamp)
                    {
                        textData = newTextData;
                        lastTimeStamp = fileTime;
                    }
                    else
                    {
                        // Wait to next time
                        text = ""; // Not ready
                    }
                }


            }
            else
            {
                // Only text and no timestamp. Take latest timestamp and return as log message
                text = AOUXMLCreator.CreateLogXmlString(timeAdd + lastTimeStamp, text.Trim());
                textData = newTextData;
            }

            return text;
        }


        private string ReplaceTimeToRealTime(string textline, uint oldTimeStamp, uint newTimeStamp)
        {
            string retText = textline;
            if (loop > 0) // Do not replace time when first time
            {
                string newTime = AOUXMLCreator.CreateTimeXmlString(newTimeStamp);
                retText = textline.Replace("<Time>" + oldTimeStamp + "</Time>", newTime);
            }
            return retText;
        }

        protected override string GetTextData()
        {
            string text = "";
            if (textData.Length == 0)
            {
                return HandleNewLoop(); // Reset to new loop and return no text data
            }
            else
            {
                uint oldTimeStamp;
                uint newTimeStamp;

                text = GetNextLineInFile(out oldTimeStamp, out newTimeStamp);
                if (text.Length > 0 && loop > 0)
                { 
                    // Replace time in tag to simulate continous data stream
                    text = ReplaceTimeToRealTime(text, oldTimeStamp, newTimeStamp);
                }

                // If reply cmd
                if (commandsToReplyList.Count > 0)
                {
                    text += handleCmdList(newTimeStamp);
                }

                return text;
            }
       }

        public override bool SendData(string data)
        {
            TimeSpan timeFromStart = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
            uint time = (uint)timeFromStart.TotalMilliseconds;
            AOUXMLCreator.CreateLogXmlString(time, "SendData:" + data);
            commandsToReplyList.Add(data);
            return true;
        }
    }
}

