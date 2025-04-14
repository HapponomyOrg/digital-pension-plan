using System;
using System.Collections.Generic;
using UnityEngine;

namespace Version1.Market.Scripts
{
    public class Listing
    {
        public Guid ListingId { get; private set; }
        public int Lister { get; private set; }
        public DateTime TimeStamp { get; private set; }
        
        public int Price { get; private set; }
        public int[] Cards { get; private set; }
        
        
        // dictionary of trade history between each player and the lister
        public Dictionary<int, BidHistory> BidHistories { get; private set; }

        
        public Listing(Guid listingId, int lister, DateTime timeStamp, int price, int[] cards)
        {
            ListingId = listingId;
            Lister = lister;
            TimeStamp = timeStamp;
            Price = price;
            Cards = cards;
            BidHistories = new Dictionary<int, BidHistory>();
        }
        
        public void AddBid(int originalBidder, Bid bid)
        {
            BidHistories.TryGetValue(originalBidder, out var bidHistory);

            if (bidHistory == null)
            {
                bidHistory = new BidHistory();
                BidHistories.Add(originalBidder, bidHistory);
            }

            bidHistory.AddBid(bid);
        }

        public void AcceptBid(int originalBidder, Guid bidId)
        {
            var bid = BidHistories[originalBidder].GetBid(bidId);

            if (bid == null)
            {
                Debug.LogWarning("bid not in history");
                return;
            }
            
            if (PlayerData.PlayerData.Instance.PlayerId != Lister || PlayerData.PlayerData.Instance.PlayerId != originalBidder)
                return;
            
            if (PlayerData.PlayerData.Instance.PlayerId == Lister)
            {
                PlayerData.PlayerData.Instance.Balance += bid.OfferedPrice;
                Debug.Log("Received money from accepted bid");

            }

            if (PlayerData.PlayerData.Instance.PlayerId == originalBidder)
            {
                PlayerData.PlayerData.Instance.AddCards(Cards);
                Debug.Log("Received cards from accepted bid");
            }
        }

        public void CancelBid(int originalBidder, Guid bidId)
        {
            BidHistories[originalBidder].CancelBid(bidId);
        }
        
        public override string ToString()
        {
            return $"ListingId: {ListingId.ToString()}, ListerId: {Lister}, Timestamp: {TimeStamp}, Price: {Price}";
        }
    }
}