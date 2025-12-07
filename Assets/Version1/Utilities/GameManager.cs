using System;
using Assets.Version1.Phases;
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

        public IPhaseManager PhaseManager { get; private set; } = new BasePhaseManager();

        public Cards.Scripts.CardLibrary CardLibrary { get; }
        
        public IListingRepository ListingRepository { get; }

        public MarketServices MarketServices { get; }


        private GameManager()
        {
            CardLibrary = Resources.Load<Cards.Scripts.CardLibrary>("CardList");
            CardLibrary.FillCardList();

            ListingRepository = new ListingRepository();
            MarketServices = new MarketServices();
        }
    }
}
