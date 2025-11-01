using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Version1.Market;
using Object = UnityEngine.Object;

namespace Version1.Phases.Trading
{
    public class TradingPhase : Phase
    {
        private const string tradingScene = "TradingScene";
        public override event EventHandler<EventArgs> InitFinished;
        public override bool InitComplete { get; protected set; }
        
        
        private Timer timer;
        //private MarketUIManager market;
        
        
        public override void Init()
        {
            SceneManager.LoadScene(tradingScene);
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                // market = Object.FindObjectOfType<MarketUiManager>();
                timer = Object.FindObjectOfType<Timer>();
                
                timer.Init(300);

                //market = Object.FindObjectOfType<MarketUIManager>();

                
                InitComplete = true;
                InitFinished?.Invoke(this, EventArgs.Empty);
                InitFinished = null;
            };
        }

        public override void Start()
        {
            timer.StartTimer();
        }

        public override void End()
        {
            timer.StopTimer();
        }
    }
}
