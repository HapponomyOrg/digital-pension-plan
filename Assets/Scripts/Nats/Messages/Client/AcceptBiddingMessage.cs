namespace NATS
{
    public class AcceptBiddingMessage : BaseMessage
    {
        public string AuctionID;
        public int OfferAmount;
        public int BidderID;
        

        public AcceptBiddingMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID, int offerAmount, int bidderID) : base(dateTimeStamp, MessageSubject.AcceptBidding, lobbyID, playerID)
        {
            AuctionID = auctionID;
            OfferAmount = offerAmount;
            BidderID = bidderID;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}, BidderID: {BidderID}, OfferAmount: {OfferAmount}";
        }
    }
}