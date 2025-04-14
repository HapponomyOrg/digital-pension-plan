using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Version1.Market.Scripts
{
    public class BidHistory
    {
        private readonly List<Tuple<int, Bid>> history = new List<Tuple<int, Bid>>();
        
        public void AddBid(Bid bid)
        {
            history.Add(new Tuple<int, Bid>(bid.Bidder, bid));
        }

        public void CancelBid(Guid bidId)
        {
            var bidTuple = history.FirstOrDefault(b => b.Item2.BidId == bidId);
            
            if (bidTuple == null)
            {
                Debug.LogWarning("Canceling bid failed");
                return;
            }

            bidTuple.Item2.BidStatus = BidStatus.Canceled;
        }

        public Bid GetBid(Guid bidId)
        {
            var bid = history.FirstOrDefault(b => b.Item2.BidId == bidId);

            return bid?.Item2;
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