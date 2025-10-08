namespace Version1.Nats.Messages.Client
{
    public class RejectBiddingMessage : BaseMessage
    {
        public string AuctionID;
        public string PlayerName;

        public int BidderID;

        public RejectBiddingMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID, string playername, int bidderID) : base(dateTimeStamp, MessageSubject.RejectBidding, lobbyID, playerID)
        {
            AuctionID = auctionID;
            PlayerName = playername;
            BidderID = bidderID;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}, PlayerName: {PlayerName}, BidderID: {BidderID}";
        }
    }
}
