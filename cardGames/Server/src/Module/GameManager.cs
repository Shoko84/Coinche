/**
 * @file    GameManager.cs
 * @author  Marc-Antoine Leconte
 * 
 * This file contains the GameManager Class.
 */

using Game;
using System.Linq;
using static Game.Card;

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
        private Deck _deck;           /**< The deck containing all the cards.*/

        /**
         *  Getter and Setter for the _status var.
         *  @return Return the state of the game.
         */
        public GAME_STATUS  status
        {
            get => (this._status);
            set => this._status = value;
        }

        /**
         *  Constructor.
         */
        public GameManager()
        {
            status = GAME_STATUS.WAIT;
            _deck = new Deck();
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
         *  Init the initial deck;
         */

        private void    InitDeck()
        {
            CardColour color = CardColour.Hearts;
            CardValue   value = CardValue.King;
            CardPosition position = CardPosition.Bottom;

            while (color != CardColour.Unknown)
            {
                value = CardValue.King;
                while (value != CardValue.Unknown)
                {
                    if (value >= CardValue.Seven || value == CardValue.Ace)
                        _deck.AddCard(color, value, position);
                    value -= 1;
                }
                color -= 1;
            }
        }

        /**
         *  This function is triggered when the game is in DISTRIB mode.
         */
        public void Distrib()
        {
            Card tmp;
            int turn = 0;
            _deck.Clear();
            InitDeck();

            while (_deck.Count != 0)
            {
                tmp = _deck.GetRandomCard();
                Server.Instance.players.list[turn].deck.AddCard(tmp);
                _deck.RemoveCard(tmp);
                turn += 1;
                if (turn == 4)
                    turn = 0;
            }

            if (Server.Instance.debug)
            {
                foreach (var it in Server.Instance.players.list)
                {
                    lock (it)
                    {
                        Server.Instance.PrintOnDebug("Player " + it.owner + ": ");
                        it.deck.Dump();
                    }
                }
            }
            status = GAME_STATUS.ANNONCE;
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
