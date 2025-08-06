using System;
using System.Collections.Generic;

namespace Version1.Market
{
    public interface IBidRepository
    {
        public Dictionary<int, LinkedList<Bid>> GetAllBids();
        public Dictionary<int, Bid> GetLastBids();
        public Bid GetLastBidOfPlayer(int playerId);
        public LinkedList<Bid> GetBidsOfPlayer(int playerId);
        public bool RemoveBidOfPlayer (int playerId, Guid bidId);
        public bool RemoveLastBidOfPlayer(int playerId);
        public bool AddBid(int playerId, Bid bid);
        public bool AddCounterBid(int playerId, Bid bid);
        public int UpdateBid(int playerId, Bid bid);
    }
}