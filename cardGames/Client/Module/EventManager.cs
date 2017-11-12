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
using System.Collections.Generic;
using Newtonsoft.Json;
using Game;
using System.Collections.ObjectModel;
using Server;

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
         *  Triggered when the a chat message is received.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     The username and the message as username:message.
         */
        public void PrintIncomingMessage(PacketHeader header, Connection connection, string message)
        {
            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                string[] args = message.Split(':');

                if (GameWindow.Instance.ChatData.Text != "")
                    GameWindow.Instance.ChatData.Text += "\n";
                GameWindow.Instance.ChatData.Text += "[" + message.Split(':')[0] + "]: " + message.Split(':')[1];
                GameWindow.Instance.ChatScroller.ScrollToBottom();
            }));
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
         *  @param   message     The id of and the user's name as id:username.
         */
        public void ConnectionOk(PacketHeader header, Connection connection, string data)
        {
            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                string[] dataSplit = data.Split(':');

                GameInfos.Instance.AddPlayer(int.Parse(dataSplit[0]), dataSplit[1], true);
                Button b = MainWindow.Instance.WaitingScreen.FindName("Player" + dataSplit[0] + "Button") as Button;
                b.BorderBrush = new BrushConverter().ConvertFrom("#FF1AD411") as Brush;
                b.Content = dataSplit[1];
                GameInfos.Instance.MyId = int.Parse(data.Split(':')[0]);
                GameInfos.Instance.NetManager.IsConnected = true;
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

        }

        /**
         *  Triggered when the server is gonna start the game or is actually playing.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     Unused.
         */
        public void Playing(PacketHeader header, Connection connection, string message)
        {
            GameInfos.Instance.NetManager.WriteMessage("111", "");
            for (int i = 0; i < GameInfos.Instance.UsersList.Count; i++)
            {
                if (GameInfos.Instance.UsersList[i].Id != GameInfos.Instance.MyId)
                    GameInfos.Instance.NetManager.WriteMessage("113", GameInfos.Instance.UsersList[i].Id.ToString());
            }
        }

        /**
         *  Triggered when the it's the announce turn of the client
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     A client id.
         */
        public void PlayerAnnounce(PacketHeader header, Connection connection, string message)
        {
            if (GameInfos.Instance.GameStatus < GAME_STATUS.ANNONCE)
                GameInfos.Instance.GameStatus = GAME_STATUS.ANNONCE;
            if (GameInfos.Instance.GameStatus == GAME_STATUS.ANNONCE)
            {
                App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
                {
                    GameWindow.Instance.ContentArea.Content = GameWindow.Instance.ContractCallCont;
                    ContractCallContent content = GameWindow.Instance.ContractCallCont;
                    int userId = int.Parse(message);

                    if (userId == GameInfos.Instance.MyId)
                    {
                        content.ContractBox.IsEnabled = true;
                        content.ContractValue.IsEnabled = true;
                        content.ContractCallButton.IsEnabled = true;
                    }
                    else
                    {
                        content.ContractBox.IsEnabled = false;
                        content.ContractValue.IsEnabled = false;
                        content.ContractCallButton.IsEnabled = false;
                    }
                    foreach (var user in GameInfos.Instance.UsersList)
                    {
                        if (user.Id == userId)
                            user.IsPlaying = true;
                        else
                            user.IsPlaying = false;
                    }
                    if (GameWindow.Instance.GameLogger.Text != "")
                        GameWindow.Instance.GameLogger.Text += "\n\n";
                    GameWindow.Instance.GameLogger.Text += "[" + DateTime.Now.ToShortTimeString().ToString() + "] " +
                                                           GameInfos.Instance.GetClientUserById(userId).Username +
                                                           " is now announcing";
                    GameWindow.Instance.GameLoggerScroller.ScrollToBottom();
                    GameWindow.Instance.DrawCanvas();
                }));
            }
        }

        /**
         *  Triggered when the it's the play turn of the client
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     A client id.
         */
        public void PlayerPlay(PacketHeader header, Connection connection, string message)
        {
            if (GameInfos.Instance.GameStatus < GAME_STATUS.TURN)
                GameInfos.Instance.GameStatus = GAME_STATUS.TURN;
            if (GameInfos.Instance.GameStatus == GAME_STATUS.TURN)
            {
                App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
                {
                    GameWindow.Instance.ContentArea.Content = GameWindow.Instance.IngameCallCont;
                    IngameCallContent content = GameWindow.Instance.IngameCallCont;
                    int userId = int.Parse(message);

                    if (userId == GameInfos.Instance.MyId)
                    {
                        content.CardsListBox.IsEnabled = true;
                        content.PickCardButton.IsEnabled = true;
                    }
                    else
                    {
                        content.CardsListBox.IsEnabled = false;
                        content.PickCardButton.IsEnabled = false;
                    }
                    foreach (var user in GameInfos.Instance.UsersList)
                    {
                        if (user.Id == userId)
                            user.IsPlaying = true;
                        else
                            user.IsPlaying = false;
                    }
                    if (GameWindow.Instance.GameLogger.Text != "")
                        GameWindow.Instance.GameLogger.Text += "\n\n";
                    GameWindow.Instance.GameLogger.Text += "[" + DateTime.Now.ToShortTimeString().ToString() + "] " +
                                                           GameInfos.Instance.GetClientUserById(userId).Username +
                                                           " is now playing";
                    GameWindow.Instance.GameLoggerScroller.ScrollToBottom();
                    GameWindow.Instance.DrawCanvas();
                }));
            }
        }

        /**
         *  Triggered when someone announced
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     A contract.
         */
        public void SomeoneHasAnnounced(PacketHeader header, Connection connection, string message)
        {
            Contract contract = JsonConvert.DeserializeObject<Contract>(message);
            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                if (contract.type != CONTRACT_TYPE.PASS)
                {
                    ContractCallContent content = GameWindow.Instance.ContractCallCont;

                    if (contract.score < 160)
                    {
                        content.ContractValue.Minimum = contract.score + 10;
                        content.ContractValue.Value = contract.score + 10;
                    }
                    else
                    {
                        content.ContractValue.Minimum = 160;
                        content.ContractValue.Value = 160;
                    }
                    if (GameInfos.Instance.ContractPicked == null || GameInfos.Instance.ContractPicked.score < contract.score)
                        GameInfos.Instance.ContractPicked = new Contract(contract.score, contract.type, contract.id);
                    if (GameWindow.Instance.GameLogger.Text != "")
                        GameWindow.Instance.GameLogger.Text += "\n\n";
                    GameWindow.Instance.GameLogger.Text += "[" + DateTime.Now.ToShortTimeString().ToString() + "] " +
                                                           GameInfos.Instance.GetClientUserById(contract.id).Username +
                                                           " announced a contract with a value of " + contract.score.ToString() +
                                                           " and a '" + contract.StringType + "' rule";
                    GameWindow.Instance.DrawCanvas();
                }
                else
                {
                    if (GameWindow.Instance.GameLogger.Text != "")
                        GameWindow.Instance.GameLogger.Text += "\n\n";
                    GameWindow.Instance.GameLogger.Text += "[" + DateTime.Now.ToShortTimeString().ToString() + "] " +
                                                           GameInfos.Instance.GetClientUserById(contract.id).Username +
                                                           " passed";
                }
                GameWindow.Instance.GameLoggerScroller.ScrollToBottom();
            }));
        }

        /**
         *  Triggered when someone played
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     A card.
         */
        public void SomeonePlayedACard(PacketHeader header, Connection connection, string message)
        {
            Card card = JsonConvert.DeserializeObject<Card>(message);
            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                if (GameWindow.Instance.GameLogger.Text != "")
                    GameWindow.Instance.GameLogger.Text += "\n\n";
                GameWindow.Instance.GameLogger.Text += "[" + DateTime.Now.ToShortTimeString().ToString() + "] " +
                                                       card.StringValue + " " + card.StringColour + " was played";
                GameWindow.Instance.GameLoggerScroller.ScrollToBottom();
            }));
        }

        /**
         *  Triggered when a new client is connected to the server.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     A list of connected users with each item in format id:name.
         */
        public void PlayersConnect(PacketHeader header, Connection connection, string message)
        {
            List<string> connectedUsers = JsonConvert.DeserializeObject<List<string>>(message);

            foreach (var item in connectedUsers)
            {
                if (GameInfos.Instance.AddPlayer(int.Parse(item.Split(':')[0]), item.Split(':')[1], false))
                {
                    App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
                    {
                        WaitingScreenContent content = MainWindow.Instance.WaitingScreen;

                        Button b = content.FindName("Player" + item.Split(':')[0] + "Button") as Button;
                        b.BorderBrush = new BrushConverter().ConvertFrom("#FF1AD411") as Brush;
                        b.Content = item.Split(':')[1];
                        if (GameInfos.Instance.UsersList.Count == 4 && !(App.Current.MainWindow is GameWindow))
                        {
                            GameWindow win = new GameWindow();
                            App.Current.MainWindow.Close();
                            App.Current.MainWindow = win;
                            win.Initialize();
                            win.Show();
                        }
                    }));
                    Console.WriteLine("A player connect : " + item);
                }
            }
        }

        /**
         *  Triggered when a client disconnect himself from the server.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     The client id.
         */
        public void PlayersQuit(PacketHeader header, Connection connection, string message)
        {
            int userId = int.Parse(message);

            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                if (GameWindow.Instance.GameLogger.Text != "")
                    GameWindow.Instance.GameLogger.Text += "\n\n";
                GameWindow.Instance.GameLogger.Text += "[" + DateTime.Now.ToShortTimeString().ToString() + "] " +
                                                       GameInfos.Instance.GetClientUserById(userId).Username +
                                                       " disconnected";
                GameWindow.Instance.GameLoggerScroller.ScrollToBottom();
            }));
        }

        /**
        *  Triggered when a client change his name.
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     The id, the old name and the new name of the client in format id|old|new.
        */
        public void PlayerRename(PacketHeader header, Connection connection, string message)
        {
            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                if (GameWindow.Instance.GameLogger.Text != "")
                    GameWindow.Instance.GameLogger.Text += "\n\n";
                GameWindow.Instance.GameLogger.Text += "[" + DateTime.Now.ToShortTimeString().ToString() + "] " + "Player " + message.Split('|')[0] + " renamed " + message.Split('|')[1] + " to " + message.Split('|')[2];
                GameWindow.Instance.GameLoggerScroller.ScrollToBottom();
            }));
        }

        /**
        *  Triggered when a client has lost
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     Unused
        */
        public void PlayerHasLost(PacketHeader header, Connection connection, string message)
        {
            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                MessageBoxResult result;
                if ((result = MessageBox.Show("You lost..\nDo you want to restart a game ?", "Game over", MessageBoxButton.YesNo, MessageBoxImage.Question)) == MessageBoxResult.Yes)
                {
                    GameInfos.Instance.NetManager.WriteMessage("022", "");
                    GameInfos.Instance.RestartGameInfos();
                    GameWindow win = new GameWindow();
                    App.Current.MainWindow.Close();
                    App.Current.MainWindow = win;
                    win.Initialize();
                    win.Show();
                }
                else if (result == MessageBoxResult.No)
                {
                    GameInfos.Instance.NetManager.WriteMessage("050", "");
                    Application.Current.Shutdown();
                }
            }));
        }

        /**
        *  Triggered when a client has won
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     Unused
        */
        public void PlayerHasWon(PacketHeader header, Connection connection, string message)
        {
            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                MessageBoxResult result;
                if ((result = MessageBox.Show("You won!!\nDo you want to restart a game ?", "Win", MessageBoxButton.YesNo, MessageBoxImage.Question)) == MessageBoxResult.Yes)
                {
                    GameInfos.Instance.NetManager.WriteMessage("022", "");
                    GameInfos.Instance.RestartGameInfos();
                    GameWindow win = new GameWindow();
                    App.Current.MainWindow.Close();
                    App.Current.MainWindow = win;
                    win.Initialize();
                    win.Show();
                }
                else if (result == MessageBoxResult.No)
                {
                    GameInfos.Instance.NetManager.WriteMessage("050", "");
                    Application.Current.Shutdown();
                }
            }));
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
                        GameWindow.Instance.DrawCanvas();
                        break;
                    }
                }
            }));
        }

        /**
         *  Triggered when a client receive the player's score.
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     The id of the player and the score as id:score.
         */
        public void GetPlayerScore(PacketHeader header, Connection connection, string message)
        {
            string[] args = message.Split(':');

            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                ClientUser user = GameInfos.Instance.GetClientUserById(int.Parse(args[0]));
                user.Score = int.Parse(args[1]);
                if (GameWindow.Instance.GameLogger.Text != "")
                    GameWindow.Instance.GameLogger.Text += "\n\n";
                GameWindow.Instance.GameLogger.Text += "[" + DateTime.Now.ToShortTimeString().ToString() + "] " +
                                                       GameInfos.Instance.GetClientUserById(int.Parse(args[0])).Username +
                                                       " has a score of " + args[1];
                GameWindow.Instance.GameLoggerScroller.ScrollToBottom();
                GameWindow.Instance.DrawCanvas();
            }));
        }

        /**
        *  Triggered when a client receive his deck.
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     A Deck.
        */
        public void PlayerReceiveDeck(PacketHeader header, Connection connection, string message)
        {
            Deck deck = JsonConvert.DeserializeObject<Deck>(message);
            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                ObservableCollection<string> cardsNames = new ObservableCollection<string>();

                GameInfos.Instance.GetClientUserById(GameInfos.Instance.MyId).CardsList = deck;
                foreach (var card in deck.cards)
                    cardsNames.Add(card.StringValue + " " + card.StringColour);
                GameWindow.Instance.DrawCanvas();
                GameWindow.Instance.IngameCallCont.CardsListBox.ItemsSource = cardsNames;
                GameWindow.Instance.IngameCallCont.CardsListBox.SelectedIndex = 0;
            }));
        }

        /**
         *  Triggered when a client receive the current pile
         *  @param   header      Infos about the header.
         *  @param   connection  Infos about the server's connection.
         *  @param   message     A Pile.
         */
        public void PlayerReceivePile(PacketHeader header, Connection connection, string message)
        {
            Pile pile = JsonConvert.DeserializeObject<Pile>(message);
            App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(delegate ()
            {
                GameInfos.Instance.CardsPlayed.Clear();
                for (int i = 0; i < pile.cards.Count; i++)
                    GameInfos.Instance.CardsPlayed.AddCard(pile.cards.cards[i].colour, pile.cards.cards[i].value, (Card.CardPosition)GameInfos.Instance.GetPosFromId(GameInfos.Instance.MyId, pile.owners[i]));
                if (pile.cards.cards.Count == 4)
                    GameInfos.Instance.LastPile = pile;
                GameWindow.Instance.DrawCanvas();
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

        }

        /**
        *  Triggered when the server tells the client it's not his turn
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     An user id.
        */
        public void NotMyTurn(PacketHeader header, Connection connection, string message)
        {
            MessageBox.Show("It's not your turn, it's player " + message + "'s turn", "Incorrect turn", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /**
        *  Triggered when the server tells the client he's not owning the played card
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     Unused.
        */
        public void NotOwningCard(PacketHeader header, Connection connection, string message)
        {
            MessageBox.Show("You don't own this card.", "Incorrect play", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /**
        *  Triggered when the server tells the client he told a lower contract value
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     Unused.
        */
        public void AnnounceIncorrect(PacketHeader header, Connection connection, string message)
        {
            MessageBox.Show("You can't announce a lower contract value", "Incorrect announce", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /**
        *  Triggered when the server tells the client he's not allowed to play the card
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     Unused.
        */
        public void CardNotAllowed(PacketHeader header, Connection connection, string message)
        {
            MessageBox.Show("You can't play this card.", "Incorrect play", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /**
        *  Triggered when the server tells the client it's not anymore a announce turn
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     Unused.
        */
        public void AnnounceNotAllowed(PacketHeader header, Connection connection, string message)
        {
            MessageBox.Show("It's not an announce turn", "Incorrect call", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /**
        *  Triggered when the server tells the client it's an incorrect turn type
        *  @param   header      Infos about the header.
        *  @param   connection  Infos about the server's connection.
        *  @param   message     Unused.
        */
        public void IncorrectTurnType(PacketHeader header, Connection connection, string message)
        {
            MessageBox.Show("Incorrect turn type", "Incorrect call", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}