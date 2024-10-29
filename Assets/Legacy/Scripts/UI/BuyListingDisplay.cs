using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuyListingDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text priceDisplay;
        [SerializeField] private TMP_Text sellerDisplay;
        [SerializeField] private TMP_Text amountDisplay;

        [SerializeField] private Transform cardRow;
        [SerializeField] private Image cardPrefab;

        [SerializeField] private Button buyButton;
        [SerializeField] private Button bidButton;


        private void Awake()
        {
            NatsClient.C.OnStartRound += (sender, msg) =>
            {
                if (buyButton)
                {
                    buyButton.interactable = true;
                }
                if (bidButton)
                {
                    bidButton.interactable = true;
                }
            };
        }

        public void SetDisplay(Listing listing)
        {
            buyButton.onClick.RemoveAllListeners();
            bidButton.onClick.RemoveAllListeners();

            sellerDisplay.text = listing.Seller;
            priceDisplay.text = listing.Price.ToString();
            amountDisplay.text = listing.Cards.Length.ToString();
            AddCardsToDisplay(listing.Cards);

            buyButton.onClick.AddListener(() => MarketManager.Instance.OpenBuyOverlay(listing.AuctionId));
            bidButton.onClick.AddListener(() => MarketManager.Instance.OpenMakeBiddingOverlay(this, listing));
        }

        private void AddCardsToDisplay(int[] cards)
        {
            // Remove all the old cards
            foreach (Transform child in cardRow)
                Destroy(child.gameObject);

            Array.Sort(cards);

            // Generate the new cards
            foreach (var card in cards)
            {
                var d = Instantiate(cardPrefab, cardRow);
                d.sprite = PlayerManager.Instance.CardLibrary.cards[card].Art;
            }
        }

        public void TurnOffBiddingButton()
        {
            bidButton.interactable = false;
        }

        public void TurnOnBiddingButton()
        {
            bidButton.interactable = true;
        }
    }
}