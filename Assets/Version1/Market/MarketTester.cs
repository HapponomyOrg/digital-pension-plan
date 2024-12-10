using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Version1.Market
{
    public class MarketTester : MonoBehaviour
    {
        private MarketManager marketManager = new MarketManager();

        private Listing[] testListings = 
        {
            new Listing(Guid.NewGuid(), 0, new DateTime(2002, 10, 5, 10, 15, 22), 4500, new[] { 2, 1, 4, 3 }),
            /*new Listing(Guid.NewGuid(), 1, new DateTime(2002, 10, 5, 12, 30, 40), 5200, new[] { 3, 2, 5, 1 }),
            new Listing(Guid.NewGuid(), 2, new DateTime(2002, 10, 5, 14, 45, 10), 4800, new[] { 4, 3, 6, 2 }),
            new Listing(Guid.NewGuid(), 3, new DateTime(2002, 10, 5, 9, 10, 5), 5000, new[] { 5, 4, 7, 3 }),
            new Listing(Guid.NewGuid(), 0, new DateTime(2002, 10, 5, 15, 20, 30), 4900, new[] { 1, 2, 3, 4 }),
            new Listing(Guid.NewGuid(), 1, new DateTime(2002, 10, 5, 11, 5, 50), 4600, new[] { 6, 1, 2, 3 }),
            new Listing(Guid.NewGuid(), 2, new DateTime(2002, 10, 5, 16, 30, 15), 5300, new[] { 7, 3, 5, 2 }),
            new Listing(Guid.NewGuid(), 3, new DateTime(2002, 10, 5, 17, 20, 45), 4800, new[] { 8, 4, 6, 1 }),
            new Listing(Guid.NewGuid(), 0, new DateTime(2002, 10, 5, 18, 15, 25), 5100, new[] { 3, 5, 2, 6 }),
            new Listing(Guid.NewGuid(), 1, new DateTime(2002, 10, 5, 19, 50, 35), 4700, new[] { 1, 6, 3, 5 }),*/
        };

        private Tuple<int, int, int>[] testBids =
        {
            new (1, 1, 2000),
            new (0, 1, 8000),
            new (1, 1, 5000),
            new (0, 1, 6000),
            
            new (5, 5, 3000),
            new (0, 5, 7000),
            new (5, 5,4000),
            
            new (8, 8, 3000),
            new (0, 8, 7000),
            new (8, 8,4000),
            
            
            new (9, 9, 3000),
            new (0, 9, 7000),
            new (9, 9,4000),
            
            new (2, 2, 3000),
            new (0, 2, 7000),
            new (2, 2,4000),
        };

        private void Start()
        {
            foreach (var listing in testListings)
            {
                marketManager.AddListing(listing);
                
                foreach (var bid in testBids)
                {
                    marketManager.AddBidToListing(listing, bid.Item2, bid.Item3, bid.Item1 == 0);
                }
            }
            
            Debug.Log("Finished");
        }

        public void GetPlayerListings()
        {
            var listings = marketManager.PlayerListings(0);
            PrintListings(listings);
        }
        
        public void GetNonPlayerListings()
        {
            var listings = marketManager.NonPlayerListings(0);
            PrintListings(listings);
        }

        private void PrintListings(Listing[] listings)
        {
            foreach (var listing in listings)
            {
                Debug.Log($"<color=#ffff00>{listing}</color>");

                foreach (var bidders in listing.BidHistories.Keys)
                {
                    PrintListingBidHistory(listing, bidders);
                }
            }
        }

        private void PrintListingBidHistory(Listing listing, int bidder)
        {
            var bidHistory = listing.BidHistories[bidder];

            if (bidHistory == null)
                return;

            var color = $"#{Random.Range(0x555555, 0xFFFFFF):X6}";
            
            foreach (var bid in bidHistory.GetSortedBiddingHistory())
            {
                Debug.Log($"<color={color}>{bid}</color>");
            }
        }
    }
}
