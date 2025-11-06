using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Utilities;

namespace Version1.Market
{
    public class MarketOfferDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text sellerDisplay;
        [SerializeField] private TMP_Text priceDisplay;

        [SerializeField] private Button buyButton;
        [SerializeField] private Button bidButton;
        [SerializeField] private Button selectButton;

        [SerializeField] private Transform cardList;
        [SerializeField] private CardAmountDisplay cardIconPrefab;

        private const int minListingPriceForBids = 2000;

        public void SetDisplay(Guid listingId, Dictionary<EListingAction, Action> listingActions)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            sellerDisplay.text = listing.Lister.ToString();
            priceDisplay.text = listing.Price.ToString();

            buyButton.onClick.RemoveAllListeners();
            bidButton.onClick.RemoveAllListeners();
            selectButton.onClick.RemoveAllListeners();

            bidButton.interactable = listing.Price >= minListingPriceForBids;


            if (listingActions.ContainsKey(EListingAction.Buy))
                buyButton.onClick.AddListener(listingActions[EListingAction.Buy].Invoke);
            if (listingActions.ContainsKey(EListingAction.Bid))
                bidButton.onClick.AddListener(listingActions[EListingAction.Bid].Invoke);
            if (listingActions.ContainsKey(EListingAction.Select))
                selectButton.onClick.AddListener(listingActions[EListingAction.Select].Invoke);

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
                var obj = Instantiate(cardIconPrefab, cardList);
                obj.SetDisplay(cardAmount.Key, cardAmount.Value);
            }
        }
    }
}
