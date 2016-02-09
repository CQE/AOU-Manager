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
using DataHandler;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            if (GlobalAppSettings.IsCelsius == true)
            {
                TempUnitCelsius.IsChecked = true;
            }
            else
            {
                TempUnitFahrenheit.IsChecked = true;
            }

            AOUDataSourceTypesCombo.Items.Add("Serial input");
            AOUDataSourceTypesCombo.Items.Add("File input");
            AOUDataSourceTypesCombo.Items.Add("Random input");
            if (GlobalAppSettings.DataRunType == AOURouter.RunType.Serial)
            {
                AOUDataSourceTypesCombo.SelectedIndex = 0;
                this.AOUDataSourceStringText.Text = "Select Port:";
            }
            else if (GlobalAppSettings.DataRunType == AOURouter.RunType.File)
            {
                AOUDataSourceTypesCombo.SelectedIndex = 1;
                this.AOUDataSourceStringText.Text = "Select File path relative to user Pictures folder:";
            }
            else
            {
                this.AOUDataSourceStringText.Text = "Select max number of values:";
                AOUDataSourceTypesCombo.SelectedIndex = 2;
            }

            this.AOUDataSourceString.Text = GlobalAppSettings.DataRunSource;

        }

        /* New to be accepted */
        private void TempUnitCelsius_Checked(object sender, RoutedEventArgs e)
        {
            { GlobalAppSettings.IsCelsius = true; }
        }

        private void TempUnitFahrenheit_Checked(object sender, RoutedEventArgs e)
        {
            { GlobalAppSettings.IsCelsius = false; }
        }

        private void AOUDataSourceTypeChanged(object sender, RoutedEventArgs e)
        {
            if (AOUDataSourceTypesCombo.SelectedIndex >= 0)
            {
                if (AOUDataSourceTypesCombo.SelectedIndex == 1)
                {
                    GlobalAppSettings.DataRunType = AOURouter.RunType.File;
                    this.AOUDataSourceStringText.Text = "Select File path relative to user Pictures folder:";
                }
                else if (AOUDataSourceTypesCombo.SelectedIndex == 0)
                {
                    GlobalAppSettings.DataRunType = AOURouter.RunType.Serial;
                    this.AOUDataSourceStringText.Text = "Select Port:";
                }
                else
                {
                    GlobalAppSettings.DataRunType = AOURouter.RunType.Random;
                    this.AOUDataSourceStringText.Text = "Select max number of values:";
                }
            }
            DataUpdater.Restart();
        }

        private void AOUDataSourceString_LostFocus(object sender, RoutedEventArgs e)
        {
            GlobalAppSettings.DataRunSource = AOUDataSourceString.Text;
            DataUpdater.Restart();
        }
    }
}
