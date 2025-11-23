using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Version1.Nats.Messages;

namespace Assets.Version1.Nats.Messages.Client
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
