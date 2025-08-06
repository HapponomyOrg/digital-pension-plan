using System;
using System.Collections.Generic;

namespace Version1.Market
{
    public class BidRepository : IBidRepository
    {
        private Dictionary<int, LinkedList<Bid>> bids;

        public bool AddBid(int playerId, Bid bid)
        {
            throw new NotImplementedException();
        }

        public bool AddCounterBid(int playerId, Bid bid)
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, LinkedList<Bid>> GetAllBids()
        {
            throw new NotImplementedException();
        }

        public LinkedList<Bid> GetBidsOfPlayer(int playerId)
        {
            throw new NotImplementedException();
        }

        public Bid GetLastBidOfPlayer(int playerId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, Bid> GetLastBids()
        {
            throw new NotImplementedException();
        }

        public bool RemoveBidOfPlayer(int playerId, Guid bidId)
        {
            throw new NotImplementedException();
        }

        public bool RemoveLastBidOfPlayer(int playerId)
        {
            throw new NotImplementedException();
        }

        public int UpdateBid(int playerId, Bid bid)
        {
            throw new NotImplementedException();
        }
    }
}