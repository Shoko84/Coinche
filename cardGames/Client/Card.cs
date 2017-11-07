using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
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
            One,
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

        private string[] listColours = new string[]
        {
            "Back", "Spades", "Clubs", "Diamonds", "Hearts"
        };

        private string[] listValues = new string[]
        {
            "Back", "A", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"
        };

        private CardColour colour = new CardColour();
        public CardColour Colour { get => colour; }
        public string StringColour { get => listColours[(int)colour]; }

        private CardValue value = new CardValue();
        public CardValue Value { get => value; }
        public string StringValue { get => listValues[(int)value]; }

        public Card(CardColour _colour, CardValue _value)
        {
            colour = _colour;
            value = _value;
        }
    }
}
