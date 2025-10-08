namespace Version1.Nats.Messages.Host
{
    public class SkipRoundMessage : BaseMessage
    {
        //TODO fill message
        
        public SkipRoundMessage(string dateTime, int lobbyID, int playerID) : base(dateTime, MessageSubject.SkipRounds, lobbyID, playerID)
        {
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}";
        }
    }
}