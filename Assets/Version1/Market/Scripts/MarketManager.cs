using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Version1.Host.Scripts;
using Version1.Nats.Messages.Client;

namespace Version1.Market.Scripts
{
    // TODO error handling
    public class MarketManager
    {
        private readonly Dictionary<Guid, Listing> listings = new();

        public event EventHandler MarketDataChanged;
        public event EventHandler<string> ListingRemoved;

        public MarketManager()
        {
            // Nats.NatsClient.C.OnBuyCards += (sender, message) => { HandleBuyCardsMessage(message); };
            // Nats.NatsClient.C.OnCancelListing += (sender, message) => { HandleCancelListingMessage(message); };
        }

        private void HandleBuyCardsMessage(BuyCardsRequestMessage message)
        {
            var guid = Guid.Parse(message.AuctionID);
            var listing = listings[guid];
            
            if (listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
                SellListing(guid);
            
            RemoveListing(guid);
        }

        public void HandleAddListingMessage(ListCardsmessage message)
        {
            var guid = Guid.Parse(message.AuctionID);
            
            AddListing(guid, message.PlayerID, DateTime.Parse(message.DateTimeStamp),  message.Amount, message.Cards);
        }
        
        private void AddListing(Guid listingId, int listerId, DateTime timestamp, int price, int[] cards)
        {
            var listing = new Listing(listingId, listerId, timestamp, price, cards);

            if (listings.ContainsKey(listingId))
            {
                Debug.Log("guid already in market");
                return;
            }

            PlayerData.PlayerData.Instance.RemoveCards(listing.Cards);
            listings.Add(listingId, listing);
            MarketDataChanged?.Invoke(this, EventArgs.Empty);
            //MarketDataChanged?.Invoke(this, EventArgs.Empty);
        }
        
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

            //MarketDataChanged?.Invoke(this, EventArgs.Empty);
            
            // TODO() networking
            var data = PlayerData.PlayerData.Instance;
            
            var message = new ListCardsmessage(DateTime.Now.ToString("o"), data.LobbyID, listerId, data.PlayerName, listingId.ToString(), cards, price);
            Nats.NatsClient.C.Publish(data.LobbyID.ToString(), message);
        }
        
        // Test method
        public void AddListing(Listing listing)
        {
            if (listings.ContainsKey(listing.ListingId))
                return;
            
            listings.Add(listing.ListingId, listing);
            MarketDataChanged?.Invoke(this, EventArgs.Empty);
        }

        public void HandleCancelListingMessage(CancelListingMessage message)
        {
            var guid = Guid.Parse(message.AuctionID);
            
            RemoveListing(guid);
        }
        
        public void CancelListing(Listing listing)
        {
            if (!listings.ContainsKey(listing.ListingId))
                return;
            
            PlayerData.PlayerData.Instance.AddCards(listing.Cards);
            RemoveListing(listing.ListingId);
            
            var message = new CancelListingMessage(
                DateTime.Now.ToString("o"), 
                PlayerData.PlayerData.Instance.LobbyID,
                PlayerData.PlayerData.Instance.PlayerId,
                listing.ListingId.ToString()
                );
            
            Nats.NatsClient.C.Publish(PlayerData.PlayerData.Instance.LobbyID.ToString(), message);        
        }
        
        private void RemoveListing(Guid listingId)
        {
            if (!listings.ContainsKey(listingId))
                return;

            Debug.LogWarning("removed listing");
            
            listings.Remove(listingId);
            ListingRemoved?.Invoke(this, listingId.ToString());
            //MarketDataChanged?.Invoke(this, EventArgs.Empty);
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
            
            // TODO() networking
            var data = PlayerData.PlayerData.Instance;
            
            var message = new BuyCardsRequestMessage(
                DateTime.Now.ToString("o"), 
                data.LobbyID,
                data.PlayerId,
                listingId.ToString()
            );
            Nats.NatsClient.C.Publish(data.LobbyID.ToString(), message);       
        }

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