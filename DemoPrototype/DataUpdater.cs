using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

namespace DemoPrototype
{
    public class DataUpdater
    {
        public enum VerifyDialogType {VeryfyOkCancelOnly, VerifyIntValue, VerifySliderValue};

        private AOURouter dataRouter = null;

        private const int defaultTimeBetween = 1000;
        private const int defaultPowerCount = 30;

        private int timeBetween = defaultTimeBetween;
        private int powerCount = defaultPowerCount;

        private int lastPowerIndex = -1;
        private Power lastPower = new Power(0);

        private AOULogFile logFile = null;

        public int TimeBetween
        {
            get { return timeBetween; }
            private set { timeBetween = value; }
        }

        public int PowerCount
        {
            get { return powerCount; }
            private set { powerCount = value; }
        }

        public Power LastPower
        {
            get { return lastPower; }
            private set { lastPower = value; }
        }

        public int LastPowerIndex
        {
            get { return lastPowerIndex; }
            private set { lastPowerIndex = value; }
        }

        public DataUpdater()
        {
            dataRouter = new AOURouter();
        }

        public void Init()
        {
            timeBetween = defaultTimeBetween;
            powerCount = defaultPowerCount;

            AOURouter.RunType dataRunType = GlobalAppSettings.DataRunType;

            // OBS!!!! only for debug logging
            logFile = new AOULogFile(DateTime.Now, "DemoPrototype");

            if (dataRunType == AOURouter.RunType.File)
            {
                AOUSettings.FileSetting fileSetting;
                fileSetting.FilePath = GlobalAppSettings.FileSettingsPath;
                dataRouter.Initialize(AOURouter.RunType.File, fileSetting, 1000);
            }
            else if (dataRunType == AOURouter.RunType.Random)
            {
                dataRouter.Initialize(AOURouter.RunType.Random, GlobalAppSettings.RandomSettings, (int)GlobalAppSettings.RandomSettings.MsBetween);
            }
            else if (dataRunType == AOURouter.RunType.Serial)
            {
                dataRouter.Initialize(AOURouter.RunType.Serial, GlobalAppSettings.SerialSettings, 1000);
            }
        }

        public void AddDebugLogLine(string className, string methodName, string text)
        {
            logFile.AddLog(0, className + " - " + methodName + " - " + text);
        }

        public string GetRunningModeStatus()
        {
            AOURouter.RunType mode = GlobalAppSettings.DataRunType;
            string text = "";

            if (mode == AOURouter.RunType.File)
            {
                text += "File";
            }
            else if (mode == AOURouter.RunType.Serial)
            {
                text += "Serial";
            }
            else if (mode == AOURouter.RunType.Random)
            {
                text += "Random";
            }

            text += " data ";
            if (dataRouter.IsConnected)
            {
                text += "started";
            }
            else
            {
                text += "not started";
            }

            return text;
        }

        public bool RouterIsStarted()
        {
            return dataRouter.IsConnected;
        }

        public void Start()
        {
            if (!dataRouter.IsInitialized)
            {
                Init();
            }

            dataRouter.Start();
        }

        public void Stop()
        {
            dataRouter.Stop();
        }

        public void Update()
        {
            dataRouter.Update();
        }

        public string GetLog()
        {
            return dataRouter.GetLogStr();
        }

        public void CreateLogMessage(string source, string text)
        {
            dataRouter.CreateLogMessage(source + " - " + text, 9);
        }

        /************************************************
        ** Commands to AOU
        *************************************************/

        public void SetCommand(AOUDataTypes.CommandType cmd)
        {
            if (dataRouter.IsConnected)
            {
                dataRouter.SendCommandToPlc(cmd, 0);
            }
        }

        public void SetCommandValue(AOUDataTypes.CommandType cmd, int value)
        {

            if (dataRouter.IsConnected)
            {
                dataRouter.SendCommandToPlc(cmd, value);
            }
        }

        public void StartHotStep(int time)
        {
            if (dataRouter.IsConnected)
            {
                dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.CmdTypeToDo, time); // ToDo
            }
        }

