﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DataHandler;

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
    }

    public static class DataUpdater
    {
        private static AOURouter dataRouter;

        public static void StartHotStep(int time)
        {
            // ToDo Send to AOU
        }

        public static void StartColdStep(int time)
        {
            // ToDo Send to AOU
        }

        private static bool CheckDataRouterSingleton()
        {
            if (dataRouter == null)
            { 
                dataRouter = new AOURouter(AOURouter.RunType.Random);
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
            ((LineChartViewModel)dataContext).UpdateNewValue(dataRouter.GetLastPowerValue());
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
