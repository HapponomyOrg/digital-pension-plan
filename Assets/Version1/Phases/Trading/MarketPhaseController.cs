using System;
using Assets.Version1.Phases;
using UnityEngine;
using Version1.Market;
using Version1.Nats.Messages.Host;
using Version1.Utilities;

namespace Version1.Phases.Trading
{
    public class MarketPhaseController : MonoBehaviour, IPhaseController
    {
        [SerializeField] private Timer timer;
        [SerializeField] private CardBar cardBar;
        [SerializeField] private TopBar topBar;
        [SerializeField] private MarketView marketView;

        private void Start()
        {
            timer.Init(300);
            cardBar.Init();
            topBar.Init();
            marketView.Clear();

            // Start the phase
            Utilities.GameManager.Instance.PhaseManager.CurrentPhaseController = this;
        }

        public void StartPhase()
        {
            timer.StartTimer();
            marketView.OpenMarket();
        }

        public void StopPhase()
        {
            timer.StopTimer();
            marketView.CloseMarket();

            ClearMarket();
        }

        public void OnDestroy()
        {
            Utilities.GameManager.Instance.PhaseManager.CurrentPhaseController = null;
        }

        // TODO Fix that disconnected players listings also get canceled
        private void ClearMarket()
        {
            var listingRepository = Utilities.GameManager.Instance.ListingRepository;

            var listings = listingRepository.GetPersonalListings(PlayerData.PlayerData.Instance.PlayerId);

            foreach (var item in listings)
            {
                Utilities.GameManager.Instance.MarketServices.CancelListingService.CancelListingLocally(item);
            }

            marketView.Clear();
        }
    }
}
