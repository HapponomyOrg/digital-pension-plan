using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Version1.Market.Scripts.UI.Displays;

namespace Version1.Market.Scripts.UI.Overlays
{
    public class AddListingOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text priceDisplay;
        [SerializeField] private Button confirmButton;
        
        private List<int> selectedCards = new List<int>();
        private List<int> remainingCards = new List<int>();

        [SerializeField] private Transform selectedCardList;
        [SerializeField] private Transform remainingCardList;
        [SerializeField] private DetailsCardDisplay cardDisplay;

        private int price;
        [SerializeField] private int priceStep;

        private const int minPrice = 1000;
        private const int maxPrice = 25000;
        private const int defaultPrice = 5000;

        private readonly CultureInfo numberFormatter = new ("en-US")
        {
            NumberFormat = { NumberGroupSeparator = "." }
        };
        
        public void Open()
        {
            gameObject.SetActive(true);
            
            price = defaultPrice;
            priceDisplay.text = price.ToString("N0", numberFormatter);
            
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                Utilities.GameManager.Instance.MarketManager.AddListing(
                    PlayerData.PlayerData.Instance.PlayerId, 
                    DateTime.Now, 
                    price, 
                    selectedCards.ToArray());
                Close();
            });
            confirmButton.interactable = selectedCards.Count > 0;
            
            selectedCards.Clear();
            remainingCards = PlayerData.PlayerData.Instance.Cards;
            
            GenerateDisplays();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }


        private void SelectCard(int id)
        {
            selectedCards.Add(id);
            remainingCards.Remove(id);
            
            confirmButton.interactable = selectedCards.Count > 0;

            GenerateDisplays();
        }
        
        private void DeselectCard(int id)
        {
            remainingCards.Add(id);
            selectedCards.Remove(id);
            
            confirmButton.interactable = selectedCards.Count > 0;

            GenerateDisplays();
        }

        private void GenerateDisplays()
        {
            remainingCards.Sort();
            selectedCards.Sort();
        
            GenerateCardDisplays(remainingCardList, true);
            GenerateCardDisplays(selectedCardList, false);
        }
        
        private void GenerateCardDisplays(Transform list, bool remainingList)
        {
            foreach (Transform child in list)
                Destroy(child.gameObject);
            
            
            var cardAmounts = new Dictionary<int, int>();
            foreach (var cardId in remainingList ? remainingCards : selectedCards)
            {
                cardAmounts[cardId] = cardAmounts.TryGetValue(cardId, out var amount)
                    ? amount + 1 
                    : 1;
            }

            foreach (var cardAmount in cardAmounts)
            {
                var obj = Instantiate(cardDisplay, list);
                
                var button = obj.gameObject.AddComponent<Button>();
                if (remainingList)
                    button.onClick.AddListener(() => { SelectCard(cardAmount.Key); });
                else
                    button.onClick.AddListener(() => { DeselectCard(cardAmount.Key); });
                
                obj.Init(cardAmount.Key, cardAmount.Value);
            }
        }

        public void IncreasePrice()
        {
            price += priceStep;

            if (price > maxPrice)
                price = maxPrice;
            
            priceDisplay.text = price.ToString("N0", numberFormatter);
        }
        
        public void DecreasePrice()
        {
            price -= priceStep;

            if (price < minPrice)
                price = minPrice;
            
            priceDisplay.text = price.ToString("N0", numberFormatter);
        }
    }
}
