using System;
using System.Collections;
using Assets.Version1.Phases;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Version1.Utilities;

namespace Version1.Phases.Explanation.Bank.scripts
{
    public class BankExplanationPhaseController : MonoBehaviour, IPhaseController
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Button continueButton;

        private void Start()
        {
            GameManager.Instance.PhaseManager.CurrentPhaseController = this;
        }

        public void StartPhase()
        {
            continueButton.interactable = false;
            StartCoroutine(DisplayTextLetterByLetter(!PlayerData.PlayerData.Instance.IsBankPlayer()
                ? $"Welcome! You’re a Regular Player\n\nYou start the game with a debt of {PlayerData.PlayerData.Instance.Debt}.\n\nDuring pauses, you owe 10% interest. If it’s less than 1,000, it carries over.\n\nIf you can’t pay, the interest is added to your debt—this means more debt and more interest later.\n\nAt the end, you repay your total debt, 10% extra, and any unpaid interest.\n\nManage your loans carefully!"
                : "Congratulations, you are the Bank Player!\n\nYou start with no money.\n\nAs the bank, you manage loans. When players borrow money, it's newly created—you're not lending your own funds.\n\nWhen they repay, only the interest goes to you. The borrowed amount is destroyed and removed from the game.\n\nSince you are the bank, you cannot take loans yourself.\nYour income comes from interest—use it wisely!"));
        }

        public void StopPhase()
        {
        }

        public void Continue()
        {
            SceneManager.LoadScene(Utilities.GameManager.LOADING);
        }

        private IEnumerator DisplayTextLetterByLetter(string message)
        {
            text.text = "";

            foreach (var letter in message)
            {
                text.text += letter;
                yield return new WaitForSeconds(0.03f);
            }

            continueButton.interactable = true;
        }
    }
}
