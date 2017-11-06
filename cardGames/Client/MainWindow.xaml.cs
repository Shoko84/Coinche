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
        private ContractWidget contractWidget = new ContractWidget();

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

            if (contractWidget.ContractGrid != null)
                Console.Write("rip");
            BottomActions.Child = contractWidget.ContractGrid;
        }
    }
}
