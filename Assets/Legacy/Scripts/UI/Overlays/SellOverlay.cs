using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{[Obsolete]
    public class SellOverlay : MonoBehaviour
    {
        [SerializeField] private ListBox listBox;
        
        [SerializeField] private Image cardDisplayPrefab;
        [SerializeField] private Transform cardDisplay;
        
        [SerializeField] private int minCardPrice;
        [SerializeField] private int maxCardPrice;

        [SerializeField] private Button confirmButton;
        
        private int currentPrice;

        private int minPrice;

        
        [SerializeField] private TMP_Text priceDisplay;
        [SerializeField] private int priceStep = 1000;
        
        public void Open(int[] cards)
        {
            gameObject.SetActive(true);
            
            minPrice = priceStep;
            
            currentPrice = 0;
            UpdatePriceDisplay();
            
            GenerateDisplays(cards);
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                // TODO RETURN AN ERROR
                if (currentPrice < minPrice)
                    return;
                MarketManager.Instance.AddSellListing(currentPrice, cards);
                listBox.DestroyCardsInBox();
                PlayerManager.Instance.RemoveCards(cards);
                gameObject.SetActive(false);
            });
        }

        private void GenerateDisplays(int[] cards)
        {
            foreach (Transform child in cardDisplay)
                Destroy(child.gameObject);

            foreach (var card in cards)
            {
                var d = Instantiate(cardDisplayPrefab, cardDisplay);
                d.sprite = PlayerManager.Instance.CardLibrary.cards[card].Art;
            }
            
        }
        
        public void UpdatePriceDisplay()
        {
            priceDisplay.text = currentPrice.ToString();
        }
        
        public void IncreasePrice()
        {
            currentPrice += priceStep;
        }
        
        public void DecreasePrice()
        {
            currentPrice -= priceStep;
            if (currentPrice < minPrice)
                currentPrice = 0;
        }
    }
}
