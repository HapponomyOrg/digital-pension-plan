using System;
using UnityEngine;
using Version1.Market.Scripts.UI;
using Version1.Nats.Messages.Host;
using Version1.Utilities;

namespace Version1.Phases.Trading
{
    public class TradingPhaseController : MonoBehaviour
    {
        [SerializeField] private Timer timer;
        [SerializeField] private MarketUIManager market;
        [SerializeField] private CardBar cardBar;
        [SerializeField] private TopBar topBar;

        private bool started;
        
        private void Start()
        {
            var gm = Utilities.GameManager.Instance;
            
            NetworkManager.Instance.WebSocketClient.OnStopRound += StopPhase;
            
            timer.Init(300);
            market.Init();
            cardBar.Init();
            topBar.Init();
            
            StartPhase();
        }

        private void StartPhase()
        {
            timer.StartTimer();
            market.OpenMarket();
        }

        private void StopPhase(object sender, StopRoundMessage message)
        {
            timer.StopTimer();
            market.CloseMarket();
        }
    }
}