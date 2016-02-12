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

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoPrototype
{
    public sealed partial class SetValueDialog : ContentDialog
    {
        public bool Ok { get; private set; }

        public string GetStringValue()
        {
            return valueSlider.Value.ToString();
        }

        public void SetValue(int value, int min, int max)
        {
            valueSlider.Minimum = min;
            valueSlider.Maximum = max;
            valueSlider.Value = value;
        }

        public SetValueDialog(string strValue, int min, int max)
        {
            this.InitializeComponent();
            SetValue(int.Parse(strValue), min, max);
            textValue.Text = strValue;
            Ok = false;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Ok = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Ok = false;
        }

        private void valueSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            textValue.Text = valueSlider.Value.ToString();
        }
    }
}
