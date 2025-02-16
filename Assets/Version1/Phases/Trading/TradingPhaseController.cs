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
        
        
        
        private void Awake()
        {
            timer.Init(300);
            market.Init();
            
        }

        private void StartPhase()
        {
            timer.StartTimer();
            
        }

        private void StopPhase()
        {
            timer.StopTimer();
        }
    }
}