using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class GameInfos
    {
        private List<ClientUser> usersList;
        private List<Card> cardsPlayed;

        public void AddCardToPlayer(string playerName, Card card)
        {
            for (int i = 0; i < usersList.Count; i++)
            {
                if (usersList[i].Username == playerName)
                {
                    usersList[i].AddCard(card);
                    if (App.Current.MainWindow is MainWindow)
                    {
                        ((MainWindow)App.Current.MainWindow).DrawGameField();
                        ((MainWindow)App.Current.MainWindow).DrawHandCards(usersList);
                        ((MainWindow)App.Current.MainWindow).DrawCardsPlayed(cardsPlayed);
                    }
                    break;
                }
            }
        }

        public void AddPlayer(string playerName)
        {
            usersList.Add(new ClientUser(playerName));
        }

        public GameInfos()
        {
            usersList = new List<ClientUser>();
            cardsPlayed = new List<Card>();
        }
    }
}
