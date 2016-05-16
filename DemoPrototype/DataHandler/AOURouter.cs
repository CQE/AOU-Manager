using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DemoPrototype
{
    public class AOURouter
    {
        public const int MaxTotalValuesInMemory = 300;
        public const int MaxTotalLogMessagesInMemory = 1000;

        private List<AOULogMessage> logMessages;
        private List<Power> powerValues;
 
        public int NewPowerValuesAvailable { get; private set; }
        public int NewLogMessagesAvailable { get; private set; }

        private DateTime startTime;

        public enum RunType { None, Serial, File, Random};
        public RunType runMode {get; private set;}

        private AOUData aouData;
        private AOULogFile aouLogFile;

        private string applogstr = "AOURouter. No run mode selected";

        public AOURouter()
        {
            logMessages = new List<AOULogMessage>();
            powerValues = new List<Power>();

            runMode = RunType.None;
            startTime = DateTime.Now;
            aouLogFile = new AOULogFile(startTime);
        }

        public AOURouter(AOUSettings.RandomSetting randomSetting) : this()
        {
            runMode = RunType.Random;
            aouData = new AOURandomData(randomSetting);
       }

        public AOURouter(AOUSettings.FileSetting fileSetting) : this()
        {
            runMode = RunType.File;
            aouData = new AOUFileData(fileSetting);
        }

        public AOURouter(AOUSettings.SerialSetting serialSetting, AOUSettings.DebugMode dbgMode) : this()
        {
            runMode = RunType.Serial;
            aouData = new AOUSerialData(serialSetting, dbgMode);
        }

        public AOURouter(AOUSettings.RemoteSetting remoteSetting) : this()
        {
            runMode = RunType.Serial;
            aouData = new AOURemoteData(remoteSetting);
        }

        ~AOURouter()
        {
            Stop();
            aouData = null;
        }


        public bool IsInitiated
        {
            get { return aouData != null; }
        }

        public bool IsConnected
        {
            get { return IsInitiated && aouData.Connected; }
        }

        public void Start()
        {
            if (IsInitiated && !IsConnected)
            {
                aouData.Connect();
            }
        }

        public void Stop()
        {
            if (IsConnected)
            {
                aouData.Disconnect();
            }
        }

        public string GetLogStr()
        {
            if (IsConnected)
            {
                return aouData.GetDataLogText();
            }
            else
            {
                string text = applogstr;
                return text;
            }

        }

        public string GetRawData()
        {
            if (aouData != null)
                return aouData.GetRawData();
            else
                return "";
        }

        // Send data
        public bool SendToPlc(string text)
        {
            logMessages.Add(new AOULogMessage(aouData.GetAOUTime_ms(), "SendToPlc: " + text, 12, 0));
            if (aouData != null)
                return aouData.SendData(text);
            else
                return false;
        }

        public void SendTagCommandToPlc(string subTag, int value)
        {
            SendToPlc(String.Format("<cmd><{0}>{1}</{0}></cmd>", subTag, value));
        }

        public void SendCommandToPlc(AOUDataTypes.CommandType cmd, int value)
        {
            /*
                (temperature in C)	<cmd><tempHotTankFeedSet>195</tempHotTankFeedSet></cmd>	
                (temperature in C)	<cmd><tempColdTankFeedSet>25</tempColdTankFeedSet></cmd>	
                (s/cycle)	        <cmd><coolingTime>15</coolingTime></cmd>	
                (s/cycle)	        <cmd><heatingTime>10</heatingTime></cmd>	
                (s/cycle)	        <cmd><toolHeatingFeedPause>5</toolHeatingFeedPause></cmd>	
            */

            switch (cmd)
            {
                case AOUDataTypes.CommandType.tempHotTankFeedSet:
                    SendTagCommandToPlc("tempHotTankFeedSet", value); break;
                case AOUDataTypes.CommandType.tempColdTankFeedSet:
                    SendTagCommandToPlc("tempColdTankFeedSet", value); break;
                case AOUDataTypes.CommandType.coolingTime:
                    SendTagCommandToPlc("coolingTime", value); break;
                case AOUDataTypes.CommandType.heatingTime:
                    SendTagCommandToPlc("heatingTime", value); break;
                case AOUDataTypes.CommandType.toolHeatingFeedPause:
                    SendTagCommandToPlc("toolHeatingFeedPause", value); break;
                default:
                    SendTagCommandToPlc(cmd.ToString(), value); break;
            }
        }

        // Update data. Get new Power values and Log messages
        public void Update()
        {
            if (aouData == null) return;

            aouData.UpdateData();

            if (aouData.AreNewValuesAvailable())
            {
                var newValues = aouData.GetNewValues();
                for (int i = 0; i < newValues.Length; i++)
                {
                    powerValues.Add(newValues[i]); // Add new value
                    NewPowerValuesAvailable++;
                    if (powerValues.Count > MaxTotalValuesInMemory)
                    {
                        powerValues.RemoveAt(0); // Delete first Power values
                    }
                }
            }

            if (aouData.AreNewLogMessagesAvailable())
            {
                var logs = aouData.GetNewLogMessages();
                AddLogToFile(logs); // Save new log messages to log file
                for (int i = 0; i < logs.Length; i++)
                {
                    logMessages.Add(logs[i]);
                    NewLogMessagesAvailable++;
                    if (logMessages.Count > MaxTotalLogMessagesInMemory)
                    {
                        logMessages.RemoveAt(0); // Delete first Log message
                    }
                }
            }
        }

        /**************************
          Power values handling
       **************************/
        public List<Power> GetLastPowerValues(int count, out int lastPowerIndex, int defaultTimeBetween)
        {
            List<Power> powerList = new List<Power>();
            int firstNullIndex = -1;

            int numValues = count;
            if (numValues > powerValues.Count)
            {
                numValues = powerValues.Count;
            }

            // Add existng values
            for (int i = 0; i < numValues; i++)
            {
                powerList.Add(powerValues[powerValues.Count - numValues + i]);
            }

            // Add dummy values with expected time
            if (numValues < count)
            {
                for (int i = numValues; i < count; i++)
                {
                    powerList.Add(new Power(0));
                }
                powerList = RecalcTime(powerList, new Power(0), out firstNullIndex, defaultTimeBetween);
            }

            NewPowerValuesAvailable = 0;

            lastPowerIndex = firstNullIndex == -1 ? count - 1 : firstNullIndex - 1;

            return powerList;
        }

        /*
        public Power GetLastNewPowerValues()
        {
            Power power  = new Power(0);
            if (NewPowerValuesAvailable > 0 && powerValues.Count > 1)
            {
                NewPowerValuesAvailable = 0;
                power = powerValues[powerValues.Count - 1];
            }
            return power; 
        }
        */

        public List<Power> GetLastNewPowerValues()
        {
            List<Power> powerList = new List<Power>();
            if (NewPowerValuesAvailable > 0 && powerValues.Count > 1)
            {
                powerList.AddRange(powerValues.GetRange(powerValues.Count- NewPowerValuesAvailable, NewPowerValuesAvailable));
                NewPowerValuesAvailable = 0;
            }
            return powerList;
        }

        private static bool IsDummyValue(Power power)
        {
            return double.IsNaN(power.THotTank) || power.THotTank < 1;
        }

        private static int GetFirstNullIndex(List<Power> powerList)
        {
            // Get first dummy values. IsNan values
            for (int i = 0; i < powerList.Count; i++)
            {
                if (IsDummyValue(powerList[i]))
                {
                    return i;
                }
            }
            return powerList.Count - 1;
        }

        public List<Power> RecalcTime(IEnumerable<Power> powers, Power newPower, out int firstNullIndex, long defaultTimeBetween)
        {
            List<Power> powerList = powers.ToList();
            firstNullIndex = GetFirstNullIndex(powerList);

            // Replace new value in first dummy index if new Power
            if (!IsDummyValue(newPower))
            {
                powerList[firstNullIndex] = newPower;
                firstNullIndex++;
            }

            if (firstNullIndex > 1 && firstNullIndex < powerList.Count) // Minimum number of real values to calculate time between
            {
                // Replace time in dummy values with expected time values
                long diff = powerList[firstNullIndex - 1].ElapsedTime - powerList[0].ElapsedTime; // time span between first and last real time
                if (diff > (100 * firstNullIndex)) // minimum difference in time accepted
                {
                    long newTimeBetween = diff / (firstNullIndex - 1); // Average value
                    long time = powerList[firstNullIndex - 1].ElapsedTime; // last real time
                    for (int i = firstNullIndex; i < powerList.Count; i++)
                    {
                        time += newTimeBetween; // next timestamp
                        Power pow = powerList[i];
                        pow.ElapsedTime = time; // replace time with new timestamp
                        powerList[i] = pow;
                    }
                }
            }
            return powerList;
        }

        /**************************
            Log Message Handling
        **************************/
        public List<AOULogMessage> GetLastLogMessages(int count)
        {
            if (logMessages.Count > 0)
            {
                if (count > logMessages.Count)
                {
                    count = logMessages.Count;
                }
                NewLogMessagesAvailable = 0;
                return logMessages.GetRange(logMessages.Count - count, count);
            }
            else
            {
                return new List<AOULogMessage>();
            }
        }

        public List<AOULogMessage> GetNewLogMessages()
        {
            if (NewLogMessagesAvailable > 0)
            {
                var logs = logMessages.GetRange(logMessages.Count - NewLogMessagesAvailable, NewLogMessagesAvailable);
                NewLogMessagesAvailable = 0;
                return logs;
            }
            else
            {
                return new List<AOULogMessage>();
            }
        }

        // Create app log message
        public void CreateLogMessage(string text, int prio)
        {
            logMessages.Add(new AOULogMessage(aouData.GetAOUTime_ms(), text, prio, 0));
        }

        // Save to files in Image folder
        private void AddLogToFile(AOULogMessage[] logs) 
        {
            aouLogFile.AddLogMessages(logs);
        }

        /**********************************************/
        // Are UI buttons, IMM and Mode states changed
        /**********************************************/
        public bool UIButtonsChanged(out AOUDataTypes.UI_Buttons buttons)
        {
            buttons = new AOUDataTypes.UI_Buttons();
            if (aouData.isUIButtonsChanged)
            {
                aouData.isUIButtonsChanged = false;
                buttons = aouData.currentUIButtons;
                return true;
            }
            return false;
        }

        public bool IMMChanged(out AOUDataTypes.IMMSettings mode)
        {
            mode = new AOUDataTypes.IMMSettings();
            if (aouData.isIMMChanged)
            {
                aouData.isIMMChanged = false;
                mode = aouData.currentIMMState;
                return true;
            }
            return false;
        }

        public bool ModeChanged(out AOUDataTypes.HT_StateType mode)
        {
            mode = AOUDataTypes.HT_StateType.HT_STATE_NOT_SET;
            if (aouData.isModesChanged)
            {
                aouData.isModesChanged = false;
                mode = aouData.currentMode;
                return true;
            }
            return false;
        }


    }
}
