using System;
using System.Collections.Generic;
using System.Linq;

namespace Version1.Market
{
    public class Listing
    {
        public int Lister { get; private set; }
        public Guid ListingId { get; private set; }
        public DateTime TimeStamp { get; private set; }
        
        public int Price { get; private set; }
        public int[] Cards { get; private set; }
        public List<BidHistory> BidHistories { get; private set; }

        public void AddBid(int bidderId, int offeredPrice)
        {
            var bidderHistory = BidHistories.FirstOrDefault(h => h.Bidder == bidderId);
            
            if (bidderHistory == null)
            {
                bidderHistory = new BidHistory();
                BidHistories.Add(bidderHistory);
            }

            bidderHistory.AddBidding(bidderId, offeredPrice);
        }
        
        public void RemoveBid(int bidderId, Guid biddingId) 
        {
            var bidderHistory = BidHistories.FirstOrDefault(h => h.Bidder == bidderId);
            
            if (bidderHistory == null)
                return;

            bidderHistory.RemoveBid(biddingId);
        }
        
        public void RemoveAllBids()
        {
            foreach (var history in BidHistories)
                history.RemoveAllBids();
        }
    }
}