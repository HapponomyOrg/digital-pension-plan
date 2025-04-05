using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Market.Scripts.UI.Displays
{
    public class MarketBidListingDisplay : MonoBehaviour, IListingDisplay
    {
        private Listing listing;
        
        [SerializeField] private TMP_Text sellerName;
        [SerializeField] private TMP_Text state;
        [SerializeField] private TMP_Text offer;
        [SerializeField] private Button cancel;
        [SerializeField] private Button selectButton;

        [SerializeField] private Transform cardList;
        [SerializeField] private ListingCardDisplay cardDisplay;
        
        public void Init(Listing l, Dictionary<ListingDisplayAction, Action> displayActions)
        {
            listing = l;
            //var lastBid = listing.BidHistories[PlayerData.PlayerData.Instance.PlayerId].GetSortedBiddingHistory().Last();

            var lastBid = listing.BidHistories[PlayerData.PlayerData.Instance.PlayerId].LastActiveBid();

            if (lastBid == null)
                return;
            
            sellerName.text = listing.Lister.ToString();
            state.text = lastBid.Item2.BidStatus.ToString();
            
            offer.text = lastBid.Item2.OfferedPrice.ToString();

            GenerateCardDisplays();
            
            cancel.onClick.AddListener(displayActions[ListingDisplayAction.Cancel].Invoke);
            selectButton.onClick.AddListener(displayActions[ListingDisplayAction.Select].Invoke);
        }

        private void GenerateCardDisplays()
        {
            var cardAmounts = new Dictionary<int, int>();
            foreach (var cardId in listing.Cards)
            {
                cardAmounts[cardId] = cardAmounts.TryGetValue(cardId, out var amount)
                    ? amount + 1 
                    : 1;
            }

            foreach (var cardAmount in cardAmounts)
            {
                var obj = Instantiate(cardDisplay, cardList);
                obj.Init(cardAmount.Key, cardAmount.Value);
            }
        }
        
        public GameObject GameObject() => gameObject;

        public void UpdateDisplay()
        {
            
        }
    }
}
