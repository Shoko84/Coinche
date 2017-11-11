/**
 * @file    GameManager.cs
 * @author  Marc-Antoine Leconte
 * 
 * This file contains the GameManager Class.
 */

using Common;
using Game;
using System;
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

        private static readonly object _padlock = new object();    /**< Thread protection.*/
        private Loop _annonceTurn;
        public int annonceTurn { get => _annonceTurn.It; }

        private Loop _gameTurn;
        public int gameTurn { get => _gameTurn.It; }

        public int nbPass;

        public Contract contract;

        public Deck pile;

        /**
         *  Getter and Setter for the _status var.
         *  @return Return the state of the game.
         */
        public GAME_STATUS status
        {
            get
            {
                lock (_padlock)
                {
                    return this._status;
                }
            }
            set
            {
                lock (_padlock)
                {
                    this._status = value;
                }
            }
        }

        /**
         *  Constructor.
         */
        public GameManager()
        {
            Random rand = new Random();

            status = GAME_STATUS.WAIT;
            _deck = new Deck();
            _annonceTurn = new Loop(0, 3, rand.Next(0, 3));
            _gameTurn = new Loop(0, 3, _annonceTurn.It);
            pile = new Deck();
            nbPass = 0;
            contract = null;
        }

        /**
         *  This function is triggered when the game is in WAIT mode.
         */
        public void Wait()
        {
            if (Server.Instance.players.list.Count() == 4)
            {
                foreach(var it in Server.Instance.players.list)
                {
                    if (it.ready == false)
                        return;
                }
                Server.Instance.WriteToAll("010", "The game can start");
                status = GAME_STATUS.DISTRIB;
            }
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
            Loop turn = new Loop(0, 3);
            _deck.Clear();
            InitDeck();

            while (_deck.Count != 0)
            {
                tmp = _deck.GetRandomCard();
                Server.Instance.players.list[turn.It].deck.AddCard(tmp);
                _deck.RemoveCard(tmp);
                turn.Next();
            }

            foreach (var it in Server.Instance.players.list)
            {
                lock (_padlock)
                {
                    if (Server.Instance.debug)
                    {
                        Console.WriteLine("dumping client deck for id " + it.id + "/" + it.owner + ":");
                        it.deck.Dump();
                    }
                    string msg = Server.Instance.serializer.ObjectToString(it.deck);
                    Server.Instance.WriteTo("211", it.ip, it.port, msg);
                    foreach (var iter in Server.Instance.players.list)
                    {
                        if (iter.ip != it.ip || iter.port != it.port)
                            Server.Instance.WriteTo("213", it.ip, it.port, iter.id + ":" + Server.Instance.players.list[iter.id].deck.Count.ToString());
                    }
                }
                break;
            }
            NextAnnonce(true);
            status = GAME_STATUS.ANNONCE;
        }

        public void NextAnnonce(bool first = false)
        {
            lock (_padlock)
            {
                if (!first)
                {
                    _annonceTurn.Next();
                    Server.Instance.PrintOnDebug("THE PLAYER WHO ANNONCE IS " + _annonceTurn.It);
                }
            }
            var it = Server.Instance.players.list[_annonceTurn.It];
            Server.Instance.WriteToAll("012", _annonceTurn.It.ToString());
            Server.Instance.PrintOnDebug("Waiting annonce from player " + _annonceTurn.It.ToString());
        }

        public bool CheckAnnonce(Contract contract)
        {
            if (contract.type == CONTRACT_TYPE.PASS)
            {
                Server.Instance.players.list[_annonceTurn.It].contract = null;
                nbPass += 1;
                Server.Instance.WriteToAll("020", Server.Instance.serializer.ObjectToString(contract));
                NextAnnonce();
                return (true);
            }
            foreach (var it in Server.Instance.players.list)
            {
                if (it.contract != null && contract != null)
                {
                    if (it.contract.score >= contract.score)
                        return (false);
                }
            }
            Server.Instance.players.list[_annonceTurn.It].contract = contract;
            this.contract = contract;
            nbPass = 0;
            Server.Instance.WriteToAll("020", Server.Instance.serializer.ObjectToString(contract));
            NextAnnonce();
            return (true);
        }

        /**
         *  This function is triggered when the game is in ANNONCE mode.
         */
        public void Annonce()
        {
            if (nbPass >= 3)
            {
                lock (_padlock)
                {
                    if (contract == null)
                        status = GAME_STATUS.DISTRIB;
                    else
                    {
                        status = GAME_STATUS.TURN;
                        var it = Server.Instance.players.list[_gameTurn.It];
                        Server.Instance.WriteToAll("013", _gameTurn.It.ToString());
                        Server.Instance.PrintOnDebug("Waiting turn from player " + _gameTurn.It.ToString());
                    }
                }
            }
        }

        private int FindWinner()
        {
            int tmp = 0;
            Card origin = pile.cards[0];
            int winnerCard = 0;
            int winnerTrump = -1;
            int winner;

            foreach (var it in pile.cards)
            {
                if (origin != it)
                {
                    if (origin.colour == it.colour)
                    {
                        if (!pile.ExistHigher(it, contract.type))
                            winnerCard = tmp;
                    }
                    else if ((int)origin.colour == (int)contract.type)
                    {
                        if (!pile.ExistHigher(it, contract.type))
                            winnerTrump = tmp;
                    }
                    else if (contract.type == CONTRACT_TYPE.ALL_TRUMP)
                    {
                        if (!pile.ExistHigher(it, (CONTRACT_TYPE)it.colour))
                            winnerTrump = tmp;
                    }
                }
                tmp += 1;    
            }
            winner = winnerCard;
            if (winnerTrump != -1)
                winner = winnerTrump;

            winner = winner + _gameTurn.It + 1;
            if (winner >= 4)
                winner -= 4;
            return (winner);
        }

        public bool NextTurn(Card card)
        {
            lock (_padlock)
            {
                pile.AddCard(card);
                if (pile.Count >= 4)
                {
                    int winner = FindWinner();
                    foreach (var i in pile.cards)
                        Server.Instance.players.list[winner].win.AddCard(i);
                    pile.Clear();
                }
                _gameTurn.Next();
            }
            var it = Server.Instance.players.list[_gameTurn.It];
            Server.Instance.WriteToAll("212", Server.Instance.serializer.ObjectToString(pile));
            Server.Instance.WriteToAll("013", _gameTurn.It.ToString());
            Server.Instance.PrintOnDebug("Waiting turn from player " + _gameTurn.It.ToString());
            return (true);
        }

        public bool CheckCard(Card card)
        {
            if (contract == null)
            {
                status = GAME_STATUS.ANNONCE;
                nbPass = 0;
                return (false);
            }
            CardColour color = card.colour;

            if (pile.Count != 0)
                color = pile.cards[0].colour;
            if (card.colour == color)
            {
                if (pile.ExistHigher(card, contract.type))
                {
                    if (Server.Instance.players.list[_gameTurn.It].deck.ExistHigher(card, contract.type))
                        return (false);
                }
                return (NextTurn(card));
            }
            else
            {
                if ((int)card.colour == (int)contract.type)
                {
                    if (pile.ExistHigher(card, contract.type))
                    {
                        if (Server.Instance.players.list[_gameTurn.It].deck.ExistHigher(card, contract.type))
                            return (false);
                    }
                    return (NextTurn(card));
                }
                return (false);
            }
        }

        /**
         *  This function is triggered when the game is in TURN mode.
         */
        public void Turn()
        {
            foreach (var it in Server.Instance.players.list)
            {
                if (it.deck.Count != 0)
                    return;
            }
            status = GAME_STATUS.REFEREE;
        }

        public  int CalculScore(int id1, int id2)
        {
            return (Server.Instance.players.list[id1].win.CalculPoint(contract) + Server.Instance.players.list[id2].win.CalculPoint(contract));
        }

        /**
         *  This function is triggered when the game is in REFEREE mode.
         */
        public void Referee()
        {
            int teamOne = CalculScore(0, 2);
            int teamTwo = CalculScore(1, 3);

            foreach (var it in Server.Instance.players.list)
            {
                Server.Instance.WriteTo("040", it.ip, it.port, "Game is finish");
                if (it.id == 0 || it.id == 2)
                {
                    if (teamOne > teamTwo)
                        Server.Instance.WriteTo("042", it.ip, it.port, "Congratulation");
                    else
                        Server.Instance.WriteTo("041", it.ip, it.port, "You're so bad omg");
                }
                else
                {
                    if (teamOne < teamTwo)
                        Server.Instance.WriteTo("042", it.ip, it.port, "Congratulation");
                    else
                        Server.Instance.WriteTo("041", it.ip, it.port, "You're so bad omg");
                }
            }
        }

        /**
         *  This function is triggered when the game is in END mode.
         */
        public void End()
        {
        }
    }
}
