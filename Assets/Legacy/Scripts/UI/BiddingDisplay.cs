using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    [Obsolete]
    public class BiddingDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text priceDisplay;
        [SerializeField] private TMP_Text sellerDisplay;
        [SerializeField] private TMP_Text offerDisplay;

        [SerializeField] private Transform cardRow;
        [SerializeField] private Image cardPrefab;

        [SerializeField] private Button cancelBidding;

        private int biddingPrice;

        public void SetDisplay(Bidding bidding)
        {
            var listing = MarketManager.Instance.GetListing(bidding.AuctionId);
            listing.AddBidding(this);

            biddingPrice = bidding.OfferPrice;

            sellerDisplay.text = listing.Seller;
            priceDisplay.text = listing.Price.ToString();
            offerDisplay.text = bidding.OfferPrice.ToString();

            cancelBidding.onClick.AddListener(() =>
            {
                MarketManager.Instance.CancelBidding(bidding, gameObject);
                PlayerManager.Instance.Balance += biddingPrice;
                MarketManager.Instance.UpdateBalance();
            });
            AddCardsToDisplay(listing.Cards);
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

        public void Remove(bool accepted)
        {
            if (!accepted)
            {
                PlayerManager.Instance.Balance += biddingPrice;
                MarketManager.Instance.UpdateBalance();
            }

            if (this.gameObject != null)
                Destroy(this.gameObject);
        }
    }
}
