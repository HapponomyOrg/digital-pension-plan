using System;
using System.Collections.Generic;
using System.Linq;

namespace Version1.Market
{
    public class BidRepository : IBidRepository
    {
        private readonly Dictionary<int, LinkedList<Bid>> bids = new();

        public bool AddBid(int playerId, Bid bid)
        {
            var hasBidHistory = bids.ContainsKey(playerId);

            if (!hasBidHistory)
                bids[playerId] = new LinkedList<Bid>();

            bids[playerId].AddLast(bid);

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

        public void RemoveBidBetweenPlayer(int playerId, Guid bidId)
        {
            var playerExists = bids.TryGetValue(playerId, out var playerBids);

            if (!playerExists)
                return;
            
            var bid = GetBidBetweenPlayer(playerId, bidId);
            if (bid == null)
                return;

            playerBids.Remove(bid);
        }

        public void RemoveLastBidBetweenPlayer(int playerId)
        {
            var playerExists = bids.TryGetValue(playerId, out var playerBids);

            if (!playerExists)
                return;

            if (playerBids.Count > 0)
                playerBids.RemoveLast();
        }
    }
}