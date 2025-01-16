namespace Version1.Nats.Messages.Host
{
    public class AbortSessionMessage : BaseMessage
    {
        // TODO define message
        public AbortSessionMessage(string dateTime, int lobbyID, int playerID) : base(dateTime, MessageSubject.AbortSession, lobbyID, playerID)
        {

        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}";
        }
    }
}