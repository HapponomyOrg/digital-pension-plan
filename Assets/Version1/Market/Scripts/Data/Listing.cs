using System;

namespace Version1.Market
{
    public class Listing
    {
        public Guid ListingId { get; private set; }
        public int Lister { get; private set; }
        public int Price { get; private set; }
        public DateTime TimeStamp { get; private set; }        
        public int[] Cards { get; private set; }
        public IBidRepository BidRepository { get; private set; }
        
        public Listing(Guid listingId, int lister, DateTime timeStamp, int price, int[] cards)
        {
            ListingId = listingId;
            Lister = lister;
            TimeStamp = timeStamp;
            Price = price;
            Cards = cards;
            BidRepository = new BidRepository();
        }
        
        public override string ToString()
        {
            return $"ListingId: {ListingId.ToString()}, ListerId: {Lister}, Timestamp: {TimeStamp}, Price: {Price}";
        }
    }
}