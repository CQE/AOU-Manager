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
    public sealed partial class MaintenancePage : Page
    {
        private DispatcherTimer dTimer;
        private DataUpdater dataUpdater;

        public MaintenancePage()
        {
            this.InitializeComponent();
            dataUpdater = new DataUpdater();
            InitDispatcherTimer();
        }

        private void InitDispatcherTimer()
        {
            dTimer = new DispatcherTimer();
            dTimer.Tick += UpdateTick;
            dTimer.Interval = new TimeSpan(0, 0, 1);
            dTimer.Start();
        }

        void UpdateTick(object sender, object e)
        {
            if (LogGrid.DataContext != null) {
                dataUpdater.UpdateInputDataLogMessages(LogGrid.DataContext);
            }
        }

    }
}
