using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Market.Scripts.UI.Displays;

namespace Version1.Market.Scripts.UI.Overlays
{
    public class BuyListingOverlay : MarketOverlay
    {
        [SerializeField] private TMP_Text sellerName;
        [SerializeField] private TMP_Text price;
        
        [SerializeField] private Button confirm;

        [SerializeField] private Transform cardList;
        [SerializeField] private DetailsCardDisplay cardDisplay;
        
        public override void Open(Listing listing)
        {
            gameObject.SetActive(true);

            sellerName.text = listing.Lister.ToString();
            price.text = listing.Price.ToString();
            
            GenerateCardDisplays(listing);

            
            confirm.onClick.RemoveAllListeners();
            confirm.interactable = PlayerData.PlayerData.Instance.Balance >= listing.Price;
            
            confirm.onClick.AddListener(() => Buy(listing));
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

        private void Buy(Listing listing)
        {
            Utilities.GameManager.Instance.MarketManager.BuyListing(listing.ListingId);
            Close();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
