using System;

namespace NATS
{
    [Obsolete]
    public class JoinRequestMessage : BaseMessage
    {
        public string PlayerName;
        public int Age;
        public string Gender;

        public JoinRequestMessage(string dateTime, int lobbyID, int playerID, string playerName, int age, string gender)
            : base(dateTime, MessageSubject.JoinRequest, lobbyID, playerID)
        {
            PlayerName = playerName;
            Age = age;
            Gender = gender;
        }

        public override string ToString()
        {
            return
                $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , PlayerName: {PlayerName} , Age: {Age} , Gender: {Gender}";
        }
    }
}