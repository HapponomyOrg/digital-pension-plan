using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Version1.Market;
using Version1.Nats.Messages.Host;
using Version1.Phases;
using Version1.Phases.BalanceModification;
using Version1.Phases.Interest;
using Version1.Phases.Trading;
using NatsHost = Version1.Nats.NatsHost;


namespace Version1.Utilities
{
    public class GameManager
    {
        public const string LOADING = "Loading";
        
        private static GameManager instance;
        
        public static GameManager Instance => instance ??= new GameManager();

        public Cards.Scripts.CardLibrary CardLibrary { get; }
        
        public IListingRepository ListingRepository { get; }

        public MarketServices MarketServices { get; }

        private readonly string[] debtBasedPhases =
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
        
        private readonly string[] sustainableMoneyPhases =
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
            "MoneyCorrectionScene",
            "Loading",
            "MarketScene",
            "MoneyCorrectionScene",
            "Loading",
            "MarketScene",
            "MoneyCorrectionScene",
            "Loading",
            "MoneyToPointScene",
            "DonatePointsScene",
            "EndScene"
        };

        private string[] phases;

        public int CurrentPhase { get; private set; }

        private GameManager()
        {
            CardLibrary = Resources.Load<Cards.Scripts.CardLibrary>("CardList");
            CardLibrary.FillCardList();
            phases = testPhases;

            ListingRepository = new ListingRepository();
            MarketServices = new MarketServices();
        }
        
        public void StartGame()
        {
            LoadPhase(0, phases[0]);
        }

        public void LoadPhase(int phase, string  name)
        {
            if (phases[phase] != name)
            {
                //TODO this is maybe not how we want it but this is a way to keep players in sync
                Debug.LogWarning($"Phase: {phases[phase]} is not the same as received {name}");
                SceneManager.LoadScene(name);
                return;
            }

            SceneManager.LoadScene(phases[phase]);
        }
        
        private void LoadNextPhase()
        {
            SceneManager.LoadScene(phases[++CurrentPhase]);
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
