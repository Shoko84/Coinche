/**
 * @file    MainWindow.xaml.cs
 * @author  Maxime Cauvin
 * 
 * This file contains the MainWindow Class.
 */

using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow instance; /**< This variable corresponds to the current MainWindow instance*/
        /**
         *  Getter of the MainWindow instance
         */
        public static MainWindow Instance { get => instance; }

        private WaitingScreenContent waitingScreen; /**< This variable corresponds to an loaded WaitingScreenContent instance that will be displayed in the MainWindow*/
        /**
         *  Getter of the waitingScreen instance
         */
        public WaitingScreenContent WaitingScreen { get => waitingScreen; }

        private LoginContent loginScreen; /**< This variable corresponds to an loaded LoginContent instance that will be displayed in the MainWindow*/
        /**
         *  Getter of the loginScreen instance
         */
        public LoginContent LoginScreen { get => loginScreen; }

        /**
         *  MainWindow's constructor - Create an MainWindow instance
         */
        public MainWindow()
        {
            instance = this;
            waitingScreen = new WaitingScreenContent();
            loginScreen = new LoginContent();

            InitializeComponent();
            ContentArea.Content = loginScreen;
        }
    }
}
