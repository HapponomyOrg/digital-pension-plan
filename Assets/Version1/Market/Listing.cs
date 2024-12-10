using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Version1.Market
{
    public class Listing
    {
        public Guid ListingId { get; private set; }
        public int Lister { get; private set; }
        public DateTime TimeStamp { get; private set; }
        
        public int Price { get; private set; }
        public int[] Cards { get; private set; }
        public Dictionary<int, BidHistory> BidHistories { get; private set; } = new Dictionary<int, BidHistory>();

        public Listing(Guid listingId, int lister, DateTime timeStamp, int price, int[] cards)
        {
            ListingId = listingId;
            Lister = lister;
            TimeStamp = timeStamp;
            Price = price;
            Cards = cards;
        }

        public void AddBuyerBid(int buyer, int offeredAmount)
        {
            BidHistories.TryGetValue(buyer, out var bidderHistory);

            if (bidderHistory == null)
            {
                bidderHistory = new BidHistory(Lister, buyer);
                bidderHistory.AddBid(offeredAmount);
                BidHistories.Add(buyer, bidderHistory);
            }
            else
                bidderHistory.AddBid(offeredAmount);
        }
        
        public void AddListerBid(int buyer, int offeredAmount)
        {
            BidHistories.TryGetValue(buyer, out var bidderHistory);

            if (bidderHistory == null)
                return;

            bidderHistory.AddBid(offeredAmount, true);
        }

        public override string ToString()
        {
            return $"ListingId: {ListingId.ToString()}, ListerId: {Lister}, Timestamp: {TimeStamp}, Price: {Price}";
        }
    }
}