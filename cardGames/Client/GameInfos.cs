using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Game;
using Server;

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

        private Deck cardsPlayed;
        public Deck CardsPlayed { get => cardsPlayed; set => cardsPlayed = value; }

        private int myId = -1;
        public int MyId { get => myId; set => myId = value; }

        private GAME_STATUS gameStatus;
        public GAME_STATUS GameStatus { get => gameStatus; set => gameStatus = value; }

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
            return (ClientUser.ClientPosition)(((id - myId) + 4) % 4);
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

        public GameInfos()
        {
            netManager = new NetworkManager();
            eventManager = new EventManager();
            usersList = new List<ClientUser>();
            cardsPlayed = new Deck();
            myId = -1;
            gameStatus = GAME_STATUS.WAIT;
        }
    }
}
