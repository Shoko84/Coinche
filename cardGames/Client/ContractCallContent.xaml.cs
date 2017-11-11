﻿using Game;
using Newtonsoft.Json;
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
    /// Interaction logic for ContractCallContent.xaml
    /// </summary>
    public partial class ContractCallContent : UserControl
    {
        public ContractCallContent()
        {
            InitializeComponent();
        }

        private void ContractCallButton_Click(object sender, RoutedEventArgs e)
        {
            Contract contract = new Contract((int)ContractValue.Value, (CONTRACT_TYPE)ContractBox.SelectedIndex, GameInfos.Instance.MyId);
            if (contract.type == CONTRACT_TYPE.PASS)
                contract.score = 0;
            GameInfos.Instance.NetManager.WriteMessage("120", JsonConvert.SerializeObject(contract));
        }
    }
}
