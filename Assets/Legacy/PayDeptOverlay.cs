using Legacy;
using NATS;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Obsolete]
public class PayDeptOverlay : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private TMP_Text PayAmountDisplay;
    [SerializeField] private GameObject takeLoanObj;
    
    private int payOffAmount;

    private int minDept;
    private int maxDept;

    [SerializeField] private int priceStep = 1000;

    private void Update()
    {
        UpdateOverlay();
    }
    public void Open()
    {
        if (GameManager.Instance.GameMode == 1)
        {
            if(PlayerManager.Instance.Dept <= 0)
            {
                takeLoanObj.SetActive(true);
                gameObject.SetActive(false);
            }
            
            gameObject.SetActive(true);
            minDept = 0;
            maxDept = PlayerManager.Instance.Balance;

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                if (payOffAmount < minDept)
                    return;
                PlayerManager.Instance.Balance -= payOffAmount;
                PlayerManager.Instance.Dept -= payOffAmount;
                SendDonateMessage();
                gameObject.SetActive(false);
            });
        }
        else
            return;
    }

    public void UpdateOverlay()
    {
        PayAmountDisplay.text = payOffAmount.ToString();
        displayText.text = $"Currently you have {PlayerManager.Instance.Dept} of dept to the bank,\nDo you want to pay of your dept?";
    }

    public void IncreaseDept()
    {
        payOffAmount += priceStep;
        if(payOffAmount > PlayerManager.Instance.Balance)
            payOffAmount = PlayerManager.Instance.Balance;
        else if(payOffAmount > PlayerManager.Instance.Dept)
            payOffAmount = PlayerManager.Instance.Dept;
    }

    public void DecreaseDept()
    {
        payOffAmount -= priceStep;
        if (payOffAmount < minDept)
            payOffAmount = minDept;
    }

    private void SendDonateMessage()
    {
        Debug.Log("Payed x-amount loan");
    }
}
