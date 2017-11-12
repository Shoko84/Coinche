/**
 *  @file   Deck.cs
 *  @author Marc-Antoine Leconte
 *  
 *  This file contains the class Deck.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using static Game.Card;

namespace Game
{
    /**
     *  This class is a list a cards.
     */
    public class Deck
    {
        /**
         *  A list representing the value of all the cards not trump.
         */
        public int[] cardValue = new int[]
        {
            0, 8, 0, 0, 0, 0, 0, 1, 2, 3, 7, 4, 5, 6
        };

        /**
         *  A list representing the value in point of all the cards not trump.
         */
        public int[] cardPoint = new int[]
        {
            0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 10, 2, 3, 4
        };

        /**
         *  A list representing the value of all the trumps.
         */
        public int[] trumpValue = new int[]
       {
            0, 6, 0, 0, 0, 0, 0, 1, 2, 7, 5, 8, 3, 4
       };

        /**
         *  A list representing the value in point of all the trumps.
         */
        public int[] trumpPoint = new int[]
        {
            0, 11, 0, 0, 0, 0, 0, 0, 0, 14, 10, 20, 3, 4
        };

        public List<Card> cards;    /**< The list of all the cards.*/

        /**
         *  Getter and setter of the count of the cards in the deck
         */
        public int Count { get => cards.Count; }

        /**
         *  Constructor.
         */
        public Deck()
        {
            cards = new List<Card>();
        }

        /**
         *  Add a card in the deck from a copy of an other card.
         *  @param  newCard The card to add in the deck.
         */
        public void AddCard(Card newCard)
        {
            cards.Add(new Card(newCard.colour, newCard.value, newCard.position));
        }

        /**
         *  Add a card in the deck.
         *  @param  color       The color of the new card.
         *  @param  value       The value of the new card.
         *  @param  position    The position of the new card.
         */
        public void AddCard(CardColour color, CardValue value, CardPosition position)
        {
            Card tmp = new Card(color, value, position);

            cards.Add(tmp);
        }

        /**
         *  Remove a card in the deck.
         *  @param  card    The card to remove.
         */
        public void RemoveCard(Card card)
        {
            foreach (var c in cards)
            {
                if (c.colour == card.colour && c.value == card.value && c.position == card.position)
                {
                    cards.Remove(c);
                    break;
                }
            }
        }

        /**
         *  Remove a card in the deck.
         *  @param  color       The color of the card to remove.
         *  @param  value       The value of the card to remove.
         *  @param  position    The position of the card to remove.
         */
        public void RemoveCard(CardColour colour, CardValue value, CardPosition position)
        {
            foreach (var c in cards)
            {
                if (c.colour == colour && c.value == value && c.position == position)
                {
                    cards.Remove(c);
                    break;
                }
            }
        }

        /**
         *  Find a card in the deck and return it.
         *  @param  card    The card.
         *  @return The card found.
         */
        public Card Find(Card card)
        {
            return (cards.Find(x => x.colour == card.colour && x.position == card.position && x.value == card.value));
        }

        /**
         *  Check if a card with the color exist in the deck.
         *  @param  color   The color to find.
         *  @return Return true if the color exist and false in the contrary
         */
        public bool ExistColour(CardColour color)
        {
            foreach (var it in cards)
            {
                if (it.colour == color)
                {
                    Console.WriteLine("-----------------------" + it.StringValue + " " + it.StringColour);
                    return (true);
                }
            }
            return (false);
        }

        /**
         *  Pick a random card in the deck.
         *  @return Return the card get randomly.
         */
        public Card GetRandomCard()
        {
            Random rand = new Random();
            return (cards[rand.Next(0, cards.Count())]);
        }

        /**
         *  Clear all the cards in the deck.
         */
        public void Clear()
        {
            cards.Clear();
        }

        /**
         *  Print all the cards in the deck.
         */
        public void Dump()
        {
            foreach (var it in cards)
                Console.WriteLine("  - " + it.StringValue + " " + it.StringColour);
        }

        /**
         *  Check if a higher card exist.
         *  @param  card    The card to compare.
         *  @param  trump   The current trump.
         *  @return Return true if a greatest card in found and false in the contrary.
         */
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
                    {
                        Console.WriteLine("---------------------------------------" + it.StringValue + " " + it.StringColour);
                        return (true);
                    }
                }
            }
            return (false);
        }

        /**
         *  Return the higher card in the deck with the same color.
         *  @param  card    The card permitting to compare the color
         *  @param  trump   The current trump.
         *  @return Return the value of the higher card with the same color.
         */
        public CardValue GetHigher(Card card, CONTRACT_TYPE trump)
        {
            CardValue max = CardValue.Unknown;
            int[] value;

            if ((int)trump == (int)card.colour || trump == CONTRACT_TYPE.ALL_TRUMP)
                value = trumpValue;
            else
                value = cardValue;
            foreach (var it in cards)
            {
                if (it.colour == card.colour)
                {
                    if (max == CardValue.Unknown || value[(int)it.value] > value[(int)max])
                        max = it.value;
                }
            }
            return (max);
        }

        /**
         *  Get the total score of the game.
         *  @param  contract    The current contract.
         *  @return Return the score of the deck.
         */
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
