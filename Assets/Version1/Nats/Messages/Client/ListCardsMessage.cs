namespace Version1.Nats.Messages.Client
{
    public class ListCardsmessage : BaseMessage
    {
        public string PlayerName;
        public string AuctionID;
        public int[] Cards;
        public int Amount;
        public string ListingDateTimeStamp;

        public ListCardsmessage(string dateTimeStamp, int lobbyID, int playerID, string playerName, string auctionID, int[] cards, int amount, string listingDateTimeStamp) : base(dateTimeStamp, MessageSubject.ListCards, lobbyID, playerID)
        {
            PlayerName = playerName;
            AuctionID = auctionID;
            Cards = cards;
            Amount = amount;
            ListingDateTimeStamp = listingDateTimeStamp;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , AuctionID: {AuctionID}, Cards: {Cards}, Amount: {Amount}";
        }
    }
}