namespace NATS
{
    public class DonateMoneyMessage : BaseMessage
    {
        public int Amount;

        public DonateMoneyMessage(string dateTimeStamp, int lobbyID, int playerID, int amount) : base(dateTimeStamp, MessageSubject.DonateMoney, lobbyID, playerID)
        {
            Amount = amount;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , Amount: {Amount}";
        }
    }
}