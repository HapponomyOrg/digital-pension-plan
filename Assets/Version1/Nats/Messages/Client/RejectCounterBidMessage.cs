namespace Version1.Nats.Messages.Client
{
    public class RejectCounterBidMessage : BaseMessage
    {
        public string AuctionID;

        public RejectCounterBidMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID) : base(dateTimeStamp, MessageSubject.AcceptCounterBidding, lobbyID, playerID)
        {
            AuctionID = auctionID;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , " +
                $"Lobby: {LobbyID} , " +
                $"Subject: {Subject} , " +
                $"Player: {PlayerID} , " +
                $"AuctionID: {AuctionID}, ";
        }
    }
}
