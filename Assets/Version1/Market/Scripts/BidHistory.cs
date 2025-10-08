using System;
using System.Collections.Generic;
using System.Linq;

namespace Version1.Market.Scripts
{
    public class BidHistory
    {
        public int Lister { get; private set; }
        public int Buyer { get; private set; }

        private readonly List<Bid> bids = new List<Bid>();

        public BidHistory(int lister, int buyer)
        {
            Lister = lister;
            Buyer = buyer;
        }

        public void AddBid(int offeredAmount, bool bidByLister = false)
        {
            bids.Add(new Bid(Guid.NewGuid(), bidByLister ? Lister : Buyer, offeredAmount, DateTime.Now));
        }

        public void CancelBid(Guid bidId)
        {
            var lastBid = LastActiveBid();

            if (lastBid == null || lastBid.Value.BidId != bidId)
                return;

            bids.RemoveAll(b => b.BidId == bidId);
        }

        public Bid[] GetSortedBiddingHistory()
        {
            return bids.OrderBy(b => b.TimeStamp).ToArray();
        }

        public Bid? LastActiveBid()
        {
            return bids.LastOrDefault(bid => bid.BidStatus == BidStatus.Active);
        }
    }
}
