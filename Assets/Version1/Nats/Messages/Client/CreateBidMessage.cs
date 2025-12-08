namespace Version1.Nats.Messages.Client
{
    public class CreateBidMessage : BaseMessage
    {
        public string AuctionID;
        public string BidID;
        public int OriginalBidder;
        public string PlayerName;
        public int OfferPrice;
        public string BidDateTimeStamp;

        public CreateBidMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID, string bidID, int originalBidder, string playername, int offerprice, string bidDateTimeStamp) : base(dateTimeStamp, MessageSubject.CreateBid, lobbyID, playerID)
        {
            AuctionID = auctionID;
            BidID = bidID;
            OriginalBidder = originalBidder;
            PlayerName = playername;
            OfferPrice = offerprice;
            BidDateTimeStamp = bidDateTimeStamp;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}, BidID: {BidID}, PlayerName: {PlayerName}, OfferPrice: {OfferPrice}, BidDateTimeStamp: {BidDateTimeStamp}";
        }
    }
}