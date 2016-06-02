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
                // Add new values and delete first values when max count of power values
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

    // For test only
    public class OrderInfo
    {
        int orderID;
        string customerId;
        string country;
        string customerName;
        string shippingCity;

        public int OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }

        public string CustomerID
        {
            get { return customerId; }
            set { customerId = value; }
        }

        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; }
        }

        public string Country
        {
            get { return country; }
            set { country = value; }
        }



        public string ShipCity
        {
            get { return shippingCity; }
            set { shippingCity = value; }
        }

        public OrderInfo(int orderId, string customerName, string country, string customerId, string shipCity)
        {
            this.OrderID = orderId;
            this.CustomerName = customerName;
            this.Country = country;
            this.CustomerID = customerId;
            this.ShipCity = shipCity;
        }
    }

    public class OrderInfoRepository
    {
        ObservableCollection<OrderInfo> orderCollection;

        public ObservableCollection<OrderInfo> OrderInfoCollection
        {
            get { return orderCollection; }
            set { orderCollection = value; }
        }

        public OrderInfoRepository()
        {
            orderCollection = new ObservableCollection<OrderInfo>();
            this.GenerateOrders();
        }

        private void GenerateOrders()
        {
            orderCollection.Add(new OrderInfo(1001, "Maria Anders", "Germany", "ALFKI", "Berlin"));
            orderCollection.Add(new OrderInfo(1002, "Ana Trujilo", "Mexico", "ANATR", "México D.F."));
            orderCollection.Add(new OrderInfo(1003, "Antonio Moreno", "Mexico", "ANTON", "México D.F."));
            orderCollection.Add(new OrderInfo(1004, "Thomas Hardy", "UK", "AROUT", "London"));
            orderCollection.Add(new OrderInfo(1005, "Christina Berglund", "Sweden", "BERGS", "Luleå"));
            orderCollection.Add(new OrderInfo(1006, "Hanna Moos", "Germany", "BLAUS", "Mannheim"));
            orderCollection.Add(new OrderInfo(1007, "Frédérique Citeaux", "France", "BLONP", "Strasbourg"));
            orderCollection.Add(new OrderInfo(1008, "Martin Sommer", "Spain", "BOLID", "Madrid"));
            orderCollection.Add(new OrderInfo(1009, "Laurence Lebihan", "France", "BONAP", "Marseille"));
            orderCollection.Add(new OrderInfo(1010, "Elizabeth Lincoln", "Canada", "BOTTM", "Tsawassen"));
        }
    }


}
