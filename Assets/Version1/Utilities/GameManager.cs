using UnityEngine;
using Version1.Phases;
using Version1.Phases.BalanceModification;
using Version1.Phases.Interest;
using Version1.Phases.Trading;
using MarketManager = Version1.Market.Scripts.MarketManager;


namespace Version1.Utilities
{
    public class GameManager
    {
        private static GameManager instance;
        
        public static GameManager Instance => instance ??= new GameManager();

        public Cards.Scripts.CardLibrary CardLibrary { get; }
        
        
        public MarketManager MarketManager { get; }


        private bool isClient = true;
        
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

        private readonly Phase[] testPhases =
        {
            new TradingPhase(),
            new InterestPhase(),
            new BalanceModificationPhase(),
            new TradingPhase()
        };

        private Phase[] phases;

        private int currentPhase;

        private GameManager()
        {
            CardLibrary = Resources.Load<Cards.Scripts.CardLibrary>("CardList");
            CardLibrary.FillCardList();
            phases = testPhases;

            if (isClient)
            {
                new Nats.NatsClient();
            }

            MarketManager = new MarketManager();
        }
        
        public void StartGame()
        {
            
        }

        public void InitPhase(int p)
        {
            if (p >= phases.Length)
                return;
            
            var phase = phases[p];
            currentPhase = p;
            
            
            phase.InitFinished += (sender, e) =>
            {
                Debug.Log("Init finished");
                StartPhase();
            };
            phase.Init();
            
        }

        public void StartPhase()
        {
            if (currentPhase >= phases.Length)
                return;
            
            if (phases[currentPhase].InitComplete)
                phases[currentPhase].Start();
            else
                phases[currentPhase].InitFinished += (sender, e) => phases[currentPhase].Start();


        }
    }
}