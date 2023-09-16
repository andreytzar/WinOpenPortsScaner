using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
