/**
 * @file    NetworkManager.cs
 * @author  Marc-Antoine Leconte
 * 
 * This file contains the NetworkManager Class.
 */

using NetworkCommsDotNet;
using System;
using Common;

namespace Client
{
    /**
     *  This Class contains the network part of the client.
     */
    public class NetworkManager
    {
        private string  _serverIP;      /**< This var correspond to the server'ip*/
        private int     _serverPort;    /**< This var correspond to the server's port*/
        private bool    _connect;       /**< This boolean permit to know if the client is connected or not to a server*/
        
        private void Init()
        {
            SetCallBackFunction<string>("msg", GameInfos.Instance.EventManager.PrintIncomingMessage);
            SetCallBackFunction<string>("011", GameInfos.Instance.EventManager.WaitingForPlayer);
            SetCallBackFunction<string>("010", GameInfos.Instance.EventManager.Playing);
            SetCallBackFunction<string>("030", GameInfos.Instance.EventManager.PlayersConnect);
            SetCallBackFunction<string>("031", GameInfos.Instance.EventManager.PlayerRename);
            SetCallBackFunction<string>("052", GameInfos.Instance.EventManager.PlayersQuit);
            SetCallBackFunction<string>("200", GameInfos.Instance.EventManager.Ok);
            SetCallBackFunction<string>("211", GameInfos.Instance.EventManager.PlayerReceiveDeck);
            SetCallBackFunction<string>("230", GameInfos.Instance.EventManager.ConnectionOk);
            SetCallBackFunction<string>("330", GameInfos.Instance.EventManager.ConnectionKo);
        }
       
        /**
         *  Getter of the connection.
         *  @return Return the state of the connection to the server.
         */
        public bool IsConnected { get => _connect; set => _connect = value; }

        /**
         *  Constructor.
         */
        public NetworkManager()
        {
        }

        /**
         *  This function permit to connect the client to a server.
         *  @param  ip      The ip of the server.
         *  @param  port    The port of the server.
         */
        public void Connect(string username, string ip, int port)
        {
            this._serverIP = ip;
            this._serverPort = port;
            this._connect = false;
            this.Init();
            GameInfos.Instance.NetManager.WriteMessage("130", username);
        }

        /**
         *  This function permit to associate an event to a function.
         *  @param  type    The type of the event.
         *  @param  func    The function to associate.
         */
        public void SetCallBackFunction<T>(string type, NetworkComms.PacketHandlerCallBackDelegate<T> func)
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<T>(type, func);
        }

        /**
         *  This function disconnect the client from the server.
         */
        public void Disconnect()
        {
            if (this._connect)
            {
                WriteMessage("050", "bye");
                this._connect = false;
                NetworkComms.Shutdown();
            }
            else
                this.Error("Warning", "The client is not connected to any servers");
        }

        /**
         *  Write an error.
         */
        public void Error(string type, string msg)
        {
            Console.WriteLine("## " + type + ": " + msg);
        }

        /**
         *  This Function allow to send and event to the server.
         *  @param  type    The type of the event.
         *  @param  msg     The main part of the event.
         */
        public void WriteMessage(string type, string msg)
        {
            try
            {
                NetworkComms.SendObject(type, this._serverIP, this._serverPort, msg);
                Console.WriteLine("send[" + type + "]: " + msg);
            }
            catch (Exception)
            {
                this.Error("Error", "The server is disconnected");
                this._connect = false;
            }
        }

        /**
         *  This function permit to send a object to the server throug an event.
         *  @param  type    The type of the event.
         *  @param  obj     The object to send.
         */
        public void WriteObject(string type, object obj)
        {
            if (this._connect)
            {
                string serialize = Serializer.ObjectToString(obj);
                WriteMessage(type, serialize);
            }
            else
                this.Error("Warning", "The client is not connected to any servers");
        }
    }
}
