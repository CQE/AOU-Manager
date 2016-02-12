using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataHandler;

namespace DemoPrototype
{
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
