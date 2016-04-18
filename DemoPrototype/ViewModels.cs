using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    public class LineChartViewModel
    // Class for handling chart data
    {
        public ObservableCollection<Power> power
        {
            get;
            set;
        }

        public LineChartViewModel()
        {
            power = new ObservableCollection<Power>();
        }

        public void SetValues(List<Power> lastPowers)
        {
            try
            {
                for (int i = 0; i < lastPowers.Count; i++)
                {
                    power.Add(lastPowers[i]);
                }
            }
            catch (Exception e)
            {
                var errmsg = e.Message;
                // ToDo logging
            }
        }

        public void SetNewValue(Power pow, int index)
        {
            try
            {
                power[index] = pow;
            }
            catch (Exception e)
            {
                var errmsg = e.Message;
                // ToDo logging
            }
        }

        public void UpdateNewValue(Power pow)
        {
            power.Add(pow);
            power.RemoveAt(0);
        }

        /*
        public TimeSpan GetActualTimeSpan()
        {
            // Todo 
            if (power.Count > 0)
            {
                if (realValueCount < power.Count)
                    return TimeSpan.FromMilliseconds(power[realValueCount - 1].ElapsedTime);
                else
                    return TimeSpan.FromMilliseconds(power[power.Count - 1].ElapsedTime);
            }
            else
                return new TimeSpan(0);
        }
        */
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

        public void AddLogMessages(List<AOULogMessage> logs)
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
