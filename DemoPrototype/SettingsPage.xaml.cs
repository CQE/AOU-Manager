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
        heatTransferFluidsList fluids;

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

            this.fluids = GlobalAppSettings.TransferFluidsList;
            initFluids();


          //  AskForAllParameters();



            //check if ask-buttons are needed
            if (GlobalVars.globTankSetTemps.HotTankSetTemp < 0)
            { AskForSetHotTankTemp.IsEnabled = true;
                AskForSetHotTankTemp_Click(null,null) ;
            }

            else
            { AskForSetHotTankTemp.IsEnabled = false; }
            if (GlobalVars.globTankSetTemps.ColdTankSetTemp < 0)
            { AskForSetColdTankTemp.IsEnabled = true;
                AskForSetColdTankTemp_Click(null, null);
            }
            else
            { AskForSetColdTankTemp.IsEnabled = false; }
            if (GlobalVars.globThresholds.ThresholdHot2Cold < 0)
            { AskForTHot2Cold.IsEnabled = true; }
            else
            { AskForTHot2Cold.IsEnabled = false; }
            if (GlobalVars.globThresholds.ThresholdCold2Hot < 0)
            { AskForTCold2Hot.IsEnabled = true; }
            else
            { AskForTCold2Hot.IsEnabled = false; }
            if (GlobalVars.globThresholds.ThresholdMidBuffTankAlarmLimit < 0)
            { AskForTMidBuf.IsEnabled = true; }
            else
            { AskForTMidBuf.IsEnabled = false; }
            //times
            if (GlobalVars.globFeedTimes.HeatingActive < 0)
            { AskForActiveHeating.IsEnabled = true; }
            else
            { AskForActiveHeating.IsEnabled = false; }
            if (GlobalVars.globFeedTimes.HeatingPause < 0)
            { AskForHeatingPause.IsEnabled = true; }
            else
            { AskForHeatingPause.IsEnabled = false; }
            if (GlobalVars.globFeedTimes.CoolingActive < 0)
            { AskForActiveCooling.IsEnabled = true; }
            else
            { AskForActiveCooling.IsEnabled = false; }
            if (GlobalVars.globFeedTimes.CoolingPause < 0)
            { AskForCoolingPause.IsEnabled = true; }
            else
            { AskForCoolingPause.IsEnabled = false; }

            if (GlobalVars.globDelayTimes.EACalibrate < 0)
            { AskForOffsetRet.IsEnabled = true; }
            else
            { AskForOffsetRet.IsEnabled = false; }

            if (GlobalVars.globDelayTimes.VACalibrate < 0)
            { AskForOffsetHot.IsEnabled = true; }
            else
            { AskForOffsetHot.IsEnabled = false; }

            if (GlobalVars.globDelayTimes.F2MCalibrateUsed < 0)
            { AskForF2MHot.IsEnabled = true; }
            else
            { AskForF2MHot.IsEnabled = false; }

            if (GlobalVars.globDelayTimes.HotCalibrate < 0)
            { AskForHotDelay.IsEnabled = true; }
            else
            { AskForHotDelay.IsEnabled = false; }

            if (GlobalVars.globDelayTimes.ColdCalibrate < 0)
            { AskForColdDelay.IsEnabled = true; }
            else
            { AskForColdDelay.IsEnabled = false; }






            dTimer = new DispatcherTimer();
            dTimer.Tick += UpdateTick;
            dTimer.Interval = new TimeSpan(0, 0, 0, 1, 0); // milliseconds
            dTimer.Start();
        }

        private void initFluids()
        {
            if (fluids != null && fluids.heatTransferFluid != null)
            {
                InputBrand.Text = fluids.heatTransferFluid.brand;
                InputType.Text = fluids.heatTransferFluid.type;
                FlashBox.Text = fluids.heatTransferFluid.temperaturesList.flashPoint.ToString();
                BoilingBox.Text = fluids.heatTransferFluid.temperaturesList.boilingPoint.ToString();
                IgnitionBox.Text = fluids.heatTransferFluid.temperaturesList.autoIgnition.ToString();
                SaveFluidFile.Visibility = Visibility.Collapsed;
            }
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

        private async void PickFile(string type)
        {
            try {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                string picturePath = "";
                picker.ViewMode = PickerViewMode.List;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary; // Todo: usb, cloud
                if (type == "FileInput")
                {
                    picker.FileTypeFilter.Add(".txt");
                }
                else
                {
                    picker.FileTypeFilter.Add(".xml");
                }

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {

                    // Save only path relative to User Pictures folder
                    if (file.Path.IndexOf("Pictures") > 0)
                    { 
                        picturePath = file.Path.Substring(file.Path.IndexOf("Pictures") + ("Pictures").Length);
                        if (type == "FileInput")
                        {
                            GlobalAppSettings.FileSettingsPath = picturePath;
                            this.FileName.Text = picturePath;
                        }
                        else if (type == "SaveHeatFluid")
                        {
                            SaveBox.Text = picturePath;
                        }
                        else if (type == "RecallHeatFluid")
                        {
                            HeatTransferFluidsList list = new HeatTransferFluidsList(picturePath);
                            GlobalAppSettings.TransferFluidsList = list.GetHeatTransferFluidsList();
                            fluids = GlobalAppSettings.TransferFluidsList;
                            initFluids();
                            RecallBox.Text = picturePath;
                        }
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


        private async void DeleteFile(string type)
        {
            try
            {
                //we want to delete all files older than date
                string path = Path.GetDirectoryName("C:\\Users\\Mia\\Pictures");
                DirectoryInfo folder = new DirectoryInfo(path);
                FileInfo[] files = folder.GetFiles();

                StorageFolder dataFolder = KnownFolders.PicturesLibrary;
                var folder2 = await dataFolder.CreateFolderAsync(path, CreationCollisionOption.OpenIfExists);

               // FileInfo[] files = await dataFolder.GetFilesAsync();
                long folderSize = files.Sum(fi => fi.Length);
                long folderSizeLimit = 1000;
                long amountToDelete = 1;

                if (folderSize > folderSizeLimit)
                {
                    // Sort the list of files with the oldest first.
                    Array.Sort(files,
                               (fi1, fi2) => fi1.CreationTime.CompareTo(fi2.CreationTime));

                    long amountDeleted = 0L;

                    foreach (FileInfo file in files)
                    {
                        amountDeleted += file.Length;
                        AppHelper.ShowMessageBox("You are about to delete 1 file");
                        file.Delete();

                        if (amountDeleted >= amountToDelete)
                        {
                            break;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                AppHelper.ShowMessageBox("Delete File: " + e.Message);
            }

        }






        private void pickButton_Click(object sender, RoutedEventArgs e)
        {
            PickFile("FileInput");
        }

        private void PickFluidFileSave_Click(object sender, RoutedEventArgs e)
        {
            PickFile("SaveHeatFluid");
        }

        private void SaveBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SaveBox.Text.Length == 0 && SaveFluidFile.Visibility == Visibility.Visible)
            {
                SaveFluidFile.Visibility = Visibility.Collapsed;
            }
            else if (SaveBox.Text.Length > 0 && SaveFluidFile.Visibility == Visibility.Collapsed)
            {
                SaveFluidFile.Visibility = Visibility.Visible;
            }
        }

        private void SaveFluidFile_Click(object sender, RoutedEventArgs e)
        {
            fluids.heatTransferFluid.brand = InputBrand.Text;
            fluids.heatTransferFluid.type = InputType.Text;
            fluids.heatTransferFluid.temperaturesList.boilingPoint = ushort.Parse(BoilingBox.Text); // ToDo: Better types and value parsing
            fluids.heatTransferFluid.temperaturesList.flashPoint = (byte)ushort.Parse(FlashBox.Text);
            fluids.heatTransferFluid.temperaturesList.autoIgnition = ushort.Parse(IgnitionBox.Text);
            HeatTransferFluidsList list = new HeatTransferFluidsList(fluids);
            list.WriteXML(SaveBox.Text);
            GlobalAppSettings.TransferFluidsList = list.GetHeatTransferFluidsList();
        }

        private void PickFluidFileRecall_Click(object sender, RoutedEventArgs e)
        {
            PickFile("RecallHeatFluid");
        }

        private void AskForSetHotTankTemp_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForHotTankTemp();
            AskForSetHotTankTemp.IsEnabled = false;
        }


        private void AskForSetColdTankTemp_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForColdTankTemp();
            AskForSetColdTankTemp.IsEnabled = false;
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            GlobalVars.globRemoteSettings.On = true;
            GlobalVars.globRemoteSettings.password = adminPassword.Password;
            GlobalVars.globRemoteSettings.URI = remoteUri.Text;
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

        public void AskForTMidBuf_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForMidBufThreshold();
            AskForTMidBuf.IsEnabled = false;
        }

        public void AskForTCold2Hot_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForCold2HotThreshold();
            AskForTCold2Hot.IsEnabled = false;
        }

        public void AskForTHot2Cold_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForHot2ColdThreshold();
            AskForTHot2Cold.IsEnabled = false;
        }

        //private void AskForActiveHeating_Click(object sender, RoutedEventArgs e)
        public void AskForActiveHeating_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForHeatingTime();
            AskForActiveHeating.IsEnabled = false;
        }

        public void AskForActiveCooling_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForCoolingTime();
            AskForActiveCooling.IsEnabled = false;
        }

        public void AskForHeatingPause_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForHeatingPause();
            AskForHeatingPause.IsEnabled = false;
        }

        public void AskForCoolingPause_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForCoolingPause();
            AskForCoolingPause.IsEnabled = false;
        }

        public void AskForOffsetHot_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForOffsetRetValveHotPeriod();
            AskForOffsetHot.IsEnabled = false;
        }

       
        public void AskForOffsetRet_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForOffsetHotFeed2RetValveTime();
            AskForOffsetRet.IsEnabled = false;
        }

        public void AskForF2MHot_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForHotFeed2MouldDelayTime();
            AskForF2MHot.IsEnabled = false;
        }

        public void AskForF2MCold_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForColdFeed2MouldDelayTime();
        }

        public void AskForHotDelay_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForHotDelayTime();
            AskForHotDelay.IsEnabled = false;
        }

        public void AskForColdDelay_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForColdDelayTime();
            AskForColdDelay.IsEnabled = false;
        }

        public void AskForAllParameters()
        {
            AppHelper.AskAOUForTemps();
            AppHelper.AskAOUForMouldTimes();
            AppHelper.AskAOUForDelayTimes();
            //AppHelper.AS


            return;
            if (GlobalVars.globTankSetTemps.HotTankSetTemp < 0)
            {
                AskForSetHotTankTemp_Click(null, null);

            }

            if (GlobalVars.globTankSetTemps.ColdTankSetTemp < 0)
            {
                AskForSetColdTankTemp_Click(null, null);
            }

            if (GlobalVars.globFeedTimes.HeatingActive < 0)
            {
                AskForActiveHeating_Click(null, null);
            }

            if (GlobalVars.globFeedTimes.HeatingPause < 0)
            {
                AskForHeatingPause_Click(null, null);
            }

            if (GlobalVars.globFeedTimes.CoolingActive < 0)
            {
                AskForActiveCooling_Click(null, null);
            }
            if (GlobalVars.globFeedTimes.CoolingPause < 0)
            {
                AskForCoolingPause_Click(null, null);
            }

        }

        private void AskForValves_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForValves();
            AskForValves.IsEnabled = false;
        }

        private void DeleteLogFiles_Click(object sender, RoutedEventArgs e)
        {
            //we want to delete all files older than date
            string path = Path.GetDirectoryName("C:\\Users\\Mia\\Pictures");
            DirectoryInfo folder = new DirectoryInfo(path);

            // var folder2 = dataFolder.CreateFolderAsync(subPath, CreationCollisionOption.OpenIfExists);
            StorageFolder dataFolder = KnownFolders.PicturesLibrary;
          

            FileInfo[] files = folder.GetFiles();
            long folderSize = files.Sum(fi => fi.Length);
            long folderSizeLimit = 1000;
            long amountToDelete = 1;

          //  ulong curFolderSize = AOULogFile.

            if (folderSize > folderSizeLimit)
            {
                // Sort the list of files with the oldest first.
                Array.Sort(files,
                           (fi1, fi2) => fi1.CreationTime.CompareTo(fi2.CreationTime));

                long amountDeleted = 0L;

                foreach (FileInfo file in files)
                {
                    amountDeleted += file.Length;
                    AppHelper.ShowMessageBox("You are about to delete 1 file");
                    file.Delete();

                    if (amountDeleted >= amountToDelete)
                    {
                        break;
                    }

                }
            }
        }

        private void TestMatrix_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskForMatrixValues();
        }

        private void AskForAllValues_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.TestAOUCommunication();
            // AppHelper.AskAOUForAllValues();
        }
    }
}
