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

        /*
            -- Set value --
            <cmd><"ParameterName">value</"ParameterName"></cmd>

            -- Ask for value --
            <cmd><"ParameterName"></"ParameterName"></cmd>

            -- Get command return with value when set or ask for value --
            <ret><Time>time_ds</Time><"ParameterName">value</"ParameterName"></ret>
        */
        public struct CommandReturn
        {
            public long time_ms;
            public string parameter;
            public string value;

            public CommandReturn(long time, string param, string val)
            {
                time_ms = time;
                parameter = param;
                value = val;
            }
        }

        private List<CommandReturn> CommandReturns;

        public bool isIMMChanged = false;
        public bool isUIButtonsChanged = false;
        public bool isModesChanged = false;

        bool UpdateDataRunning = false;

        private DateTime lastDataRealTime;
        private long lastDataTime_ms;

        protected DateTime startTime;

        public bool Connected { get; protected set; }

        protected AOUSettings.DebugMode debugMode;


        // Base constructor
        protected AOUData(AOUSettings.DebugMode dbgMode)
        {
            Connected = false;
            debugMode = dbgMode;

            currentSeqState = AOUDataTypes.StateType.NOTHING;

            newLogMessages = new List<AOULogMessage>();
            newPowerValues = new List<Power>();
            CommandReturns = new List<CommandReturn>();

            startTime = DateTime.Now;
            lastDataRealTime = startTime;
            lastDataTime_ms = -1;
        }

        /* Virtual functions to be overrided by child classes */
        public virtual void Connect()
        {
            newLogMessages.Clear();
            newPowerValues.Clear();
            CommandReturns.Clear();

            startTime = DateTime.Now;
            lastDataRealTime = startTime;
            lastDataTime_ms = -1;

            currentSeqState = AOUDataTypes.StateType.NOTHING;

            Connected = true;
        }

        public virtual void Disconnect()
        {
            Connected = false;
        }

        protected virtual string GetTextData()
        {
            // Receive latest text from data source. Must be overrided by child classes
            throw new Exception("AOUData.GetTextData Not overrided");
        }

        public virtual bool SendData(string data)
        {
            // Send text to data source. Must be overrided by child classes
            throw new Exception("AOUData.SendData Not overrided");
        }

        public virtual void UpdateData()
        {
            // Update newLogMessages and newPowerValues by child instances
            throw new Exception("AOUData.UpdateData Not overrided");
        }


        #region bit methods
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

        protected bool IsStateSet(byte state, byte mask)
        {
            int res = state & mask;
            return (res != 0);
        }

        #endregion
 
        #region Public methods

        /* Log end error tracking */
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

        // Get raw text data
        public string GetRawData()
        {
            return GetTextData();
        }

        /* Get new Power values */
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

        /* Get new log messages */
        public bool AreNewLogMessagesAvailable()
        {
            return newLogMessages.Count > 0;
        }

        public bool AreNewCommandReturnsAvailable()
        {
            return CommandReturns.Count > 0;
        }

        public CommandReturn GetNextCommandReturn()
        {
            if (CommandReturns.Count > 0)
            {
                CommandReturn ret = CommandReturns[0];
                CommandReturns.RemoveAt(0);
                return ret;
            }
            return new CommandReturn();
        }

        public bool GetNextCommandReturnStrValue(AOUDataTypes.CommandType cmd, out string value)
        {
            value = "";
            string str = GlobalVars.aouCommands.StringValue(cmd);
            int n = CommandReturns.FindIndex(0, CommandReturns.Count, cr => cr.parameter == str);
            if (n >= 0)
            {
                CommandReturn ret = CommandReturns[n];
                CommandReturns.RemoveAt(n);
                value = ret.value;
                return true;
            }
            return false;
        }

        public bool GetNextCommandReturnIntValue(AOUDataTypes.CommandType cmd, out int value)
        {
            string strval = "";
            value = 0;
            return GetNextCommandReturnStrValue(cmd, out strval) && int.TryParse(strval, out value);
        }

        public AOULogMessage[] GetNewLogMessages()
        {
            AOULogMessage[] logs = newLogMessages.ToArray();
            newLogMessages.Clear();
            return logs;
        }

        // Used to get valid time for log messages when time stamp is missing
        public long GetAOUTime_ms()
        {
            // TimeSpan.Zero;
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - lastDataRealTime.Ticks);
            long ms = lastDataTime_ms + (long)(ts.TotalMilliseconds);
            return AOUHelper.ToCurTimeStep(ms, curTimeSpan_ms);
        }

        #endregion

        #region protected help methods
        /* Protected methods to be used by child classes */

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

        // Convert to double
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

        // Converting special types
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

        protected bool GetStateData(string tagContent, long time_ms, out Power power)
        {
            AOUStateData stateData;
            AOUInputParser2.ParseState(tagContent, out stateData);

            power = new Power();

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

            //if (stateData.RetForTemp < 1000 && stateData.RetForTemp > -100) //no new value if hotvalve and coldvalve is low
            if (currentHotValve != 18 && currentColdValve != 26 )
            {
                //lastTReturnForecasted = GetValidDoubleValue(stateData.RetForTemp);
                lastTReturnForecasted = GetValidDoubleValue(stateData.retTemp);
                newLastTReturnForecasted = true;
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

            if (stateData.hotTankTemp < 1000) // Only temperature data. ToDo better test
            {
                // power.ElapsedTime = AOUDataTypes.AOUModelTimeSecX10_to_TimeMs(stateData.time_hours, stateData.time_sek_x_10_of_hour);
                power.ElapsedTime = time_ms; // Use same time for all data

                power.THotTank = GetValidDoubleValue(stateData.hotTankTemp);
                power.TColdTank = GetValidDoubleValue(stateData.coldTankTemp);
                power.TReturnValve = GetValidDoubleValue(stateData.retTemp);

                if (newLastTReturnForecasted)
                {
                    power.TReturnForecasted = lastTReturnForecasted;
                    newLastTReturnForecasted = false;
                }
                else
                {
                    power.TReturnForecasted = -100; //just temporary
                }

                power.TReturnActual = GetValidDoubleValue(stateData.retTemp);

                power.TBufferCold = GetValidDoubleValue(stateData.bufColdTemp);
                power.TBufferMid = GetValidDoubleValue(stateData.bufMidTemp);
                power.TBufferHot = GetValidDoubleValue(stateData.bufHotTemp);

                power.State = currentSeqState;

                power.ValveFeedCold = currentColdValve;
                power.ValveFeedHot = currentHotValve;
                power.ValveReturn = currentReturnValve;
                power.ValveCoolant = currentCoolantValve;

                //We use TReturnForecasted for TRetFlowActive


                power.THeaterOilOut = GetValidDoubleValue(stateData.heaterTemp);

                power.PowerHeating = currentPower;

                power.THeatExchangerCoolantOut = GetValidDoubleValue(stateData.coolerTemp);

                return true; // Only add new power if temperature data
            }
            return false;
        }

        protected string GetSeqString(string tagContent)
        {
            // <seq><Time>81</Time><State>SQ_INITIAL</State><Cycle>-1</Cycle><Descr>...</Descr><Leave>IMM</Leave></seq>
            string state;
            string cycle;
            string descr;
            string leave;
            AOUTagParser.ParseString("State", tagContent, out state);
            AOUTagParser.ParseString("Cycle", tagContent, out cycle);
            AOUTagParser.ParseString("Descr", tagContent, out descr);
            AOUTagParser.ParseString("Leave", tagContent, out leave);
            //string retStr = "seq:" + tagContent;
            string retStr = state + " cycle:" + cycle + " leave " + leave;
            if (descr != "...")
            {
                retStr += ", " + descr;
            }
            return retStr;
        }

        // Main converting loop. XML Text to Power and Log message lists
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

            newPowerValues = new List<Power>();
            newLogMessages = new List<AOULogMessage>();

            string textDataStream = GetTextData();

            if (HaveLogs())
            {
                string dataLogText = GetDataLogText();
                string[] logList = dataLogText.Split('\n');
                {
                    foreach (string logLine in logList)
                    {
                        string text = logLine.Trim();
                        if (text.Length > 0)
                        {
                            newLogMessages.Add(new AOULogMessage(GetAOUTime_ms(), text));
                        }
                    }
                }
            }

            string nextLines = "-"; // Can not be empty for next statement;
            while (textDataStream.Length > 0)
            {
                List<string> loglines;

                string tagContent;
                string nextTag = AOUInputParser2.GetNextTag(textDataStream, out time_ms, out tagContent, out loglines, out nextLines);
                textDataStream = nextLines;

                // Save last AOU time for adding log messages without time
                lastDataRealTime = DateTime.Now;

                if (loglines.Count > 0 )
                {
                    // Add text lines without tags as log messages
                    foreach (string log in loglines)
                    {
                        newLogMessages.Add(new AOULogMessage(GetAOUTime_ms(), log, 8, 0));
                    }
                }
                else if (time_ms > lastDataTime_ms)
                {
                    //newLogMessages.Add(new AOULogMessage(GetAOUTime_ms(), nextTag +" diff ms: " + (time_ms - lastDataTime_ms).ToString())); // For testing time between
                    lastDataTime_ms = time_ms;
                }
                else if (lastDataTime_ms > time_ms)
                {
                    newLogMessages.Add(new AOULogMessage(GetAOUTime_ms(), "New time " + time_ms + " is less then last time " + lastDataTime_ms));
                }

                if (nextTag == AOUInputParser2.tagRetValue)
                {
                    string parameterTag = "";
                    string parameterText = tagContent.Substring(tagContent.IndexOf("</Time>") + 7); // Parameter tag pair is after time tag pair
                    int tagEndPos = 0;
                    if (AOUTagParser.GetTagAndContent(parameterText, out parameterTag, out tagContent, out tagEndPos))
                    {
                        CommandReturns.Add(new CommandReturn(time_ms, parameterTag, tagContent)); 
                    }
                }
                else if (nextTag == AOUInputParser2.tagState)
                {
                    Power power;
                    // When power data have temperature data add to power list else save other data as current values
                    if (GetStateData(tagContent, time_ms, out power))
                    {
                        //if none of HotFeedValve and ColdFeelValve is on, replace new TRetCalculated with last
                        if (power.ValveFeedHot == 18 && power.ValveFeedCold == 26)  //18 and 26 = low, 26 and 32 = high
                        {
                            //skip this point
                            power.TReturnForecasted = lastTReturnForecasted -2 ; // double.NaN;
                        }
                        else
                        {
                            power.TReturnForecasted = power.TReturnActual-2;
                        }
                        //manipulate THeatExchangerCoolantOut
                        power.THeatExchangerCoolantOut = power.THotTank - power.TBufferHot;
                        //manipulate 
                        power.THeaterOilOut = power.TBufferCold - power.TColdTank;
                        newPowerValues.Add(power);
                    }
                }
                else if (nextTag == "seq")
                {
                    /* Just log message for "seq" tag */
                    newLogMessages.Add(new AOULogMessage(time_ms, GetSeqString(tagContent), 3));
                }
                else if (nextTag == AOUInputParser2.tagLog)
                {
                    string logMsg = ""; int pid = 0; int prio = 0;
                    if (AOUInputParser2.ParseLog(tagContent, out logMsg, out prio, out pid))
                    {
                        newLogMessages.Add(new AOULogMessage(time_ms, logMsg, prio, pid));
                    }
                }
                else if (nextTag.Length > 0)
                {
                    time_ms = time_ms == 0 ? GetAOUTime_ms() : time_ms;
                    // Unknow tag. Add log message
                    newLogMessages.Add(new AOULogMessage(time_ms, "Unknown tag:" + nextTag + " = " + tagContent, 1));
                }

            }
            UpdateDataRunning = false;
        }
        #endregion
    }
}
