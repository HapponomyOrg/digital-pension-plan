using System;

namespace Version1.Market
{
    public class BidEventArgs : EventArgs
    {
        public Listing Listing { get; }
        public Bid Bid { get; }

        public BidEventArgs(Listing listing, Bid bid)
        {
            Listing = listing;
            Bid = bid;
        }
    }
}