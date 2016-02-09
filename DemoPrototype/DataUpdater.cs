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

    public static class GlobalAppSettings
    {
        static public bool IsCelsius
        {
            get
            {
                return ApplicationData.Current.LocalSettings.Values.ContainsKey("IsCelsius") ?
                       (bool)ApplicationData.Current.LocalSettings.Values["IsCelsius"] : true;
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["IsCelsius"] = (bool)value;
            }
        }

        static public AOURouter.RunType DataRunType
        { // Serial, File, Random
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("DataRunType"))
                {
                    return (AOURouter.RunType)ApplicationData.Current.LocalSettings.Values["DataRunType"];
                }
                else
                { 
                    return AOURouter.RunType.Random;
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["DataRunType"] = (int)value;
            }
        }

        static public string DataRunFile
        { // Serial, File, Random
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("DataRunFile"))
                {
                    return (string)ApplicationData.Current.LocalSettings.Values["DataRunFile"];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["DataRunFile"] = value;
            }
        }

        static public string DataSerialSettings
        { // Serial, File, Random
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("DataSerialSettings"))
                {
                    return (string)ApplicationData.Current.LocalSettings.Values["DataSerialSettings"];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["DataSerialSettings"] = value;
            }
        }

        static public string DataRandomSettings
        { // Serial, File, Random
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("DataRandomSettings"))
                {
                    return (string)ApplicationData.Current.LocalSettings.Values["DataRandomSettings"];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["DataRandomSettings"] = value;
            }
        }
    }

    public static class DataUpdater
    {
        private static AOURouter dataRouter;

        public static void sendToPLC(string cmd, int value)
        {
            switch (cmd)
            {
                case "Idle":
                    break;
                case "Heating":
                    break;
                case "Cooling":
                    break;
                case "Fixed Cycling":
                    break;
                case "Auto with IMM":
                    break;

            }
            

        }


        public static async void VerifySendToAOUDlg(string title, string message, Page pg)
        {
            var dlg = new ContentDialog();
            dlg.Title = title;
            dlg.Content = message;
            dlg.PrimaryButtonText = "Ok";
            dlg.SecondaryButtonText = "Cancel";

            ContentDialogResult res = await dlg.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                sendToPLC(title, 0);
                ((OperatorPage)pg).AsyncResponseDlg("Command sent", true);
            }
            else
            {
                ((OperatorPage)pg).AsyncResponseDlg("Command canceled", false);
            }

        }

        public static void StartHotStep(int time)
        {
            // ToDo Send to AOU
        }

        public static void StartColdStep(int time)
        {
            // ToDo Send to AOU
        }

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
                string dataRunSource = GlobalAppSettings.DataRunFile;

                dataRouter = new AOURouter(dataRunType, dataRunSource);
                return false;
            }
            return true;
        }

        public static void Update()
        {
            if (CheckDataRouterSingleton())
            {
                dataRouter.Update(1);
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

                // dc.power = pwrCol;
            }
            else
            {
                int n = dc.power.Count;
            }

        }

        public static void UpdateInputDataLogMessages(object dataContext)//NewPowerDataIsAvailable
        {
            CheckDataRouterSingleton();
            if (dataContext != null && dataRouter.NewLogMessagesAreAvailable())
            {
                ((LogMessageViewModel)dataContext).AddLogMessages(dataRouter.GetNewLogMessages());
            }
        }
    }


    public class LineChartViewModel
    // Class for handling chart data
    {
        public const int maxNumPoints = 30;
        private int lastRealValue = 0;

        public ObservableCollection<Power> power
        {
            get;
            set;
        }

        public LineChartViewModel()
        {
            power = new ObservableCollection<Power>();
        }

        public TimeSpan GetActualTimeSpan()
        {
            if (power.Count > 0)
            {
                return TimeSpan.FromMilliseconds(power[power.Count - 1].ElapsedTime);
            }
            else
                return new TimeSpan(0);
        }

        public void UpdateNewValue(Power pow)
        {
            try
            {
                if (power.Count == 0) // First
                {
                    power.Add(pow);
                    lastRealValue = 1;
                    for (int i = 1; i < maxNumPoints; i++)
                    {
                        power.Add(new Power(pow.ElapsedTime + i * 1000)); // 1000 ms expected
                    }

                }
                else if (lastRealValue < maxNumPoints)
                { 
                    // Fill with real values in empty points for every new value
                    power[lastRealValue++] = pow;
                }
                else
                {
                    // And go this forever
                    power.RemoveAt(0);
                    power.Add(pow);
                }
            }
            catch (Exception e)
            {
                var errmsg = e.Message;
                // ToDo logging
            }
        }
     }

    public class LogMessageViewModel
    {
        // Class for handling data grid log messages

        public ObservableCollection<AOULogMessage> logMessages
        {
            get;
            set;
        }

        public LogMessageViewModel()
        {
            logMessages = new ObservableCollection<AOULogMessage>();
        }

        public void AddLogMessages(AOULogMessage[] logs)
        {
            foreach (AOULogMessage log in logs)
            {
                logMessages.Add(log);
            }
        }

        public void AddLogMessage(AOULogMessage log)
        {
            logMessages.Add(new AOULogMessage(0, "Log Message null", 3, 0));
        }

    }
}
