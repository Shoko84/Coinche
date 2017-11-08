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

namespace Client
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private ContractWidget contractWidget;
        private CoincheCallWidget coincheCallWidget;
        private IngameCallWidget ingameCallWidget;

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

        public void DrawGameField()
        {
            this.TestCanvas.Children.Clear();
            //Background
            DisplayRectOnCanvas(0, 0, this.TestCanvas.Width - this.TestCanvas.Margin.Right - this.TestCanvas.Margin.Left,
                                this.TestCanvas.Height - this.TestCanvas.Margin.Top - this.TestCanvas.Margin.Bottom,
                                Colors.ForestGreen);

            //Left side
            DisplayRectOnCanvas(0, this.TestCanvas.Height / 2 - 350 / 2, 80, 350, Colors.Brown);

            //Right side
            DisplayRectOnCanvas(this.TestCanvas.Width - 80,
                                this.TestCanvas.Height / 2 - 350 / 2, 80, 350, Colors.Brown);

            // Top side
            DisplayRectOnCanvas(this.TestCanvas.Width / 2 - 350 / 2, 0, 350, 80, Colors.Brown);

            // Bottom side
            DisplayRectOnCanvas(this.TestCanvas.Width / 2 - 350 / 2,
                                this.TestCanvas.Height - 80, 350, 80, Colors.Brown);
        }

        public void DrawHandCards(List<ClientUser> cardsList)
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
                    int posCard = (int)cardsList[i].CardsList[j].Position;

                    Point spaceTaken;
                    if (posCard % 2 == 0)
                        spaceTaken = new Point(cardsList[i].CardsList.Count * (b.Width * 0.70) - (cardsList[i].CardsList.Count - 1) * ((b.Width * 0.70) - 20), b.Height * 0.70);
                    else
                        spaceTaken = new Point((b.Width * 0.70), cardsList[i].CardsList.Count * (b.Height * 0.70) - (cardsList[i].CardsList.Count - 1) * ((b.Height * 0.70) - 20));

                    Image img = new Image();

                    string path = System.IO.Path.Combine(Environment.CurrentDirectory, "..", "..", "ressources", "assets", "cards", cardsList[i].CardsList[j].StringColour, cardsList[i].CardsList[j].StringValue + ".png");
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

        public void DrawCardsPlayed(List<Card> cardsPlayed)
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

                string path = System.IO.Path.Combine(Environment.CurrentDirectory, "..", "..", "ressources", "assets", "cards", cardsPlayed[i].StringColour, cardsPlayed[i].StringValue + ".png");
                Uri uri = new Uri(path);
                BitmapImage bmp = new BitmapImage(uri);

                img.Source = bmp;
                img.Width = bmp.Width * 0.70;
                img.Height = bmp.Height * 0.70;
                Canvas.SetLeft(img, cardsPos[(int)cardsPlayed[i].Position].X - img.Width / 2);
                Canvas.SetTop(img, cardsPos[(int)cardsPlayed[i].Position].Y - img.Height / 2);
                this.TestCanvas.Children.Add(img);
            }
        }

        public void Initialize()
        {
            DrawGameField();

            BottomActions.Child = contractWidget.ContractGrid;
            BottomActions.Child = coincheCallWidget.CoincheGrid;
            BottomActions.Child = ingameCallWidget.IngameCallGrid;
            
            GameInfos.Instance.AddCardToPlayerId(2, new Card(Card.CardColour.Spades, Card.CardValue.Ace, Card.CardPosition.Bottom));
            GameInfos.Instance.AddCardToPlayerId(2, new Card(Card.CardColour.Hearts, Card.CardValue.Queen, Card.CardPosition.Bottom));
            GameInfos.Instance.AddCardToPlayerId(2, new Card(Card.CardColour.Spades, Card.CardValue.Ten, Card.CardPosition.Bottom));
            GameInfos.Instance.AddCardToPlayerId(2, new Card(Card.CardColour.Diamonds, Card.CardValue.King, Card.CardPosition.Bottom));
            GameInfos.Instance.AddCardToPlayerId(2, new Card(Card.CardColour.Clubs, Card.CardValue.Four, Card.CardPosition.Bottom));
            GameInfos.Instance.AddPlayer(0, "Marco", false);
            GameInfos.Instance.AddCardToPlayerId(0, new Card(Card.CardColour.Unknown, Card.CardValue.Unknown, Card.CardPosition.Top));
            GameInfos.Instance.AddPlayer(1, "Herbaux", false);
            GameInfos.Instance.AddCardToPlayerId(1, new Card(Card.CardColour.Unknown, Card.CardValue.Unknown, Card.CardPosition.Right));
            GameInfos.Instance.AddCardToPlayerId(1, new Card(Card.CardColour.Unknown, Card.CardValue.Unknown, Card.CardPosition.Right));
            GameInfos.Instance.AddCardToPlayerId(1, new Card(Card.CardColour.Unknown, Card.CardValue.Unknown, Card.CardPosition.Right));
            GameInfos.Instance.AddCardToPlayerId(1, new Card(Card.CardColour.Unknown, Card.CardValue.Unknown, Card.CardPosition.Right));
            GameInfos.Instance.AddCardToPlayerId(1, new Card(Card.CardColour.Unknown, Card.CardValue.Unknown, Card.CardPosition.Right));
            GameInfos.Instance.AddCardToPlayerId(1, new Card(Card.CardColour.Unknown, Card.CardValue.Unknown, Card.CardPosition.Right));
            GameInfos.Instance.AddPlayer(3, "Albert", false);
            GameInfos.Instance.AddCardToPlayerId(3, new Card(Card.CardColour.Unknown, Card.CardValue.Unknown, Card.CardPosition.Left));
            GameInfos.Instance.AddCardToPlayerId(3, new Card(Card.CardColour.Unknown, Card.CardValue.Unknown, Card.CardPosition.Left));
            GameInfos.Instance.AddCardToPlayerId(3, new Card(Card.CardColour.Unknown, Card.CardValue.Unknown, Card.CardPosition.Left));
            GameInfos.Instance.AddCardsPlayed(new Card(Card.CardColour.Diamonds, Card.CardValue.Six, Card.CardPosition.Right));
            GameInfos.Instance.AddCardsPlayed(new Card(Card.CardColour.Clubs, Card.CardValue.King, Card.CardPosition.Top));
            GameInfos.Instance.AddCardsPlayed(new Card(Card.CardColour.Hearts, Card.CardValue.Jack, Card.CardPosition.Left));
            GameInfos.Instance.AddCardsPlayed(new Card(Card.CardColour.Spades, Card.CardValue.Nine, Card.CardPosition.Bottom));
        }

        public GameWindow()
        {
            contractWidget = new ContractWidget();
            coincheCallWidget = new CoincheCallWidget();
            ingameCallWidget = new IngameCallWidget();

            InitializeComponent();
        }
    }
}
