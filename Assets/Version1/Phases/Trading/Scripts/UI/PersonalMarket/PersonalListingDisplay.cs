using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Phases.Trading.Scripts.UI.PersonalMarket
{
    public class PersonalListingDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text priceDisplay;
        [SerializeField] private TMP_Text bidCountDisplay;
        [SerializeField] private Transform cardList;
        [SerializeField] private CardAmountDisplay cardIconPrefab;
        [SerializeField] private Button selectButton;
        [SerializeField] private Button cancelButton;

        private readonly CultureInfo numberFormatter = new("en-US")
        {
            NumberFormat = { NumberGroupSeparator = "." }
        };

        public void SetDisplay(Guid listingId, Dictionary<EListingAction, Action> listingActions)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            priceDisplay.text = listing.Price.ToString("N0", numberFormatter);

            var bidderCount = listing.BidRepository.GetUniqueBidderCount();
            var bidSuffix = bidderCount == 1 ? "bid" : "bids";

            bidCountDisplay.text = $"{bidderCount} {bidSuffix}";

            selectButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();

            if (listingActions.ContainsKey(EListingAction.Select))
                selectButton.onClick.AddListener(listingActions[EListingAction.Select].Invoke);
            if (listingActions.ContainsKey(EListingAction.Cancel))
                cancelButton.onClick.AddListener(listingActions[EListingAction.Cancel].Invoke);

            GenerateCards(listing.Cards);
        }

        private void GenerateCards(int[] cards)
        {
            foreach (Transform child in cardList)
                Destroy(child.gameObject);

            var cardAmounts = new Dictionary<int, int>();
            foreach (var cardId in cards)
            {
                cardAmounts[cardId] = cardAmounts.TryGetValue(cardId, out var amount)
                    ? amount + 1
                    : 1;
            }

            foreach (var cardAmount in cardAmounts)
            {
                var obj = Instantiate(cardIconPrefab, cardList);
                obj.SetDisplay(cardAmount.Key, cardAmount.Value, icon: true);
            }
        }
    }
}
