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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalibratePage : Page
    {
        private DispatcherTimer dTimer;

        private int calibrationTime = 0;

        public CalibratePage()
        {
            this.Loaded += MaintenancePage_Loaded;
            this.Unloaded += MaintenancePage_Unloaded;

            this.InitializeComponent();
            this.Name = "CalibratePage";

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
            DataUpdater.UpdateInputData(CalibrateGrid.DataContext);
            if (calibrationTime > 0)
            {
                calibrationTime--;
                if (calibrationTime == 0)
                {
                    dTimer.Stop();
                    HotStepButton.IsEnabled = true;
                    ColdStepButton.IsEnabled = true;
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
            int hotStepLength = AppHelper.ConvertToValidInteger(CalibrateHotStepValue.Text, 5, 25);
            if (hotStepLength == -1)
            {
                AppHelper.ShowMessageBox("No valid time value");
            }
            else
            {
                SetAxisRangeForTempStep(hotStepLength);
                //sent command and value to AOU 
                //plot Hot Step response for x seconds
                calibrationTime = hotStepLength;
                DataUpdater.StartHotStep(hotStepLength);
                HotStepButton.IsEnabled = false;
                ColdStepButton.IsEnabled = false;
                //done! Freeze output in grid 
            }
        }

        private void DoColdStep(object sender, RoutedEventArgs e)
        {
            int coldStepLength = AppHelper.ConvertToValidInteger(CalibrateColdStepValue.Text, 5, 10);
            DataUpdater.StartHotStep(coldStepLength);
            if (coldStepLength == -1)
            {
                AppHelper.ShowMessageBox("No valid time value");
            }
            else
            {
                SetAxisRangeForTempStep(coldStepLength);
                calibrationTime = coldStepLength;
                DataUpdater.StartColdStep(coldStepLength);
                HotStepButton.IsEnabled = false;
                ColdStepButton.IsEnabled = false;
                //done! 
            }
        }

        private void FreezeCalibrateGraphs(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (dTimer.IsEnabled && calibrationTime == 0)
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
            int phaseDiff;
            //we take x-value from the line and any y-value should do
            myPoint1.X = AppHelper.SafeConvertToDouble(CalibratePhaseVLine1.X1);
            myPoint1.Y = 0;
            myX1 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myPoint1);
            //and the other line
            myPoint2.X = AppHelper.SafeConvertToDouble(CalibratePhaseVLine2.X1);
            myPoint2.Y = 0;
            myX2 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myPoint2);
            //calculate the difference in seconds
            phaseDiff = (int)Math.Abs(myX2-myX1)/1000;
            //write result
            CalibratePhaseDiffResult.Text = phaseDiff.ToString() + " (s)";
        }

        private void CalibratePhaseVLine2_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            double myX1, myX2;
            Point myPoint1, myPoint2;
            int phaseDiff;
            //we take x-value from the line and any y-value should do
            myPoint1.X = AppHelper.SafeConvertToDouble(CalibratePhaseVLine1.X1);
            myPoint1.Y = 0;
            myX1 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myPoint1);
            //and the other line
            myPoint2.X = AppHelper.SafeConvertToDouble(CalibratePhaseVLine2.X1);
            myPoint2.Y = 0;
            myX2 = this.MyDelayChart.PointToValue(MyDelayChart.PrimaryAxis, myPoint2);
            //calculate the difference in seconds
            phaseDiff = (int)Math.Abs(myX2 - myX1) / 1000;
            //write result
            CalibratePhaseDiffResult.Text = phaseDiff.ToString() + " (s)";
        }

        private void SetAxisRangeForTempStep(int stepLength)
        {
            //need to know new starting value for x-axis
            //get curent time value
            TimeSpan TNow;
            if (CalibrateGrid.DataContext != null)
            {
                TNow = ((LineChartViewModel)CalibrateGrid.DataContext).GetActualTimeSpan();
            }
            else
            {
                TNow = new TimeSpan(0);
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

        }

        private void DoCycle(object sender, RoutedEventArgs e)
        {
            //here we need to make x-axis auto again, and loop hot and cold steps
            AppHelper.ShowMessageBox("Not yet implemented: Cycle");
        }

       

        private void ColdToHotLineAnnotation_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            double newThreshold = 0;
            if (ColdToHotLineAnnotation.Y1 != null)
            {
                newThreshold = (double)ColdToHotLineAnnotation.Y1;
            }
            //ask user if new threshold is OK
            string title = "Calibrate";
            string message = "You are about to change Cold to Hot valve Return threshold";

            int value = AppHelper.SafeConvertToInt(ColdToHotLineAnnotation.Y1);
            int oldValue = 0; // Todo
            DataUpdater.VerifySendToAOUDlg(title, message, AOUTypes.CommandType.TBufferHotLowerLimit, DataUpdater.VerifyDialogType.VerifyIntValue, (Page)this, value, oldValue);
        }

        private void HotToColdLineAnnotation_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            double newThreshold = 0;
            if (HotToColdLineAnnotation.Y1 != null)
            {
                newThreshold = (double)HotToColdLineAnnotation.Y1;
            }
        }

        private void TBufHotHLine_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Buffer tank hot temperature lower limit";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUTypes.CommandType.TBufferHotLowerLimit, TBufHotHLine, this, GlobalVars.globThresholds.ThresholdHot2Cold);
        }

        private void TBufMidHLine_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Buffer tank mid temperature threshold";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUTypes.CommandType.TBufferMidRefThreshold, TBufMidHLine, this, 0); // ToDo OldValue
        }

        private void TBufColdHLine_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Buffer tank cold temperature upper limit";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUTypes.CommandType.TBufferColdUpperLimit, TBufColdHLine, this, 0); // ToDo OldValue
        }

        private void HotFeedToReturnDelayTime_GotFocus(object sender, RoutedEventArgs e)
        {
            //show slider and send command to AOU
            AppHelper.GetValueToTextBox((TextBox)sender, null, "Change Hot feed-to-return delay time", AOUTypes.CommandType.TReturnThresholdCold2Hot, 0, 30);         
        }

        private void ColdFeedToReturnDelayTime_GotFocus(object sender, RoutedEventArgs e)
        {
            //show slider and send command to AOU
            AppHelper.GetValueToTextBox((TextBox)sender, null, "Change Cold feed-to-return delay time", AOUTypes.CommandType.TReturnThresholdCold2Hot, 0, 30);
        }

        public void AsyncResponseDlg(AOUTypes.CommandType cmd, bool ok)
        {
            if (!ok)
            {
                switch (cmd)
                {
                    case AOUTypes.CommandType.coolingTime: break; // TODO: Reset old value saved in GlobalAppSettings
                }
                AppHelper.ShowMessageBox("Command not sent. Old value restored");
            }
        }

    }
}
