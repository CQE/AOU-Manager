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

        private string[] runningModes = new string[]
        {
            "Idle", // AOUTypes.CommandType.RunningModeIdle
            "Heating", // RunningModeHeating
            "Cooling", // RunningModeCooling
            "Fixed Cycling", // RunningModefixedCycling
            "Auto with IMM" // RunningModeAutoWidthIMM
        };

        public OperatorPage()
        {
            this.Loaded += MaintenancePage_Loaded;
            this.Unloaded += MaintenancePage_Unloaded;

            this.InitializeComponent();
            this.Name = "OperatorPage";

            foreach (string mode in runningModes)
            {
                RunningModeCombo.Items.Add(mode);
            }
            RunningModeCombo.SelectedIndex = 0; // Idle

            // PhaseHLine1.ToolTipContent = "Drag to change Buffer tank hot temperature lower limit";
            // PhaseHLine2.ToolTipContent = "Drag to change Buffer tank cold temperature upper limit";
            // PhaseHLineTBM.ToolTipContent = "Drag to change Buffer tank mid temperature threshold";
            // Set tooltip contents
            HLineSetHotTankAlarmThreshold.ToolTipContent = "Threshold hot"+ " ↘ " + "cold";
            ColdTankHLine.ToolTipContent= "Threshold cold" + " ↗ " + "hot";
            //this is NOT correct just testing my idea
           
            HLineSetHotTankAlarmThreshold.Y1 = GlobalVar.ThresholdHot2Cold;


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
            string title = "Buffer tank cold temperature upper limit";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUTypes.CommandType.TBufferColdUpperLimit, PhaseHLine2, this);

            //Urban please replace this code with code showing diff between the lines, and center the Chartstripline
            PhaseDiffResult.Text = "pl2, Cold";
        }

        private void PhaseLineTBM_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Buffer tank mid temperature threshold";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUTypes.CommandType.TBufferMidRefThreshold, PhaseHLineTBM, this);
        }
        private void ColdTankHLine_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title = "Cold tank temperature threshold";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUTypes.CommandType.TColdTankAlarmHighThreshold, ColdTankHLine, this);
        }

        private void HLineSetHotTankAlarmThreshold_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            string title =  "Threshold hot" + " ↘ " + "cold";
            string message = "You are about to set value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUTypes.CommandType.THotTankAlarmLowThreshold, HLineSetHotTankAlarmThreshold, this);
            //save new value to global var, todo: Urban handle cancel too asap
            GlobalVar.ThresholdHot2Cold = (int)AppHelper.SafeConvertToXCoordinate(HLineSetHotTankAlarmThreshold.Y1);
        }

        private void MouldingDelayVLine1_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //calculate diff between lines and show result in overlaying text
          
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
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, "Change Hot Tank Value", AOUTypes.CommandType.tempHotTankFeedSet, 100, 300);
        }

        private void NewTColdTankTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, "Change Cold Tank Value", AOUTypes.CommandType.tempColdTankFeedSet, 0, 30);
        }

        private void NewActiveHeatingTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, "Active heating time", AOUTypes.CommandType.heatingTime, 0, 30);
        }

        private void NewPauseHeatingTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, "Heating pause time", AOUTypes.CommandType.toolHeatingFeedPause, 0, 30);
        }

        private void NewActiveCoolingTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, "Active cooling time", AOUTypes.CommandType.coolingTime, 0, 30);
        }

        private void NewPauseCoolingTimeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, "Cooling pause time", AOUTypes.CommandType.toolCoolingFeedPause, 0, 30);
        }

        private void HotFeedToReturnDelayCalTime_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, "Hot return delay time", AOUTypes.CommandType.hotDelayTime, 0, 30);
        }

        private void ColdFeedToReturnDelayCalTime_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, "Cold return delay time", AOUTypes.CommandType.coldDelayTime, 0, 30);
        }

        private void SetHotSafeZoneLine_DragDelta(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragDeltaEventArgs e)
        {
            //what is the new position of the line?
            double newY = AppHelper.SafeConvertToXCoordinate(SetHotSafeZoneLine.Y1); //(double)SetHotSafeZoneLine.Y1;
            //make chart strip line follow the line when dragged
            HotSafeZone.Start = newY;
        }

        private void SetHotSafeZoneLine_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //TBD set new threshold value
            string title = "Hot tank safe zone";
            string message = "You are about to set alarm value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUTypes.CommandType.CmdTypeToDo, SetHotSafeZoneLine, this);
        }

        private void SetColdSafeZoneLine_DragDelta(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragDeltaEventArgs e)
        {
            //what is the new position of the line?
            double newY = AppHelper.SafeConvertToXCoordinate(SetColdSafeZoneLine.Y1); //(double)SetHotSafeZoneLine.Y1;
            //make chart strip line follow the line when dragged
            ColdSafeZone.Start = newY;
        }

        private void SetColdSafeZoneLine_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            // TBD set new threshold value
            string title = "Cold tank safe zone";
            string message = "You are about to set alarm value to ";
            AppHelper.SetLimitValueFromHorizontalLine(title, message, AOUTypes.CommandType.CmdTypeToDo, SetColdSafeZoneLine, this);
        }
    }

}
