using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Version1.Nats.Messages.Host;
using Version1.Phases;
using Version1.Phases.BalanceModification;
using Version1.Phases.Interest;
using Version1.Phases.Trading;
using MarketManager = Version1.Market.Scripts.MarketManager;
using NatsHost = Version1.Nats.NatsHost;


namespace Version1.Utilities
{
    public class GameManager
    {
        private static GameManager instance;
        
        public static GameManager Instance => instance ??= new GameManager();

        public Cards.Scripts.CardLibrary CardLibrary { get; }
        
        public MarketManager MarketManager { get; }
        
        
        private readonly Phase[] debtBasedPhases =
        {
            // Trading
            // Debt payment
            // Take out loan
            // Trading
            // Debt payment
            // Take out loan
            // Trading
            // Debt payment
            // Pension point buying
            // Pension point donation
            // Pension Calculation
        };
        
        private readonly Phase[] sustainableMoneyPhases =
        {
            // Trading
            // Money correction
            // Trading
            // Money correction
            // Trading
            // Money correction
            // Pension point buying
            // Pension point donation
            // Pension Calculation
        };

        private readonly string[] testPhases =
        {
            "MarketScene",
            "Loading",
            "MarketScene",
            "Loading",
            "MarketScene",
            "Loading",
        };

        private string[] phases;

        private int currentPhase;

        private GameManager()
        {
            CardLibrary = Resources.Load<Cards.Scripts.CardLibrary>("CardList");
            CardLibrary.FillCardList();
            phases = testPhases;

            new Nats.NatsClient();
            

            MarketManager = new MarketManager();

            Nats.NatsClient.C.OnStartGame += (sender, message) => { StartGame(); };
            Nats.NatsClient.C.OnStartRound += (sender, message) => { LoadPhase(message.RoundNumber); };
        }
        
        public void StartGame()
        {
            LoadPhase(0);
        }

        private void LoadPhase(int index)
        {
            SceneManager.LoadScene(phases[index]);
        }
        
        private void LoadNextPhase()
        {
            SceneManager.LoadScene(phases[++currentPhase]);
        }

        // public void InitPhase(int p)
        // {
        //     if (p >= phases.Length)
        //         return;
        //     
        //     var phase = phases[p];
        //     currentPhase = p;
        //     
        //     
        //     phase.InitFinished += (sender, e) =>
        //     {
        //         Debug.Log("Init finished");
        //         StartPhase();
        //     };
        //     phase.Init();
        //     
        // }
        //
        // public void StartPhase()
        // {
        //     if (currentPhase >= phases.Length)
        //         return;
        //     
        //     if (phases[currentPhase].InitComplete)
        //         phases[currentPhase].Start();
        //     else
        //         phases[currentPhase].InitFinished += (sender, e) => phases[currentPhase].Start();
        //
        //
        // }
    }
}