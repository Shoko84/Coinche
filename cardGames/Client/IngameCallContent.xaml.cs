/**
 * @file    IngameCallContent.xaml.cs
 * @author  Maxime Cauvin
 * 
 * This file contains the IngameCallContent Class.
 */

using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for IngameCallContent.xaml
    /// </summary>
    public partial class IngameCallContent : UserControl
    {
        /**
         *  IngameCallContent's constructor - Create an IngameCallContent instance
         */
        public IngameCallContent()
        {
            InitializeComponent();
        }

        /**
         *  PickCardButton Click Event trigger
         *  @param  sender     The component that triggered the event
         *  @param  e          Object that contains the state information and data about the event triggered.
         */
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
