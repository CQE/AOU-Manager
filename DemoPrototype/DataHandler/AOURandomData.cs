using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    public class AOURandomData:AOUData
    {
        private double time_res;

        public AOURandomData(AOUSettings.RandomSetting rndSettings, AOUSettings.DebugMode dbgMode = AOUSettings.DebugMode.noDebug) : base(dbgMode)
        {
            AddDataLogText("Random Data Ready - num values:" + rndSettings.NumValues + ", ms between:" + rndSettings.MsBetween);
            time_res = rndSettings.MsBetween;

        }

        public override bool SendData(string data)
        {
            newLogMessages.Add(new AOULogMessage(0, "Send Data:" + data)); // ToDo Time
            return true;
        }

        public override void UpdateData()
        {
            if (ValueGenerator.GetRandomOk(30)) { 
                newLogMessages.Add(ValueGenerator.GetRandomLogMsg(time_res));
            }

            newPowerValues.Add(ValueGenerator.GetRandomPower(time_res));
        }

        protected override string GetTextData()
        {
            return "."; // Todo Nothing. Only Raw Data
        }

        #region private static methods
        private static string CreateRandomTempXmlString(long time)
        {
            var rndData = ValueGenerator.GetRandomTempData(time);
            return AOUInputParser.CreateTempXmlString(time, rndData);
        }

        private static string CreateRandomSeqXmlString(long time, AOUDataTypes.StateType state)
        {
            var rndData = ValueGenerator.GetRandomSeqData(time, state);
            return AOUInputParser.CreateSeqXmlString(time, rndData);
        }

        /*
        private static string CreateRandomIMMXmlString(long time, AOUDataTypes.IMMSettings settings)
        {
            var rndData = ValueGenerator.GetRandomIMMData(time, settings);
            return AOUInputParser.CreateIMMXmlString(time, rndData);
        }
        */
        private static string CreateRandomColdFeedXmlString(long time)
        {
            var rndData = ValueGenerator.GetRandomColdFeedData(time);
            return AOUInputParser.CreateColdFeedXmlString(time, rndData);
        }

        private static string CreateRandomHotFeedXmlString(long time)
        {
            var rndData = ValueGenerator.GetRandomHotFeedData(time);
            return AOUInputParser.CreateHotFeedXmlString(time, rndData);
        }

        private static string CreateRandomColdLevelString(long time)
        {
            var rndData = ValueGenerator.GetRandomColdLevelData(time);
            return AOUInputParser.CreateColdLevelXmlString(time, rndData);
        }

        private static string CreateRandomHotLevelString(long time)
        {
            var rndData = ValueGenerator.GetRandomHotLevelData(time);
            return AOUInputParser.CreateHotLevelXmlString(time, rndData);
        }

        private static string CreateRandomValvesXmlString(long time)
        {
            var rndData = ValueGenerator.GetRandomValvesData(time);
            return AOUInputParser.CreateValvesXmlString(time, rndData);
        }

        private static string CreateRandomLogXmlString(long time)
        {
            return AOUInputParser.CreateLogXmlString(time, ValueGenerator.GetRandomLogMsg(1000));
        }

        #endregion

        public static string CreateRandomText(uint num, long startTime, uint msBetween)
        {
            AOURandomData rndData = new AOURandomData(new AOUSettings.RandomSetting(num, msBetween));
            string str = "CreateRandomText";
            long time = startTime + msBetween;
            // AOUTypes.StateType state = AOUTypes.StateType.SQ_WAIT_HOT_AT_MOULD_ENTRY;
            // AOUTypes.IMMSettings immSetting = AOUTypes.IMMSettings.CycleAuto;
            return str;
        }

        public static string CreateRandomXML(uint num, long startTime, uint msBetween)
        {
            string xml = "";
            long time = startTime + msBetween;
            AOUDataTypes.StateType state = AOUDataTypes.StateType.SQ_WAIT_HOT_AT_MOULD_ENTRY;
            AOUDataTypes.IMMSettings immSetting = AOUDataTypes.IMMSettings.InCycleAuto;

            for (int i = 0; i < num; i++)
            {
                xml += CreateRandomTempXmlString(time) + "\r\n";

                if (state == AOUDataTypes.StateType.SQ_WAIT_HOT_AT_MOULD_ENTRY)
                    xml += CreateRandomHotFeedXmlString(time) + "\r\n";
                else if (state == AOUDataTypes.StateType.SQ_WAIT_COLD_AT_MOULD_ENTRY)
                    xml += CreateRandomColdFeedXmlString(time) + "\r\n";

                if (state == AOUDataTypes.StateType.SQ_WAIT_HOT_AT_MOULD_ENTRY)
                    xml += CreateRandomHotLevelString(time) + "\r\n";
                else if (state == AOUDataTypes.StateType.SQ_WAIT_COLD_AT_MOULD_ENTRY)
                    xml += CreateRandomColdLevelString(time) + "\r\n";

                xml += CreateRandomValvesXmlString(time) + "\r\n";

                time += msBetween;

                if ((i % 5) == 0)
                {
                    xml += CreateRandomSeqXmlString(time, state) + "\r\n";
                    if (state == AOUDataTypes.StateType.SQ_WAIT_FOR_OPEN_END)
                    {
                        state = AOUDataTypes.StateType.SQ_WAIT_HOT_AT_MOULD_ENTRY;
                    }
                    else
                    {
                        state++;
                    }
                }

                if ((i % 8) == 0)
                {
                    xml += CreateRandomLogXmlString(time) + "\r\n";
                }

                if ((i % 10) == 0)
                {
                   /* xml += CreateRandomIMMXmlString(time, immSetting) + "\r\n";
                    if (immSetting == AOUDataTypes.IMMSettings.IMMToolClosed)
                    {
                        immSetting = AOUDataTypes.IMMSettings.SetIMMBlockInject;
                    }
                    else
                    {
                        immSetting++;
                    }
                    */
                }
            }
            return xml;
        }
    }

}

