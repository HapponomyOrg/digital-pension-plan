using System;

namespace NATS
{
    [Obsolete]
    public class MakeBiddingMessage : BaseMessage
    {
        public string AuctionID;
        public string PlayerName;
        public int OfferPrice;

        public MakeBiddingMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID, string playername, int offerprice) : base(dateTimeStamp, MessageSubject.MakeBidding, lobbyID, playerID)
        {
            AuctionID = auctionID;
            PlayerName = playername;
            OfferPrice = offerprice;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}, PlayerName: {PlayerName}, OfferPrice: {OfferPrice}";
        }
    }
}