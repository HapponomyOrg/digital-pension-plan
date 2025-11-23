namespace Version1.Nats.Messages.Client
{
    public class RejectBidMessage : BaseMessage
    {
        public string AuctionID;
        public string BidID;
        public int BidderID;
        public int OriginalBidderID;
        public string PlayerName;

        public RejectBidMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID, string bidID, int bidderID, int originalBidder, string playerName) : base(dateTimeStamp, MessageSubject.RejectBid, lobbyID, playerID)
        {
            AuctionID = auctionID;
            BidID = bidID;
            BidderID = bidderID;
            OriginalBidderID = originalBidder;
            PlayerName = playerName;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}, PlayerName: {PlayerName}, BidderID: {BidderID}";
        }
    }
}
