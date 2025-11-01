using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Utilities;

namespace Version1.Market
{
    public class PersonalListingDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text priceDisplay;
        [SerializeField] private TMP_Text bidCountDisplay;
        [SerializeField] private Transform cardList;
        [SerializeField] private CardAmountDisplay cardIconPrefab;
        [SerializeField] private Button selectButton;
        [SerializeField] private Button cancelButton;

        public void SetDisplay(Guid listingId, Dictionary<EListingAction, Action> listingActions)
        {
            var listing = GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            priceDisplay.text = listing.BidRepository.GetUniqueBidderCount().ToString();
            bidCountDisplay.text = listing.Lister.ToString();

            selectButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();

            if (listingActions.ContainsKey(EListingAction.Select))
                selectButton.onClick.AddListener(listingActions[EListingAction.Select].Invoke);
            if (listingActions.ContainsKey(EListingAction.Cancel))
                cancelButton.onClick.AddListener(listingActions[EListingAction.Cancel].Invoke);

            GenerateCards(listing.Cards);
        }

        private void GenerateCards(int[] cards)
        {
            foreach (Transform child in cardList)
                Destroy(child.gameObject);


            var cardAmounts = new Dictionary<int, int>();
            foreach (var cardId in cards)
            {
                cardAmounts[cardId] = cardAmounts.TryGetValue(cardId, out var amount)
                    ? amount + 1
                    : 1;
            }

            foreach (var cardAmount in cardAmounts)
            {
                var obj = Instantiate(cardIconPrefab, cardList);
                obj.SetDisplay(cardAmount.Key, cardAmount.Value);
            }
        }
    }
}
