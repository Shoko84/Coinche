/**
 * @file    Utils.cs
 * @author  Marc-Antoine Leconte
 * 
 * This file contains different util Class.
 */

using Newtonsoft.Json;

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

    /**
     * This class is a class permit to serialize and unserialized an object.
     */
    public class Serializer
    {
        /**
         *  This function serialize an object into a string.
         *  @param  obj An instance of a object.
         *  @return Return a string containing the serialisation of the object obj.
         */
        public string ObjectToString(object obj)
        {
            return (JsonConvert.SerializeObject(obj));
        }

        /**
         *  This function serialize a string into an object.
         *  @param  str A string containing the serialisation of an object.
         *  @return Return an object corresponding to the serialized object contain in the string str.
         */
        public T StringToObject<T>(string str)
        {
            T obj = JsonConvert.DeserializeObject<T>(str);
            return (obj);
        }
    }
}