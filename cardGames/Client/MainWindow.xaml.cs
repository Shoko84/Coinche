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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Grid g = new Grid();
        private ComboBox c = new ComboBox();
        private Button b = new Button();

        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Shapes.Rectangle rect;
            rect = new System.Windows.Shapes.Rectangle
            {
                Fill = new SolidColorBrush(Colors.ForestGreen),
                Width = this.TestCanvas.Width - this.TestCanvas.Margin.Right - this.TestCanvas.Margin.Left,
                Height = 300
            };
            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, this.TestCanvas.Height / 2 - rect.Height / 2);
            this.TestCanvas.Children.Add(rect);

            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.ColumnDefinitions[0].Width = new GridLength(3, GridUnitType.Star);
            g.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

            c.Width = 300;
            c.Height = 22;
            c.Items.Add("Sans atout");
            c.Items.Add("Pique");
            c.Items.Add("Trefle");
            c.Items.Add("Carreau");
            c.Items.Add("Coeur");
            c.Items.Add("Tout atout");
            c.SelectedIndex = 0;

            b.Width = 100;
            b.Height = 30;
            b.Content = "Choose";

            Grid.SetColumn(c, 0);
            Grid.SetColumn(b, 1);

            g.Children.Add(c);
            g.Children.Add(b);

            BottomActions.Child = g;

        }
    }
}
