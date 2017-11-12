/**
 * @file    GameWindow.xaml.cs
 * @author  Maxime Cauvin
 * 
 * This file contains the GameWindow Class.
 */

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Game;

namespace Client
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private ContractCallContent contractCallCont;
        private IngameCallContent ingameCallCont;

        public ContractCallContent ContractCallCont { get => contractCallCont; }
        public IngameCallContent IngameCallCont { get => ingameCallCont; }

        private static GameWindow instance;
        public static GameWindow Instance { get => instance; }

        /**
         *  This function draw the Gamefield background
         */
        private void DrawGameField()
        {
            Image img = new Image();
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "..", "..", "ressources", "assets", "background", "background.jpg");
            Uri uri = new Uri(path);
            BitmapImage bmp = new BitmapImage(uri);

            img.Source = bmp;
            img.Width = bmp.Width * 3.26f + 1;
            img.Height = bmp.Height * 3.27f;
            img.Stretch = Stretch.Fill;
            Canvas.SetLeft(img, 0);
            Canvas.SetTop(img, 0);
            TestCanvas.Children.Add(img);
        }

        /**
         *  This function draw cards in hand
         */
        private void DrawHandCards(List<ClientUser> cardsList)
        {
            Point[] cardsPos = new Point[]
            {
                new Point(this.TestCanvas.Width / 2, this.TestCanvas.Height),
                new Point(0, this.TestCanvas.Height / 2),
                new Point(this.TestCanvas.Width / 2, 0),
                new Point(this.TestCanvas.Width, this.TestCanvas.Height / 2),
            };

            Rotation[] cardsRot = new Rotation[]
            {
                Rotation.Rotate0, Rotation.Rotate90, Rotation.Rotate0, Rotation.Rotate90
            };

            for (int i = 0; i < cardsList.Count; i++)
            {
                string p = System.IO.Path.Combine(Environment.CurrentDirectory, "..", "..", "ressources", "assets", "cards", "Clubs", "A.png");
                Uri u = new Uri(p);
                BitmapImage b = new BitmapImage(u);

                for (int j = 0; j < cardsList[i].CardsList.Count; j++)
                {
                    int posCard = (int)cardsList[i].CardsList.cards[j].position;

                    Point spaceTaken;
                    if (posCard % 2 == 0)
                        spaceTaken = new Point(cardsList[i].CardsList.Count * (b.Width * 0.70) - (cardsList[i].CardsList.Count - 1) * ((b.Width * 0.70) - 20), b.Height * 0.70);
                    else
                        spaceTaken = new Point((b.Width * 0.70), cardsList[i].CardsList.Count * (b.Height * 0.70) - (cardsList[i].CardsList.Count - 1) * ((b.Height * 0.70) - 20));

                    Image img = new Image();

                    string path = System.IO.Path.Combine(Environment.CurrentDirectory, "..", "..", "ressources", "assets", "cards", cardsList[i].CardsList.cards[j].StringColour, cardsList[i].CardsList.cards[j].StringValue + ".png");
                    Uri uri = new Uri(path);
                    BitmapImage bmp = new BitmapImage();

                    bmp.BeginInit();
                    bmp.UriSource = uri;
                    bmp.Rotation = cardsRot[posCard];
                    bmp.EndInit();

                    img.Source = bmp;
                    img.Width = bmp.Width * 0.70;
                    img.Height = bmp.Height * 0.70;

                    switch (posCard)
                    {
                        case 0:
                            Canvas.SetLeft(img, cardsPos[posCard].X + (j * 20) - spaceTaken.X / 2);
                            Canvas.SetTop(img, cardsPos[posCard].Y - img.Height);
                            break;
                        case 1:
                            Canvas.SetLeft(img, cardsPos[posCard].X);
                            Canvas.SetTop(img, cardsPos[posCard].Y + (j * 20) - spaceTaken.Y / 2);
                            break;
                        case 2:
                            Canvas.SetLeft(img, cardsPos[posCard].X + (j * 20) - spaceTaken.X / 2);
                            Canvas.SetTop(img, cardsPos[posCard].Y);
                            break;
                        case 3:
                            Canvas.SetLeft(img, cardsPos[posCard].X - img.Width);
                            Canvas.SetTop(img, cardsPos[posCard].Y + (j * 20) - spaceTaken.Y / 2);
                            break;
                    }

                    this.TestCanvas.Children.Add(img);
                }
            }
        }

        /**
         *  This function draw cards in the current pile
         */
        private void DrawCardsPlayed(Deck cardsPlayed)
        {
            Point[] cardsPos = new Point[]
            {
                new Point(this.TestCanvas.Width / 2, this.TestCanvas.Height / 2 + 40),
                new Point(this.TestCanvas.Width / 2 - 40, this.TestCanvas.Height / 2),
                new Point(this.TestCanvas.Width / 2, this.TestCanvas.Height / 2 - 40),
                new Point(this.TestCanvas.Width / 2 + 40, this.TestCanvas.Height / 2),
            };

            for (int i = 0; i < cardsPlayed.Count; i++)
            {
                Image img = new Image();

                string path = System.IO.Path.Combine(Environment.CurrentDirectory, "..", "..", "ressources", "assets", "cards", cardsPlayed.cards[i].StringColour, cardsPlayed.cards[i].StringValue + ".png");
                Uri uri = new Uri(path);
                BitmapImage bmp = new BitmapImage(uri);

                img.Source = bmp;
                img.Width = bmp.Width * 0.70;
                img.Height = bmp.Height * 0.70;
                Canvas.SetLeft(img, cardsPos[(int)cardsPlayed.cards[i].position].X - img.Width / 2);
                Canvas.SetTop(img, cardsPos[(int)cardsPlayed.cards[i].position].Y - img.Height / 2);
                this.TestCanvas.Children.Add(img);
            }
        }

        /**
         *  This function draw the user's name and score
         */
        private void DrawNameAndScore()
        {
            for (int i = 0; i < GameInfos.Instance.UsersList.Count; i++)
            {
                Label text = new Label
                {
                    Foreground = Brushes.White,
                    Content = GameInfos.Instance.UsersList[i].Username + ": " + GameInfos.Instance.UsersList[i].Score.ToString()
                };
                if ((int)GameInfos.Instance.UsersList[i].Position % 2 == 0)
                    text.Background = new BrushConverter().ConvertFrom("#CC2C5CA2") as Brush;
                else
                    text.Background = new BrushConverter().ConvertFrom("#CCA83939") as Brush;
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Point[] scoresPos = new Point[]
                {
                    new Point(this.TestCanvas.Width / 2 - text.DesiredSize.Width / 2, this.TestCanvas.Height - 130),
                    new Point(100, this.TestCanvas.Height / 2 - text.DesiredSize.Height / 2),
                    new Point(this.TestCanvas.Width / 2 - text.DesiredSize.Width / 2, 100),
                    new Point(this.TestCanvas.Width - 100 - text.DesiredSize.Width, this.TestCanvas.Height / 2 - text.DesiredSize.Height / 2),
                };
                Canvas.SetLeft(text, scoresPos[(int)GameInfos.Instance.UsersList[i].Position].X);
                Canvas.SetTop(text, scoresPos[(int)GameInfos.Instance.UsersList[i].Position].Y);
                TestCanvas.Children.Add(text);
            }
        }

        /**
         *  This function draw an icon to show who is playing
         */
        private void DrawWhosPlaying()
        {
            foreach (var user in GameInfos.Instance.UsersList)
            {
                if (user.IsPlaying)
                {
                    Image img = new Image();
                    string path = System.IO.Path.Combine(Environment.CurrentDirectory, "..", "..", "ressources", "assets", "icons", "playing.png");
                    Uri uri = new Uri(path);
                    BitmapImage bmp = new BitmapImage(uri);
                    img.Source = bmp;
                    img.Width = bmp.Width;
                    img.Height = bmp.Height;

                    Point[] playingPos = new Point[]
                    {
                        new Point(this.TestCanvas.Width / 2 - img.Width / 2, this.TestCanvas.Height - 155),
                        new Point(104, this.TestCanvas.Height / 2 + 15),
                        new Point(this.TestCanvas.Width / 2 - img.Width / 2, 130),
                        new Point(this.TestCanvas.Width - 128, this.TestCanvas.Height / 2 - 40),
                    };

                    Canvas.SetLeft(img, playingPos[(int)user.Position].X);
                    Canvas.SetTop(img, playingPos[(int)user.Position].Y);
                    TestCanvas.Children.Add(img);
                }
            }
        }

        /**
         *  This function draw informations about the picked contract
         */
        private void DrawContractPicked()
        {
            Contract contract = GameInfos.Instance.ContractPicked;

            if (contract != null)
            {
                Label text = new Label
                {
                    Foreground = Brushes.White,
                    Content = contract.StringType + "\n" + contract.score + " points\nPicked by " + GameInfos.Instance.GetClientUserById(contract.id).Username
                };
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Canvas.SetLeft(text, TestCanvas.Width - text.DesiredSize.Width - 15);
                Canvas.SetTop(text, 15);
                TestCanvas.Children.Add(text);
            }
        }

        /**
         *  This function draw the last pile
         */
        private void DrawLastPile()
        {
            Pile pile = GameInfos.Instance.LastPile;

            if (pile != null)
            {
                List<Card> cards = pile.cards.cards;
                for (int i = 0; i < cards.Count; i++)
                {
                    Image img = new Image();

                    string path = System.IO.Path.Combine(Environment.CurrentDirectory, "..", "..", "ressources", "assets", "cards", cards[i].StringColour, cards[i].StringValue + ".png");
                    Uri uri = new Uri(path);
                    BitmapImage bmp = new BitmapImage(uri);

                    img.Source = bmp;
                    img.Width = bmp.Width * 0.55;
                    img.Height = bmp.Height * 0.55;
                    Canvas.SetLeft(img, 18 + (i * img.Width) + (i * 10));
                    Canvas.SetTop(img, 18);
                    TestCanvas.Children.Add(img);
                }
            }
        }

        /**
         *  This function redraw the entire canvas
         */
        public void DrawCanvas()
        {
            DrawGameField();
            DrawHandCards(GameInfos.Instance.UsersList);
            DrawCardsPlayed(GameInfos.Instance.CardsPlayed);
            DrawNameAndScore();
            DrawWhosPlaying();
            DrawContractPicked();
            DrawLastPile();
        }

        /**
         *  This function initialize the class post instantiation
         */
        public void Initialize()
        {
            GameInfos.Instance.NetManager.WriteMessage("100", "");
            Label text = new Label
            {
                Foreground = Brushes.White,
                Content = "Waiting for players...",
                FontSize = 28
            };
            text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(text, TestCanvas.Width / 2 - text.DesiredSize.Width / 2);
            Canvas.SetTop(text, TestCanvas.Height / 2 - text.DesiredSize.Height / 2);
            TestCanvas.Children.Add(text);
        }

        /**
         *  GameWindow's constructor - Create an GameWindow instance
         */
        public GameWindow()
        {
            instance = this;
            contractCallCont = new ContractCallContent();
            ingameCallCont = new IngameCallContent();

            InitializeComponent();
            ContentArea.Content = contractCallCont;
        }

        /**
         *  Window On Close Event trigger
         *  @param  sender     The component that triggered the event
         *  @param  e          Object that contains the state information and data about the event triggered.
         */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GameInfos.Instance.NetManager.WriteMessage("050", "");
        }

        /**
         *  ChatInput On KeyDown Event trigger
         *  @param  sender     The component that triggered the event
         *  @param  e          Object that contains the state information and data about the event triggered.
         */
        private void ChatInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && ChatInput.Text != "")
            {
                GameInfos.Instance.NetManager.WriteMessage("msg", ChatInput.Text);
                ChatInput.Text = "";
            }
        }

        /**
         *  Window On KeyDown Event trigger
         *  @param  sender     The component that triggered the event
         *  @param  e          Object that contains the state information and data about the event triggered.
         */
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
                DrawCanvas();
        }
    }
}
