

using System;
/**
*  @file ServerLobby
*  @author Marc-Anroine Leconte
*
* This file contain the EntryPoint of the server.
*/
namespace Server
{
    /**
     *  This class contain the main function.
     */
    public class ServerLobby
    {
        /**
         *  Main function.
         */
        static void Main(string[] args)
        {
            Server.Instance.debug = true;
            Server.Instance.Open();
            Console.WriteLine("Press any key to stop the server..");
            while (true)
                Server.Instance.Run();
            Server.Instance.Close();
        }
    }
}