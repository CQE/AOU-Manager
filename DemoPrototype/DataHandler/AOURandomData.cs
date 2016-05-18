using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    public class AOURandomData : AOUData
    {
        private AOUSettings.RandomSetting settings;
        private DateTime lastTime;

        private uint curSeqState = 0;
        private uint curHotValve = 0;
        private uint curColdValve = 0;
        private uint curReturnValve = 0;
        private int btnMode = 0;
        private int immMode = 0;

        private uint currentCount = 0;

        public AOURandomData(AOUSettings.RandomSetting rndSettings, AOUSettings.DebugMode dbgMode = AOUSettings.DebugMode.noDebug) : base(dbgMode)
        {
            settings = rndSettings;
            lastTime = startTime;
        }

        public override void Connect()
        {
            base.Connect();
            AddDataLogText("Random data started - num values:" + settings.NumValues + ", ms between:" + settings.MsBetween);
        }

        public override void Disconnect()
        {
            base.Disconnect();
            AddDataLogText("Random Data stopped");
        }


        public override bool SendData(string data)
        {
            TimeSpan timeFromStart = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
            uint time = (uint)timeFromStart.TotalMilliseconds;
            AOUInputParser.CreateLogXmlString(time, "SendData:" + data);
            return true;
        }

        public override void UpdateData()
        {
            base.GetTextDataList();
        }

        protected override string GetTextData()
        {
            string text = "";
            DateTime now = DateTime.Now;
            TimeSpan ts = new TimeSpan(now.Ticks - lastTime.Ticks);
            if (ts.TotalMilliseconds > settings.MsBetween)
            {
                TimeSpan timeFromStart = new TimeSpan(now.Ticks - startTime.Ticks);
                uint time = (uint)timeFromStart.TotalMilliseconds;
                lastTime = now;
                if (currentCount % 20 >= 0) // Test: No return forcasted values for a while
                {
                    text = CreateRandomTempDataXML(time, true);
                }
                else
                {
                    text = CreateRandomTempDataXML(time, false);
                }

                text += CreateRandomLogDataXMLString(time);
                //text += GetPowerHeatingValue(time);

                if (currentCount % 5 == 3)
                {
                    if (curHotValve == 1)
                    {
                        curHotValve = 0;
                        curColdValve = 1;
                        curColdValve = 0;
                    }
                    else if (curColdValve == 1)
                    {
                        curHotValve = 0;
                        curColdValve = 0;
                        curColdValve = 1;
                    }
                    else
                    {
                        curSeqState++;
                        text += CreateNextSeqXMLString(time, curSeqState);
                        if (curSeqState > 10)
                        {
                            curSeqState = 1;
                        }

                        curHotValve = 1;
                        curColdValve = 0;
                        curColdValve = 0;
                    }

                    text += CreateNextValvesXMLString(time, curHotValve, curColdValve, curReturnValve);
                }
                if ((currentCount % 8) == 2)
                {
                    int mode = ValueGenerator.GetRandomInt(0, 4);
                    text += AOURandomData.CreateModeXMLString(time, mode);
                }
                if ((currentCount % 9) == 1)
                {
                    byte mask = 0; byte mode = 0;

                    // ToDo onoff and emerg

                    int lastbtnMode = btnMode;
                    btnMode = ValueGenerator.GetRandomInt(2, 5);
                    if (lastbtnMode >= 2)
                    {
                        mode = DelBit(mode, lastbtnMode);
                        mask = AddBit(mask, lastbtnMode);
                    }
                    mode = AddBit(mode, btnMode);
                    mask = AddBit(mask, btnMode);

                    text += CreateUIButtonsXMLString(time, mask, mode);
                }
                if ((currentCount % 9) == 5)
                {
                    byte mask = 0; byte mode = 0;
                    int lastimmMode = immMode;
                    immMode = ValueGenerator.GetRandomInt(1, 8);
                    if (lastimmMode != immMode)
                        if (lastimmMode > 0)
                        {
                            mode = DelBit(mode, lastimmMode);
                            mask = AddBit(mask, lastimmMode);
                        }
                    mode = AddBit(mode, immMode);
                    mask = AddBit(mask, immMode);

                    text += CreateIMMModeXMLString(time, mask, mode);
                }

                currentCount++;
            }
            return text;
        }


        public static string CreateRandomLogDataXMLString(uint time)
        {
            string logstr = "";
            if (ValueGenerator.GetRandomOk(8)) // Not every time
            {
                logstr = AOUInputParser.CreateLogXmlString(time / 100, ValueGenerator.GetRandomString(6)) + "\r\n";
            }
            return logstr;
        }

        public static string CreateRandomPower(uint time)
        {
            string logstr = "";
            if (ValueGenerator.GetRandomOk(8)) // Not every time
            {
                logstr = AOUInputParser.CreatePowXmlString(time / 100, ValueGenerator.RandomFromUIntArray(new uint[] { 0, 100 })) + "\r\n";
            }
            return logstr;
        }

        public static string CreateNextValvesXMLString(uint time, uint hotValve, uint coldValve, uint retValve)
        {
            string str = AOUInputParser.CreateValvesXmlString(time / 100, hotValve, coldValve, retValve) + "\r\n";
            return str;
        }

        public static string CreateModeXMLString(uint time, int mode)
        {
            string str = AOUInputParser.CreateModeXmlString(time / 100, mode) + "\r\n";
            return str;
        }

        public string CreateHexString(UInt16 word)
        {
            return word.ToString("X4");
        }

        public string CreateHexString(byte mask, byte mode)
        {
            return (CombineToWord(mask, mode)).ToString("X4");
        }

        public string CreateUIButtonsXMLString(uint time, byte mask, byte mode)
        {
            string str = AOUInputParser.CreateUIXmlString(time / 100, CreateHexString(mask, mode)) + "\r\n";
            return str;
        }

        public string CreateIMMModeXMLString(uint time, byte mask, byte mode)
        {
            string str = AOUInputParser.CreateIMMXmlString(time / 100, CreateHexString(mask, mode)) + "\r\n";
            return str;
        }

        public static string CreateNextSeqXMLString(uint time, uint seq)
        {
            string str = AOUInputParser.CreateSeqXmlString(time / 100, seq) + "\r\n";
            return str;
        }

        public static string CreateRandomTempDataXML(uint time, bool createRetFor)
        {
            AOUStateData data = ValueGenerator.GetRandomStateData(time, createRetFor);

            string xml = AOUInputParser.CreateWholeTempStateXmlString(data) + "\r\n";
            return xml;
        }
    }

}

