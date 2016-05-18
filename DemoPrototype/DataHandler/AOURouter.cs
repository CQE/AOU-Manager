﻿using System;
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

        public enum RunType { None, Serial, File, Random};
        public RunType runMode {get; private set;}

        private AOUData aouData;
        private AOULogFile aouLogFile ;

        private string notInitializedStr = "AOU Router not initialized";

        public AOURouter()
        {
            logMessages = new List<AOULogMessage>();
            powerValues = new List<Power>();

            runMode = RunType.None;
        }

        ~AOURouter()
        {
            Stop();
            aouData = null;
        }


        public void Initialize(RunType runType, Object settings)
        {
            powerValues.Clear();
            logMessages.Clear();

            aouLogFile = new AOULogFile(DateTime.Now);

            if (runType == RunType.Random)
            {
                aouData = new AOURandomData((AOUSettings.RandomSetting)settings);
            }
            else if (runType == RunType.Serial)
            {
                aouData = new AOUSerialData((AOUSettings.SerialSetting)settings);
            }
            else if (runType == RunType.File)
            {
                aouData = new AOUFileData((AOUSettings.FileSetting)settings);
            }
            /*
            else if (runMode == RunType.Remote)
            {
                aouData = new AOUFileData((AOUSettings.RemoteSetting)settings);
            }
            */
            else
            {
                aouData = null;
            }
        }

        public bool IsInitialized
        {
            get { return aouData != null; }
        }

        public bool IsConnected
        {
            get { return IsInitialized && aouData.Connected; }
        }

        public void Start()
        {
            if (IsInitialized && !IsConnected)
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
            if (!IsInitialized)
            {
                return notInitializedStr;
            }
            else
            {
                return aouData.GetDataLogText();
            }
        }

        public string GetRawData()
        {
            if (IsConnected)
                return aouData.GetRawData();
            else
                return "";
        }

        // Send data
        public bool SendToPlc(string text)
        {
            if (IsConnected)
            {
                logMessages.Add(new AOULogMessage(aouData.GetAOUTime_ms(), "SendToPlc: " + text, 12, 0));
                return aouData.SendData(text);
            }
            else
            {
                return false;
            }
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
            if (!IsConnected) return; // Can not update data

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
            var lastPowerValues = powerValues; // Take last. Can be updateded when running this method
            int lastPowerCount = lastPowerValues.Count;

            List<Power> powerList = new List<Power>(); // New Power list to return
            int firstNullIndex = -1;

            int numValues = count;
            if (numValues > lastPowerCount)
            {
                numValues = lastPowerCount; // Have not count values
            }

            // Add existng values
            for (int i = 0; i < numValues; i++)
            {
                powerList.Add(powerValues[lastPowerCount - numValues + i]); // Add last power values. Not more then count
            }

            // Add dummy values with expected time
            if (numValues < count)
            {
                for (int i = numValues; i < count; i++)
                {
                    powerList.Add(new Power(0));
                }
                firstNullIndex = GetFirstNullIndex(powerList);

                if (firstNullIndex > 1 && firstNullIndex < powerList.Count) // Minimum number of real values to calculate time between
                {

                    powerList = RecalcTime(powerList, firstNullIndex, defaultTimeBetween);
                }
            }

            NewPowerValuesAvailable = 0;

            lastPowerIndex = firstNullIndex == -1 ? count - 1 : firstNullIndex - 1;

            return powerList;
        }

        public List<Power> GetLastNewPowerValues()
        {
            var lastPowerValues = powerValues; // Take last. Can be updateded when running this method
            int lastPowerCount = lastPowerValues.Count;

            List<Power> powerList = new List<Power>();
            powerList.AddRange(lastPowerValues.GetRange(lastPowerCount - NewPowerValuesAvailable, NewPowerValuesAvailable));

            NewPowerValuesAvailable = 0;

            return powerList;
        }

        private static bool IsDummyValue(Power power)
        {
            return double.IsNaN(power.THotTank) || power.THotTank < 1;
        }

        public static int GetFirstNullIndex(List<Power> powerList)
        {
            // Get first dummy values. IsNan values
            for (int i = 0; i < powerList.Count; i++)
            {
                if (IsDummyValue(powerList[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public List<Power> RecalcTime(List<Power> powerList, int firstNullIndex, long defaultTimeBetween)
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
            long time = 0;
            if (IsInitialized)
            {
                time = aouData.GetAOUTime_ms();
            }
            logMessages.Add(new AOULogMessage(time, text, prio, 0));
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
            if (IsConnected && aouData.isUIButtonsChanged)
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
            if (IsConnected && aouData.isIMMChanged)
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
            if (IsConnected && aouData.isModesChanged)
            {
                aouData.isModesChanged = false;
                mode = aouData.currentMode;
                return true;
            }
            return false;
        }


    }
}
