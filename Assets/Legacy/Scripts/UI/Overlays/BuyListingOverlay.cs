using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{[Obsolete]
    public class BuyListingOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text priceDisplay;
        [SerializeField] private TMP_Text messageDisplay;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        public string Current;
        

        public void Open(string auctionId)
        {
            gameObject.SetActive(true);
            Current = auctionId;

            // Get the data of the given auction
            var listingData = MarketManager.Instance.GetListing(auctionId);

            messageDisplay.text = listingData.Price > PlayerManager.Instance.Balance 
                ? "You don't have enough money to buy this listing" 
                : "Are you sure you want to buy this listing?";

            priceDisplay.text = $"{listingData.Price.ToString()}$";

            // Check if the player can afford the cards
            confirmButton.interactable = PlayerManager.Instance.Balance >= listingData.Price;
            cancelButton.interactable = true;

            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => { Current = ""; }
        
        );

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
            {
                // Ensure player has enough money to buy the cards
                if (PlayerManager.Instance.Balance < listingData.Price)
                    return;
                
                // Handle the purchase of the cards
                MarketManager.Instance.BuyListingRequest(auctionId);

                // Turn of the buttons until a confirmation or denial is received
                confirmButton.interactable = false;
                cancelButton.interactable = false;
            });
        }
    }
}
