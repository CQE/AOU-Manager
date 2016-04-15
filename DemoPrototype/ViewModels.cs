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
        private int lastRealValue = 0;
        private long timeBetween = 1000;

        public ObservableCollection<Power> power
        {
            get;
            set;
        }

        public int GetMaxNumOfPoints()
        {
            return maxNumPoints;
        }

        public LineChartViewModel()
        {
            power = new ObservableCollection<Power>();
        }

        public bool IsEmpty()
        {
            return power.Count == 0; 
        }

        public bool IsMoreTheMaxNumPoints()
        {
            return power.Count >= maxNumPoints && (lastRealValue+1) >= maxNumPoints;
        }

        public void SetValues(List<Power> lastPowers)
        {
            if (lastPowers.Count == 0)
            {
                return; // No data to handle
            }

            try
            {
                if (power.Count == 0)
                {
                    lastRealValue = lastPowers.Count;
                    for (int i = 0; i < lastPowers.Count; i++)
                    {
                        power.Add(lastPowers[i]);
                    }

                    long lastTime = 0;
                    if (lastPowers.Count > 0)
                    {
                        lastTime = lastPowers[lastPowers.Count - 1].ElapsedTime;
                    }

                    if (lastPowers.Count > 1)
                    {
                        long diff = lastPowers[lastPowers.Count - 1].ElapsedTime - lastPowers[0].ElapsedTime;
                        if (diff >= 100 * lastPowers.Count)
                        {
                            timeBetween = diff / (lastPowers.Count - 1);
                        }
                    }
                    if (lastPowers.Count < maxNumPoints)
                    {
                        for (int i = lastPowers.Count; i < maxNumPoints; i++)
                        {
                            power.Add(new Power(lastTime + i * timeBetween));
                        }
                    }
                }
                else if (power.Count == maxNumPoints && lastRealValue < maxNumPoints)
                {
                    int num = lastPowers.Count;
                    int start = lastRealValue;
                    lastRealValue += num;

                    if ((lastRealValue + num) > maxNumPoints)
                    {
                        num = maxNumPoints - lastRealValue - 1;
                        lastRealValue = 30;
                    }
                    for (int i = 1; i <= num; i++)
                    {
                        power[start+i] = lastPowers[i-1];
                    }
                }
                else
                {
                    int err = 1;
                }

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
                if (power.Count > 0)
                {
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

        public TimeSpan GetActualTimeSpan()
        {
            if (power.Count > 0)
            {
                if (lastRealValue < power.Count)
                    return TimeSpan.FromMilliseconds(power[lastRealValue - 1].ElapsedTime);
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
