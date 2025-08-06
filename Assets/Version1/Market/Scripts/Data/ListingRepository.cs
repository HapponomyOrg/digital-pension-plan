using System;
using System.Collections.Generic;

namespace Version1.Market
{
    public class ListingRepository : IListingRepository
    {
        private Dictionary<Guid, Listing> listings;
        private Dictionary<int, HashSet<Guid>> playerListingIds;


        public bool AddListing(Listing listing)
        {
            throw new NotImplementedException();
        }

        public Listing GetListing(Guid listingId)
        {
            throw new NotImplementedException();
        }

        public Listing[] GetListings()
        {
            throw new NotImplementedException();
        }

        public Listing[] GetPeerListings(int playerId)
        {
            throw new NotImplementedException();
        }

        public Listing[] GetPersonalListings(int playerId)
        {
            throw new NotImplementedException();
        }

        public bool RemoveListing(Guid listingId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateListing(Listing listing)
        {
            throw new NotImplementedException();
        }
        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}