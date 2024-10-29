using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class RespondToBiddingOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text responseMessageDisplay;
        [SerializeField] private TMP_Text priceDisplay;
        
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private int currentPrice;

        private int minPrice;
        private int maxPrice;

        [SerializeField] private int priceStep = 1000;
        public void Open(ReceivedBiddingDisplay biddingDisplay, Bidding bidding)
        {
            gameObject.SetActive(true);

            
            // TODO remove increase and decrease when bid and price differs one step
            minPrice = bidding.OfferPrice + priceStep;
            maxPrice = MarketManager.Instance.GetListing(bidding.AuctionId).Price - priceStep;
            
            currentPrice = 0;
            
            UpdatePriceDisplay();
            
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => { gameObject.SetActive(false); });

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                
                // TODO RETURN AN ERROR
                if (currentPrice < minPrice)
                    return;
                
                MarketManager.Instance.RespondToBidding(biddingDisplay, 
                    new Bidding(PlayerManager.Instance.PlayerId, 
                    PlayerManager.Instance.PlayerName, 
                    bidding.AuctionId, 
                    currentPrice), bidding.SenderId);
                gameObject.SetActive(false);
            });
        }

        public void UpdatePriceDisplay()
        {
            responseMessageDisplay.text = $"Are you sure you want to respond to this bidding with {currentPrice}$?";
            priceDisplay.text = currentPrice.ToString();
        }

        public void IncreasePrice()
        {
            currentPrice += priceStep;
            if (currentPrice > maxPrice)
                currentPrice = maxPrice;
        }
        
        public void DecreasePrice()
        {
            currentPrice -= priceStep;
            if (currentPrice < minPrice)
                currentPrice = 0;
        }
    }
}
