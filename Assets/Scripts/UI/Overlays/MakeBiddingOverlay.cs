using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Overlays
{
    public class MakeBiddingOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text responseMessageDisplay;
        [SerializeField] private TMP_Text priceDisplay;
        
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private int currentPrice;

        private int minPrice;
        private int maxPrice;

        [SerializeField] private int priceStep = 1000;
        
        public void Open(BuyListingDisplay buyListingDisplay, Listing listing)
        {
            gameObject.SetActive(true);

            
            // TODO remove increase and decrease when bid and price differs one step
            minPrice = priceStep;
            
            maxPrice = listing.Price - priceStep;
            if (maxPrice > PlayerManager.Instance.Balance)
                maxPrice = PlayerManager.Instance.Balance;

            // TODO make player buy card isntead of responding a makebidding because price is the same.
            if (maxPrice <= 0)
                maxPrice = listing.Price;
            
            currentPrice = 0;
            
            UpdatePriceDisplay();
            
            confirmButton.interactable = PlayerManager.Instance.Balance > 0;
            
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => { gameObject.SetActive(false); });

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                // TODO RETURN AN ERROR
                if (currentPrice < minPrice)
                    return;
                MarketManager.Instance.MakeBidding(
                    new Bidding(
                        PlayerManager.Instance.PlayerId, 
                        PlayerManager.Instance.PlayerName, 
                        listing.AuctionId, 
                        currentPrice));
                buyListingDisplay.TurnOffBiddingButton();
                PlayerManager.Instance.Balance -= currentPrice;
                MarketManager.Instance.UpdateBalance();
                gameObject.SetActive(false);
            });
        }

        public void UpdatePriceDisplay()
        {
            responseMessageDisplay.text = $"Are you sure you want to make an offer of {currentPrice}$ for this listing?";
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
            if (currentPrice < minPrice || currentPrice == 0)
                currentPrice = 0;
        }
    }
}
