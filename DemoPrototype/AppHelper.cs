using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Syncfusion.UI.Xaml;

namespace DemoPrototype
{
    public static class AppHelper
    {

        static ContentDialog dialog = null;

        public static int SafeConvertToInt(object value)
        {
            double dblVal = SafeConvertToDouble(value);
            if (dblVal == Double.NaN)
            {
                return -1; // What value is best to represent a non valid integer
            }
            else
            { 
                return (int)Math.Round(dblVal);
            }
        }

        public static double SafeConvertToDouble(object value)
        {
            if (value is String)
            {
                double res = Double.NaN;
                Double.TryParse((string)value, out res);
                return res;
            }
            else
            {
                try
                {
                    return (double)value;
                }
                catch (Exception)
                {
                    return Double.NaN;
                }
            }
        }

        public static TimeSpan SafeConvertToTimeSpan(object value)
        {

            TimeSpan res = new TimeSpan(0);  // What value is best to represent a non valid TimeSpan
            if (value is String)
            {
               TimeSpan.TryParse((string)value, out res);
                return res;
            }
            else
            {
                try
                {
                    return (TimeSpan)value;
                }
                catch (Exception)
                {
                    return res;
                }
            }
        }

        public static int ConvertToValidInteger(string strValue, int minValidValue, int maxValidValue)
        {
            int value = -1;
            int.TryParse(strValue, out value);
            if (value >= minValidValue && value <= maxValidValue)
            {
                return value;
            }
            else
            {
                return -1;
            }
        }

        public static void SetLimitValueFromHorizontalLine(string title, string message, AOUDataTypes.CommandType cmd, Syncfusion.UI.Xaml.Charts.HorizontalLineAnnotation hLine, Page pg)
        {
            int val = SafeConvertToInt(hLine.Y1); //only want whole integers on Y axis label
            Data.Updater.VerifySendToAOUDlg(title, message + val, cmd, pg, val);
        }

        public static void SetLimitValueFromVerticalLine(string title, string message, AOUDataTypes.CommandType cmd, Syncfusion.UI.Xaml.Charts.VerticalLineAnnotation vLine, Page pg)
        {
            int val = SafeConvertToInt(vLine.X1); //only want whole integers on X axis label
            Data.Updater.VerifySendToAOUDlg(title, message + val, cmd, pg, val);
        }

