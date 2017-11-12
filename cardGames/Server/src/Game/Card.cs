/**
 *  @file   Card.cs
 *  @author MAxime Cauvin
 *  
 *  This file contains the card Class.
 */

 namespace Game
{
    /**
     *  This class define what a card is.
     */
    public class Card
    {
        /**
         *  This enum define the colors of the card.
         */
        public enum CardColour : int
        {
            Unknown = 0,
            Spades = 1,
            Clubs,
            Diamonds,
            Hearts
        }

        /**
         *  This enum define the values of the card.
         */
        public enum CardValue : int
        {
            Unknown = 0,
            Ace = 1,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
        }

        /**
         *  This enum define the positions of the card.
         */
        public enum CardPosition : int
        {
            NotSpecified = -1,
            Bottom = 0,
            Left = 1,
            Top,
            Right
        }

        /**
         *  This string[] contains the names of the colors.
         */
        private string[] listColours = new string[]
        {
            "Back", "Spades", "Clubs", "Diamonds", "Hearts"
        };

        /**
        *  This string[] contains the names of the values.
        */
        private string[] listValues = new string[]
        {
            "Back", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"
        };

        public CardColour colour;       /**< The color of the card.*/
        public CardValue value;         /**< The value of the card.*/
        public CardPosition position;   /**< The position of the card.*/

        /**
         *  The getter of the name of the color.
         */
        public string StringColour { get => listColours[(int)colour]; }

        /**
         *  The getter of the name of the value.
         */
        public string StringValue { get => listValues[(int)value]; }

        /**
         *  Constructor.
         */
        public Card(CardColour _colour, CardValue _value, CardPosition _position)
        {
            colour = _colour;
            value = _value;
            position = _position;
        }
    }
}
