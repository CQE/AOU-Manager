using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataHandler;
using Windows.UI.Xaml.Controls;

namespace DemoPrototype
{
    public static class DataUpdater
    {
        // Todo ms to sek

        public enum VerifyDialogType {VeryfyOkCancelOnly, VerifyIntValue, VerifySliderValue};

        private static AOURouter dataRouter;

        public static string GetLog()
        {
            if (dataRouter != null)
            { 
                return dataRouter.GetLogStr();
            }
            else
            {
                return "No Datasource";
            }
        }

        /************************************************
        ** Commands to AOU
        *************************************************/

        public static void SetCommand(AOUTypes.CommandType cmd)
        {
            if (dataRouter != null)
            {
                dataRouter.SendCommandToPlc(cmd, 0);
            }
        }

        public static void SetCommandValue(AOUTypes.CommandType cmd, int value)
        {

            if (dataRouter != null)
            {
                dataRouter.SendCommandToPlc(cmd, value);
            }
        }

        public static void StartHotStep(int time)
        {
            if (dataRouter != null)
            {
                //dataRouter.SendCommandToPlc(AOUTypes.CommandType.hotDelayTime, time); // ToDo
                dataRouter.SendCommandToPlc(AOUTypes.CommandType.CmdTypeToDo, time);
            }
        }

