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
using System.Threading;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Graphics.Display;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OperatorPage : Page
    {
        private int prevRunModeSelected = -1; // First time or

        private LineChartViewModel chartModel;
        private bool hasAskedAOU = false;

        // private Brush OrgTuneChartBrush;
        // private SolidColorBrush FreezeBrush;

        DispatcherTimer dTimer;

        public OperatorPage()
        {
            // Create oservable for syncfusion prívate instance of o LineChartViewModel

           this.chartModel = new LineChartViewModel();

            this.Loaded += MaintenancePage_Loaded;
            this.Unloaded += MaintenancePage_Unloaded;

            // Init xaml
            this.InitializeComponent();

            
            ShowProgress();

            this.Name = "OperatorPage";

            try
            {
                // Connect Datacontest to chartModel
                mainGrid.DataContext = chartModel;

                // If power values are existing  return all 30 last
                var powers = Data.Updater.GetAllPowerValues();
                if (Data.Updater.LastPowerIndex > 2)
                {
                    HideProgress();
                    chartModel.SetValues(powers);
                }

            }
            catch (Exception e)
            {
                Data.Updater.CreateLogMessage("OperatorPage", "chartModel - " + e.Message);
            }
           

            foreach (string mode in GlobalAppSettings.RunningModeStrings)
            {
                RunningModeCombo.Items.Add(mode);
            }

            prevRunModeSelected = -1; // First time to prevent sending new mode everytime entering OperatorPage
            RunningModeCombo.SelectedIndex = GlobalAppSettings.RunningMode; // Get saved Running Mode. Idle default, Only for Run ? Global settings only

            //set tool temp mode
            SetToolTemperingText();

            //ASk AOU
            AppHelper.AskAOUForTemps();
            AppHelper.AskAOUForMouldTimes();

            //set tank temperatures, check if received.
            if (GlobalVars.globTankSetTemps.HotTankSetTemp < 0)
            {
                NewTHotTankTextBox.Text = "-"; ;
            }
            else
            {
                NewTHotTankTextBox.Text = GlobalVars.globTankSetTemps.HotTankSetTemp.ToString();
            }
            if (GlobalVars.globTankSetTemps.ColdTankSetTemp < 0)
            {
                NewTColdTankTextBox.Text = "-";
            }
            else
            {
                NewTColdTankTextBox.Text = GlobalVars.globTankSetTemps.ColdTankSetTemp.ToString();
            }
                  
            //set feed times
            if (GlobalVars.globFeedTimes.HeatingActive>= 0)
            {
                TextBox_NewActiveHeatingTime.Text = GlobalVars.globFeedTimes.HeatingActive.ToString();
            }
                else
                TextBox_NewActiveHeatingTime.Text ="-";
            if (GlobalVars.globFeedTimes.CoolingActive >= 0)
            {
                TextBox_NewActiveCoolingTime.Text = GlobalVars.globFeedTimes.CoolingActive.ToString();
            }
            else
                TextBox_NewActiveCoolingTime.Text = "-";
            if (GlobalVars.globFeedTimes.HeatingPause >= 0)
            {
                TextBox_NewPauseHeatingTime.Text = GlobalVars.globFeedTimes.HeatingPause.ToString();
            }
            else
                TextBox_NewPauseHeatingTime.Text = "-";
            if (GlobalVars.globFeedTimes.CoolingPause >= 0)
            {
                TextBox_NewPauseCoolingTime.Text = GlobalVars.globFeedTimes.CoolingPause.ToString();
            }
            else
                TextBox_NewPauseCoolingTime.Text = "-";


           
           

          
            
            // Set tooltip contents
  
            SetHotSafeZoneLine.ToolTipContent = "Lower limit THotTank";
            SetColdSafeZoneLine.ToolTipContent = "Upper limit TColdTank";


            // Set saved global values to all threshold lines

  
            //Set lineSeries colors
            Series_THotTank.Interior = new SolidColorBrush(Colors.Red);
            //Series_EB_THotTank.Interior = new SolidColorBrush(Colors.Red);
            //Series_THotBuffer.Interior = new SolidColorBrush(Colors.OrangeRed);
            //Series_VB_THotBuffer.Interior = new SolidColorBrush(Colors.OrangeRed);

            Series_TColdTank.Interior = new SolidColorBrush(Colors.Blue);
            //Series_EB_TColdTank.Interior = new SolidColorBrush(Colors.Blue);
            //Series_TColdBuffer.Interior = new SolidColorBrush(Colors.LightBlue);
            //Series_VB_TColdBuffer.Interior = new SolidColorBrush(Colors.LightBlue);

            //Series_TMidBuffer.Interior = new SolidColorBrush(Colors.Khaki);
            //Series_VB_TMidBuffer.Interior = new SolidColorBrush(Colors.Khaki);

            Series_TRetActual.Interior = new SolidColorBrush(Colors.Purple);
            //Series_Delay_TRetActual.Interior = new SolidColorBrush(Colors.RosyBrown);
            //Series_EB_TRetActual.Interior = new SolidColorBrush(Colors.RosyBrown);
            //Series_TRetForecasted.Interior = new SolidColorBrush(Colors.Purple);
            //Series_Delay_TRetForecasted.Interior = new SolidColorBrush(Colors.Purple);

            Series_ValveReturn.Interior = new SolidColorBrush(Colors.LightGreen);
            //Series_EB_ValveReturn.Interior = new SolidColorBrush(Colors.LightGreen);

            //Series_ValveFeedHot.Interior = new SolidColorBrush(Colors.Red);
            //Series_ValveFeedCold.Interior = new SolidColorBrush(Colors.CornflowerBlue);
            //Series_ValveCoolant.Interior = new SolidColorBrush(Colors.Pink);

            //Series_PowerHeating.Interior = new SolidColorBrush(Colors.LightGray);
            //Series_THeaterOilOut.Interior = new SolidColorBrush(Colors.LightYellow);


            //should rename these too MW
            SetHotSafeZoneLine.Y1 = GlobalVars.globThresholds.ThresholdHotTankLowLimit;
            HotSafeZone.Start = GlobalVars.globThresholds.ThresholdHotTankLowLimit;
            SetColdSafeZoneLine.Y1 = GlobalVars.globThresholds.ThresholdColdTankUpperLimit;
            ColdSafeZone.Start = GlobalVars.globThresholds.ThresholdColdTankUpperLimit - ColdSafeZone.Width;  //nytt önskemål OF

            // Set initial values for temperature unit. App settings
            if (GlobalAppSettings.IsCelsius)
            {
                TextCorF.Text = " (°C)";
            }
            else
            {
                TextCorF.Text = " (°F)";
            }

            //var FreezeColor = new Windows.UI.Color(); FreezeColor.R = 255; FreezeColor.G = 255; FreezeColor.B = 230; FreezeColor.A = 255;
            //OrgTuneChartBrush = MyTuneChart.Background;
            //FreezeBrush = new SolidColorBrush();
            //FreezeBrush.Color = FreezeColor;

            InitDispatcherTimer();
        }

        private void MaintenancePage_Unloaded(object sender, RoutedEventArgs e)
        {
            dTimer.Stop();
            //clean data
            Series_THotTank.ItemsSource = null;
            Series_TColdTank.ItemsSource = null;
            //Series_TRetActual.ItemsSource = null;
            //Series_Delay_TRetActual.ItemsSource = null;
            /*Series_PowerHeating.ItemsSource = null;
            Series_TColdBuffer.ItemsSource = null;
            Series_THeaterOilOut.ItemsSource = null;
            Series_THotBuffer.ItemsSource = null;
            Series_TRetForecasted.ItemsSource = null;
            Series_ValveCoolant.ItemsSource = null;
            Series_ValveFeedCold.ItemsSource = null;
            Series_ValveFeedHot.ItemsSource = null;
          */  Series_ValveReturn.ItemsSource = null;
            //Series_Delay_TRetActual.ItemsSource = null;
            
        }

        private void MaintenancePage_Loaded(object sender, RoutedEventArgs e)
        {
            dTimer.Start();
        }

        private void InitDispatcherTimer()
        {
            dTimer = new DispatcherTimer();
            dTimer.Tick += UpdateTick;
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 400); //0,4 second
        }

        private void ShowProgress()
        {
            mainGridProgresRing.IsActive = true;
            mainGridProgresRing.IsEnabled = true;
        }

        private void HideProgress()
        {
            mainGridProgresRing.IsActive = false;
            mainGridProgresRing.IsEnabled = false;
        }

        void UpdateTick(object sender, object e)
        {
            AOUDataTypes.HT_StateType mode = AOUDataTypes.HT_StateType.HT_STATE_NOT_SET;
            AOUDataTypes.UI_Buttons buttons = new AOUDataTypes.UI_Buttons();

            if (chartModel.power.Count == 0)
            {
                var powers = Data.Updater.GetAllPowerValues();
                if (Data.Updater.LastPowerIndex > 2)
                {
                    chartModel.SetValues(powers);
                    HideProgress();
                    if (GlobalAppSettings.valueFeedHaveStarted && (hasAskedAOU==false))
                    {
                        //AppHelper.AskAOUForMouldingTimes(); 
                        //AppHelper.AskAOUForDelayTimes();
                        //AppHelper.AskAOUForThresholds();
                        //AppHelper.AskAOUForTankTemps();
                        hasAskedAOU = true;
                    }
                }

            }
            else if (Data.Updater.IsChartFilled()) // special uppdating
            {
                Data.Updater.UpdatePowerValues(chartModel);
            }
            else
            {
                var newValues = Data.Updater.GetNewPowerValues();
                if (newValues.Count > 0)
                {
                    chartModel.UpdateNewValues(newValues);
                }
            }

            //update textboxes
            SetHotTankTempText();
            SetColdTankTempText();

            int time = 0;
            int temp = 0;
            if (Data.Updater.HotTankSetTempChanged(out temp))
            {
                NewTHotTankTextBox.Text= temp.ToString();
                

                //else
                //{ TextBox_THotTankCaution.Text = "Below coolant"; }
            }
            if (Data.Updater.ColdTankSetTempChanged(out temp))
            {
                NewTColdTankTextBox.Text = temp.ToString();
            }

            if (Data.Updater.HotTimeChanged(out time))
            {
                // SetHeatingTimeText();
                //need to display time in seconds, we get deciseconds
              //  time = time / 10;
                TextBox_NewActiveHeatingTime.Text = time.ToString(); // GlobalVars.globFeedTimes.HeatingActive.ToString();
            }
            if (Data.Updater.HotDelayChanged(out time))
            {
                // SetHotDelayTimeText();
               // time = time / 10;
                TextBox_NewPauseHeatingTime.Text = time.ToString(); // GlobalVars.globFeedTimes.HeatingPause.ToString();
            }
            if (Data.Updater.CoolTimeChanged(out time))
            {
                // SetColdFeedTimeText();
               // time = time / 10;
                TextBox_NewActiveCoolingTime.Text = time.ToString(); // GlobalVars.globFeedTimes.CoolingActive.ToString();
            }
            if (Data.Updater.CoolDelayChanged(out time))
            {
                // SetColdDelayTimeText();
              //  time = time / 10;
                TextBox_NewPauseCoolingTime.Text = time.ToString(); // GlobalVars.globFeedTimes.CoolingPause.ToString();
            }
            if (Data.Updater.ModeChanged(out mode))
            {
                GlobalAppSettings.ToolTempMode = (int) mode;
                SetToolTemperingText();
            }

            if (Data.Updater.UIButtonsChanged(out buttons))
            {
                UpdateFromUIButtons(buttons);
            }

         }

        /****************************************************************************
        ** Reset methods when Cancel in Modal Dialogs
        *****************************************************************************/
        public void Reset_RunningMode()
        {
            int oldIndex = prevRunModeSelected;
            prevRunModeSelected = -1; // Reset to old active mode. Prevent NewModeSelected
            RunningModeCombo.SelectedIndex = oldIndex;
        }

        public void Reset_HotTankAlarmThreshold()
        {
            SetHotSafeZoneLine.Y1 = GlobalVars.globThresholds.ThresholdHotTankLowLimit;
            HotSafeZone.Start = GlobalVars.globThresholds.ThresholdHotTankLowLimit;
        }
        public void Reset_ColdTankAlarmHighThreshold()
        {
            SetColdSafeZoneLine.Y1 = GlobalVars.globThresholds.ThresholdColdTankUpperLimit;
            ColdSafeZone.Start = GlobalVars.globThresholds.ThresholdColdTankUpperLimit - ColdSafeZone.Width;
        }

    
        //--------------------------------------------------------------------------------
        private void NewModeSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (prevRunModeSelected != -1) // Not first time when selected is default value
                {
                    string modeTitle = RunningModeCombo.Items[RunningModeCombo.SelectedIndex].ToString();
                    string message = "You are about to change running mode";
                    // Data.Updater.VerifySendToAOUDlg(modeTitle, message, AOUDataTypes.CommandType.RunningMode, this, RunningModeCombo.SelectedIndex);
                    Data.Updater.VerifySendToAOUDlg(modeTitle, message, AOUDataTypes.CommandType.runModeAOU, this, RunningModeCombo.SelectedIndex);
                }
                else
                {
                    prevRunModeSelected = RunningModeCombo.SelectedIndex;
                }
            }
        }
        
      

   

        private void FreezeEnergyChart(object sender, TappedRoutedEventArgs e)
        {

        }

        private void FreezeVolumeChart(object sender, TappedRoutedEventArgs e)
        {

        }

        private void FreezeDelayChart(object sender, DoubleTappedRoutedEventArgs e)
        {
            //which mode?
            bool isRunning = dTimer.IsEnabled;
            if (isRunning)
            {
                dTimer.Stop();
                //MyTuneChart.Background = FreezeBrush;

                //where are the lines?
                //double firstSlope = AppHelper.SafeConvertToDouble(PhaseVLine2.X1);
                //double secondSlope = AppHelper.SafeConvertToDouble(PhaseVLine1.X1);
                //and what is min on the X-axis?
               // Double startX = AppHelper.SafeConvertToDouble(OperatorDelayXAxis.Minimum);
            }
            else
            {
                dTimer.Start();
                //MyTuneChart.Background = OrgTuneChartBrush;
            }
        }

        /*  GotFocus works better than TextChanged but problems when tab trough controls

            Using TextBlock can solve the problem

            ex. 
            private void tb_active_Tapped(object sender, TappedRoutedEventArgs e)
            {
                // AppHelper.GetValueToTextBox(ColdFeedToReturnDelayCalTime, (Control)coldTankSet, "Cold delay time", AOUTypes.CommandType.tempHotTankFeedSet, 0, 30);
            }
        */
        private void NewTHotTankTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Change Hot Tank Value", AOUDataTypes.CommandType.tempHotTankFeedSet, 100, 300,1, this);
        }

        private void NewTColdTankTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Change Cold Tank Value", AOUDataTypes.CommandType.tempColdTankFeedSet, 0, 30, 1,this);
        }

        private void NewActiveHeatingTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Active heating time", AOUDataTypes.CommandType.heatingTime, 0, 30, 1,this);
        }

        private void NewPauseHeatingTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Heating pause time", AOUDataTypes.CommandType.toolHeatingFeedPause, 0, 30,1, this);
        }

        private void NewActiveCoolingTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Active cooling time", AOUDataTypes.CommandType.coolingTime, 0, 30,1, this);
        }

        private void NewPauseCoolingTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Cooling pause time", AOUDataTypes.CommandType.toolCoolingFeedPause, 0, 30,1, this);
        }

        private void HotFeedToReturnDelayCalTime_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Hot return delay time", AOUDataTypes.CommandType.hotDelayTime, 0, 30,1, this);
        }

        private void ColdFeedToReturnDelayCalTime_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Cold return delay time", AOUDataTypes.CommandType.coldDelayTime, 0, 30, 1, this);
        }

        private void SetHotSafeZoneLine_DragDelta(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragDeltaEventArgs e)
        {
            //what is the new position of the line?
            double newY = AppHelper.SafeConvertToDouble(SetHotSafeZoneLine.Y1); //(double)SetHotSafeZoneLine.Y1;
            //make chart strip line follow the line when dragged
            HotSafeZone.Start = newY;
        }

        private void SetHotSafeZoneLine_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Lower temperature limit Hot Tank Safe Zone";
            string message = "You are about to set alarm value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUDataTypes.CommandType.THotTankAlarmLowThreshold, SetHotSafeZoneLine, this);
        }

        private void SetColdSafeZoneLine_DragDelta(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragDeltaEventArgs e)
        {
            //what is the new position of the line?
            double newY = AppHelper.SafeConvertToDouble(SetColdSafeZoneLine.Y1); //(double)SetHotSafeZoneLine.Y1;
            //make chart strip line follow the line when dragged
            ColdSafeZone.Start = newY - ColdSafeZone.Width;  //nytt förslag OF
        }

        private void SetColdSafeZoneLine_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            // TBD set new threshold value
            string title = "Upper temperature limit Cold Tank Safe Zone";
            string message = "You are about to set alarm value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUDataTypes.CommandType.TColdTankAlarmHighThreshold, SetColdSafeZoneLine, this);
        }

        private void ColdFeedToReturnDelayCalTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            //calculate and show new sum
            double sum = GlobalVars.globDelayTimes.ColdCalibrate + GlobalVars.globDelayTimes.ColdTune;
         //   TextBlock_SumColdDelayTime.Text = sum.ToString();
        }

        private void HotFeedToReturnDelayCalTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            //calculate and show new sum
            double sum = GlobalVars.globDelayTimes.HotCalibrate + GlobalVars.globDelayTimes.HotTune;
          //  TextBlock_SumHotDelayTime.Text = sum.ToString();
        }

        private void Button_Freeze_Run_Click(object sender, RoutedEventArgs e)
        {
            bool isRunning = dTimer.IsEnabled;
            if (isRunning)
            {
                dTimer.Stop();
                //MyTuneChart.Background = FreezeBrush;
                Button_Freeze_Run.Content="Run";
                SaveImage();
                //where are the lines?
                //double firstSlope = AppHelper.SafeConvertToDouble(PhaseVLine2.X1);
                //double secondSlope = AppHelper.SafeConvertToDouble(PhaseVLine1.X1);
                //and what is min on the X-axis?
               // Double startX = AppHelper.SafeConvertToDouble(OperatorDelayXAxis.Minimum);
            }
            else
            {
                //MyTuneChart.Save("chart.gif");
                dTimer.Start();
                //MyTuneChart.Background = OrgTuneChartBrush;
                Button_Freeze_Run.Content="Freeze";
            }
        }

        private void SetHotTankTempText()
        {
            if (Data.Updater.LastPowerIndex >= 0)
            {
                TextBox_THotTank.Text = ((int)Math.Round(Data.Updater.LastPower.THotTank)).ToString();
                //caution: 
                int temp = (int)Math.Round(Data.Updater.LastPower.THotTank);
                if (temp < 220)
                { TextBox_THotTankCaution.Text = "-"; }
                if (temp > 219 && temp < 316)
                { TextBox_THotTankCaution.Text = "Smoking stains"; }
                if (temp > 315 && temp < 320)
                {
                    TextBox_THotTankCaution.Text = "Danger";
                    TextBox_THotTankCaution.Background = new SolidColorBrush(Colors.OrangeRed);
                }
                if (temp > 319)
                {
                    TextBox_THotTankCaution.Text = "Fire";
                    TextBox_THotTankCaution.Background = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                TextBox_THotTank.Text = "-";
            }
        }

        private void SetColdTankTempText()
        {
            if (Data.Updater.LastPowerIndex >= 0)
            {
                TextBox_TColdTank.Text = ((int)Math.Round(Data.Updater.LastPower.TColdTank)).ToString();
            }
            else
            {
                TextBox_TColdTank.Text = "-";
            }
        }

        private void SetToolTemperingText()
        {
            int toolTemp = GlobalAppSettings.ToolTempMode;
            AOUDataTypes.HT_StateType mode = (AOUDataTypes.HT_StateType)toolTemp;
            switch (mode)
            {
                case AOUDataTypes.HT_StateType.HT_STATE_COLD:
                    TextBlock_ToolTempering.Text = "COLD";
                    TextBlock_ToolTempering.Foreground = AppColors.blue; break;
                case AOUDataTypes.HT_StateType.HT_STATE_HOT:
                    TextBlock_ToolTempering.Text = "HEAT";
                    TextBlock_ToolTempering.Foreground = AppColors.red; break;
                case AOUDataTypes.HT_StateType.HT_STATE_INVALID:
                    TextBlock_ToolTempering.Text = "Invalid";
                    TextBlock_ToolTempering.Foreground = AppColors.gray; break;
                case AOUDataTypes.HT_StateType.HT_STATE_UNKNOWN:
                    TextBlock_ToolTempering.Text = "Unknown";
                    TextBlock_ToolTempering.Foreground = AppColors.gray; break;
                default:
                    TextBlock_ToolTempering.Text = "Not set";
                    TextBlock_ToolTempering.Foreground = AppColors.gray; break;
            }
        }

        /*
        private void SetHeatingTimeText()
        {
            if (Data.Updater.LastPowerIndex >= 0)
            {

                TextBox_NewActiveHeatingTime.Text = GlobalVars.globFeedTimes.HeatingActive.ToString();
              }
            else
            {
                TextBox_NewActiveHeatingTime.Text  = "-";
            }
        }

        private void SetCoolingTimeText()
        {
            if (Data.Updater.LastPowerIndex >= 0)
            {

                TextBox_NewActiveCoolingTime.Text = GlobalVars.globFeedTimes.CoolingActive.ToString();
            }
            else
            {
                TextBox_NewActiveCoolingTime.Text = "-";
            }
        }

        private void SetPauseHeatingTimeText()
        {
            if (Data.Updater.LastPowerIndex >= 0)
            {

                TextBox_NewPauseHeatingTime.Text = GlobalVars.globFeedTimes.HeatingPause.ToString();
            }
            else
            {
                TextBox_NewPauseHeatingTime.Text = "-";
            }
        }

        private void SetPauseCoolingTimeText()
        {
            if (Data.Updater.LastPowerIndex >= 0)
            {

                TextBox_NewPauseCoolingTime.Text = GlobalVars.globFeedTimes.CoolingPause.ToString();
            }
            else
            {
                TextBox_NewPauseCoolingTime.Text = "-";
            }
        }
        */




        private async void SaveImage( )
        {
           // int myX = (int)toExport.ActualWidth;
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(ChartParameters, (int)ChartParameters.ActualWidth, (int)ChartParameters.ActualHeight);
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();


            var localFolder = KnownFolders.PicturesLibrary;
            var saveFile = await localFolder.CreateFileAsync("AOUChart.png", Windows.Storage.CreationCollisionOption.GenerateUniqueName);// Windows.Storage.CreationCollisionOption.OpenIfExists);

           
            using (var fileStream = await saveFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,// Ignore,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    pixelBuffer.ToArray());

                await encoder.FlushAsync();
            }
        }

        private void ButtonExportParameterChart_Click(object sender, RoutedEventArgs e)
        {
            SaveImage();
        }


        private void UpdateFromUIButtons(AOUDataTypes.UI_Buttons buttons)
        {
            int runningMode = 0;

            if (buttons.OnOffButton == AOUDataTypes.ButtonState.on)
            {
                string s = "On";
            }
            else
            {
                string s = "Off";
            }

            if (buttons.ButtonEmergencyOff == AOUDataTypes.ButtonState.on)
            {
                // What to display when emergency button is pushed
            }

            if (buttons.ButtonForcedHeating == AOUDataTypes.ButtonState.on)
            {
                runningMode = 1;
            }
            else if (buttons.ButtonForcedCooling == AOUDataTypes.ButtonState.on)
            {
                runningMode = 2;
            }
            else if (buttons.ButtonForcedCycling == AOUDataTypes.ButtonState.on)
            {
                runningMode = 3;
            }
            else if (buttons.ButtonRunWithIMM == AOUDataTypes.ButtonState.on)
            {
                runningMode = 4;
            }

            if (runningMode >= 0 && RunningModeCombo.SelectedIndex != runningMode)
            {
                prevRunModeSelected = -1;
                GlobalAppSettings.RunningMode = runningMode;
                RunningModeCombo.SelectedIndex = runningMode;
            }


        }

    }

}
