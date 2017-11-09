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
                win.ContentArea.Content = new WaitingScreenContent();
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
            //MessageBox.Show(message, "Id: " + GameInfos.Instance.MyId);
            GameInfos.Instance.AddPlayer(int.Parse(message.Split(':')[0]), message.Split(':')[1], false);

            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                MainWindow.Instance.ChangeButton(message);
                if (GameInfos.Instance.UsersList.Count == 4)
                {
                    GameWindow win = new GameWindow();
                    App.Current.MainWindow.Close();
                    App.Current.MainWindow = win;
                    win.Initialize();
                    win.Show();
                }

                //Button b1 = content.FindName("Player0Button") as Button;
                //Button b2 = content.FindName("Player1Button") as Button;
                //Button b3 = content.FindName("Player2Button") as Button;
                //Button b4 = content.FindName("Player3Button") as Button;

                //switch (GameInfos.Instance.UsersList.Count)
                //{
                //    case 2:
                //        b2.BorderBrush = new BrushConverter().ConvertFrom("#FF1AD411") as Brush;
                //        b2.Content = "Ready";
                //        Thread.Sleep(1000);
                //        break;
                //    case 3:
                //        b2.BorderBrush = new BrushConverter().ConvertFrom("#FF1AD411") as Brush;
                //        b2.Content = "Ready";
                //        b3.BorderBrush = new BrushConverter().ConvertFrom("#FF1AD411") as Brush;
                //        b3.Content = "Ready";
                //        Thread.Sleep(1000);
                //        break;
                //    case 4:
                //        GameWindow win = new GameWindow();
                //        App.Current.MainWindow.Close();
                //        App.Current.MainWindow = win;
                //        win.Initialize();
                //        win.Show();
                //        break;
                //}
            }));
            Console.WriteLine("A player connect : " + message);
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