using System;
using System.Collections;
using System.Collections.Generic;
using NATS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DonateMoneyOverlay : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private TMP_Text donationAmountDisplay;
    
    private int currentDonation = 1000;

    private int minDonation;
    private int maxDonation;

    [SerializeField] private int priceStep = 1000;

    private void Update()
    {
        UpdateOverlay();
    }
    public void Open()
    {
        gameObject.SetActive(true);

        minDonation = priceStep;
        maxDonation = PlayerManager.Instance.Balance;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            if (currentDonation < minDonation)
                return;
            PlayerManager.Instance.Balance -= currentDonation;
            SendDonateMessage();
            gameObject.SetActive(false);
        });
    }

    public void UpdateOverlay()
    {
        donationAmountDisplay.text = currentDonation.ToString();
        displayText.text = $"Are you sure you want to donate {currentDonation}?";
    }
        
    public void IncreaseDonation()
    {
        currentDonation += priceStep;
        if (currentDonation > maxDonation)
            currentDonation = maxDonation;
    }
        
    public void DecreaseDonation()
    {
        currentDonation -= priceStep;
        if (currentDonation < minDonation)
            currentDonation = minDonation;
    }

    private void SendDonateMessage()
    {
        var sessionId = PlayerManager.Instance.LobbyID;
        var msg = new DonateMoneyMessage(DateTime.Now.ToString("o"), sessionId, PlayerManager.Instance.PlayerId,
            currentDonation);

        NatsClient.Instance.Publish(sessionId.ToString(), msg);
    }
}
