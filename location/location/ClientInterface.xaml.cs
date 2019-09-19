using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

namespace location
{
    /// <summary>
    /// Interaction logic for ClientInterface.xaml
    /// </summary>
    public partial class ClientInterface : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool DebugMode
        {
            get
            {
                return Client.DebugMode_B;
            }
            set
            {
                Client.DebugMode_B = value;
                NotifyPropertyChanged();
            }
        }
        public int Timeout
        {
            get { return Client.Timeout_B; }
            set
            {
                Client.Timeout_B = value;
                NotifyPropertyChanged();
            }
        }
        public string Host
        {
            get
            {
                return Client.HostName_B;
            }
            set
            {
                Client.HostName_B= value;
                NotifyPropertyChanged();
            }
        }
        public string UserName
        {
            get
            {
                return Client.Name_B;
            }
            set
            {
                Client.Name_B = value;
                NotifyPropertyChanged();
            }
        }
        public string Location
        {
            get
            {
                return Client.Location_B;
            }
            set
            {
                Client.Location_B = value;
                NotifyPropertyChanged();
            }
        }
        public int Port
        {
            get
            {
                return Client.Port_B;
            }
            set
            {
                Client.Port_B = value;
                NotifyPropertyChanged();
            }
        }
        public string ServerResponse
        {
            get
            {
                return Client.ServerResponse_B;
            }
        }
        public ComboBoxItem Protocol
        {
            get
            {
                return new ComboBoxItem() { Content = Client.Protocol_B };
            }
            set
            {
                Client.Protocol_B = value.Content.ToString();
                NotifyPropertyChanged();
            }
        }
        public RequestType TypeOfRequest
        {
            set
            {
                Client.RequestType_B = value;
                NotifyPropertyChanged();
            }
        }
        public ClientInterface()
        {
            InitializeComponent();
        }
        private void SendRequestButton_Click(object sender, RoutedEventArgs e)
        {
            new Task(Client.ExecuteClient).Start();
        }
        public void NotifyPropertyChanged([CallerMemberName] string pProperty = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pProperty));
        }
        private void LookUpRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            TypeOfRequest = (RequestType) Enum.Parse(typeof(RequestType),((RadioButton)sender).Tag.ToString());
        }

        private void DebugCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
