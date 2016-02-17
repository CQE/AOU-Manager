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
            return dataRouter.GetLogStr();
        }

        /************************************************
        ** Commands to AOU
        *************************************************/

        public static void SetToDo(string ToDo, int value)
        {
            //dataRouter.SendCommandToPlc(AOUTypes.CommandType.todo, value);
            dataRouter.SendToPlc("<ToDo value=\""+value+"\">" + ToDo + "</ToDo>");
        }

        public static void SetHotDelayTime(int value)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.hotDelayTime, value);
        }

        public static void SetColdDelayTime(int value)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.coldDelayTime, value);
        }

        public static void SetHotTankFeedTemp(int value)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.tempHotTankFeedSet, value);
        }

        public static void SetColdTankFeedTemp(int value)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.tempColdTankFeedSet, value);
        }

        public static void SetCoolingTime(int value)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.coolingTime, value);
        }

        public static void SetHeatingTime(int value)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.heatingTime, value);
        }

        public static void SetToolHeatingFeedPauseTime(int value)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.toolHeatingFeedPause, value);
        }

        public static void SetToolCoolingFeedPauseTime(int value)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.toolCoolingFeedPause, value);
        }

        public static void ChangeToIdleMode()
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.idleMode, 0);
        }

        public static void ChangeToHeatingMode()
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.heatingMode, 0);
        }

        public static void ChangeToCoolingMode()
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.coolingMode, 0);
        }

        public static void ChangeToFixedCyclingMode()
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.fixedCyclingMode, 0);
        }

        public static void ChangeToAutoWidthIMMMode()
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.autoWidthIMMMode, 0);
        }

        public static void StartHotStep(int time)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.tempHotTankFeedSet, time); // ToDo
        }

        public static void StartColdStep(int time)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.tempColdTankFeedSet, time); // ToDo
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


        public static void SetTHotTankAlarmLowThreshold(int time)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.THotTankAlarmLowThreshold, time); // ToDo
        }

        public static void SetTColdTankAlarmHighThreshold(int time)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.TColdTankAlarmHighThreshold, time); // ToDo
        }

        public static void SetTReturnThresholdCold2Hot(int time)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.TReturnThresholdCold2Hot, time); // ToDo
        }

        public static void SetTReturnThresholdHot2Cold(int time)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.TReturnThresholdHot2Cold, time); // ToDo
        }



        public static void SetTBufferHotLowerLimit(int time)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.TBufferHotLowerLimit, time); // ToDo
        }

        public static void SetTBufferColdUpperLimit(int time)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.TBufferColdUpperLimit, time); // ToDo
        }

        public static void SetTBufferMidRefThreshold(int time)
        {
            dataRouter.SendCommandToPlc(AOUTypes.CommandType.TBufferMidRefThreshold, time); // ToDo
        }

        public static async void VerifySendToAOUDlg(string mode, string title, VerifyDialogType dlgType, Page pg)
        {
            int value = 0;

            // SetValueDialog dlg;

            //if (dlgType = VerifyDialogType.VeryfyOkCancelOnly)
            //    dlg = new SetValueDialog(strValue)
            var dlg = new ContentDialog();
            dlg.Title = title;
            dlg.Content = mode;
            dlg.PrimaryButtonText = "Ok";
            dlg.SecondaryButtonText = "Cancel";

            ContentDialogResult res = await dlg.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                switch (mode)
                {
                    case "Idle": ChangeToIdleMode(); break;
                    case "Heating": ChangeToHeatingMode(); break;
                    case "Cooling": ChangeToCoolingMode(); break;
                    case "Fixed Cycling": ChangeToFixedCyclingMode(); break;
                    case "Auto with IMM": ChangeToAutoWidthIMMMode(); break;

                    case "THotTankAlarmLowThreshold": SetTHotTankAlarmLowThreshold(value); break;
                    case "SetTColdTankAlarmHighThreshold": SetTColdTankAlarmHighThreshold(value); break;

                    case "TReturnThresholdCold2Hot": SetTReturnThresholdCold2Hot(value); break;
                    case "TReturnThresholdHot2Cold": SetTReturnThresholdHot2Cold(value); break;

                    case "TBufferHotLowerLimit": SetTBufferHotLowerLimit(value); break;
                    case "TBufferColdUpperLimit": SetTBufferColdUpperLimit(value); break;
                    case "TBufferMidRefThreshold": SetTBufferMidRefThreshold(value); break;
                }

                //sendToPLC(title, 0);
                // if (pg.Name == "OperatorPage")
//                    ((OperatorPage)pg).AsyncResponseDlg("Command sent", true);
               // else if (pg.Name == "CalibratePage")
               //     ((CalibratePage)pg).AsyncResponseDlg("Command sent", true);

            }
            else
            {
//                if (pg.Name == "OperatorPage")
//                    ((OperatorPage)pg).AsyncResponseDlg("Command canceled", false);
                // else if (pg.Name == "CalibratePage")
                // ((CalibratePage)pg).AsyncResponseDlg("Command canceled", false);
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
            // AOURouter.RunType.Random, @"AOU\Data\example_data2.txt"
            if (dataRouter == null || restart)
            {
                AOURouter.RunType dataRunType = GlobalAppSettings.DataRunType;
                string dataRunSource = GlobalAppSettings.DataSerialSettings;
                if (dataRunType == AOURouter.RunType.File)
                {
                    dataRunSource = GlobalAppSettings.DataRunFile;
                }
                else if (dataRunType == AOURouter.RunType.Random)
                {
                    dataRunSource = GlobalAppSettings.DataRandomSettings;
                }

                dataRouter = new AOURouter(dataRunType, dataRunSource);
                return false;
            }
            return true;
        }

        public static void Update()
        {
            if (CheckDataRouterSingleton())
            {
                dataRouter.Update();
            }
        }

        public static void UpdateInputData(object dataContext)
        {
            CheckDataRouterSingleton();
            var dc = (LineChartViewModel)dataContext;
            if (GlobalAppSettings.DataRunType == AOURouter.RunType.Random)
            {
                // var ts = dc.GetActualTimeSpan();
                dc.UpdateNewValue(dataRouter.GetLastPowerValue());
                /*
                if (dataContext != null && dataRouter.NewPowerDataIsAvailable())
                {
                }
                else
                {
                    bool error = true;
                }
                */
            }
            else if (dc.power.Count == 0)
            {
                var pwrArr = dataRouter.GetLastPowerValues(dataRouter.GetNumPowerValues());
                var pwrCol = new ObservableCollection<Power>();
                foreach (var pwr in pwrArr)
                {
                    dc.power.Add(pwr);
                }
            }
            else
            {
                int n = dc.power.Count;
            }

        }

        public static void UpdateInputDataLogMessages(object dataContext)//NewPowerDataIsAvailable
        {
            CheckDataRouterSingleton();

            if (GlobalAppSettings.DataRunType == AOURouter.RunType.Random)
            {
                if (dataContext != null && dataRouter.NewLogMessagesAreAvailable())
                {
                    ((LogMessageViewModel)dataContext).AddLogMessages(dataRouter.GetNewLogMessages());
                }
            }
            else 
            {
                if (dataContext != null && ((LogMessageViewModel)dataContext).logMessages.Count == 0)
                { 
                    ((LogMessageViewModel)dataContext).AddLogMessages(dataRouter.GetNewLogMessages());
                }
            }
        }
    }
}
