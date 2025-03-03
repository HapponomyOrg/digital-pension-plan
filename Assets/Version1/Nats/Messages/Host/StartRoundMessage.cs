namespace Version1.Nats.Messages.Host
{
    public class StartRoundMessage : BaseMessage
    {
        public int RoundNumber;
        public int Duration;
        public string RoundName;
        
        public StartRoundMessage(string dateTime, int lobbyID, int playerID, int roundNumber, string roundName,int duration) : base(dateTime, MessageSubject.StartRound, lobbyID, playerID)
        {
            RoundNumber = roundNumber;
            Duration = duration;
            RoundName = roundName;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , RoundNumber: {RoundNumber}, RoundName: {RoundName}, Duration: {Duration}";
        }
    }
}