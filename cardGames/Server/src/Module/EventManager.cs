/**
 * @file    EventManager.cs
 * @author  Marc-Antoine Leconte
 * 
 * This file contains the EventManager Class.
 */

using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet;
using System.Linq;
using Common;
using System;
using System.Threading;

namespace Server
{
/**
 *  This class contain all the functions which will be triggered by the client's requests.
 */
    public class EventManager
    {
        /**
         *  Constructor.
         */
        public EventManager()
        {
        }

        /**
         *  Triggered when the client asked to connect.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the client connection.
         *  @param   message     The name of the user.
         */
        public void Connection(PacketHeader header, Connection connection, string message)
        {
            bool connect = false;
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);
            int id = 0;

            Server.Instance.PrintOnDebug("A new player try to connect " + ip + " " + port + " " + id + " " + message);

            if (Server.Instance.players.list.Count() < 4)
            {
                id = Server.Instance.players.list.Count();
                Profile newDeck = new Profile(message, id, ip, port);
                Server.Instance.players.list.Add(newDeck);
                connect = true;
            }
            else
            {
                foreach (var entry in Server.Instance.players.list)
                {
                    if (entry.ip == ip && entry.port == port && entry.status == PLAYER_STATUS.OFFLINE)
                    {
                        id = entry.id;
                        Server.Instance.players.list[entry.id].status = PLAYER_STATUS.ONLINE;
                        connect = true;
                        break;
                    }
                }
                foreach (var entry in Server.Instance.players.list)
                {
                    if (entry.status == PLAYER_STATUS.OFFLINE)
                    {
                        id = entry.id;
                        Server.Instance.players.list[entry.id].ip = ip;
                        Server.Instance.players.list[entry.id].port = port;
                        Server.Instance.players.list[entry.id].status = PLAYER_STATUS.ONLINE;
                        Server.Instance.players.list[entry.id].owner = message;
                        connect = true;
                        break;
                    }
                }
            }
            if (connect == true)
            {
                Server.Instance.PrintOnDebug("A new player just connect " + ip + " " + port + " " + id);
                Server.Instance.WriteTo("230", ip, port, id.ToString() + ":" + message);
                if (Server.Instance.players.list.Count() == 4)
                {
                    if (Server.Instance.game.status == GAME_STATUS.WAIT)
                        Server.Instance.WriteToAll("010", "The game can start");
                    else
                        Server.Instance.WriteTo("010", ip, port, "The game can start");
                }
                else
                    Server.Instance.WriteTo("011", ip, port, "Waiting for players");
                foreach (var it in Server.Instance.players.list)
                {
                    if (it.id != id || it.id != port)
                    {
                        Server.Instance.WriteTo("030", ip, port, it.id + ":" + it.owner);
                        Console.WriteLine("030" + " " + ip + " " + port + " " + it.id + ":" + it.owner);
                    }
                }
                Server.Instance.WriteToOther("030", ip, port, id + ":" + message);
                Console.WriteLine("030" + " " + ip + " " + port + " " + id + ":" + message);
            }
            else
                Server.Instance.WriteTo("330", ip, port, "Sorry, there are too many client which are already connected");
        }

        /**
        *   Triggered when the client rename himself.
        *   @param   header      Infos about the header.
        *   @param   connection  Infos about the client connection.
        *   @param   message     The new name of the user.
        */
        public void Rename(PacketHeader header, Connection connection, string message)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);

            foreach (var entry in Server.Instance.players.list)
            {
                if (entry.ip == ip && entry.port == port)
                {
                    Server.Instance.PrintOnDebug("A player " + entry.ip + " rename: " + message);
                    Server.Instance.WriteToOther("031", ip, port, entry.id + "|" + Server.Instance.players.list[entry.id].owner + "|" + message);
                    Server.Instance.players.list[entry.id].owner = message;
                    Server.Instance.WriteTo("200", ip, port, "Rename OK");
                    break;
                }
            }
        }

        /**
        *   Triggered when the client disconnect himself.
        *   @param   header      Infos about the header.
        *   @param   connection  Infos about the client connection.
        *   @param   message     Message of deconnection.
        */
        public void Deconnection(PacketHeader header, Connection connection, string message)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);

            foreach (var entry in Server.Instance.players.list)
            {
                if (entry.ip == ip && entry.port == port)
                {
                    Server.Instance.PrintOnDebug("A player Quit " + entry.ip + " " + entry.port + " " + entry.id);
                    Server.Instance.WriteToOther("052", entry.ip, entry.port, entry.id + " Has quit");
                    Server.Instance.players.list[entry.id].status = PLAYER_STATUS.OFFLINE;
                    break;
                }
            }
        }

        /**
        *   Triggered when the client write a message.
        *   @param   header      Infos about the header.
        *   @param   connection  Infos about the client connection.
        *   @param   message     The message of the user.
        */
        public void ReceiveMessage(PacketHeader header, Connection connection, string message)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);

            Server.Instance.PrintOnDebug(message);
            Server.Instance.WriteTo("Response", ip, port, "OK");
        }

        /**
        *   Triggered when the client send a Data object.
        *   @param   header      Infos about the header.
        *   @param   connection  Infos about the client connection.
        *   @param   obj         An instance of a Data object in string.
        */
        public void ReceiveData(PacketHeader header, Connection connection, string obj)
        {
            Serializer serializer = new Serializer();
            Data data = serializer.StringToObject<Data>(obj);
            Console.WriteLine("[" + data._name + "]: " + data._msg);
        }

        public void SendDeck(PacketHeader header, Connection connection, string message)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);

            Serializer serializer = new Serializer();
            foreach (var it in Server.Instance.players.list)
            {
                if (it.ip == ip && it.port == port)
                {
                    string msg = serializer.ObjectToString(it.deck);
                    Console.WriteLine("dumping client deck for id " + it.id + ":");
                    it.deck.Dump();
                    Server.Instance.WriteTo("211", ip, port, msg);
                    Console.WriteLine("211" + " " + ip + " " + port + " " + msg);
                    break;
                }
            }
        }

        public void SendPile(PacketHeader header, Connection connection, string message)
        {

        }

        public void SendPlayedCards(PacketHeader header, Connection connection, string message)
        {

        }
    }
}
