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

       // private Brush OrgTuneChartBrush;
       // private SolidColorBrush FreezeBrush;

        DispatcherTimer dTimer;

        public OperatorPage()
        {
            this.Loaded += MaintenancePage_Loaded;
            this.Unloaded += MaintenancePage_Unloaded;

            this.InitializeComponent();
            this.Name = "OperatorPage";

            foreach (string mode in GlobalAppSettings.RunningModeStrings)
            {
                RunningModeCombo.Items.Add(mode);
            }

            prevRunModeSelected = -1; // First time to prevent sending new mode everytime entering OperatorPage
            RunningModeCombo.SelectedIndex = GlobalAppSettings.RunningMode; // Get saved Running Mode. Idle default, Only for Run ? Global settings only

            //set feed times
            TextBox_NewActiveHeatingTime.Text = GlobalVars.globFeedTimes.HeatingActive.ToString();
            TextBox_NewActiveCoolingTime.Text = GlobalVars.globFeedTimes.CoolingActive.ToString();
            TextBox_NewPauseHeatingTime.Text = GlobalVars.globFeedTimes.HeatingPause.ToString();
            TextBox_NewPauseCoolingTime.Text = GlobalVars.globFeedTimes.CoolingPause.ToString();

            //set and calculate delay time values
            TextBlock_HotCalibrate.Text = GlobalVars.globDelayTimes.HotCalibrate.ToString();
            HotFeedToReturnDelayCalTime.Text = GlobalVars.globDelayTimes.HotTune.ToString();
            int sum = GlobalVars.globDelayTimes.HotCalibrate + GlobalVars.globDelayTimes.HotTune;
            TextBlock_SumHotDelayTime.Text = sum.ToString();
            TextBlock_ColdCalibrate.Text = GlobalVars.globDelayTimes.ColdCalibrate.ToString();
            ColdFeedToReturnDelayCalTime.Text = GlobalVars.globDelayTimes.ColdTune.ToString();
            sum = GlobalVars.globDelayTimes.ColdCalibrate + GlobalVars.globDelayTimes.ColdTune;
            TextBlock_SumColdDelayTime.Text = sum.ToString();
            
            // Set tooltip contents
            HLineSet_ThresholdHot2Cold.ToolTipContent = "Threshold TRetActual hot"+ " ↘ " + "cold";
            HLineSet_ThresholdCold2Hot.ToolTipContent = "Threshold TRetActual cold" + " ↗ " + "hot";

            HLineSet_ThresholdHotTankAlarm.ToolTipContent = "Lower limit THotBuffer";
            HLineSet_ThresholdMidTankAlarm.ToolTipContent = "Threshold TMidBuffer";
            HLineSet_ThresholdColdTankAlarm.ToolTipContent = "Upper limit TColdBuffer";

            SetHotSafeZoneLine.ToolTipContent = "Lower limit THotTank";
            SetColdSafeZoneLine.ToolTipContent = "Upper limit TColdTank";


            // Set saved global values to all threshold lines

            HLineSet_ThresholdCold2Hot.Y1 = GlobalVars.globThresholds.ThresholdCold2Hot;
            HLineSet_ThresholdHot2Cold.Y1 = GlobalVars.globThresholds.ThresholdHot2Cold;

            HLineSet_ThresholdHotTankAlarm.Y1 = GlobalVars.globThresholds.ThresholdHotBuffTankAlarmLimit;
            HLineSet_ThresholdMidTankAlarm.Y1 = GlobalVars.globThresholds.ThresholdMidBuffTankAlarmLimit;
            HLineSet_ThresholdColdTankAlarm.Y1 = GlobalVars.globThresholds.ThresholdColdTankBuffAlarmLimit;

            //Set lineSeries colors
            Series_THotTank.Interior = new SolidColorBrush(Colors.Red);
            Series_EB_THotTank.Interior = new SolidColorBrush(Colors.Red);
            Series_THotBuffer.Interior = new SolidColorBrush(Colors.OrangeRed);
            Series_VB_THotBuffer.Interior = new SolidColorBrush(Colors.OrangeRed);

            Series_TColdTank.Interior = new SolidColorBrush(Colors.Blue);
            Series_EB_TColdTank.Interior = new SolidColorBrush(Colors.Blue);
            Series_TColdBuffer.Interior = new SolidColorBrush(Colors.LightBlue);
            Series_VB_TColdBuffer.Interior = new SolidColorBrush(Colors.LightBlue);

            Series_TMidBuffer.Interior = new SolidColorBrush(Colors.Khaki);
            Series_VB_TMidBuffer.Interior = new SolidColorBrush(Colors.Khaki);

            Series_TRetActual.Interior = new SolidColorBrush(Colors.RosyBrown);
            Series_Delay_TRetActual.Interior = new SolidColorBrush(Colors.RosyBrown);
            Series_EB_TRetActual.Interior = new SolidColorBrush(Colors.RosyBrown);
            Series_TRetForecasted.Interior = new SolidColorBrush(Colors.Purple);
            Series_Delay_TRetForecasted.Interior = new SolidColorBrush(Colors.Purple);

            Series_ValveReturn.Interior = new SolidColorBrush(Colors.LightGreen);
            Series_EB_ValveReturn.Interior = new SolidColorBrush(Colors.LightGreen);

            Series_ValveFeedHot.Interior = new SolidColorBrush(Colors.Red);
            Series_ValveFeedCold.Interior = new SolidColorBrush(Colors.CornflowerBlue);
            Series_ValveCoolant.Interior = new SolidColorBrush(Colors.Pink);

            Series_PowerHeating.Interior = new SolidColorBrush(Colors.LightGray);
            Series_THeaterOilOut.Interior = new SolidColorBrush(Colors.LightYellow);
         

            //should rename these too MW
            SetHotSafeZoneLine.Y1 = GlobalVars.globThresholds.ThresholdHotTankLowLimit;
            HotSafeZone.Start = GlobalVars.globThresholds.ThresholdHotTankLowLimit;
            SetColdSafeZoneLine.Y1 = GlobalVars.globThresholds.ThresholdColdTankUpperLimit;
            ColdSafeZone.Start = GlobalVars.globThresholds.ThresholdColdTankUpperLimit;

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
        }

        private void MaintenancePage_Loaded(object sender, RoutedEventArgs e)
        {
            dTimer.Start();
        }

        private void InitDispatcherTimer()
        {
            dTimer = new DispatcherTimer();
            dTimer.Tick += UpdateTick;
            // dTimer.Interval = new TimeSpan(0, 0, 1); // Seconds
            dTimer.Interval = new TimeSpan(0, 0, 0, 1, 0); // milliseconds
        }

        void UpdateTick(object sender, object e)
        {
            AOUDataTypes.HT_StateType mode = AOUDataTypes.HT_StateType.HT_STATE_NOT_SET;
            AOUDataTypes.UI_Buttons buttons = new AOUDataTypes.UI_Buttons();

            DataUpdater.UpdateInputData(mainGrid.DataContext);
            //update textboxes
            SetHotTankTempText();
            SetColdTankTempText();
            if (DataUpdater.ModeChanged(out mode))
            {
                switch (mode)
                {
                    case AOUDataTypes.HT_StateType.HT_STATE_COLD:
                        TextBlock_ToolTempering.Text = "COLD" + " " + mode.ToString();
                        TextBlock_ToolTempering.Foreground = AppColors.blue; break;
                    case AOUDataTypes.HT_StateType.HT_STATE_HOT:
                        TextBlock_ToolTempering.Text = "HEAT" + " " + mode.ToString();
                        TextBlock_ToolTempering.Foreground = AppColors.red; break;
                    case AOUDataTypes.HT_StateType.HT_STATE_INVALID:
                        TextBlock_ToolTempering.Text = "Invalid" + " " + mode.ToString();
                        TextBlock_ToolTempering.Foreground = AppColors.gray; break;
                    case AOUDataTypes.HT_StateType.HT_STATE_UNKNOWN:
                        TextBlock_ToolTempering.Text = "Unknown " + " " + mode.ToString();
                        TextBlock_ToolTempering.Foreground = AppColors.gray; break;
                    default:
                        TextBlock_ToolTempering.Text = "Not set" + " " + mode.ToString();
                        TextBlock_ToolTempering.Foreground = AppColors.gray; break;
                }
            }

            if (DataUpdater.UIButtonsChanged(out buttons))
            {

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
            ColdSafeZone.Start = GlobalVars.globThresholds.ThresholdColdTankUpperLimit;
        }

        public void Reset_ThresholdHot2Cold()
        {
            HLineSet_ThresholdHot2Cold.Y1 = GlobalVars.globThresholds.ThresholdHot2Cold;
        }

        public void Reset_ThresholdCold2Hot()
        {
            HLineSet_ThresholdCold2Hot.Y1 = GlobalVars.globThresholds.ThresholdCold2Hot;
        }
        //--------------------------------------------------------------------------------

        public void Reset_ThresholdHotTankAlarm() //not a good name MW
        {
            HLineSet_ThresholdHotTankAlarm.Y1 = GlobalVars.globThresholds.ThresholdHotBuffTankAlarmLimit;
        }

        public void Reset_ThresholdColdTankAlarm()
        {
            HLineSet_ThresholdColdTankAlarm.Y1 = GlobalVars.globThresholds.ThresholdColdTankBuffAlarmLimit;
        }

        public void Reset_ThresholdMidTankAlarm()
        {
           this.HLineSet_ThresholdMidTankAlarm.Y1 = GlobalVars.globThresholds.ThresholdMidBuffTankAlarmLimit;
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
                    DataUpdater.VerifySendToAOUDlg(modeTitle, message, AOUDataTypes.CommandType.RunningMode, this, RunningModeCombo.SelectedIndex);
                }
                else
                {
                    prevRunModeSelected = RunningModeCombo.SelectedIndex;
                }
            }
        }
        
        private void HLineSet_ThresholdHot2Cold_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Threshold TRetActual hot" + " ↘ " + "cold";
            string message = "You are about to set new threshold value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUDataTypes.CommandType.TReturnThresholdHot2Cold, HLineSet_ThresholdHot2Cold, this);
        }

        private void HLineSet_ThresholdCold2Hot_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Threshold TRetActual cold" + " ↘ " + "hot";
            string message = "You are about to set new threshold value to "; 
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUDataTypes.CommandType.TReturnThresholdCold2Hot, HLineSet_ThresholdCold2Hot, this);
        }

        
        private void HLineSet_ThresholdMidTank_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Buffer tank mid temperature threshold";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUDataTypes.CommandType.TBufferMidRefThreshold, HLineSet_ThresholdMidTankAlarm, this);
        }

        private void HLineSet_ThresholdHotTankAlarm_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Buffer tank Hot end Lower temperature limit";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUDataTypes.CommandType.TBufferHotLowerLimit, HLineSet_ThresholdHotTankAlarm, this);
        }

        private void HLineSet_ThresholdColdTankAlarm_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Buffer tank Cold end Upper temperature limit";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUDataTypes.CommandType.TBufferColdUpperLimit, HLineSet_ThresholdColdTankAlarm, this);
        }

        //
        private void MouldingDelayVLine1_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //calculate diff between lines and show result in overlaying text
          
            double myX1, myX2;
            Point myPoint1, myPoint2;
            int phaseDiff;
            //we take x-value from the line and any y-value should do
            myPoint1.X = AppHelper.SafeConvertToDouble(MouldingDelayVLine1.X1);
            myPoint1.Y = 0;
            myX1 = this.MyTuneChart.PointToValue(MyTuneChart.PrimaryAxis, myPoint1);
            //and the other line
            myPoint2.X = AppHelper.SafeConvertToDouble(MouldingDelayVLine2.X1);
            myPoint2.Y = 0;
            myX2 = this.MyTuneChart.PointToValue(MyTuneChart.PrimaryAxis, myPoint2);
            //calculate the difference in seconds
            phaseDiff = (int)Math.Abs(myX2 - myX1) / 1000;
            //write result
            PhaseDiffResult.Text = phaseDiff.ToString() + " (s)";
        }

        private void MouldingDelayVLine2_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //calculate diff between lines and show result in overlaying text
            double myX1, myX2;
            Point myPoint1, myPoint2;
            int phaseDiff;
            //we take x-value from the line and any y-value should do
            myPoint1.X = AppHelper.SafeConvertToDouble(MouldingDelayVLine1.X1);
            myPoint1.Y = 0;
            myX1 = this.MyTuneChart.PointToValue(MyTuneChart.PrimaryAxis, myPoint1);
            //and the other line
            myPoint2.X = AppHelper.SafeConvertToDouble(MouldingDelayVLine2.X1);
            myPoint2.Y = 0;
            myX2 = this.MyTuneChart.PointToValue(MyTuneChart.PrimaryAxis, myPoint2);
            //calculate the difference in seconds
            phaseDiff = (int)Math.Abs(myX2 - myX1) / 1000;
            //write result
            PhaseDiffResult.Text = phaseDiff.ToString() + " (s)";
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
                Double startX = AppHelper.SafeConvertToDouble(OperatorDelayXAxis.Minimum);
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
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Change Hot Tank Value", AOUDataTypes.CommandType.tempHotTankFeedSet, 100, 300, this);
        }

        private void NewTColdTankTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Change Cold Tank Value", AOUDataTypes.CommandType.tempColdTankFeedSet, 0, 30, this);
        }

        private void NewActiveHeatingTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Active heating time", AOUDataTypes.CommandType.heatingTime, 0, 30, this);
        }

        private void NewPauseHeatingTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Heating pause time", AOUDataTypes.CommandType.toolHeatingFeedPause, 0, 30, this);
        }

        private void NewActiveCoolingTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Active cooling time", AOUDataTypes.CommandType.coolingTime, 0, 30, this);
        }

        private void NewPauseCoolingTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Cooling pause time", AOUDataTypes.CommandType.toolCoolingFeedPause, 0, 30, this);
        }

        private void HotFeedToReturnDelayCalTime_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Hot return delay time", AOUDataTypes.CommandType.hotDelayTime, 0, 30, this);
        }

        private void ColdFeedToReturnDelayCalTime_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_TColdTank, "Cold return delay time", AOUDataTypes.CommandType.coldDelayTime, 0, 30, this);
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
            ColdSafeZone.Start = newY;
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
            int sum = GlobalVars.globDelayTimes.ColdCalibrate + GlobalVars.globDelayTimes.ColdTune;
            TextBlock_SumColdDelayTime.Text = sum.ToString();
        }

        private void HotFeedToReturnDelayCalTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            //calculate and show new sum
            int sum = GlobalVars.globDelayTimes.HotCalibrate + GlobalVars.globDelayTimes.HotTune;
            TextBlock_SumHotDelayTime.Text = sum.ToString();
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
            int newTemp = 0;
            if (mainGrid.DataContext != null)
            {
                var dc = (LineChartViewModel)mainGrid.DataContext;
                newTemp = (int)dc.power[GetCurrentIndex()].THotTank;
            }
            //zero decimals
            TextBox_THotTank.Text = newTemp.ToString();
        }

        private void SetColdTankTempText()
        {
            int newTemp = 0;
            if (mainGrid.DataContext != null)
            {
                var dc = (LineChartViewModel)mainGrid.DataContext;
                int index = GetCurrentIndex();
                // newTemp = dc.power[dc.power.Count-1].THotTank;
                newTemp = (int)dc.power[index].TColdTank;
            }
            //zero decimals
            TextBox_TColdTank.Text = newTemp.ToString();
        }

        private void SetToolTemperingText()
        {
            int toolTemp = -1;
            return;
            //this code is for debugging purposes
            //How do I get the mode here?
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

        private int GetCurrentIndex()
        {
            //This is a dirty(?) solution TODO Urban please improve this
            var dc = (LineChartViewModel)mainGrid.DataContext;
            int myIndex;
            for (myIndex = 29; myIndex > 0; myIndex--)
            {
                double testTemp = dc.power[myIndex].TBufferHot;
                if (double.IsNaN(testTemp) == false)
                {
                    //we are done!
                    return myIndex;
                }
            }            
         return myIndex;
        }

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
    }

}
