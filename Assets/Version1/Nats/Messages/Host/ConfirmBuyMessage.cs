namespace Version1.Nats.Messages.Host
{
    public class ConfirmBuyMessage : BaseMessage
    {
        public string AuctionID;
        public int Seller;
        public int Buyer;
        public int Amount;

        public ConfirmBuyMessage(string dateTime, int lobbyID, int playerID, string auctionID, int seller, int buyer, int amount) : base(dateTime, MessageSubject.ConfirmBuy, lobbyID, playerID)
        {
            AuctionID = auctionID;
            Seller = seller;
            Buyer = buyer;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}, AuctionID: {AuctionID}, Seller: {Seller}, Buyer: {Buyer}, Amount: {Amount}";
        }
    }
}
