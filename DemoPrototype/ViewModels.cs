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
                Data.Updater.CreateLogMessage("LineChartViewModel.SetValues", e.Message);
            }
        }

        public int SetNewValues(List<Power> powerList, int firstNullIndex)
        {
            int newLastIndex = Data.Updater.PowerCount - 1;

            try
            {
                int count = powerList.Count;
                if (firstNullIndex + count > Data.Updater.PowerCount)
                {
                    count = Data.Updater.PowerCount - count;
                }
                else
                {
                    newLastIndex = firstNullIndex + powerList.Count - 1;
                }

                // Replace dummyvalues
                for (int i = 0; i < count; i++)
                {
                    power[firstNullIndex + i] = powerList[i];
                }

                // Add new values and delete first values when max count of power values
                for (int i = count; i < powerList.Count; i++)
                {
                    power.Add(powerList[i]);
                    power.RemoveAt(0);
                }

            }
            catch (Exception e)
            {
                Data.Updater.CreateLogMessage("LineChartViewModel.SetNewValues", e.Message);
            }
            return newLastIndex;
        }


        public void UpdateNewValues(List<Power> powerList)
        {
            try
            {
                for (int i = 0; i < powerList.Count; i++)
                {
                    power.Add(powerList[i]);
                    power.RemoveAt(0);
                }
            }
            catch (Exception e)
            {
                Data.Updater.CreateLogMessage("LineChartViewModel.UpdateNewValues", e.Message);
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

        public void AddLogMessages(List<AOULogMessage> logs)
        {
            foreach (AOULogMessage log in logs)
            {
                logMessages.Add(log);
            }
        }

     }
}
