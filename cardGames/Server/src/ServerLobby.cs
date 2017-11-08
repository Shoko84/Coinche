namespace Server
{
    public class ServerLobby
    {
        static void Main(string[] args)
        {
            Server.Instance.debug = true;
            Server.Instance.Open();

            while (true)
            {
                Server.Instance.Run();
            }
            Server.Instance.Close();
        }
    }
}