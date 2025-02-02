using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Market.Scripts.UI
{
    public class MarketListingDisplay : MonoBehaviour
    {
        private Listing listing;
        
        [SerializeField] private TMP_Text sellerName;
        [SerializeField] private TMP_Text price;
        [SerializeField] private Button buy;
        [SerializeField] private Button bid;

        [SerializeField] private Transform cardList;
        [SerializeField] private ListingCardDisplay cardDisplay;
        
        public void Init(Listing marketListing, Action buyMethod, Action bidMethod)
        {
            listing = marketListing;
            
            sellerName.text = listing.Lister.ToString();
            price.text = listing.Price.ToString();

            GenerateCardDisplays();
            
            buy.onClick.AddListener(buyMethod.Invoke);
            bid.onClick.AddListener(bidMethod.Invoke);
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
    }
}
