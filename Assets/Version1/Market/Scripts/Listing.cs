using System;
using System.Collections.Generic;

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

        // public void AddBuyerBid(int buyer, int offeredAmount)
        // {
        //     BidHistories.TryGetValue(buyer, out var bidderHistory);
        //
        //     if (bidderHistory == null)
        //     {
        //         bidderHistory = new BidHistory(Lister, buyer);
        //         bidderHistory.AddBid(offeredAmount);
        //         BidHistories.Add(buyer, bidderHistory);
        //     }
        //     else
        //         bidderHistory.AddBid(offeredAmount);
        // }
        //
        // public void AddListerBid(int buyer, int offeredAmount)
        // {
        //     BidHistories.TryGetValue(buyer, out var bidderHistory);
        //
        //     if (bidderHistory == null)
        //         return; // TODO error handling
        //
        //     bidderHistory.AddBid(offeredAmount, true);
        // }

        public void AddBid(int offeredAmount, int bidder, int originalBidder, DateTime timestamp)
        {
            BidHistories.TryGetValue(originalBidder, out var bidHistory);

            if (bidHistory == null)
            {
                bidHistory = new BidHistory();
                BidHistories.Add(originalBidder, bidHistory);
            }

            bidHistory.AddBid(Guid.NewGuid(), offeredAmount, bidder, timestamp);
        }
        
        public void AddBid(Guid guid, int offeredAmount, int bidder, int originalBidder, DateTime timestamp)
        {
            BidHistories.TryGetValue(originalBidder, out var bidHistory);

            if (bidHistory == null)
            {
                bidHistory = new BidHistory();
                BidHistories.Add(originalBidder, bidHistory);
            }

            bidHistory.AddBid(guid, offeredAmount, bidder, timestamp);
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
        
        public override string ToString()
        {
            return $"ListingId: {ListingId.ToString()}, ListerId: {Lister}, Timestamp: {TimeStamp}, Price: {Price}";
        }
    }
}