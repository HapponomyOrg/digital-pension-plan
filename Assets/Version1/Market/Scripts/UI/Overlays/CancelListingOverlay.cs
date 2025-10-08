using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Version1.Market.Scripts.UI.Displays;

namespace Version1.Market.Scripts.UI.Overlays
{
    public class CancelListingOverlay : MarketOverlay
    {

        [SerializeField] private Button confirm;

        [SerializeField] private Transform cardList;
        [SerializeField] private DetailsCardDisplay cardDisplay;

        public override void Open(Listing listing)
        {
            gameObject.SetActive(true);

            confirm.onClick.RemoveAllListeners();
            confirm.onClick.AddListener(() => CancelListing(listing));

            GenerateCardDisplays(listing);
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

        private void CancelListing(Listing listing)
        {
            Utilities.GameManager.Instance.MarketManager.CancelListing(listing);
            Close();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
