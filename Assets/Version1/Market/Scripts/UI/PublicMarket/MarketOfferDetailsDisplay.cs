using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Utilities;

#nullable enable
namespace Version1.Market.Scripts.UI.PublicMarket
{
    public class MarketOfferDetailsDisplay : MonoBehaviour
    {
        [SerializeField] private MarketOfferList marketOfferList;
        private Listing? listing;

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
            this.listing = listing;

            if (listing == null)
                return; // TODO Error handling

            sellerDisplay.text = listing.ListerName;
            priceDisplay.text = listing.Price.ToString("N0", numberFormatter);

            _buyButton.Init(BuyListing, CanBuyListing, SubscribeCommonEvents);
            _bidButton.Init(BidOnListing, CanBidOnListing, SubscribeCommonEvents);

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
            listing = null;

            foreach (Transform child in cardList)
                Destroy(child.gameObject);
        }

        private void BuyListing()
        {
            marketOfferList.OpenBuyListingOverlay(listing);
        }

        private bool CanBuyListing()
        {
            if (listing == null)
                return false;
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
            if (listing == null)
                return false;
            if (PlayerData.PlayerData.Instance.Balance <= 0)
                return false;
            if (listing.Price < minListingPriceForBids)
                return false;

            return true;
        }

        private void SubscribeCommonEvents(Delegate handler)
        {
            // Player balance changes
            PlayerData.PlayerData.Instance.OnBalanceChange += (EventHandler<int>)handler;

            //// Market events
            //var market = GameManager.Instance.MarketServices;
            //market.BuyListingService.BuyListing += (EventHandler<ListingEventArgs>)handler;
            //market.CancelListingService.CancelListing += (EventHandler<ListingEventArgs>)handler;
            //market.AcceptBidService.AcceptBid += (EventHandler<BidEventArgs>)handler;
        }
    }
}
