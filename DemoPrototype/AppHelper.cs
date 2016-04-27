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
            DataUpdater.VerifySendToAOUDlg(title, message + val, cmd, pg, val);
        }

        public static void SetLimitValueFromVerticalLine(string title, string message, AOUDataTypes.CommandType cmd, Syncfusion.UI.Xaml.Charts.VerticalLineAnnotation vLine, Page pg)
        {
            int val = SafeConvertToInt(vLine.X1); //only want whole integers on X axis label
            DataUpdater.VerifySendToAOUDlg(title, message + val, cmd, pg, val);
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
                        //Handle all feeding times
                        if (cmd == AOUDataTypes.CommandType.heatingTime)
                        {
                            GlobalVars.globFeedTimes.HeatingActive = val;
                        }
                        if (cmd == AOUDataTypes.CommandType.toolHeatingFeedPause)
                        {
                            GlobalVars.globFeedTimes.HeatingPause = val;
                        }
                        if (cmd == AOUDataTypes.CommandType.coolingTime)
                        {
                            GlobalVars.globFeedTimes.CoolingActive = val;
                        }
                        if (cmd == AOUDataTypes.CommandType.toolCoolingFeedPause)
                        {
                            GlobalVars.globFeedTimes.CoolingPause = val;
                        }

                        //Need to handle delay time to calculate correct val
                        if (cmd == AOUDataTypes.CommandType.hotDelayTime)
                        {
                            //save new value
                            if (pg.Name == "OperatorPage")
                                GlobalVars.globDelayTimes.HotTune = val;
                            else
                                GlobalVars.globDelayTimes.HotCalibrate = val;
                            val = GlobalVars.globDelayTimes.HotCalibrate + GlobalVars.globDelayTimes.HotTune;
                        }
                        if (cmd == AOUDataTypes.CommandType.coldDelayTime)
                        {
                            //save new value
                            if (pg.Name == "OperatorPage")
                                GlobalVars.globDelayTimes.ColdTune = val;
                            else
                                GlobalVars.globDelayTimes.ColdCalibrate = val;
                            val = GlobalVars.globDelayTimes.ColdCalibrate + GlobalVars.globDelayTimes.ColdTune;
                        }
                        //need to handle thresholds too
                        if (cmd == AOUDataTypes.CommandType.TReturnThresholdHot2Cold)
                        {
                            //save new value
                            GlobalVars.globThresholds.ThresholdHot2Cold = val;
                            //and now what? MW
                        }

                        DataUpdater.SetCommandValue(cmd, val);
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
