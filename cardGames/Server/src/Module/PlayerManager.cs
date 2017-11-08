/**
 * @file    PlayerManager.cs
 * @author  Marc-Antoine Leconte
 * 
 * This file contains the PlayerManager Class.
 */

using System.Collections.Generic;

public enum PLAYER_STATUS
{
    ONLINE,
    OFFLINE
};

namespace Server
{
    public  class Contract
    {
    }

    public class Card
    {
        public string value;
        public string color;
    }

    public class Profile
    {
        public string           owner;
        public int              id;
        public string           ip;
        public int              port;
        public List<Card>       deck;
        public PLAYER_STATUS    status;
        public Contract         contract;

        public Profile(int _id, string _ip, int _port)
        {
            owner = "lambda";
            id = _id;
            ip = _ip;
            port = _port;
            deck = new List<Card>();
            status = PLAYER_STATUS.ONLINE;
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
