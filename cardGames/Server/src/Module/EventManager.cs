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
using Game;
using System.Collections.Generic;

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
                Server.Instance.WriteTo("011", ip, port, "Waiting for players");

                List<string> list = new List<string>();

                foreach (var it in Server.Instance.players.list)
                {
                    if (it.ip != ip || it.port != port)
                        list.Add(it.id + ":" + it.owner);
                }
                Server.Instance.WriteTo("030", ip, port, Server.Instance.serializer.ObjectToString(list));

                list.Clear();
                list.Add(id + ":" + message);

                Server.Instance.WriteToOther("030", ip, port, Server.Instance.serializer.ObjectToString(list));
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
            Server.Instance.WriteTo("message", ip, port, "OK");
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
                    Server.Instance.PrintOnDebug("-->[" + it.id + "]: Send Deck");
                    Server.Instance.WriteTo("211", ip, port, msg);
                    break;
                }
            }
        }

        public void SendPile(PacketHeader header, Connection connection, string message)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);
            string serial = Server.Instance.serializer.ObjectToString(Server.Instance.game.pile);

            Server.Instance.PrintOnDebug("-->[]: How many cards");
            Server.Instance.WriteTo("212", ip, port, serial);
        }

        public void HowManyCards(PacketHeader header, Connection connection, string id)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);
            int _id = int.Parse(id);

            Server.Instance.PrintOnDebug("-->[]: How many cards player " + id);
            Server.Instance.WriteTo("213", ip, port, id + ":" + Server.Instance.players.list[_id].deck.Count.ToString());
        }

        public void PlayerAnnonce(PacketHeader header, Connection connection, string message)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);
            Contract annonce = Server.Instance.serializer.StringToObject<Contract>(message);
            string type = "";
            string msg = "";

            foreach (var it in Server.Instance.players.list)
            {
                if (it.ip == ip && it.port == port)
                {
                    if (Server.Instance.game.status != GAME_STATUS.ANNONCE && type == "")
                    {
                        type = "324";
                        msg = Server.Instance.game.status.ToString();
                    }
                    if (Server.Instance.game.gameTurn != it.id && type == "")
                    {
                        type = "320";
                        msg = Server.Instance.game.gameTurn.ToString();
                    }
                    if (!Server.Instance.game.CheckAnnonce(annonce) && type == "")
                        type = "322";
                    if (type == "")
                    {
                        type = "200";
                        Server.Instance.game.NextAnnonce();
                        Server.Instance.WriteToAll("020", message);
                    }
                    Server.Instance.WriteTo(type, ip, port, msg);
                    break;
                }
            }
        }

        public void PlayerPlay(PacketHeader header, Connection connection, string message)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);
            Card card = Server.Instance.serializer.StringToObject<Card>(message);
            string type = "";
            string msg = "";

            foreach (var it in Server.Instance.players.list)
            {
                if (it.ip == ip && it.port == port)
                {
                    if (Server.Instance.game.status != GAME_STATUS.TURN && type == "")
                    {
                        type = "325";
                        msg = Server.Instance.game.status.ToString();
                    }
                    if (Server.Instance.game.gameTurn != it.id && type == "")
                    {
                        type = "320";
                        msg = Server.Instance.game.gameTurn.ToString();
                    }
                    if (it.deck.Find(card) == null && type == "")
                        type = "321";
                    if (!Server.Instance.game.CheckCard(card) && type == "")
                        type = "323";
                    if (type == "")
                    {
                        type = "200";
                        Server.Instance.WriteToAll("021", message);
                    }
                    Server.Instance.WriteTo(type, ip, port, msg);
                    break;
                }
            }
        }

        public void GetScore(PacketHeader header, Connection connection, string id)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);

            Server.Instance.WriteTo("214", ip, port, id + ":" + Server.Instance.players.list[int.Parse(id)].win.CalculPoint(Server.Instance.game.contract).ToString());
        }

        
        public void PlayerReady(PacketHeader header, Connection connection, string id)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);

            foreach (var it in Server.Instance.players.list)
            {
                if (it.ip == ip && it.port == port)
                {
                    if (Server.Instance.game.status != GAME_STATUS.WAIT)
                        Server.Instance.WriteTo("010", ip, port, "");
                    it.ready = true;
                    return;
                }
            }
        }

    }
}
