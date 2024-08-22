namespace NATS
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
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , Cards: {CardID}, Points: {Points}";
        }
    }
}
