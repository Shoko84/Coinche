using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Pile
    {
        public Deck cards;
        public List<int> owners;

        public Pile()
        {
            cards = new Deck();
            owners = new List<int>();
        }
    }
}
