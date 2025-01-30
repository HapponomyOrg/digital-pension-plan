namespace Version1.Nats.Messages.Client
{
    public class BuyCardsRequestMessage : BaseMessage
    {
        public string AuctionID;

        public BuyCardsRequestMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID) : base(dateTimeStamp, MessageSubject.BuyCards, lobbyID, playerID)
        {
            AuctionID = auctionID;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}";
        }
    }
}