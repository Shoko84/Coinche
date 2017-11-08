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
    /// Interaction logic for WaitingScreenContent.xaml
    /// </summary>
    public partial class WaitingScreenContent : UserControl
    {
        int count;

        public WaitingScreenContent()
        {
            InitializeComponent();
            count = 1;
        }

        public void SwitchToGameWindow()
        {
            GameWindow win = new GameWindow();
            App.Current.MainWindow.Close();
            App.Current.MainWindow = win;
            win.Initialize();
            win.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            count += 1;
            switch (count)
            {
                case 2:
                    Button b = this.FindName("Player2Button") as Button;
                    b.BorderBrush = new BrushConverter().ConvertFrom("#FF1AD411") as Brush;
                    b.Content = "Ready";
                    break;
                case 3:
                    Button c = this.FindName("Player3Button") as Button;
                    c.BorderBrush = new BrushConverter().ConvertFrom("#FF1AD411") as Brush;
                    c.Content = "Ready";
                    break;
                case 4:
                    Button d = this.FindName("Player4Button") as Button;
                    d.BorderBrush = new BrushConverter().ConvertFrom("#FF1AD411") as Brush;
                    d.Content = "Ready";
                    SwitchToGameWindow();
                    break;
            }
        }
    }
}
