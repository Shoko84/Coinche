/**
 * @file    EventManager.cs
 * @author  Marc-Antoine Leconte
 * 
 * This file contains the EventManager Class.
 */

using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using Common;
using System.Collections.Generic;
using Newtonsoft.Json;
using Game;
using System.Linq;

namespace Client
{
    /**
     *  This class regroup all the functions wich will be triggered by the server's responses.
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
         *  Triggered when the server answer to the client.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     The message.
         */
        public void PrintIncomingMessage(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine("\nA message was received from " + connection.ToString() + " which said '" + message + "'.");
        }

        /**
         *  Triggered when the server refuse the connection.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     Unused.
         */
        public void ConnectionKo(PacketHeader header, Connection connection, string message)
        {
            MessageBox.Show(message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Warning);
            GameInfos.Instance.NetManager.IsConnected = false;
        }

        /**
         *  Triggered when the server accept the connection.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     The id of the client.
         */
        public void ConnectionOk(PacketHeader header, Connection connection, string data)
        {
            Console.WriteLine("Connected:", data);
            GameInfos.Instance.NetManager.IsConnected = true;
            GameInfos.Instance.MyId = int.Parse(data.Split(':')[0]);
            GameInfos.Instance.AddPlayer(GameInfos.Instance.MyId, data.Split(':')[1], true);
            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                MainWindow win = MainWindow.Instance;
                win.ContentArea.Content = MainWindow.Instance.WaitingScreen;
            }));
        }

        /**
         *  Triggered when the server is waiting for players.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     Unused.
         */
        public void WaitingForPlayer(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine("Waiting for players");
        }

        /**
         *  Triggered when the server is gonna start the game or is actually playing.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     Unused.
         */
        public void Playing(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine("Let's play !!!");
        }

        /**
         *  Triggered when a new client is connected to the server.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     The id of the new player and the name of the player in format id:name.
         */
        public void PlayersConnect(PacketHeader header, Connection connection, string message)
        {
            List<string> connectedUsers = JsonConvert.DeserializeObject<List<string>>(message);

            for (int i = 0; i < connectedUsers.Count; i++)
            {
                if (GameInfos.Instance.AddPlayer(int.Parse(connectedUsers[i].Split(':')[0]), connectedUsers[i].Split(':')[1], false))
                {
                    //MessageBox.Show(message, GameInfos.Instance.MyId.ToString());
                    App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
                    {
                        WaitingScreenContent content = MainWindow.Instance.WaitingScreen;

                        Button b = content.FindName("Player" + connectedUsers[i].Split(':')[0] + "Button") as Button;
                        b.BorderBrush = new BrushConverter().ConvertFrom("#FF1AD411") as Brush;
                        b.Content = connectedUsers[i].Split(':')[1];
                        if (GameInfos.Instance.UsersList.Count == 4)
                        {
                            GameWindow win = new GameWindow();
                            App.Current.MainWindow.Close();
                            App.Current.MainWindow = win;
                            win.Initialize();
                            win.Show();
                        }
                    }));
                    Console.WriteLine("A player connect : " + connectedUsers[i]);
                }
            }
        }

        /**
         *  Triggered when a client disconnect himself from the server.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     The id of the client id.
         */
        public void PlayersQuit(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine("Player " + message);
        }

        /**
        *  Triggered when a client change his name.
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     The id, the old name and the new name of the client in format id|old|new.
        */
        public void PlayerRename(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine("Player " + message.Split('|')[0] + " renamed " + message.Split('|')[1] + " to " + message.Split('|')[2]);
        }

        /**
        *  Triggered when a client receive the cards number from someone.
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     The id of the player and the number of cards as id:nbCard.
        */
        public void GetPlayerCardsNumber(PacketHeader header, Connection connection, string message)
        {
            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                for (int i = 0; i < GameInfos.Instance.UsersList.Count; i++)
                {
                    if (GameInfos.Instance.UsersList[i].Id == int.Parse(message.Split(':')[0]))
                    {
                        int nbCard = int.Parse(message.Split(':')[1]);
                        GameInfos.Instance.UsersList[i].CardsList.Clear();
                        for (int j = 0; j < nbCard; j++)
                            GameInfos.Instance.UsersList[i].CardsList.AddCard(Card.CardColour.Unknown, Card.CardValue.Unknown, (Card.CardPosition)GameInfos.Instance.GetPosFromId(GameInfos.Instance.MyId, GameInfos.Instance.UsersList[i].Id));
                        GameWindow.Instance.DrawGameField();
                        GameWindow.Instance.DrawHandCards(GameInfos.Instance.UsersList);
                        GameWindow.Instance.DrawCardsPlayed(GameInfos.Instance.CardsPlayed);
                        break;
                    }
                }
            }));
        }

        /**
        *  Triggered when a client receive his deck.
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     The id, the old name and the new name of the client in format id|old|new.
        */
        public void PlayerReceiveDeck(PacketHeader header, Connection connection, string message)
        {
            Deck deck = JsonConvert.DeserializeObject<Deck>(message);

            GameInfos.Instance.GetClientUserById(GameInfos.Instance.MyId).CardsList = deck;

            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                GameWindow.Instance.DrawGameField();
                GameWindow.Instance.DrawHandCards(GameInfos.Instance.UsersList);
                GameWindow.Instance.DrawCardsPlayed(GameInfos.Instance.CardsPlayed);
            }));
        }

        /**
        *  Triggered when the server agree with one of the client request
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     Unused.
        */
        public void Ok(PacketHeader header, Connection connection, string message)
        {
            Console.WriteLine("OK");
        }
    }
}