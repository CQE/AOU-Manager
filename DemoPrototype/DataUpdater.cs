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
    public static class DataUpdater
    {
        public enum VerifyDialogType {VeryfyOkCancelOnly, VerifyIntValue, VerifySliderValue};

        private static AOURouter dataRouter = null;

        private static int timeBetween;
        private const int defaultTimeBetween = 1000;
        private const int maxPowerCount = 30;

        private static bool started = false;

        public static bool IsStarted
        {
            get { return started && dataRouter != null; }
        }

        private static int lastPowerIndex = -1;
        private static Power lastPower = new Power(0);

        public static Power LastPower
        {
            get { return lastPower; }
            private set { lastPower = value; }
        }


        public static int LastPowerIndex
        {
            get { return lastPowerIndex; }
            private set { lastPowerIndex = value; }
        }

        public static void Init()
        {
            AOURouter.RunType dataRunType = GlobalAppSettings.DataRunType;

            if (dataRunType == AOURouter.RunType.File)
            {
                dataRouter = new AOURouter(new AOUSettings.FileSetting("", GlobalAppSettings.FileSettingsPath));
            }
            else if (dataRunType == AOURouter.RunType.Random)
            {
                dataRouter = new AOURouter(GlobalAppSettings.RandomSettings);
            }
            else if (dataRunType == AOURouter.RunType.Serial)
            {
                dataRouter = new AOURouter(GlobalAppSettings.SerialSettings, AOUSettings.DebugMode.noDebug);
            }
        }

        public static void Start()
        {
            started = true;
            dataRouter.Start();
        }

        public static void Stop()
        {
            started = false;
            if (IsStarted)
            {
                dataRouter.Stop();
                dataRouter = null; 
            }
        }

        public static void Update()
        {
            if (IsStarted)
            {
                dataRouter.Update();
            }
        }

        public static string GetLog()
        {
            if (dataRouter != null)
            { 
                return dataRouter.GetLogStr();
            }
            else
            {
                return "No datasource selected";
            }
        }

        public static void CreateLogMessage(string source, string text)
        {
            dataRouter.CreateLogMessage(source + " - " + text, 9);
        }

        /************************************************
        ** Commands to AOU
        *************************************************/

        public static void SetCommand(AOUDataTypes.CommandType cmd)
        {
            if (IsStarted)
            {
                dataRouter.SendCommandToPlc(cmd, 0);
            }
        }

        public static void SetCommandValue(AOUDataTypes.CommandType cmd, int value)
        {

            if (IsStarted)
            {
                dataRouter.SendCommandToPlc(cmd, value);
            }
        }

        public static void StartHotStep(int time)
        {
            if (IsStarted)
            {
                //dataRouter.SendCommandToPlc(AOUTypes.CommandType.hotDelayTime, time); // ToDo
                dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.CmdTypeToDo, time);
            }
        }

        public static void StartColdStep(int time)
        {
            if (IsStarted)
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

        public static async void VerifySendToAOUDlg(string title, string message, AOUDataTypes.CommandType cmd, Page pg, int val)
        {
            if (!IsStarted) return;

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
         public static bool UIButtonsChanged(out AOUDataTypes.UI_Buttons buttons)
        {
            buttons = new AOUDataTypes.UI_Buttons();
            if (dataRouter != null)
            {
                return dataRouter.UIButtonsChanged(out buttons);
            }
            return false;
        }

        public static bool IMMStatusChanged(out AOUDataTypes.IMMSettings status)
        {
            status = new AOUDataTypes.IMMSettings();
            if (dataRouter != null)
            {
                return dataRouter.IMMChanged(out status);
            }
            return false;
        }

        public static bool ModeChanged(out AOUDataTypes.HT_StateType mode)
        {
            mode = AOUDataTypes.HT_StateType.HT_STATE_NOT_SET;
            if (dataRouter != null)
            {
                return dataRouter.ModeChanged(out mode);
            }
            return false;
        }

        public static void UpdateInputData(object dataContext)
        {
            if (dataContext != null && IsStarted)
            {
                var dc = (LineChartViewModel)dataContext;
                int firstNullIndex = -1;

                try {
                    if (dc.power.Count == 0)
                    {
                        // If the first time then get all last values
                        var values = dataRouter.GetLastPowerValues(maxPowerCount, out firstNullIndex, defaultTimeBetween);
                        dc.SetValues(values);
                    }
                    else if (dataRouter.NewPowerValuesAvailable > 0)
                    {
                        List<Power> powerList = dataRouter.GetLastNewPowerValues();
                        if (powerList.Count > 0)
                        {
                            lastPower = powerList.Last();

                            var newPowerList = dataRouter.RecalcTime(dc.power, lastPower, out firstNullIndex, defaultTimeBetween);

                            if (firstNullIndex < (maxPowerCount - 1) && firstNullIndex >= 0)
                            {
                                // Replace with calculated null/dummy values
                                for (int i = 0; i < (powerList.Count); i++)
                                {
                                    dc.power[i + firstNullIndex - 1] = powerList[i];
                                }
                                lastPowerIndex = firstNullIndex - 1;
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

        public static void UpdateInputDataLogMessages(object dataContext)
        {
       
            if (dataContext != null && IsStarted)
            {
                var dc = (LogMessageViewModel)dataContext;
                if (dc.logMessages.Count == 0)
                {
                    dc.AddLogMessages(dataRouter.GetLastLogMessages(100));
                }
                else if (dataRouter.NewLogMessagesAvailable > 0)
                {
                    dc.AddLogMessages(dataRouter.GetNewLogMessages());
                }
           }

        }
    }
}
