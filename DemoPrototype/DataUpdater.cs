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

    public class DataUpdater
    {
        private AOURouter router;

        public DataUpdater()
        {
            router = new AOURouter(AOURouter.RunType.Random);
            //            router = new AOURouter(AOURouter.RunType.File);
            //            router = new AOURouter(AOURouter.RunType.Serial);
        }

        public void UpdateInputData(object dataContext)
        {
            var lcvm = (LineChartViewModel)dataContext; // refer LineChartttViewModel
            router.Update(); // Important for updating to latest data
            if (lcvm.numPoints < lcvm.maxNumPoints)
            {
                lcvm.AddPoints(router.GetLastPowerValues((uint)lcvm.maxNumPoints)); 
            }
            else if (router.NewPowerDataIsAvailable())
            {
                lcvm.DeleteFirstPoint();
                lcvm.AddPoint(router.GetNewPowerValues());
            }
        }

        public void UpdateInputDataLogMessages(object dataContext)
        {
            var lmvm = (LogMessageViewModel)dataContext; // refer LineChartttViewModel
            router.Update(); // Important for updating to latest data

            if (router.NewLogMessagesAreAvailable())
            { 
                lmvm.AddLogMessage(router.GetNewLogMessage());
            }
        }
    }

    public class LineChartViewModel
    // Class for handling chart data
    {
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

        public ObservableCollection<Power> power
        {
            get;
            set;
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
            if (newPoint != null)
            { 
                power.Add(newPoint);
            }
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
            if (log != null) { 
                logMessages.Add(log);
            }
            else
            {
                logMessages.Add(new AOULogMessage(0, "Log Message null", 3, 0));
            }
        }

    }

    /* Only for test 
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
    */
}
