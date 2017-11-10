/**
 * @file    PlayerManager.cs
 * @author  Marc-Antoine Leconte
 * 
 * This file contains the PlayerManager Class.
 */

using System.Collections.Generic;
using Game;

public enum PLAYER_STATUS
{
    ONLINE,
    OFFLINE
};

namespace Server
{
    public class Profile
    {
        public string           owner;
        public int              id;
        public string           ip;
        public int              port;
        public Deck             deck;
        public PLAYER_STATUS    status;
        public Contract         contract;
        public Deck             win;

        public Profile(string name, int _id, string _ip, int _port)
        {
            owner = name;
            id = _id;
            ip = _ip;
            port = _port;
            deck = new Deck();
            status = PLAYER_STATUS.ONLINE;
            contract = null;
            win = new Deck();
        }
    }

    public class PlayerManager
    {
        public List<Profile> list;

        public PlayerManager()
        {
            list = new List<Profile>();
        }
    }
}
