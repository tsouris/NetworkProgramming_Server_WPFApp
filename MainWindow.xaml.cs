using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NetworkProgramming_Server_WPFApp
{
    public partial class MainWindow : Window
    {
        private UdpClient udpServer;
        private IPEndPoint remoteEndPoint;
        private TextBox logTextBox;
        public MainWindow()
        {
            InitializeComponent();
            InitializeServer();
        }

        private void InitializeServer()
        {
            udpServer = new UdpClient(8888);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 8888);

            Console.WriteLine("Server is running...");

            logTextBox = new TextBox();
            logTextBox.TextWrapping = TextWrapping.Wrap;
            logTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            logTextBox.IsReadOnly = true;
            Grid.SetRow(logTextBox, 3);
            Grid.SetColumnSpan(logTextBox, 2);
            mainGrid.Children.Add(logTextBox);
        }

        private static string GetComponentPrice(string componentName)
        {
            Random random = new Random();
            int price = random.Next(100, 1000);

            return $"${price}";
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            LogMessage("Server started.");
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            LogMessage("Server stopped.");
        }

        private void LogMessage(string message)
        {
            logTextBox.AppendText($"{message}\n");
            logTextBox.ScrollToEnd();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (udpServer != null)
            {
                udpServer.Close();
                LogMessage("Server stopped.");
            }

            base.OnClosing(e);
        }

        private void ProcessClientRequests()
        {
            while (true)
            {
                try
                {
                    byte[] requestData = udpServer.Receive(ref remoteEndPoint);
                    string componentName = Encoding.UTF8.GetString(requestData);

                    string price = GetComponentPrice(componentName);

                    byte[] responseData = Encoding.UTF8.GetBytes(price);
                    udpServer.Send(responseData, responseData.Length, remoteEndPoint);

                    LogMessage($"Request received for {componentName}. Sent response: {price}");
                }
                catch (Exception ex)
                {
                    LogMessage(ex.Message);
                }
            }
        }
    }
}
