using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Game;

namespace Client
{
    public class GameInfos
    {
        private static GameInfos instance;
        private static readonly object padlock = new object();
        public static GameInfos Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new GameInfos();
                    return (instance);
                }
            }
        }

        private NetworkManager netManager;
        public NetworkManager NetManager { get => netManager; }
        private EventManager eventManager;
        public EventManager EventManager { get => eventManager; }

        private List<ClientUser> usersList;
        public List<ClientUser> UsersList { get => usersList; }

        private List<Game.Card> cardsPlayed;
        public List<Game.Card> CardsPlayed { get => cardsPlayed; }

        private int myId;
        public int MyId { get => myId; set => myId = value; }

        public ClientUser GetClientUserById(int id)
        {
            for (int i = 0; i < usersList.Count; i++)
            {
                if (usersList[i].Id == id)
                    return (usersList[i]);
            }
            return (null);
        }

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
            if (App.Current.MainWindow is GameWindow)
            {
                ((GameWindow)App.Current.MainWindow).DrawGameField();
                ((GameWindow)App.Current.MainWindow).DrawHandCards(usersList);
                ((GameWindow)App.Current.MainWindow).DrawCardsPlayed(cardsPlayed);
            }
        }

        public void ClearCardsPlayed()
        {
            cardsPlayed.Clear();
            if (App.Current.MainWindow is GameWindow)
            {
                ((GameWindow)App.Current.MainWindow).DrawGameField();
                ((GameWindow)App.Current.MainWindow).DrawHandCards(usersList);
                ((GameWindow)App.Current.MainWindow).DrawCardsPlayed(cardsPlayed);
            }
        }

        public void AddCardToPlayerId(int id, Card card)
        {
            for (int i = 0; i < usersList.Count; i++)
            {
                if (usersList[i].Id == id)
                {
                    usersList[i].AddCard(card);

                    if (App.Current.MainWindow is GameWindow)
                    {
                        ((GameWindow)App.Current.MainWindow).DrawGameField();
                        ((GameWindow)App.Current.MainWindow).DrawHandCards(usersList);
                        ((GameWindow)App.Current.MainWindow).DrawCardsPlayed(cardsPlayed);
                    }
                    break;
                }
            }
        }

        public void RemoveCardFromPlayerId(int id, Card card)
        {
            // TODO

            //if (App.Current.MainWindow is GameWindow)
            //{
            //    ((GameWindow)App.Current.MainWindow).DrawGameField();
            //    ((GameWindow)App.Current.MainWindow).DrawHandCards(usersList);
            //    ((GameWindow)App.Current.MainWindow).DrawCardsPlayed(cardsPlayed);
            //}
        }

        public bool AddPlayer(int id, string playerName, bool isMyself)
        {
            if (isMyself && myId == -1)
            {
                usersList.Add(new ClientUser(id, playerName, GetPosFromId(id, id)));
                myId = id;
            }
            else if (!isMyself && myId != -1 && !usersList.Exists(e => e.Id == id))
                usersList.Add(new ClientUser(id, playerName, GetPosFromId(myId, id)));
            else
                return (false);
            return (true);
        }

        public void RemovePlayer(string playerName)
        {
            // TODO

            //if (App.Current.MainWindow is GameWindow)
            //{
            //    ((GameWindow)App.Current.MainWindow).DrawGameField();
            //    ((GameWindow)App.Current.MainWindow).DrawHandCards(usersList);
            //    ((GameWindow)App.Current.MainWindow).DrawCardsPlayed(cardsPlayed);
            //}
        }

        public GameInfos()
        {
            netManager = new NetworkManager();
            eventManager = new EventManager();
            usersList = new List<ClientUser>();
            cardsPlayed = new List<Card>();
            myId = -1;
        }
    }
}
