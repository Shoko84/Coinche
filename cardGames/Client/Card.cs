using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Card : Object
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

        private CardColour colour;
        public CardColour Colour { get => colour; }
        public string StringColour { get => listColours[(int)colour]; }

        private CardValue value;
        public CardValue Value { get => value; }
        public string StringValue { get => listValues[(int)value]; }

        private CardPosition position;
        public CardPosition Position { get => position; }

        public override bool Equals(object obj)
        {
            Card other = obj as Card;
            return (this.colour == other.Colour &&
                    this.value == other.Value &&
                    this.position == other.Position);
        }

        public Card(CardColour _colour, CardValue _value, CardPosition _position)
        {
            colour = _colour;
            value = _value;
            position = _position;
        }
    }
}
