using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Version1.Host.Scripts;
using Version1.Nats.Messages.Client;
using Version1.Utilities;
using Version1.Utilities.NetworkManager;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.Explanation.Bank.scripts
{
    public class BankExplanationPhaseController : MonoBehaviour, IPhaseController
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Button continueButton;

        private static readonly System.Globalization.CultureInfo deCulture = new("de-DE");

        private void Start()
        {
            GameManager.Instance.PhaseManager.CurrentPhaseController = this;
        }

        public void StartPhase()
        {
            continueButton.interactable = false;

            StartCoroutine(DisplayTextLetterByLetter(
                !PlayerData.Instance.IsBankPlayer()
                    ? $"Welcome! You’re a Regular Player.\n\n" +
                      $"You start the game with a debt of €{FormatMoney(PlayerData.Instance.Debt)} " +
                      $"and a balance of €{FormatMoney(PlayerData.Instance.Balance)}, " +
                      $"which you borrowed from the bank.\n\n" +
                      $"After each round, you owe 10% interest on your debt. " +
                      $"If the interest is less than €1,000, it carries over to the next round.\n\n" +
                      $"You can repay part or all of your debt after each round, and you can also take additional loans if needed.\n\n"
                    : $"Congratulations! You are the Bank Player.\n\n" +
                      $"You start with €0.\n\n" +
                      $"As the bank, you receive interest payments from all regular players after each round. " +
                      $"You cannot take loans and you have no debt.\n\n"));
        }


        public void StopPhase()
        {
        }

        public void Continue()
        {
            NetworkManager.Instance.Publish(SessionData.Instance.LobbyCode.ToString(),
                new ContinueMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, PlayerData.Instance.PlayerId, GameManager.Instance.PhaseManager.CurrentRound()));
            SceneManager.LoadScene(GameManager.LOADING);
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

        private static string FormatMoney(int amount) => amount.ToString("N0", deCulture);
    }
}
