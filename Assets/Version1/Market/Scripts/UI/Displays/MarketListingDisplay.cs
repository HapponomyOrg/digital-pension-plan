using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Market.Scripts.UI.Displays
{
    public class MarketListingDisplay : MonoBehaviour, IListingDisplay
    {
        private Listing listing;
        
        [SerializeField] private TMP_Text sellerName;
        [SerializeField] private TMP_Text price;
        [SerializeField] private Button buy;
        [SerializeField] private Button bid;
        [SerializeField] private Button selectButton;

        [SerializeField] private Transform cardList;
        [SerializeField] private ListingCardDisplay cardDisplay;
        
        public void Init(Listing l, Dictionary<ListingDisplayAction, Action> displayActions)
        {
            listing = l;
            
            sellerName.text = listing.Lister.ToString();
            price.text = listing.Price.ToString();

            GenerateCardDisplays();

            buy.interactable = PlayerData.PlayerData.Instance.Balance >= listing.Price;

            buy.onClick.AddListener(displayActions[ListingDisplayAction.Buy].Invoke);
            bid.onClick.AddListener(displayActions[ListingDisplayAction.Bid].Invoke);
            selectButton.onClick.AddListener(displayActions[ListingDisplayAction.Select].Invoke);
        }

        public GameObject GameObject() => gameObject;

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

        public void UpdateDisplay()
        {
            buy.interactable = PlayerData.PlayerData.Instance.Balance >= listing.Price;
        }
    }
}
