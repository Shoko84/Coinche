/**
 *  @file   Pile.cs
 *  @author Marc-Antoine Leconte
 *  
 *  This file contain the Pile class.
 */

using System.Collections.Generic;

namespace Game
{
    /**
     *  This class represent the last 4 cards played by the players.
     */
    public class Pile
    {
        public Deck         cards;  /**< The last four cards.*/
        public List<int>    owners; /**< The owners of the last four cards.*/

        /**
         *  Constructor
         */
        public Pile()
        {
            cards = new Deck();
            owners = new List<int>();
        }
    }
}
