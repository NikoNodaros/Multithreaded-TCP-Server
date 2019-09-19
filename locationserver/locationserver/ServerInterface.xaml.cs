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
using System.Runtime.InteropServices;
using System.Threading;

[assembly: ComVisible(false)] namespace locationserver
{
    /// <summary>
    /// Interaction logic for ServerInterface.xaml
    /// </summary>
    [ComVisible(false)] public partial class ServerInterface : Window
    {
        Thread ServerThread = null;
        bool ServerRunning = false;
        public ServerInterface(bool pDebug, string pLogFilePath, string pSaveFilePath, int pTimeOut)
        {
            InitializeComponent();
            ToggleServerButton.Content = "Start Server";
            ToggleServerButton.Background = Brushes.Green;
            DebugCheckBox.IsChecked = pDebug;
            LogFilePathTextBox.Text = pLogFilePath;
            SaveFilePathTextBox.Text = pSaveFilePath;
            TimeOutTextBox.Text = pTimeOut.ToString();
        }
        private void ToggleServer(object sender, RoutedEventArgs e)
        {
            if (!ServerRunning)
            {
                if (LogFilePathTextBox.Text != "")
                {
                    if (Uri.IsWellFormedUriString(LogFilePathTextBox.Text, UriKind.RelativeOrAbsolute))
                    {
                        Server.LogFilePath = LogFilePathTextBox.Text;
                        Server.Logging = true;
                        MessageBox.Show("Path loaded");
                    }
                    else
                    {
                        
                        MessageBox.Show("Invalid path");
                    }
                }
                if (SaveFilePathTextBox.Text != "")
                {
                    if (Uri.IsWellFormedUriString(SaveFilePathTextBox.Text, UriKind.RelativeOrAbsolute))
                    {
                        Server.NameLocationFilePath = SaveFilePathTextBox.Text;
                        Server.Logging = true;
                        MessageBox.Show("Path loaded");
                    }
                    else
                    {
                        MessageBox.Show("Invalid path");
                    }
                }

                int timeOut;
                int.TryParse(TimeOutTextBox.Text, out timeOut);
                Server.Factory((bool)DebugCheckBox.IsChecked, LogFilePathTextBox.Text, SaveFilePathTextBox.Text, timeOut);
                ServerThread = new Thread(Server.RunServer); ServerThread.IsBackground = true; ServerThread.Start();
                ToggleServerButton.Content = "Stop Server";
                ToggleServerButton.Background = Brushes.Red;
                ServerRunning = true;
            }
            else
            {
                ToggleServerButton.Content = "Start Server";
                ToggleServerButton.Background = Brushes.Green;
                Server.StopServer();
                ServerThread.Abort();
                ServerRunning = false;
            }
        }
    }
}
