namespace Version1.Nats.Messages.Host
{
    public class EndOfRoundsMessage : BaseMessage
    {
        public EndOfRoundsMessage(string dateTime, int lobbyID, int playerID) : base(dateTime,
            MessageSubject.EndOfRounds, lobbyID, playerID)
        {
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}";
        }
    }
}
