/**
 * @file    NetworkManager.cs
 * @author  Marc-Antoine Leconte
 * 
 * This file contains the NetworkManager Class.
 */

using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet;
using System;

namespace Server
{
    /**
     *  This class countain all the functions representing the server's network.
     */
    public class NetworkManager
    {
        /**
         *  Constructor.
         */
        public NetworkManager()
        {
        }

        /**
         *  This function link the types of the events to triggered, to their associated functions.
         */
        public void InitFunc()
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("msg", Server.Instance.events.ReceiveMessage);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("050", Server.Instance.events.Deconnection);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("130", Server.Instance.events.Connection);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("131", Server.Instance.events.Rename);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("111", Server.Instance.events.SendDeck);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("112", Server.Instance.events.SendPile);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("113", Server.Instance.events.HowManyCards);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("114", Server.Instance.events.GetScore);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("120", Server.Instance.events.PlayerAnnonce);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("121", Server.Instance.events.PlayerPlay);
        }

        /**
         *  This function permitt to start the listening of the clients by the server.
         */
        public void StartServer()
        {
            Server.Instance.PrintOnDebug("Start the listen of the server.");
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0));
        }

        /**
         *  This function display all the IP EndPoints.
         */
        public void ListEndPoints()
        {
            Server.Instance.PrintOnDebug("Server listening for TCP connection on :");
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
                Console.WriteLine("\t{0}:{1}", localEndPoint.Address, localEndPoint.Port);
        }

        /**
         *  This function Stop the server.
         */
        public void StopServer()
        {
            NetworkComms.Shutdown();
        }
    }
}
