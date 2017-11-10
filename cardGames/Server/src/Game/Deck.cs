using System;
using System.Collections.Generic;
using System.Linq;
using static Game.Card;

namespace Game
{
    public class Deck
    {
        public List<Card> cards;

        public int Count { get => cards.Count; }

        public Deck()
        {
            cards = new List<Card>();
        }

        public void AddCard(Card newCard)
        {
            cards.Add(new Card(newCard.colour, newCard.value, newCard.position));
        }

        public void AddCard(CardColour color, CardValue value, CardPosition position)
        {
            Card tmp = new Card(color, value, position);

            cards.Add(tmp);
        }

        public void RemoveCard(Card card)
        {
            cards.Remove(card);
        }

        public void RemoveCard(CardColour color, CardValue value, CardPosition position)
        {
            int cpt = 0;

            foreach (var it in cards)
            {
                if (it.colour == color
                    && it.value == value
                    && it.position == position)
                {
                    cards.RemoveAt(cpt);
                    break;
                }
                cpt += 1;
            }
        }

        public Card Find(Card card)
        {
            return (cards.Find(x => x == card));
        }

        public bool ExistColour(CardColour color)
        {
            return (cards.Exists(x => x.colour == color));
        }

        public Card GetRandomCard()
        {
            Random rand = new Random();
            return (cards[rand.Next(0, cards.Count())]);
        }

        public void Clear()
        {
            cards.Clear();
        }

        public void Dump()
        {
            foreach (var it in cards)
                Console.WriteLine("  - " + it.StringValue + " " + it.StringColour);
        }
    }
}
