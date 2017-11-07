using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientUser
    {
        private string username;
        public string Username { get => username; }

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

        public ClientUser(string _username)
        {
            username = _username;
            cardsList = new List<Card>();
        }
    }
}
