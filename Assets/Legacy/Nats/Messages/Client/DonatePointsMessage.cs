using System;

namespace NATS
{
    [Obsolete]
    public class DonatePointsMessage : BaseMessage
    {
        public int Receiver;
        public int Amount;

        public DonatePointsMessage(string dateTime, int lobbyID, int playerID, int receiver, int amount) : base(dateTime, MessageSubject.DonatePoints, lobbyID, playerID)
        {
            Receiver = receiver;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , Amount: {Amount}, Receiver: {Receiver}";
        }
    }
}
