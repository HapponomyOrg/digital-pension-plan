namespace Version1.Nats.Messages.Client
{
    public class ContinueMessage : BaseMessage
    {
        public int RoundNumber;
        public ContinueMessage(string dateTimeStamp, int lobbyID, int playerID, int roundNumber) : base(dateTimeStamp, MessageSubject.Continue, lobbyID, playerID)
        {
            RoundNumber = roundNumber;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} RoundNumber: {RoundNumber}";
        }
    }
}
