using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Client;

namespace Version1.Donate.money.scripts
{
    public class DonateMoney : MonoBehaviour
    {
           [SerializeField] private Button confirmButton;
            [SerializeField] private TMP_Text donationAmountDisplay;
            
            private int currentDonation = 1000;
        
            private int minDonation;
            private int maxDonation;
        
            [SerializeField] private int priceStep = 1000;
            

            private void Update()
            {
                UpdateOverlay();
            }
            public void OnEnable()
            {
                minDonation = priceStep;
                maxDonation = PlayerData.PlayerData.Instance.Balance;

                currentDonation = (int)Mathf.Floor(PlayerData.PlayerData.Instance.Balance / 2000) * 1000;
                
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(() =>
                {
                    if (currentDonation < minDonation)
                        return;
                    PlayerData.PlayerData.Instance.Balance -= currentDonation;
                    SendDonateMessage();
                    gameObject.SetActive(false);
                });
                
                UpdateOverlay();
            }
        
            private void UpdateOverlay()
            {
                donationAmountDisplay.text = currentDonation.ToString("N0", new System.Globalization.CultureInfo("de-DE"));
            }
                
            public void IncreaseDonation()
            {
                currentDonation += priceStep;
                if (currentDonation > maxDonation)
                    currentDonation = maxDonation;

                UpdateOverlay();
            }
                
            public void DecreaseDonation()
            {
                currentDonation -= priceStep;
                if (currentDonation < minDonation)
                    currentDonation = minDonation;

                UpdateOverlay();
            }
        
            private void SendDonateMessage()
            {
                var sessionId = PlayerData.PlayerData.Instance.LobbyID;
                var msg = new DonateMoneyMessage(DateTime.Now.ToString("o"), sessionId, PlayerData.PlayerData.Instance.PlayerId,
                    currentDonation);
        
                Nats.NatsClient.C.Publish(sessionId.ToString(), msg);
            }
    }
}