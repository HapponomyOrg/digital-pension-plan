using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Utilities;

namespace Version1.Market
{
    public class MarketOfferDetailsDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text priceDisplay;
        //[SerializeField] private TMP_Text bidCountDisplay;

        [SerializeField] private Button buyButton;
        [SerializeField] private Button bidButton;

        [SerializeField] private Transform cardList;
        [SerializeField] private CardAmountDisplay cardAmountPrefab;


        public void SetDisplay(Guid listingId, Dictionary<EListingAction, Action> listingActions)
        {
            var listing = GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            priceDisplay.text = listing.Price.ToString();
            //bidCountDisplay.text = listing.BidRepository.GetUniqueBidderCount().ToString();

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
    }
}
