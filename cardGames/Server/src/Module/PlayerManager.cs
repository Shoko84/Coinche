/**
 * @file    PlayerManager.cs
 * @author  Marc-Antoine Leconte
 * 
 * This file contains the PlayerManager Class.
 */

using System.Collections.Generic;
using Game;

/**
 *  This enum define the player state.
 */
public enum PLAYER_STATUS
{
    ONLINE,
    OFFLINE
};

namespace Server
{
    /**
     *  This class define a player.
     */
    public class Profile
    {
        public string           owner;      /**< The user name.*/
        public int              id;         /**< The id of the player (define the turn order).*/
        public string           ip;         /**< The ip of the player.*/
        public int              port;       /**< The port of the player.*/
        public Deck             deck;       /**< The deck of the player.*/
        public PLAYER_STATUS    status;     /**< The status of the player.*/
        public Contract         contract;   /**< The contract of the player.*/
        public Deck             win;        /**< The deck containing all the cards the player won.*/
        public bool             ready;      /**< A boolean describing if the client is ready to get info from the serv.*/
        
        /**
         *  Constructor.
         *  @param  name    The name of the player.
         *  @param  _id     The id of the player.
         *  @param  _ip     The ip of the player.
         *  @param  _port   The port of the player.
         */
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
            ready = false;
        }
    }

    /**
     *  This class contain the representation of all the players.
     */
    public class PlayerManager
    {
        public List<Profile> list;  /**< The list of all the players.*/

        /**
         *  Constructor.
         */
        public PlayerManager()
        {
            list = new List<Profile>();
        }
    }
}