        public static async void GetValueToTextBox(TextBox textbox, Control nextControl, string title, AOUDataTypes.CommandType cmd, int min, int max, double stepFrequency, Page pg,  bool sendToAOU = true)
        {
            /* Example to use GetValueToTextBox
            
            <TextBox x:Name="coldTankSet" IsReadOnly="True" BorderThickness="0" Text="20"></TextBox>
            <TextBox x:Name="coldTankValue" IsReadOnly="True" Margin="15,0,0,0" Text="25" GotFocus="coldTankValue_GotFocus"></TextBox>

            private void coldTankValue_GotFocus(object sender, RoutedEventArgs e)
            {
                AppHelper.GetValueToTextBox((TextBox)sender, (Control)coldTankSet, AOUTypes.CommandType. "Change Cold Tank Value", 0, 300);
                // coldTankSet is control to go to after that the value have been changed. This prevent repeating dialog boxes
            }

            */

            string value = textbox.Text;
            try {
                if (dialog == null) // Prevent more then one dialg
                { 
                    dialog = new SetValueDialog(value, min, max, stepFrequency);
                    dialog.Title = title;
                    dialog.PrimaryButtonText = "Ok";
                    dialog.SecondaryButtonText = "Cancel";
                    // dialog.MaxWidth = ActualWidth // Required for Mobile!

                    await dialog.ShowAsync();
                    if (((SetValueDialog)dialog).Ok)
                    {
                        textbox.Text = ((SetValueDialog)dialog).GetStringValue();
                        //val now a double
                        double val = ((SetValueDialog)dialog).GetDoubleValue();

                        /* ToDo: Set when ret */  //need to test if this is working we can set the variable two times
                        //Handle all feeding times
                        //remeber to send times i deciseconds i e multiply by 10
                        if (cmd == AOUDataTypes.CommandType.heatingTime)
                        {
                            GlobalVars.globFeedTimes.HeatingActive = (int)val;
                            val = val * 10;
                        }
                        if (cmd == AOUDataTypes.CommandType.toolHeatingFeedPause)
                        {
                            GlobalVars.globFeedTimes.HeatingPause = (int)val;
                            val = val * 10;
                        }
                        if (cmd == AOUDataTypes.CommandType.coolingTime)
                        {
                           // val = val * 10;
                            GlobalVars.globFeedTimes.CoolingActive = (int)val;
                            val = val * 10;
                        }
                        if (cmd == AOUDataTypes.CommandType.toolCoolingFeedPause)
                        {
                            //val = val * 10;
                            GlobalVars.globFeedTimes.CoolingPause = (int)val;
                            val = val * 10;
                        }

                        //Need to handle delay time to calculate correct val
                        if (cmd == AOUDataTypes.CommandType.hotDelayTime)
                        {
                            //save new value
                            //val = val * 10;
                            if (pg.Name == "TunePage")
                                GlobalVars.globDelayTimes.HotTune = val;
                            else
                                GlobalVars.globDelayTimes.HotCalibrate = val;
                            val = Math.Max(GlobalVars.globDelayTimes.HotCalibrate,0) + Math.Max(GlobalVars.globDelayTimes.HotTune,0);
                            val = val * 10;
                        }
                        if (cmd == AOUDataTypes.CommandType.coldDelayTime)
                        {
                            //save new value
                            //val = val * 10;
                            if (pg.Name == "TunePage")
                                GlobalVars.globDelayTimes.ColdTune = val;
                            else
                                GlobalVars.globDelayTimes.ColdCalibrate = val;
                            val = Math.Max(GlobalVars.globDelayTimes.ColdCalibrate, 0) + Math.Max(GlobalVars.globDelayTimes.ColdTune, 0);
                            val = val * 10;
                        }
                        //need to handle thresholds too
                        if (cmd == AOUDataTypes.CommandType.TReturnThresholdHot2Cold)
                        {
                            //save new value
                            GlobalVars.globThresholds.ThresholdHot2Cold = (int)val;
                            //and now what? MW
                        }
                        if (cmd == AOUDataTypes.CommandType.TReturnThresholdCold2Hot)
                        {
                            //save new value
                            GlobalVars.globThresholds.ThresholdCold2Hot = (int)val;
                            //and now what? MW
                        }
                        //and the four new commands
                        if (cmd == AOUDataTypes.CommandType.hotFeed2MouldDelayTime)
                        {
                            //save new value
                            if (pg.Name == "TunePage")
                                GlobalVars.globDelayTimes.F2MTuneUsed = val;
                            else
                                GlobalVars.globDelayTimes.F2MCalibrateUsed= val;
                            //Calculate new val to send
                            //val = (GlobalVars.globDelayTimes.F2MTuneUsed + GlobalVars.globDelayTimes.F2MCalibrateUsed) * 10;
                            val = Math.Max(GlobalVars.globDelayTimes.F2MTuneUsed, 0) + Math.Max(GlobalVars.globDelayTimes.F2MCalibrateUsed, 0);
                            val = val * 10;
                        }
                        if (cmd == AOUDataTypes.CommandType.coldFeed2MouldDelayTime)
                        {
                            //save new value
                            GlobalVars.globDelayTimes.F2MCalibrateUsed = val;
                            //and now what? MW
                            //val = (GlobalVars.globDelayTimes.F2MTuneUsed + GlobalVars.globDelayTimes.F2MCalibrateUsed) * 10;
                            val = Math.Max(GlobalVars.globDelayTimes.F2MTuneUsed, 0) + Math.Max(GlobalVars.globDelayTimes.F2MCalibrateUsed, 0);
                            val = val * 10;
                        }
                        if (cmd == AOUDataTypes.CommandType.offsetHotFeed2RetValveTime)
                        {
                            if (pg.Name == "TunePage")
                                GlobalVars.globDelayTimes.EATune = val;
                            else
                                GlobalVars.globDelayTimes.EACalibrate = val;
                            //save new value
                            //GlobalVars.globDelayTimes.EATune = val;
                            val = Math.Max(GlobalVars.globDelayTimes.EACalibrate, 0) + Math.Max(GlobalVars.globDelayTimes.EATune, 0);
                            val = val * 10;
                        }
                        if (cmd == AOUDataTypes.CommandType.offsetRetValveHotPeriod)
                        {
                            //save new value
                            if (pg.Name == "TunePage")
                                GlobalVars.globDelayTimes.VATune = val;
                            else
                                GlobalVars.globDelayTimes.VACalibrate = val;
                            val = Math.Max(GlobalVars.globDelayTimes.VACalibrate, 0) + Math.Max(GlobalVars.globDelayTimes.VATune, 0);
                            val = val * 10;
                        }
                        //hot step and cold step
                        if (cmd == AOUDataTypes.CommandType.runModeHeating)
                        {
                            //save new value
                            GlobalVars.globDelayTimes.HotStep = val;
                        }
                        if (cmd == AOUDataTypes.CommandType.runModeCooling)
                        {
                            //save new value
                            GlobalVars.globDelayTimes.ColdStep = val;
                        }
                        
                        //*/
                        if (sendToAOU==true) 
                            Data.Updater.SetCommandValue(cmd, (int)val);
                    }
                    if (nextControl != null)
                    {
                        nextControl.Focus(FocusState.Pointer);
                    }
                    dialog = null;
                }
            }
            catch (Exception e)
            {
                ShowMessageBox(e.Message);
            }
        }


