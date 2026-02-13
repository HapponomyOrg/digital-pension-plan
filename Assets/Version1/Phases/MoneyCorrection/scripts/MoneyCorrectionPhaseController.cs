using System;
using System.Collections;
using Assets.Version1.Phases;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Version1.Nats.Messages.Client;
using Version1.Utilities;

namespace Version1.Phases.MoneyCorrection.scripts
{
    public class MoneyCorrectionPhaseController : MonoBehaviour, IPhaseController
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Button continueButton;

        private static readonly System.Globalization.CultureInfo deCulture = new("de-DE");

        private static PlayerData.PlayerData Player => PlayerData.PlayerData.Instance;

        private void Start()
        {
            GameManager.Instance.PhaseManager.CurrentPhaseController = this;
        }

        public void StartPhase()
        {
            switch (Player.CurrentMoneySystem)
            {
                case MoneySystems.Sustainable:
                    HandleSustainableSystem();
                    break;
                case MoneySystems.DebtBased:
                    HandleDebtBasedSystem();
                    break;
                default:
                    throw new NotImplementedException(
                        $"Money system '{Player.CurrentMoneySystem}' not yet implemented.");
            }
        }

        private void HandleDebtBasedSystem()
        {
            var balance = Player.Balance;
            var interest = 0;
            // TODO B.Nierop do something with the balance.
            // wait for the script of steff to do this.

            // TODO B.Nierop at the end pay the interest to the bank player.
            var msg = new PayInterestToBankMessage(DateTime.Now.ToString("o"), PlayerData.PlayerData.Instance.LobbyID,
                PlayerData.PlayerData.Instance.PlayerId, PlayerData.PlayerData.Instance.bankPlayer, interest);
            NetworkManager.Instance.Publish(PlayerData.PlayerData.Instance.LobbyID.ToString(), msg);
        }

        private void HandleSustainableSystem()
        {
            var balance = Player.Balance;

            switch (balance)
            {
                case > 6000:
                {
                    var penalty = RoundToThousand((balance - 6000) / 2f);
                    Player.Balance -= penalty;

                    Debug.LogWarning($"Over 6000 | Penalty: {penalty} | New Balance: {FormatMoney(Player.Balance)}");
                    StartCoroutine(DisplayTextLetterByLetter(
                        $"! Balance Penalty !\n\n" +
                        $"Your balance exceeded €6.000!\n" +
                        $"Penalty: €{FormatMoney(penalty)}\n\n" +
                        $"Previous balance: €{FormatMoney(balance)}\n" +
                        $"New balance: €{FormatMoney(Player.Balance)}"));
                    break;
                }
                case < 4000:
                    Player.Balance += 2000;

                    Debug.LogWarning($"Under 4000 | New Balance: {FormatMoney(Player.Balance)}");
                    StartCoroutine(DisplayTextLetterByLetter(
                        $"! Balance Bonus !\n\n" +
                        $"Your balance fell below €4.000!\n" +
                        $"Bonus received: €2.000\n\n" +
                        $"Previous balance: €{FormatMoney(balance)}\n" +
                        $"New balance: €{FormatMoney(Player.Balance)}"));
                    break;
                default:
                    StartCoroutine(DisplayTextLetterByLetter(
                        $"! No Balance Adjustment !\n\n" +
                        $"Your balance is between €4.000 and €6.000\n" +
                        $"Current balance: €{FormatMoney(balance)}"));
                    break;
            }
        }

        public void StopPhase()
        {
        }

        public void Continue()
        {
            continueButton.interactable = false;
            SceneManager.LoadScene(GameManager.LOADING);
        }

        public void OnDestroy()
        {
            GameManager.Instance.PhaseManager.CurrentPhaseController = null;
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

        private static int RoundToThousand(float value)
        {
            return (int)Math.Round(value / 1000, MidpointRounding.AwayFromZero) * 1000;
        }

        private static string FormatMoney(int amount) => amount.ToString("N0", deCulture);
    }
}
