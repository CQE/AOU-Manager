using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    public class AOURandomData:AOUData
    {
        private AOUSettings.RandomSetting settings;
        private DateTime lastTime;

        private uint currentSeqState = 0;
        private uint currentHotValve = 0;
        private uint currentColdValve = 0;
        private uint currentReturnValve = 0;
        private uint currentCount = 0;

        public AOURandomData(AOUSettings.RandomSetting rndSettings, AOUSettings.DebugMode dbgMode = AOUSettings.DebugMode.noDebug) : base(dbgMode)
        {
            AddDataLogText("Random Data Ready - num values:" + rndSettings.NumValues + ", ms between:" + rndSettings.MsBetween);
            settings = rndSettings;
            lastTime = startTime;
        }

        public override bool SendData(string data)
        {
            TimeSpan timeFromStart = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
            uint time = (uint)timeFromStart.TotalMilliseconds;
            AOUInputParser.CreateLogXmlString(time, "SendData:"+data);
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
                text = CreateRandomTempDataXML(time);
                text += CreateRandomLogDataXMLString(time);
                //text += GetPowerHeatingValue(time);

                if (currentCount % 5 == 3)
                { 
                    if (currentHotValve == 1)
                    { 
                        currentHotValve = 0;
                        currentColdValve = 1;
                        currentReturnValve = 0;
                    }
                    else if (currentColdValve == 1)
                    {
                        currentHotValve = 0;
                        currentColdValve = 0;
                        currentReturnValve = 1;
                    }
                    else
                    {
                        currentSeqState++;
                        text += CreateNextSeqXMLString(time, currentSeqState);
                        if (currentSeqState > 10)
                        {
                            currentSeqState = 1;
                        }

                        currentHotValve = 1;
                        currentColdValve = 0;
                        currentReturnValve = 0;
                    }

                    text += CreateNextValvesXMLString(time, currentHotValve, currentColdValve, currentReturnValve);
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

        public static string CreateNextSeqXMLString(uint time, uint seq)
        {
            string str = AOUInputParser.CreateSeqXmlString(time / 100, seq) + "\r\n";
            return str;
        }

        public static string CreateRandomTempDataXML(uint time)
        {
            AOUStateData data = ValueGenerator.GetRandomStateData(time);
            string xml = AOUInputParser.CreateStateXmlString(data) + "\r\n";
            return xml;
        }
    }

}

