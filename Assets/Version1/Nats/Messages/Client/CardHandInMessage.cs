namespace Version1.Nats.Messages.Client
{
    public class CardHandInMessage : BaseMessage
    {
        public int CardID;
        public int Points;

        public CardHandInMessage(string dateTimeStamp, int lobbyID, int playerID, int cardID, int points) : base(dateTimeStamp, MessageSubject.CardHandIn, lobbyID, playerID)
        {
            CardID = cardID;
            Points = points;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , CardID: {CardID}, Points: {Points}";
        }
    }
}
