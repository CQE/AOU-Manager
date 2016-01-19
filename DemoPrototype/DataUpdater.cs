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
        private static AOURouter dataRouter;

        private static bool CheckDataRouterSingleton()
        {
            if (dataRouter == null)
            { 
                dataRouter = new AOURouter(AOURouter.RunType.Random);
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
                ((LineChartViewModel)dataContext).AddPoints(dataRouter.GetLastPowerValues(((LineChartViewModel)dataContext).maxNumPoints));
            }
        }

        public static void UpdateInputData(object dataContext)
        {
            CheckDataRouterSingleton();
            if (dataContext != null && dataRouter.NewPowerDataIsAvailable())
            {
                ((LineChartViewModel)dataContext).DeleteFirstPoint();
                ((LineChartViewModel)dataContext).AddPoint(dataRouter.GetLastPowerValue());
            }
        }

        public static void InitInputDataLogMessages(object dataContext)
        {
            CheckDataRouterSingleton();
            if (dataContext != null)
            {
                ((LogMessageViewModel)dataContext).AddLogMessages(dataRouter.GetLastLogMessages(10));
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
        public ObservableCollection<Power> power
        {
            get;
            set;
        }

        public int maxNumPoints
        {
            get;
        }

        public int numPoints
        {
            get
            {
                return power.Count;
            }
        }

        public LineChartViewModel()
        {
            power = new ObservableCollection<Power>();
            maxNumPoints = 30;
        }

        public LineChartViewModel(int maxNumPoints) : this()
        {
            this.maxNumPoints = maxNumPoints;
        }

        public void AddPoints(Power[] points)
        {
            foreach (Power point in points)
            { 
                power.Add(point);
            }
        }

        public void AddPoint(Power newPoint)
        {
            power.Add(newPoint);
        }

        public void DeleteFirstPoint()
        {
            if (power.Count > maxNumPoints)
            {
                power.RemoveAt(0);
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
