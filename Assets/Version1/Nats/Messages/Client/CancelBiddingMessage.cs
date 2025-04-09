namespace Version1.Nats.Messages.Client
{
    public class CancelBiddingMessage : BaseMessage
    {
        public string AuctionID;
        public string BidID;
        public int OriginalBidder;

        public CancelBiddingMessage(
            string dateTimeStamp, 
            int lobbyID, 
            int playerID, 
            string auctionID, 
            string bidID, 
            int originalBidder) : base(dateTimeStamp, MessageSubject.CancelBidding, lobbyID, playerID)
        {
            AuctionID = auctionID;
            BidID = bidID;
            OriginalBidder = originalBidder;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}";
        }
    }
}