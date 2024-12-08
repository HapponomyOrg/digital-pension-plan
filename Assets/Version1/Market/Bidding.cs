using System;

namespace Version1.Market
{
    public struct Bidding
    {
        public Guid BidId { get; private set; }
        public int OfferedPrice { get; private set; }
        public BidStatus BidStatus { get; private set; }
        public DateTime TimeStamp { get; private set; }

        public Bidding(Guid bidId, int offeredPrice, DateTime timeStamp)
        {
            BidId = bidId;
            OfferedPrice = offeredPrice;
            BidStatus = BidStatus.Active;
            TimeStamp = timeStamp;
        }

        public void AcceptBidding()
        {
            BidStatus = BidStatus.Accepted;
        }
        
        public void RejectBidding()
        {
            BidStatus = BidStatus.Rejected;
        }
    }
}