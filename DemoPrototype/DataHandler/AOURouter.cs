using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DemoPrototype
{
    /*
f.     Visning ”Tool tempering” (HEAT/COOL/IDLE) styrd av FA-DUINO (beräknas löpande av FA-DUINO)
    Funkar inte

g.       Visning ”Running mode” (Heating/Cooling/Fxed cyclig/AOU with IMM/Idle) styrd av FA-DUINO (i.e. knapparna på AOU frontpanel)
    Funkar inte, kan ändra och kommandot skickas. Vi tar ej emot

h.      Visning ”AOU/IMM interaction” styrd av FA-DUINO
    Dessa kom inte förut 

i.         Logg-meddelandena från FA-DUINO visas löpande i logglistan på GUI
    Funkar i debug kraschade nyss i release. Urban tror han har fixat

2.       Önskvärt:

a.       Kunna använda vår display med touch ansluten till denna PC
    Ingen aning hur jag skulle kunna fixa det. Skärmen är hos Urban just nu. Jag kan fråga honom hur det fungerar hos honom.

b.      Läsa av temperaturer från kurvorna
    Ska titta på det

3.       Bra att ha:
    a.       Händelselogg-filer (någon besökare an tänkas fråga)
        Tror textfiler sparas, excel fungerar inte just nu
    */

    public class AOURouter
    {
        public const int MaxTotalValuesInMemory = 1000;
        public const int MaxTotalLogMessagesInMemory = 1000;

        private List<AOULogMessage> logMessages;
        private List<Power> powerValues;

        private Power lastPower;

        public int NewPowerValuesAvailable { get; private set; }

        public int NewLogMessagesAvailable { get; private set; }

        public int TotNumValues
        { get
            {
                return logMessages.Count;
            }
        }

        public int TotNumLogMessages
        { get
            {
                return logMessages.Count;
            }
        }

        private DateTime startTime;

        // Different Run types. File and Random are test modes
        public enum RunType {None, Serial, File, Random, Client};
        public RunType runMode
        {
            get; private set;
        }

        private AOUData aouData;

        private AOULogFile aouLogFile;

        private string applogstr = "AOURouter. No run mode selected";


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
            aouData.Connect();
        }

        public AOURouter(AOUSettings.FileSetting fileSetting) : this()
        {
            runMode = RunType.File;
            aouData = new AOUFileData(fileSetting);
            aouData.Connect();
        }

        public AOURouter(AOUSettings.SerialSetting serialSetting, AOUSettings.DebugMode dbgMode) : this()
        {
            runMode = RunType.Serial;
            aouData = new AOUSerialData(serialSetting, dbgMode);
            aouData.Connect();
        }

        public AOURouter(AOUSettings.RemoteSetting remoteSetting) : this()
        {
            runMode = RunType.Serial;
            aouData = new AOURemoteData(remoteSetting);
            aouData.Connect();
        }

        ~AOURouter()
        {
            Stop();
        }

        public void Stop()
        {
            if (aouData != null)
                aouData.Disconnect();
        }

        public string GetLogStr()
        {
            if (aouData != null)
                return aouData.GetDataLogText();
            else
            { 
                string text = applogstr;
                applogstr = "";
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

        public bool SendToPlc(string text)
        {
            // logMessages.Enqueue(new AOULogMessage(AOUHelper.GetNowToMs(), "SendToPlc: "+ text, 12, 0));
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

        private void AddLogToFile(AOULogMessage[] logs)
        {
            aouLogFile.AddLogMessages(logs);
        }

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

        public long GetTimeBetween(List<Power> powers, long defaultTimeBetween)
        {
            int firstNullTime = -1;
            for (int i = 0; i < powers.Count; i++)
            {
                if (powers[i].ElapsedTime == 0 && i > 0)
                {
                    firstNullTime = i;
                    break;
                }
            }
            if (firstNullTime > 2 && firstNullTime < powers.Count) // Minimum number of real values to calculate time between
            {

                // Replace time in dummy values with expected time values
                long diff = powers[firstNullTime - 1].ElapsedTime - powers[0].ElapsedTime;
                if (diff > (100 * firstNullTime)) // minimum accepted
                {
                    long newTimeBetween = diff / (firstNullTime - 1);
                    long time = powers[firstNullTime - 1].ElapsedTime; // last real time
                    for (int i = firstNullTime; i < powers.Count; i++)
                    {
                        time += newTimeBetween;
                        Power pow = powers[i];
                        pow.ElapsedTime = time;
                        powers[i] = pow;
                    }
                    return newTimeBetween;
                }
            }
            return defaultTimeBetween;
        }

        public List<Power> GetLastPowerValues(int count, out int timeBetween, int defaultTimeBetween)
        {
            timeBetween = defaultTimeBetween;
            List<Power> powers = new List<Power>();
            int numValues = count;
            if (numValues > powerValues.Count)
            {
                numValues = powerValues.Count;
            }
            for (int i = 0; i < numValues; i++)
            {
                powers.Add(powerValues[powerValues.Count - numValues + i]); 
            }
            NewPowerValuesAvailable = 0;
            if (numValues < count)
            {
                for (int i = numValues; i < count; i++)
                {
                    powers.Add(new Power(0));
                }
                GetTimeBetween(powers, defaultTimeBetween);
            }

            return powers;
        }

        public Power GetLastNewPowerValue()
        {
            if (NewPowerValuesAvailable > 0)
            {
                NewPowerValuesAvailable = 0; 
                return powerValues[powerValues.Count-1];
            }
            else
            {
                return new Power(0); // Must return something
            }
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
                NewPowerValuesAvailable = 0;
                return logMessages.GetRange(logMessages.Count - count, count);
            }
            else
            {
                return new List<AOULogMessage>();
            }
        }

        public List<AOULogMessage> GetNewLogMessages()
        {
            if (NewPowerValuesAvailable > 0)
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
    }
}
