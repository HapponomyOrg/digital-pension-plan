namespace Version1.Nats.Messages.Host
{
    public class StopRoundMessage : BaseMessage
    {
        public int RoundNumber;
        public StopRoundMessage(string dateTime, int lobbyID, int playerID, int roundNumber) : base(dateTime, MessageSubject.StopRound, lobbyID, playerID)
        {
            RoundNumber = roundNumber;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , RoundNumber: {RoundNumber}";
        }
    }
}
