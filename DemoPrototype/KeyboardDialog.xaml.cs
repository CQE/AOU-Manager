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
    public sealed partial class KeyboardDialog : ContentDialog
    {
        public bool Ok { get; private set; }

        private TextBox txtbox;

        public string GetStringValue()
        {
            return ""; // textValue.Text;
        }

        public KeyboardDialog(char[,] chars, bool isPassword = false)
        {
            this.InitializeComponent();
            int rowLength = chars.GetLength(0);
            int colLength = chars.GetLength(1);

            GridLength gl = new GridLength(30); // GridLength in Pixel

            try
            {
                // Add definitons for all rows
                for (int row = 0; row < rowLength + 1; row++)
                {
                    RowDefinition rowDef = new RowDefinition();
                    rowDef.Height = gl;
                    dlgKeysGrid.RowDefinitions.Add(rowDef);
                }

                // Add definitons for all columns with same size as rows
                var colDef = new ColumnDefinition();
                colDef.Width = gl;
                for (int col = 0; col < colLength; col++) {
                    dlgKeysGrid.ColumnDefinitions.Add(colDef);
                }
            }
            catch (Exception e)
            {
                string s = e.Message;
            }


            // DependencyProperty dp;
            int x = 0;
            int y = 0;
            Thickness margThickness = new Thickness(2);
            dlgKeysGrid.MinWidth = 500;
            int index = 0;
            for (int row = 0; row < rowLength; row++)
            {
                for (int col = 0; col < colLength; col++)
                {
                    Button tb =new Button();
                    Grid.SetColumn(tb, col);
                    Grid.SetRow(tb, row);

                    tb.Name = "key_" + chars[row,col];
                    tb.Content = chars[row, col].ToString();
                    tb.Width = 20;
                    tb.Height = 20;
                    tb.Margin = margThickness;
                    tb.Click += Tb_Click;
                    dlgKeysGrid.Children.Add(tb);
                    index++;
                }
            }
            txtbox = new TextBox();
            Grid.SetColumn(txtbox, 0);
            Grid.SetRow(txtbox, rowLength);

            txtbox.Name = "text";
            txtbox.Margin = margThickness;
            dlgKeysGrid.Children.Add(txtbox);

            dlgKeysGrid.UpdateLayout();
            Ok = false;
        }

        private void Tb_Click(object sender, RoutedEventArgs e)
        {
            txtbox.Text += ((Button)e.OriginalSource).Content;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Ok = true;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Ok = false;
        }

    }
}
