using System;
using Assets.Version1.Phases;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Version1.Phases.TakeALoan.scripts
{
    public class TakeALoanPhaseController : MonoBehaviour, IPhaseController
    {
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private Button confirmButton;

        private int currentAmount;
        private const int priceStep = 1000;

        private void Start()
        {
            Utilities.GameManager.Instance.PhaseManager.CurrentPhaseController = this;
        }

        public void StartPhase()
        {
            switch (PlayerData.PlayerData.Instance.CurrentMoneySystem)
            {
                case MoneySystems.Sustainable:
                    Debug.Log("take a loan is not used in the sustainable money system.");
                    break;
                case MoneySystems.DebtBased:
                    descriptionText.text =
                        "Here you can take a loan against 10% of interest you have to pay back by the end of the game.";
                    break;
                default:
                    throw new NotImplementedException();
            }

            amountText.text = "0";
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
            amountText.text = currentAmount.ToString("N0", new System.Globalization.CultureInfo("de-DE"));
            confirmButton.interactable = currentAmount > 0;
        }

        public void TakeALoanButton()
        {
            PlayerData.PlayerData.Instance.Balance += currentAmount;

            SceneManager.LoadScene(PhaseLibrary.LoadingPhase.Scene);

            // TODO check if there is a message to be sent.
        }

        public void Continue()
        {
            SceneManager.LoadScene(PhaseLibrary.LoadingPhase.Scene);
        }


        public void StopPhase()
        {
        }
    }
}
