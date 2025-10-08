namespace Version1.Nats.Messages.Client
{
    public class ContinueMessage : BaseMessage
    {
        public ContinueMessage(string dateTimeStamp, int lobbyID, int playerID) : base(dateTimeStamp, MessageSubject.Continue, lobbyID, playerID)
        {
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}";
        }
    }
}