using System;
using System.Linq;

namespace Version1.Market
{
    public class Listing
    {
        public Guid ListingId { get; private set; }
        public int Lister { get; private set; }
        public string ListerName { get; private set; }
        public int Price { get; private set; }
        public DateTime TimeStamp { get; private set; }        
        public int[] Cards { get; private set; }
        public IBidRepository BidRepository { get; private set; }

        public Listing(Guid listingId, int lister, string listerName, DateTime timeStamp, int price, int[] cards)
        {
            ListingId = listingId;
            Lister = lister;
            ListerName = listerName;
            TimeStamp = timeStamp;
            Price = price;
            Cards = cards.OrderByDescending(c => c).ToArray();
            BidRepository = new BidRepository();
        }
        
        public override string ToString()
        {
            return $"ListingId: {ListingId.ToString()}, ListerId: {Lister}, Timestamp: {TimeStamp}, Price: {Price}";
        }

        public int GetHighestCard()
        {
            return Cards.Max();
        }
    }
}
