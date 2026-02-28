using System;

namespace Version1.Market
{
    public class Bid
    {
        public Guid BidId { get; private set; }
        public int Bidder { get; private set; }
        public string BidderName { get; private set; }
        public int BidOffer { get; private set; }
        public EBidStatus BidStatus { get; set; }
        public DateTime TimeStamp { get; private set; }

        public Bid(Guid bidId, int bidder, string bidderName, int bidOffer, DateTime timeStamp)
        {
            BidId = bidId;
            Bidder = bidder;
            BidderName = bidderName;
            BidOffer = bidOffer;
            BidStatus = EBidStatus.Active;
            TimeStamp = timeStamp;
        }

        public override string ToString()
        {
            return $"BidId: {BidId}, Bidder: {Bidder}, OfferedPrice: {BidOffer}, BidStatus: {BidStatus}, Timestamp: {TimeStamp}";
        }
    }
}