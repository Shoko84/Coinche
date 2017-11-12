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

        public Deck CardsPlayed;
        public int MyId;
        public GAME_STATUS GameStatus;
        public Contract contractPicked;

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
            if (isMyself && MyId == -1)
            {
                usersList.Add(new ClientUser(id, playerName, GetPosFromId(id, id)));
                MyId = id;
            }
            else if (!isMyself && MyId != -1 && !usersList.Exists(e => e.Id == id))
                usersList.Add(new ClientUser(id, playerName, GetPosFromId(MyId, id)));
            else
                return (false);
            return (true);
        }

        public GameInfos()
        {
            netManager = new NetworkManager();
            eventManager = new EventManager();
            usersList = new List<ClientUser>();
            CardsPlayed = new Deck();
            MyId = -1;
            GameStatus = GAME_STATUS.WAIT;
            contractPicked = null;
        }
    }
}
