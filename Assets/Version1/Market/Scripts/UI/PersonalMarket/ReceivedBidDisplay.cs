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
    public class ReceivedBidDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text bidderNameDisplay;
        [SerializeField] private TMP_Text offerDisplay;

        [SerializeField] private Button acceptButton;
        [SerializeField] private Button counterButton;
        [SerializeField] private Button rejectButton;

        public void SetDisplay(Guid listingId, Guid bidId, Dictionary<EBidAction, Action> bidActions)
        {
            var listing = Utilities.GameManager.Instance.ListingRepository.GetListing(listingId);

            if (listing == null)
                return; // TODO Error handling

            var bidder = listing.BidRepository.GetBidOwner(bidId);
            var bid = listing.BidRepository.GetBidBetweenPlayer(bidder, bidId);

            if (bid == null)
                return; // TODO Error handling

            bidderNameDisplay.text = bid.BidderName;
            offerDisplay.text = bid.BidOffer.ToString();

            acceptButton.interactable = bid.BidStatus == EBidStatus.Active;
            counterButton.interactable = bid.BidStatus == EBidStatus.Active;
            rejectButton.interactable = bid.BidStatus == EBidStatus.Active;


            acceptButton.onClick.RemoveAllListeners();
            counterButton.onClick.RemoveAllListeners();
            rejectButton.onClick.RemoveAllListeners();

            if (bidActions.ContainsKey(EBidAction.Accept))
                acceptButton.onClick.AddListener(bidActions[EBidAction.Accept].Invoke);
            if (bidActions.ContainsKey(EBidAction.Counter))
                counterButton.onClick.AddListener(bidActions[EBidAction.Counter].Invoke);
            if (bidActions.ContainsKey(EBidAction.Reject))
                rejectButton.onClick.AddListener(bidActions[EBidAction.Reject].Invoke);
        }
    }
}
