using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public enum CONTRACT_TYPE
    {
        PASS = 0,
        SPADES,
        CLUBS,
        DIAMONDS,
        HEARTS,
        ALL_TRUMP,
        WITHOUT_TRUMP
    };

    public class Contract
    {
        private string[] typeStr = new string[]
        {
            "Pass", "Spades", "Clubs", "Diamonds", "Hearts", "All trumps", "Without trumps"
        };

        public int              score;
        public CONTRACT_TYPE    type;
        public int              id;
        public string           StringType { get => typeStr[(int)type]; }

        public Contract(int _score, CONTRACT_TYPE _type, int _id)
        {
            score = _score;
            type = _type;
            id = _id;
        }
    }
}
