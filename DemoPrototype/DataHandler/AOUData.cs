using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    public class AOUData
    {
        private const double curTimeSpan_ms = 100; // For adjusting to nearest time

        public const int VALVE_HOT = 0x01;
        public const int VALVE_COLD = 0x02;
        public const int VALVE_RET = 0x04;
        public const int VALVE_COOL = 0x10;

        public const int BUTTON_ONOFF = 0x01;  // Soft on/Off;
        public const int BUTTON_EMERGENCYOFF = 0x02;  // Hard Off
        public const int BUTTON_MANUALOPHEAT = 0x04;  // Forced Heating; 
        public const int BUTTON_MANUALOPCOOL = 0x08;  // Forced Cooling
        public const int BUTTON_CYCLE = 0x10;  // Forced Cycling; 
        public const int BUTTON_RUN = 0x0020;  // Run with IMM

        private string dataLogStr = "";
        private string dataErrStr = "";

        protected List<AOULogMessage> newLogMessages;
        protected List<Power> newPowerValues;

        protected AOUDataTypes.StateType currentSeqState;
        protected int currentHotValve = GlobalVars.globValveChartValues.HotValveLow;
        protected int currentColdValve = GlobalVars.globValveChartValues.ColdValveLow;
        protected int currentReturnValve = GlobalVars.globValveChartValues.ReturnValveLow;
        protected int currentCoolantValve = GlobalVars.globValveChartValues.CoolantValveLow; 

        protected int currentPower = 0;
        protected uint currentEnergy = 0;

        protected double lastTReturnForecasted = double.NaN;
        protected bool newLastTReturnForecasted = false;

        public AOUDataTypes.UI_Buttons currentUIButtons = new AOUDataTypes.UI_Buttons();
        public AOUDataTypes.HT_StateType currentMode = AOUDataTypes.HT_StateType.HT_STATE_NOT_SET;
        public AOUDataTypes.IMMSettings currentIMMState = AOUDataTypes.IMMSettings.Nothing;

        public bool isIMMChanged = false;
        public bool isUIButtonsChanged = false;
        public bool isModesChanged = false;

        bool UpdateDataRunning = false;

        private DateTime lastDataRealTime;
        private long lastDataTime_ms;

        protected DateTime startTime;

        public bool Connected { get; protected set; }

        protected AOUSettings.DebugMode debugMode;


        protected Byte LowByte(UInt16 word)
        {
            return (Byte)(word & 0xff);
        }

        protected Byte HighByte(UInt16 word)
        {
            return (Byte)(word >> 8);
        }

        protected UInt16 CombineToWord(byte mask, byte mode)
        {
            UInt16 word = (UInt16)(mask << 8);
            word += mode;
            return word;
        }

        protected byte AddBit(byte data, int bitnr)
        {
            int bit = 1;
            int value = bit << bitnr;
            value |= data;
            return (byte)value;
        }

        protected byte DelBit(byte data, int bitnr)
        {
            int bit = 1;
            int value = data;
            int mask = bit << bitnr;
            value &= ~mask;
            return (byte)value;
        }

        protected int BitNum(byte value)
        {
            int val = value;
            int num = 0;
            while ((val = val >> 1) != 0)
            {
                ++num;
            }
            return num;
         }

        protected AOUData(AOUSettings.DebugMode dbgMode)
        {
            Connected = false;
            debugMode = dbgMode;

            currentSeqState = AOUDataTypes.StateType.NOTHING;

            newLogMessages = new List<AOULogMessage>();
            newPowerValues = new List<Power>();

            startTime = DateTime.Now;
            lastDataRealTime = startTime;
            lastDataTime_ms = 0;
        }

        public long GetAOUTime_ms()
        {
           // TimeSpan.Zero;
 
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - lastDataRealTime.Ticks);
            long ms = lastDataTime_ms + (long)(ts.TotalMilliseconds);
            return AOUHelper.ToCurTimeStep(ms, curTimeSpan_ms);
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

        protected double GetValidDoubleValue(UInt16 value)
        {
            if (AOUDataTypes.IsUInt16NaN(value))
            {
                return double.NaN;
            }
            else
            {
                return value;
            }
        }

        protected double GetValidDoubleValue(Int16 value)
        {
            if (AOUDataTypes.IsInt16NaN(value))
            {
                return double.NaN;
            }
            else
            {
                return value;
            }
        }

        protected bool IsStateSet(byte state, byte mask)
        {
            int res = state & mask;
            return (res != 0);
        }

        protected AOUDataTypes.ButtonState GetButtonState(byte state, byte mask)
        {
            if (IsStateSet(state, mask))
            {
                return AOUDataTypes.ButtonState.on;
            }
            else
            {
                return AOUDataTypes.ButtonState.off;
            }
         }

        protected int GetValveState(byte state, byte mask)
        {
            if (IsStateSet(state, mask))
            {
                switch (mask)
                {
                    case VALVE_HOT: return GlobalVars.globValveChartValues.HotValveHi;
                    case VALVE_COLD: return GlobalVars.globValveChartValues.ColdValveHi;
                    case VALVE_RET: return GlobalVars.globValveChartValues.ReturnValveHi;
                    case VALVE_COOL: return GlobalVars.globValveChartValues.CoolantValveHi;
                    default: return 999; // Must have default value. Error if reached.
                }
            }
            else
            {
                switch (mask)
                {
                    case VALVE_HOT: return GlobalVars.globValveChartValues.HotValveLow;
                    case VALVE_COLD: return GlobalVars.globValveChartValues.ColdValveLow;
                    case VALVE_RET: return GlobalVars.globValveChartValues.ReturnValveLow;
                    case VALVE_COOL: return GlobalVars.globValveChartValues.CoolantValveLow;
                    default: return 999;
                }
            }
        }

        protected void GetTextDataList()
        {
            if (UpdateDataRunning)
            {
                return;
            }

            UpdateDataRunning = true;
            long time_ms = 0;

            if (this.debugMode == AOUSettings.DebugMode.rawData)
            {
                return;
            }

            AOUStateData stateData;

            newPowerValues = new List<Power>();
            newLogMessages = new List<AOULogMessage>();
            // newReturnValveValues = new List<ReturnValve>();

            string textDataStream = GetTextData();
            int prevTextLength = textDataStream.Length;

            while (prevTextLength > 0)
            {
                Power tempPower = new Power(0);
                // ReturnValve tempReturnValve = new ReturnValve(0);
                bool IsTempData = false;
                // bool IsReturnValveData = false;

                int count = 0;
                string tagContent;
                List<string> loglines;

                string nextTag = AOUInputParser.GetNextTag(textDataStream, out time_ms, out tagContent, out loglines, out count);

                // Save last AOU time
                lastDataRealTime = DateTime.Now;
                if (time_ms > lastDataTime_ms)
                {
                    lastDataTime_ms = time_ms;
                }

                // Add text lines without tags as log message
                foreach (string log in loglines)
                {
                   newLogMessages.Add(new AOULogMessage(GetAOUTime_ms(), log, 8, 0));
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
                        // -- VALVES -- <Valves>MMSS</Valves> MASK (e.g. “3F”), STATE Bits: 0/Hot valve, 1/Cold valve, 2/Return valve, 4/Coolant valve
                        byte mask = HighByte(stateData.Valves);
                        byte state = LowByte(stateData.Valves);

                        if (IsStateSet(mask, VALVE_HOT)) currentHotValve = GetValveState(state, VALVE_HOT);
                        if (IsStateSet(mask, VALVE_COLD)) currentColdValve = GetValveState(state, VALVE_COLD);
                        if (IsStateSet(mask, VALVE_RET)) currentReturnValve = GetValveState(state, VALVE_RET);
                        if (IsStateSet(mask, VALVE_COOL)) currentCoolantValve = GetValveState(state, VALVE_COOL);
                    }

                    if (stateData.RetForTemp < 1000 && stateData.RetForTemp > -100) 
                    {
                        lastTReturnForecasted = GetValidDoubleValue(stateData.RetForTemp);
                        newLastTReturnForecasted = true;
                    }

                    if (stateData.hotTankTemp < 1000) // Only temperature data. ToDo better test
                    {
                        tempPower.ElapsedTime = AOUDataTypes.AOUModelTimeSecX10_to_TimeMs(stateData.time_hours, stateData.time_sek_x_10_of_hour);

                        tempPower.THotTank = GetValidDoubleValue(stateData.hotTankTemp);
                        tempPower.TColdTank = GetValidDoubleValue(stateData.coldTankTemp);
                        tempPower.TReturnValve = GetValidDoubleValue(stateData.retTemp);

                       // if (newLastTReturnForecasted)
                        {
                            tempPower.TReturnForecasted = lastTReturnForecasted;
                            newLastTReturnForecasted = false; 
                        }

                        tempPower.TReturnActual = GetValidDoubleValue(stateData.retTemp);
 
                        tempPower.TBufferCold = GetValidDoubleValue(stateData.bufColdTemp);
                        tempPower.TBufferMid = GetValidDoubleValue(stateData.bufMidTemp);
                        tempPower.TBufferHot = GetValidDoubleValue(stateData.bufHotTemp);

                        tempPower.State = currentSeqState;

                        tempPower.ValveFeedCold = currentColdValve;
                        tempPower.ValveFeedHot = currentHotValve;
                        tempPower.ValveReturn = currentReturnValve;
                        tempPower.ValveCoolant = currentCoolantValve;

                        tempPower.THeaterOilOut = GetValidDoubleValue(stateData.heaterTemp);

                        tempPower.PowerHeating = currentPower;

                        tempPower.THeatExchangerCoolantOut = GetValidDoubleValue(stateData.coolerTemp);

                        IsTempData = true; // Only add new power if temperature data
                    }


                    if (!AOUDataTypes.IsUInt16NaN(stateData.IMM))
                    {
                        // TODO when IMM data
                        // <IMM>MMSS</IMM>, 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”).
                        // IMM_OutIMMError: 0x01; IMM_OutIMMBlockInject: 0x02; IMM_OutIMMBlockOpen: 0x04; IMM_InIMMStop: 0x08;
                        // IMM_InCycleAuto: 0x10; IMM_InIMMInjecting: 0x20; IMM_InIMMEjecting: 0x40; IMM_InIMMToolClosed: 0x80;
                        byte mask = HighByte(stateData.IMM);
                        byte state = LowByte(stateData.IMM);
                        switch (mask)
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
                        isIMMChanged = true;
                    }


                    if (!AOUDataTypes.IsUInt16NaN(stateData.UIButtons))
                    {
                        // UI>MMSS</UI> (hex) MM=8bit mask, SS=8bits. 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”).
                        byte mask = HighByte(stateData.UIButtons);
                        byte state = LowByte(stateData.UIButtons);

                        if (IsStateSet(mask, BUTTON_ONOFF)) currentUIButtons.OnOffButton = GetButtonState(state, BUTTON_ONOFF);
                        if (IsStateSet(mask, BUTTON_EMERGENCYOFF)) currentUIButtons.ButtonEmergencyOff = GetButtonState(state, BUTTON_EMERGENCYOFF);
                        if (IsStateSet(mask, BUTTON_MANUALOPHEAT)) currentUIButtons.ButtonForcedHeating = GetButtonState(state, BUTTON_MANUALOPHEAT);
                        if (IsStateSet(mask, BUTTON_MANUALOPCOOL)) currentUIButtons.ButtonForcedCooling = GetButtonState(state, BUTTON_MANUALOPCOOL);
                        if (IsStateSet(mask, BUTTON_CYCLE)) currentUIButtons.ButtonForcedCycling = GetButtonState(state, BUTTON_CYCLE);
                        if (IsStateSet(mask, BUTTON_RUN)) currentUIButtons.ButtonRunWithIMM = GetButtonState(state, BUTTON_RUN);
                        isUIButtonsChanged = true;
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
                        isModesChanged = true;
                    }

                }
                else if (nextTag == "seq")
                {
                    /* Old tag. Handle ? */
                    newLogMessages.Add(new AOULogMessage(GetAOUTime_ms(), "seq:" + tagContent, 0, 0));
                }
                else if (nextTag == AOUInputParser.tagLog)
                {
                    string logMsg = ""; int pid = 0; int prio = 0;
                    if (AOUInputParser.ParseLog(tagContent, out logMsg, out prio, out pid))
                    {
                        newLogMessages.Add(new AOULogMessage(time_ms, logMsg, prio, pid));
                    }
                }
                else if (nextTag.Length > 0)
                {
                    // Unknow tag. Add log message
                    newLogMessages.Add(new AOULogMessage(GetAOUTime_ms(), "Unknown tag:" + nextTag + " = " + tagContent, 0, 0));
                }


                if (AOUInputParser.ValidPowerTag(nextTag))
                {
                    if (IsTempData)
                    {
                        newPowerValues.Add(tempPower);
                    }
                    /*
                    if (IsReturnValveData)
                    {
                        newReturnValveValues.Add(tempReturnValve);
                    }
                    */
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
            UpdateDataRunning = false;
        }
    }
}
