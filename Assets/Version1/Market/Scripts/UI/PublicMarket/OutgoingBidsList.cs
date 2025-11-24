using System;
using System.Collections.Generic;
using UnityEngine;
using Version1.Utilities;

namespace Version1.Market
{
    public class OutgoingBidsList : MonoBehaviour
    {
        private readonly Dictionary<Guid, OutgoingBidDisplay> outgoingBids = new();
        [SerializeField] private OutgoingBidDisplay outgoingBidDisplayPrefab;

        [Header("Overlays")]
        [SerializeField] private CancelBidOverlay cancelBidOverlay;

        public void InitializeData(List<(Guid listingId, Guid bid)> bids)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            outgoingBids.Clear();

            foreach (var (listingId, bid) in bids)
                CreateDisplay(listingId, bid);
        }

        public void CreateDisplay(Guid listingId, Guid bidId)
        {
            var display = Instantiate(outgoingBidDisplayPrefab, transform);

            var displayActions = new Dictionary<EBidAction, Action>
            {
                { EBidAction.Cancel, () => { CancelAction(listingId, bidId); } }
            };

            display.SetDisplay(listingId, bidId, displayActions);
            outgoingBids.Add(listingId, display);
        }

        public bool ContainsListing(Guid listingId)
        {
            return outgoingBids.ContainsKey(listingId);
        }

        public void UpdateDisplay(Guid listingId, Guid bidId)
        {
            var display = outgoingBids[listingId];

            var displayActions = new Dictionary<EBidAction, Action>
            {
                { EBidAction.Cancel, () => { CancelAction(listingId, bidId); } }
            };

            display.SetDisplay(listingId, bidId, displayActions);
            Destroy(display.gameObject);
        }

        public void RemoveDisplay(Guid listingId)
        {
            var display = outgoingBids[listingId];

            outgoingBids.Remove(listingId);
            Destroy(display.gameObject);
        }

        // TODO implementation
        private void CancelAction(Guid listingId, Guid bidId)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            var bid = listing.BidRepository.GetBidBetweenPlayer(PlayerData.PlayerData.Instance.PlayerId, bidId);

            cancelBidOverlay.Open(listingId, bid);
        }
    }
}
