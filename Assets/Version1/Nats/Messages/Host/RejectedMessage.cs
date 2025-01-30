namespace Version1.Nats.Messages.Host
{
    public class RejectedMessage : BaseMessage
    {
        public string TargetPlayer;
        public string ReferenceID;
        public string Message;
        
        public RejectedMessage(string dateTime, int lobbyID, int playerID, string targetPlayer, string referenceID, string message) : base(dateTime,
            MessageSubject.Rejected, lobbyID, playerID)
        {
            TargetPlayer = targetPlayer;
            ReferenceID = referenceID;
            Message = message;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}, TargetPlayer: {TargetPlayer} ReferenceID: {ReferenceID}, Message: {Message}";
        }
    }
}