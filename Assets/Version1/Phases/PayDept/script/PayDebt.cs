using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Version1.PayDept.script
{
    public class PayDept : MonoBehaviour
    {
        [SerializeField] private TMP_Text amountText;

        private int _currentAmount = 0;
        private const int PriceStep = 1000;

        private int _maxAmount = 0;

        private void Start()
        {
            // TODO skip to next phase
            if (PlayerData.PlayerData.Instance.Debt <= 0) SceneManager.LoadScene(Utilities.GameManager.LOADING);

            amountText.text = "0";

            _maxAmount = PlayerData.PlayerData.Instance.Debt;
        }

        public void IncreaseAmount()
        {
            _currentAmount += PriceStep;
            // TODO check what is max to take a loan
            if (_currentAmount > _maxAmount)
                _currentAmount = _maxAmount;

            UpdateOverlay();
        }

        public void DecreaseAmount()
        {
            _currentAmount -= PriceStep;
            if (_currentAmount < 0)
                _currentAmount = 0;

            UpdateOverlay();
        }

        private void UpdateOverlay()
        {
            amountText.text = _currentAmount.ToString("N0", new System.Globalization.CultureInfo("de-DE"));
        }

        public void Continue()
        {
            SceneManager.LoadScene(Utilities.GameManager.LOADING);
        }

        public void PayDeptButton()
        {
            PlayerData.PlayerData.Instance.Debt -= _currentAmount;
            PlayerData.PlayerData.Instance.Balance -= _currentAmount;

            // TODO check if there is a message
            /*var sessionId = PlayerData.PlayerData.Instance.LobbyID;
            var msg = new DonateMoneyMessage(DateTime.Now.ToString("o"), sessionId, PlayerData.PlayerData.Instance.PlayerId,
                currentDonation);

            Nats.NatsClient.C.Publish(sessionId.ToString(), msg);*/
        }
    }
}