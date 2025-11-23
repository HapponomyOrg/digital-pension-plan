namespace Version1.Nats.Messages
{
    public class MessageSubject
    {
        // Client Gameplay
        public const string None = nameof(None);
        public const string Base = nameof(Base);
        public const string DonateMoney = nameof(DonateMoney);
        public const string DonatePoints = nameof(DonatePoints);
        public const string DeptUpdate = nameof(DeptUpdate);
        public const string CardHandIn = nameof(CardHandIn);
        public const string HeartBeat = nameof(HeartBeat);
        public const string Continue = nameof(Continue);
        // Listing
        public const string CreateListing = nameof(CreateListing);
        public const string CancelListing = nameof(CancelListing);
        public const string BuyListing = nameof(BuyListing);
        // Bid
        public const string CreateBid = nameof(CreateBid);
        public const string CancelBid = nameof(CancelBid);
        public const string AcceptBid = nameof(AcceptBid);
        public const string AcceptCounterBidding = nameof(AcceptCounterBidding);
        public const string CounterBid = nameof(CounterBid);
        public const string RejectBid = nameof(RejectBid);
        public const string RejectCounterBidding = nameof(RejectCounterBidding);
        // Client Misc.
        public const string JoinRequest = nameof(JoinRequest);


        // Host Hosting
        public const string CreateSession = nameof(CreateSession);
        public const string StartGame = nameof(StartGame);
        public const string StartRound = nameof(StartRound);
        public const string StopRound = nameof(StopRound);
        public const string EndOfRounds = nameof(EndOfRounds);
        public const string EndGame = nameof(EndGame);
        public const string AbortSession = nameof(AbortSession);
        public const string SkipRounds = nameof(SkipRounds);

        // Host Misc.
        public const string ConfirmJoin = nameof(ConfirmJoin);
        public const string Rejected = nameof(Rejected);
        public const string ConfirmBuy = nameof(ConfirmBuy);
        public const string ConfirmHandIn = nameof(ConfirmHandIn);
        public const string ConfirmCancelListing = nameof(ConfirmCancelListing);
    }
}
