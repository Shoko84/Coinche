/**
 * @file    GameManager.cs
 * @author  Marc-Antoine Leconte
 * 
 * This file contains the GameManager Class.
 */

using System.Linq;

namespace Server
{
    /**
     *  This enum list all the different states of the game.
     */
    public enum GAME_STATUS
    {
        WAIT,
        DISTRIB,
        ANNONCE,
        TURN,
        REFEREE,
        END
    }

    /**
     *  This class permit to rule the game.
     */
    public class GameManager
    {
        private GAME_STATUS _status;  /**< This var determine the state of the game.*/

        /**
         *  Getter and Setter for the _status var.
         *  @return Return the state of the game.
         */
        public GAME_STATUS  status
        {
            get
            {
                return (this._status);
            }
            set
            {
                this._status = value;
            }

        }

        /**
         *  Constructor.
         */
        public GameManager()
        {
            status = GAME_STATUS.WAIT;
        }

        /**
         *  This function is triggered when the game is in WAIT mode.
         */
        public void Wait()
        {
            if (Server.Instance.players.list.Count() == 4)
                status = GAME_STATUS.DISTRIB;
        }

        /**
         *  This function is triggered when the game is in DISTRIB mode.
         */
        public void Distrib()
        {
        }

        /**
         *  This function is triggered when the game is in ANNONCE mode.
         */
        public void Annonce()
        {
        }

        /**
         *  This function is triggered when the game is in TURN mode.
         */
        public void Turn()
        {
        }

        /**
         *  This function is triggered when the game is in REFEREE mode.
         */
        public void Referee()
        {
        }

        /**
         *  This function is triggered when the game is in END mode.
         */
        public void End()
        {
        }
    }
}
