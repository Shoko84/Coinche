/**
 * @file    ClientUser.cs
 * @author  Maxime Cauvin
 * 
 * This file contains the ClientUser Class.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;

namespace Client
{
    /**
     *  This Class contains all the data used for an User.
     */
    public class ClientUser : object
    {
        /**
         *  This Enumeration defines an User position on screen.
         */
        public enum ClientPosition : int
        {
            NotSpecified = -1,
            Bottom = 0,
            Left = 1,
            Top,
            Right
        }

        private int id; /**< This int corresponds to the user's id*/
        /**
         *  Getter of the user's id.
         *  @return Return the user's id.
         */
        public int Id { get => id; }

        private string username; /**< This string corresponds to the user's name*/
        /**
         *  Getter of the user's name.
         *  @return Return the user's name.
         */
        public string Username { get => username; }

        private ClientPosition position; /**< This enum corresponds to the user's position*/
        /**
         *  Getter of the user's position.
         *  @return Return the user's position.
         */
        public ClientPosition Position { get => position; }

        private Deck cardsList; /**< This Deck class corresponds to the user's deck card*/
        /**
         *  Getter of the user's deck card.
         *  @return Return the user's deck card.
         */
        public Deck CardsList { get => cardsList; set => cardsList = value; }

        /**
         *  This function overrides the object 'Equals' method
         *  @param  obj      The object to test the operation with.
         */
        public override bool Equals(object obj)
        {
            ClientUser other = obj as ClientUser;
            return (this.username == other.Username &&
                    this.position == other.Position &&
                    this.cardsList == other.CardsList);
        }

        /**
         *  This function add a Card to the deck card
         *  @param  card      The card being added.
         */
        public void AddCard(Card card)
        {
            cardsList.AddCard(card);
        }

        /**
         *  This function remove a Card from the deck card
         *  @param  card      The card being removed.
         */
        public void RemoveCard(Card card)
        {
            cardsList.RemoveCard(card);
        }

        /**
         *  ClientUser's constructor - Create an ClientUser instance
         *  @param  _id        The user's id.
         *  @param  _username  The user's name.
         *  @param  _position  The user's position.
         */
        public ClientUser(int _id, string _username, ClientPosition _position)
        {
            id = _id;
            username = _username;
            position = _position;
            cardsList = new Deck();
        }
    }
}
