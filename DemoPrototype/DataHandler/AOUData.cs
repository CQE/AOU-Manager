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

        protected AOUDataTypes.StateType currentSeqState;
        protected int currentHotValve = 50; // 50, 70
        protected int currentColdValve = 50;
        protected int currentReturnValve = 50;
        protected int currentPower = 0;
        protected uint currentEnergy = 0;

        protected AOUDataTypes.UI_Buttons currentUIButtons = new AOUDataTypes.UI_Buttons();
        protected AOUDataTypes.HT_StateType currentMode = AOUDataTypes.HT_StateType.HT_STATE_NOT_SET;
        protected AOUDataTypes.IMMSettings currentIMMState = AOUDataTypes.IMMSettings.Nothing;

        private double curTimeSpan = 1000; // 1 sek between 

        protected DateTime startTime;


        public bool Connected { get; protected set; }

        protected AOUSettings.DebugMode debugMode;

        // protected string rawData = "";

        protected AOUData(AOUSettings.DebugMode dbgMode)
        {
            Connected = false;
            debugMode = dbgMode;

            currentSeqState = AOUDataTypes.StateType.NOTHING;

            newLogMessages = new List<AOULogMessage>();
            newPowerValues = new List<Power>();

            startTime = DateTime.Now;
        }

        protected long GetTimeSecx10()
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
            return (long)(ts.TotalMilliseconds / 100);
        }

        protected long GetTime_ms()
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
            long ms = (long)(ts.TotalMilliseconds);
            return AOUHelper.ToCurTimeStep(ms, curTimeSpan);
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

        protected byte GetStateByte(UInt16 word)
        {
            byte mask = (byte)(word >> 8);
            return (byte)(word & 0x00FF); // ???? Do mask
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

            newPowerValues = new List<Power>();
            newLogMessages = new List<AOULogMessage>();

            string textDataStream = GetTextData();
            int prevTextLength = textDataStream.Length;

            while (prevTextLength > 0)
            {
                Power tempPower = new Power(0);
                bool IsTempData = false;

                int count = 0;
                string tagContent;
                List<string> loglines;

                string nextTag = AOUInputParser.GetNextTag(textDataStream, out tagContent, out loglines, out count);
                foreach (string log in loglines)
                {
                    if (log.Length > 0)
                        newLogMessages.Add(new AOULogMessage(GetTime_ms(), log, 8, 0));
                }

                loglines = AOUInputParser.ParseBetweenTagsMessages(tagContent);
                foreach (string log in loglines)
                {
                    string logStr = log;
                    if (logStr.Length > 0)
                    {
                        if (logStr[0] == ',')
                            logStr = logStr.Substring(1);

                        if (logStr[logStr.Length - 1] == ',')
                            logStr = logStr.Substring(0, logStr.Length - 1);

                        log.Trim(new char[] { ',', ' ' });
                        newLogMessages.Add(new AOULogMessage(GetTime_ms(), logStr, 9, 0));
                    }
                }

                if (nextTag == AOUInputParser.tagState)
                {
                    AOUInputParser.ParseState(tagContent, out stateData);

                    if (!AOUDataTypes.IsUInt16NaN(stateData.Power))
                    {
                        currentPower = stateData.Power;
                    }

                    if (!AOUDataTypes.IsUInt16NaN(stateData.seqState))
                    {   /*
                        12 - "Unknown", 11 - "WOpenEnd", 10 - "WEjectEnd", 9 - "WEjectBegin", 8 - "WOpenBegin", 7 - "WCoolingEnd"
                        6 - "WInjectionEnd",  5 - "WInjectionBegin", 4 - "WColdAtMEntry", 3 - "WHotAtMEntry", 2 - "Idle", 1 - "Initial"
                        NOTHING = 0, SQ_INITIAL, IDLE, SQ_WAIT_HOT_AT_MOULD_ENTRY, SQ_WAIT_COLD_AT_MOULD_ENTRY,
                        SQ_WAIT_FOR_INJECTION_BEGIN, SQ_WAIT_FOR_INJECTION_END, SQ_WAIT_FOR_COOLING_END,
                        SQ_WAIT_FOR_OPEN_BEGIN, SQ_WAIT_FOR_EJECT_BEGIN, SQ_WAIT_FOR_EJECT_END, SQ_WAIT_FOR_OPEN_END  */
                        currentSeqState = (AOUDataTypes.StateType)stateData.seqState;
                    }

                    if (!AOUDataTypes.IsUInt16NaN(stateData.Valves))
                    {
                        // -- VALVES -- <Valves>MMSS</Valves> MASK (e.g. “3F”), STATE Bits: 0/Hot valve, 1/Cold valve, 2/Return valve
                        currentHotValve = (stateData.Valves & 1) != 0 ? 70 : 50;  // Off=50, On=70  
                        currentColdValve = (stateData.Valves & 2) != 0 ? 70 : 50;  // Off=50, On=70  
                        currentReturnValve = (stateData.Valves & 4) != 0 ? 70 : 50;  // Cold=50, Hot=70  
                        // tempPower.ValveCoolant = (valveState & 8) != 0 ? 100 : 0; // ????
                    }

                    if (stateData.hotTankTemp < 500) // Temp data. ToDo better test
                    {
                        tempPower.ElapsedTime = AOUDataTypes.AOUModelTimeSecX10_to_TimeMs(stateData.time_hours, stateData.time_sek_x_10_of_hour);
                        tempPower.THotTank = stateData.hotTankTemp;
                        tempPower.TColdTank = stateData.coldTankTemp;
                        tempPower.TReturnValve = stateData.retTemp;

                        tempPower.TReturnActual = stateData.retTemp;      // Todo
                        tempPower.TReturnForecasted = stateData.retTemp;  // Todo

                        tempPower.TBufferCold = stateData.bufColdTemp;
                        tempPower.TBufferMid = stateData.bufMidTemp;
                        tempPower.TBufferHot = stateData.bufHotTemp;

                        tempPower.State = currentSeqState;

                        tempPower.ValveFeedCold = currentColdValve;
                        tempPower.ValveFeedHot = currentHotValve;
                        tempPower.ValveReturn = currentReturnValve;

                        tempPower.THeaterOilOut = stateData.heaterTemp;

                        tempPower.PowerHeating = currentPower;
                        IsTempData = true; // Only add new power if temperature data

                        /* ToDo when ????
                        tempPower.THeatExchangerCoolantOut = 0;
                        */
                        tempPower.ValveCoolant = stateData.coolerTemp; // ????? %
                    }


                    if (!AOUDataTypes.IsUInt16NaN(stateData.IMM))
                    {
                        // <IMM>MMSS</IMM>, 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”).
                        // IMM_OutIMMError: 0x01; IMM_OutIMMBlockInject: 0x02; IMM_OutIMMBlockOpen: 0x04; IMM_InIMMStop: 0x08;
                        // IMM_InCycleAuto: 0x10; IMM_InIMMInjecting: 0x20; IMM_InIMMEjecting: 0x40; IMM_InIMMToolClosed: 0x80;
                        switch (stateData.IMM)
                        {
                            case 0x01: currentIMMState = AOUDataTypes.IMMSettings.OutIMMError; break;
                            case 0x02: currentIMMState = AOUDataTypes.IMMSettings.OutIMMBlockInject; break;
                            case 0x04: currentIMMState = AOUDataTypes.IMMSettings.OutIMMBlockOpen; break;
                            case 0x08: currentIMMState = AOUDataTypes.IMMSettings.InIMMStop; break;
                            case 0x10: currentIMMState = AOUDataTypes.IMMSettings.InCycleAuto; break;
                            case 0x20: currentIMMState = AOUDataTypes.IMMSettings.InIMMInjecting; break;
                            case 0x40: currentIMMState = AOUDataTypes.IMMSettings.InIMMEjecting; break;
                            case 0x80: currentIMMState = AOUDataTypes.IMMSettings.InIMMToolClosed; break;
                            default: currentIMMState = AOUDataTypes.IMMSettings.Nothing; break;
                        }
                    }


                    if (!AOUDataTypes.IsUInt16NaN(stateData.UIButtons))
                    {
                        // UI>MMSS</UI> (hex) MM=8bit mask, SS=8bits. 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”).
                        // BUTTON_ONOFF: 0x0001  // Soft on/Off;  BUTTON_EMERGENCYOFF: 0x0002  // Hard Off
                        // BUTTON_MANUALOPHEAT: 0x0004  // Forced Heating; BUTTON_MANUALOPCOOL  0x0008  // Forced Cooling
                        // BUTTON_CYCLE: 0x0010  // Forced Cycling; BUTTON_RUN: 0x0020  // Run with IMM
                        byte mask = AOUInputParser.HighByte(stateData.UIButtons);
                        byte state = AOUInputParser.LowByte(stateData.UIButtons);
                        switch (mask)
                        {
                            case 0x01: currentUIButtons.OnOffButton = (AOUDataTypes.ButtonState)state; break;
                            case 0x02: currentUIButtons.ButtonEmergencyOff = (AOUDataTypes.ButtonState)state; break;
                            case 0x04: currentUIButtons.ButtonForcedHeating = (AOUDataTypes.ButtonState)state; break;
                            case 0x08: currentUIButtons.ButtonForcedCooling = (AOUDataTypes.ButtonState)state; break;
                            case 0x10: currentUIButtons.ButtonForcedCycling = (AOUDataTypes.ButtonState)state; break;
                            case 0x20: currentUIButtons.ButtonRunWithIMM = (AOUDataTypes.ButtonState)state; break;
                            default: break;
                        }
                        // ToDo: Send message changed
                    }

                    if (!AOUDataTypes.IsUInt16NaN(stateData.Energy))
                    {
                        // <Energy>MMSS</Energy>, 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”).
                        currentEnergy = stateData.Energy;
                    }

                    if (stateData.Mode < Int16.MaxValue)
                    {
                        // <Mode>1</Mode> (int); 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”). Which???
                        // #define HT_STATE_INVALID: -999; #define HT_STATE_COLD: -1; #define HT_STATE_UNKNOWN: 0; #define HT_STATE_HOT 1
                        Int16 mode = stateData.Mode;
                        currentMode = (AOUDataTypes.HT_StateType)mode;
                    }

                }
                else if (nextTag == "seq")
                {
                    newLogMessages.Add(new AOULogMessage(GetTime_ms(), "seq:" + tagContent, 0, 0));
                }
                else if (nextTag == AOUInputParser.tagLog)
                {
                    if (AOUInputParser.ParseLog(tagContent, out time_ms, out logMsg))
                    {
                        AOULogMessage msg = new AOULogMessage(time_ms*100, logMsg);
                        if (msg.prio == 0) msg.prio = 1;
                        newLogMessages.Add(msg);
                    }
                }
                else if (nextTag.Length > 0)
                {
                    newLogMessages.Add(new AOULogMessage(GetTime_ms(), "Unknown tag:" + nextTag + " = " + tagContent, 0, 0));
                }


                if (AOUInputParser.ValidPowerTag(nextTag))
                {
                    if (IsTempData)
                    {
                        newPowerValues.Add(tempPower);
                    }
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
