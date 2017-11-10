using System;
using System.Collections.Generic;
using System.Linq;
using static Game.Card;

namespace Game
{
    public class Deck
    {
        public int[] cardValue = new int[]
        {
            0, 8, 0, 0, 0, 0, 0, 1, 2, 3, 7, 4, 5, 6
        };

        public int[] cardPoint = new int[]
        {
            0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 10, 2, 3, 4
        };

        public int[] trumpValue = new int[]
       {
            0, 6, 0, 0, 0, 0, 0, 1, 2, 7, 5, 8, 3, 4
       };

        public int[] trumpPoint = new int[]
        {
            0, 11, 0, 0, 0, 0, 0, 0, 0, 14, 10, 20, 3, 4
        };

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

        public bool ExistHigher(Card card, CONTRACT_TYPE trump)
        {
            int[] value;

            if ((int)trump == (int)card.colour || trump == CONTRACT_TYPE.ALL_TRUMP)
                value = trumpValue;
            else
                value = cardValue;
            foreach (var it in cards)
            {
                if (it.colour == card.colour)
                {
                    if (value[(int)it.value] > value[(int)card.value])
                        return (true);
                }
            }
            return (false);
        }

        public int CalculPoint(Contract contract)
        {
            int score = 0;

            foreach (var it in cards)
            {
                if ((int)it.colour == (int)contract.type || contract.type == CONTRACT_TYPE.ALL_TRUMP)
                    score += trumpPoint[(int)it.value];
                else
                    score += cardPoint[(int)it.value];
                if ((int)it.value == 1 && contract.type == CONTRACT_TYPE.WITHOUT_TRUMP)
                    score += 8;
            }
            if (contract.type == CONTRACT_TYPE.ALL_TRUMP)
                score = score * 162 / 258;
            return (score);
        }

    }
}
