using System;
using UnityEngine;
using Version1.Market.Scripts.UI;

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
            timer.Init(300);
            market.Init();
            cardBar.Init();
            topBar.Init();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.P) && !started)
            {
                started = true;
                StartPhase();
            }

            if (Input.GetKeyUp(KeyCode.S) && started)
            {
                started = false;
                StopPhase();
            }
        }

        private void StartPhase()
        {
            timer.StartTimer();
            market.OpenMarket();
        }

        private void StopPhase()
        {
            timer.StopTimer();
            market.CloseMarket();
        }
    }
}