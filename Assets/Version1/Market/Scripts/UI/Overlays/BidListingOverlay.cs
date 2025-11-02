using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Market.Scripts.UI.Displays;

namespace Version1.Market.Scripts.UI.Overlays
{
    public class BidListingOverlay : MarketOverlay
    {
        [SerializeField] private TMP_Text sellerName;
        [SerializeField] private TMP_Text price;
        [SerializeField] private TMP_Text offer;

        [SerializeField] private Button confirm;
        [SerializeField] private Slider offerSlider;
        [SerializeField] private int priceStep = 1000;
        private int offerAmount;

        [SerializeField] private Transform cardList;
        [SerializeField] private DetailsCardDisplay cardDisplay;


        public override void Open(Listing listing)
        {
            gameObject.SetActive(true);

            sellerName.text = listing.Lister.ToString();
            price.text = listing.Price.ToString();

            const int minOffer = 1;
            var maxOffer = ((PlayerData.PlayerData.Instance.Balance >= listing.Price
                ? listing.Price
                : PlayerData.PlayerData.Instance.Balance) - priceStep) / priceStep;

            offerSlider.minValue = minOffer;
            offerSlider.maxValue = maxOffer;
            offerSlider.value = (int)(maxOffer / 2);

            UpdateOfferAmount();

            GenerateCardDisplays(listing);


            confirm.onClick.RemoveAllListeners();
            confirm.interactable = PlayerData.PlayerData.Instance.Balance >= minOffer * priceStep;

            confirm.onClick.AddListener(() => Bid(listing));
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

        private void Bid(Listing listing)
        {
            Utilities.GameManager.Instance.MarketManager.AddBidToListing(listing, PlayerData.PlayerData.Instance.PlayerId, offerAmount);
            Close();
        }

        public void UpdateOfferAmount()
        {
            offerAmount = (int)offerSlider.value * priceStep;
            offer.text = offerAmount.ToString();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
