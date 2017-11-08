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

        private string SendCredentialsToServer(string username, string ip, string port)
        {
            string status = "OK";

            if (string.IsNullOrEmpty(username))
                status = "Username not specified";
            else
            {
                // TODO: Call a function to send to the server the credentials

                int myId = 2; // Returned by the server
                // if connection allowed
                if (true)
                    GameInfos.Instance.AddPlayer(myId, username, true);
                else
                    status = "Could not connect to the server";
            }
            return (status);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win;

            if (App.Current.MainWindow is MainWindow)
            {
                string status;
                if ((status = SendCredentialsToServer(UsernameField.Text, AddressField.Text, PortField.Text)) == "OK")
                {
                    win = App.Current.MainWindow as MainWindow;
                    win.ContentArea.Content = new WaitingScreenContent();
                }
                else
                    MessageBox.Show(status, "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
