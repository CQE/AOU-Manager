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
        private const int maxNumPoints = 30;

        private int realValueCount = 0;

        private long defaultTimeBetween = 1000;
        private long newTimeBetween = 0;

        public ObservableCollection<Power> power
        {
            get;
            set;
        }

        public LineChartViewModel()
        {
            power = new ObservableCollection<Power>();
            for (int i = 0; i < maxNumPoints; i++)
            {
                power.Add(new Power(i * defaultTimeBetween)); // Create dummy array with no values
            }
        }

        public int GetMaxNumOfPoints()
        {
            return maxNumPoints;
        }

        public bool IsEmpty()
        {
            return realValueCount == 0; 
        }

        public bool NotMaxValuesInCharts()
        {
            return realValueCount < maxNumPoints;
        }

        private void UpdateTime()
        {
            // Shall we update dummy time stamp in dummy values?
            if (newTimeBetween == 0 && realValueCount > 2 && realValueCount < maxNumPoints)
            {
                // Replace time in dummy values with expected time values
                long diff = power[realValueCount - 1].ElapsedTime - power[0].ElapsedTime;
                if (diff > (100*realValueCount)) // minimum accepted
                {
                    newTimeBetween = diff / (realValueCount - 1);
                    long time = power[realValueCount - 1].ElapsedTime;
                    for (int i = realValueCount; i < (maxNumPoints - realValueCount); i++)
                    {
                        time += newTimeBetween;
                        Power pow = power[i];
                        pow.ElapsedTime = time;
                        power[i] = pow;
                    }
                }
            }
        }

        public void SetValues(List<Power> lastPowers)
        {
            if (lastPowers.Count == 0)
            {
                return; // No data to handle
            }

            try
            {
                for (int i = 0; i < lastPowers.Count; i++)
                {
                    power[i] = lastPowers[i];
                }
                realValueCount = lastPowers.Count;
                UpdateTime();
            }
            catch (Exception e)
            {
                var errmsg = e.Message;
                // ToDo logging
            }
        }

        public void SetNewValue(Power pow)
        {
            try
            {
                power[realValueCount] = pow;
                realValueCount++;
                UpdateTime();
            }
            catch (Exception e)
            {
                var errmsg = e.Message;
                // ToDo logging
            }
        }

        public void UpdateNewValue(Power pow)
        {
            try
            {
                power.RemoveAt(0);
                power.Add(pow);
            }
            catch (Exception e)
            {
                var errmsg = e.Message;
                // ToDo logging
            }
        }

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
