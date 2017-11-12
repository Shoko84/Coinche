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

        public int Id;                  /**< This int corresponds to the user's id*/
        public string Username;         /**< This string corresponds to the user's name*/
        public ClientPosition Position; /**< This enum corresponds to the user's position*/
        public Deck CardsList;          /**< This Deck class corresponds to the user's deck card*/
        public int Score;               /**< This integer corresponds to the user's score*/
        public bool IsPlaying;          /**< This boolean corresponds if the user is playing*/

        /**
         *  This function overrides the object 'Equals' method
         *  @param  obj      The object to test the operation with.
         */
        public override bool Equals(object obj)
        {
            ClientUser other = obj as ClientUser;
            return (Username == other.Username &&
                    Position == other.Position &&
                    CardsList == other.CardsList);
        }

        /**
         *  This function add a Card to the deck card
         *  @param  card      The card being added.
         */
        public void AddCard(Card card)
        {
            CardsList.AddCard(card);
        }

        /**
         *  This function remove a Card from the deck card
         *  @param  card      The card being removed.
         */
        public void RemoveCard(Card card)
        {
            CardsList.RemoveCard(card);
        }

        /**
         *  ClientUser's constructor - Create an ClientUser instance
         *  @param  _id        The user's id.
         *  @param  _username  The user's name.
         *  @param  _position  The user's position.
         */
        public ClientUser(int _id, string _username, ClientPosition _position)
        {
            Id = _id;
            Username = _username;
            Position = _position;
            CardsList = new Deck();
            Score = 0;
            IsPlaying = false;
        }
    }
}
