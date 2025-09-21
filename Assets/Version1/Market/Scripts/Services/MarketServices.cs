namespace Version1.Market
{
    public class MarketServices
    { 
        public CreateListingService CreateListingService { get; }
        public BuyListingService BuyListingService { get; }
        public CancelListingService CancelListingService { get; }
        //public EditListingService EditListingService { get; }

        public CreateBidService CreateBidService { get; }
        public CancelBidService CancelBidService { get; }
        public RejectBidService RejectBidService { get; }
        public AcceptBidService AcceptBidService { get; }
        public CounterBidService CounterBidService { get; }
    }
}