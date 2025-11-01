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

        public MarketServices()
        {
            CreateListingService = new CreateListingService();
            BuyListingService = new BuyListingService();
            CancelListingService = new CancelListingService();

            CreateBidService = new CreateBidService();
            CancelBidService = new CancelBidService();
            RejectBidService = new RejectBidService();
            AcceptBidService = new AcceptBidService();
            CounterBidService = new CounterBidService();
        }
    }
}