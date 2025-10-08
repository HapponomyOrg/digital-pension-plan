namespace Version1.Nats.Messages.Host
{
    public class EndGameMessage : BaseMessage
    {
        public EndGameMessage(string dateTime, int lobbyID, int playerID) : base(dateTime, MessageSubject.EndGame, lobbyID, playerID)
        {

        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}";
        }
    }
}
