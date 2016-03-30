using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using Windows.Web;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.ApplicationModel;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using Windows.Graphics.Display;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        static WebView webView;
        static StreamUriWinRTResolver myResolver;
        // static RemoteServer server;

        public MainPage()
        {
            //Urban change this asap
            GlobalVar.ThresholdHot2Cold = 180;

            this.InitializeComponent();
            TitleTextBlock.Text = "AOU Control System Main View";

            // Test: server = new RemoteServer("remote", "19999");
            // this.ImageSwapChainPanel.Loaded += ImageSwapChainPanel_Loaded; // Not working good

            BackButton.Visibility = Visibility.Collapsed;
            //want to start with Operator page, is there a better way then change
            MyFrame.Navigate(typeof(OperatorPage));
            // MyFrame.Navigate(typeof(SettingsPage));
            TitleTextBlock.Text = "Run Injection moulding";

            //create these only once
            webView = new WebView();
            myResolver = new StreamUriWinRTResolver();

            //set window title
            var applicationView = ApplicationView.GetForCurrentView();
            string version = GetAppVersion();
            applicationView.Title = "AOU version " + version;
         

        }

        /* Test
        private void ImageSwapChainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            // http://igrali.com/2015/09/26/using-swapchainpanelrenderer-to-improve-real-time-rendering-in-lumia-imaging-sdk-3/
            // NuGet Install-Package LumiaImagingSDK.UWP
            server = new RemoteServer("remote", "19999");
        }
        */

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoBack)
            {
                MyFrame.GoBack();
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void ListBox_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (OperatorListBox.IsSelected)
            {
                MyFrame.Navigate(typeof(OperatorPage));
                TitleTextBlock.Text = "Run Injection moulding";
                BackButton.Visibility = Visibility.Collapsed;
            }

            else if (SettingsListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(SettingsPage));
                TitleTextBlock.Text = "System settings";
                BackButton.Visibility = Visibility.Visible;
            }
            else if (CalibrateListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(CalibratePage));
                TitleTextBlock.Text = "Calibrate system";
                BackButton.Visibility = Visibility.Visible;
            }
            else if (MaintenanceListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(MaintenancePage));
                TitleTextBlock.Text = "System maintenance";
                BackButton.Visibility = Visibility.Visible;
            }
            else if (HelpListBoxItem.IsSelected)
            {
                TitleTextBlock.Text = "System support";
                BackButton.Visibility = Visibility.Collapsed;
                DisplayHtml("Help");
            }
            else if (AboutListBoxItem.IsSelected)
            {
                /*Test to Capture App Image
                if (server != null)
                {
                    server.CaptureUIElementAndSaveToFile(MainGrid);
                    //server.CaptureUIElementAndSaveToFile(ImageSwapChainPanel);
                }
                */
                //we show version number after the title until a better place is decided

                string version = GetAppVersion();
                string AboutHeader = "About AOU " + version;
                TitleTextBlock.Text = AboutHeader;
                BackButton.Visibility = Visibility.Collapsed;
                DisplayHtml("About");
            }
        }

        private string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            // throw new NotImplementedException();
        }

        public void DisplayHtml(string htmlPage)
        {
            Uri uri = webView.BuildLocalStreamUri("page3", "/Assets/" + htmlPage + ".html");
            webView.NavigateToLocalStreamUri(uri, myResolver);
            MyFrame.Content = webView;
        }

    }

    public sealed class StreamUriWinRTResolver : IUriToStreamResolver
    {
        StorageFile f;
        IRandomAccessStream stream;


        ~StreamUriWinRTResolver()
        {
            if (stream != null)
                stream.Dispose();
        }

        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            if (uri == null)
            {
                throw new Exception();
            }
            string path = uri.AbsolutePath;

            // Because of the signature of the this method, it can't use await, so we 
            // call into a seperate helper method that can use the C# await pattern.
            return GetContent(path).AsAsyncOperation();
        }

        private async Task<IInputStream> GetContent(string path)
        {
            try
            {
                Uri localUri = new Uri("ms-appx://" + path);
                f = await StorageFile.GetFileFromApplicationUriAsync(localUri);
                stream = await f.OpenAsync(FileAccessMode.Read);
                return stream;
            }
            catch (Exception) { throw new Exception("Invalid path"); }
        }

    }

}
