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

                    if ((currentCount % 5) == 3)
                    {
                        int mode = ValueGenerator.GetRandomInt(0, 4);
                        text += AOURandomData.CreateModeXMLString(time, mode);
                    }
                    if ((currentCount % 9) == 1)
                    {
                        AOUDataTypes.UI_Buttons  buttons = new AOUDataTypes.UI_Buttons();
                        buttons.OnOffButton = ValueGenerator.GetRandomOk(2) ? AOUDataTypes.ButtonState.on : AOUDataTypes.ButtonState.off;
                        buttons.ButtonRunWithIMM = ValueGenerator.GetRandomOk(2) ? AOUDataTypes.ButtonState.on : AOUDataTypes.ButtonState.off;
                        buttons.ButtonEmergencyOff = ValueGenerator.GetRandomOk(2) ? AOUDataTypes.ButtonState.on : AOUDataTypes.ButtonState.off;
                        buttons.ButtonForcedCooling = ValueGenerator.GetRandomOk(2) ? AOUDataTypes.ButtonState.on : AOUDataTypes.ButtonState.off;
                        buttons.ButtonForcedHeating = ValueGenerator.GetRandomOk(2) ? AOUDataTypes.ButtonState.on : AOUDataTypes.ButtonState.off;
                        buttons.ButtonForcedCycling = ValueGenerator.GetRandomOk(2) ? AOUDataTypes.ButtonState.on : AOUDataTypes.ButtonState.off;
                        text += CreateUIButtonsXMLString(time, buttons);
                    }
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

        public static string CreateUIButtonsXMLString(uint time, AOUDataTypes.UI_Buttons buttons)
        {
            UInt16 b = 0xFF00;
            if (buttons.OnOffButton == AOUDataTypes.ButtonState.on) b += 1;
            if (buttons.ButtonEmergencyOff == AOUDataTypes.ButtonState.on) b += 1;
            if (buttons.ButtonForcedCooling == AOUDataTypes.ButtonState.on) b += 2;
            if (buttons.ButtonForcedHeating == AOUDataTypes.ButtonState.on) b += 4;
            if (buttons.ButtonForcedCycling == AOUDataTypes.ButtonState.on) b += 8;
            if (buttons.ButtonRunWithIMM == AOUDataTypes.ButtonState.on) b += 16;

            string ui = b.ToString("X4"); 
            string str = AOUInputParser.CreateUIXmlString(time / 100, ui);
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

