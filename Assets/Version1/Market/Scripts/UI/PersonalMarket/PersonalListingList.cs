using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Version1.Utilities;

namespace Version1.Market
{
    public class PersonalListingList : MonoBehaviour
    {
        private readonly Dictionary<Guid, PersonalListingDisplay> listingDisplays = new();
        [SerializeField] private PersonalListingDisplay listingDisplayPrefab;

        [field: SerializeField] public PersonalListingDetailsDisplay DetailsDisplay { get; private set; }
        [field: SerializeField] public ReceivedBidsList ReceivedBidsList { get; private set; }

        [Header("Overlays")]
        [SerializeField] private CancelListingOverlay cancelListingOverlay;


        public void InitializeData(Guid[] listings)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            listingDisplays.Clear();

            foreach (var offer in listings)
                CreateDisplay(offer);
        }

        public void CreateDisplay(Guid listingId)
        {
            var display = Instantiate(listingDisplayPrefab, transform);

            var displayActions = new Dictionary<EListingAction, Action>
            {
                { EListingAction.Cancel, () => { CancelAction(listingId); } },
                { EListingAction.Select, () => { SelectAction(listingId); } }
            };

            display.SetDisplay(listingId, displayActions);
            listingDisplays.Add(listingId, display);
        }

        public void UpdateDisplay(Guid listingId)
        {
            var display = listingDisplays[listingId];

            var displayActions = new Dictionary<EListingAction, Action>
            {
                { EListingAction.Cancel, () => { CancelAction(listingId); } },
                { EListingAction.Select, () => { SelectAction(listingId); } }
            };

            display.SetDisplay(listingId, displayActions);
        }

        public void RemoveDisplay(Guid listingId)
        {
            var display = listingDisplays[listingId];

            listingDisplays.Remove(listingId);
            Destroy(display.gameObject);
        }

        private void CancelAction(Guid listingId)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            cancelListingOverlay.Open(listing);
            Console.WriteLine("CancelAction");
        }

        private void SelectAction(Guid listingId)
        {
            var displayActions = new Dictionary<EListingAction, Action>
            {
                { EListingAction.Cancel, () => { CancelAction(listingId); } }
            };

            DetailsDisplay.SetDisplay(listingId, displayActions);

            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            var uniqueBidders = listing.BidRepository.GetUniqueBidders();
            var uniqueBids = new List<Guid>();

            foreach (var bidder in uniqueBidders)
            {
                var lastBid = listing.BidRepository.GetLastBidBetweenPlayer(bidder);
                if (lastBid.Bidder == bidder)
                    uniqueBids.Add(lastBid.BidId);
            }

            ReceivedBidsList.InitializeData(listingId, uniqueBids);
            Console.WriteLine("SelectAction");
        }
    }
}
