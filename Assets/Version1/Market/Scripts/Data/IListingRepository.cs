using System;

namespace Version1.Market
{
    public interface IListingRepository
    {
        public Listing GetListing(Guid listingId);
        public Listing[] GetPersonalListings(int playerId);

        public Listing[] GetPeerListings(int playerId);
        public Listing[] GetListings();
        public bool RemoveListing(Guid listingId);
        public bool AddListing(Listing listing);
        public bool UpdateListing(Listing listing);
    }
}