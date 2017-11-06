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
    class IngameCallWidget
    {
        private Grid ingameCallGrid = new Grid();
        public Grid IngameCallGrid
        {
            get { return ingameCallGrid; }
        }

        private ComboBox cardChoice = new ComboBox();
        public ComboBox CardChoice
        {
            get { return cardChoice; }
        }

        private Button cardChoiceConfirm = new Button();
        public Button CardChoiceConfirm
        {
            get { return cardChoiceConfirm; }
        }

        public void addInCardChoiceList(params string[] list)
        {
            for (int i = 0; i < list.Length; i++)
                cardChoice.Items.Add(list[i]);
        }

        public void clearCardChoiceList()
        {
            cardChoice.Items.Clear();
            cardChoice.Items.Add("-- Please select a card --");
            cardChoice.SelectedIndex = 0;
        }

        public IngameCallWidget()
        {
            ingameCallGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ingameCallGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ingameCallGrid.ColumnDefinitions[0].Width = new GridLength(3, GridUnitType.Star);
            ingameCallGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

            cardChoiceConfirm.Width = 100;
            cardChoiceConfirm.Content = "Pick";

            cardChoice.Width = 300;
            cardChoice.Height = 22;
            cardChoice.Items.Add("-- Please select a card --");
            cardChoice.SelectedIndex = 0;

            Grid.SetColumn(cardChoice, 0);
            Grid.SetColumn(cardChoiceConfirm, 1);

            ingameCallGrid.Children.Add(cardChoice);
            ingameCallGrid.Children.Add(cardChoiceConfirm);
        }
    }
}
