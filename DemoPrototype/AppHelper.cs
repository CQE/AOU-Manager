using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Syncfusion.UI.Xaml;
using DataHandler;

namespace DemoPrototype
{
    public static class AppHelper
    {

        static ContentDialog dialog = null;

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

            TimeSpan res;
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
                    //what should I return here?
                    return res;
                }
            }
        }

        public static double SafeConvertToXCoordinate(object value)
        {

            double res;
            if (value is String)
            {
                double.TryParse((string)value, out res);
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
                    //what should I return here?
                    return 0;
                }
            }
        }

        public static int ConvertToInteger(string strValue, int minValidValue, int maxValidValue)
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


        public static void SetLimitValueFromHorizontalLine(string title, string message, AOUTypes.CommandType cmd, Syncfusion.UI.Xaml.Charts.HorizontalLineAnnotation hLine, Page pg)
        {
            double dblVal = SafeConvertToXCoordinate(hLine.Y1);
            int val = (int)Math.Round(dblVal);

            //only want whole integers on axis label
            hLine.Y1 = val;

            DataUpdater.VerifySendToAOUDlg(title, message + val, cmd, DataUpdater.VerifyDialogType.VeryfyOkCancelOnly, pg, val);

        }

        public static void SetLimitValueFromVerticalLine(string title, string message, AOUTypes.CommandType cmd, Syncfusion.UI.Xaml.Charts.VerticalLineAnnotation vLine, Page pg)
        {
            double dblVal = SafeConvertToXCoordinate(vLine.X1);
            int val = (int)Math.Round(dblVal);

            //only want whole integers on axis label
            vLine.X1 = val;

            DataUpdater.VerifySendToAOUDlg(title, message + val, cmd, DataUpdater.VerifyDialogType.VeryfyOkCancelOnly, pg, val);

            //Urban please replace this code with code showing diff between the lines, and center the Chartstripline
            // double firstSlope = AppHelper.SafeConvertToDouble(PhaseVLine2.X1);
            //double secondSlope = AppHelper.SafeConvertToDouble(PhaseVLine1.X1);
            // PhaseDiffResult.Text = "5"; //(secondSlope - firstSlope).ToString();
        }

        public static async void GetValueToTextBox(TextBox textbox, Control nextControl, string title, AOUTypes.CommandType cmd, int min, int max, bool sendToAOU = true)
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
    }
}
