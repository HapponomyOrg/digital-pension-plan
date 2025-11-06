namespace Version1.Nats.Messages.Host
{
    public class ConfirmHandInMessage : BaseMessage
    {
        public int Receiver;
        public int[] Cards;

        public ConfirmHandInMessage(string dateTime, int lobbyID, int playerID, int receiver, int[] cards) : base(dateTime, MessageSubject.ConfirmHandIn, lobbyID, playerID)
        {
            Receiver = receiver;
            Cards = cards;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}, Receiver: {Receiver}, Cards: {Cards} ";
        }
    }
}
