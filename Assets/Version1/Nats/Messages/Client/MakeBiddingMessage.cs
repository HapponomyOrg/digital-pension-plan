namespace Version1.Nats.Messages.Client
{
    public class MakeBiddingMessage : BaseMessage
    {
        public string AuctionID;
        public string BidID;
        public int OriginalBidder;
        public string PlayerName;
        public int OfferPrice;

        public MakeBiddingMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID, string bidID, int originalBidder, string playername, int offerprice) : base(dateTimeStamp, MessageSubject.MakeBidding, lobbyID, playerID)
        {
            AuctionID = auctionID;
            BidID = bidID;
            OriginalBidder = originalBidder;
            PlayerName = playername;
            OfferPrice = offerprice;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}, BidID: {BidID}, PlayerName: {PlayerName}, OfferPrice: {OfferPrice}";
        }
    }
}