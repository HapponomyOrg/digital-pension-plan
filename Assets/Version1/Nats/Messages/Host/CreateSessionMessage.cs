namespace Version1.Nats.Messages.Host
{
    public class CreateSessionMessage : BaseMessage
    {
        public int SessionToken;
        public CreateSessionMessage(string dateTime, int lobbyID, byte playerID, int sessionToken) : base(dateTime, MessageSubject.CreateSession, lobbyID, playerID)
        {
            SessionToken = sessionToken;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}, SessionToken: {SessionToken}";
        }
    }
}