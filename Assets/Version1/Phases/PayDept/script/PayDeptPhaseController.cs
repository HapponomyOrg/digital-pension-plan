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
        [SerializeField] private Button ConfirmButton;

        private static readonly System.Globalization.CultureInfo deCulture = new("de-DE");

        private int _currentAmount = 0;
        private const int PriceStep = 1000;

        private int _maxAmount = 0;

        private void Start()
        {
            Utilities.GameManager.Instance.PhaseManager.CurrentPhaseController = this;
        }

        public void StartPhase()
        {
            if (PlayerData.PlayerData.Instance.Debt <= 0) SceneManager.LoadScene(Utilities.GameManager.LOADING);

            amountText.text = "0";

            _maxAmount = PlayerData.PlayerData.Instance.Debt;
            currentBalanceText.text =  $"Current balance: €{FormatMoney(PlayerData.PlayerData.Instance.Balance)}\n" +
                                       $"Current debt: €{FormatMoney(PlayerData.PlayerData.Instance.Debt)}";
        }

        public void StopPhase()
        {
        }

        public void IncreaseAmount()
        {
            int newAmount = _currentAmount + PriceStep;

            if (newAmount > PlayerData.PlayerData.Instance.Balance)
                return;

            _currentAmount = newAmount;

            // TODO B.Nierop check if there is a max amount of loan.
            /*// Clamp to max loan amount
            if (_currentAmount > _maxAmount)
                _currentAmount = _maxAmount;*/

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
            amountText.text = FormatMoney(_currentAmount);
            currentBalanceText.text =  $"Current balance: €{FormatMoney(PlayerData.PlayerData.Instance.Balance - _currentAmount)}\n" +
                                       $"Current debt: €{FormatMoney(PlayerData.PlayerData.Instance.Debt - _currentAmount)}";
            ConfirmButton.interactable = _currentAmount != 0;
        }

        public void Continue()
        {
            SceneManager.LoadScene(Utilities.GameManager.LOADING);
        }

        public void PayDept()
        {
            PlayerData.PlayerData.Instance.Debt -= _currentAmount;
            PlayerData.PlayerData.Instance.Balance -= _currentAmount;

            SceneManager.LoadScene(Utilities.GameManager.LOADING);
        }

        private static string FormatMoney(int amount) => amount.ToString("N0", deCulture);
    }
}
