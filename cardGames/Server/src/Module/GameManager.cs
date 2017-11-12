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

        public Contract contract;     /**< The current contract.*/
        public Pile pile;             /**< The current pile.*/
        public bool relance;          /**< if the game must be relance.*/

        private Loop _annonceTurn;    /**< The player who must annonce.*/
        private Loop _gameTurn;       /**< The player who must play.*/
        private static readonly object _padlock = new object();    /**< Thread protection.*/

        /**
         *  Getter and Setter for the _status variable.
         *  @return Return the id of the player who annonce.
         */
        public int annonceTurn { get => _annonceTurn.It; }

        /**
         *  Getter and Setter for the _status variable.
         *  @return Return the id of the player who play.
         */
        public int gameTurn { get => _gameTurn.It; }


        /**
         *  Getter and Setter for the _status variable.
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
            pile = new Pile();
            contract = null;
            relance = false;
        }

        /**
         *  This function is triggered when the game is in WAIT mode.
         */
        public void Wait()
        {
            if (Server.Instance.players.list.Count() == 4)
            {
                foreach (var it in Server.Instance.players.list.ToList<Profile>())
                {
                    if (it.ready == false)
                    {
                        Console.WriteLine("Missing player " + it.id.ToString());
                        return;
                    }
                }
                Server.Instance.WriteToAll("010", "The game can start");
                status = GAME_STATUS.DISTRIB;
            }
        }

        /**
         *  Init the game deck;
         */
        private void    InitDeck()
        {
            Server.Instance.PrintOnDebug("\nInitDeck");

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

            Server.Instance.PrintOnDebug("\nDistribution");

            _deck.Clear();
            Reset();
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
                    foreach (var dest in Server.Instance.players.list)
                        Server.Instance.WriteToAll("214", dest.id + ":" + dest.win.CalculPoint(contract).ToString());
                }
                break;
            }
            status = GAME_STATUS.ANNONCE;
            NextAnnonce(true);
        }

        /**
         *  This function change the annonce turn and prevent the players of the changement.
         */
        public void NextAnnonce(bool first = false)
        {
            lock (_padlock)
            {
                if (!first)
                {
                    int cpt = 0;
                    do
                    {
                        _annonceTurn.Next();
                        Server.Instance.PrintOnDebug("______________________loop");
                        if (cpt >= 4)
                        {
                            status = GAME_STATUS.DISTRIB;
                            return;
                        }
                    } while (Server.Instance.players.list[_annonceTurn.It].contract != null && Server.Instance.players.list[_annonceTurn.It].contract.type == CONTRACT_TYPE.PASS);
                    Server.Instance.PrintOnDebug("THE PLAYER WHO ANNONCE IS " + _annonceTurn.It);
                }
            }
            var it = Server.Instance.players.list[_annonceTurn.It];
            Server.Instance.WriteToAll("012", _annonceTurn.It.ToString());
            Server.Instance.PrintOnDebug("Waiting annonce from player " + _annonceTurn.It.ToString());
            Server.Instance.PrintOnDebug("status = " + status.ToString());
        }

        /**
         *  This function check if the annonce made by a player is allowed.
         */
        public bool CheckAnnonce(Contract contract)
        {
            if (contract.type == CONTRACT_TYPE.PASS)
            {
                Server.Instance.players.list[_annonceTurn.It].contract = contract;
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
            Server.Instance.WriteToAll("020", Server.Instance.serializer.ObjectToString(contract));
            NextAnnonce();
            return (true);
        }

        /**
         *  This function is triggered when the game is in ANNONCE mode.
         */
        public void Annonce()
        {
            foreach (var it in Server.Instance.players.list)
            {
                if (it.contract == null)
                    return;
                if (contract != null && it.contract.type != CONTRACT_TYPE.PASS && contract.score > it.contract.score)
                    return;
            }

            Server.Instance.PrintOnDebug("\nThere are 3 pass");
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

        /**
         *  This function find the winner of the game according to the points of the players.
         */
        private int FindWinner()
        {
            int tmp = 0;
            Card origin = pile.cards.cards[0];
            int winnerCard = 0;
            int winnerTrump = -1;
            int winner;

            foreach (var it in pile.cards.cards)
            {
                if (origin != it)
                {
                    if (origin.colour == it.colour)
                    {
                        if (!pile.cards.ExistHigher(it, contract.type))
                            winnerCard = tmp;
                    }
                    else if ((int)origin.colour == (int)contract.type)
                    {
                        if (!pile.cards.ExistHigher(it, contract.type))
                            winnerTrump = tmp;
                    }
                    else if (contract.type == CONTRACT_TYPE.ALL_TRUMP)
                    {
                        if (!pile.cards.ExistHigher(it, (CONTRACT_TYPE)it.colour))
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

        /**
         *  This function change the player who's currently playing and alert the players of the changes.
         *  @param  card    The card played by the player.
         */
        public bool NextTurn(Card card)
        {
            int winner = -1;

            Server.Instance.PrintOnDebug("NEXT TURN");
           
            pile.cards.AddCard(card);
            pile.owners.Add(_gameTurn.It);
            if (pile.cards.Count >= 4)
            {
                winner = FindWinner();
                foreach (var i in pile.cards.cards)
                    Server.Instance.players.list[winner].win.AddCard(i);
                Server.Instance.WriteToAll("212", Server.Instance.serializer.ObjectToString(pile));
                pile.cards.Clear();
                pile.owners.Clear();
                foreach(var dest in Server.Instance.players.list)
                    Server.Instance.WriteToAll("214", dest.id + ":" + dest.win.CalculPoint(contract).ToString());
            }
            lock (_padlock)
            {
                if (winner < 0)
                    _gameTurn.Next();
                else
                    _gameTurn.It = winner;
            }
            var it = Server.Instance.players.list[_gameTurn.It];
            Server.Instance.WriteToAll("212", Server.Instance.serializer.ObjectToString(pile));
            Server.Instance.WriteToAll("013", _gameTurn.It.ToString());
            Server.Instance.PrintOnDebug("Waiting turn from player " + _gameTurn.It.ToString());
            return (true);
        }

        /**
         *  The function check if the card played by the player can be played
         *  @param card The card played by the player.
         */
        public bool CheckCard(Card card)
        {
            Server.Instance.PrintOnDebug("CHECK CARD");

            if (contract == null)
            {
                status = GAME_STATUS.DISTRIB;
                return (false);
            }
            CardColour color = card.colour;

            if (pile.cards.Count != 0)
                color = pile.cards.cards[0].colour;
            Server.Instance.PrintOnDebug("COLOR => " + color.ToString());
            if (card.colour != color)
            {
                if (!Server.Instance.players.list[_gameTurn.It].deck.ExistColour(color))
                {
                    if ((int)card.colour == (int)contract.type)
                    {
                        if (Server.Instance.game.pile.cards.ExistHigher(card, contract.type))
                        {
                            Server.Instance.PrintOnDebug("A card in the deck is higher");
                            return (false);
                        }
                    }
                    else
                    {
                        if (Server.Instance.players.list[_gameTurn.It].deck.ExistColour((CardColour)((int)contract.type)))
                            return (false);
                    }
                    return (NextTurn(card));
                }
                Server.Instance.PrintOnDebug("You have a card with the same color in the deck");
                return (false);
            }
            if ((int)card.colour == (int)contract.type)
            {
                if (Server.Instance.game.pile.cards.ExistHigher(card, contract.type))
                {
                    if (!Server.Instance.players.list[_gameTurn.It].deck.ExistHigher(new Card(card.colour, Server.Instance.game.pile.cards.GetHigher(card, contract.type), 0), contract.type))
                        return (NextTurn(card));
                    Server.Instance.PrintOnDebug("You must play a trump more powerfull");
                    return (false);
                }
            }
            return (NextTurn(card));
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

        /**
         *  Calculate the score of a team according the scores of the two players.
         *  @param  id1 The id of the first player in the team.
         *  @param  id2 The id of the second player in the team.
         */
        public int CalculScore(int id1, int id2)
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

            Server.Instance.PrintOnDebug("\nCount of the points");
            Server.Instance.WriteToAll("040", "Game is finish");
            foreach (var it in Server.Instance.players.list)
            {
                if (it.id == 0 || it.id == 2)
                {
                    if (teamOne > teamTwo)
                        Server.Instance.WriteTo("042", it.ip, it.port, "");
                    else
                        Server.Instance.WriteTo("041", it.ip, it.port, "");
                }
                else
                {
                    if (teamOne < teamTwo)
                        Server.Instance.WriteTo("042", it.ip, it.port, "");
                    else
                        Server.Instance.WriteTo("041", it.ip, it.port, "");
                }
            }
            relance = false;
            status = GAME_STATUS.END;
        }

        /**
         *  This function rest the game parameters
         */
         private void Reset()
        {
            Random rand = new Random();

            foreach (var it in Server.Instance.players.list)
                it.ready = false;
            contract = null;
            foreach (var pl in Server.Instance.players.list)
            {
                pl.deck.Clear();
                pl.win.Clear();
                pl.contract = null;
            }
            _annonceTurn.It = rand.Next(0, 3);
            _gameTurn.It = _annonceTurn.It;
            pile.cards.Clear();
            pile.owners.Clear();
        }

        /**
         *  This function is triggered when the game is in END mode.
         */
        public void End()
        {
            if (relance == true)
            {
                Reset();
                status = GAME_STATUS.WAIT;
                Console.WriteLine("Relance");
            }
        }
    }
}
