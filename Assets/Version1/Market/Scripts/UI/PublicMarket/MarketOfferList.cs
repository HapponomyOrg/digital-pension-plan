using Codice.Client.BaseCommands.BranchExplorer.Layout;
using System;
using System.Collections.Generic;
using UnityEngine;
using Version1.Market.Scripts.UI.Overlays;
using Version1.Market.Scripts.UI.PublicMarket;
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

        public void Clear()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            marketOfferDetailsDisplayPrefab.Clear();
            marketOffers.Clear();
        }

        public void CreateDisplay(Guid listingId)
        {
            var listing = GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
            {
                // TODO Error handling
                return;
            }

            var display = Instantiate(marketOfferDisplayPrefab, transform);

            display.SetDisplay(this, listing);
            marketOffers.Add(listingId, display);
        }

        public bool ContainsListing(Guid listingId)
        {
            return marketOffers.ContainsKey(listingId);
        }

        public void UpdateDisplay(Guid listingId)
        {
            var display = marketOffers[listingId];
            var listing = GameManager.Instance.ListingRepository.GetListing(listingId);

            display.SetDisplay(this, listing);
            Destroy(display.gameObject);
        }

        public void RemoveDisplay(Guid listingId)
        {
            var display = marketOffers[listingId];

            marketOffers.Remove(listingId);
            Destroy(display.gameObject);
        }

        public void OpenBuyListingOverlay(Listing listing)
        {
            buyListingOverlay.Open(listing);
        }

        public void OpenCreateBidOverlay(Listing listing)
        {
            buyListingOverlay.Open(listing);
        }

        public void SetDetailsDisplay(Listing listing)
        {
            marketOfferDetailsDisplayPrefab.SetDisplay(listing);
        }
    }
}
