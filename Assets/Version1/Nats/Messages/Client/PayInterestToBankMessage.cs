namespace Version1.Nats.Messages.Client
{
    public class PayInterestToBankMessage : BaseMessage
    {
        public string PlayerName;
        public string BankPlayer;
        public int Amount;

        public PayInterestToBankMessage(string dateTimeStamp, int lobbyID, int playerID, string bankPlayer, int amount, string playerName)
            : base(dateTimeStamp, MessageSubject.PayInterestToBank, lobbyID, playerID)
        {
            PlayerName = playerName;
            BankPlayer = bankPlayer;
            Amount = amount;
        }

        public override string ToString()
        {
            return
                $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , BankPlayer: {BankPlayer}, Amount: {Amount}, PlayerName: {PlayerName}";
        }
    }
}
