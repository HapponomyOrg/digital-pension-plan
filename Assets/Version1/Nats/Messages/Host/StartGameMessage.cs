namespace Version1.Nats.Messages.Host
{
    public class StartGameMessage : BaseMessage
    {
        public int OtherPlayerID;
        public int Balance;
        public int[] Cards;
        public int IntrestMode;
        public string BankPlayer;

        public StartGameMessage(string dateTime, int lobbyID, int playerID, int otherPlayerID, int balance, int[] cards, int intrestMode, string bankPlayer) : base(dateTime, MessageSubject.StartGame, lobbyID, playerID)
        {
            OtherPlayerID = otherPlayerID;
            Cards = cards;
            Balance = balance;
            IntrestMode = intrestMode;
            BankPlayer = bankPlayer;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , OtherPlayerID: {OtherPlayerID}, Balance: {Balance}, Cards: {Cards}, IntrestMode: {IntrestMode}, BankPlayer: {BankPlayer}";
        }
    }
}
