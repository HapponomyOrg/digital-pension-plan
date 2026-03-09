using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Utilities;
using Version1.Utilities.PlayerData;

namespace Version1.Market.Scripts.UI.PublicMarket
{
    public class MarketOfferDetailsDisplay : MonoBehaviour
    {
        [SerializeField] private MarketOfferList marketOfferList;
        public Listing? Listing { get; private set; }

        [SerializeField] private TMP_Text sellerDisplay;
        [SerializeField] private TMP_Text priceDisplay;

        [SerializeField] private ButtonCommand _buyButton;
        [SerializeField] private ButtonCommand _bidButton;

        [SerializeField] private Transform cardList;
        [SerializeField] private CardAmountDisplay cardAmountPrefab;

        private const int minListingPriceForBids = 2000;

        private readonly CultureInfo numberFormatter = new("en-US")
        {
            NumberFormat = { NumberGroupSeparator = "." }
        };

        public void SetDisplay(Listing listing)
        {
            Listing = listing;

            if (Listing == null)
                return; // TODO Error handling

            sellerDisplay.text = Listing.ListerName;
            priceDisplay.text = Listing.Price.ToString("N0", numberFormatter);

            var updateOnBalanceChange = new Func<Action, Action>(refresh => EventExtensions.SubscribeIgnoringParameters<int>(h => PlayerData.Instance.OnBalanceChange += h, h => PlayerData.Instance.OnBalanceChange -= h, refresh));

            var market = GameManager.Instance.MarketServices;
            var updateOnListingBought = new Func<Action, Action>(refresh => EventExtensions.SubscribeIgnoringParameters<ListingEventArgs>(h => market.BuyListingService.BuyListing += h, h => market.BuyListingService.BuyListing -= h, refresh));
            var updateOnListingCanceled = new Func<Action, Action>(refresh => EventExtensions.SubscribeIgnoringParameters<ListingEventArgs>(h => market.CancelListingService.CancelListing += h, h => market.CancelListingService.CancelListing -= h, refresh));
            var updateOnBidAccepted = new Func<Action, Action>(refresh => EventExtensions.SubscribeIgnoringParameters<BidEventArgs>(h => market.AcceptBidService.AcceptBid += h, h => market.AcceptBidService.AcceptBid -= h, refresh));

            _buyButton.Init(BuyListing, CanBuyListing, updateOnBalanceChange, updateOnListingBought, updateOnListingCanceled, updateOnBidAccepted);
            _bidButton.Init(BidOnListing, CanBidOnListing, updateOnBalanceChange, updateOnListingBought, updateOnListingCanceled, updateOnBidAccepted);

            GenerateCardDisplays(Listing.Cards);
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
            sellerDisplay.text = string.Empty;
            priceDisplay.text = string.Empty;
            Listing = null;

            foreach (Transform child in cardList)
                Destroy(child.gameObject);
        }

        private void BuyListing()
        {
            marketOfferList.OpenBuyListingOverlay(Listing);
        }

        private bool CanBuyListing()
        {
            if (Listing == null)
                return false;
            if (PlayerData.Instance.Balance < Listing.Price)
                return false;

            return true;
        }

        private void BidOnListing()
        {
            marketOfferList.OpenCreateBidOverlay(Listing);
        }

        private bool CanBidOnListing()
        {
            if (Listing == null)
                return false;
            if (PlayerData.Instance.Balance <= 0)
                return false;
            if (Listing.Price < minListingPriceForBids)
                return false;

            return true;
        }
    }
}
