using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Version1.Phases;
using Version1.Phases.BalanceModification;
using Version1.Phases.Interest;
using Version1.Phases.Trading;


namespace Version1.Utilities
{
    public class GameManager
    {
        private static GameManager instance;
        
        public static GameManager Instance => instance ??= new GameManager();

        public Cards.Scripts.CardLibrary CardLibrary { get; }
        
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
            Debug.Log($"card lib: {CardLibrary}");
            phases = testPhases;
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