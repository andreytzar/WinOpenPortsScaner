
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;


namespace WinOpenPortsScaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<PortInfo> ports;
        public MainWindow()
        {
            InitializeComponent();
            dataGrid.ItemsSource = ports = PortScaner.Scan();
        }

        private void BTNScan_Click(object sender, RoutedEventArgs e)
            => dataGrid.ItemsSource = ports = PortScaner.Scan();

        private async void BTNProcessInfo_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                foreach (var port in ports)
                {
                    port.InitProcess();
                    Dispatcher.Invoke(() => dataGrid.Items.Refresh());
                }
            });
            dataGrid.Items.Refresh();
        }

        private async void BTNRemoteInfo_Click(object sender, RoutedEventArgs e)
        {
            foreach (var port in ports)
            {
                Task.Run(() =>
                {
                    port.InitRemoteInfo();
                    Dispatcher.Invoke(() => dataGrid.Items.Refresh());
                });
            }
        }
    }
}
