namespace Version1.Nats.Messages.Host
{
    public class ConfirmCancelListingMessage : BaseMessage
    {
        public string AuctionID;
        
        public ConfirmCancelListingMessage(string dateTime, int lobbyID, int playerID, string auctionID) : base(dateTime, MessageSubject.CancelListing, lobbyID, playerID)
        {
            AuctionID = auctionID;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}, AuctionID: {AuctionID}";
        }
    }
}