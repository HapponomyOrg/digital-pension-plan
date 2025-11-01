using System;
using System.Collections.Generic;
using System.Linq;

namespace Version1.Market
{
    public class BidRepository : IBidRepository
    {

        private readonly Dictionary<int, LinkedList<Bid>> bids = new();

        private readonly Dictionary<Guid, int> bidOwners = new();

        public bool AddBid(int playerId, Bid bid)
        {
            var hasBidHistory = bids.ContainsKey(playerId);

            if (!hasBidHistory)
                bids[playerId] = new LinkedList<Bid>();

            bids[playerId].AddLast(bid);
            bidOwners.Add(bid.BidId, bid.Bidder);

            return true;
        }

        public Dictionary<int, LinkedList<Bid>> GetAllBids()
        {
            return new Dictionary<int, LinkedList<Bid>>(bids);
        }

        public Bid GetBidBetweenPlayer(int playerId, Guid bidId)
        {
            var playerExists = bids.TryGetValue(playerId, out var playerBids);

            if (!playerExists)
                return null;

            var bid = playerBids.FirstOrDefault(b => b.BidId == bidId);

            return bid;
        }

        public LinkedList<Bid> GetBidsBetweenPlayer(int playerId)
        {
            if (!bids.ContainsKey(playerId))
                return new LinkedList<Bid>();

            return bids[playerId];
        }

        public Bid GetLastBidBetweenPlayer(int playerId)
        {
            if (!bids.ContainsKey(playerId))
                return null;
            
            return bids[playerId].Last.Value;
        }

        public Dictionary<int, Bid> GetLastBids()
        {
            var lastBids = new Dictionary<int, Bid>();

            foreach (var playerBids in bids)
                lastBids.Add(playerBids.Key, playerBids.Value.Last.Value);
            
            return lastBids;
        }

        public void UpdateBidStatus(int playerId, Guid bidId, EBidStatus bidStatus)
        {
            var playerExists = bids.TryGetValue(playerId, out var playerBids);

            if (!playerExists)
                return;

            var bid = GetBidBetweenPlayer(playerId, bidId);
            if (bid == null)
                return;

            bid.BidStatus = bidStatus;
        }

        public void RemoveBidBetweenPlayer(int playerId, Guid bidId)
        {
            var playerExists = bids.TryGetValue(playerId, out var playerBids);

            if (!playerExists)
                return;
            
            var bid = GetBidBetweenPlayer(playerId, bidId);
            if (bid == null)
                return;

            bidOwners.Remove(bid.BidId);
            playerBids.Remove(bid);
        }

        public void RemoveLastBidBetweenPlayer(int playerId)
        {
            var playerExists = bids.TryGetValue(playerId, out var playerBids);

            if (!playerExists)
                return;


            if (playerBids.Count > 0)
            {
                bidOwners.Remove(playerBids.Last.Value.BidId);
                playerBids.RemoveLast();
            }
        }

        public int GetUniqueBidderCount()
        {
            return bids.Count;
        }

        public int[] GetUniqueBidders()
        {
            return bids.Keys.ToArray();
        }

        public int GetBidOwner(Guid bidId)
        {
            if (!bidOwners.ContainsKey(bidId))
                return -1;

            return bidOwners[bidId];
        }
    }
}