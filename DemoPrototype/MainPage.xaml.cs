﻿using System;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        WebView webView;
        StreamUriWinRTResolver myResolver;

        public MainPage()
        {
            this.InitializeComponent();
           TitleTextBlock.Text = "AOU Control System Main View";
            BackButton.Visibility = Visibility.Collapsed;
            //want to start with Operator page, is there a better way then change
            MyFrame.Navigate(typeof(OperatorPage));
            TitleTextBlock.Text = "Run Injection Moulding";
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
            if (myResolver != null)
            {
                myResolver = null; // Remove from memory
            }
            if (webView != null)
            {
                webView = null; // Remove from memory
            }

            if (OperatorListBox.IsSelected)
            {
                MyFrame.Navigate(typeof(OperatorPage));
                TitleTextBlock.Text = "Run Injection Moulding";
                BackButton.Visibility = Visibility.Collapsed;
            }
           
            else if (SettingsListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(SettingsPage));
                TitleTextBlock.Text = "AOU Control System Settings";
                BackButton.Visibility = Visibility.Visible;
            }
            else if (CalibrateListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(CalibratePage));
                TitleTextBlock.Text = "AOU Control System Calibrate";
                BackButton.Visibility = Visibility.Visible;
            }
            else if (MaintenanceListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(MaintenancePage));
                TitleTextBlock.Text = "AOU Control System Maintenance";
                BackButton.Visibility = Visibility.Visible;
            }
            else if (HelpListBoxItem.IsSelected)
            {
                TitleTextBlock.Text = "AOU Control System Help";
                BackButton.Visibility = Visibility.Collapsed;
                DisplayHtml("Help");
            }
            else if (AboutListBoxItem.IsSelected)
            {
                TitleTextBlock.Text = "About AOU Control System";
                BackButton.Visibility = Visibility.Collapsed;
                DisplayHtml("About");
            }
        }

        public void DisplayHtml(string htmlPage)
        {
            webView = new WebView();
            myResolver = new StreamUriWinRTResolver();

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
