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
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using Windows.Graphics.Imaging;
using Windows.Graphics.Display;

namespace DemoPrototype
{
    /// <summary>
    /// Main Page for navigating to other pages. Operator page is default page
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static WebView webView;
        static StreamUriWinRTResolver myResolver;

        public MainPage()
        {
            this.InitializeComponent();
            BackButton.Visibility = Visibility.Collapsed;

            //want to start with Operator page, is there a better way then change
            MyFrame.Navigate(typeof(OperatorPage));

            // MyFrame.Navigate(typeof(SettingsPage));
            TitleTextBlock.Text = "Run moulding";
 
            //create these only once
            webView = new WebView();
            myResolver = new StreamUriWinRTResolver();

            InfoTextBlock.Text = Data.Updater.GetRunningModeStatus();

            //set window title
            var applicationView = ApplicationView.GetForCurrentView();
            string version = GetAppVersion();

            applicationView.Title = "Mould Tempering Manager version " + version;
            
            // Set High prio for menu buttons
            Dispatcher.CurrentPriority = CoreDispatcherPriority.High;

            //start maximised
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            //applicationView.ExitFullScreenMode();
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;

           /* if (applicationView.IsFullScreenMode)
            {
                applicationView.ExitFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
                // The SizeChanged event will be raised when the exit from full-screen mode is complete.
            }
            else
            {
                if (applicationView.TryEnterFullScreenMode())
                {
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
                    // The SizeChanged event will be raised when the entry to full-screen mode is complete.
                }
            }
            */

        }

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
                InfoTextBlock.Text = Data.Updater.GetRunningModeStatus();
                MyFrame.Navigate(typeof(OperatorPage));
                TitleTextBlock.Text = "Run moulding";
                BackButton.Visibility = Visibility.Collapsed;
            }

            else if (SettingsListBoxItem.IsSelected)
            {
                InfoTextBlock.Text = "";
                MyFrame.Navigate(typeof(SettingsPage));
                TitleTextBlock.Text = "System settings";
                //BackButton.Visibility = Visibility.Visible;
            }
            else if (CalibrateListBoxItem.IsSelected)
            {
                InfoTextBlock.Text = Data.Updater.GetRunningModeStatus();
                MyFrame.Navigate(typeof(CalibratePage));
                TitleTextBlock.Text = "Calibrate moulding";
                //BackButton.Visibility = Visibility.Visible;
            }
            else if (TuneListBoxItem.IsSelected)
            {
                InfoTextBlock.Text = Data.Updater.GetRunningModeStatus();
                MyFrame.Navigate(typeof(TunePage));
                TitleTextBlock.Text = "Tune operation";
                //BackButton.Visibility = Visibility.Visible;
            }

            else if (MaintenanceListBoxItem.IsSelected)
            {
                InfoTextBlock.Text = Data.Updater.GetRunningModeStatus();
                MyFrame.Navigate(typeof(MaintenancePage));
                TitleTextBlock.Text = "System maintenance";
                //BackButton.Visibility = Visibility.Visible;
            }
            else if (HelpListBoxItem.IsSelected)
            {
                TitleTextBlock.Text = "System support";
                BackButton.Visibility = Visibility.Collapsed;
                DisplayHtml("Help");
            }
            else if (AboutListBoxItem.IsSelected)
            {
                //we show version number after the title until a better place is decided

                string version = GetAppVersion();
                //string AboutHeader = "About AOU " + version;
                string AboutHeader = "About system";
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
