using System.Collections.Generic;

namespace Version1.Nats.Messages.Host
{
    // TODO() implement this message in the donate screen
    public class DonateInfoMessage : BaseMessage
    {
        public Dictionary<int,Dictionary<string,int>> PlayerPointsList;
        
        public DonateInfoMessage(string dateTime, int lobbyID, int playerID, Dictionary<int,Dictionary<string,int>> playerPointsList) : base(dateTime, MessageSubject.CancelListing, lobbyID, playerID)
        {
            PlayerPointsList = playerPointsList;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}, AuctionID: {PlayerPointsList}";
        }
    }
}