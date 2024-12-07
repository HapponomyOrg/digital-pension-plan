using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Version1.Phases.Trading
{
    public class TradingPhase : Phase
    {
        private const string tradingScene = "test";

        private Timer timer;
        private MarketUiManager market;
        
        public override event EventHandler<EventArgs> InitFinished;
        public override bool InitComplete { get; protected set; }
        
        public override void Init()
        {
            SceneManager.LoadScene(tradingScene);
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                market = Object.FindObjectOfType<MarketUiManager>();
                timer = Object.FindObjectOfType<Timer>();
                
                timer.Init(300);
                
                InitFinished?.Invoke(this, EventArgs.Empty);
                InitFinished = null;
                InitComplete = true;
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
