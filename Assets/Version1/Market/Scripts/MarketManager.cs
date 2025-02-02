using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Version1.Market
{
    // TODO error handling
    public class MarketManager
    {
        private readonly Dictionary<Guid, Listing> listings = new Dictionary<Guid, Listing>();

        public void AddListing(int listerId, DateTime timestamp, int price, int[] cards)
        {
            var listingId = new Guid();
            var listing = new Listing(listingId, listerId, timestamp, price, cards);

            if (listings.ContainsKey(listingId))
                return;
            
            listings.Add(listingId, listing);
        }
        
        public void AddListing(Listing listing)
        {
            if (listings.ContainsKey(listing.ListingId))
                return;
            
            listings.Add(listing.ListingId, listing);
        }

        public void RemoveListing(Guid listingId)
        {
            if (!listings.ContainsKey(listingId))
                return;

            listings.Remove(listingId);
        }

        public void AddBidToListing(Listing listing, int buyer, int offeredPrice, bool listerBid = false)
        {
            if (listing == null)
                return;
            
            if (listerBid)
                listing.AddListerBid(buyer, offeredPrice);
            else
                listing.AddBuyerBid(buyer, offeredPrice);
        }
        
        public void RemoveBidFromListing(Guid listingId, int bidder, int offeredPrice)
        {
            var listing = listings[listingId];

            if (listing == null)
                return;
            
        }

        // The player's own listings
        public Listing[] PersonalListings(int playerId)
        {
            return listings.Values.Where(l => l.Lister == playerId).OrderBy(l => l.TimeStamp).ToArray();
        }

        // Other player's listings
        public Listing[] PeerListings(int playerId)
        {
            return listings.Values.Where(l => l.Lister != playerId).OrderBy(l => l.TimeStamp).ToArray();
        }
    }
}