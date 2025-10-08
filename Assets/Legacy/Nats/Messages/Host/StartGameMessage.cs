using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace NATS
{
    [Obsolete]
    public class StartGameMessage : BaseMessage
    {
        public int OtherPlayerID;
        public int Balance;
        public int[] Cards;
        public int IntrestMode;

        public StartGameMessage(string dateTime, int lobbyID, int playerID, int otherPlayerID, int balance, int[] cards, int intrestMode) : base(dateTime, MessageSubject.StartGame, lobbyID, playerID)
        {
            OtherPlayerID = otherPlayerID;
            Cards = cards;
            Balance = balance;
            IntrestMode = intrestMode;
        }

        public override string ToString()
        {
            return $"{DateTimeStamp} , Lobby: {LobbyID} , Subject: {Subject} , Player: {PlayerID} , OtherPlayerID: {OtherPlayerID}, Balance: {Balance}, Cards: {Cards}, IntrestMode: {IntrestMode}";
        }
    }
}
