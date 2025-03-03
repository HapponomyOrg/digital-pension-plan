namespace Version1.Nats.Messages.Client
{
    public class JoinRequestMessage : BaseMessage
    {
        public string PlayerName;
        public int Age;
        public int Gender;
        public string RequestID;

        public JoinRequestMessage(string dateTime, int lobbyID, int playerID, string playerName, int age, int gender, string requestID)
            : base(dateTime, MessageSubject.JoinRequest, lobbyID, playerID)
        {
            PlayerName = playerName;
            Age = age;
            Gender = gender;
            RequestID = requestID;
        }

        public override string ToString()
        {
            return
                $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , PlayerName: {PlayerName} , Age: {Age} , Gender: {Gender}, RequestID: {RequestID}";
        }
    }
}