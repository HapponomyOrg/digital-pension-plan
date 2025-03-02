using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Market.Scripts.UI.Displays
{
    public class PersonalListingDetailsDisplay : MonoBehaviour, IListingDisplay
    {
        [SerializeField] private TMP_Text price;
        [SerializeField] private TMP_Text bids;
        
        [SerializeField] private Button cancel;

        [SerializeField] private Transform cardList;
        [SerializeField] private DetailsCardDisplay cardDisplay;
        
        public void Init(Listing l, Dictionary<ListingDisplayAction, Action> displayActions)
        {
            price.text = l.Price.ToString();
            bids.text = l.BidHistories.Count.ToString();
            
            GenerateCardDisplays(l);

            cancel.onClick.RemoveAllListeners();
            cancel.onClick.AddListener(displayActions[ListingDisplayAction.Cancel].Invoke);
        }
        
        private void GenerateCardDisplays(Listing listing)
        {
            foreach (Transform child in cardList)
                Destroy(child.gameObject);
            
            
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
