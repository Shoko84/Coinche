/**
 * @file    LoginContent.xaml.cs
 * @author  Maxime Cauvin
 * 
 * This file contains the LoginContent Class.
 */

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Interaction logic for LoginContent.xaml
    /// </summary>
    public partial class LoginContent : UserControl
    {
        /**
         *  LoginContent's constructor - Create an LoginContent instance
         */
        public LoginContent()
        {
            InitializeComponent();
        }

        /**
         *  Send a request to the server to be connected
         *  @param  username     The username filled in the UsernameField
         *  @param  ip           The IP filled in the AddressField
         *  @param  port         The port filled in the PortField
         */
        private void SendCredentialsToServer(string username, string ip, string port)
        {
            if (string.IsNullOrEmpty(username))
                MessageBox.Show("Username wasn't specified", "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if (string.IsNullOrEmpty(ip))
                MessageBox.Show("IP address wasn't specified", "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            else if (string.IsNullOrEmpty(port))
                MessageBox.Show("Port wasn't specified", "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                GameInfos.Instance.NetManager.Connect(username, ip, int.Parse(port));
        }

        /**
         *  ConnectButton Click Event trigger
         *  @param  sender     The component that triggered the event
         *  @param  e          Object that contains the state information and data about the event triggered.
         */
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            SendCredentialsToServer(UsernameField.Text, AddressField.Text, PortField.Text);
        }
    }
}
