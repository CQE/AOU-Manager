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
                DataUpdater.CreateLogMessage("LineChartViewModel.SetValues", e.Message);
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
                DataUpdater.CreateLogMessage("LineChartViewModel.SetNewValue", e.Message);
            }
        }

        /* Replaced by UpdateNewValues
        public void UpdateNewValue(Power pow)
        {
            power.Add(pow);
            power.RemoveAt(0);
        }
        */

        /* ToDo
        public void SetNewValues(List<Power> powerList, int startIndex)
        {
            try
            {
                int count = powerList.Count;
                if (startIndex+count < 30)
                {
                  // ToDo
                }
                for (int i = 0; i < powerList.Count; i++)
                {
                    power[startIndex + i] = powerList[i];
                }
            }
            catch (Exception e)
            {
                DataUpdater.CreateLogMessage("LineChartViewModel.SetNewValues", e.Message);
            }
        }
        */

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
                DataUpdater.CreateLogMessage("LineChartViewModel.UpdateNewValues", e.Message);
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
