using System;
using System.Collections.Generic;
using System.Linq;

namespace Version1.Market.Scripts
{
    public class BidHistory
    {
        // public int Lister { get; private set; }
        // public int Buyer { get; private set; }

        // private readonly List<Bid> bids = new List<Bid>();

        // history between lister and buyer, int is the bidder id
        private readonly List<Tuple<int, Bid>> history = new List<Tuple<int, Bid>>();


        // public void AddBid(int offeredAmount, bool bidByLister = false)
        // {
        //     bids.Add(new Bid(Guid.NewGuid(), bidByLister ? Lister : Buyer, offeredAmount, DateTime.Now));
        // }

        // public void CancelBid(Guid bidId)
        // {
        //     var lastBid = LastActiveBid();
        //     
        //     if (lastBid == null || lastBid.Value.BidId != bidId)
        //         return;
        //
        //     bids.RemoveAll(b => b.BidId == bidId);
        // }

        // public Bid[] GetSortedBiddingHistory()
        // {
        //     return bids.OrderBy(b => b.TimeStamp).ToArray();
        // }

        // public Bid? LastActiveBid()
        // {
        //     return bids.LastOrDefault(bid => bid.BidStatus == BidStatus.Active);
        // }

        // public void AddBid(Guid guid, int offeredAmount, int bidder, DateTime timestamp)
        // {
        //     history.Add(new Tuple<int, Bid>(bidder, new Bid(guid, bidder, offeredAmount, timestamp)));
        // }
        
        public void AddBid(Bid bid)
        {
            history.Add(new Tuple<int, Bid>(bid.Bidder, bid));
        }

        public void CancelBid()
        {
            
        }
        
        public Tuple<int, Bid>[] GetBiddingHistory()
        {
            return history.OrderBy(b => b.Item2.TimeStamp).ToArray();
        }

        public Tuple<int, Bid> LastActiveBid()
        {
            return history.LastOrDefault(b => b.Item2.BidStatus == BidStatus.Active);
        }
    }
}