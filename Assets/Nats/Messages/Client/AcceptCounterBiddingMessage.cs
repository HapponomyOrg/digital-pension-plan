namespace NATS
{
    public class AcceptCounterBiddingMessage : BaseMessage
    {
        public string AuctionID;
        public int OfferAmount;
        public int CounterBidderID;
        

        public AcceptCounterBiddingMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID, int offerAmount, int counterBidderID) : base(dateTimeStamp, MessageSubject.AcceptCounterBidding, lobbyID, playerID)
        {
            AuctionID = auctionID;
            OfferAmount = offerAmount;
            CounterBidderID = counterBidderID;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}, OfferAmount: {OfferAmount}, CounterBidderID: {CounterBidderID}";
        }
    }
}