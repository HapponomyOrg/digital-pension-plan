using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Market.Scripts.UI.PublicMarket
{
    public class MarketOfferDisplay : MonoBehaviour
    {
        private MarketOfferList marketOfferList;
        private Listing listing;

        [SerializeField] private TMP_Text sellerDisplay;
        [SerializeField] private TMP_Text priceDisplay;

        [SerializeField] private ButtonCommand _buyButton;
        [SerializeField] private ButtonCommand _bidButton;
        [SerializeField] private ButtonCommand _selectButton;


        [SerializeField] private Transform cardList;
        [SerializeField] private CardAmountDisplay cardIconPrefab;

        private const int minListingPriceForBids = 2000;

        private readonly CultureInfo numberFormatter = new("en-US")
        {
            NumberFormat = { NumberGroupSeparator = "." }
        };

        public void SetDisplay(MarketOfferList marketOfferList, Listing listing)
        {
            this.marketOfferList = marketOfferList;
            this.listing = listing;

            if (listing == null)
                return; // TODO Error handling

            sellerDisplay.text = listing.ListerName;
            priceDisplay.text = listing.Price.ToString("N0", numberFormatter);

            var playerData = PlayerData.PlayerData.Instance;
            var updateOnBalanceChange = new Func<Action, Action>(refresh => EventExtensions.SubscribeIgnoringParameters<int>(h => playerData.OnBalanceChange += h, h => playerData.OnBalanceChange -= h, refresh));

            _buyButton.Init(BuyListing, CanBuyListing, updateOnBalanceChange);
            _bidButton.Init(BidOnListing, CanBidOnListing, updateOnBalanceChange);

            _selectButton.Init(SelectListing);

            GenerateCardDisplays(listing.Cards);
        }

        private void BuyListing()
        {
            marketOfferList.OpenBuyListingOverlay(listing);
        }

        private bool CanBuyListing()
        {
            if (PlayerData.PlayerData.Instance.Balance < listing.Price)
                return false;

            return true;
        }

        private void BidOnListing()
        {
            marketOfferList.OpenCreateBidOverlay(listing);
        }

        private bool CanBidOnListing()
        {
            if (PlayerData.PlayerData.Instance.Balance <= 0)
                return false;
            if (listing.Price < minListingPriceForBids)
                return false;

            return true;
        }

        private void SelectListing()
        {
            marketOfferList.SetDetailsDisplay(listing);
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
                obj.SetDisplay(cardAmount.Key, cardAmount.Value, icon: true);
            }
        }
    }
}
