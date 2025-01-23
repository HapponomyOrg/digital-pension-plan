using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Version1.Host.Scripts
{
    public class playerlistprefab : MonoBehaviour
    {
        public DateTime LastPing;

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
        public int Points         {
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
                BalanceTextField.text = value.ToString();
                _balance = value;
            }
        }

        [SerializeField] private TMP_Text NameTextField;
        [SerializeField] private TMP_Text PointsTextField;
        [SerializeField] private TMP_Text BalanceTextField;

        public playerlistprefab(string PlayerName, int playerId, int points, DateTime now)
        {
            Name = PlayerName;
            ID = playerId;
            Points = points;
            LastPing = now;

        }
    }
}
