using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.Trading
{
    public class TopBar : MonoBehaviour
    {
        [SerializeField] private TMP_Text points;
        [SerializeField] private TMP_Text balance;
        [SerializeField] private TMP_Text debt;
        [SerializeField] private TMP_Text remainder;

        [SerializeField] private GameObject donateOverlay;
        [SerializeField] private Button donateButton;

        private CultureInfo customCulture;

        private void Awake()
        {
            customCulture = new CultureInfo("en-US")
            {
                NumberFormat =
                {
                    NumberGroupSeparator = "."
                }
            };

            PlayerData.Instance.OnPointsChange += OnPointsChanged;
            PlayerData.Instance.OnBalanceChange += OnBalanceChanged;
        }

        private void OnDestroy()
        {
            if (PlayerData.Instance != null)
            {
                PlayerData.Instance.OnPointsChange -= OnPointsChanged;
                PlayerData.Instance.OnBalanceChange -= OnBalanceChanged;
            }
        }

        public void Init()
        {
            UpdateUI();
        }

        private void OnPointsChanged(object sender, int newPoints)
        {
            points.text = newPoints.ToString();
        }

        private void OnBalanceChanged(object sender, int newBalance)
        {
            balance.text = newBalance.ToString("N0", customCulture);
            UpdateDonateButtonState();
        }

        private void UpdateUI()
        {
            points.text = PlayerData.Instance.Points.ToString();
            balance.text = PlayerData.Instance.Balance.ToString("N0", customCulture);

            if (debt)
            {
                debt.text = PlayerData.Instance.Debt.ToString("N0", customCulture);
            }

            if (remainder)
            {
                remainder.text = PlayerData.Instance.InterestRemainder.ToString("N0", customCulture);
            }

            UpdateDonateButtonState();
        }

        private void UpdateDonateButtonState()
        {
            if (donateButton != null)
            {
                donateButton.interactable = PlayerData.Instance.Balance >= 1000;
            }
        }

        public void OpenDonateOverlay()
        {
            if (PlayerData.Instance.Balance < 1000)
            {
                Debug.LogWarning("Cannot donate - balance is less than 1000");
                return;
            }

            donateOverlay.SetActive(true);
        }
    }
}