        public void StartColdStep(int time)
        {
            if (dataRouter.IsConnected)
            {
                dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.CmdTypeToDo, time); // ToDo
            }
        }

        /*
        Energy balance
         TReturnActual:
            TReturnThresholdHot2Cold[guess(THotTank + TColdTank) / 2 - (THotTank - TColdTank) / 4]
            TReturnThresholdCold2Hot[guess(THotTank + TColdTank) / 2 + (THotTank - TColdTank) / 4]

        Volume balance
         TBufferMid: TBufferMidRefThreshold[guess(THotTank + TColdTank) / 2]
         TBufferHot: TBufferHotLowerLimit[guess TBufferMidRefThreshold]
         TBufferCold: TBufferColdUpperLimit[guess TBufferMidRefThreshold]

        Storage tanks
         THotTank: THotTankAlarmLowThreshold[guess THotTankSet - (THotTankSet - TColdTankSet) / 4]
         TColdTank: TColdTankAlarmHighThreshold[guess TColdTankSet + (THotTankSet - TColdTankSet) / 4]

        */

        public async void VerifySendToAOUDlg(string title, string message, AOUDataTypes.CommandType cmd, Page pg, int val)
        {
            if (!dataRouter.IsConnected) return;

            var dlg = new ContentDialog();
            dlg.Title = title;
            dlg.Content = message;
            dlg.PrimaryButtonText = "Ok";
            dlg.SecondaryButtonText = "Cancel";

            ContentDialogResult res = await dlg.ShowAsync();
            try
            {
                if (res == ContentDialogResult.Primary)
                {
                    //Store new value and send to AOU
                    switch (cmd)
                    {
                        case AOUDataTypes.CommandType.RunningMode:
                            dataRouter.SendCommandToPlc(cmd, val);
                            GlobalAppSettings.RunningMode = val;
                            break;
                        case AOUDataTypes.CommandType.THotTankAlarmLowThreshold:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.THotTankAlarmLowThreshold, val);
                            GlobalVars.globThresholds.ThresholdHotTankLowLimit = val;
                            break;
                        case AOUDataTypes.CommandType.TColdTankAlarmHighThreshold:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.TColdTankAlarmHighThreshold, val);
                            GlobalVars.globThresholds.ThresholdColdTankUpperLimit = val;
                            break;
                        case AOUDataTypes.CommandType.TReturnThresholdCold2Hot:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.TReturnThresholdCold2Hot, val);
                            GlobalVars.globThresholds.ThresholdCold2Hot = val;
                            break;
                        case AOUDataTypes.CommandType.TReturnThresholdHot2Cold:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.TReturnThresholdHot2Cold, val);
                            GlobalVars.globThresholds.ThresholdHot2Cold = val;
                            break;
                        case AOUDataTypes.CommandType.TBufferHotLowerLimit:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.TBufferHotLowerLimit, val);
                            GlobalVars.globThresholds.ThresholdHotBuffTankAlarmLimit = val;
                            break;
                        case AOUDataTypes.CommandType.TBufferMidRefThreshold:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.TBufferMidRefThreshold, val);
                            GlobalVars.globThresholds.ThresholdMidBuffTankAlarmLimit = val;
                            break;
                        case AOUDataTypes.CommandType.TBufferColdUpperLimit:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.TBufferColdUpperLimit, val);
                            GlobalVars.globThresholds.ThresholdColdTankBuffAlarmLimit = val;
                            break;
                        default:
                            break;
                    }
                }
                else  //need to handle cancel
                {
                    switch (cmd)
                    {
                        case AOUDataTypes.CommandType.RunningMode:
                            if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_RunningMode();
                            break;
                        case AOUDataTypes.CommandType.TColdTankAlarmHighThreshold:
                            //if (pg.Name == "CalibratePage")
                            //    ((CalibratePage)pg).Reset_ColdTankAlarmHighThreshold();
                            if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_ColdTankAlarmHighThreshold();
                            break;
                        case AOUDataTypes.CommandType.TReturnThresholdCold2Hot:
                            if (pg.Name == "CalibratePage")
                                ((CalibratePage)pg).Reset_ThresholdCold2Hot();
                            else if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_ThresholdCold2Hot();
                            break;
                        case AOUDataTypes.CommandType.TReturnThresholdHot2Cold:
                            if (pg.Name == "CalibratePage")
                                ((CalibratePage)pg).Reset_ThresholdHot2Cold();
                            else if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_ThresholdHot2Cold();
                            break;
                        case AOUDataTypes.CommandType.TBufferHotLowerLimit:
                            if (pg.Name == "CalibratePage")
                                ((CalibratePage)pg).Reset_ThresholdHotTankAlarm();
                            else if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_ThresholdHotTankAlarm();
                            break;
                        case AOUDataTypes.CommandType.TBufferMidRefThreshold:
                            if (pg.Name == "CalibratePage")
                                ((CalibratePage)pg).Reset_ThresholdMidTankAlarm();
                            else if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_ThresholdMidTankAlarm();
                            break;
                        case AOUDataTypes.CommandType.TBufferColdUpperLimit:
                            if (pg.Name == "CalibratePage")
                                ((CalibratePage)pg).Reset_ThresholdColdTankAlarm();
                            else if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_ThresholdColdTankAlarm();
                            break;
                        default:
                            break;
                    }
                    AppHelper.ShowMessageBox("Command not sent. Old value restored");
                }
            }
            catch (Exception e)
            {
                AppHelper.ShowMessageBox("Error: " + e.Message);
            }

        }

        /*****************************************************
        ** Router Engine
        *****************************************************/
         public bool UIButtonsChanged(out AOUDataTypes.UI_Buttons buttons)
        {
            buttons = new AOUDataTypes.UI_Buttons();
            if (dataRouter.IsConnected)
            {
                return dataRouter.UIButtonsChanged(out buttons);
            }
            return false;
        }

        public bool IMMStatusChanged(out AOUDataTypes.IMMSettings status)
        {
            status = new AOUDataTypes.IMMSettings();
            if (dataRouter.IsConnected)
            {
                return dataRouter.IMMChanged(out status);
            }
            return false;
        }

        public bool ModeChanged(out AOUDataTypes.HT_StateType mode)
        {
            mode = AOUDataTypes.HT_StateType.HT_STATE_NOT_SET;
            if (dataRouter.IsConnected)
            {
                return dataRouter.ModeChanged(out mode);
            }
            return false;
        }

        /* old solution */
        public void UpdateInputData(object dataContext)
        {
            if (dataContext != null && dataRouter.IsConnected)
            {
                var dc = (LineChartViewModel)dataContext;

                try
                {
                    if (dc.power.Count == 0)
                    {
                        // If the first time then get all last values
                        var values = dataRouter.GetLastPowerValues(powerCount, out lastPowerIndex);
                        if (values.Count > 0 && lastPowerIndex >= 0)
  
                        // if (lastPowerIndex > 2)
                        {
                            dc.SetValues(values);
                            lastPower = dc.power[lastPowerIndex];
                            // AddDebugLogLine("DataUpdater", "UpdateInputData.start.first", lastPower.ToString());
                            // AddDebugLogLine("DataUpdater", "UpdateInputData.start.last", lastPower.ToString());
                        }
                    }
                    else if (dataRouter.NewPowerValuesAvailable > 0)
                    {
                        List<Power> powerList = dataRouter.GetLastNewPowerValues();
                        if (powerList.Count > 0)
                        {
                            lastPower = powerList.Last();

                            int firstNullIndex = AOURouter.GetFirstNullIndex(dc.power.ToList());
                            if (firstNullIndex > -1)
                            {
                                lastPowerIndex = dc.SetNewValues(powerList, firstNullIndex);

                                if ((lastPowerIndex > 1 && lastPowerIndex < 6) && lastPowerIndex < (dc.power.Count - 1)) // Minimum number of real values to calculate time between
                                {
                                    firstNullIndex = AOURouter.GetFirstNullIndex(dc.power.ToList());
                                    var newPowerList = dataRouter.RecalcTime(dc.power.ToList(), firstNullIndex, timeBetween);
                                    for (int i = firstNullIndex; i < dc.power.Count; i++)
                                    {
                                        dc.power[i] = newPowerList[i];
                                        // AddDebugLogLine("DataUpdater", "UpdateInputData.replace:"+i, dc.power[i].ToString());
                                    }
                                }
                            }
                            else
                            {
                                // Normal handling. No dummy values. Add new values and delete first values
                                dc.UpdateNewValues(powerList);
                                lastPowerIndex = dc.power.Count - 1;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    CreateLogMessage("DataUpdater.UpdateInputData", e.Message);
                }
            }
        }


        public List<Power> GetNextPowerValues()
        {
            var values = new List<Power>();
            /*
            ToDo. No working in realwease
            */
            return values;
        }

        public List<Power> GetAllPowerValues()
        {
            var values = new List<Power>();
            
            // Must be connected
            if (dataRouter.IsConnected)
            {
                try
                {
                    values = dataRouter.GetLastPowerValues(powerCount, out lastPowerIndex);
                    if (values.Count > 0 && lastPowerIndex >= 0)
                    {
                        lastPower = values[lastPowerIndex];
                    }
                }
                catch (Exception e)
                {
                    CreateLogMessage("DataUpdater.GetAllPowerValues", e.Message);
                }
            }
            return values;
        }

        public List<AOULogMessage> GetLogMessages(bool all)
        {

            if (dataRouter.IsInitialized)
            {
                // If new power values then set the last power values property to last values
                if (dataRouter.NewPowerValuesAvailable > 0)
                {
                    List<Power> powerList = dataRouter.GetLastNewPowerValues();
                    if (powerList.Count > 0)
                    {
                        lastPower = powerList.Last();
                    }
                }

                if (all)
                {
                    return dataRouter.GetLastLogMessages(100);
                }
                else if (dataRouter.NewLogMessagesAvailable > 0)
                {
                    return dataRouter.GetNewLogMessages();
                }

            }
            return new List<AOULogMessage>(); // return empty list when no new log messages
        }

    }
}
