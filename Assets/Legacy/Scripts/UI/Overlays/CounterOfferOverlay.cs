using System;
using NATS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Overlays
{
    public class CounterOfferOverlay : MonoBehaviour
    {
        [SerializeField] private Image cardDisplayPrefab;
        [SerializeField] private Transform cardDisplay;

        [SerializeField] private TMP_Text counterMessageDisplay;
        [SerializeField] private TMP_Text oldPriceDisplay;
        [SerializeField] private TMP_Text newPriceDisplay;
        
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button rejectButton;

        
        public void Open(Bidding bidding, int originalOffer)
        {
            gameObject.SetActive(true);

            var l = MarketManager.Instance.GetListing(bidding.AuctionId);
            GenerateDisplays(l.Cards);

            var remainingCost = bidding.OfferPrice - originalOffer;

            counterMessageDisplay.text = $"{bidding.Sender} has countered your offer";
            oldPriceDisplay.text = originalOffer.ToString();
            newPriceDisplay.text = bidding.OfferPrice.ToString();
            
            acceptButton.interactable = PlayerManager.Instance.Balance >= remainingCost;
            
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(() =>
            {
                MarketManager.Instance.AcceptCounterBidding(bidding, remainingCost);
                gameObject.SetActive(false);
            });
            
            rejectButton.onClick.RemoveAllListeners();
            rejectButton.onClick.AddListener(() =>
            {
                var msg = new RejectBiddingMessage(
                    DateTime.Now.ToString("o"),
                    PlayerManager.Instance.LobbyID,
                    PlayerManager.Instance.PlayerId,
                    bidding.AuctionId,
                    PlayerManager.Instance.PlayerName,
                    bidding.SenderId
                );
                NatsClient.Instance.Publish(PlayerManager.Instance.LobbyID.ToString(), msg);
                
                l.RemoveAllBiddings(false);
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
    }
}
