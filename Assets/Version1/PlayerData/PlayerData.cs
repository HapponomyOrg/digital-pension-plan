using System;
using System.Collections.Generic;
using UnityEngine;

namespace Version1.PlayerData
{
    public class PlayerData : MonoBehaviour
    {
        private int _balance = 0;
        private int _points = 0;
        private int _lobbyID = 0;

        public string playerName = "";
        public int age = 0;
        public string gender = "";
        public int playerId = 456;
        public int dept = 11000;
        public int interestRemainder = 0;
        public List<int> allPoints = new List<int>();


        public event EventHandler<int> OnBalanceChange;
        public event EventHandler<int> OnPointsChange;
        public event EventHandler<int> onLobbyIDChange;


        private static PlayerData _instance;

        public static PlayerData Instance
        {
            get
            {
                if (!_instance) _instance = new PlayerData();
                return _instance;
            }
            private set => _instance = value;
        }

        public int Balance
        {
            get => _balance;
            set
            {
                _balance = value;
                OnBalanceChange?.Invoke(this, value);
            }
        }

        public int Points
        {
            get => _points;
            set
            {
                _points = value;
                OnPointsChange?.Invoke(this, value);
            }
        }

        public int LobbyID
        {
            get => _lobbyID;
            set
            {
                _lobbyID = value;
                onLobbyIDChange?.Invoke(this, value);
            }
        }
    }
}