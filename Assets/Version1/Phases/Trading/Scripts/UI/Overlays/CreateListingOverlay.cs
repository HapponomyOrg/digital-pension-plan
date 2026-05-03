using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Market;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.Trading.Scripts.UI.Overlays
{
    public class CreateListingOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text priceDisplay;
        [SerializeField] private ButtonCommand _createListing;

        [SerializeField] private Transform selectedCardList;
        [SerializeField] private Transform remainingCardList;
        [SerializeField] private CardAmountDisplay cardDisplay;


        private event EventHandler<List<int>> _selectedCardsChanged;
        private List<int> selectedCards = new();
        private List<int> remainingCards = new();

        private const int minPrice = 1000;
        private const int maxPrice = 25000;
        private const int defaultPrice = 5000;

        private int price;
        [SerializeField] private int priceStep;


        private readonly CultureInfo numberFormatter = new("en-US")
        {
            NumberFormat = { NumberGroupSeparator = "." }
        };

        public void Open()
        {
            gameObject.SetActive(true);

            price = defaultPrice;
            priceDisplay.text = price.ToString("N0", numberFormatter);

            var updateOnSelectedChange = new Func<Action, Action>(refresh => EventExtensions.SubscribeIgnoringParameters<List<int>>(h => _selectedCardsChanged += h, h => _selectedCardsChanged -= h, refresh));
            _createListing.Init(CreateListing, CanCreateListing, updateOnSelectedChange);

            selectedCards.Clear();
            remainingCards = PlayerData.Instance.Cards;

            GenerateDisplays();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void CreateListing()
        {
            var listing = new Listing(
                Guid.NewGuid(),
                PlayerData.Instance.PlayerId,
                PlayerData.Instance.PlayerName,
                DateTime.Now,
                price,
                selectedCards.ToArray());

            Utilities.GameManager.Instance.MarketServices.CreateListingService.CreateListingLocally(listing);
            selectedCards.Clear();
            remainingCards.Clear();

            Close();
        }

        private bool CanCreateListing()
        {
            return selectedCards.Count > 0;
        }

        private void SelectCard(int id)
        {
            selectedCards.Add(id);
            remainingCards.Remove(id);

            _selectedCardsChanged?.Invoke(this, selectedCards);
            GenerateDisplays();
        }

        private void DeselectCard(int id)
        {
            remainingCards.Add(id);
            selectedCards.Remove(id);

            _selectedCardsChanged?.Invoke(this, selectedCards);
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

                obj.SetDisplay(cardAmount.Key, cardAmount.Value);
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
