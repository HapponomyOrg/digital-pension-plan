namespace Version1.Nats.Messages.Client
{
    public class CounterBidMessage : BaseMessage
    {
        public string AuctionID;
        public string BidID;
        public int OriginalBidder;
        public string PlayerName;
        public int CounterOfferPrice;
        public string BidDateTimeStamp;

        public CounterBidMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID, string bidID, int originalBidder, string playerName, int counterOfferPrice, string bidDateTimeStamp) : base(dateTimeStamp, MessageSubject.RespondBidding, lobbyID, playerID)
        {
            AuctionID = auctionID;
            BidID = bidID;
            OriginalBidder = originalBidder;
            PlayerName = playerName;
            CounterOfferPrice = counterOfferPrice;
            BidDateTimeStamp = bidDateTimeStamp;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}, PlayerName: {PlayerName}, CounterOfferPrice: {CounterOfferPrice}";
        }
    }
}