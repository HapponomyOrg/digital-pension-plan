using System;
using System.Collections.Generic;
using UI.Overlays;
using UnityEngine;
using Version1.Utilities;

namespace Version1.Market
{
    public class ReceivedBidsList : MonoBehaviour
    {
        private readonly Dictionary<Guid, ReceivedBidDisplay> bidDisplays = new();
        [SerializeField] private ReceivedBidDisplay bidDisplayPrefab;
        public Guid? ActiveListing { get; private set; } = null;

        [Header("Overlays")]
        [SerializeField] private AcceptBidOverlay acceptBidOverlay;
        [SerializeField] private ListerCounterBidOverlay createCounterBidOverlay;
        [SerializeField] private RejectBidOverlay rejectBidOverlay;

        public void InitializeData(Guid listing, List<Guid> uniqueBids)
        {
            Clear();

            ActiveListing = listing;

            foreach (var bid in uniqueBids)
                CreateDisplay(listing, bid);
        }

        public void CreateDisplay(Guid listingId, Guid bidId)
        {
            var display = Instantiate(bidDisplayPrefab, transform);

            var displayActions = new Dictionary<EBidAction, Action>
            {
                { EBidAction.Accept, () => { AcceptAction(listingId, bidId); } },
                { EBidAction.Counter, () => { CounterAction(listingId, bidId); } },
                { EBidAction.Reject, () => { RejectAction(listingId, bidId); } },
            };

            display.SetDisplay(listingId, bidId, displayActions);
            bidDisplays.Add(listingId, display);
        }

        public void UpdateDisplay(Guid listingId, Guid bidId)
        {
            var display = bidDisplays[listingId];

            var displayActions = new Dictionary<EBidAction, Action>
            {
                { EBidAction.Accept, () => { AcceptAction(listingId, bidId); } },
                { EBidAction.Counter, () => { CounterAction(listingId, bidId); } },
                { EBidAction.Reject, () => { RejectAction(listingId, bidId); } },
            };

            display.SetDisplay(listingId, bidId, displayActions);
            Destroy(display.gameObject);
        }

        public void RemoveDisplay(Guid listingId)
        {
            var display = bidDisplays[listingId];

            bidDisplays.Remove(listingId);
            Destroy(display.gameObject);
        }

        public void Clear()
        {
            ActiveListing = null;

            foreach (Transform child in transform)
                Destroy(child.gameObject);
            bidDisplays.Clear();
        }

        // TODO implementation
        private void AcceptAction(Guid listingId, Guid bidId)
        {
            var listing = GameManager.Instance.ListingRepository.GetListing(listingId);

            var bidOwner = listing.BidRepository.GetBidOwner(bidId);
            var bid = listing.BidRepository.GetBidBetweenPlayer(bidOwner, bidId);

            acceptBidOverlay.Open(listingId, bid);

            Console.WriteLine("AcceptAction");
        }

        // TODO implementation
        private void CounterAction(Guid listingId, Guid bidId)
        {
            var listing = GameManager.Instance.ListingRepository.GetListing(listingId);

            var bidOwner = listing.BidRepository.GetBidOwner(bidId);
            var bid = listing.BidRepository.GetBidBetweenPlayer(bidOwner, bidId);

            createCounterBidOverlay.Open(listing, bid);
            Console.WriteLine("CounterAction");
        }

        // TODO implementation
        private void RejectAction(Guid listingId, Guid bidId)
        {
            var listing = GameManager.Instance.ListingRepository.GetListing(listingId);

            var bidOwner = listing.BidRepository.GetBidOwner(bidId);
            var bid = listing.BidRepository.GetBidBetweenPlayer(bidOwner, bidId);

            rejectBidOverlay.Open(listingId, bid);
            Console.WriteLine("RejectAction");
        }
    }
}
