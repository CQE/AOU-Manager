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
using Syncfusion.UI.Xaml.Charts;

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
            myTuneChart.Series.Add(Series_Delay_TRetActual);




            //add chart to grid
            this.TuneGrid.Children.Add(myTuneChart);
            Grid.SetRow(myTuneChart, 1);
            Grid.SetColumn(myTuneChart, 1);

            //Energy Balande Chart
            SfChart myEnergyChart = new SfChart();
            myEnergyChart.Header = "Energy Balance";
            
            //Adding horizontal and vertical axis to the chart
            TimeSpanAxis energyChartCategoryAxis = new TimeSpanAxis();
            energyChartCategoryAxis.Name = "OperatorDelayAxis";
            myEnergyChart.PrimaryAxis = energyChartCategoryAxis;
            NumericalAxis energyChartNumericalAxis = new NumericalAxis() { Minimum = 0, Maximum = 200 };
            myEnergyChart.SecondaryAxis = energyChartNumericalAxis;
         

            //add series to chart
            SplineSeries Series_EB_TRetActual = new SplineSeries();
            Series_Delay_TRetActual.ItemsSource = chartModel.power;
            Series_Delay_TRetActual.Label = "TRetActual";
            Series_Delay_TRetActual.XBindingPath = "ElapsedTime";
            Series_Delay_TRetActual.YBindingPath = "TReturnActual";
            myEnergyChart.Series.Add(Series_EB_TRetActual);

            SplineSeries Series_EB_THotTank = new SplineSeries();
            Series_Delay_TRetActual.ItemsSource = chartModel.power;
            Series_Delay_TRetActual.Label = "THotTank";
            Series_Delay_TRetActual.XBindingPath = "ElapsedTime";
            Series_Delay_TRetActual.YBindingPath = "THotTank";
            myEnergyChart.Series.Add(Series_EB_THotTank);
            
            //add chart to grid
            this.TuneGrid.Children.Add(myEnergyChart);
            Grid.SetRow(myEnergyChart, 2);
            Grid.SetColumn(myEnergyChart, 1);
            
            InitDispatcherTimer();
        }

        private void TunePage_Unloaded(object sender, RoutedEventArgs e)
        {
            dTimer.Stop();
            //clean data
            
            //Series_EB_TRetActual.ItemsSource = null;
            
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



    }
}
