using System;
using System.Collections.Generic;
using System.Linq;

namespace Version1.Market
{
    public class BidHistory
    {
        public int Lister { get; private set; }
        public int Bidder { get; private set; }

        private readonly List<Tuple<Bidding, int>> bids = new List<Tuple<Bidding, int>>();

        public void AddBidding(int bidderId, int offeredPrice)
        {
            if (bidderId != Lister || bidderId != Bidder)
                return;
            
            bids.Add(new Tuple<Bidding, int>(new Bidding(new Guid(), offeredPrice, DateTime.Now), bidderId));
        }

        public void RemoveBid(Guid biddingId) 
        {
            bids.RemoveAll(b => b.Item1.BidId == biddingId);
        }

        public void RemoveAllBids()
        {
            bids.Clear();
        }

        public Tuple<Bidding, int>[] GetSortedBiddingHistory()
        {
            return bids.OrderBy(h => h.Item1.TimeStamp).ToArray();
        }
    }
}