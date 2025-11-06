using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Utilities;

namespace Version1.Market
{
    public class OutgoingBidDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text sellerDisplay;
        [SerializeField] private TMP_Text offerDisplay;

        [SerializeField] private Button cancelButton;

        [SerializeField] private Transform cardList;
        [SerializeField] private CardAmountDisplay cardIconPrefab;


        public void SetDisplay(Guid listingId, Guid bidId, Dictionary<EBidAction, Action> bidActions)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            var bid = listing.BidRepository.GetBidBetweenPlayer(PlayerData.PlayerData.Instance.PlayerId, bidId);

            if (bid == null)
                return; // TODO Error handling

            sellerDisplay.text = listing.Lister.ToString();
            offerDisplay.text = bid.BidOffer.ToString();

            cancelButton.onClick.RemoveAllListeners();

            if (bidActions.ContainsKey(EBidAction.Cancel))
                cancelButton.onClick.AddListener(bidActions[EBidAction.Cancel].Invoke);

            GenerateCardDisplays(listing.Cards);
        }

        private void GenerateCardDisplays(int[] cards)
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