        public static void AskAOUForTemps()
        {
            string msg = "Asked for: ";
            if (GlobalVars.globTankSetTemps.HotTankSetTemp < 0)
            {
                Data.Updater.AskCommandValue(AOUDataTypes.CommandType.tempHotTankFeedSet);
                msg += "setHotTankTemp";
            }

            if (GlobalVars.globTankSetTemps.ColdTankSetTemp < 0)
            {
                Data.Updater.AskCommandValue(AOUDataTypes.CommandType.tempColdTankFeedSet);
                msg += " setColdTankTemp";
            }
            ShowMessageBox(msg);

        }

        public static void AskAOUForMouldTimes()
        {
            string msg = "Asked for: ";
            if (GlobalVars.globFeedTimes.HeatingActive < 0)
            {
                Data.Updater.AskCommandValue(AOUDataTypes.CommandType.heatingTime);
                msg += "heatingTime ";
            }

            if (GlobalVars.globFeedTimes.HeatingPause < 0)
            {
                Data.Updater.AskCommandValue(AOUDataTypes.CommandType.toolHeatingFeedPause);
                msg += "toolHeatingFeedPause ";
            }

            if (GlobalVars.globFeedTimes.CoolingActive < 0)
            {
                Data.Updater.AskCommandValue(AOUDataTypes.CommandType.coolingTime);
                msg += "coolingTime ";
            }
            if (GlobalVars.globFeedTimes.CoolingPause < 0)
            {
                Data.Updater.AskCommandValue(AOUDataTypes.CommandType.toolCoolingFeedPause);
                msg += "toolCoolingFeedPause ";
            }
            ShowMessageBox(msg);
        }

        public static void AskAOUForDelayTimes()
        {
            if (GlobalVars.globDelayTimes.HotCalibrate < 0 && GlobalVars.globDelayTimes.HotTune <0)
            {
                Data.Updater.AskCommandValue(AOUDataTypes.CommandType.hotDelayTime);
            }
            if (GlobalVars.globDelayTimes.ColdCalibrate < 0 && GlobalVars.globDelayTimes.ColdTune <0)
            {
                Data.Updater.AskCommandValue(AOUDataTypes.CommandType.coldDelayTime);
            }

        }


