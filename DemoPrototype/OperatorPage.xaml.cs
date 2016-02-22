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
using DataHandler;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OperatorPage : Page
    {
        private int prevRunModeSelected = -1; 

        DispatcherTimer dTimer;

        public OperatorPage()
        {
            this.Loaded += MaintenancePage_Loaded;
            this.Unloaded += MaintenancePage_Unloaded;

            this.InitializeComponent();

            //set initial values for temperature unit
            if (GlobalAppSettings.IsCelsius)
            {
                TextCorF.Text = " (°C)";
            }
            else
            {
                TextCorF.Text = " (°F)";
            }

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
            DataUpdater.UpdateInputData(mainGrid.DataContext);
        }

        private void ShowHotTankSlider(object sender, RoutedEventArgs e)
        {
       //     SetHotTankSlider.Visibility="True";
        }

        public void AsyncResponseDlg(AOUTypes.CommandType cmd, bool ok)
        {
            if (ok)
            {
                if (cmd <= AOUTypes.CommandType.RunningModeAutoWidthIMM)
                {
                    prevRunModeSelected = RunningModeCombo.SelectedIndex;
                }
            }
            else
            {
                if (cmd <= AOUTypes.CommandType.RunningModeAutoWidthIMM)
                {
                    int oldIndex = prevRunModeSelected;
                    prevRunModeSelected = -1; // Reset to old active mode. Prevent NewModeSelected
                    RunningModeCombo.SelectedIndex = oldIndex;
                }
                else
                {
                    switch (cmd)
                    {
                        case AOUTypes.CommandType.coolingTime: break; // ToDo Reset old value saved in GloabaAppSettings
                    }

                }
                AppHelper.ShowMessageBox("Command not sent. Old value restored");
            }
        }

        private void NewModeSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (prevRunModeSelected != -1) // Not first time when selected is default value
                {
                    string modeTitle = RunningModeCombo.Items[RunningModeCombo.SelectedIndex].ToString();
                    string message = "You are about to change running mode";
                    DataUpdater.VerifySendToAOUDlg(modeTitle, message, (AOUTypes.CommandType)RunningModeCombo.SelectedIndex,
                                                    DataUpdater.VerifyDialogType.VeryfyOkCancelOnly, this);
                }
                else
                {
                    prevRunModeSelected = RunningModeCombo.SelectedIndex;
                }
            }
        }

        private void PhaseLine1_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Buffer tank hot temperature lower limit";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUTypes.CommandType.TBufferHotLowerLimit, PhaseHLine1, this);
        }

        private void PhaseLine2_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //Urban please replace this code with code showing diff between the lines, and center the Chartstripline
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            /* Stop timer and clean up
            dTimer.Stop();
            dTimer = null;
            mainGrid.DataContext = null;
            */
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
                //where are the lines?
                //double firstSlope = AppHelper.SafeConvertToDouble(PhaseVLine2.X1);
                //double secondSlope = AppHelper.SafeConvertToDouble(PhaseVLine1.X1);
                //and what is min on the X-axis?
                Double startX = AppHelper.SafeConvertToDouble(OperatorDelayXAxis.Minimum);
            }
            else dTimer.Start();
        }

        private void PhaseLineTBM_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //TBD
        }

        private void MouldingDelayVLine1_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //calculate diff between lines and show result in overlaying text
            //TimeSpan newX1 = AppHelper.SafeConvertToTimeSpan(MouldingDelayVLine1.X1);
            //TimeSpan deltaX = newX1 - AppHelper.SafeConvertToTimeSpan(MouldingDelayVLine2.X1);
            //PhaseDiffResult.Text = Math.Abs(deltaX.Seconds).ToString() + " (s)";
            double myX1, myX2;
            Point myPoint1, myPoint2;
            int phaseDiff;
            //we take x-value from the line and any y-value should do
            myPoint1.X = AppHelper.SafeConvertToXCoordinate(MouldingDelayVLine1.X1);
            myPoint1.Y = 0;
            myX1 = this.MyTuneChart.PointToValue(MyTuneChart.PrimaryAxis, myPoint1);
            //and the other line
            myPoint2.X = AppHelper.SafeConvertToXCoordinate(MouldingDelayVLine2.X1);
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
            //TimeSpan newX1 = (TimeSpan)MouldingDelayVLine1.X1;
            //TimeSpan deltaX = newX1 - (TimeSpan)MouldingDelayVLine2.X1;
            //PhaseDiffResult.Text = Math.Abs(deltaX.Seconds).ToString() + " (s)";

            double myX1, myX2;
            Point myPoint1, myPoint2;
            int phaseDiff;
            //we take x-value from the line and any y-value should do
            myPoint1.X = AppHelper.SafeConvertToXCoordinate(MouldingDelayVLine1.X1);
            myPoint1.Y = 0;
            myX1 = this.MyTuneChart.PointToValue(MyTuneChart.PrimaryAxis, myPoint1);
            //and the other line
            myPoint2.X = AppHelper.SafeConvertToXCoordinate(MouldingDelayVLine2.X1);
            myPoint2.Y = 0;
            myX2 = this.MyTuneChart.PointToValue(MyTuneChart.PrimaryAxis, myPoint2);
            //calculate the difference in seconds
            phaseDiff = (int)Math.Abs(myX2 - myX1) / 1000;
            //write result
            PhaseDiffResult.Text = phaseDiff.ToString() + " (s)";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        /* GotFocus works better than TextChanged 
        private void NewTHotTankTextBox_TextChanged(object sender, TextChangedEventArgs e) 
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, "Change Hot Tank Value", 0, 300);
            // AppHelper.ShowMessageBox("Just testing SetHotTank");
        }

        private void NewTColdTankTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, "Change Cold Tank Value", 0, 300);
            // AppHelper.ShowMessageBox("Just testing SetColdTank");
        }
        */

        private void NewTHotTankTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, "Change Hot Tank Value", AOUTypes.CommandType.tempHotTankFeedSet, 100, 300);
        }

        private void NewTColdTankTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, "Change Cold Tank Value", AOUTypes.CommandType.tempColdTankFeedSet, 0, 30);
        }

        private void NewActiveHeatingTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppHelper.ShowMessageBox("Just testing NewHeatingTime");
        }

        private void NewPauseHeatingTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppHelper.ShowMessageBox("Just testing NewHeatingPause");
        }

        private void NewActiveCoolingTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppHelper.ShowMessageBox("Just testing NewCoolingTime");
        }

        private void NewPauseCoolingTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppHelper.ShowMessageBox("Just testing NewCoolingPause");
        }

        private void HotFeedToReturnDelayCalTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppHelper.ShowMessageBox("Just testing HotDelayTime");
        }

        private void ColdFeedToReturnDelayCalTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            AppHelper.ShowMessageBox("Just testing ColdDelayTime");
        }
    }


}
