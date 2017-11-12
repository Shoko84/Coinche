/**
 * @file    GameInfos.cs
 * @author  Maxime Cauvin
 * 
 * This file contains the GameInfos Class.
 */

using System.Collections.Generic;
using Game;
using Server;

namespace Client
{
    public class GameInfos
    {
        private static GameInfos instance; /**< This variable corresponds to the current GameInfos instance*/
        private static readonly object padlock = new object(); /**< This object is used for thread security*/
        /**
         *  Getter of the GameInfos instance
         */
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

        private NetworkManager netManager; /**< This variable corresponds to the current NetworkManager instance*/
        /**
         *  Getter of the NetworkManager instance
         */
        public NetworkManager NetManager { get => netManager; }

        private EventManager eventManager; /**< This variable corresponds to the current EventManager instance*/
        /**
         *  Getter of the EventManager instance
         */
        public EventManager EventManager { get => eventManager; }

        private List<ClientUser> usersList; /**< This List contains all users displayed ingame*/
        /**
         *  Getter of the Game users list
         */
        public List<ClientUser> UsersList { get => usersList; }

        public Deck CardsPlayed;        /**< This Deck class contains the current pile (in the center of the board)*/
        public int MyId                 /**< This int corresponds to the current player id*/;
        public GAME_STATUS GameStatus   /**< This enum corresponds to the current game status*/;
        public Contract ContractPicked  /**< This List contains all users displayed ingame*/;
        public Pile LastPile;           /**< This Pile class contains the last Pile*/

        /**
         *  This function returns the ClientData by its id
         *  @param  id      The user's id data to be found.
         *  @return The ClientData that contains the user's id [id]
         */
        public ClientUser GetClientUserById(int id)
        {
            for (int i = 0; i < usersList.Count; i++)
            {
                if (usersList[i].Id == id)
                    return (usersList[i]);
            }
            return (null);
        }

        /**
         *  This function returns the ClientPosition of an id compared to another one
         *  @param  myId    Mostly the current user id.
         *  @param  id      The user's id data to be found.
         *  @return The ClientData that contains the user's id [id]
         */
        public ClientUser.ClientPosition GetPosFromId(int myId, int id)
        {
            return (ClientUser.ClientPosition)(((id - myId) + 4) % 4);
        }

        /**
         *  This function add a player into the game userlist
         *  @param  id          The user's id to be added
         *  @param  playerName  The user's name to be added
         *  @param  isMyself    Is the current player or not
         *  @return A boolean if the player as been added or not
         */
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

        /**
         *  This function clear all game infos
         */
        public void RestartGameInfos()
        {
            foreach (var user in usersList)
            {
                user.CardsList.Clear();
                user.Score = 0;
                user.IsPlaying = false;
            }
            CardsPlayed = new Deck();
            GameStatus = GAME_STATUS.WAIT;
            ContractPicked = null;
            LastPile = null;
        }

        /**
         *  GameInfos's constructor - Create an GameInfos instance
         */
        public GameInfos()
        {
            netManager = new NetworkManager();
            eventManager = new EventManager();
            MyId = -1;
            usersList = new List<ClientUser>();
            CardsPlayed = new Deck();
            GameStatus = GAME_STATUS.WAIT;
            ContractPicked = null;
            LastPile = null;
        }
    }
}
