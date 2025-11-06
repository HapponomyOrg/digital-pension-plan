using System;
using System.Collections.Generic;
using UnityEngine;
using Version1.Utilities;

namespace Version1.Market
{
    public class MarketOfferList : MonoBehaviour
    {
        private readonly Dictionary<Guid, MarketOfferDisplay> marketOffers = new();
        [SerializeField] private MarketOfferDisplay marketOfferDisplayPrefab;

        [SerializeField] private MarketOfferDetailsDisplay marketOfferDetailsDisplayPrefab;

        [Header("Overlays")]
        [SerializeField] private BuyListingOverlay buyListingOverlay;
        [SerializeField] private CreateBidOverlay createBidOverlay;

        public void InitializeData(Guid[] listings)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            marketOffers.Clear();

            foreach (var offer in listings)
                CreateDisplay(offer);
        }

        public void CreateDisplay(Guid listingId)
        {
            var display = Instantiate(marketOfferDisplayPrefab, transform);

            var displayActions = new Dictionary<EListingAction, Action>
            {
                { EListingAction.Buy, () => { BuyAction(listingId); } },
                { EListingAction.Bid, () => { BidAction(listingId); } },
                { EListingAction.Select, () => { SelectAction(listingId); } }
            };

            display.SetDisplay(listingId, displayActions);
            marketOffers.Add(listingId, display);
        }

        public bool ContainsListing(Guid listingId)
        {
            return marketOffers.ContainsKey(listingId);
        }

        public void UpdateDisplay(Guid listingId)
        {
            var display = marketOffers[listingId];

            var displayActions = new Dictionary<EListingAction, Action>
            {
                { EListingAction.Buy, () => { BuyAction(listingId); } },
                { EListingAction.Bid, () => { BidAction(listingId); } },
                { EListingAction.Select, () => { SelectAction(listingId); } }
            };

            display.SetDisplay(listingId, displayActions);
            Destroy(display.gameObject);
        }

        public void RemoveDisplay(Guid listingId)
        {
            var display = marketOffers[listingId];

            marketOffers.Remove(listingId);
            Destroy(display.gameObject);
        }

        private void BuyAction(Guid listingId)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);
            buyListingOverlay.Open(listing);
            Console.WriteLine("buyAction");
        }

        private void BidAction(Guid listingId)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);
            createBidOverlay.Open(listing);
            Console.WriteLine("bidAction");
        }

        private void SelectAction(Guid listingId)
        {
            var displayActions = new Dictionary<EListingAction, Action>
            {
                { EListingAction.Buy, () => { BuyAction(listingId); } },
                { EListingAction.Bid, () => { BidAction(listingId); } }
            };

            marketOfferDetailsDisplayPrefab.SetDisplay(listingId, displayActions);
            Console.WriteLine("selectAction");
        }
    }
}
