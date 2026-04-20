using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Version1.Host.Scripts;
using Version1.Nats.Messages.Client;
using Version1.Utilities;
using Version1.Utilities.NetworkManager;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.TakeALoan.scripts
{
    public class TakeALoanPhaseController : MonoBehaviour, IPhaseController
    {
        [SerializeField] private TMP_Text currentBalanceText;
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private Button confirmButton;

        private static readonly System.Globalization.CultureInfo deCulture = new("de-DE");

        private int currentAmount;
        private const int priceStep = 1000;

        private void Start()
        {
            Utilities.GameManager.Instance.PhaseManager.CurrentPhaseController = this;
        }

        public void StartPhase()
        {
            amountText.text = "0";
            currentBalanceText.text =  $"Current balance: €{FormatMoney(PlayerData.Instance.Balance)}\n" +
                                       $"Current debt: €{FormatMoney(PlayerData.Instance.Debt)}";
        }

        public void IncreaseAmount()
        {
            currentAmount += priceStep;
            // TODO check what is max to take a loan
            /*if (currentAmount > maxDonation)
                currentAmount = maxDonation;*/

            UpdateOverlay();
        }

        public void DecreaseAmount()
        {
            currentAmount -= priceStep;
            if (currentAmount < 0)
                currentAmount = 0;

            UpdateOverlay();
        }

        private void UpdateOverlay()
        {
            amountText.text = FormatMoney(currentAmount);
            currentBalanceText.text =  $"Current balance: €{FormatMoney(PlayerData.Instance.Balance + currentAmount)}\n" +
                                       $"Current debt: €{FormatMoney(PlayerData.Instance.Debt + currentAmount)}";
            confirmButton.interactable = currentAmount > 0;
        }

        public void TakeALoanButton()
        {
            PlayerData.Instance.Balance += currentAmount;
            PlayerData.Instance.Debt += currentAmount;

            SceneManager.LoadScene(PhaseLibrary.LoadingPhase.Scene);
        }

        public void Continue()
        {
            NetworkManager.Instance.Publish(SessionData.Instance.LobbyCode.ToString(),
                new ContinueMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, PlayerData.Instance.PlayerId, GameManager.Instance.PhaseManager.CurrentRound()));
            SceneManager.LoadScene(PhaseLibrary.LoadingPhase.Scene);
        }


        public void StopPhase()
        {
        }


        private static string FormatMoney(int amount) => amount.ToString("N0", deCulture);
    }
}
