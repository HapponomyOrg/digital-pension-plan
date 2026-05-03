using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Market;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.Trading.Scripts.UI.Overlays
{
    public class CreateBidOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text originalAmountDisplay;
        [SerializeField] private TMP_Text bidAmountDisplay;

        [SerializeField] private ButtonCommand _confirmButton;

        [SerializeField] private Transform cardList;
        [SerializeField] private CardAmountDisplay cardAmountPrefab;

        private event EventHandler<int> _bidOfferChanged;
        private Listing? _listing;

        private const int minBidAmount = 1000;
        private int maxBidAmount;

        private int _bidAmount;
        private int BidAmount
        {
            get => _bidAmount;
            set
            {
                _bidAmount = value;
                _bidOfferChanged?.Invoke(this, _bidAmount);
            }
        }

        [SerializeField] private int priceStep = 1000;


        private readonly CultureInfo numberFormatter = new("en-US")
        {
            NumberFormat = { NumberGroupSeparator = "." }
        };


        public void Open(Listing listing)
        {
            gameObject.SetActive(true);
            _listing = listing;

            originalAmountDisplay.text = _listing.Price.ToString("N0", numberFormatter);
            bidAmountDisplay.text = minBidAmount.ToString("N0", numberFormatter);
            BidAmount = minBidAmount;

            maxBidAmount = _listing.Price - priceStep;
            if (maxBidAmount > PlayerData.Instance.Balance)
                maxBidAmount = PlayerData.Instance.Balance;


            var updateOnOfferChange = new Func<Action, Action>(refresh => EventExtensions.SubscribeIgnoringParameters<int>(h => _bidOfferChanged += h, h => _bidOfferChanged -= h, refresh));
            _confirmButton.Init(CreateBid, CanCreateBid, updateOnOfferChange);

            GenerateCards(_listing.Cards);
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
                var obj = Instantiate(cardAmountPrefab, cardList);
                obj.SetDisplay(cardAmount.Key, cardAmount.Value);
            }
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void IncreasePrice()
        {
            BidAmount += priceStep;

            if (BidAmount > maxBidAmount)
                BidAmount = maxBidAmount;

            bidAmountDisplay.text = BidAmount.ToString("N0", numberFormatter);
        }

        public void DecreasePrice()
        {
            BidAmount -= priceStep;

            if (BidAmount < minBidAmount)
                BidAmount = minBidAmount;

            bidAmountDisplay.text = BidAmount.ToString("N0", numberFormatter);
        }

        private void CreateBid()
        {
            var bid = new Bid(
                Guid.NewGuid(),
                PlayerData.Instance.PlayerId,
                PlayerData.Instance.PlayerName,
                BidAmount,
                DateTime.Now);

            Utilities.GameManager.Instance.MarketServices.CreateBidService.CreateBidLocally(_listing.ListingId, bid);
            Close();
        }

        private bool CanCreateBid()
        {
            if (_listing == null)
                return false;
            if (PlayerData.Instance.Balance < BidAmount)
                return false;

            return true;
        }
    }
}
