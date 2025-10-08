using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Obsolete]
public class IntrestsFunctions : MonoBehaviour
{
    public GameObject simple, complex;
    public TextMeshProUGUI nameInput;

    [Header("Complex inputfields")] public TextMeshProUGUI debtInput;
    public TextMeshProUGUI remainderInput, intrestInput, toPayInput;

    [Header("Simple inputfields")] public TextMeshProUGUI balanceInput;
    public TextMeshProUGUI changeInput, newBalanceInput;

    [SerializeField] private Button continueButton;

    private void Start()
    {
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.GameMode == 0)
            {
                if (GameManager.Instance.Round == 4)
                {
                    GameManager.Instance.ChangeScene(GameManager.Instance.pointsDonateScene);
                }
            }
        });
    }

    private void OnEnable()
    {
        nameInput.text = PlayerManager.Instance.PlayerName;
        switch (GameManager.Instance.GameMode)
        {
            case 0:
                CalculateIntrestSimple();
                simple.SetActive(true);
                complex.SetActive(false);
                break;
            case 1:
                CalculateIntrestComplicated();
                complex.SetActive(true);
                simple.SetActive(false);
                break;
        }
    }

    public void CalculateIntrestSimple()
    {
        int balance = PlayerManager.Instance.Balance;

        balanceInput.text = balance.ToString();

        int newBalance = balance;
        string change = 0.ToString();
        if (balance < 3000)
        {
            change = "+" + 2000.ToString();
            newBalance += 2000;
        }

        if (balance > 6000)
        {
            change = "-" + roundToThousand((balance - 6000) / 2).ToString();
            newBalance -= roundToThousand((balance - 6000) / 2);
        }

        PlayerManager.Instance.SetBalance(newBalance);

        changeInput.text = change;
        newBalanceInput.text = newBalance.ToString();
    }

    private int roundToThousand(float value)
    {
        return (int)Math.Round(value / 1000, MidpointRounding.AwayFromZero) * 1000;
    }

    public void CalculateIntrestComplicated()
    {
        int debt = PlayerManager.Instance.Dept;
        int remainder = PlayerManager.Instance.IntrestRemainder;
        int balance = PlayerManager.Instance.Balance;



        // TODO CHECK IF THIS IS THE WAY TO GO FOR THE LAST ROUND CALCULATION OF THE DEBT BASED SYSTEM
        if (GameManager.Instance.Round == 4)
        {
            int total = (int)(debt * 1.1f) + remainder;
            // rounded to thousands
            int toPay = ((int)(total / 1000)) * 1000;

            if (balance > toPay)
            {
                PlayerManager.Instance.RemoveBalance(toPay);
            }
            else
            {
                toPay -= balance;
                PlayerManager.Instance.RemoveBalance(toPay);

                int result = (toPay / 1000);

                PlayerManager.Instance.Points -= result;
            }
        }
        else
        {
            int intrest = (int)(debt * .1f) + remainder;
            // rounded to thousands
            int toPay = ((int)(intrest / 1000)) * 1000;
            remainder = remainder + (intrest - toPay);

            if (balance > toPay)
            {
                PlayerManager.Instance.RemoveBalance(toPay);
            }
            else
            {
                PlayerManager.Instance.Dept += toPay;
            }

            PlayerManager.Instance.IntrestRemainder = intrest - toPay;
            debtInput.text = debt.ToString();
            remainderInput.text = remainder.ToString();
            intrestInput.text = intrest.ToString();
            toPayInput.text = toPay.ToString();
        }

    }
}
