namespace Version1.Nats.Messages.Host
{
    public class StartRoundMessage : BaseMessage
    {
        public int RoundNumber;
        public int Duration;
        public StartRoundMessage(string dateTime, int lobbyID, int playerID, int roundNumber, int duration) : base(dateTime, MessageSubject.StartRound, lobbyID, playerID)
        {
            RoundNumber = roundNumber;
            Duration = duration;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , RoundNumber: {RoundNumber}, Duration: {Duration}";
        }
    }
}