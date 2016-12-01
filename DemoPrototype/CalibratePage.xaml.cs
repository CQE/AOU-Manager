using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalibratePage : Page
    {
        private DispatcherTimer dTimer;

        private int doStepTimer = 0;

        private LineChartViewModel chartModel;

        public CalibratePage()
        {
            this.Loaded += MaintenancePage_Loaded;
            this.Unloaded += MaintenancePage_Unloaded;

            this.InitializeComponent();
            this.Name = "CalibratePage";

            try
            {
                chartModel = new LineChartViewModel();
                // Connect Datacontext to chartModel
                CalibrateGrid.DataContext = chartModel;

                // If power values are existing  return all 30 last
                var powers = Data.Updater.GetAllPowerValues();
                if (Data.Updater.LastPowerIndex > 2)
                {
                    // HideProgress();
                    chartModel.SetValues(powers);
                }

            }
            catch (Exception e)
            {
                Data.Updater.CreateLogMessage("OperatorPage", "chartModel - " + e.Message);
            }

            InitDispatcherTimer();

            //set and calculate delay time values
            HotFeedToReturnDelayTime.Text = GlobalVars.globDelayTimes.HotCalibrateStr;
            TextBlock_HotTune.Text = GlobalVars.globDelayTimes.HotTuneStr;
            TextBlock_SumHotDelayTime.Text = GlobalVars.globDelayTimes.HotDelayTimeSumStr;

            ColdFeedToReturnDelayTime.Text = GlobalVars.globDelayTimes.ColdCalibrateStr;
            TextBlock_ColdTune.Text = GlobalVars.globDelayTimes.ColdTuneStr;
            TextBlock_SumColdDelayTime.Text = GlobalVars.globDelayTimes.ColdDelayTimeSumStr;     //sum.ToString();

            double sum = GlobalVars.globDelayTimes.HotStep;
            CalibrateHotStepValue.Text = sum.ToString();
            sum = GlobalVars.globDelayTimes.ColdStep;
            CalibrateColdStepValue.Text = sum.ToString();


            F2MCalComputed.Text = GlobalVars.globDelayTimes.F2MCalibrateComputedStr;  
            F2MTuneComputed.Text = GlobalVars.globDelayTimes.F2MTuneComputedStr;
            F2MTotalComputed.Text = GlobalVars.globDelayTimes.F2MTotalComputedSumStr;  

            ColdF2MDelay.Text = GlobalVars.globDelayTimes.F2MCalibrateUsedStr;
            ColdF2MTuneUsed.Text = GlobalVars.globDelayTimes.F2MTuneUsedStr;
            ColdF2MTotalUsed.Text = GlobalVars.globDelayTimes.F2MUsedSumStr;

            //EA and VA delay times
            EACalText.Text = GlobalVars.globDelayTimes.EACalibrateStr;
            EATuneText.Text = GlobalVars.globDelayTimes.EATuneStr;
            EATotalText.Text = GlobalVars.globDelayTimes.EASumStr;
            VACalText.Text = GlobalVars.globDelayTimes.VACalibrateStr;
            VATuneText.Text = GlobalVars.globDelayTimes.VATuneStr;
            VATotalText.Text = GlobalVars.globDelayTimes.VASumStr;
            
            //set threshold levels (should have better names)
            TBufHotHLine.Y1 = GlobalVars.globThresholds.ThresholdHotBuffTankAlarmLimit;
            TBufMidHLine.Y1 = GlobalVars.globThresholds.ThresholdMidBuffTankAlarmLimit;
            


            //we can NOT ask two commands
            if (GlobalVars.globThresholds.ThresholdHot2Cold<0)
            {
                AppHelper.AskAOUForHot2ColdThreshold();
                TextBox_HotToColdThreshold.Text = "-";
                if (GlobalVars.globThresholds.ThresholdCold2Hot < 0)
                {
                    TextBox_ColdToHotThreshold.Text = "-";
                }
                else
                {
                    ColdToHotLineAnnotation.Y1 = GlobalVars.globThresholds.ThresholdCold2Hot;
                    TextBox_ColdToHotThreshold.Text = GlobalVars.globThresholds.ThresholdCold2Hot.ToString();
                }
                if (GlobalVars.globThresholds.ThresholdMidBuffTankAlarmLimit < 0)
                {
                    BufMidThresholdValue.Text = "-";
                }
                else
                {
                    BufMidThresholdValue.Text = GlobalVars.globThresholds.ThresholdMidBuffTankAlarmLimit.ToString();
                    TBufColdHLine.Y1 = GlobalVars.globThresholds.ThresholdColdTankBuffAlarmLimit;
                }
            }
            else
            {
                HotToColdLineAnnotation.Y1 = GlobalVars.globThresholds.ThresholdHot2Cold;
                TextBox_HotToColdThreshold.Text = GlobalVars.globThresholds.ThresholdHot2Cold.ToString();
                if (GlobalVars.globThresholds.ThresholdCold2Hot < 0)
                {
                    AppHelper.AskAOUForCold2HotThreshold();
                    TextBox_ColdToHotThreshold.Text = "-";
                }
                else
                {
                    ColdToHotLineAnnotation.Y1 = GlobalVars.globThresholds.ThresholdCold2Hot;
                    TextBox_ColdToHotThreshold.Text = GlobalVars.globThresholds.ThresholdCold2Hot.ToString();
                    if (GlobalVars.globThresholds.ThresholdMidBuffTankAlarmLimit < 0)
                    {
                        AppHelper.AskAOUForMidBufThreshold();
                        BufMidThresholdValue.Text = "-";
                    }
                    else
                    {
                        BufMidThresholdValue.Text = GlobalVars.globThresholds.ThresholdMidBuffTankAlarmLimit.ToString();
                        TBufColdHLine.Y1 = GlobalVars.globThresholds.ThresholdColdTankBuffAlarmLimit;
                    }
                }
            }
            





            //set tooltip contents
            TBufHotHLine.ToolTipContent = "Lower limit THotBuffer";
            TBufMidHLine.ToolTipContent = "Threshold TMidBuffer";
            TBufColdHLine.ToolTipContent = "Upper limit TColdBuffer";
            HotToColdLineAnnotation.ToolTipContent = "Threshold TRetActual hot" + " ↘ " + "cold";
            ColdToHotLineAnnotation.ToolTipContent = "Threshold TRetActual cold" + " ↗ " + "hot";
            
            //Set lineSeries colors
            Series_Delay_THotTank.Interior = new SolidColorBrush(Colors.Red);
           // Series_EB_THotTank.Interior = new SolidColorBrush(Colors.Red);
          //  Series_VB_THotBuffer.Interior = new SolidColorBrush(Colors.OrangeRed);
            Series_Delay_TColdTank.Interior = new SolidColorBrush(Colors.Blue);
           //Series_EB_TColdTank.Interior = new SolidColorBrush(Colors.Blue);
           // Series_VB_TColdBuffer.Interior = new SolidColorBrush(Colors.LightBlue);
           // Series_VB_TMidBuffer.Interior = new SolidColorBrush(Colors.Khaki);
            Series_Delay_TRetActual.Interior = new SolidColorBrush(Colors.Purple);
            Series_EB_TRetActual.Interior = new SolidColorBrush(Colors.Purple);
            //Series_Delay_TRetForecasted.Interior = new SolidColorBrush(Colors.Purple);
           Series_EB_ValveReturn.Interior = new SolidColorBrush(Colors.LightGreen);
        }


        private void MaintenancePage_Unloaded(object sender, RoutedEventArgs e)
        {
            dTimer.Stop();
            //clean data
            //Series_EB_THotTank.ItemsSource = null;
           //Series_EB_TColdTank.ItemsSource = null;
            //Series_EB_TRetActual.ItemsSource = null;
            Series_Delay_TRetActual.ItemsSource = null;
            Series_Delay_TColdTank.ItemsSource = null;
            Series_Delay_THotTank.ItemsSource = null;
         //   Series_VB_THotBuffer.ItemsSource = null;
         //  Series_VB_TMidBuffer.ItemsSource = null;
         //   Series_VB_TColdBuffer.ItemsSource = null;
        }

        private void MaintenancePage_Loaded(object sender, RoutedEventArgs e)
        {
            dTimer.Start();
        }

        private void InitDispatcherTimer()
        {
            dTimer = new DispatcherTimer();
            dTimer.Tick += UpdateTick;
            dTimer.Interval = new TimeSpan(0, 0, 1); //1 seconds
        }

        void UpdateTick(object sender, object e)
        {
            if (chartModel.power.Count == 0)
            {
                var powers = Data.Updater.GetAllPowerValues();
                if (Data.Updater.LastPowerIndex > 2)
                {
                    chartModel.SetValues(powers);
                    // HideProgress();
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


            if (doStepTimer > 0)
            {
                doStepTimer--;
                if (doStepTimer == 0)
                {
                    dTimer.Stop();
                    //todo: set back to Idle
                    //Data.Updater.SetCommandValue(AOUDataTypes.CommandType.RunningMode, (int)AOUDataTypes.AOURunningMode.Idle);
                    Data.Updater.SetCommandValue(AOUDataTypes.CommandType.runModeAOU,0);
                    //  HotStepButton.IsEnabled = true;
                    //  ColdStepButton.IsEnabled = true;
                    HotStepStatus.Text = "";
                }
            }
        }


        private void CalibrateFreezeVolumeChart(object sender, TappedRoutedEventArgs e)
        {
            //todo make work
        }

        private void CalibrateFreezeEnergyChart(object sender, TappedRoutedEventArgs e)
        {
            //todo make work
        }

        private void CalibrateFreezeDelayChart(object sender, TappedRoutedEventArgs e)
        {
            //todo make work
        }



        private void DoHotStep(object sender, RoutedEventArgs e)
        {
            //check if already ongoing
            if (doStepTimer > 0)
            {
                AppHelper.ShowMessageBox("Hot/Cold step already running");
                return;
            }

            int hotStepLength = (int)GlobalVars.globDelayTimes.HotStep;//AppHelper.ConvertToValidInteger(CalibrateHotStepValue.Text, 2, 25);
            //must check if in state IDLE. If not, display error message and return
        
            if (GlobalAppSettings.RunningMode != (int)AOUDataTypes.AOURunningMode.Idle)
            {
                AppHelper.ShowMessageBox("AOU must be in mode IDLE for this command");
                return;
            }
          
            if (hotStepLength < 1)
            {
                AppHelper.ShowMessageBox("No valid time value");
            }
            else
            {
                SetAxisRangeForTempStep(hotStepLength);
                //sent command and value to AOU 
                //plot Hot Step response for x + 1 seconds, sometimes we miss one update. 
                //the step time remains
                doStepTimer = hotStepLength + 1;
                HotStepStatus.Text = "Working...";
                dTimer.Start();
                Data.Updater.StartHotStep(hotStepLength*10); //must convert to deciseconds
               // HotStepButton.IsEnabled = false;
                //ColdStepButton.IsEnabled = false;
                //done! Freeze output in grid 
            }
        }

        private void DoColdStep(object sender, RoutedEventArgs e)
        {
            if (doStepTimer > 0)
            {
                AppHelper.ShowMessageBox("Hot/Cold step already running");
                return;
            }


            int coldStepLength = (int)GlobalVars.globDelayTimes.ColdStep;// AppHelper.ConvertToValidInteger(CalibrateColdStepValue.Text, 2, 25);

            if (GlobalAppSettings.RunningMode != (int)AOUDataTypes.AOURunningMode.Idle)
            {
                AppHelper.ShowMessageBox("AOU must be in mode IDLE for this command");
                return;
            }
            if (coldStepLength < 1)
            {
                AppHelper.ShowMessageBox("No valid time value");
            }
            else
            {
                SetAxisRangeForTempStep(coldStepLength);
                doStepTimer = coldStepLength +1;
                dTimer.Start(); //hope this works even if already running
                HotStepStatus.Text = "Working...";
                Data.Updater.StartColdStep(coldStepLength*10);
                //HotStepButton.IsEnabled = false;
                //ColdStepButton.IsEnabled = false;
                //done! 
            }
        }

        private void FreezeCalibrateGraphs(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (dTimer.IsEnabled && doStepTimer == 0)
            {
                dTimer.Stop();
            }
            else
            {
                //make axis auto again
                //todo: make work for all graphs
                CalibrateDelayXAxis.Minimum = null;
                CalibrateDelayXAxis.Maximum = null;
                CalibrateDelayXAxis.Interval = null;
                //and start the plotting
                dTimer.Start();
            }
        }

     

        private void CalibratePhaseVLine1_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            double myX1, myX2;
            Point myPoint1, myPoint2;
            double phaseDiff;
            //we take x-value from the line and any y-value should do
            myPoint1.X = AppHelper.SafeConvertToDouble(CalibratePhaseVLine1.X1);
            myPoint1.Y = 0;
            myX1 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myPoint1);
            //and the other line
            myPoint2.X = AppHelper.SafeConvertToDouble(CalibratePhaseVLine2.X1);
            myPoint2.Y = 0;
            myX2 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myPoint2);
            //calculate the difference in seconds
            //phaseDiff = (int)Math.Abs(myX2-myX1)/1000;
            phaseDiff = Math.Abs(myX2 - myX1) / 1000;
            //write result
            CalibratePhaseDiffResult.Text = phaseDiff.ToString("0.00") + " (s)";
        }

        private void CalibratePhaseVLine2_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            double myX1, myX2;
            Point myPoint1, myPoint2;
            double phaseDiff;
            //we take x-value from the line and any y-value should do
            myPoint1.X = AppHelper.SafeConvertToDouble(CalibratePhaseVLine1.X1);
            myPoint1.Y = 0;
            myX1 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myPoint1);
            //and the other line
            myPoint2.X = AppHelper.SafeConvertToDouble(CalibratePhaseVLine2.X1);
            myPoint2.Y = 0;
            myX2 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myPoint2);
            //calculate the difference in seconds
            phaseDiff = Math.Abs(myX2 - myX1) / 1000;
            //write result
            CalibratePhaseDiffResult.Text = phaseDiff.ToString("0.00") + " (s)";
        }

        private void SetAxisRangeForTempStep(int stepLength)
        {
            //need to know new starting value for x-axis
            //get curent time value
            TimeSpan TNow;
            if (Data.Updater.LastPowerIndex >= 0)
            {
                TNow = TimeSpan.FromMilliseconds(Data.Updater.LastPower.ElapsedTime);
            }
            else
            {
                TNow = new TimeSpan(100); // OBS. ticks not ms
            }

            //save this code a bit longer
            //myOrigin.X = MyDelayChart.SeriesClipRect.Right;
            //myOrigin.Y = 0;
            //double myX0;
            //myX0 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myOrigin); 

            //need to make the seconds in timespan integer
            int newX0InWholeSeconds = TNow.Seconds;
            int newX0InWholeMinutes = TNow.Minutes;
            int newX0InWholeHours = TNow.Hours;
            int newX0InWholeDays = TNow.Days;
            TimeSpan timeInWholeSeconds = new TimeSpan(newX0InWholeDays, newX0InWholeHours, newX0InWholeMinutes, newX0InWholeSeconds, 0);
            CalibrateDelayXAxis.Minimum = timeInWholeSeconds.ToString();
            CalibrateDelayXAxis.Interval = "00:00:02"; //two seconds will do
                                                       //use value  x in Textbox and set new length of x-axis in grid
            TimeSpan newMaximum = new TimeSpan(0, 0, 0, stepLength, 0) + TNow;
            CalibrateDelayXAxis.Maximum = newMaximum;
            //phase diff text no longer valid
            CalibratePhaseDiffResult.Text = "-";

        }

        private void DoCycle(object sender, RoutedEventArgs e)
        {
            //here we need to make x-axis auto again, and loop hot and cold steps
            AppHelper.ShowMessageBox("Not yet implemented: Cycle");
        }

        private void ColdToHotLineAnnotation_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //ask user if new threshold is OK
            string title = "Threshold TRetActual cold" + " ↗ " + "hot";
            string message = "You are about to set new threshold value to ";
            //set new value in textbox, will restore if cancel
            TextBox_ColdToHotThreshold.Text = AppHelper.SafeConvertToInt(ColdToHotLineAnnotation.Y1).ToString();
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUDataTypes.CommandType.TReturnThresholdCold2Hot, ColdToHotLineAnnotation, this);
        }

        // need to handle cancel button press in this page too
        public void Reset_ThresholdHot2Cold()
        {
            HotToColdLineAnnotation.Y1 = GlobalVars.globThresholds.ThresholdHot2Cold;
            TextBox_HotToColdThreshold.Text = GlobalVars.globThresholds.ThresholdHot2Cold.ToString();
        }

        public void Reset_ThresholdCold2Hot()
        {
            ColdToHotLineAnnotation.Y1 = GlobalVars.globThresholds.ThresholdCold2Hot;
            TextBox_ColdToHotThreshold.Text = GlobalVars.globThresholds.ThresholdCold2Hot.ToString();
        }

        private void HotToColdLineAnnotation_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Threshold TRetActual hot" + " ↘ " + "cold";
            string message = "You are about to set new threshold value to ";
            //set new value in textbox, will restore if cancel
            TextBox_HotToColdThreshold.Text = AppHelper.SafeConvertToInt(HotToColdLineAnnotation.Y1).ToString();
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUDataTypes.CommandType.TReturnThresholdHot2Cold, HotToColdLineAnnotation, this);
        }

        private void TBufHotHLine_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Buffer tank Hot temperature Lower limit";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUDataTypes.CommandType.TBufferHotLowerLimit, TBufHotHLine, this);
        }

        private void TBufMidHLine_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Buffer tank mid temperature threshold";
            string message = "You are about to set value to ";
            //set new value in textbox, will restore if cancel
            BufMidThresholdValue.Text = AppHelper.SafeConvertToInt(TBufMidHLine.Y1).ToString();
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUDataTypes.CommandType.TBufferMidRefThreshold, TBufMidHLine, this);
        }

        private void TBufColdHLine_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Buffer tank Cold temperature Upper limit";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUDataTypes.CommandType.TBufferColdUpperLimit, TBufColdHLine, this);
            int val = AppHelper.SafeConvertToInt(TBufColdHLine.Y1);
            TBufColdHLine.Y1 =val;        }


        public void Reset_ThresholdHotTankAlarm() //not a good name MW
        {
            TBufHotHLine.Y1 = GlobalVars.globThresholds.ThresholdHotBuffTankAlarmLimit;
        }

        public void Reset_ThresholdColdTankAlarm()
        {
            TBufColdHLine.Y1 = GlobalVars.globThresholds.ThresholdColdTankBuffAlarmLimit;
        }

        public void Reset_ThresholdMidTankAlarm()
        {
            TBufMidHLine.Y1 = GlobalVars.globThresholds.ThresholdMidBuffTankAlarmLimit;
            BufMidThresholdValue.Text = GlobalVars.globThresholds.ThresholdMidBuffTankAlarmLimit.ToString();

        }


        private void HotFeedToReturnDelayTime_GotFocus(object sender, RoutedEventArgs e)
        {
            //show slider and send command to AOU
            AppHelper.GetValueToTextBox((TextBox)sender, null, "Change Hot feed-to-return delay time", AOUDataTypes.CommandType.hotDelayTime, 0, 30, 0.5, this);         
        }

        private void ColdFeedToReturnDelayTime_GotFocus(object sender, RoutedEventArgs e)
        {
            //show slider and send command to AOU
            AppHelper.GetValueToTextBox((TextBox)sender, null, "Change Cold feed-to-return delay time", AOUDataTypes.CommandType.coldDelayTime, 0, 30, 0.5, this);
        }

        public void AsyncResponseDlg(AOUDataTypes.CommandType cmd, bool ok)
        {
            if (!ok)
            {
                switch (cmd)
                {
                    case AOUDataTypes.CommandType.coolingTime: break; // TODO: Reset old value saved in GlobalAppSettings
                }
                AppHelper.ShowMessageBox("Command not sent. Old value restored");
            }
        }

        private void HotFeedToReturnDelayTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            //do we need to change the sum? All times are now double, not int

            TextBlock_SumHotDelayTime.Text = GlobalVars.globDelayTimes.HotDelayTimeSumStr;
            //need to update calculated values too
            F2MCalComputed.Text = GlobalVars.globDelayTimes.F2MCalibrateComputedStr;
            F2MTotalComputed.Text = GlobalVars.globDelayTimes.F2MTotalComputedSumStr;
        }

        private void ColdFeedToReturnDelayTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            //do we need to change the sum?
            TextBlock_SumColdDelayTime.Text = GlobalVars.globDelayTimes.ColdDelayTimeSumStr;
        }

        private void TextBox_HotToColdThreshold_GotFocus(object sender, RoutedEventArgs e)
        {
            //show slider and send command to AOU
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)CalibrateColdStepValue, "Threshold TRetActual hot" + " ↘ " + "cold", AOUDataTypes.CommandType.TReturnThresholdHot2Cold, 0, 300, 1,this);
        }

        private void TextBox_ColdToHotThreshold_GotFocus(object sender, RoutedEventArgs e)
        {
            //show slider and send command to AOU
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)CalibrateColdStepValue, "Threshold TRetActual cold" + " ↗ " + "hot", AOUDataTypes.CommandType.TReturnThresholdCold2Hot, 0, 300, 1, this);
            //todo update line
        }

        private void BufMidThresholdValue_GotFocus(object sender, RoutedEventArgs e)
        {
            //show slider and send command to AOU
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)CalibrateColdStepValue, "Threshold TMidBuffer", AOUDataTypes.CommandType.TBufferMidRefThreshold, 0, 300, 1,this);
            //todo update line
        }

        private void Button_Freeze_Run_Click(object sender, RoutedEventArgs e)
        {
            if (dTimer.IsEnabled && doStepTimer == 0)
            {
                dTimer.Stop();
               // Button_Freeze_Run.Content = "Run";
            }
            else
            {
                //if in a step, do nothing
                if (doStepTimer > 0)
                    return;
                //else make axis auto again
                //todo: make work for all graphs
                CalibrateDelayXAxis.Minimum = null;
                CalibrateDelayXAxis.Maximum = null;
                CalibrateDelayXAxis.Interval = null;
                //and start the plotting
                dTimer.Start();
                //Button_Freeze_Run.Content = "Freeze";
            }
        }

        private void ColdF2MDelay_TextChanged(object sender, TextChangedEventArgs e)
        {

            ColdF2MTotalUsed.Text = GlobalVars.globDelayTimes.F2MUsedSumStr;
        }

        private void ColdF2MDelay_GotFocus(object sender, RoutedEventArgs e)
        {
            //show slider and send command to AOU
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)FocusHere, "Feed-To-Mould delay time", AOUDataTypes.CommandType.coldFeed2MouldDelayTime, 0, 30, 0.5,this);
        }

        private void EACalText_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)FocusHere, "Offset Return valve Switch time", AOUDataTypes.CommandType.offsetHotFeed2RetValveTime, 0, 30, 0.5,this);
        }

        private void EACalText_TextChanged(object sender, TextChangedEventArgs e)
        {
            EATotalText.Text = GlobalVars.globDelayTimes.EASumStr;
        }

        private void VACalText_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)FocusHere, "Offset Return valve Switching period", AOUDataTypes.CommandType.offsetRetValveHotPeriod, 0, 30, 0.5, this);
        }

        private void VACalText_TextChanged(object sender, TextChangedEventArgs e)
        {
            VATotalText.Text = GlobalVars.globDelayTimes.VASumStr;
        }

        private void CalibrateHotStepValue_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, null, "Hot Step time ", AOUDataTypes.CommandType.runModeHeating, 0, 30, 1, this, false);
        }

        private void CalibrateColdStepValue_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)FocusHere, "Cold Step time ", AOUDataTypes.CommandType.runModeCooling, 0, 30,1, this, false);
        }

        private void CalibratePhaseVLine1_DragDelta(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragDeltaEventArgs e)
        {
            double myX1, myX2;
            Point myPoint1, myPoint2;
            double phaseDiff;
            //we take x-value from the line and any y-value should do
            myPoint1.X = AppHelper.SafeConvertToDouble(CalibratePhaseVLine1.X1);
            myPoint1.Y = 0;
            myX1 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myPoint1);
            //and the other line
            myPoint2.X = AppHelper.SafeConvertToDouble(CalibratePhaseVLine2.X1);
            myPoint2.Y = 0;
            myX2 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myPoint2);
            //calculate the difference in seconds
            //phaseDiff = (int)Math.Abs(myX2-myX1)/1000;
            phaseDiff = Math.Abs(myX2 - myX1) / 1000;
            //write result
            CalibratePhaseDiffResult.Text = phaseDiff.ToString("0.00") + " (s)";
        }

        private void CalibratePhaseVLine2_DragDelta(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragDeltaEventArgs e)
        {
            double myX1, myX2;
            Point myPoint1, myPoint2;
            double phaseDiff;
            //we take x-value from the line and any y-value should do
            myPoint1.X = AppHelper.SafeConvertToDouble(CalibratePhaseVLine1.X1);
            myPoint1.Y = 0;
            myX1 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myPoint1);
            //and the other line
            myPoint2.X = AppHelper.SafeConvertToDouble(CalibratePhaseVLine2.X1);
            myPoint2.Y = 0;
            myX2 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myPoint2);
            //calculate the difference in seconds
            phaseDiff = Math.Abs(myX2 - myX1) / 1000;
            //write result
            CalibratePhaseDiffResult.Text = phaseDiff.ToString("0.00") + " (s)";
        }
    }
}
