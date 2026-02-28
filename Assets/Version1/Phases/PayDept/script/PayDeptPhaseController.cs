using Assets.Version1.Phases;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Version1.Phases.PayDept.script
{
    public class PayDeptPhaseController : MonoBehaviour, IPhaseController
    {
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private TMP_Text currentBalanceText;
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
            if (PlayerData.PlayerData.Instance.Debt <= 0) SceneManager.LoadScene(Utilities.GameManager.LOADING);

            amountText.text = "0";

            currentBalanceText.text = $"Current balance: €{FormatMoney(PlayerData.PlayerData.Instance.Balance)}\n" +
                                      $"Current debt: €{FormatMoney(PlayerData.PlayerData.Instance.Debt)}";
        }

        public void StopPhase()
        {
        }

        public void IncreaseAmount()
        {
            int newAmount = currentAmount + priceStep;

            if (newAmount > PlayerData.PlayerData.Instance.Balance)
                return;

            currentAmount = newAmount;

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
            currentBalanceText.text =
                $"Current balance: €{FormatMoney(PlayerData.PlayerData.Instance.Balance - currentAmount)}\n" +
                $"Current debt: €{FormatMoney(PlayerData.PlayerData.Instance.Debt - currentAmount)}";
            confirmButton.interactable = currentAmount != 0;
        }

        public void Continue()
        {
            Utilities.GameManager.Instance.PhaseManager.LoadNextPhase();
        }

        public void PayDept()
        {
            PlayerData.PlayerData.Instance.Debt -= currentAmount;
            PlayerData.PlayerData.Instance.Balance -= currentAmount;

            Utilities.GameManager.Instance.PhaseManager.LoadNextPhase();
        }

        private static string FormatMoney(int amount) => amount.ToString("N0", deCulture);
    }
}
