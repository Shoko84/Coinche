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
                    if (it.id != id)
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

            foreach (var it in Server.Instance.players.list)
            {
                if (it.ip == ip && it.port == port)
                {
                    Server.Instance.PrintOnDebug(message);
                    Server.Instance.WriteToAll("msg", it.owner + ":" + message);
                }
            }
        }

        /**
        *   Triggered when the client ask his deck to the server.
        *   @param   header      Infos about the header.
        *   @param   connection  Infos about the client connection.
        *   @param   message     Unused.
        */
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

        /**
        *   Triggered when the client ask The pile to the server.
        *   @param   header      Infos about the header.
        *   @param   connection  Infos about the client connection.
        *   @param   message     Unused.
        */
        public void SendPile(PacketHeader header, Connection connection, string message)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);
            string serial = Server.Instance.serializer.ObjectToString(Server.Instance.game.pile);

            Server.Instance.PrintOnDebug("-->[]: How many cards");
            Server.Instance.WriteTo("212", ip, port, serial);
        }

        /**
        *   Triggered when the client ask the number of card of a player.
        *   @param   header      Infos about the header.
        *   @param   connection  Infos about the client connection.
        *   @param   message     The id of the player he want to know the number of card.
        */
        public void HowManyCards(PacketHeader header, Connection connection, string id)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);
            int _id = int.Parse(id);

            Server.Instance.PrintOnDebug("-->[]: How many cards player " + id);
            Server.Instance.WriteTo("213", ip, port, id + ":" + Server.Instance.players.list[_id].deck.Count.ToString());
        }

        /**
        *   Triggered when the client want to make an annonce.
        *   @param   header      Infos about the header.
        *   @param   connection  Infos about the client connection.
        *   @param   serial      The serialisation of a Contract object.
        */
        public void PlayerAnnonce(PacketHeader header, Connection connection, string serial)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);
            Contract annonce = Server.Instance.serializer.StringToObject<Contract>(serial);
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
                    if (Server.Instance.game.annonceTurn != it.id && type == "")
                    {
                        type = "320";
                        msg = Server.Instance.game.annonceTurn.ToString();
                    }
                    if (!Server.Instance.game.CheckAnnonce(annonce) && type == "")
                        type = "322";
                    if (type == "")
                        type = "200";
                    Server.Instance.WriteTo(type, ip, port, msg);
                    Server.Instance.WriteTo("211", ip, port, Server.Instance.serializer.ObjectToString(Server.Instance.players.list[it.id].deck));
                    foreach (var itA in Server.Instance.players.list)
                    {
                        if (itA.id != it.id)
                            Server.Instance.WriteTo("213", itA.ip, itA.port, it.id + ":" + Server.Instance.players.list[it.id].deck.Count.ToString());
                    }
                    break;
                }
            }
           
        }

        /**
        *   Triggered when the client want to play a card.
        *   @param   header      Infos about the header.
        *   @param   connection  Infos about the client connection.
        *   @param   serial      Serialisation of a Card object.
        */
        public void PlayerPlay(PacketHeader header, Connection connection, string serial)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);
            Card card = Server.Instance.serializer.StringToObject<Card>(serial);

            Server.Instance.PrintOnDebug("THE PLAYER WANT TO PLAY");

            foreach (var it in Server.Instance.players.list)
            {
                if (it.ip == ip && it.port == port)
                {
                    if (Server.Instance.game.status != GAME_STATUS.TURN)
                        Server.Instance.WriteTo("325", ip, port, Server.Instance.game.status.ToString());
                    if (Server.Instance.game.gameTurn != it.id)
                        Server.Instance.WriteTo("320", ip, port, Server.Instance.game.gameTurn.ToString());
                    if (it.deck.Find(card) == null)
                        Server.Instance.WriteTo("321", ip, port, "");
                    if (!Server.Instance.game.CheckCard(card))
                        Server.Instance.WriteTo("323", ip, port, "");
                    else
                    {
                        Server.Instance.PrintOnDebug("THE PLAYER PLAY RIGHT");
                        Server.Instance.players.list[it.id].deck.RemoveCard(card);
                        Server.Instance.WriteToAll("021", serial);
                    }

                    Server.Instance.WriteTo("211", ip, port,Server.Instance.serializer.ObjectToString(Server.Instance.players.list[it.id].deck));
                    Server.Instance.WriteToAll("212", Server.Instance.serializer.ObjectToString(Server.Instance.game.pile));
                    foreach (var itA in Server.Instance.players.list)
                    {
                        if (itA.id != it.id)
                            Server.Instance.WriteTo("213", itA.ip, itA.port, it.id + ":" + Server.Instance.players.list[it.id].deck.Count.ToString());
                    }
                    break;
                }
            }
        }

        /**
        *   Triggered when the client want to play a card.
        *   @param   header      Infos about the header.
        *   @param   connection  Infos about the client connection.
        *   @param   id          The id of the player from who the player want to know the score.
        */
        public void GetScore(PacketHeader header, Connection connection, string id)
        {
            var ip = connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[0];
            var port = int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':')[1]);

            Server.Instance.WriteTo("214", ip, port, id + ":" + Server.Instance.players.list[int.Parse(id)].win.CalculPoint(Server.Instance.game.contract).ToString());
        }

        /**
        *   Triggered when the client want to play a card.
        *   @param   header      Infos about the header.
        *   @param   connection  Infos about the client connection.
        *   @param   id          The id of the player who's ready.
        */
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