        public static void StartColdStep(int time)
        {
            if (dataRouter != null)
            {
                dataRouter.SendCommandToPlc(AOUTypes.CommandType.CmdTypeToDo, time); // ToDo
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

        public static bool IsStarted()
        {
            return dataRouter != null;
        }


        public static void Stop()
        {
            if (dataRouter != null)
            {
                dataRouter.Stop();
                dataRouter = null; //
            }
        }

        public static async void VerifySendToAOUDlg(string title, string message, AOUTypes.CommandType cmd, Page pg, int val)
        {
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
                        case AOUTypes.CommandType.RunningMode:
                            dataRouter.SendCommandToPlc(cmd, val);
                            GlobalAppSettings.RunningMode = val;
                            break;
                        case AOUTypes.CommandType.THotTankAlarmLowThreshold:
                            dataRouter.SendCommandToPlc(AOUTypes.CommandType.THotTankAlarmLowThreshold, val);
                            GlobalVars.globThresholds.ThresholdHotTankLowLimit = val;
                            break;
                        case AOUTypes.CommandType.TColdTankAlarmHighThreshold:
                            dataRouter.SendCommandToPlc(AOUTypes.CommandType.TColdTankAlarmHighThreshold, val);
                            GlobalVars.globThresholds.ThresholdColdTankUpperLimit = val;
                            break;
                        case AOUTypes.CommandType.TReturnThresholdCold2Hot:
                            dataRouter.SendCommandToPlc(AOUTypes.CommandType.TReturnThresholdCold2Hot, val);
                            GlobalVars.globThresholds.ThresholdCold2Hot = val;
                            break;
                        case AOUTypes.CommandType.TReturnThresholdHot2Cold:
                            dataRouter.SendCommandToPlc(AOUTypes.CommandType.TReturnThresholdHot2Cold, val);
                            GlobalVars.globThresholds.ThresholdHot2Cold = val;
                            break;
                        case AOUTypes.CommandType.TBufferHotLowerLimit:
                            dataRouter.SendCommandToPlc(AOUTypes.CommandType.TBufferHotLowerLimit, val);
                            GlobalVars.globThresholds.ThresholdHotBuffTankAlarmLimit = val;
                            break;
                        case AOUTypes.CommandType.TBufferMidRefThreshold:
                            dataRouter.SendCommandToPlc(AOUTypes.CommandType.TBufferMidRefThreshold, val);
                            GlobalVars.globThresholds.ThresholdMidBuffTankAlarmLimit = val;
                            break;
                        case AOUTypes.CommandType.TBufferColdUpperLimit:
                            dataRouter.SendCommandToPlc(AOUTypes.CommandType.TBufferColdUpperLimit, val);
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
                        case AOUTypes.CommandType.RunningMode:
                            if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_RunningMode();
                            break;
                        case AOUTypes.CommandType.TColdTankAlarmHighThreshold:
                            //if (pg.Name == "CalibratePage")
                            //    ((CalibratePage)pg).Reset_ColdTankAlarmHighThreshold();
                            if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_ColdTankAlarmHighThreshold();
                            break;
                        case AOUTypes.CommandType.TReturnThresholdCold2Hot:
                            if (pg.Name == "CalibratePage")
                                ((CalibratePage)pg).Reset_ThresholdCold2Hot();
                            else if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_ThresholdCold2Hot();
                            break;
                        case AOUTypes.CommandType.TReturnThresholdHot2Cold:
                            if (pg.Name == "CalibratePage")
                                ((CalibratePage)pg).Reset_ThresholdHot2Cold();
                            else if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_ThresholdHot2Cold();
                            break;
                        case AOUTypes.CommandType.TBufferHotLowerLimit:
                            if (pg.Name == "CalibratePage")
                                ((CalibratePage)pg).Reset_ThresholdHotTankAlarm();
                            else if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_ThresholdHotTankAlarm();
                            break;
                        case AOUTypes.CommandType.TBufferMidRefThreshold:
                            if (pg.Name == "CalibratePage")
                                ((CalibratePage)pg).Reset_ThresholdMidTankAlarm();
                            else if (pg.Name == "OperatorPage")
                                ((OperatorPage)pg).Reset_ThresholdMidTankAlarm();
                            break;
                        case AOUTypes.CommandType.TBufferColdUpperLimit:
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
        public static void Restart()
        {
            CheckDataRouterSingleton(true);
        }

        private static bool CheckDataRouterSingleton(bool restart = false)
        {
            if (dataRouter == null || restart)
            {
                AOURouter.RunType dataRunType = GlobalAppSettings.DataRunType;

                if (dataRunType == AOURouter.RunType.File)
                {
                    dataRouter = new AOURouter(GlobalAppSettings.FileSettings);
                }
                else if (dataRunType == AOURouter.RunType.Random)
                {
                    dataRouter = new AOURouter(GlobalAppSettings.RandomSettings);
                }
                else if (dataRunType == AOURouter.RunType.Serial)
                {
                    dataRouter = new AOURouter(GlobalAppSettings.SerialSettings, AOUSettings.DebugMode.noDebug);
                }

                return false;
            }
            return true;
        }

        public static void Update()
        {
            try
            { 
                if (CheckDataRouterSingleton())
                {
                    dataRouter.Update();
                }
            }
            catch (Exception e)
            {

            }
        }

        public static void UpdateInputData(object dataContext)
        {
            if (dataContext != null)
            {
                CheckDataRouterSingleton();
                var dc = (LineChartViewModel)dataContext;
                if (dataContext != null)
                {
                    if (dc.IsEmpty())
                    {
                        // If first time then get all last values
                        dc.SetValues(dataRouter.GetLastPowerValues((int)dc.GetMaxNumOfPoints()));
                    }
                    else if (dc.NotMaxValuesInCharts())
                    {
                        if (dataRouter.NewPowerDataIsAvailable())
                        {
                            dc.SetNewValue(dataRouter.GetLastNewPowerValue());
                        }
                    }
                    else
                    { 
                        if (dataRouter.NewPowerDataIsAvailable())
                        { 
                            dc.UpdateNewValue(dataRouter.GetLastNewPowerValue());
                        }
                    }
                }
            }
        }

        public static void UpdateInputDataLogMessages(object dataContext)
        {
            if (dataContext != null)
            {
                CheckDataRouterSingleton();
                var dc = (LogMessageViewModel)dataContext;
                if (dataContext != null && dataRouter.NewLogMessagesAreAvailable())
                {
                    dc.AddLogMessages(dataRouter.GetNewLogMessages());
                }
           }
        }
    }
}
