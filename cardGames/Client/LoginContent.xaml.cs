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

namespace Client
{
    /// <summary>
    /// Interaction logic for LoginContent.xaml
    /// </summary>
    public partial class LoginContent : UserControl
    {
        public LoginContent()
        {
            InitializeComponent();
        }

        private void SendCredentialsToServer(string username, string ip, string port)
        {
            if (string.IsNullOrEmpty(username))
                MessageBox.Show("Username wasn't specified", "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                GameInfos.Instance.NetManager.Connect(username, ip, int.Parse(port));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendCredentialsToServer(UsernameField.Text, AddressField.Text, PortField.Text);
        }
    }
}
