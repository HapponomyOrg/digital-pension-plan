namespace Version1.Nats.Messages
{
    [System.Serializable]
    public class BaseMessage
    {
        public string DateTimeStamp;
        public string Subject;
        public int LobbyID;
        public int PlayerID;
        
        // Parameterless constructor for JsonUtility
        public BaseMessage()
        {
        }
        
        // Keep your existing constructor for manual instantiation
        public BaseMessage(string dateTimeStamp, string subjectString, int lobbyID, int playerID)
        {
            DateTimeStamp = dateTimeStamp;
            Subject = subjectString;
            LobbyID = lobbyID;
            PlayerID = playerID;
        }
    }
}