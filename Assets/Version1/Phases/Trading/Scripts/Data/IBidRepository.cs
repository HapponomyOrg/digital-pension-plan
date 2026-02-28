using System;
using System.Collections.Generic;

namespace Version1.Market
{
    public interface IBidRepository
    {
        /// <summary>
        /// Retrieves all bid histories between the owner and each player.
        /// </summary>
        /// <returns>A dictionary mapping player IDs to their respective bid lists.</returns>
        public Dictionary<int, LinkedList<Bid>> GetAllBids();

        /// <summary>
        /// Retrieves the most recent bid between the owner and each player.
        /// </summary>
        /// <returns>A dictionary mapping player IDs to their latest bids.</returns>
        public Dictionary<int, Bid> GetLastBids();

        /// <summary>
        /// Retrieves a specific bid made between the owner and a player, identified by its bid ID.
        /// </summary>
        /// <param name="playerId">The ID of the player who made the original bid.</param>
        /// <param name="bidId">The unique ID of the bid.</param>
        /// <returns>The matching bid if found; otherwise, null.</returns>
        public Bid GetBidBetweenPlayer(int playerId, Guid bidId);

        /// <summary>
        /// Retrieves all bids made between the owner and a player.
        /// </summary>
        /// <param name="playerId">The ID of the player who made the original bid.</param>
        /// <returns>A linked list of bids made between the owner and the player, or an empty list if none exist.</returns>
        public LinkedList<Bid> GetBidsBetweenPlayer(int playerId);

        /// <summary>
        /// Retrieves the most recent bid made between the owner and a player.
        /// </summary>
        /// <param name="playerId">The ID of the player who made the original bid.</param>
        /// <returns>The latest bid made between the owner and the player, or null if none exist.</returns>
        public Bid GetLastBidBetweenPlayer(int playerId);

        /// <summary>
        /// Removes a specific bid from a bid history between the owner and a player.
        /// </summary>
        /// <param name="playerId">The ID of the player who made original the bid.</param>
        /// <param name="bidId">The ID of the bid to remove.</param>
        public void RemoveBidBetweenPlayer (int playerId, Guid bidId);

        /// <summary>
        /// Removes the most recent bid from a bid history between the owner and a player.
        /// </summary>
        /// <param name="playerId">The ID of the player who made original the bid.</param>
        public void RemoveLastBidBetweenPlayer(int playerId);

        /// <summary>
        /// Adds a new bid between the owner and the specified player.
        /// </summary>
        /// <param name="playerId">The ID of the player placing the original bid.</param>
        /// <param name="bid">The bid to add.</param>
        /// <returns>True if the bid was successfully added.</returns>
        public bool AddBid(int playerId, Bid bid);

        public void UpdateBidStatus(int playerId, Guid bidId, EBidStatus bidStatus);

        public int GetUniqueBidderCount();
        public int[] GetUniqueBidders();

        public int GetBidOwner(Guid bidId);
    }
}