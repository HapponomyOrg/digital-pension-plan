using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Version1.Phases.Trading
{
    public class TopBar : MonoBehaviour
    {
        [SerializeField] private TMP_Text points;
        [SerializeField] private TMP_Text balance;

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
            
            points.text = PlayerData.PlayerData.Instance.Points.ToString();
            balance.text = PlayerData.PlayerData.Instance.Balance.ToString("N0", customCulture);
            
            PlayerData.PlayerData.Instance.OnPointsChange += (sender, i) => { points.text = PlayerData.PlayerData.Instance.Points.ToString(); };
            PlayerData.PlayerData.Instance.OnBalanceChange += (sender, i) => { balance.text = PlayerData.PlayerData.Instance.Balance.ToString("N0", customCulture); };
        }
        
        public void OpenDonateOverlay()
        {
            donateOverlay.SetActive(true);
        }
    }
}
