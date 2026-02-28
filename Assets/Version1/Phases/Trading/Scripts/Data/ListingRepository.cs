using System;
using System.Collections.Generic;
using System.Linq;

namespace Version1.Market
{
    public class ListingRepository : IListingRepository
    {
        private readonly Dictionary<Guid, Listing> listings = new();
        private readonly Dictionary<int, HashSet<Guid>> playerListingIds = new();

        public bool AddListing(Listing listing)
        {
            var success = listings.TryAdd(listing.ListingId, listing);

            if (!success)
                return false;

            if (playerListingIds.TryGetValue(listing.Lister, out var guids))
                guids.Add(listing.ListingId);
            else
                playerListingIds.Add(listing.Lister, new HashSet<Guid>() { listing.ListingId });

            return success;
        }

        public Listing GetListing(Guid listingId)
        {
            if (listings.TryGetValue(listingId, out var result))
                return result;

            return null;
        }

        public Listing[] GetListings()
        {
            return listings.Values.ToArray();
        }

        public Listing[] GetPeerListings(int playerId)
        {
            if (!playerListingIds.ContainsKey(playerId))
                return GetListings();

            var listingIds = new HashSet<Guid>(listings.Keys);
            listingIds.ExceptWith(playerListingIds[playerId]);

            var peerListings = listingIds.Select(id => listings[id]).ToArray();

            return peerListings;
        }

        public Listing[] GetPersonalListings(int playerId)
        {
            if (!playerListingIds.ContainsKey(playerId))
                return Array.Empty<Listing>();

            var listingIds = playerListingIds[playerId];

            var personalListings = listingIds.Select(id => listings[id]).ToArray();

            return personalListings;
        }

        public void RemoveListing(Listing listing)
        {
            listings.Remove(listing.ListingId);

            if (playerListingIds.TryGetValue(listing.Lister, out var playerListings))
                playerListings.Remove(listing.ListingId);
        }

        public bool UpdateListing(Listing listing)
        {
            if (!listings.ContainsKey(listing.ListingId))
                return false;

            listings[listing.ListingId] = listing;
            return true;
        }

        public void Clear()
        {
            listings.Clear();
            playerListingIds.Clear();
        }
    }
}
