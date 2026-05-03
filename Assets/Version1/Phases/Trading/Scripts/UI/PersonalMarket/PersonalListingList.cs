using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Version1.Phases.Trading.Scripts.UI.Overlays;
using Version1.Utilities;

namespace Version1.Phases.Trading.Scripts.UI.PersonalMarket
{
    public class PersonalListingList : MonoBehaviour
    {
        private readonly Dictionary<Guid, PersonalListingDisplay> listingDisplays = new();
        [SerializeField] private PersonalListingDisplay listingDisplayPrefab;

        [field: SerializeField] public PersonalListingDetailsDisplay DetailsDisplay { get; private set; }
        [field: SerializeField] public ReceivedBidsList ReceivedBidsList { get; private set; }

        [Header("Overlays")]
        [SerializeField] private CancelListingOverlay cancelListingOverlay;


        public void Clear()
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            DetailsDisplay.Clear();
            listingDisplays.Clear();
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

            OrderList();
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

            OrderList();
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

        private void OrderList()
        {
            var ordered = listingDisplays
                .Select(kvp => new
                {
                    Display = kvp.Value,
                    Listing = GameManager.Instance.ListingRepository.GetListing(kvp.Key)
                })
                .Where(x => x.Listing != null)
                .OrderByDescending(x => x.Listing.GetHighestCard())
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].Display.transform.SetSiblingIndex(i);
            }
        }
    }
}
