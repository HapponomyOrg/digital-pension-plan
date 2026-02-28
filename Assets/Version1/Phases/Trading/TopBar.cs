using System;
using System.Globalization;
using TMPro;
using UnityEngine;
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

        public void Init()
        {
            var customCulture = new CultureInfo("en-US")
            {
                NumberFormat =
                {
                    NumberGroupSeparator = "."
                }
            };

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

            PlayerData.Instance.OnPointsChange += (sender, i) =>
            {
                points.text = PlayerData.Instance.Points.ToString();
            };
            PlayerData.Instance.OnBalanceChange += (sender, i) =>
            {
                balance.text = PlayerData.Instance.Balance.ToString("N0", customCulture);
            };
        }

        public void OpenDonateOverlay()
        {
            donateOverlay.SetActive(true);
        }
    }
}
