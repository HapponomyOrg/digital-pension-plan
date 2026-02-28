using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Version1.Host.Scripts
{
    public class PlayerListPrefab : MonoBehaviour
    {
        public DateTime LastPing;

        private static readonly System.Globalization.CultureInfo deCulture = new("de-DE");

        public int ID { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                NameTextField.text = value;
                _name = value;
            }
        }

        private int _points;
        public int Points
        {
            get => _points;
            set
            {
                PointsTextField.text = value.ToString();
                _points = value;
            }
        }

        private int _balance;
        public int Balance
        {
            get => _balance;
            set
            {
                BalanceTextField.text = FormatMoney(value);
                _balance = value;
            }
        }

        private int currentRound;

        public int CurrentRound
        {
            get => currentRound;
            set =>
                // TODO here can set in ui.
                currentRound = value;
        }

        [SerializeField] private TMP_Text NameTextField;
        [SerializeField] private TMP_Text PointsTextField;
        [SerializeField] private TMP_Text BalanceTextField;

        public PlayerListPrefab(string PlayerName, int playerId, int points, DateTime now, int currentRound)
        {
            Name = PlayerName;
            ID = playerId;
            Points = points;
            LastPing = now;
            CurrentRound = currentRound;
        }

        private static string FormatMoney(int amount) => amount.ToString("N0", deCulture);
    }
}
