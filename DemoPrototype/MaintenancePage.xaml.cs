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
        private bool inInit = true;

        public MaintenancePage()
        {
            this.logMsgModel = new LogMessageViewModel();

            this.Loaded += MaintenancePage_Loaded;
            this.Unloaded += MaintenancePage_Unloaded;
            inInit = true;

            this.InitializeComponent();

            // Connect Operetor page data context to LogMessageViewModel logMsgModel
            LogGrid.DataContext = logMsgModel;

            UpdateLogMessages(true);

            InitDispatcherTimer();

            if (GlobalAppSettings.RunningMode != (int)AOUDataTypes.AOURunningMode.Idle)
            {
                hotFeedValve.IsEnabled = false;
                coldFeedValve.IsEnabled = false;
                returnValve.IsEnabled = false;
               forceHelpText.Text = "Set running mode Idle to enable";
            }
            else
            {
                hotFeedValve.IsEnabled = true;
                coldFeedValve.IsEnabled = true;
                returnValve.IsEnabled = true;
                forceHelpText.Text = "";
            }
            //  AppHelper.AskAOUForTankTemps();

            hotFeedValve.IsOn = GetValveState(GlobalVars.globValveChartValues.HotValveValue, GlobalVars.globValveChartValues.HotValveHi);
            coldFeedValve.IsOn = GetValveState(GlobalVars.globValveChartValues.ColdValveValue, GlobalVars.globValveChartValues.ColdValveHi);
            returnValve.IsOn = GetValveState(GlobalVars.globValveChartValues.ReturnValveValue, GlobalVars.globValveChartValues.ReturnValveHi);
            coolantValve.IsOn = GetValveState(GlobalVars.globValveChartValues.CoolantValveValue, GlobalVars.globValveChartValues.CoolantValveHi);

            inInit = false;
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
            AOUDataTypes.UI_Buttons buttons = new AOUDataTypes.UI_Buttons();

            UpdateLogMessages(false);
            
            var t1 = GetStringValue(Data.Updater.LastPower.TReturnActual);
            var t2 = GetStringValue(Data.Updater.LastPower.TReturnForecasted);

            //elapsed time
            elapsedTime.Text = GetStringValue(Data.Updater.LastPower.ElapsedTime/1000);

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

            if (Data.Updater.UIButtonsChanged(out buttons))
            {
                //UpdateFromUIButtons(buttons);
                if (buttons.OnOffButton == AOUDataTypes.ButtonState.on)
                {
                    //string s = "On";
                }
                else
                {
                    //string s = "Off";
                }
                //pump hot
                if (buttons.ButtonPumpHot == AOUDataTypes.ButtonState.on)
                {
                    ButtonPumpHot.IsOn = true;
                }
                else
                {
                    ButtonPumpHot.IsOn = false;
                }
                if (buttons.ButtonPumpCold == AOUDataTypes.ButtonState.on)
                {
                    ButtonPumpCold.IsOn = true;
                }
                else
                {
                    ButtonPumpCold.IsOn = false;
                }
                if (buttons.ButtonHeater == AOUDataTypes.ButtonState.on)
                {
                    ButtonHeater.IsOn = true;
                }
                else
                {
                    ButtonHeater.IsOn = false;
                }
                if (buttons.ButtonCooler == AOUDataTypes.ButtonState.on)
                {
                    ButtonCooler.IsOn = true;
                }
                else
                {
                    ButtonCooler.IsOn = false;
                }
            }
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

        private void hotFeedValve_Toggled(object sender, RoutedEventArgs e)
        {
            //check if idle and not init, only show message if button enabled
            if (GlobalAppSettings.RunningMode != (int)AOUDataTypes.AOURunningMode.Idle && hotFeedValve.IsEnabled )
            {
                AppHelper.ShowMessageBox("AOU must be in mode IDLE for this command");
                return;
            }
            //Buttom must still be enabled to send command
            int val = 100; //mask int
            if (hotFeedValve.IsOn)
            {
                val = val + 1;
                GlobalVars.globValveChartValues.HotValveValue = GlobalVars.globValveChartValues.HotValveHi;
            }
            else
                GlobalVars.globValveChartValues.HotValveValue = GlobalVars.globValveChartValues.HotValveLow;
            if (hotFeedValve.IsEnabled && !inInit)
            {
                Data.Updater.SetCommandValue(AOUDataTypes.CommandType.forceValves, val); //hard converted to "0101"
                hotFeedValve.IsEnabled = false;
            }
        }

        private void hotFeedValve_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AppHelper.ShowMessageBox("Tapped");
        }

        private void hotFeedValve_GotFocus(object sender, RoutedEventArgs e)
        {
            //check if idle
            if (GlobalAppSettings.RunningMode != (int)AOUDataTypes.AOURunningMode.Idle)
            {
                AppHelper.ShowMessageBox("AOU must be in mode IDLE for this command",FocusButton);
                return;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AppHelper.AskAOUForValves();
        }

        private void coldFeedValve_Toggled(object sender, RoutedEventArgs e)
        {
            //check if idle
            if (GlobalAppSettings.RunningMode != (int)AOUDataTypes.AOURunningMode.Idle && coldFeedValve.IsEnabled)
            {
                AppHelper.ShowMessageBox("AOU must be in mode IDLE for this command");
                return;
            }
            int val = 200; //mask int
            if (coldFeedValve.IsOn)
            {
                val = val + 2;
                GlobalVars.globValveChartValues.ColdValveValue = GlobalVars.globValveChartValues.ColdValveHi;
            }
            else
                GlobalVars.globValveChartValues.ColdValveValue = GlobalVars.globValveChartValues.ColdValveLow;

            if (coldFeedValve.IsEnabled)
                if (coldFeedValve.IsEnabled && !inInit)
                {
                    Data.Updater.SetCommandValue(AOUDataTypes.CommandType.forceValves, val); //hard converted to "0101"
                    coldFeedValve.IsEnabled = false;

                }
        }

        private void returnValve_Toggled(object sender, RoutedEventArgs e)
        {
            //check if idle
            if (GlobalAppSettings.RunningMode != (int)AOUDataTypes.AOURunningMode.Idle && returnValve.IsEnabled)
            {
                AppHelper.ShowMessageBox("AOU must be in mode IDLE for this command");
                return;
            }
            int val = 400; //mask int
            if (returnValve.IsOn)
            {
                val = val + 4;
                GlobalVars.globValveChartValues.ReturnValveValue = GlobalVars.globValveChartValues.ReturnValveHi;
            }
            else
                GlobalVars.globValveChartValues.ReturnValveValue = GlobalVars.globValveChartValues.ReturnValveLow;

            if (returnValve.IsEnabled && !inInit)
            {
                Data.Updater.SetCommandValue(AOUDataTypes.CommandType.forceValves, val); //hard converted to "0101
                returnValve.IsEnabled = false;
            }
             
        }
    }

}
