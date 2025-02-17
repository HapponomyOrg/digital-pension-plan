using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Version1.Market.Scripts
{
    // TODO error handling
    public class MarketManager
    {
        private readonly Dictionary<Guid, Listing> listings = new();

        public event EventHandler MarketDataChanged;
        
        public void AddListing(int listerId, DateTime timestamp, int price, int[] cards)
        {
            var listingId = Guid.NewGuid();
            var listing = new Listing(listingId, listerId, timestamp, price, cards);

            if (listings.ContainsKey(listingId))
            {
                Debug.Log("guid already in market");
                return;
            }

            PlayerData.PlayerData.Instance.RemoveCards(listing.Cards);
            listings.Add(listingId, listing);
            MarketDataChanged?.Invoke(this, EventArgs.Empty);
            
            // TODO() networking
        }
        
        // Test method
        public void AddListing(Listing listing)
        {
            if (listings.ContainsKey(listing.ListingId))
                return;
            
            listings.Add(listing.ListingId, listing);
            MarketDataChanged?.Invoke(this, EventArgs.Empty);
            
            // TODO() networking
        }

        public void CancelListing(Listing listing)
        {
            if (!listings.ContainsKey(listing.ListingId))
                return;
            
            listings.Remove(listing.ListingId);
            PlayerData.PlayerData.Instance.AddCards(listing.Cards);
            MarketDataChanged?.Invoke(this, EventArgs.Empty);

            // TODO() networking
        }
        
        private void RemoveListing(Guid listingId)
        {
            if (!listings.ContainsKey(listingId))
                return;

            listings.Remove(listingId);
            MarketDataChanged?.Invoke(this, EventArgs.Empty);
            // TODO() networking
        }

        public void AddBidToListing(Listing listing, int buyer, int offeredPrice, bool listerBid = false)
        {
            if (listing == null)
                return;
            
            if (listerBid)
                listing.AddListerBid(buyer, offeredPrice);
            else
                listing.AddBuyerBid(buyer, offeredPrice);
            
            PlayerData.PlayerData.Instance.Balance -= offeredPrice;
            
            MarketDataChanged?.Invoke(this, EventArgs.Empty);
            // TODO() networking
        }
        
        public void RemoveBidFromListing(Guid listingId, Guid bidId, int bidder)
        {
            var listing = listings[listingId];

            if (listing == null)
                return;

            var bid = listing.BidHistories[bidder].LastActiveBid();
            
            if (bid == null)
                return;

            PlayerData.PlayerData.Instance.Balance += bid.Value.OfferedPrice;
            listing.BidHistories.Remove(bidder);
            
            
            //listing.BidHistories[bidder].CancelBid(bidId);
            
            MarketDataChanged?.Invoke(this, EventArgs.Empty);
            // TODO() networking
        }


        public void BuyListing(Guid listingId)
        {
            var listing = listings[listingId];
            
            if (PlayerData.PlayerData.Instance.Balance < listing.Price)
                return;

            PlayerData.PlayerData.Instance.Balance -= listing.Price;
            PlayerData.PlayerData.Instance.AddCards(listing.Cards);


            RemoveListing(listingId);

            MarketDataChanged?.Invoke(this, EventArgs.Empty);
            // TODO() Add networking
        }

        // TODO() Invoke when receiving buy message
        public void SellListing(Guid listingId)
        {
            var listing = listings[listingId];
            
            PlayerData.PlayerData.Instance.Balance += listing.Price;

            RemoveListing(listingId);

            MarketDataChanged?.Invoke(this, EventArgs.Empty);
        }
        
        public Listing[] PersonalListings(int playerId)
        {
            return listings.Values.Where(l => l.Lister == playerId).OrderBy(l => l.TimeStamp).ToArray();
        }
        
        public Listing[] PeerListings(int playerId)
        {
            return listings.Values.Where(l => l.Lister != playerId).OrderBy(l => l.TimeStamp).ToArray();
        }

        public Listing[] PeerListingsWithoutBid(int playerId)
        {
            return listings.Values.Where(l => l.Lister != playerId).Where(l => !l.BidHistories.ContainsKey(playerId)).ToArray();
        }
        
        /*public Listing[] PeerListingsWithoutBid(int playerId)
        {
            var ls = new List<Listing>();

            foreach (var listing in listings.Values)
            {
                if (listing.Lister == playerId)
                    continue;

                if (listing.BidHistories.TryGetValue(playerId, out var history))
                    if (history.LastActiveBid() == null)
                        continue;
                
                ls.Add(listing);
            }

            return ls.ToArray();
        }*/
        
        public Listing[] PeerListingsWithBid(int playerId)
        {
            return listings.Values.Where(l => l.Lister != playerId).Where(l => l.BidHistories.ContainsKey(playerId)).ToArray();
        }
        
        /*public Listing[] PeerListingsWithBid(int playerId)
        {
            var ls = new List<Listing>();
        
            foreach (var listing in listings.Values)
            {
                if (listing.Lister == playerId)
                    continue;
        
                if (!listing.BidHistories.TryGetValue(playerId, out var history)) 
                    continue;
                
                if (history.LastActiveBid() != null)
                    ls.Add(listing);
            }
        
            return ls.ToArray();
        }*/
        
    }
}