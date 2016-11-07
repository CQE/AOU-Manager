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

        private bool isRecalculated = false;

        private const int defaultTimeBetween = 1000;
        private const int defaultPowerCount = 60;

        private int timeBetween = defaultTimeBetween;
        private int powerCount = defaultPowerCount;

        private int lastPowerIndex = -1;
        private Power lastPower = new Power(0);
        private AOURemoteData remoteServer;

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

        public bool IsChartFilled()
        {
            return LastPowerIndex < PowerCount-1;
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
 
            if (dataRunType == AOURouter.RunType.File)
            {
                GlobalVars.SetStaticValues(); // Set static values only when file;
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

            if (GlobalVars.globRemoteSettings.On == true && GlobalVars.globRemoteSettings.URI.Length > 0)
            {
                AOUSettings.RemoteSetting settings = new AOUSettings.RemoteSetting();

                remoteServer = new AOURemoteData(settings, AOUSettings.DebugMode.noDebug);
            }


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

        public void CreateLogMessage(string text)
        {
            dataRouter.CreateLogMessage(text, 9);
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

        public void SetRunningMode(AOUDataTypes.AOURunningMode mode)
        {
            if (dataRouter.IsConnected)
            {
                dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.runModeAOU, (int)mode); // ToDo: Right way
            }
        }

        public void AskCommandValue(AOUDataTypes.CommandType cmd)
        {
            if (dataRouter.IsConnected)
            {
                dataRouter.AskCommandValueFromPlc(cmd); 

            }
        }

        public void StartHotStep(int time)
        {
            if (dataRouter.IsConnected)
            {
                //set delay time 
                dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.hotDelayTime, time); // ToDo: Right way
                //set running mode
                Data.Updater.SetCommandValue(AOUDataTypes.CommandType.RunningMode, (int)AOUDataTypes.AOURunningMode.Heating);
                //SetRunningMode((AOUDataTypes.AOURunningMode)1);
            }
        }

        public void StartColdStep(int time)
        {
            if (dataRouter.IsConnected)
            {
                dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.coldDelayTime, time); // ToDo: Right way
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
                        case AOUDataTypes.CommandType.runModeAOU:
                            dataRouter.SendCommandToPlc(cmd, val);
                            GlobalAppSettings.RunningMode = val;
                            break;
                        case AOUDataTypes.CommandType.THotTankAlarmLowThreshold:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.THotTankAlarmLowThreshold, val);
                            break;
                        case AOUDataTypes.CommandType.TColdTankAlarmHighThreshold:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.TColdTankAlarmHighThreshold, val);
                            break;
                        case AOUDataTypes.CommandType.TReturnThresholdCold2Hot:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.TReturnThresholdCold2Hot, val);
                            break;
                        case AOUDataTypes.CommandType.TReturnThresholdHot2Cold:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.TReturnThresholdHot2Cold, val);
                            break;
                        case AOUDataTypes.CommandType.TBufferHotLowerLimit:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.TBufferHotLowerLimit, val);
                            break;
                        case AOUDataTypes.CommandType.TBufferMidRefThreshold:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.TBufferMidRefThreshold, val);
                            break;
                        case AOUDataTypes.CommandType.TBufferColdUpperLimit:
                            dataRouter.SendCommandToPlc(AOUDataTypes.CommandType.TBufferColdUpperLimit, val);
                            break;
                        default:
                            break;
                    }
                }
                else  //need to handle cancel
                {
                    switch (cmd)
                    {
                        case AOUDataTypes.CommandType.runModeAOU:
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

        public bool HotTimeChanged(out int time)
        {
            time = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.heatingTime, out time);
        }

        
        public bool HotTankSetTempChanged(out int temp)
        {
            temp = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.tempHotTankFeedSet, out temp);
        }

        public bool ColdTankSetTempChanged(out int time)
        {
            time = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.tempColdTankFeedSet, out time);
        }

        public bool HotDelayChanged(out int time)
        {
            time = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.hotDelayTime, out time);
        }

        public bool CoolTimeChanged(out int time)
        {
            time = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.coolingTime, out time);
        }

        public bool CoolDelayChanged(out int time)
        {
            time = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.coldDelayTime, out time);
        }

        private bool PauseHeatingTimeChanged(out int time)
        {
            time = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.toolHeatingFeedPause, out time);
        }

        private bool PauseCoolingTimeChanged(out int time)
        {
            time = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.toolCoolingFeedPause, out time);
        }

        private bool HotTankTempChanged(out int temp)
        {
            temp = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.tempHotTankFeedSet, out temp);
        }

        private bool ColdTankTempChanged(out int temp)
        {
            temp = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.tempColdTankFeedSet, out temp);
        }

        // Thresholds
        private bool HLineSet_ThresholdHot2Cold_Dragged(out int temp)
        {
            temp = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.TReturnThresholdHot2Cold, out temp);
        }
        private bool HLineSet_ThresholdCold2Hot_Dragged(out int temp)
        {
            temp = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.TReturnThresholdCold2Hot, out temp);
        }

        private bool HLineSet_ThresholdMidTank_Dragged(out int temp)
        {
            temp = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.TBufferMidRefThreshold, out temp);
        }
        private bool HLineSet_ThresholdHotTankAlarm_Dragged(out int temp)
        {
            temp = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.TBufferHotLowerLimit, out temp);
        }
        private bool HLineSet_ThresholdColdTankAlarm_Dragged(out int temp)
        {
            temp = 0;
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.TBufferColdUpperLimit, out temp);
        }


        private bool SetHotSafeZoneLine_DragCompleted(out int temp)
        {
            temp = 0; // globThresholds.ThresholdHotBuffTankAlarmLimit
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.THotTankAlarmLowThreshold, out temp);
        }

        private bool SetColdSafeZoneLine_DragCompleted(out int temp)
        {
            temp = 0; // globThresholds.ThresholdColdTankBuffAlarmLimit
            return GlobalVars.IsCommandValueChanged(AOUDataTypes.CommandType.TColdTankAlarmHighThreshold, out temp);
        }

        public List<Power> GetNewPowerValues()
        {
            var values = new List<Power>();

            // Must be connected
            if (dataRouter.IsConnected && dataRouter.NewPowerValuesAvailable > 0)
            {
                try
                {
                    values = dataRouter.GetLastNewPowerValues();
                    if (values.Count > 0)
                    {
                        lastPower = values.Last();
                        lastPowerIndex = powerCount-1;
                    }
                }
                catch (Exception e)
                {
                    CreateLogMessage("DataUpdater.GetAllPowerValues", e.Message);
                }
            }
            return values;
        }

        public void UpdatePowerValues(LineChartViewModel model)
        {
            if (dataRouter.IsConnected)
            {
                try
                {
                    if (dataRouter.NewPowerValuesAvailable > 0)
                    {
                        List<Power> powerList = dataRouter.GetLastNewPowerValues();
                        if (powerList.Count > 0)
                        {
                            lastPower = powerList.Last();
                            int firstNullIndex = AOURouter.GetFirstNullIndex(model.power.ToList());
                            lastPowerIndex = firstNullIndex + powerList.Count - 1;

                            if (isRecalculated || firstNullIndex < 2)
                            {
                                for (int i = 0; i < powerList.Count; i++)
                                {
                                    int pos = firstNullIndex + i;
                                    if (pos < model.power.Count)
                                    {
                                        model.power[pos] = powerList[i];
                                    }
                                }
                            }
                            else
                            {
                                var newPowerList = dataRouter.RecalcTime(model.power.ToList(), firstNullIndex, timeBetween);
                                for (int i = firstNullIndex; i < model.power.Count; i++)
                                {
                                    model.power[i] = newPowerList[i];
                                }
                                isRecalculated = true;
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
