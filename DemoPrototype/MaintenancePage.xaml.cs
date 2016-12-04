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
using Syncfusion.UI.Xaml.Grid;
//using Syncfusion.UI.Xaml.Grid.Converter; new handling in v14
using Windows.Storage;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using Syncfusion.UI.Xaml.Grid.Converter;

namespace DemoPrototype
{
    public sealed partial class MaintenancePage : Page
    {
        private DispatcherTimer dTimer;
        private LogMessageViewModel logMsgModel;

        public MaintenancePage()
        {
            this.logMsgModel = new LogMessageViewModel();

            this.Loaded += MaintenancePage_Loaded;
            this.Unloaded += MaintenancePage_Unloaded;

            this.InitializeComponent();

            // Connect Operetor page data context to LogMessageViewModel logMsgModel
            LogGrid.DataContext = logMsgModel;

            UpdateLogMessages(true);

            InitDispatcherTimer();
          //  AppHelper.AskAOUForTankTemps();

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
            dTimer.Interval = new TimeSpan(0, 0, 1); // Every second
        }


        private void UpdateLogMessages(bool all)
        {
            try
            {
                var logs = Data.Updater.GetLogMessages(all);
                if (logs.Count > 0)
                {
                    logMsgModel.AddLogMessages(logs);
                    int cnt = logMsgModel.logMessages.Count - 1;
                    LogGrid.ScrollInView((new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex() { RowIndex = cnt, ColumnIndex = 0 }));
                }
            }
            catch (Exception e)
            {
               // tempHeaderText.Text = e.Message;
            }
        }

        private string GetStringValue(double val)
        {
            if (!double.IsNaN(val))
            {
                return val.ToString();
            }
            else
            {
                return "-";
            }
        }

        private bool GetValveState(double value, int onValue)
        {
            return (int)value >= onValue;
        }

        void UpdateTick(object sender, object e)
        {
            UpdateLogMessages(false);

            var t1 = GetStringValue(Data.Updater.LastPower.TReturnActual);
            var t2 = GetStringValue(Data.Updater.LastPower.TReturnForecasted);

            hotTankTemp.Text = GetStringValue(Data.Updater.LastPower.THotTank);
            coldTankTemp.Text = GetStringValue(Data.Updater.LastPower.TColdTank);
            //returnValveTemp.Text = GetStringValue(Data.Updater.LastPower.TReturnValve);

            oilHotSideTemp.Text = GetStringValue(Data.Updater.LastPower.TBufferHot);
            oilMiddleTemp.Text = GetStringValue(Data.Updater.LastPower.TBufferMid);
            oilColdSideTemp.Text = GetStringValue(Data.Updater.LastPower.TBufferCold);

            oilInletTemp.Text = GetStringValue(double.NaN);
            oilOutletTemp.Text = GetStringValue(Data.Updater.LastPower.THeaterOilOut);

            coolantWater.Text= GetStringValue(double.NaN);
            heatTransferOil.Text = GetStringValue(double.NaN);

            retValveTemp.Text= GetStringValue(Data.Updater.LastPower.TReturnValve); 
            bearingHotPumpTemp.Text = GetStringValue(Data.Updater.LastPower.TBearHot);

            oilExchangeInletTemp.Text = GetStringValue(double.NaN); GetStringValue(double.NaN);

            oilExchangeOutletTemp.Text = GetStringValue(Data.Updater.LastPower.TCoolingCartridgeOut);  //<Cool>

            coolantWaterExchangeInletTemp.Text = GetStringValue(double.NaN);
            coolantWaterExchangeOutletTemp.Text = GetStringValue(Data.Updater.LastPower.TCoolant); 

            hotPumpOutput.Text = GetStringValue(double.NaN);
            coldPumpOutput.Text = GetStringValue(double.NaN);
            toolReturn.Text = GetStringValue(double.NaN);
            particleFilter.Text = GetStringValue(double.NaN);
            pneumatics.Text = GetStringValue(double.NaN);

            vL1.Text = GetStringValue(double.NaN);
            vL2.Text = GetStringValue(double.NaN);
            vL3.Text = GetStringValue(double.NaN);
            aL1.Text = GetStringValue(double.NaN);
            aL2.Text = GetStringValue(double.NaN);
            aL3.Text = GetStringValue(double.NaN);

            hotFeedValve.IsOn = GetValveState(Data.Updater.LastPower.ValveFeedHot, GlobalVars.globValveChartValues.HotValveHi);
            coldFeedValve.IsOn = GetValveState(Data.Updater.LastPower.ValveFeedCold, GlobalVars.globValveChartValues.ColdValveHi);
            returnValve.IsOn = GetValveState(Data.Updater.LastPower.ValveReturn, GlobalVars.globValveChartValues.ReturnValveHi);
            coolantValve.IsOn = GetValveState(Data.Updater.LastPower.ValveCoolant, GlobalVars.globValveChartValues.CoolantValveHi);

            safetyStop.IsOn = GetValveState(Data.Updater.LastPower.SafetyStop, GlobalVars.globSafetyAlarms.SafetyStopHi);
            safetyReset.IsOn = GetValveState(Data.Updater.LastPower.SafetyReset, GlobalVars.globSafetyAlarms.SafetyResetHi);
            safetyEmergency.IsOn = GetValveState(Data.Updater.LastPower.SafetyEmergency, GlobalVars.globSafetyAlarms.SafetyEmergencyHi);
            safetyFluidLevel.IsOn = GetValveState(Data.Updater.LastPower.SafetyFluidLevel, GlobalVars.globSafetyAlarms.SafetyFluidLevelHi);
            safetyOverHeated.IsOn = GetValveState(Data.Updater.LastPower.SafetyOverHeated, GlobalVars.globSafetyAlarms.SafetyOverHeatedHi);
        }

        private async void SaveExcelToFile(Syncfusion.XlsIO.IWorkbook workBook)
        {
            string today = DateTime.Now.ToString("yyMMdd");
            string fileName = "LogMessages" + today + ".xlsx";
            StorageFile storageFile = await KnownFolders.PicturesLibrary.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
            try
            {
                if (storageFile != null)
                    await workBook.SaveAsAsync(storageFile);
            }
            catch (Exception e)
            {
               Data.Updater.CreateLogMessage("Save Excel To File " + fileName, e.Message);
            }
            AppHelper.ShowMessageBox("Exporting file " + storageFile.Path + " succeeded"  );
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions();
            options.ExcelVersion = Syncfusion.XlsIO.ExcelVersion.Excel2013;
            var excelEngine = LogGrid.ExportToExcel(LogGrid.View, options);
            SaveExcelToFile(excelEngine.Excel.Workbooks[0]);
        }
    }

}
