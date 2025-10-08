namespace Version1.Nats.Messages.Client
{
    public class CancelListingMessage : BaseMessage
    {
        public string AuctionID;

        public CancelListingMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID) : base(dateTimeStamp, MessageSubject.CancelListing, lobbyID, playerID)
        {
            AuctionID = auctionID;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}";
        }
    }
}
