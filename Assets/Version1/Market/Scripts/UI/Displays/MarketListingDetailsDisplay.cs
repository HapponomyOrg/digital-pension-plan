using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Market.Scripts.UI.Displays
{
    public class MarketListingDetailsDisplay : MonoBehaviour, IListingDisplay
    {
        [SerializeField] private TMP_Text sellerName;
        [SerializeField] private TMP_Text price;

        [SerializeField] private Button buy;
        [SerializeField] private Button bid;

        [SerializeField] private Transform cardList;
        [SerializeField] private DetailsCardDisplay cardDisplay;

        public void Init(Listing l, Dictionary<ListingDisplayAction, Action> displayActions)
        {
            sellerName.text = l.Lister.ToString();
            price.text = l.Price.ToString();

            GenerateCardDisplays(l);

            buy.onClick.RemoveAllListeners();
            bid.onClick.RemoveAllListeners();

            buy.onClick.AddListener(displayActions[ListingDisplayAction.Buy].Invoke);
            bid.onClick.AddListener(displayActions[ListingDisplayAction.Bid].Invoke);
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
