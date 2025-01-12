using System;
using System.Collections.Generic;
using UnityEngine;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;

namespace Version1.PlayerData
{
    public class PlayerData : MonoBehaviour
    {
        // Singleton instance
        private static PlayerData _instance;
        public static PlayerData Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("PlayerData instance is not initialized. Ensure it is in the scene.");
                }
                return _instance;
            }
        }
        
        // TODO dont destroy on load
        

        // Serialized fields for Inspector visibility
        [SerializeField] private string playerName = "Player";
        [SerializeField] private int age = 0;
        [SerializeField] private string gender = "Unknown";
        [SerializeField] private int playerId = 456;
        [SerializeField] private int dept = 0;
        [SerializeField] private List<int> cards = new List<int>();
        [SerializeField] private List<int> allPoints = new List<int>();

        // Private fields for balance, points, lobby ID
        private int _balance = 0;
        private int _points = 0;
        private int _lobbyID = 0;

        // Events
        public event EventHandler<int> OnBalanceChange;
        public event EventHandler<int> OnPointsChange;
        public event EventHandler<int> OnLobbyIDChange;
        public event EventHandler<List<int>> OnCardsChange;

        private void Awake()
        {
            // Ensure only one instance exists
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }
            _instance = this;
        }

        #region Public Properties
        public string PlayerName
        {
            get => playerName;
            set => playerName = value;
        }

        public int Age
        {
            get => age;
            set => age = value;
        }

        public string Gender
        {
            get => gender;
            set => gender = value;
        }

        public int PlayerId
        {
            get => playerId;
            set => playerId = value;
        }

        public int Dept
        {
            get => dept;
            set => dept = value;
        }

        public List<int> Cards => new List<int>(cards);

        public List<int> AllPoints => new List<int>(allPoints);

        public int Balance
        {
            get => _balance;
            set => SetAndInvoke(ref _balance, value, OnBalanceChange);
        }

        public int Points
        {
            get => _points;
            set => SetAndInvoke(ref _points, value, OnPointsChange);
        }

        public int LobbyID
        {
            get => _lobbyID;
            set => SetAndInvoke(ref _lobbyID, value, OnLobbyIDChange);
        }
        #endregion

        #region Public State Methods

        public void StartGame(StartGameMessage msg)
        {
            //reset
            
            playerId = msg.OtherPlayerID;
            
            AddToBalance(msg.Balance);
            
            foreach (var card in msg.Cards)
            {
                AddCard(card);
            }
        }
        
        public void ConfirmHandIn(ConfirmHandInMessage msg)
        {
            if (msg.Receiver != playerId) return;

            foreach (var card in msg.Cards)
            {
                AddCard(card);
            }
        }

        #endregion
        
        #region Public Methods
        public void AddCard(int card)
        {
                cards.Add(card);
                OnCardsChange?.Invoke(this, new List<int>(cards));
        }

        public void RemoveCard(int card)
        {
            if (cards.Remove(card))
            {
                OnCardsChange?.Invoke(this, new List<int>(cards));
            }
            else
            {
                // TODO ERROR MESSAGE
                Debug.LogWarning($"Card {card} not found in player's collection.");
            }
        }

        public void AddPoints(int points)
        {
            _points += points;
            OnPointsChange?.Invoke(this, _points);
        }

        public void AddToBalance(int amount)
        {
            _balance += amount;
            OnBalanceChange?.Invoke(this, _balance);
        }

        public void SubtractFromBalance(int amount)
        {
            if (_balance >= amount)
            {
                _balance -= amount;
                OnBalanceChange?.Invoke(this, _balance);
            }
            else
            {
                // TODO ERROR MESSAGE
                Debug.LogWarning("Insufficient balance.");
            }
        }
        #endregion

        #region Private Methods
        private void SetAndInvoke<T>(ref T field, T value, EventHandler<T> eventHandler)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                eventHandler?.Invoke(this, value);
            }
        }
        #endregion

        public void PointsDonated(DonatePointsMessage msg)
        {
            if (msg.Receiver != playerId) return;
            AddPoints(msg.Amount);
        }

        public void Reset()
        {
            // TODO RESET EVERYTHING
            throw new NotImplementedException();
        }
    }
}
