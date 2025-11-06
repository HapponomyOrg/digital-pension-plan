using System;
using TMPro;
using UnityEngine;

namespace Version1.Phases.TakeALoan.scripts
{
    public class TakeALoan : MonoBehaviour
    {
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text amountText;

        private int _currentAmount = 0;
        private const int PriceStep = 1000;

        private void Start()
        {
            switch (PlayerData.PlayerData.Instance.CurrentMoneySystem)
            {
                case MoneySystems.Sustainable:
                    Debug.Log("take a loan is not used in the sustainable money system.");
                    break;
                case MoneySystems.DebtBased:
                    descriptionText.text = "Here you can take a loan against 10% of interest you have to pay back by the end of the game.";
                    break;
                case MoneySystems.InterestAtIntervals:
                    descriptionText.text = "Here you can take a loan against 10% of interest you have to pay back after every round.";
                    break;
                case MoneySystems.ClosedEconomy:
                    descriptionText.text = "Here you can take a loan against 10% of interest you have to pay back after every round.";
                    break;
                case MoneySystems.RealisticDebtDistribution:
                    descriptionText.text = "Here you can take a loan against 10% of interest you have to pay back after every round.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            amountText.text = "0";
        }

        public void IncreaseAmount()
        {
            _currentAmount += PriceStep;
            // TODO check what is max to take a loan
            /*if (currentAmount > maxDonation)
                currentAmount = maxDonation;*/

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

        public void TakeALoanButton()
        {
            PlayerData.PlayerData.Instance.Balance -= _currentAmount;

            // TODO check if there is a message
            /*var sessionId = PlayerData.PlayerData.Instance.LobbyID;
            var msg = new DonateMoneyMessage(DateTime.Now.ToString("o"), sessionId, PlayerData.PlayerData.Instance.PlayerId,
                currentDonation);
        
            Nats.NatsClient.C.Publish(sessionId.ToString(), msg);*/
        }
    }
}
