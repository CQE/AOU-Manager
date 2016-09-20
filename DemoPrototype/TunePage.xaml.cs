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
            myTuneChart.Header = "Feed and Return delay timings";
            
            //Adding horizontal and vertical axis to the chart
            TimeSpanAxis tuneChartCategoryAxis = new TimeSpanAxis();
            tuneChartCategoryAxis.Name = "OperatorDelayAxis";
            myTuneChart.PrimaryAxis = tuneChartCategoryAxis;
            NumericalAxis tuneChartNumericalAxis = new NumericalAxis();
            myTuneChart.SecondaryAxis = tuneChartNumericalAxis;
            
            //add series to chart
            SplineSeries Series_Delay_TRetActual = new SplineSeries();
            Series_Delay_TRetActual.ItemsSource = chartModel.power;
            Series_Delay_TRetActual.Label = "TRetActual";
            Series_Delay_TRetActual.XBindingPath = "ElapsedTime";
            Series_Delay_TRetActual.YBindingPath = "TReturnActual";
            Series_Delay_TRetActual.Interior = new SolidColorBrush(Colors.Brown);
            myTuneChart.Series.Add(Series_Delay_TRetActual);

            SplineSeries Series_Delay_THotTank = new SplineSeries();
            Series_Delay_THotTank.ItemsSource = chartModel.power;
            Series_Delay_THotTank.Label = "THotTank";
            Series_Delay_THotTank.XBindingPath = "ElapsedTime";
            Series_Delay_THotTank.YBindingPath = "THotTank";
            Series_Delay_THotTank.Interior = new SolidColorBrush(Colors.Red);
            myTuneChart.Series.Add(Series_Delay_THotTank);
            
            SplineSeries Series_Delay_TColdTank = new SplineSeries();
            Series_Delay_TColdTank.ItemsSource = chartModel.power;
            Series_Delay_TColdTank.Label = "TColdTank";
            Series_Delay_TColdTank.XBindingPath = "ElapsedTime";
            Series_Delay_TColdTank.YBindingPath = "TColdTank";
            Series_Delay_TColdTank.Interior = new SolidColorBrush(Colors.Blue);
            myTuneChart.Series.Add(Series_Delay_TColdTank);


            //add legends
            ChartLegend myDelayLegend = new ChartLegend();
            myTuneChart.Legend = myDelayLegend;

            //add chart to grid
            this.TuneGrid.Children.Add(myTuneChart);
            Grid.SetRow(myTuneChart, 2);
            Grid.SetRowSpan(myTuneChart, 2);
            Grid.SetColumn(myTuneChart, 0);
            Grid.SetColumnSpan(myTuneChart, 2);
           
           
            //Energy Balance Chart
            SfChart myEnergyChart = new SfChart();
            myEnergyChart.Header = "Buffer Tank Energy Balance";
            
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
            myEnergyChart.Series.Add(Series_EB_TRetActual);

            SplineSeries Series_EB_THotTank = new SplineSeries();
            Series_EB_THotTank.ItemsSource = chartModel.power;
            Series_EB_THotTank.Label = "THotTank";
            Series_EB_THotTank.XBindingPath = "ElapsedTime";
            Series_EB_THotTank.YBindingPath = "THotTank";
            Series_EB_THotTank.Interior = new SolidColorBrush(Colors.Red);
            myEnergyChart.Series.Add(Series_EB_THotTank);

            SplineSeries Series_EB_TColdTank = new SplineSeries();
            Series_EB_TColdTank.ItemsSource = chartModel.power;
            Series_EB_TColdTank.Label = "TColdTank";
            Series_EB_TColdTank.XBindingPath = "ElapsedTime";
            Series_EB_TColdTank.YBindingPath = "TColdTank";
            Series_EB_TColdTank.Interior = new SolidColorBrush(Colors.Blue);
            myEnergyChart.Series.Add(Series_EB_TColdTank);
            
            //add legends
            ChartLegend myEBLegend = new ChartLegend();
            myEnergyChart.Legend = myEBLegend;
            
            //add chart to grid
            this.TuneGrid.Children.Add(myEnergyChart);
            Grid.SetRow(myEnergyChart, 0);
            Grid.SetRowSpan(myEnergyChart, 2);
            Grid.SetColumn(myEnergyChart, 2);
            Grid.SetColumnSpan(myEnergyChart, 2);


            //Volume Balande Chart
            SfChart myVolumeChart = new SfChart();
            myVolumeChart.Header = "Buffer tank volume balance";

            TimeSpanAxis volumeChartCategorAxis = new TimeSpanAxis();
            myVolumeChart.PrimaryAxis = volumeChartCategorAxis;

            NumericalAxis volumeChartNumericalAxis = new NumericalAxis();
            myVolumeChart.SecondaryAxis = volumeChartNumericalAxis;

            SplineSeries Series_VB_THotBuffer = new SplineSeries();
            Series_VB_THotBuffer.ItemsSource = chartModel.power;
            Series_VB_THotBuffer.Label = "TBufHot";
            Series_VB_THotBuffer.XBindingPath = "ElapsedTime";
            Series_VB_THotBuffer.YBindingPath = "TBufferHot";
            Series_VB_THotBuffer.Interior = new SolidColorBrush(Colors.Red);
            myVolumeChart.Series.Add(Series_VB_THotBuffer);

            SplineSeries Series_VB_TColdBuffer = new SplineSeries();
            Series_VB_TColdBuffer.ItemsSource = chartModel.power;
            Series_VB_TColdBuffer.Label = "TBufCold";
            Series_VB_TColdBuffer.XBindingPath = "ElapsedTime";
            Series_VB_TColdBuffer.YBindingPath = "TBufferCold";
            Series_VB_TColdBuffer.Interior = new SolidColorBrush(Colors.Blue);
            myVolumeChart.Series.Add(Series_VB_TColdBuffer);

            SplineSeries Series_VB_TMidBuffer = new SplineSeries();
            Series_VB_TMidBuffer.ItemsSource = chartModel.power;
            Series_VB_TMidBuffer.Label = "TBufMid";
            Series_VB_TMidBuffer.XBindingPath = "ElapsedTime";
            Series_VB_TMidBuffer.YBindingPath = "TBufferMid";
            myVolumeChart.Series.Add(Series_VB_TMidBuffer);

            //add legends
            ChartLegend myVBLegend = new ChartLegend();
            myVolumeChart.Legend = myVBLegend;


            this.TuneGrid.Children.Add(myVolumeChart);
            Grid.SetRow(myVolumeChart,2);
            Grid.SetRowSpan(myVolumeChart,2);
            Grid.SetColumn(myVolumeChart,2);
            Grid.SetColumnSpan(myVolumeChart, 2);
            

            //set and calculate delay time values
            TextBlock_HotCalibrate.Text = GlobalVars.globDelayTimes.HotCalibrate.ToString();
            HotFeedToReturnDelayCalTime.Text = GlobalVars.globDelayTimes.HotTune.ToString();
            int sum = GlobalVars.globDelayTimes.HotCalibrate + GlobalVars.globDelayTimes.HotTune;
            TextBlock_SumHotDelayTime.Text = sum.ToString();
            TextBlock_ColdCalibrate.Text = GlobalVars.globDelayTimes.ColdCalibrate.ToString();
            ColdFeedToReturnDelayCalTime.Text = GlobalVars.globDelayTimes.ColdTune.ToString();
            sum = GlobalVars.globDelayTimes.ColdCalibrate + GlobalVars.globDelayTimes.ColdTune;
            TextBlock_SumColdDelayTime.Text = sum.ToString();


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
            //calculate and show new sum
            int sum = GlobalVars.globDelayTimes.HotCalibrate + GlobalVars.globDelayTimes.HotTune;
            TextBlock_SumHotDelayTime.Text = sum.ToString();
        }

        private void ColdFeedToReturnDelayCalTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            //calculate and show new sum
            int sum = GlobalVars.globDelayTimes.ColdCalibrate + GlobalVars.globDelayTimes.ColdTune;
            TextBlock_SumColdDelayTime.Text = sum.ToString();
        }

        private void ColdFeedToReturnDelayCalTime_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void HotFeedToReturnDelayCalTime_GotFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}