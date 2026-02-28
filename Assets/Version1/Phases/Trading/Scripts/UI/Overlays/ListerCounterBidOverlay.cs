using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Market;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.Trading.Scripts.UI.Overlays
{
    public class ListerCounterBidOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text originalBidAmountDisplay;
        [SerializeField] private TMP_Text newBidAmountDisplay;
        [SerializeField] private Button confirmButton;

        private int minBidAmount;
        private int maxBidAmount;

        private int bidAmount;
        [SerializeField] private int priceStep = 1000;

        private readonly CultureInfo numberFormatter = new("en-US")
        {
            NumberFormat = { NumberGroupSeparator = "." }
        };

        public void Open(Listing listing, Bid bid)
        {
            gameObject.SetActive(true);

            minBidAmount = bid.BidOffer + priceStep;
            maxBidAmount = listing.Price - priceStep;

            newBidAmountDisplay.text = minBidAmount.ToString(numberFormatter);
            bidAmount = minBidAmount;

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => { Confirm(listing.ListingId); });
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void Confirm(Guid listingId)
        {
            var bid = new Bid(
                Guid.NewGuid(),
                PlayerData.Instance.PlayerId,
                PlayerData.Instance.PlayerName,
                bidAmount,
                DateTime.Now);

            Utilities.GameManager.Instance.MarketServices.CreateBidService.CreateBidLocally(listingId, bid);
            Close();
        }

        public void IncreasePrice()
        {
            bidAmount += priceStep;

            if (bidAmount > maxBidAmount)
                bidAmount = maxBidAmount;

            newBidAmountDisplay.text = bidAmount.ToString("N0", numberFormatter);
        }

        public void DecreasePrice()
        {
            bidAmount -= priceStep;

            if (bidAmount < minBidAmount)
                bidAmount = minBidAmount;

            newBidAmountDisplay.text = bidAmount.ToString("N0", numberFormatter);
        }
    }
}
