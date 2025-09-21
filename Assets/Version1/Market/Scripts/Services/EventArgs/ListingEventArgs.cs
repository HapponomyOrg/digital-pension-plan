using System;

namespace Version1.Market
{
    public class ListingEventArgs : EventArgs
    {
        public Listing Listing;

        public ListingEventArgs(Listing listing)
        {
            Listing = listing;
        }
    }
}