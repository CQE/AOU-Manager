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
using Syncfusion.UI.Xaml.Grid.Converter;
using DataHandler;
using Windows.Storage;
using System.IO.IsolatedStorage;

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
            var options = new ExcelExportingOptions();
            options.ExcelVersion = Syncfusion.XlsIO.ExcelVersion.Excel2013;
            var excelEngine = LogGrid.ExportToExcel(LogGrid.View, options);
            SaveExcelToFile(excelEngine.Excel.Workbooks[0]);
        }
    }
}
