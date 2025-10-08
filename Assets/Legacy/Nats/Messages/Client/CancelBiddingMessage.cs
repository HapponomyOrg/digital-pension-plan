using System;

namespace NATS
{
    [Obsolete]
    public class CancelBiddingMessage : BaseMessage
    {
        public string AuctionID;

        public CancelBiddingMessage(string dateTimeStamp, int lobbyID, int playerID, string auctionID) : base(dateTimeStamp, MessageSubject.CancelBidding, lobbyID, playerID)
        {
            AuctionID = auctionID;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}";
        }
    }
}