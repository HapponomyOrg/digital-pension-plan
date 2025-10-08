using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Obsolete]
public class TakeLoanOverlay : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private TMP_Text TakeAmountDisplay;
    [SerializeField] private Transform payLoanBuyLoanOverlay;

    private int currentTake;

    private int minTake;

    [SerializeField] private int priceStep = 1000;

    private void Update()
    {
        UpdateOverlay();
    }

    private void Start()
    {
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.Round == 4)
            {
                GameManager.Instance.ChangeScene(GameManager.Instance.pointsDonateScene);   
            }
        });
    }

    public void Open()
    {
        gameObject.SetActive(true);
        payLoanBuyLoanOverlay.gameObject.SetActive(false);
        minTake = 0;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            if (currentTake < minTake)
                return;
            PlayerManager.Instance.Balance += currentTake;
            PlayerManager.Instance.Dept += currentTake;
            SendDonateMessage();
            if (GameManager.Instance.Round == 4)
            {
                GameManager.Instance.ChangeScene(GameManager.Instance.pointsDonateScene);   
            }
            gameObject.SetActive(false);
        });
    }

    public void UpdateOverlay()
    {
        TakeAmountDisplay.text = currentTake.ToString();
        displayText.text = $"Do you want to take a loan from the bank of {currentTake} \nwith an interest rate of 10%?";
    }

    public void IncreaseTake()
    {
        currentTake += priceStep;
    }

    public void DecreaseTake()
    {
        currentTake -= priceStep;
        if (currentTake < minTake)
            currentTake = minTake;
    }

    private void SendDonateMessage()
    {
        Debug.Log("Took x-amount loan");
    }
}
