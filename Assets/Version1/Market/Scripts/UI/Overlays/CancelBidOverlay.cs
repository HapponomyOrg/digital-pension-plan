using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Market.Scripts.UI.Displays;

namespace Version1.Market.Scripts.UI.Overlays
{
    public class CancelBidOverlay : MarketOverlay
    {
        
        [SerializeField] private TMP_Text sellerName;
        [SerializeField] private TMP_Text price;
        [SerializeField] private TMP_Text offer;
        
        [SerializeField] private Button confirm;

        [SerializeField] private Transform cardList;
        [SerializeField] private DetailsCardDisplay cardDisplay;
        
        
        public override void Open(Listing listing)
        {
            gameObject.SetActive(true);
            var playerId = PlayerData.PlayerData.Instance.PlayerId;
            //var bid = listing.BidHistories[playerId].GetSortedBiddingHistory()
                //.Last(b => b.Bidder == playerId);
            
            
            sellerName.text = listing.Lister.ToString();
            price.text = listing.Price.ToString();
            //offer.text = bid.OfferedPrice.ToString();
            
            GenerateCardDisplays(listing);
            
            confirm.onClick.RemoveAllListeners();
            //confirm.onClick.AddListener(() => CancelBid(listing, bid));
        }

        private void GenerateCardDisplays(Listing listing)
        {
            foreach (Transform child in cardList)
                Destroy(child.gameObject);
            
            
            var cardAmounts = new Dictionary<int, int>();
            foreach (var cardId in listing.Cards)
            {
                cardAmounts[cardId] = cardAmounts.TryGetValue(cardId, out var amount)
                    ? amount + 1 
                    : 1;
            }

            foreach (var cardAmount in cardAmounts)
            {
                var obj = Instantiate(cardDisplay, cardList);
                obj.Init(cardAmount.Key, cardAmount.Value);
            }
        }

        private void CancelBid(Listing listing, Bid bid)
        {
            //Utilities.GameManager.Instance.MarketManager.RemoveBidFromListing(listing.ListingId, bid.BidId, PlayerData.PlayerData.Instance.PlayerId);
            Close();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
