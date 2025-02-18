using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;

namespace Version1.PlayerData
{
    // TODO when start looking at the money system set the debt to the balance.
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
        [FormerlySerializedAs("dept")] [SerializeField] private int debt = 0;
        [SerializeField] private int interestRemainder = 0;
        [SerializeField] private List<int> cards = new List<int>();
        [SerializeField] private List<int> allPoints = new List<int>();
        
        [SerializeField] private MoneySystems currentMoneySystem = 0;
        

        // Private fields for balance, points, lobby ID
        [SerializeField] private int balance = 0;
        [SerializeField] private int points = 0;
        [SerializeField] private int lobbyID = 0;

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

        public int Debt
        {
            get => debt;
            set => debt = value;
        }
        
        public int InterestRemainder
        {
            get => interestRemainder;
            set => interestRemainder = value;
        }

        public MoneySystems CurrentMoneySystem
        {
            get => currentMoneySystem;
            set => currentMoneySystem = value;
        }

        public List<int> Cards => new List<int>(cards);

        public List<int> AllPoints => new List<int>(allPoints);

        public int Balance
        {
            get => balance;
            set => SetAndInvoke(ref balance, value, OnBalanceChange);
        }

        public int Points
        {
            get => points;
            set => SetAndInvoke(ref points, value, OnPointsChange);
        }

        public int LobbyID
        {
            get => lobbyID;
            set => SetAndInvoke(ref lobbyID, value, OnLobbyIDChange);
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
                cards.Sort();

                OnCardsChange?.Invoke(this, new List<int>(cards));
        }
        
        public void AddCards(int[] c)
        {
            foreach (var card in c)
            {
                cards.Add(card);
            }
            cards.Sort();
            
            OnCardsChange?.Invoke(this, new List<int>(cards));
        }

        public void RemoveCard(int card)
        {
            if (cards.Remove(card))
            {
                cards.Sort();
                OnCardsChange?.Invoke(this, new List<int>(cards));
            }
            else
            {
                // TODO ERROR MESSAGE
                Debug.LogWarning($"Card {card} not found in player's collection.");
            }
        }

        public void RemoveCards(int[] c)
        {
            foreach (var card in c)
            {
                cards.Remove(card);
            }
            cards.Sort();
            
            OnCardsChange?.Invoke(this, new List<int>(cards));
        }
        
        public void AddPoints(int points)
        {
            this.points += points;
            OnPointsChange?.Invoke(this, this.points);
        }

        public void AddToBalance(int amount)
        {
            balance += amount;
            OnBalanceChange?.Invoke(this, balance);
        }

        public void SubtractFromBalance(int amount)
        {
            if (balance >= amount)
            {
                balance -= amount;
                OnBalanceChange?.Invoke(this, balance);
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

        // TODO() Rename to non-event function
        public void ResetData()
        {
            // TODO RESET EVERYTHING
            throw new NotImplementedException();
        }
    }
}
