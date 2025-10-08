namespace Version1.Nats.Messages.Client
{
    public class DonatePointsMessage : BaseMessage
    {
        public string PlayerName;
        public int Receiver;
        public int Amount;

        public DonatePointsMessage(string dateTime, int lobbyID, int playerID, string playerName, int receiver, int amount) : base(dateTime, MessageSubject.DonatePoints, lobbyID, playerID)
        {
            PlayerName = playerName;
            Receiver = receiver;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , PlayerName: {PlayerName}, Amount: {Amount}, Receiver: {Receiver}";
        }
    }
}
