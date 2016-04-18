using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    public class AOUData
    {
        private string dataLogStr = "";
        private string dataErrStr = "";

        protected List<AOULogMessage> newLogMessages;
        protected List<Power> newPowerValues;

        private double curTimeSpan = 1000; // 1 sek between 

        public bool Connected { get; protected set; }

        protected AOUSettings.DebugMode debugMode;

        // protected string rawData = "";

        protected AOUData(AOUSettings.DebugMode dbgMode)
        {
            Connected = false;
            debugMode = dbgMode;

            newLogMessages = new List<AOULogMessage>();
            newPowerValues = new List<Power>();
        }

        /* Virtual functions */
        public virtual void Connect()
        {
            Connected = true;
        }

        public virtual void Disconnect()
        {
            Connected = false;
        }

        protected virtual string GetTextData()
        {
            throw new Exception("AOUData.GetTextData Not overrided");
        }

        public virtual bool SendData(string data)
        {
            throw new Exception("AOUData.SendData Not overrided");
        }

        public virtual void UpdateData()
        {
            /*
            Update newLogMessages and newPowerValues
            */
            throw new Exception("AOUData.UpdateData Not overrided");
        }

        /* Protected methods */
        protected void AddDataLogText(string text)
        {
            if (dataLogStr.Length > 0)
                dataLogStr += Environment.NewLine;
            dataLogStr += text;
        }

        protected void AddDataErrText(string text)
        {
            if (dataErrStr.Length > 0)
                dataErrStr += Environment.NewLine;
            dataErrStr += text;
        }

        /* Public methods */
        public bool HaveErrors()
        {
            return dataErrStr.Length > 0;
        }

        public bool HaveLogs()
        {
            return dataLogStr.Length > 0;
        }

        public string GetDataLogText()
        {
            string text = dataLogStr;
            dataLogStr = "";

            return text;
        }

        public string GetDataErrText()
        {
            string text = dataErrStr;
            dataErrStr = "";

            return text;
        }

       public bool AreNewValuesAvailable()
        {
            if (newPowerValues.Count > 0)
            {
                return true;
            }
            return false;
        }

        public Power[] GetNewValues()
        {
            Power[] powers = newPowerValues.ToArray();
            newPowerValues.Clear();
            return powers;
        }

        public bool AreNewLogMessagesAvailable()
        {
            return newLogMessages.Count > 0;
        }

        public AOULogMessage[] GetNewLogMessages()
        {
            AOULogMessage[] logs = newLogMessages.ToArray();
            newLogMessages.Clear();
            return logs;
        }

        // dbgMode

        public string GetRawData()
        {
            return GetTextData();
        }

        protected void GetTextDataList()
        {
            long time_ms = 0;
            string logMsg = "";

            if (this.debugMode == AOUSettings.DebugMode.rawData)
            {
                return;
            }

            AOUStateData stateData;

            AOUSeqData seqData;
            AOUTemperatureData tempData;
            AOUHotFeedData hotFeedData;
            AOUColdFeedData coldFeedData;
            AOUHotLevelData hotLevelData;
            AOUColdLevelData coldLevelData;
            AOUValvesData valvesData;
            AOUIMMData immData;

            newPowerValues = new List<Power>();

            newLogMessages = new List<AOULogMessage>();

            string textDataStream = GetTextData();
            int prevTextLength = textDataStream.Length;

            while (prevTextLength > 0)
            {
                Power tempPower = new Power(0);
                int count = 0;
                string tagContent;
                List<string> loglines;

                string nextTag = AOUInputParser.GetNextTag(textDataStream, out tagContent, out loglines, out count);
                foreach (string log in loglines)
                {
                    if (log.Length > 0)
                        newLogMessages.Add(new AOULogMessage(AOUHelper.GetNowToMs(), log, 8, 0));
                }

                loglines = AOUInputParser.ParseBetweenTagsMessages(tagContent);
                foreach (string log in loglines)
                {
                    string logStr = log;
                    if (logStr.Length > 0)
                    {
                        if (logStr[0] == ',')
                            logStr = logStr.Substring(1);

                        if (logStr[logStr.Length-1] == ',')
                            logStr = logStr.Substring(0, logStr.Length-1);

                        log.Trim(new char[] { ',', ' ' });
                        newLogMessages.Add(new AOULogMessage(AOUHelper.GetNowToMs(), logStr, 9, 0));
                    }
                }

                if (nextTag == AOUInputParser.tagTemperature)
                {
                    if (AOUInputParser.ParseTemperature(tagContent, out tempData))
                    {
                        tempPower.ElapsedTime = AOUDataTypes.AOUModelTimeToTimeMs(tempData.time_min_of_week, tempData.time_ms_of_min);
                        tempPower.THotTank = tempData.hotTankTemp;
                        tempPower.TColdTank = tempData.coldTankTemp;
                        tempPower.TReturnActual = tempData.retTemp;
                        tempPower.ValveCoolant = tempData.coolerTemp;
                    }
                }
                else if (nextTag == AOUInputParser.tagState)
                {
                    AOUInputParser.ParseState(tagContent, out stateData);
                    tempPower.ElapsedTime = AOUDataTypes.AOUModelTimeDecSecToTimeMs(stateData.time_min_of_week, stateData.time_ms_of_min);
                    tempPower.THotTank = stateData.hotTankTemp;
                    tempPower.TColdTank = stateData.coldTankTemp;
                    tempPower.TReturnActual = stateData.retTemp;
                    tempPower.ValveCoolant = stateData.coolerTemp;
                    tempPower.TBufferCold = stateData.bufCold;
                    tempPower.TBufferMid = stateData.bufMid;
                    tempPower.TBufferHot = stateData.bufHot;
                }
                else if (nextTag == AOUInputParser.tagSequence)
                {
                    if (AOUInputParser.ParseSequence(tagContent, out seqData))
                    {
                        tempPower.ElapsedTime = AOUDataTypes.AOUModelTimeToTimeMs(seqData.time_min_of_week, seqData.time_ms_of_min);
                        tempPower.State = (AOUDataTypes.StateType)seqData.state;
                        // tempPower.Cycle = seqData.cycle;
                    }
                }
                else if (nextTag == AOUInputParser.tagIMM)
                {
                    if (AOUInputParser.ParseIMM(tagContent, out immData))
                    {
                        tempPower.ElapsedTime = AOUDataTypes.AOUModelTimeToTimeMs(immData.time_min_of_week, immData.time_ms_of_min);
                        AOUDataTypes.IMMSettings set = (AOUDataTypes.IMMSettings)immData.imm_setting_type;
                        long value = immData.imm_setting_val;
                        // tempPower.State = Types(immData;
                    }
                }
                else if (nextTag == AOUInputParser.tagFeeds)
                {
                    if (AOUInputParser.FindTag(AOUInputParser.tagFeedsHot, tagContent))
                    {
                        if (AOUInputParser.ParseHotFeed(tagContent, out hotFeedData))
                        {
                            tempPower.ElapsedTime = AOUDataTypes.AOUModelTimeToTimeMs(hotFeedData.time_min_of_week, hotFeedData.time_ms_of_min);
                            tempPower.TReturnActual = hotFeedData.prevFeedTemp;
                            tempPower.TReturnForecasted = hotFeedData.newFeedTemp;
                        }
                    }
                    else
                        if (AOUInputParser.ParseColdFeed(tagContent, out coldFeedData))
                    {
                        tempPower.ElapsedTime = AOUDataTypes.AOUModelTimeToTimeMs(coldFeedData.time_min_of_week, coldFeedData.time_ms_of_min);
                        tempPower.TReturnActual = coldFeedData.prevFeedTemp;
                        tempPower.TReturnForecasted = coldFeedData.newFeedTemp;
                    }
                }
                else if (nextTag == AOUInputParser.tagLevels)
                {
                    if (AOUInputParser.FindTag(AOUInputParser.tagLevelsSubTagHot, tagContent))
                    {
                        if (AOUInputParser.ParseHotLevel(tagContent, out hotLevelData))
                        {
                            tempPower.ElapsedTime = AOUDataTypes.AOUModelTimeToTimeMs(hotLevelData.time_min_of_week, hotLevelData.time_ms_of_min);
                            // tempPower.TReturnActual = hotLevelData.prevLevel;
                            // tempPower.TReturnForecasted = hotLevelData.newLevel;
                        }
                    }
                    else
                    {
                        if (AOUInputParser.ParseColdLevel(tagContent, out coldLevelData))
                        {
                            tempPower.ElapsedTime = AOUDataTypes.AOUModelTimeToTimeMs(coldLevelData.time_min_of_week, coldLevelData.time_ms_of_min);
                            // tempPower.TReturnActual = coldLevelData.prevLevel;
                            // tempPower.TReturnForecasted = coldLevelData.newLevel;
                        }
                    }
                }
                else if (nextTag == AOUInputParser.tagValves)
                {
                    if (AOUInputParser.ParseValves(tagContent, out valvesData))
                    {
                        tempPower.ElapsedTime = AOUDataTypes.AOUModelTimeToTimeMs(valvesData.time_min_of_week, valvesData.time_ms_of_min);
                        tempPower.TReturnValve = valvesData.prevValveReturnTemp;
                        tempPower.TReturnActual = valvesData.prevValveReturnTemp;
                    }
                }
                else if (nextTag == AOUInputParser.tagLog)
                {
                    if (AOUInputParser.ParseLog(tagContent, out time_ms, out logMsg))
                    {
                        AOULogMessage msg = new AOULogMessage(AOUHelper.ToCurTimeStep(time_ms, curTimeSpan), logMsg);
                        if (msg.prio == 0) msg.prio = 1;
                        newLogMessages.Add(msg);
                    }
                }
                else if (nextTag.Length > 0)
                {
                    newLogMessages.Add(new AOULogMessage(AOUHelper.GetNowToMs(), "Unknown:" + tagContent, 0, 0));
                }

                // long curtimeStep = AOUHelper.ToCurTimeStep(tempPower.ElapsedTime, curTimeSpan);
                if (AOUInputParser.ValidPowerTag(nextTag))
                {
                    newPowerValues.Add(tempPower);
                }
                if (count == 0) // No more valid tags. Wait for more data
                {
                    break;
                }
                else
                {
                    textDataStream = textDataStream.Substring(count); // Delete handled tag
                }
            }
        }

    }
}
