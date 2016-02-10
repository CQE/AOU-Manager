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
using DataHandler;
using Windows.Storage;
using Windows.Storage.Pickers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private const string RunTypeFileText = "Select File path relative to user Pictures folder:";
        private const string RunTypeSerialText = "Select <Port>, <Baudrate> :";
        private const string RunTypeRandomText = "Select<max number of values>, <ms between values>:";

        DispatcherTimer dTimer;

        public SettingsPage()
        {
            this.InitializeComponent();
            if (GlobalAppSettings.IsCelsius == true)
            {
                TempUnitCelsius.IsChecked = true;
            }
            else
            {
                TempUnitFahrenheit.IsChecked = true;
            }

            AOUDataSourceTypesCombo.Items.Add("Serial input");
            AOUDataSourceTypesCombo.Items.Add("File input");
            AOUDataSourceTypesCombo.Items.Add("Random input");
            if (GlobalAppSettings.DataRunType == AOURouter.RunType.Serial)
            {
                AOUDataSourceTypesCombo.SelectedIndex = 0;
                AOUDataSourceStringText.Text = RunTypeSerialText;
                AOUDataSourceString.Text = GlobalAppSettings.DataSerialSettings;
                this.pickButton.Visibility = Visibility.Collapsed;
                AOUDataSourceString.IsReadOnly = false;
            }
            else if (GlobalAppSettings.DataRunType == AOURouter.RunType.File)
            {
                AOUDataSourceTypesCombo.SelectedIndex = 1;
                this.AOUDataSourceStringText.Text = RunTypeFileText;
                this.AOUDataSourceString.Text = GlobalAppSettings.DataRunFile;
                this.pickButton.Visibility = Visibility.Visible;
                AOUDataSourceString.IsReadOnly = true;
            }
            else
            {
                this.AOUDataSourceStringText.Text = RunTypeRandomText;
                AOUDataSourceTypesCombo.SelectedIndex = 2;
                this.AOUDataSourceString.Text = GlobalAppSettings.DataRandomSettings;
                this.pickButton.Visibility = Visibility.Collapsed;
                AOUDataSourceString.IsReadOnly = false;
            }

            dTimer = new DispatcherTimer();
            dTimer.Tick += UpdateTick;
            dTimer.Interval = new TimeSpan(0, 0, 0, 1, 0); // milliseconds
            dTimer.Start();
        }

        void UpdateTick(object sender, object e)
        {
            string s = DataUpdater.GetLog();
            if (s.Length > 0)
            {
                AppHelper.ShowMessageBox(s);
            }
        }

        /* New to be accepted */
        private void TempUnitCelsius_Checked(object sender, RoutedEventArgs e)
        {
            { GlobalAppSettings.IsCelsius = true; }
        }

        private void TempUnitFahrenheit_Checked(object sender, RoutedEventArgs e)
        {
            { GlobalAppSettings.IsCelsius = false; }
        }

        private void AOUDataSourceTypeChanged(object sender, RoutedEventArgs e)
        {
            AOURouter.RunType newRunType = GlobalAppSettings.DataRunType;
            if (AOUDataSourceTypesCombo.SelectedIndex >= 0)
            {
                if (AOUDataSourceTypesCombo.SelectedIndex == 1)
                {
                    newRunType = AOURouter.RunType.File;

                    this.AOUDataSourceStringText.Text = RunTypeFileText;
                    this.AOUDataSourceString.Text = GlobalAppSettings.DataRunFile;
                    this.pickButton.Visibility = Visibility.Visible;
                    AOUDataSourceString.IsReadOnly = true;
                }
                else if (AOUDataSourceTypesCombo.SelectedIndex == 0)
                {
                    newRunType = AOURouter.RunType.Serial;

                    this.AOUDataSourceStringText.Text = RunTypeSerialText;
                    this.AOUDataSourceString.Text = GlobalAppSettings.DataSerialSettings;
                    this.pickButton.Visibility = Visibility.Collapsed;
                    AOUDataSourceString.IsReadOnly = false;
                }
                else
                {
                    newRunType = AOURouter.RunType.Random;

                    this.AOUDataSourceStringText.Text = RunTypeRandomText;
                    this.AOUDataSourceString.Text = GlobalAppSettings.DataRandomSettings;
                    this.pickButton.Visibility = Visibility.Collapsed;
                    AOUDataSourceString.IsReadOnly = false;
                }
            }

            if (GlobalAppSettings.DataRunType != newRunType)
            {
                GlobalAppSettings.DataRunType = newRunType;
                DataUpdater.Restart();
            }
        }

        private void AOUDataSourceString_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AOUDataSourceTypesCombo.SelectedIndex == 1)
            {
                GlobalAppSettings.DataRunFile = AOUDataSourceString.Text;
            }
            else if (AOUDataSourceTypesCombo.SelectedIndex == 0)
            {
                GlobalAppSettings.DataSerialSettings = AOUDataSourceString.Text;
            }
            else
            {
                GlobalAppSettings.DataRandomSettings = AOUDataSourceString.Text;
            }

            DataUpdater.Restart();
        }

        private async void PickFile()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary; // Todo: usb, cloud
            picker.FileTypeFilter.Add(".txt");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Save only path relative to User Pictures folder
                string picturePath = file.Path.Substring(file.Path.IndexOf("Pictures") + ("Pictures").Length);
                AOUDataSourceString.Text = picturePath;
                GlobalAppSettings.DataRunFile = picturePath;
                DataUpdater.Restart();
            }

        }

        private void pickButton_Click(object sender, RoutedEventArgs e)
        {
            PickFile();
        }
    }
}
