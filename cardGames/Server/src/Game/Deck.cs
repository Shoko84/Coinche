using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Server.Card;

namespace Server
{
    public class Deck
    {
        private List<Card> _cards;

        public int  Count { get => _cards.Count; }

        public Deck()
        {
            _cards = new List<Card>();
        }

        public void AddCard(Card newCard)
        {
            _cards.Add(newCard);
        }

        public void AddCard(CardColour color, CardValue value, CardPosition position)
        {
            Card tmp = new Card(color, value, position);

            _cards.Add(tmp);
        }

        public void RemoveCard(Card card)
        {
            _cards.Remove(card);
        }

        public void RemoveCard(CardColour color, CardValue value, CardPosition position)
        {
            int cpt = 0;

            foreach(var it in _cards)
            {
                if (it.Colour == color
                    && it.Value == value
                    && it.Position == position)
                {
                    _cards.RemoveAt(cpt);
                    break;
                }
                cpt += 1;
            }
        }

        public Card Find(Card card)
        {
            return (_cards.Find(x => x == card));
        }

        public bool ExistColour(CardColour color)
        {
            return (_cards.Exists(x => x.Colour == color));
        }

        public Card GetRandomCard()
        {
            Random rand = new Random();
            return (_cards[rand.Next(0, _cards.Count())]);
        }

        public void Clear()
        {
            _cards.Clear();
        }

        public void Dump()
        {
            foreach (var it in _cards)
                Console.WriteLine("  - " + it.StringValue + " " + it.StringColour);
        }
    }
}
