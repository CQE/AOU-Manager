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
using Syncfusion.UI.Xaml.Charts;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TunePage : Page
     {

        private int prevRunModeSelected = -1; // First time or
        private DispatcherTimer dTimer;
        private int doStepTimer = 0;
        private LineChartViewModel chartModel;

        public TunePage()
        {
            this.Loaded += TunePage_Loaded;
            this.Unloaded += TunePage_Unloaded;
            this.InitializeComponent();
            this.Name = "TunePage";
            //load all data to power
            try
            {
                chartModel = new LineChartViewModel();
                // Connect Datacontext to chartModel
                TuneGrid.DataContext = chartModel;

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

            //Create three whole charts using code behind

            //Tune delay chart
            SfChart myTuneChart = new SfChart();
            //myTuneChart.Header = "Feed and Return delay timings";
            
            //Adding horizontal and vertical axis to the chart
            TimeSpanAxis tuneChartCategoryAxis = new TimeSpanAxis();
            tuneChartCategoryAxis.Name = "OperatorDelayAxis";
            myTuneChart.PrimaryAxis = tuneChartCategoryAxis;
            NumericalAxis tuneChartNumericalAxis = new NumericalAxis();
            myTuneChart.SecondaryAxis = tuneChartNumericalAxis;
           // myTuneChart.SecondaryAxis.VisibleRange.mi
            
            //add series to chart
            SplineSeries Series_Delay_TRetActual = new SplineSeries();
            Series_Delay_TRetActual.ItemsSource = chartModel.power;
            Series_Delay_TRetActual.Label = "TRetActual";
            Series_Delay_TRetActual.XBindingPath = "ElapsedTime";
            Series_Delay_TRetActual.YBindingPath = "TReturnActual";
            Series_Delay_TRetActual.Interior = new SolidColorBrush(Colors.Purple);
            myTuneChart.Series.Add(Series_Delay_TRetActual);

            //SplineSeries Series_Delay_THotTank = new SplineSeries();
            //Series_Delay_THotTank.ItemsSource = chartModel.power;
            //Series_Delay_THotTank.Label = "THotTank";
            //Series_Delay_THotTank.XBindingPath = "ElapsedTime";
            //Series_Delay_THotTank.YBindingPath = "THotTank";
            //Series_Delay_THotTank.Interior = new SolidColorBrush(Colors.Red);
            //myTuneChart.Series.Add(Series_Delay_THotTank);

            StepLineSeries Series_Delay_FeedHotValve = new StepLineSeries();
            Series_Delay_FeedHotValve.ItemsSource = chartModel.power;
            Series_Delay_FeedHotValve.Label = "FeedHotValve";
            Series_Delay_FeedHotValve.XBindingPath = "ElapsedTime";
            Series_Delay_FeedHotValve.YBindingPath = "ValveFeedHot";
            Series_Delay_FeedHotValve.Interior = new SolidColorBrush(Colors.Orange);
            myTuneChart.Series.Add(Series_Delay_FeedHotValve);

            StepLineSeries Series_Delay_FeedColdValve = new StepLineSeries();
            Series_Delay_FeedColdValve.ItemsSource = chartModel.power;
            Series_Delay_FeedColdValve.Label = "FeedColdValve";
            Series_Delay_FeedColdValve.XBindingPath = "ElapsedTime";
            Series_Delay_FeedColdValve.YBindingPath = "ValveFeedCold";
            Series_Delay_FeedColdValve.Interior = new SolidColorBrush(Colors.LightBlue);
            myTuneChart.Series.Add(Series_Delay_FeedColdValve);


            //SplineSeries Series_Delay_TColdTank = new SplineSeries();
            //Series_Delay_TColdTank.ItemsSource = chartModel.power;
            //Series_Delay_TColdTank.Label = "TColdTank";
            //Series_Delay_TColdTank.XBindingPath = "ElapsedTime";
            //Series_Delay_TColdTank.YBindingPath = "TColdTank";
            //Series_Delay_TColdTank.Interior = new SolidColorBrush(Colors.Blue);
            //myTuneChart.Series.Add(Series_Delay_TColdTank);

            StepLineSeries Series_Delay_ValveReturn = new StepLineSeries();
            Series_Delay_ValveReturn.ItemsSource = chartModel.power;
            Series_Delay_ValveReturn.Label = "ValveReturn";
            Series_Delay_ValveReturn.XBindingPath = "ElapsedTime";
            Series_Delay_ValveReturn.YBindingPath = "ValveReturn";
            Series_Delay_ValveReturn.Interior = new SolidColorBrush(Colors.YellowGreen);
            myTuneChart.Series.Add(Series_Delay_ValveReturn);



            //add legends
            ChartLegend myDelayLegend = new ChartLegend();
            myTuneChart.Legend = myDelayLegend;

            //margin
            myTuneChart.Margin = new Thickness(10,30,10,30);

            //add chart to grid
          /*  this.TuneGrid.Children.Add(myTuneChart);
            Grid.SetRow(myTuneChart, 0);
            Grid.SetRowSpan(myTuneChart, 8);
            Grid.SetColumn(myTuneChart, 0);
            Grid.SetColumnSpan(myTuneChart, 6);
         */   //Grid.SetMarginProperty(myTuneChart,"10,10,10,10");
            
           
           
            //Energy Balance Chart
            SfChart myEnergyChart = new SfChart();
            myEnergyChart.Header = "Energy (Hot and Cold tanks) (°C)";
            
            //Adding horizontal and vertical axis to the chart
            TimeSpanAxis energyChartCategoryAxis = new TimeSpanAxis();
            energyChartCategoryAxis.Name = "OperatorDelayAxis";
            myEnergyChart.PrimaryAxis = energyChartCategoryAxis;
            NumericalAxis energyChartNumericalAxis = new NumericalAxis();
            myEnergyChart.SecondaryAxis = energyChartNumericalAxis;
         
            //add series to chart
            SplineSeries Series_EB_TRetActual = new SplineSeries();
            Series_EB_TRetActual.ItemsSource = chartModel.power;
            Series_EB_TRetActual.Label = "TRetActual";
            Series_EB_TRetActual.XBindingPath = "ElapsedTime";
            Series_EB_TRetActual.YBindingPath = "TReturnActual";
            Series_EB_TRetActual.Interior = new SolidColorBrush(Colors.Purple);
            myEnergyChart.Series.Add(Series_EB_TRetActual);

            StepLineSeries Series_EB_ValveReturn = new StepLineSeries();
            Series_EB_ValveReturn.ItemsSource = chartModel.power;
            Series_EB_ValveReturn.Label = "ValveReturn";
            Series_EB_ValveReturn.XBindingPath = "ElapsedTime";
            Series_EB_ValveReturn.YBindingPath = "ValveReturn";
            Series_EB_ValveReturn.Interior = new SolidColorBrush(Colors.YellowGreen);
            myEnergyChart.Series.Add(Series_EB_ValveReturn);


            //SplineSeries Series_EB_THotTank = new SplineSeries();
            //Series_EB_THotTank.ItemsSource = chartModel.power;
            //Series_EB_THotTank.Label = "THotTank";
            //Series_EB_THotTank.XBindingPath = "ElapsedTime";
            //Series_EB_THotTank.YBindingPath = "THotTank";
            //Series_EB_THotTank.Interior = new SolidColorBrush(Colors.Red);
            //myEnergyChart.Series.Add(Series_EB_THotTank);

            //SplineSeries Series_EB_TColdTank = new SplineSeries();
            //Series_EB_TColdTank.ItemsSource = chartModel.power;
            //Series_EB_TColdTank.Label = "TColdTank";
            //Series_EB_TColdTank.XBindingPath = "ElapsedTime";
            //Series_EB_TColdTank.YBindingPath = "TColdTank";
            //Series_EB_TColdTank.Interior = new SolidColorBrush(Colors.Blue);
            //myEnergyChart.Series.Add(Series_EB_TColdTank);

            //add legends
            ChartLegend myEBLegend = new ChartLegend();
            myEnergyChart.Legend = myEBLegend;

            //margin
            myEnergyChart.Margin = new Thickness(10, 10, 10, 10);


            //add chart to grid
            this.TuneGrid.Children.Add(myEnergyChart);
            Grid.SetRow(myEnergyChart, 8);
            Grid.SetRowSpan(myEnergyChart, 2);
            Grid.SetColumn(myEnergyChart, 0);
            Grid.SetColumnSpan(myEnergyChart, 6);


            //Volume Balande Chart
            SfChart myVolumeChart = new SfChart();
            myVolumeChart.Header = "Volume (Buffer tank) (°C)";

            TimeSpanAxis volumeChartCategorAxis = new TimeSpanAxis();
            myVolumeChart.PrimaryAxis = volumeChartCategorAxis;

            NumericalAxis volumeChartNumericalAxis = new NumericalAxis();
            myVolumeChart.SecondaryAxis = volumeChartNumericalAxis;

            SplineSeries Series_VB_THotBuffer = new SplineSeries();
            Series_VB_THotBuffer.ItemsSource = chartModel.power;
            Series_VB_THotBuffer.Label = "TBuHot";
            Series_VB_THotBuffer.XBindingPath = "ElapsedTime";
            Series_VB_THotBuffer.YBindingPath = "TBufferHot";
            Series_VB_THotBuffer.Interior = new SolidColorBrush(Colors.Red);
            myVolumeChart.Series.Add(Series_VB_THotBuffer);

            SplineSeries Series_VB_TColdBuffer = new SplineSeries();
            Series_VB_TColdBuffer.ItemsSource = chartModel.power;
            Series_VB_TColdBuffer.Label = "TBuCold";
            Series_VB_TColdBuffer.XBindingPath = "ElapsedTime";
            Series_VB_TColdBuffer.YBindingPath = "TBufferCold";
            Series_VB_TColdBuffer.Interior = new SolidColorBrush(Colors.Blue);
            myVolumeChart.Series.Add(Series_VB_TColdBuffer);

            SplineSeries Series_VB_TMidBuffer = new SplineSeries();
            Series_VB_TMidBuffer.ItemsSource = chartModel.power;
            Series_VB_TMidBuffer.Label = "TBuMid";
            Series_VB_TMidBuffer.XBindingPath = "ElapsedTime";
            Series_VB_TMidBuffer.YBindingPath = "TBufferMid";
            myVolumeChart.Series.Add(Series_VB_TMidBuffer);

            StepLineSeries Series_VB_ValveReturn = new StepLineSeries();
            Series_VB_ValveReturn.ItemsSource = chartModel.power;
            Series_VB_ValveReturn.Label = "ValveReturn";
            Series_VB_ValveReturn.XBindingPath = "ElapsedTime";
            Series_VB_ValveReturn.YBindingPath = "ValveReturn";
            Series_VB_ValveReturn.Interior = new SolidColorBrush(Colors.YellowGreen);
            myVolumeChart.Series.Add(Series_VB_ValveReturn);


            //add legends
            ChartLegend myVBLegend = new ChartLegend();
            myVolumeChart.Legend = myVBLegend;

            //margin
            myVolumeChart.Margin = new Thickness(10, 10, 10, 10);


            this.TuneGrid.Children.Add(myVolumeChart);
            Grid.SetRow(myVolumeChart,8);
            Grid.SetRowSpan(myVolumeChart,2);
            Grid.SetColumn(myVolumeChart,6);
            Grid.SetColumnSpan(myVolumeChart, 6);



            AppHelper.AskAOUForDelayTimes();
            AppHelper.AskAOUForFeedTimes();

            //ask for all values
            if (GlobalVars.globTankSetTemps.HotTankSetTemp < 0)
            {
                AppHelper.AskAOUForHotTankTemp();
            }
            else
               if (GlobalVars.globTankSetTemps.ColdTankSetTemp < 0)
            {
                AppHelper.AskAOUForColdTankTemp();
            }
            else
               if
               (GlobalVars.globFeedTimes.HeatingActive < 0)
            {
                AppHelper.AskAOUForHeatingTime();
            }
            else
                if (GlobalVars.globFeedTimes.HeatingPause < 0)
            {
                AppHelper.AskAOUForHeatingPause();
            }
            
            //set and calculate delay time values
            TextBlock_HotCalibrate.Text = GlobalVars.globDelayTimes.HotCalibrateStr;
            HotFeedToReturnDelayCalTime.Text = GlobalVars.globDelayTimes.HotTuneStr;
            TextBlock_SumHotDelayTime.Text = GlobalVars.globDelayTimes.HotDelayTimeSumStr;
            TextBlock_ColdCalibrate.Text = GlobalVars.globDelayTimes.ColdCalibrateStr;
            ColdFeedToReturnDelayCalTime.Text = GlobalVars.globDelayTimes.ColdTuneStr;
            TextBlock_SumColdDelayTime.Text = GlobalVars.globDelayTimes.ColdDelayTimeSumStr;
            //F2MCalText.Text = GlobalVars.globDelayTimes.F2MCalibrate.ToString();
            //F2MTuneText.Text = GlobalVars.globDelayTimes.F2MTune.ToString();
            //sum = GlobalVars.globDelayTimes.F2MCalibrate + GlobalVars.globDelayTimes.F2MTune;
            //F2MTotalText.Text = sum.ToString();


            F2MCalText.Text = GlobalVars.globDelayTimes.F2MCalibrateComputedStr;
            F2MTuneText.Text = GlobalVars.globDelayTimes.F2MTuneComputedStr;
            F2MTotalText.Text = GlobalVars.globDelayTimes.F2MTotalComputedSumStr;

            F2MCalUsedText.Text = GlobalVars.globDelayTimes.F2MCalibrateUsedStr;
            F2MTuneUsedText.Text = GlobalVars.globDelayTimes.F2MTuneUsedStr;
            F2MTotalUsedText.Text = GlobalVars.globDelayTimes.F2MUsedSumStr;

            ////energy Active
            EACalText.Text = GlobalVars.globDelayTimes.EACalibrateStr;
            EATuneText.Text = GlobalVars.globDelayTimes.EATuneStr;
            EATotalText.Text = GlobalVars.globDelayTimes.EASumStr;

            //total fetch value with hotDelayTime, stored in hotCalibrate
            //delta/tune: offsHotFe2RetValveTim, store in EATune
            EASwitchTimeTotal.Text = GlobalVars.globDelayTimes.EATotalStr;
            EANewTune.Text = GlobalVars.globDelayTimes.EATuneStr;
            EACombo.Items.Add("Manual");
            EACombo.Items.Add("Auto");
            if (GlobalVars.globDelayTimes.EAAutoMode)
                EACombo.SelectedItem = "Auto";
            else
                EACombo.SelectedItem = "Manual";


            //Volume Active
            VACalText.Text = GlobalVars.globDelayTimes.VACalibrateStr;
            VATuneText.Text = GlobalVars.globDelayTimes.VATuneStr;
            VATotalText.Text = GlobalVars.globDelayTimes.VASumStr;
            if (GlobalVars.globFeedTimes.HeatingActive >= 0)
                if (GlobalVars.globDelayTimes.VATune >= 0)
                {
                    //Add together
                    VASwitchTimeTotal.Text = (GlobalVars.globFeedTimes.HeatingActive+ GlobalVars.globDelayTimes.VATune).ToString();
                }
            else
                    VASwitchTimeTotal.Text = GlobalVars.globFeedTimes.HeatingActive.ToString();
            else
            { VASwitchTimeTotal.Text = "-"; }

            VANewTune.Text = GlobalVars.globDelayTimes.VATuneStr;

            // VACombo.
            VACombo.Items.Add("Manual");
            VACombo.Items.Add("Auto");
            if (GlobalVars.globDelayTimes.VAAutoMode)
                VACombo.SelectedItem = "Auto";
            else
                VACombo.SelectedItem = "Manual";


            InitDispatcherTimer();
        }

        private void TunePage_Unloaded(object sender, RoutedEventArgs e)
        {
            dTimer.Stop();
            //clean data
            /*Series_EB_THotTank.ItemsSource = null;
            Series_EB_TColdTank.ItemsSource = null;
            Series_EB_TRetActual.ItemsSource = null;
            Series_Delay_TRetActual.ItemsSource = null;
            Series_Delay_TColdTank.ItemsSource = null;
            Series_Delay_THotTank.ItemsSource = null;
            Series_VB_THotBuffer.ItemsSource = null;
            Series_VB_TMidBuffer.ItemsSource = null;
            Series_VB_TColdBuffer.ItemsSource = null;
            */
        }

        private void TunePage_Loaded(object sender, RoutedEventArgs e)
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
                    Data.Updater.SetCommandValue(AOUDataTypes.CommandType.RunningMode, (int)AOUDataTypes.AOURunningMode.Idle);
                  //  HotStepButton.IsEnabled = true;
                  //  ColdStepButton.IsEnabled = true;
                }
            }
        }

        private void HotFeedToReturnDelayCalTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBlock_SumHotDelayTime.Text = GlobalVars.globDelayTimes.HotDelayTimeSumStr;
            //need to update calculated values too
           
            F2MCalText.Text = GlobalVars.globDelayTimes.F2MCalibrateComputedStr;

            F2MTuneText.Text = GlobalVars.globDelayTimes.F2MTuneComputedStr;

            F2MTotalText.Text = GlobalVars.globDelayTimes.F2MTotalComputedSumStr;
        }

        private void ColdFeedToReturnDelayCalTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            //calculate and show new sum
            TextBlock_SumColdDelayTime.Text = GlobalVars.globDelayTimes.ColdDelayTimeSumStr;
        }

        private void ColdFeedToReturnDelayCalTime_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_FocusControl, "Cold return delay time", AOUDataTypes.CommandType.coldDelayTime, 0, 30, 0.5, this);
        }

        private void HotFeedToReturnDelayCalTime_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_FocusControl, "Hot return delay time", AOUDataTypes.CommandType.hotDelayTime, 0, 30, 0.5,  this);
            //AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_FocusControl, "What shal we write here??", AOUDataTypes.CommandType.offsetHotFeed2RetValveTime, 0, 30, this);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TuneFreezeButton_Click(object sender, RoutedEventArgs e)
        {
            if (dTimer.IsEnabled && doStepTimer == 0)
            {
                dTimer.Stop();
                TuneFreezeButton.Content = "Run";
            }
            else
            {
                //make axis auto again
                //todo: make work for all graphs
                //this.myTuneChart.tuneChartCategoryAxis.Minimum = null;
                //tuneChartCategoryAxis.Maximum = null;
                //tuneChartCategoryAxis.Interval = null;
                //and start the plotting
                dTimer.Start();
                TuneFreezeButton.Content = "Freeze";
            }
        }

        private void F2MTuneUsedText_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_FocusControl, "Feed-To-Mould delay time", AOUDataTypes.CommandType.hotFeed2MouldDelayTime, 0, 30, 0.5, this);
        }

        private void F2MTuneUsedText_TextChanged(object sender, TextChangedEventArgs e)
        {
            //calculate and show new sum
            F2MTotalUsedText.Text = GlobalVars.globDelayTimes.F2MUsedSumStr;
        }

        private void EATuneText_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_FocusControl, "Offset Return valve Switch time", AOUDataTypes.CommandType.offsetHotFeed2RetValveTime, 0, 30, 0.5,  this);
        }

        private void EATuneText_TextChanged(object sender, TextChangedEventArgs e)
        {
            EATotalText.Text = GlobalVars.globDelayTimes.EASumStr;  
        }

        private void VATuneText_GotFocus(object sender, RoutedEventArgs e)
        {
            AppHelper.GetValueToTextBox((TextBox)sender, (Control)TextBox_FocusControl, "Offset Return valve Switching period", AOUDataTypes.CommandType.offsetRetValveHotPeriod, 0, 30, 0.5, this);
        }

        private void VATuneText_TextChanged(object sender, TextChangedEventArgs e)
        {
            VATotalText.Text = GlobalVars.globDelayTimes.VASumStr;
        }

        private void EANewTune_TextChanged(object sender, TextChangedEventArgs e)
        {
            //set new total text
            double newDelta;
            if (double.TryParse(EANewTune.Text, out newDelta) && (GlobalVars.globDelayTimes.HotCalibrate > -1000))
            {
                double newTotal = newDelta + GlobalVars.globDelayTimes.HotCalibrate;
                EASwitchTimeTotal.Text = newTotal.ToString("0.##");
            }
            else
                EASwitchTimeTotal.Text = "-";

                           
            
        }

        private void VANewTune_TextChanged(object sender, TextChangedEventArgs e)
        {
            double newDelta;
            if (double.TryParse(VANewTune.Text, out newDelta) && (GlobalVars.globFeedTimes.HeatingActive > -1000))
            {
                double newTotal = newDelta + GlobalVars.globFeedTimes.HeatingActive;
                VASwitchTimeTotal.Text = newTotal.ToString("0.##");
            }
            else
                VASwitchTimeTotal.Text = "-";
        }

        private void EACombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (EACombo.SelectedItem.ToString() == "Auto")
            {
                EANewTune.IsEnabled = false;
                GlobalVars.globDelayTimes.EAAutoMode = true;
            }
            else
            {
                EANewTune.IsEnabled = true;
                GlobalVars.globDelayTimes.EAAutoMode = false;
            }
        }

        private void VACombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VACombo.SelectedItem.ToString() == "Auto")
            {
                VANewTune.IsEnabled = false;
                GlobalVars.globDelayTimes.VAAutoMode = true;
            } else
            {
                VANewTune.IsEnabled = true;
                GlobalVars.globDelayTimes.VAAutoMode = false;
            }
           
        }
    }
}
