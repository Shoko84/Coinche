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
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow instance;
        private static readonly object padlock = new object(); 

        public static MainWindow Instance { get => instance; }
        public static object Padlock { get => padlock; }

        public MainWindow()
        {
            instance = this;
            InitializeComponent();
            ContentArea.Content = new Client.LoginContent();
        }

        public object ContentAreaContent
        {
            get
            {
                return this.ContentArea.Content;
            }
            set
            {
                lock (padlock)
                {
                    this.ContentArea.Content = value;
                }
            }
        }
    }
}
