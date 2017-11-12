/**
 * @file    ContractCallContent.xaml.cs
 * @author  Maxime Cauvin
 * 
 * This file contains the ContractCallContent Class.
 */

using Game;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for ContractCallContent.xaml
    /// </summary>
    public partial class ContractCallContent : UserControl
    {
        /**
         *  ContractCallContent's constructor - Create an ContractCallContent instance
         */
        public ContractCallContent()
        {
            InitializeComponent();
        }

        /**
         *  ContractCallButton Click Event trigger
         *  @param  sender     The component that triggered the event
         *  @param  e          Object that contains the state information and data about the event triggered.
         */
        private void ContractCallButton_Click(object sender, RoutedEventArgs e)
        {
            Contract contract = new Contract((int)ContractValue.Value, (CONTRACT_TYPE)ContractBox.SelectedIndex, GameInfos.Instance.MyId);
            if (contract.type == CONTRACT_TYPE.PASS)
                contract.score = 0;
            GameInfos.Instance.NetManager.WriteMessage("120", JsonConvert.SerializeObject(contract));
            ContractBox.SelectedIndex = 0;
        }
    }
}
