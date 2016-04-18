using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.Grid;
//using Syncfusion.UI.Xaml.Grid.Converter; new handling in v14
using Windows.Storage;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MaintenancePage : Page
    {
        private DispatcherTimer dTimer;
       
        public MaintenancePage()
        {
            this.Loaded += MaintenancePage_Loaded;
            this.Unloaded += MaintenancePage_Unloaded;

            this.InitializeComponent();

            InitDispatcherTimer();
            //just testing 
            AOULogMessage msg = new AOULogMessage((long)1000, "just testing");
        //    LogMessageViewModel. logMessages.add(msg);
            
        }

        private void MaintenancePage_Unloaded(object sender, RoutedEventArgs e)
        {
           dTimer.Stop();
        }

        private void MaintenancePage_Loaded(object sender, RoutedEventArgs e)
        {
            dTimer.Start();
        }

        private void InitDispatcherTimer()
        {
            dTimer = new DispatcherTimer();
            dTimer.Tick += UpdateTick;
            dTimer.Interval = new TimeSpan(0, 0, 1);
        }

        void UpdateTick(object sender, object e)
        {
           DataUpdater.UpdateInputDataLogMessages(LogGrid.DataContext);
        }

        private async void SaveExcelToFile(Syncfusion.XlsIO.IWorkbook workBook)
        {
            string filename = "Logs.xlsx";
            StorageFolder folder = KnownFolders.PicturesLibrary;

            try
            {
                StorageFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists); ;
                var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);

                var isf = IsolatedStorageFile.GetUserStoreForApplication(); // m_AppFilesPath = "C:\Users\Benurme\AppData\Local\Packages\DemoPrototype_dcmya832rqt3c\LocalState"
                IsolatedStorageFileStream outStream = new IsolatedStorageFileStream(filename, FileMode.Create, isf);
                await workBook.SaveAsAsync(outStream);
                // await workBook.SaveAsAsync(fileStream);

                workBook.Close();

                // isf.CopyFile()
            }
            catch (Exception e)
            {
                string err = e.Message;
            }
            // StorageFile f = await folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            // await workBook.SaveAsAsync(f.);

            // var skydrive = new SkyDriveHandler(App.LiveSession, "000000004010FA0B", "InvoiceGenerater"); // How to use Microsoft.Live

        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            //var options = new ExcelExportingOptions();
            //options.ExcelVersion = Syncfusion.XlsIO.ExcelVersion.Excel2013;
            //var excelEngine = LogGrid.ExportToExcel(LogGrid.View, options);
            //SaveExcelToFile(excelEngine.Excel.Workbooks[0]);
        }
    }

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
