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
                dataRouter.SendCommandToPlc(AOUTypes.CommandType.tempHotTankFeedSet, time); // ToDo
            }
        }

        public static void StartColdStep(int time)
        {
            if (dataRouter != null)
            {
                dataRouter.SendCommandToPlc(AOUTypes.CommandType.tempColdTankFeedSet, time); // ToDo
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


        public static async void VerifySendToAOUDlg(string title, string message, AOUTypes.CommandType cmd, VerifyDialogType dlgType, Page pg, int value = 0)
        {
            // SetValueDialog dlg;

            //if (dlgType = VerifyDialogType.VeryfyOkCancelOnly)
            //    dlg = new SetValueDialog(strValue)
            var dlg = new ContentDialog();
            dlg.Title = title;
            dlg.Content = message;
            dlg.PrimaryButtonText = "Ok";
            dlg.SecondaryButtonText = "Cancel";

            ContentDialogResult res = await dlg.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                if (pg.Name == "CalibratePage")
                    ((CalibratePage)pg).AsyncResponseDlg(cmd, true);
                else if (pg.Name == "OperatorPage")
                    ((OperatorPage)pg).AsyncResponseDlg(cmd, true);
            }
            else
            {
                if (pg.Name == "CalibratePage")
                    ((CalibratePage)pg).AsyncResponseDlg(cmd, false);
                else if (pg.Name == "OperatorPage")
                    ((OperatorPage)pg).AsyncResponseDlg(cmd, false);
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
                    dataRouter = new AOURouter(GlobalAppSettings.SerialSettings);
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
                if (GlobalAppSettings.DataRunType == AOURouter.RunType.Random)
                {
                    if (dataContext != null && dataRouter.NewPowerDataIsAvailable())
                        dc.UpdateNewValue(dataRouter.GetLastNewPowerValue());
                }
                else // if (dataContext != null && dataRouter.NewPowerDataIsAvailable())
                {
                    /* Real code to use later
                    dc.UpdateNewValue(dataRouter.GetLastPowerValue());
                    */
                    var pwrArr = dataRouter.GetLastPowerValues(30);
                    for (int i = 0; i < dc.power.Count; i++)
                    {
                        dc.power.RemoveAt(0);
                    }

                    foreach (var pwr in pwrArr)
                    {
                        dc.power.Add(pwr);
                    }
                }
            }
        }

        public static void UpdateInputDataLogMessages(object dataContext)//NewPowerDataIsAvailable
        {
            if (dataContext != null)
            {
                CheckDataRouterSingleton();
                var dc = (LogMessageViewModel)dataContext;

                if (GlobalAppSettings.DataRunType == AOURouter.RunType.Random)
                {
                    if (dataContext != null && dataRouter.NewLogMessagesAreAvailable())
                    {
                        dc.AddLogMessages(dataRouter.GetNewLogMessages());
                    }
                }
                else
                {
                    for (int i = 0; i < dc.logMessages.Count; i++)
                    {
                        dc.logMessages.RemoveAt(0);
                    }
                    dc.AddLogMessages(dataRouter.GetLastLogMessages(20));
                }
            }
        }
    }
}
