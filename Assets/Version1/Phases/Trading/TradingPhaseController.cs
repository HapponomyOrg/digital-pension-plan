using System;
using Assets.Version1.Phases;
using UnityEngine;
using Version1.Market;
using Version1.Nats.Messages.Host;
using Version1.Utilities;

namespace Version1.Phases.Trading
{
    public class TradingPhaseController : MonoBehaviour, IPhaseController
    {
        [SerializeField] private Timer timer;
        [SerializeField] private CardBar cardBar;
        [SerializeField] private TopBar topBar;
        [SerializeField] private MarketView marketView;

        private void Start()
        {
            timer.Init(300);
            cardBar.Init();
            topBar.Init();
            
            marketView.InitializeData();

            // Start the phase
            Utilities.GameManager.Instance.PhaseManager.CurrentPhaseController = this;
        }

        public void StartPhase()
        {
            timer.StartTimer();
            marketView.OpenMarket();
        }

        public void StopPhase()
        {
            timer.StopTimer();
            marketView.CloseMarket();
        }

        public void OnDestroy()
        {
            Utilities.GameManager.Instance.PhaseManager.CurrentPhaseController = null;
        }
    }
}
