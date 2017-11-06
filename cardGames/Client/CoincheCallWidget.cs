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
    class CoincheCallWidget
    {
        private Grid coincheGrid = new Grid();
        public Grid CoincheGrid {
            get { return coincheGrid; }
        }

        private Button coincheConfirm = new Button();
        public Button CoincheConfirm {
            get { return coincheConfirm; }
        }

        public void setContentCoincheCall(string name) {
            coincheConfirm.Content = name;
        }

        public CoincheCallWidget()
        {
            coincheGrid.ColumnDefinitions.Add(new ColumnDefinition());
            coincheGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);

            coincheConfirm.Width = 140;
            coincheConfirm.Content = "Coinche";
            coincheConfirm.VerticalContentAlignment = VerticalAlignment.Center;

            Grid.SetColumn(coincheConfirm, 0);

            coincheGrid.Children.Add(coincheConfirm);
        }
    }
}
