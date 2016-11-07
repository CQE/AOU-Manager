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
using Windows.Storage;
using Windows.Storage.Pickers;

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
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
           // AOUDataSourceTypesCombo.Items.Add("Remote Client"); // Future option

            if (GlobalAppSettings.DataRunType == AOURouter.RunType.Serial)
            {
                AOUDataSourceTypesCombo.SelectedIndex = 0;
            }
            else if (GlobalAppSettings.DataRunType == AOURouter.RunType.File)
            {
                AOUDataSourceTypesCombo.SelectedIndex = 1;
            }
            else
            {
                AOUDataSourceTypesCombo.SelectedIndex = 2;
            }

            if (Data.Updater.RouterIsStarted())
            {
                DisableChangeDataSource();

            }
            else
            {
                EnableChangeDataSource();
            }

            dTimer = new DispatcherTimer();
            dTimer.Tick += UpdateTick;
            dTimer.Interval = new TimeSpan(0, 0, 0, 1, 0); // milliseconds
            dTimer.Start();
        }

        private void initSerial()
        {
            this.FileName.Visibility = Visibility.Collapsed;
            this.pickButton.Visibility = Visibility.Collapsed;
            this.Param3Text.Visibility = Visibility.Collapsed;

            this.Param1Text.Visibility = Visibility.Visible;
            this.Param1Text.Text = "Serial Port";

            this.Param1Combo.Visibility = Visibility.Visible;
            this.Param1Combo.Items.Clear();
            this.Param1Combo.Items.Add("RPI1");
            this.Param1Combo.Items.Add("RPI2");
            this.Param1Combo.Items.Add("RPI3");
            this.Param1Combo.Items.Add("COM1");
            this.Param1Combo.Items.Add("COM2");
            this.Param1Combo.Items.Add("COM3");
            this.Param1Combo.Items.Add("COM4");
            this.Param1Combo.Items.Add("COM5");
            this.Param1Combo.Items.Add("COM6");
            this.Param1Combo.Items.Add("COM7");
            this.Param1Combo.Items.Add("COM8");
            this.Param1Combo.Items.Add("UART0");

            this.Param2Text.Visibility = Visibility.Visible;
            this.Param2Text.Text = "Baud Rate";

            this.Param2Combo.Visibility = Visibility.Visible;
            this.Param2Combo.Items.Clear();
            this.Param2Combo.Items.Add("9600");
            this.Param2Combo.Items.Add("14400");
            this.Param2Combo.Items.Add("19200");
            this.Param2Combo.Items.Add("28800");
            this.Param2Combo.Items.Add("38400");
            this.Param2Combo.Items.Add("57600");
            this.Param2Combo.Items.Add("115200");
            this.Param2Combo.Items.Add("230400");

            var settings = GlobalAppSettings.SerialSettings;
            this.Param1Combo.SelectedItem = settings.ComPort;
            this.Param2Combo.SelectedItem = settings.BaudRate.ToString();
        }

        private void initFile()
        {
            this.Param3Text.Visibility = Visibility.Visible;
            this.Param3Text.Text = "Select file";
            this.FileName.Visibility = Visibility.Visible;
            this.pickButton.Visibility = Visibility.Visible;
            this.FileName.IsReadOnly = true;

            this.Param1Text.Visibility = Visibility.Collapsed;
            this.Param1Combo.Visibility = Visibility.Collapsed;
            this.Param2Text.Visibility = Visibility.Collapsed;
            this.Param2Combo.Visibility = Visibility.Collapsed;

            this.FileName.Text = GlobalAppSettings.FileSettingsPath;
        }

        private void initRandom()
        {
            this.FileName.Visibility = Visibility.Collapsed;
            this.pickButton.Visibility = Visibility.Collapsed;
            this.Param3Text.Visibility = Visibility.Collapsed;

            this.Param1Text.Visibility = Visibility.Visible;
            this.Param1Text.Text = "Number of values";

            this.Param1Combo.Visibility = Visibility.Visible;
            this.Param1Combo.Items.Clear();
            this.Param1Combo.Items.Add("10");
            this.Param1Combo.Items.Add("20");
            this.Param1Combo.Items.Add("30");
            this.Param1Combo.Items.Add("50");

            this.Param2Text.Visibility = Visibility.Visible;
            this.Param2Text.Text = "ms between each value";

            this.Param2Combo.Visibility = Visibility.Visible;
            this.Param2Combo.Items.Clear();
            this.Param2Combo.Items.Add("100");
            this.Param2Combo.Items.Add("500");
            this.Param2Combo.Items.Add("1000");
            this.Param2Combo.Items.Add("1500");
            this.Param2Combo.Items.Add("2000");

            Param1Combo.SelectedValue = GlobalAppSettings.RandomSettings.NumValues.ToString();
            Param2Combo.SelectedValue = GlobalAppSettings.RandomSettings.MsBetween.ToString();
        }

        void UpdateTick(object sender, object e)
        {
            string s = Data.Updater.GetLog();
            if (s.Length > 0)
            {
                SourceStatus.Text = s;
            }
        }

        private void EnableChangeDataSource()
        {
            AOUDataSourceTypesCombo.IsEnabled = true;
            Param1Combo.IsEnabled = true;
            Param2Combo.IsEnabled = true;
            pickButton.IsEnabled = true;
            StartStopButton.Content = "Start";
        }

        private void DisableChangeDataSource()
        {
            AOUDataSourceTypesCombo.IsEnabled = false;
            Param1Combo.IsEnabled = false;
            Param2Combo.IsEnabled = false;
            pickButton.IsEnabled = false;
            StartStopButton.Content = "Stop";
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartStopButton.Content.ToString() == "Start")
            {
                switch (GlobalAppSettings.DataRunType)
                {
                    case AOURouter.RunType.Serial:
                        String comPort = Param1Combo.SelectedItem.ToString();
                        uint baudRate = uint.Parse(Param2Combo.SelectedItem.ToString());
                        GlobalAppSettings.SerialSettings = new AOUSettings.SerialSetting(comPort, baudRate);
                        break;
                    case AOURouter.RunType.File:
                        AOUSettings.FileSetting filesettings;
                        filesettings.FilePath = this.FileName.Text;
                        GlobalAppSettings.FileSettingsPath = filesettings.FilePath;
                        break;
                    case AOURouter.RunType.Random:
                        AOUSettings.RandomSetting randomsettings;
                        uint numValues = uint.Parse(Param1Combo.SelectedItem.ToString());
                        uint msBetween = uint.Parse(Param2Combo.SelectedItem.ToString());
                        GlobalAppSettings.RandomSettings = new AOUSettings.RandomSetting(numValues, msBetween);
                        break;
                    default:
                        break;
                }

                Data.Updater.Init();
                Data.Updater.Start();
                DisableChangeDataSource();

            }
            else if(StartStopButton.Content.ToString() == "Stop")
            {
                Data.Updater.Stop();
                EnableChangeDataSource();
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
                if (AOUDataSourceTypesCombo.SelectedIndex == 0)
                {
                    newRunType = AOURouter.RunType.Serial;
                    initSerial();
                }
                else if (AOUDataSourceTypesCombo.SelectedIndex == 1)
                {
                    newRunType = AOURouter.RunType.File;
                    initFile();
                }
                else
                {
                    newRunType = AOURouter.RunType.Random;
                    initRandom();
                }
            }

            if (GlobalAppSettings.DataRunType != newRunType)
            {
                GlobalAppSettings.DataRunType = newRunType;
            }
        }

        private async void PickFile()
        {
            try {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                string picturePath = "";
                picker.ViewMode = PickerViewMode.List;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary; // Todo: usb, cloud
                picker.FileTypeFilter.Add(".txt");

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {

                    // Save only path relative to User Pictures folder
                    if (file.Path.IndexOf("Pictures") > 0)
                    { 
                        picturePath = file.Path.Substring(file.Path.IndexOf("Pictures") + ("Pictures").Length);
                        GlobalAppSettings.FileSettingsPath = picturePath;
                        this.FileName.Text = picturePath;
                    }
                    else
                    {
                        AppHelper.ShowMessageBox("Pick File: Must be in Pictures folder");
                        string errpath = file.Path;
                    }
                }
            }
            catch (Exception e)
            {
                AppHelper.ShowMessageBox("Pick File: " + e.Message);
            }

    }

        private void pickButton_Click(object sender, RoutedEventArgs e)
        {
            PickFile();
        }

        private async void keyboardDlg()
        {
            char[,] chars = new char[2, 6];
            chars[0, 0] = 'a';
            chars[0, 1] = 'b';
            chars[0, 2] = 'c';
            chars[0, 3] = 'd';
            chars[0, 4] = 'e';
            chars[0, 5] = 'f';
            chars[1, 0] = 'g';
            chars[1, 1] = 'h';
            chars[1, 2] = 'i';
            chars[1, 3] = 'j';
            chars[1, 4] = 'k';
            chars[1, 5] = 'l';

            var dialog = new KeyboardDialog(chars);
            dialog.Title = "Enter Admin Password";
            dialog.PrimaryButtonText = "Enter";
            dialog.SecondaryButtonText = "Cancel";
            
            // dialog.MaxWidth = ActualWidth // Required for Mobile!

            await dialog.ShowAsync();
            userPassword.Focus(FocusState.Pointer);
        }

        private void adminPassword_Tapped(object sender, TappedRoutedEventArgs e)
        {
          //  keyboardDlg(); Not woking MW
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
          //  keyboardDlg(); Not working MW
        }

        private void AskForSetHotTankTemp_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForTankTemps();
        }

        private void AskForSetColdTankTemp_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForOffsetHotFeed2RetValveTime();
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            GlobalVars.globRemoteSettings.On = true;
            GlobalVars.globRemoteSettings.password = adminPassword.Password;
            GlobalVars.globRemoteSettings.URI = remoteUri.Text;
        }
    }
}