        public static void AskAOUForFeedTimes()
        {
            if (GlobalVars.globDelayTimes.EACalibrate < 0 && GlobalVars.globDelayTimes.EATune < 0)
                Data.Updater.AskCommandValue(AOUDataTypes.CommandType.offsetRetValveHotPeriod);
            if (GlobalVars.globDelayTimes.VACalibrate < 0 && GlobalVars.globDelayTimes.VATune < 0)
                Data.Updater.AskCommandValue(AOUDataTypes.CommandType.offsetHotFeed2RetValveTime);
            if (GlobalVars.globDelayTimes.F2MCalibrate < 0 && GlobalVars.globDelayTimes.F2MTune < 0)
                Data.Updater.AskCommandValue(AOUDataTypes.CommandType.hotFeed2MouldDelayTime);
        }
        
        public static void AskAOUForHeatingTime()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.heatingTime);
        }
        public static void AskAOUForCoolingTime()
        {
           Data.Updater.AskCommandValue(AOUDataTypes.CommandType.coolingTime);
        }
        public static void AskAOUForHeatingPause()
        {
           Data.Updater.AskCommandValue(AOUDataTypes.CommandType.toolHeatingFeedPause);
        }
        public static void AskAOUForCoolingPause()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.toolCoolingFeedPause);
        }

        public static void AskAOUForHot2ColdThreshold()
        {
            //Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TBufferColdUpperLimit);
            //Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TBufferHotLowerLimit);
            //Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TBufferMidRefThreshold);
            //Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TReturnThresholdCold2Hot);
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TReturnThresholdHot2Cold);
        }

        public static void AskAOUForCold2HotThreshold()
        {
            //Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TBufferColdUpperLimit);
            //Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TBufferHotLowerLimit);
            //Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TBufferMidRefThreshold);
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TReturnThresholdCold2Hot);
            //Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TReturnThresholdHot2Cold);
        }


        public static void AskAOUForMidBufThreshold()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TBufferMidRefThreshold);
            //Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TReturnThresholdCold2Hot);
            //Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TReturnThresholdHot2Cold);
        }




        public static void AskAOUForHotDelayTime()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.hotDelayTime);
          //  Data.Updater.AskCommandValue(AOUDataTypes.CommandType.coldDelayTime);
        }

        public static void AskAOUForColdDelayTime()
        {
            //Data.Updater.AskCommandValue(AOUDataTypes.CommandType.hotDelayTime);
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.coldDelayTime);
        }


        public static void AskAOUForOffsetHotFeed2RetValveTime()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.offsetHotFeed2RetValveTime);
        }
        public static void AskAOUForOffsetRetValveHotPeriod()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.offsetRetValveHotPeriod);
        }
        public static void AskAOUForHotFeed2MouldDelayTime()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.hotFeed2MouldDelayTime);
        }
        public static void AskAOUForColdFeed2MouldDelayTime()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.coldFeed2MouldDelayTime);
        }
        public static void AskAOUForHotTankTemp()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.tempHotTankFeedSet);
            //remove this line later
            //GlobalVars.globTankSetTemps.HotTankSetTemp = 333;
        }

        public static void AskAOUForColdTankTemp()
        {
            
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.tempColdTankFeedSet); 
        }

        public static void AskAOUForValves()
        {

            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.forceValves);
        }

        public static void SetValves2(AOUDataTypes.CommandType cmd, int val)
        {
          
          Data.Updater.SetValveValue(cmd, (int)val);
          

            //Data.Updater.AskCommandValue(AOUDataTypes.CommandType.tempColdTankFeedSet);
        }


        public static async void ShowMessageBox(string text, Control nextControl = null)
        {
            try
            {
                var dialog = new Windows.UI.Popups.MessageDialog(text);
                dialog.Commands.Add(new Windows.UI.Popups.UICommand("Ok") { Id = 0 });
                dialog.DefaultCommandIndex = 0;
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                Data.Updater.CreateLogMessage(text);
            }
            if (nextControl != null)
            {
                nextControl.Focus(FocusState.Pointer);
            }
        }



        /*
        var savePicker = new FileSavePicker();
        savePicker.DefaultFileExtension = ".png";
        savePicker.FileTypeChoices.Add(".png", new List<string> { ".png" });
        savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        savePicker.SuggestedFileName = "snapshot.png";

        var file = await savePicker.PickSaveFileAsync();
        if (file == null)
            return;
        */

    }
}
