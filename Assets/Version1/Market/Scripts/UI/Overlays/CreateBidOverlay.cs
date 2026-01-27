using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Utilities;

namespace Version1.Market
{
    public class CreateBidOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text originalAmountDisplay;
        [SerializeField] private TMP_Text bidAmountDisplay;
        [SerializeField] private Button confirmButton;

        [SerializeField] private Transform cardList;
        [SerializeField] private CardAmountDisplay cardAmountPrefab;


        private const int minBidAmount = 1000;
        private int maxBidAmount;

        private int bidAmount;
        [SerializeField] private int priceStep = 1000;


        private readonly CultureInfo numberFormatter = new("en-US")
        {
            NumberFormat = { NumberGroupSeparator = "." }
        };


        public void Open(Listing listing)
        {
            gameObject.SetActive(true);

            originalAmountDisplay.text = listing.Price.ToString("N0", numberFormatter);
            bidAmountDisplay.text = minBidAmount.ToString("N0", numberFormatter);
            bidAmount = minBidAmount;
            maxBidAmount = listing.Price - priceStep;

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => { Confirm(listing.ListingId); });

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
                var obj = Instantiate(cardAmountPrefab, cardList);
                obj.SetDisplay(cardAmount.Key, cardAmount.Value);
            }
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void Confirm(Guid listingId)
        {
            var bid = new Bid(
                Guid.NewGuid(), 
                PlayerData.PlayerData.Instance.PlayerId,
                PlayerData.PlayerData.Instance.PlayerName, 
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

            bidAmountDisplay.text = bidAmount.ToString("N0", numberFormatter);
        }

        public void DecreasePrice()
        {
            bidAmount -= priceStep;

            if (bidAmount < minBidAmount)
                bidAmount = minBidAmount;

            bidAmountDisplay.text = bidAmount.ToString("N0", numberFormatter);
        }
    }
}
