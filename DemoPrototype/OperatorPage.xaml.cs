﻿using System;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OperatorPage : Page
    {
        private bool isInitSelection = true; // ToDo. Better check
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

        private async void modalDlg(string title, string message)
        {
            var dlg = new ContentDialog();
            dlg.Title = title;
            dlg.Content = message;
            dlg.PrimaryButtonText = "Enable";
            dlg.SecondaryButtonText = "Cancel";

            ContentDialogResult res = await dlg.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                switch (title)
                {
                    //kommunicera med PLC?
                    case "": break; // example. sendToPLC 
                }

            }

        }

        private void ShowHotTankSlider(object sender, RoutedEventArgs e)
        {
       //     SetHotTankSlider.Visibility="True";
        }

        private void NewModeSelected(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)e.AddedItems[0];
            string modeTitle = item.Content.ToString();
            string message = "You are about to change running mode";
            if (!isInitSelection)
            {
                modalDlg(modeTitle, message);
            }
            else
            {
                isInitSelection = false;
            }
        }

        private void PhaseShiftButton(object sender, RoutedEventArgs e)
        {
            //Just testing
           

            //PhaseShiftResultText.Text = firstSlope.ToString();
            //PhaseShiftValue.Text = (secondSlope-firstSlope).ToString();
        }

        private void PhaseLine1_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //Urban please replace this code with code showing diff between the lines, and center the Chartstripline
           // double firstSlope = AppHelper.SafeConvertToDouble(PhaseVLine2.X1);
            //double secondSlope = AppHelper.SafeConvertToDouble(PhaseVLine1.X1);
            PhaseDiffResult.Text = "5"; //(secondSlope - firstSlope).ToString();
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
            TimeSpan newX1 = AppHelper.SafeConvertToTimeSpan(MouldingDelayVLine1.X1);
            TimeSpan deltaX = newX1 - AppHelper.SafeConvertToTimeSpan(MouldingDelayVLine2.X1);
            PhaseDiffResult.Text = Math.Abs(deltaX.Seconds).ToString() + " (s)";
        }

        private void MouldingDelayVLine2_DragCompleted(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //calculate diff between lines and show result in overlaying text
            TimeSpan newX1 = (TimeSpan)MouldingDelayVLine1.X1;
            TimeSpan deltaX = newX1 - (TimeSpan)MouldingDelayVLine2.X1;
            PhaseDiffResult.Text = Math.Abs(deltaX.Seconds).ToString() + " (s)";
        }
    }


}
