using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;

namespace Client
{
    public class ClientUser : object
    {
        public enum ClientPosition : int
        {
            NotSpecified = -1,
            Bottom = 0,
            Left = 1,
            Top,
            Right
        }

        private int id;
        public int Id { get => id; }

        private string username;
        public string Username { get => username; }

        private ClientPosition position;
        public ClientPosition Position { get => position; }

        private Deck cardsList;
        public Deck CardsList { get => cardsList; set => cardsList = value; }

        public override bool Equals(object obj)
        {
            ClientUser other = obj as ClientUser;
            return (this.username == other.Username &&
                    this.position == other.Position &&
                    this.cardsList == other.CardsList);
        }

        public void AddCard(Card card)
        {
            cardsList.AddCard(card);
        }

        public void RemoveCard(Card card)
        {
            cardsList.RemoveCard(card);
        }

        public ClientUser(int _id, string _username, ClientPosition _position)
        {
            id = _id;
            username = _username;
            position = _position;
            cardsList = new Deck();
        }
    }
}
