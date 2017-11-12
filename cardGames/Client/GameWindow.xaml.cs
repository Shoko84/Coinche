using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Game;

namespace Client
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private ContractCallContent contractCallCont;
        private CoincheCallContent coincheCallWidget;
        private IngameCallContent ingameCallCont;

        public ContractCallContent ContractCallCont { get => contractCallCont; }
        public CoincheCallContent CoincheCallWidget { get => coincheCallWidget; }
        public IngameCallContent IngameCallCont { get => ingameCallCont; }

        private static GameWindow instance;
        public static GameWindow Instance { get => instance; }

        private void DisplayRectOnCanvas(double x, double y, double width, double height, Color color)
        {
            Rectangle rect = new System.Windows.Shapes.Rectangle
            {
                Fill = new SolidColorBrush(color),
                Width = width,
                Height = height
            };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            this.TestCanvas.Children.Add(rect);
        }

        private void DrawGameField()
        {
            TestCanvas.Children.Clear();
            Image img = new Image();
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, "..", "..", "ressources", "assets", "background", "background.jpg");
            Uri uri = new Uri(path);
            BitmapImage bmp = new BitmapImage(uri);

            img.Source = bmp;
            img.Width = bmp.Width * 3.27f + 1;
            img.Height = bmp.Height * 3.27f;
            img.Stretch = Stretch.Fill;
            Canvas.SetLeft(img, 0);
            Canvas.SetTop(img, 0);
            TestCanvas.Children.Add(img);
        }

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

        private void DrawNameAndScore()
        {
            for (int i = 0; i < GameInfos.Instance.UsersList.Count; i++)
            {
                Label text = new Label
                {
                    Foreground = Brushes.White,
                    Content = GameInfos.Instance.UsersList[i].Username + ": " + GameInfos.Instance.UsersList[i].Score.ToString()
                };
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

        private void DrawContractPicked()
        {
            Contract contract = GameInfos.Instance.contractPicked;

            if (contract != null)
            {
                Label text = new Label
                {
                    Foreground = Brushes.White,
                    Content = contract.StringType + "\n" + contract.score + " points\nPicked by: " + GameInfos.Instance.GetClientUserById(contract.id).Username
                };
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Canvas.SetLeft(text, TestCanvas.Width - text.DesiredSize.Width - 15);
                Canvas.SetTop(text, 15);
                TestCanvas.Children.Add(text);
            }
        }

        public void DrawCanvas()
        {
            DrawGameField();
            DrawHandCards(GameInfos.Instance.UsersList);
            DrawCardsPlayed(GameInfos.Instance.CardsPlayed);
            DrawNameAndScore();
            DrawWhosPlaying();
            DrawContractPicked();
        }

        public void Initialize()
        {
            DrawCanvas();
            GameInfos.Instance.NetManager.WriteMessage("100", "");
        }

        public GameWindow()
        {
            instance = this;
            contractCallCont = new ContractCallContent();
            coincheCallWidget = new CoincheCallContent();
            ingameCallCont = new IngameCallContent();

            InitializeComponent();
            ContentArea.Content = contractCallCont;
            Title = GameInfos.Instance.GetClientUserById(GameInfos.Instance.MyId).Username + ":" + GameInfos.Instance.MyId;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GameInfos.Instance.NetManager.WriteMessage("050", "");
        }
    }
}
