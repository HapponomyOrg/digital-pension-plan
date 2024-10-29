using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NATS
{
    public abstract class BaseMessage
    {
        public string DateTimeStamp;
        public string Subject;
        public int LobbyID;
        public int PlayerID;
        
        protected BaseMessage(string dateTimeStamp, string subjectString, int lobbyID, int playerID)
        {
            DateTimeStamp = dateTimeStamp;
            Subject = subjectString;
            LobbyID = lobbyID;
            PlayerID = playerID;
        }
    }
}