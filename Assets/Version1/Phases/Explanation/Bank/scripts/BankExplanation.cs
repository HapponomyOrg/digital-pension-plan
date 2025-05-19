using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Version1.Phases.Explanation.Bank.scripts
{
    public class BankExplanation : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        public void Start()
        {
            text.text = PlayerData.PlayerData.Instance.CurrentMoneySystem switch
            {
                MoneySystems.Sustainable => throw new NotImplementedException(),
                MoneySystems.RealisticDebtDistribution => throw new NotImplementedException(),
                MoneySystems.DebtBased =>
                    "This game uses a simplified money system.\n\nAll money you get is borrowed from the bank and must be repaid with 10% interest at the end. If you can’t pay, the bank takes businesses from you.\n\nFor every 1,000 (or part of 1,000) unpaid, the bank takes 1 business point, lowering your final score and pension.\n\nDuring pauses, you can repay part of your loan or borrow more. Fractions under 1,000 are paid at the end.",
                MoneySystems.InterestAtIntervals =>
                    $"Welcome!\n\nYou start the game with a debt of {PlayerData.PlayerData.Instance.Debt}.\n\nDuring pauses, you owe 10% interest. If it’s less than 1,000, it carries over.\n\nIf you can’t pay, the interest is added to your debt—this means more debt and more interest later.\n\nAt the end, you repay your total debt, 10% extra, and any unpaid interest.\n\nManage your loans carefully!",
                MoneySystems.ClosedEconomy => !PlayerData.PlayerData.Instance.isBank
                    ? $"Welcome! You’re a Regular Player\n\nYou start the game with a debt of {PlayerData.PlayerData.Instance.Debt}.\n\nDuring pauses, you owe 10% interest. If it’s less than 1,000, it carries over.\n\nIf you can’t pay, the interest is added to your debt—this means more debt and more interest later.\n\nAt the end, you repay your total debt, 10% extra, and any unpaid interest.\n\nManage your loans carefully!"
                    : "Congratulations, you are the Bank Player!\n\nYou start with no money.\n\nAs the bank, you manage loans. When players borrow money, it's newly created—you're not lending your own funds.\n\nWhen they repay, only the interest goes to you. The borrowed amount is destroyed and removed from the game.\n\nSince you are the bank, you cannot take loans yourself.\nYour income comes from interest—use it wisely!",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Continue()
        {
            SceneManager.LoadScene("Loading");
        }

    }
}
