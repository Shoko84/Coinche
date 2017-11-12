/**
 *  @file   Contract.cs
 *  @author Marc-Antoine Leconte
 *  
 *  This file contains the contract Class.
 */

 namespace Game
{
    /**
     *  This enum list the types of trump in the contract.
     */
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

    /**
     *  This class describe what a contract is.
     */
    public class Contract
    {
        /**
         *  This string[] contains the name of all the trump types.
         */
        private string[] typeStr = new string[]
        {
            "Pass", "Spades", "Clubs", "Diamonds", "Hearts", "All trumps", "Without trumps"
        };

        public int              score;  /**< The score of the contract.*/
        public CONTRACT_TYPE    type;   /**< The type of the trump.*/
        public int              id;     /**< The id of the owner of the contract.*/

        /**
         *  Getter of the string name of the type of trump.
         */
        public string StringType { get => typeStr[(int)type]; }

        /**
         *  Constructor
         */
        public Contract(int _score, CONTRACT_TYPE _type, int _id)
        {
            score = _score;
            type = _type;
            id = _id;
        }
    }
}
