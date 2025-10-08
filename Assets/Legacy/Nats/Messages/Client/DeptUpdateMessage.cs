using System;

namespace NATS
{
    [Obsolete]
    public class DeptUpdateMessage : BaseMessage
    {
        public int Dept;

        public DeptUpdateMessage(string dateTime, int lobbyID, int playerID, int newDept) : base(dateTime, MessageSubject.DeptUpdate, lobbyID, playerID)
        {
            Dept = newDept;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , Dept: {Dept}";
        }
    }
}
