using System;
using UnityEngine;
using Version1.Market;
using Version1.Nats.Messages.Host;

namespace Version1.Phases.Trading
{
    public class TradingPhaseController : MonoBehaviour
    {
        [SerializeField] private Timer timer;
        //[SerializeField] private MarketUIManager market;
        [SerializeField] private CardBar cardBar;
        [SerializeField] private TopBar topBar;
        [SerializeField] private GameObject overlay;
        [SerializeField] private MarketView marketView;

        private bool started;
        
        private void Start()
        {
            var gm = Utilities.GameManager.Instance;

            Nats.NatsClient.C.OnStopRound += StopPhase;
            
            timer.Init(300);
            //market.Init();
            cardBar.Init();
            topBar.Init();
            
            marketView.InitializeData();

            StartPhase();
        }

        private void StartPhase()
        {
            timer.StartTimer();
            overlay.SetActive(false);
            //market.OpenMarket();
        }

        private void StopPhase(object sender, StopRoundMessage message)
        {
            timer.StopTimer();
            overlay.SetActive(true);


            //market.CloseMarket();
        }
    }
}