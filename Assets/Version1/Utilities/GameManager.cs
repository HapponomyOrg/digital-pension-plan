using Assets.Version1.Phases;
using UnityEngine;
using Version1.Market;

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
