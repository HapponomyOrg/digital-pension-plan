using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Market.Scripts.UI.PublicMarket
{
    public class MarketOfferDetailsDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text sellerDisplay;
        [SerializeField] private TMP_Text priceDisplay;

        [SerializeField] private Button buyButton;
        [SerializeField] private Button bidButton;

        [SerializeField] private Transform cardList;
        [SerializeField] private CardAmountDisplay cardAmountPrefab;

        private readonly CultureInfo numberFormatter = new("en-US")
        {
            NumberFormat = { NumberGroupSeparator = "." }
        };

        public void SetDisplay(Guid listingId, Dictionary<EListingAction, Action> listingActions)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            sellerDisplay.text = listing.ListerName;
            priceDisplay.text = listing.Price.ToString("N0", numberFormatter);

            buyButton.onClick.RemoveAllListeners();
            bidButton.onClick.RemoveAllListeners();

            if (listingActions.ContainsKey(EListingAction.Buy))
                buyButton.onClick.AddListener(listingActions[EListingAction.Buy].Invoke);
            if (listingActions.ContainsKey(EListingAction.Bid))
                bidButton.onClick.AddListener(listingActions[EListingAction.Bid].Invoke);

            GenerateCardDisplays(listing.Cards);
        }

        private void GenerateCardDisplays(int[] cards)
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
                var obj = Instantiate(cardAmountPrefab, cardList);
                obj.SetDisplay(cardAmount.Key, cardAmount.Value);
            }
        }

        public void Clear()
        {
            priceDisplay.text = string.Empty;

            buyButton.onClick.RemoveAllListeners();
            bidButton.onClick.RemoveAllListeners();

            foreach (Transform child in cardList)
                Destroy(child.gameObject);
        }
    }
}
