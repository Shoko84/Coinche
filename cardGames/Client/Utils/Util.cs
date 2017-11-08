using Newtonsoft.Json;

namespace Common
{
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

    public class Serializer
    {
        public string ObjectToString(object obj)
        {
            return (JsonConvert.SerializeObject(obj));
        }
        public T StringToObject<T>(string str)
        {
            T obj = JsonConvert.DeserializeObject<T>(str);
            return (obj);
        }
    }
}