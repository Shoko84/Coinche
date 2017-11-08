/**
 * @file    Utils.cs
 * @author  Marc-Antoine Leconte
 * 
 * This file contains the Data Class.
 */

namespace Common
{
    /**
    *  This class is a class used in test.
    *  TO_DESTROY
    */
    public class Data
    {
        public string _name;
        public string _msg;
        public Data()
        {
            _msg = "";
            _name = "";
        }
        public Data(string msg, string name)
        {
            _msg = msg;
            _name = name;
        }
    }
}