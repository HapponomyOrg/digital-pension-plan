namespace NATS
{
    public class HeartBeatMessage : BaseMessage
    {
        public string PlayerName;
        public int Balance;
        public int[] Cards;
        public int Points;
        public int[] AllPoints;
        public HeartBeatMessage(string dateTime, int lobbyID, int playerID, string playerName, int balance, int[] cards, int points, int[] allPoints) : base(dateTime, MessageSubject.HeartBeat, lobbyID, playerID)
        {
            PlayerName = playerName;
            Balance = balance;
            Cards = cards;
            Points = points;
            AllPoints = allPoints;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}, Playername: {PlayerName}, Balance: {Balance}, Cards: {Cards}, Points: {Points}, AllPoints: {AllPoints}";
        }
    }
}