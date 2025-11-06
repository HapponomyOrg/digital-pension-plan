using System;

namespace NATS
{
    [Obsolete]
    public class ConfirmJoinMessage : BaseMessage
    {
        public int LobbyPlayerID;
        public string PlayerName;
        public int Age;
        public string Gender;
        public ConfirmJoinMessage(string dateTime, int lobbyID, int playerID, int lobbyPlayerID, string playerName, int age, string gender) : base(dateTime, MessageSubject.ConfirmJoin, lobbyID, playerID)
        {
            LobbyPlayerID = lobbyPlayerID;
            PlayerName = playerName;
            Age = age;
            Gender = gender;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , LobbyPlayerID: {LobbyPlayerID}, PlayerName : {PlayerName} , Age: {Age} , Gender: {Gender}";
        }
    }
}
