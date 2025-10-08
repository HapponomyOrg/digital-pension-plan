namespace Version1.Nats.Messages.Client
{
    public class ListCardsmessage : BaseMessage
    {
        public string PlayerName;
        public string AuctionID;
        public int[] Cards;
        public int Amount;

        public ListCardsmessage(string dateTimeStamp, int lobbyID, int playerID, string playerName, string auctionID, int[] cards, int amount) : base(dateTimeStamp, MessageSubject.ListCards, lobbyID, playerID)
        {
            PlayerName = playerName;
            AuctionID = auctionID;
            Cards = cards;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}, Cards: {Cards}, Amount: {Amount}";
        }
    }
}
