using Newtonsoft.Json;
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
    /// Interaction logic for IngameCallContent.xaml
    /// </summary>
    public partial class IngameCallContent : UserControl
    {
        public IngameCallContent()
        {
            InitializeComponent();
        }

        private void PickCardButton_Click(object sender, RoutedEventArgs e)
        {
            ClientUser user = GameInfos.Instance.GetClientUserById(GameInfos.Instance.MyId);
            if (CardsListBox.SelectedIndex == -1)
                MessageBox.Show("Impossibulu");
            if (CardsListBox.SelectedIndex != -1)
                GameInfos.Instance.NetManager.WriteMessage("121", JsonConvert.SerializeObject(user.CardsList.cards[CardsListBox.SelectedIndex]));
        }
    }
}
