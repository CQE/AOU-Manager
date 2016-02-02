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

        private void CalibratePhaseLine1_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //todo make work
        }

        private void CalibratePhaseLine2_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //todo make work
        }
        
        private void DoHotStep(object sender, RoutedEventArgs e)
        {
            int hotStepLength = AppHelper.ConvertToInteger(CalibrateHotStepValue.Text, 5, 20);
            if (hotStepLength == -1)
            {
                AppHelper.ShowMessageBox("No valid time value");
            }
            else
            {
                //use value  x in Textbox and set length of X-axis in grid
                //sent command and value to AOU 
                //plot Hot Step response for x seconds
                CalibrateDelayXAxis.MaxWidth = hotStepLength;
                CalibrateDelayXAxis.Interval = 1;
                calibrationTime = hotStepLength;
                DataUpdater.StartHotStep(hotStepLength);
                HotStepButton.IsEnabled = false;
                ColdStepButton.IsEnabled = false;
                //done! Freeze output in grid 
            }
        }

        private void DoColdStep(object sender, RoutedEventArgs e)
        {
            int coldStepLength = AppHelper.ConvertToInteger(CalibrateColdStepValue.Text, 5, 10);
            DataUpdater.StartHotStep(coldStepLength);
            if (coldStepLength == -1)
            {
                AppHelper.ShowMessageBox("No valid time value");
            }
            else
            {
                //use value  x in Textbox and set length of X-axis in grid
                //sent command and value to AOU 
                //plot Hot Step response for x seconds
                CalibrateDelayXAxis.MaxWidth = coldStepLength;
                CalibrateDelayXAxis.Interval = 1;
                calibrationTime = coldStepLength;
                DataUpdater.StartColdStep(coldStepLength);
                HotStepButton.IsEnabled = false;
                ColdStepButton.IsEnabled = false;
                //done! Freeze output in grid//use value in Textbox and set length of X-axis in grid
            }
        }

        private void FreezeCalibrateGraphs(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (dTimer.IsEnabled && calibrationTime == 0)
            {
                dTimer.Stop();
                // double firstSlope = AppHelper.SafeConvertToDouble(PhaseVLine2.X1);
            }
            else dTimer.Start();
        }

        private void PhaseHLine2_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {

        }

        private void PhaseHLine3_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //todo make work
            CalColdEndLimitValue.Text = "42";
        }
    }
}
