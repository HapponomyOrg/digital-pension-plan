using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace NATS
{
    [Obsolete]
    public class EndOfRoundsMessage : BaseMessage
    {
        public EndOfRoundsMessage(string dateTime, int lobbyID, int playerID) : base(dateTime,
            MessageSubject.EndOfRounds, lobbyID, playerID)
        {
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID}";
        }
    }
}