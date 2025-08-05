using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

        public MarketManager()
        {
            Nats.NatsClient.C.OnListCards += (sender, message) =>
            {
                if (message.PlayerID == PlayerData.PlayerData.Instance.PlayerId)
                    return;
            
                Utilities.GameManager.Instance.MarketManager.HandleAddListingMessage(message);

            };
            
            Nats.NatsClient.C.OnCancelListing += (sender, message) =>
            {
                {
                    if (message.PlayerID == PlayerData.PlayerData.Instance.PlayerId)
                        return;
                    HandleCancelListingMessage(message);
                }
            };

            Nats.NatsClient.C.OnBuyCards += HandleBuyCardsMessage;

            Nats.NatsClient.C.OnMakeBidding += HandleReceiveBid;
            Nats.NatsClient.C.OnCancelBidding += HandleCancelBid;
            Nats.NatsClient.C.OnAcceptBidding += HandleAcceptBid;
        }


        #region AddListing
        
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

            listings.Add(listingId, listing);
            MarketDataChanged?.Invoke(this, EventArgs.Empty);
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

            
            var data = PlayerData.PlayerData.Instance;
            
            var message = new ListCardsmessage(DateTime.Now.ToString("o"), data.LobbyID, listerId, data.PlayerName, listingId.ToString(), cards, price);
            Nats.NatsClient.C.Publish(data.LobbyID.ToString(), message);
        }

        #endregion
        
        #region CancelListing
        
        public void HandleCancelListingMessage(CancelListingMessage message)
        {
            var guid = Guid.Parse(message.AuctionID);

            if (listings[guid].BidHistories.TryGetValue(PlayerData.PlayerData.Instance.PlayerId, out var bidHistory)) {
                var lastBid = bidHistory.LastActiveBid();

                if (lastBid != null && lastBid.Item2.Bidder == PlayerData.PlayerData.Instance.PlayerId)
                {
                    PlayerData.PlayerData.Instance.Balance += lastBid.Item2.OfferedPrice;
                }
                    
            }

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

        #endregion

        #region BuyListing

        public void HandleBuyCardsMessage(object sender, BuyCardsRequestMessage message)
        {
            var guid = Guid.Parse(message.AuctionID);
            var listing = listings[guid];
            
            if (listing.Lister == PlayerData.PlayerData.Instance.PlayerId)
                SellListing(guid);
            else
                RemoveListing(guid);
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
            
            var data = PlayerData.PlayerData.Instance;
            
            var message = new BuyCardsRequestMessage(
                DateTime.Now.ToString("o"), 
                data.LobbyID,
                data.PlayerId,
                listingId.ToString()
            );
            Nats.NatsClient.C.Publish(data.LobbyID.ToString(), message);       
        }

        private void SellListing(Guid listingId)
        {
            var listing = listings[listingId];
            
            PlayerData.PlayerData.Instance.Balance += listing.Price;

            RemoveListing(listingId);

            MarketDataChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        
        private void RemoveListing(Guid listingId)
        {
            if (!listings.ContainsKey(listingId))
                return;

            Debug.LogWarning("removed listing");
            
            listings.Remove(listingId);
            // ListingRemoved?.Invoke(this, listingId.ToString());
            MarketDataChanged?.Invoke(this, EventArgs.Empty);
        }

        
        
        
        
        #region Bids

        private void HandleReceiveBid(object sender, MakeBiddingMessage message)
        {
            var listing = listings[Guid.Parse(message.AuctionID)];

            if (listings == null)
            {
                Debug.LogWarning($"Listing: {message.AuctionID} does not exist for bid: {message}");
                return;
            }

            var playerId = PlayerData.PlayerData.Instance.PlayerId;

            if (playerId != message.OriginalBidder && playerId != listing.Lister)
                return;

            var bid = new Bid(Guid.Parse(message.BidID), message.PlayerID, message.PlayerName, message.OfferPrice, DateTime.Parse(message.DateTimeStamp));
            
            AddBidToListing(listing, bid, message.OriginalBidder);
        }
        
        // Sender (Buyer)
        public void AddBidToListing(Listing listing, int originalBidder, int offeredPrice)
        {
            if (listing == null)
            {
                Debug.Log("listing doesnt exist");
                return;
            }
            
            var bidId = Guid.NewGuid();
            var bid = new Bid(bidId, PlayerData.PlayerData.Instance.PlayerId, PlayerData.PlayerData.Instance.PlayerName, offeredPrice, DateTime.Now);
            
            // TODO decide when to subtract money from original bidder
            PlayerData.PlayerData.Instance.Balance -= offeredPrice;
            listing.AddBid(originalBidder, bid);
            MarketDataChanged?.Invoke(this, EventArgs.Empty);

            var data = PlayerData.PlayerData.Instance;
            var message = new MakeBiddingMessage(bid.TimeStamp.ToString("o"), data.LobbyID, data.PlayerId, listing.ListingId.ToString(), bid.BidId.ToString(), originalBidder, data.PlayerName, offeredPrice);

            Nats.NatsClient.C.Publish(PlayerData.PlayerData.Instance.LobbyID.ToString(), message);
        }
        
        // Receiver (Lister)
        private void AddBidToListing(Listing listing, Bid bid, int originalBidder)
        {
            if (listing == null)
            {
                Debug.Log("listing doesnt exist");
                return;
            }
            
            listing.AddBid(originalBidder, bid);

            MarketDataChanged?.Invoke(this, EventArgs.Empty);
            Debug.Log($"Bid added to listing {listing}");
        }


        private void HandleCancelBid(object sender, CancelBiddingMessage message)
        {
            var listing = listings[Guid.Parse(message.AuctionID)];

            if (listings == null)
            {
                Debug.LogWarning($"Listing: {message.AuctionID} does not exist for bid: {message}");
                return;
            }

            var playerId = PlayerData.PlayerData.Instance.PlayerId;
            
            if (playerId != message.OriginalBidder && playerId != listing.Lister)
                return;
            
            CancelBidFromListing(listing, Guid.Parse(message.BidID), message.OriginalBidder);
        }

        // Sender (Buyer)
        public void CancelBidFromListing(Listing listing, Bid bid, int originalBidder)
        {
            if (listing == null)
            {
                Debug.Log("listing doesnt exist");
                return;
            }
            
            listing.CancelBid(originalBidder, bid.BidId);
            
            // Restore balance
            PlayerData.PlayerData.Instance.Balance += bid.OfferedPrice;
            
            MarketDataChanged?.Invoke(this, EventArgs.Empty);

            var data = PlayerData.PlayerData.Instance;
            var message = new CancelBiddingMessage(DateTime.Now.ToString("o"), data.LobbyID, data.PlayerId, listing.ListingId.ToString(), bid.BidId.ToString(), originalBidder);

            Nats.NatsClient.C.Publish(PlayerData.PlayerData.Instance.LobbyID.ToString(), message);
        }
        
        
        // Receiver (Lister)
        private void CancelBidFromListing(Listing listing, Guid bidId, int originalBidder)
        {
            if (listing == null)
            {
                Debug.Log("listing doesnt exist");
                return;
            }
            
            Debug.Log($"Cancel bid: {bidId}");
            
            listing.CancelBid(originalBidder, bidId);

            MarketDataChanged?.Invoke(this, EventArgs.Empty);
        }
        
        
        
        private void HandleAcceptBid(object sender, AcceptBiddingMessage message)
        {
            Debug.Log($"Received {message}");

            
            var listing = listings[Guid.Parse(message.AuctionID)];

            if (listings == null)
            {
                Debug.LogWarning($"Listing: {message.AuctionID} does not exist for bid: {message}");
                return;
            }

            AcceptBidOnListing(listing, message.OriginalBidder);
        }

        // Sender (lister)
        public void AcceptBidOnListing(Listing listing, Bid bid, int originalBidder)
        {
            if (listing == null)
            {
                Debug.Log("listing doesnt exist");
                return;
            }

            PlayerData.PlayerData.Instance.Balance += bid.OfferedPrice;
            listings.Remove(listing.ListingId);
            
            MarketDataChanged?.Invoke(this, EventArgs.Empty);

            var data = PlayerData.PlayerData.Instance;
            var message = new AcceptBiddingMessage(DateTime.Now.ToString("o"), data.LobbyID, data.PlayerId, listing.ListingId.ToString(), bid.BidId.ToString(), originalBidder);

            Debug.Log($"Sent {message}");
            
            Nats.NatsClient.C.Publish(PlayerData.PlayerData.Instance.LobbyID.ToString(), message);
        }
        
        // Receiver (Buyer)
        private void AcceptBidOnListing(Listing listing, int originalBidder)
        {
            if (listing == null)
            {
                Debug.Log("listing doesnt exist");
                return;
            }

            foreach (var bidHistory in listing.BidHistories)
            {
                if (bidHistory.Key == originalBidder)
                    continue;

                if (bidHistory.Key != PlayerData.PlayerData.Instance.PlayerId)
                    continue;

                var bid = bidHistory.Value.LastActiveBid().Item2;
                
                if (bid.Bidder != PlayerData.PlayerData.Instance.PlayerId)
                    continue;
                
                PlayerData.PlayerData.Instance.Balance += bid.OfferedPrice;
            }

            if (PlayerData.PlayerData.Instance.PlayerId == originalBidder)
                PlayerData.PlayerData.Instance.AddCards(listing.Cards);

            listings.Remove(listing.ListingId);
            
            MarketDataChanged?.Invoke(this, EventArgs.Empty);
        }
        
        
        // public void RemoveBidFromListing(Guid listingId, Guid bidId, int bidder)
        // {
        //     var listing = listings[listingId];
        //
        //     if (listing == null)
        //         return;
        //
        //     var bid = listing.BidHistories[bidder].LastActiveBid();
        //     
        //     if (bid == null)
        //         return;
        //
        //     //PlayerData.PlayerData.Instance.Balance += bid.Value.OfferedPrice;
        //     listing.BidHistories.Remove(bidder);
        //     
        //     
        //     //listing.BidHistories[bidder].CancelBid(bidId);
        //     
        //     MarketDataChanged?.Invoke(this, EventArgs.Empty);
        //     // TODO() networking
        // }

        #endregion






        #region ListingFetchers

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
            var listingsWithoutActiveBid = new List<Listing>();

            foreach (var listing in listings.Values)
            {
                if (listing.Lister == playerId)
                    continue;

                // Either no bid history, or the bid is inactive
                if (!listing.BidHistories.TryGetValue(playerId, out var bidHistory) || bidHistory.LastActiveBid() == null)
                {
                    listingsWithoutActiveBid.Add(listing);
                }
            }

            return listingsWithoutActiveBid.ToArray();
        }

        public Listing[] PeerListingsWithBid(int playerId)
        {
            var listingsWithBid = new List<Listing>();

            foreach (var listing in listings.Values)
            {
                if (listing.Lister == playerId)
                    continue;

                if (!listing.BidHistories.TryGetValue(playerId, out var bidHistory) || bidHistory.LastActiveBid() == null)
                    continue;
                
                listingsWithBid.Add(listing);
            }

            return listingsWithBid.ToArray();
        }

        #endregion
    }
}