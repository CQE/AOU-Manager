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

        public static async void GetValueToTextBox(TextBox textbox, Control nextControl, string title, AOUDataTypes.CommandType cmd, int min, int max, Page pg, bool sendToAOU = true)
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
                    dialog = new SetValueDialog(value, min, max);
                    dialog.Title = title;
                    dialog.PrimaryButtonText = "Ok";
                    dialog.SecondaryButtonText = "Cancel";
                    // dialog.MaxWidth = ActualWidth // Required for Mobile!

                    await dialog.ShowAsync();
                    if (((SetValueDialog)dialog).Ok)
                    {
                        textbox.Text = ((SetValueDialog)dialog).GetStringValue();
                        int val = ((SetValueDialog)dialog).GetIntValue();

                        /* ToDo: Set when ret */  //need to test if this is working we can set the variable two times
                        //Handle all feeding times
                        //remeber to send times i deciseconds i e multiply by 10
                        if (cmd == AOUDataTypes.CommandType.heatingTime)
                        {
                            GlobalVars.globFeedTimes.HeatingActive = val;
                            val = val * 10;
                        }
                        if (cmd == AOUDataTypes.CommandType.toolHeatingFeedPause)
                        {
                            GlobalVars.globFeedTimes.HeatingPause = val;
                            val = val * 10;
                        }
                        if (cmd == AOUDataTypes.CommandType.coolingTime)
                        {
                           // val = val * 10;
                            GlobalVars.globFeedTimes.CoolingActive = val;
                            val = val * 10;
                        }
                        if (cmd == AOUDataTypes.CommandType.toolCoolingFeedPause)
                        {
                            //val = val * 10;
                            GlobalVars.globFeedTimes.CoolingPause = val;
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
                            val = GlobalVars.globDelayTimes.HotCalibrate + GlobalVars.globDelayTimes.HotTune;
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
                            val = GlobalVars.globDelayTimes.ColdCalibrate + GlobalVars.globDelayTimes.ColdTune;
                            val = val * 10;
                        }
                        //need to handle thresholds too
                        if (cmd == AOUDataTypes.CommandType.TReturnThresholdHot2Cold)
                        {
                            //save new value
                            GlobalVars.globThresholds.ThresholdHot2Cold = val;
                            //and now what? MW
                        }
                        if (cmd == AOUDataTypes.CommandType.TReturnThresholdCold2Hot)
                        {
                            //save new value
                            GlobalVars.globThresholds.ThresholdCold2Hot = val;
                            //and now what? MW
                        }
                        //*/
                        Data.Updater.SetCommandValue(cmd, val);
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

        public static void AskAOUForMouldingTimes()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.heatingTime);
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.coolingTime);
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.toolHeatingFeedPause);
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.toolCoolingFeedPause);
        }

        public static void AskAOUForThresholds()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TBufferColdUpperLimit);
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TBufferHotLowerLimit);
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TBufferMidRefThreshold);
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TReturnThresholdCold2Hot);
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.TReturnThresholdHot2Cold);
        }

        public static void AskAOUForDelayTimes()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.hotDelayTime);
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.coldDelayTime);
        }

        public static void AskAOUForTankTemps()
        {
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.tempHotTankFeedSet);
            Data.Updater.AskCommandValue(AOUDataTypes.CommandType.tempColdTankFeedSet);
        }

        public static async void ShowMessageBox(string text)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(text);
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Ok") { Id = 0 });
            dialog.DefaultCommandIndex = 0;
            await dialog.ShowAsync();
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
