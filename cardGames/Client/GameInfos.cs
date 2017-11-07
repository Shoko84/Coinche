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
        private int myId;

        public ClientUser.ClientPosition GetPosFromId(int myId, int id)
        {
            int tmp = myId;
            int it = 0;
            while (tmp != id)
            {
                if (tmp == 4)
                    tmp = -1;
                it += 1;
                tmp += 1;
            }
            return ((ClientUser.ClientPosition)it);
        }

        public void AddCardsPlayed(Card card)
        {
            cardsPlayed.Add(card);
            if (App.Current.MainWindow is MainWindow)
            {
                ((MainWindow)App.Current.MainWindow).DrawGameField();
                ((MainWindow)App.Current.MainWindow).DrawHandCards(usersList);
                ((MainWindow)App.Current.MainWindow).DrawCardsPlayed(cardsPlayed);
            }
        }

        public void ClearCardsPlayed()
        {
            cardsPlayed.Clear();
            if (App.Current.MainWindow is MainWindow)
            {
                ((MainWindow)App.Current.MainWindow).DrawGameField();
                ((MainWindow)App.Current.MainWindow).DrawHandCards(usersList);
                ((MainWindow)App.Current.MainWindow).DrawCardsPlayed(cardsPlayed);
            }
        }

        public void AddCardToPlayerId(int id, Card card)
        {
            for (int i = 0; i < usersList.Count; i++)
            {
                if (usersList[i].Id == id)
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

        public void RemoveCardFromPlayerId(int id, Card card)
        {
            // TODO

            //if (App.Current.MainWindow is MainWindow)
            //{
            //    ((MainWindow)App.Current.MainWindow).DrawGameField();
            //    ((MainWindow)App.Current.MainWindow).DrawHandCards(usersList);
            //    ((MainWindow)App.Current.MainWindow).DrawCardsPlayed(cardsPlayed);
            //}
        }

        public void AddPlayer(int id, string playerName, bool isMyself)
        {
            if (isMyself && myId == -1)
            {
                usersList.Add(new ClientUser(id, playerName, GetPosFromId(id, id)));
                myId = id;
            }
            else if (!isMyself && myId != -1)
                usersList.Add(new ClientUser(id, playerName, GetPosFromId(myId, id)));
        }

        public void RemovePlayer(string playerName)
        {
            // TODO

            //if (App.Current.MainWindow is MainWindow)
            //{
            //    ((MainWindow)App.Current.MainWindow).DrawGameField();
            //    ((MainWindow)App.Current.MainWindow).DrawHandCards(usersList);
            //    ((MainWindow)App.Current.MainWindow).DrawCardsPlayed(cardsPlayed);
            //}
        }

        public GameInfos()
        {
            usersList = new List<ClientUser>();
            cardsPlayed = new List<Card>();
            myId = -1;
        }
    }
}
