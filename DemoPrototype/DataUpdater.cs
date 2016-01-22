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

        // ToDo: Move to static helper class
        public static double SafeConvertToDouble(object value)
        {
            if (value is String)
            {
                double res = Double.NaN;
                Double.TryParse((string)value, out res);
                return res;
            }
            else
            {
                try
                {
                    return (double)value;
                }
                catch (Exception)
                {
                    return Double.NaN;
                }
            }
        }

    }

    public static class DataUpdater
    {
        private static DataRouter dataRouter;

        private static bool CheckDataRouterSingleton()
        {
            if (dataRouter == null)
            { 
                dataRouter = new DataRouter(DataRouter.RunType.Random, 0);
                dataRouter.Update(); // First to do
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

        public static void InitInputData(object dataContext)
        {
            CheckDataRouterSingleton();
            if (dataContext != null)
            { 

                // int numPoints = ((LineChartViewModel)dataContext).maxNumPoints);
                //int numPoints = 5;
                //((LineChartViewModel)dataContext).AddPoints(numPoints);
            }
        }

        public static void UpdateInputData(object dataContext)
        {
            CheckDataRouterSingleton();
            if (dataContext != null && dataRouter.NewPowerDataIsAvailable())
            {
                ((LineChartViewModel)dataContext).UpdateNewValue(dataRouter.GetLastPowerValue());
            }
        }

        public static void InitInputDataLogMessages(object dataContext)
        {
            CheckDataRouterSingleton();
            if (dataContext != null)
            {
             //   ((LogMessageViewModel)dataContext).AddLogMessages(dataRouter.GetLastLogMessages(10));
            }
        }

        public static void UpdateInputDataLogMessages(object dataContext)
        {
            CheckDataRouterSingleton();
            if (dataContext != null && dataRouter.NewPowerDataIsAvailable())
            {
                ((LogMessageViewModel)dataContext).AddLogMessages(dataRouter.GetNewLogMessages());
            }
        }
    }


    public class LineChartViewModel
    // Class for handling chart data
    {
        public const int maxNumPoints = 30;

        public ObservableCollection<Power> power
        {
            get;
            set;
        }

        public LineChartViewModel()
        {
            Power[] powerArr = new Power[maxNumPoints];
            for (int i = 0; i < powerArr.Length; i++)
                powerArr[i] = new Power(true);
            power = new ObservableCollection<Power>(powerArr);
        }

        public void UpdateNewValue(Power pow)
        {
            power.Add(pow);
            if (power.Count > maxNumPoints)
                power.RemoveAt(0);
        }

        private void SetPoints(Power[] points)
        {
            foreach (Power point in points)
            { 
                power.Add(point);
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
