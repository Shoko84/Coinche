/**
 *  @file Server.cs
 *  @author Marc-Antoine Leconte
 *  
 *  This file contain the server class.
 */

using NetworkCommsDotNet;
using Common;
using System;

namespace Server
{
    /**
     *  This class contain all the module composing the server.
     *  It contains the I/O module too.
     *  
     *  This class is a singleton.
     */
    public class Server
    {
        public NetworkManager           network;    /**< Contains the network gestion.*/
        public PlayerManager            players;    /**< List all the player's ips by id.*/
        public EventManager             events;     /**< List of all the events.*/
        public GameManager              game;       /**< Contains the state of the game.*/
        public Serializer               serializer; /**< Object wich serialize and deserialize objects.*/

        private bool                    _debug;     /**< Allow or not debug prints.*/

        private static Server           _instance;  /**< The instance of the singleton.*/
        private static readonly object  _padlock = new object();    /**< Thread protection.*/

        /**
         *  Getter of the singleton.
         */
        public static Server Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new Server();
                    }
                    return (_instance);
                }
            }
        }

       /**
        *   Getter / Setter of the status of debug.
        */
        public bool debug
        {
            get => (this._debug);
            set => this._debug = value;
        }

        /**
         *  Constructor
         *  @param  debug   Allow or not the debug prints.
         */
        public Server(bool debug = false)
        {
            this._debug = debug;
            this.network = new NetworkManager();
            this.players = new PlayerManager();
            this.events = new EventManager();
            this.game = new GameManager();
            this.serializer = new Serializer();
        }

        /**
         *  Open the server.
         */
        public void Open()
        {
            network.InitFunc();
            network.StartServer();
            network.ListEndPoints();
        }

        /**
         *  Close the server.
         */
        public void Close()
        {
            network.StopServer();
        }

        /**
         *  Print an Error.
         *  @param  type    The type of the error.
         *  @param  msg     The error message.
         */
        public void Error(string type, string msg)
        {
            Console.WriteLine("Error[" + type + "]: " + msg);
        }

        /**
         *  Print something wich can only be display in debug mode.
         *  @param  msg The message to print.
         */
        public void PrintOnDebug(string msg)
        {
            if (this._debug)
                Console.WriteLine("## " + msg);
        }

        /**
         *  Send a string to a client.
         *  @param  type    The type of the event.
         *  @param  ip      The ip of the client.
         *  @param  port    The port of the client.
         *  @param  msg     The message to send to the client.
         */
        public void WriteTo(string type, string ip, int port, string msg)
        {
            foreach(var pl in Server.Instance.players.list)
            {
                if (pl.ip == ip && pl.port == port)
                {
                    if (pl.status == PLAYER_STATUS.OFFLINE)
                        return;
                    break;
                }
            }
            lock (_padlock)
            {
                try
                {
                    NetworkComms.SendObject(type, ip, port, msg);
                    PrintOnDebug("[" + type + "]: " + msg);
                }
                catch (Exception)
                {
                    this.Error("Error", "the client is not connected");
                    foreach (var entry in players.list)
                    {
                        if (entry.ip == ip && entry.port == port)
                        {
                            if (players.list[entry.id].status != PLAYER_STATUS.OFFLINE)
                            {
                                Server.Instance.PrintOnDebug("A player is offline " + entry.ip + " " + entry.port + " " + entry.id);
                                players.list[entry.id].status = PLAYER_STATUS.OFFLINE;
                                Server.Instance.WriteToOther("052", entry.ip, entry.port, entry.id.ToString());
                            }
                            break;
                        }
                    }
                }
            }
        }

        /**
         *  Send a string to a all of the client.
         *  @param  type    The type of the event.
         *  @param  msg     The message to send to the clients.
         */
        public void WriteToAll(string type, string msg)
        {
            PrintOnDebug("<ALL>");
            foreach (var entry in players.list)
                WriteTo(type, entry.ip, entry.port, msg);
            PrintOnDebug("</ALL>");
        }

        /**
        *  Send a string to a all of the clients except one.
        *  @param  type    The type of the event.
        *  @param  ip      The ip of the client to ignore.
        *  @param  port    The port of the client to ignore.
        *  @param  msg     The message to send to the clients.
        */
        public void WriteToOther(string type, string ip, int port, string msg)
        {
            PrintOnDebug("<OTHER>");
            foreach (var entry in players.list)
            {
                if (entry.ip != ip || entry.port != port)
                    WriteTo(type, entry.ip, entry.port, msg);
            }
            PrintOnDebug("</OTHER>");
        }

        /**
         *  Check if all the players are ONLINE
         *  @return Return False if all the players are offline
         */
        public bool    CheckClose()
        {
            if (players.list.Count != 4)
                return (true);
            foreach (var it in players.list)
            {
                if (it.status == PLAYER_STATUS.ONLINE)
                    return (true);
            }
            return (false);
        }

        /**
         *  Play one turn of the game according to the game state.
         */
        public void Run()
        {
            switch (game.status)
            {
                case GAME_STATUS.WAIT:
                    {
                        game.Wait();
                        break;
                    }
                case GAME_STATUS.DISTRIB:
                    {
                        game.Distrib();
                        break;
                    }
                case GAME_STATUS.ANNONCE:
                    {
                        game.Annonce();
                        break;
                    }
                case GAME_STATUS.TURN:
                    {
                        game.Turn();
                        break;
                    }
                case GAME_STATUS.REFEREE:
                    {
                        game.Referee();
                        break;
                    }
                case GAME_STATUS.END:
                    {
                        game.End();
                        break;
                    }
            }
        }
    }
}
