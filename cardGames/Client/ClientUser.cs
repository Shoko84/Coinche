using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientUser
    {
        public enum ClientPosition : int
        {
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

        private List<Card> cardsList;
        public List<Card> CardsList { get => cardsList; }

        public void AddCard(Card card)
        {
            cardsList.Add(card);
        }

        public void RemoveCard(Card card)
        {
            // TODO
        }

        public ClientUser(int _id, string _username, ClientPosition _position)
        {
            id = _id;
            username = _username;
            position = _position;
            cardsList = new List<Card>();
        }
    }
}
