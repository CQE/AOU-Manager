using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPrototype
{
    public static class AppHelper
    {

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

        public static async void ShowMessageBox(string text)
        {
            var dialog = new Windows.UI.Popups.MessageDialog(text);
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Ok") { Id = 0 });
            dialog.DefaultCommandIndex = 0;
            await dialog.ShowAsync();
        }
    }
}
