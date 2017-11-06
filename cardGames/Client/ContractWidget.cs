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
    class ContractWidget
    {
        private Grid contractGrid = new Grid();
        public Grid ContractGrid {
            get { return contractGrid; }
        }

        private ComboBox contractChoice = new ComboBox();
        public ComboBox ContractChoice  {
            get { return contractChoice; }
        }

        private Button contractConfirm = new Button();
        public Button ContractConfirm {
            get { return contractConfirm; }
        }

        public ContractWidget()
        {
            contractGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contractGrid.ColumnDefinitions.Add(new ColumnDefinition());
            contractGrid.ColumnDefinitions[0].Width = new GridLength(3, GridUnitType.Star);
            contractGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

            contractChoice.Width = 300;
            contractChoice.Height = 22;
            contractChoice.Items.Add("Sans atout");
            contractChoice.Items.Add("Pique");
            contractChoice.Items.Add("Trefle");
            contractChoice.Items.Add("Carreau");
            contractChoice.Items.Add("Coeur");
            contractChoice.Items.Add("Tout atout");
            contractChoice.SelectedIndex = 0;

            contractConfirm.Width = 100;
            contractConfirm.Height = 30;
            contractConfirm.Content = "Choose";

            Grid.SetColumn(contractChoice, 0);
            Grid.SetColumn(contractConfirm, 1);

            contractGrid.Children.Add(contractChoice);
            contractGrid.Children.Add(contractConfirm);
        }
    }
}
