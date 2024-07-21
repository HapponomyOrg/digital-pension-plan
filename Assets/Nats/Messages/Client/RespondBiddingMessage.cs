namespace NATS
{
    public class RespondBiddingMessage : BaseMessage
    {
        public string AuctionID;
        public string PlayerName;
        public int BidderID;
        public int OriginalOfferPrice;
        public int CounterOfferPrice;

        public RespondBiddingMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID, string playername, int bidderID, int originalOfferPrice, int counterOfferPrice) : base(dateTimeStamp, MessageSubject.RespondBidding, lobbyID, playerID)
        {
            AuctionID = auctionID;
            PlayerName = playername;
            BidderID = bidderID;
            OriginalOfferPrice = originalOfferPrice;
            CounterOfferPrice = counterOfferPrice;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}, PlayerName: {PlayerName}, BidderID:  {BidderID} CounterOfferPrice: {CounterOfferPrice}";
        }
    }
}