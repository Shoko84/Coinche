using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Card
    {
        public enum CardColour : int
        {
            Unknown = 0,
            Spades = 1,
            Clubs,
            Diamonds,
            Hearts
        }

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

        public enum CardPosition : int
        {
            NotSpecified = -1,
            Bottom = 0,
            Left = 1,
            Top,
            Right
        }

        private string[] listColours = new string[]
        {
            "Back", "Spades", "Clubs", "Diamonds", "Hearts"
        };

        private string[] listValues = new string[]
        {
            "Back", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"
        };

        public CardColour colour;
        public string StringColour { get => listColours[(int)colour]; }

        public CardValue value;
        public string StringValue { get => listValues[(int)value]; }

        public CardPosition position;


        public Card(CardColour _colour, CardValue _value, CardPosition _position)
        {
            colour = _colour;
            value = _value;
            position = _position;
        }
    }
}
