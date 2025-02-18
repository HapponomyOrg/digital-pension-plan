using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{[Obsolete]
    public class SellListingDisplay : MonoBehaviour
    {
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_Text priceDisplay;

        [SerializeField] private Transform cardRow;
        [SerializeField] private Image cardPrefab;

        [SerializeField] private Transform biddingList;
        [SerializeField] private ReceivedBiddingDisplay biddingPrefab;

        [SerializeField] public List<ReceivedBiddingDisplay> receivedBiddings = new List<ReceivedBiddingDisplay>();

        private void Awake()
        {
            NatsClient.Instance.OnStopRound += (sender, msg) => { RemoveBiddings(); };
        }

        public void SetDisplay(Listing listing)
        {
            cancelButton.onClick.RemoveAllListeners();

            priceDisplay.text = listing.Price.ToString();
            AddCardsToDisplay(listing.Cards);

            cancelButton.onClick.AddListener(() => MarketManager.Instance.OpenCancelOverlay(listing.AuctionId));
        }

        private void AddCardsToDisplay(int[] cards)
        {
            // Remove all the old cards
            foreach (Transform child in cardRow)
                Destroy(child.gameObject);

            Array.Sort(cards);

            // Generate the new cards
            foreach (var card in cards)
            {
                var d = Instantiate(cardPrefab, cardRow);
                d.sprite = PlayerManager.Instance.CardLibrary.cards[card].Art;
            }
        }

        public void AddBidding(Listing listing, Bidding bidding)
        {
            var display = Instantiate(biddingPrefab, biddingList);
            display.SetDisplay(listing,bidding);
            receivedBiddings.Add(display);
        }

        private void RemoveBiddings()
        {
            var biddingCount = receivedBiddings.Count;
            for (int i = 0; i < biddingCount; i++)
            {
                if (!receivedBiddings[i])
                    return;
                
                Destroy(receivedBiddings[i].gameObject);
                receivedBiddings.RemoveAt(i);
            }
        }
    }
}