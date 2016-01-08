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

    public class DataUpdater
    {
        public DataUpdater()
        {
            /*
            ToDo More Initializing
            */
        }

        public void UpdateInputData(object dataContext)
        {
            ((LineChartViewModel)dataContext).DeleteFirstPoint();
            ((LineChartViewModel)dataContext).AddPoint(ValueGenerator.GetRandomPower());
        }
    }

    public class LineChartViewModel
    //Our class for handling chart data
    {
        long ts; // Time, Timestamp or ...
        Random rnd;


        public LineChartViewModel()
        {
            this.power = new ObservableCollection<Power>();

            //add some datapoints
            for (int i = 0; i < 30; i++)
            {
                AddPoint(ValueGenerator.GetRandomPower());
            }
        }

        public ObservableCollection<Power> power
        {
            get;
            set;
        }

        public void AddPoint(Power newPoint)
        {
            power.Add(newPoint);
        }

        public void DeleteFirstPoint()
        {
            power.RemoveAt(0);
        }

    }

    public class AOULogMessageHandler
    {
        //replace this with correct error handler later, this code is for GUI testing only
        public static List<AOULogMessage> GetAOULogMessages()
        {
            var logMessages = new List<AOULogMessage>();
            logMessages.Add(new AOULogMessage(1, "message 1"));
            logMessages.Add(new AOULogMessage(2, "message 2"));
            logMessages.Add(new AOULogMessage(3, "message 3"));
            logMessages.Add(new AOULogMessage(4, "message 4"));
            return logMessages;
        }
    }

}
