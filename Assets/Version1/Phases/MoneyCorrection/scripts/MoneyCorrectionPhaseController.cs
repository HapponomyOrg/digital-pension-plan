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

        private const double interestRate = 0.1;

        private void Start()
        {
            GameManager.Instance.PhaseManager.CurrentPhaseController = this;
        }

        public void StartPhase()
        {
            switch (PlayerData.PlayerData.Instance.CurrentMoneySystem)
            {
                case MoneySystems.Sustainable:
                    HandleSustainableSystem();
                    break;
                case MoneySystems.DebtBased:
                    HandleDebtBasedSystem();
                    break;
                default:
                    throw new NotImplementedException(
                        $"Money system '{PlayerData.PlayerData.Instance.CurrentMoneySystem}' not yet implemented.");
            }
        }

        private void HandleDebtBasedSystem()
        {
            var newInterest = PlayerData.PlayerData.Instance.Debt * interestRate;
            var totalInterest = newInterest + PlayerData.PlayerData.Instance.InterestRemainder;

            int toPay;

            if (GameManager.Instance.PhaseManager.GetRoundNumber() == 12) // Last round
            {
                toPay = (int)(Math.Round(totalInterest / 1000, MidpointRounding.AwayFromZero) * 1000);
                PlayerData.PlayerData.Instance.InterestRemainder = 0;
            }
            else
            {
                toPay = (int)Math.Floor(totalInterest / 1000) * 1000;
                PlayerData.PlayerData.Instance.InterestRemainder = (int)(totalInterest - toPay);
            }

            var affordable = PlayerData.PlayerData.Instance.Balance / 1000 * 1000;
            var paid = Math.Min(affordable, toPay);
            var unpaid = toPay - paid;

            PlayerData.PlayerData.Instance.Balance -= paid;
            PlayerData.PlayerData.Instance.Debt += unpaid;

            var msg = new PayInterestToBankMessage(
                DateTime.Now.ToString("o"),
                PlayerData.PlayerData.Instance.LobbyID,
                PlayerData.PlayerData.Instance.PlayerId,
                PlayerData.PlayerData.Instance.bankPlayer,
                paid
            );
            NetworkManager.Instance.Publish(PlayerData.PlayerData.Instance.LobbyID.ToString(), msg);

            // TODO B.Nierop check for last round what text should be and what we do (check slack).
            var message = $"! Interest Payment !\n\n" +
                          $"Your debt: €{FormatMoney(PlayerData.PlayerData.Instance.Debt)}\n" +
                          $"Interest due this round: €{FormatMoney(toPay)}\n" +
                          $"Amount paid to bank: €{FormatMoney(paid)}\n" +
                          (unpaid > 0
                              ? $"Remaining unpaid interest added to your debt: €{FormatMoney(unpaid)}\n"
                              : "") +
                          $"Previous balance: €{FormatMoney(PlayerData.PlayerData.Instance.Balance + paid)}\n" +
                          $"New balance: €{FormatMoney(PlayerData.PlayerData.Instance.Balance)}";

            StartCoroutine(DisplayTextLetterByLetter(message));
        }


        private void HandleSustainableSystem()
        {
            var balance = PlayerData.PlayerData.Instance.Balance;

            switch (balance)
            {
                case > 6000:
                {
                    var penalty = RoundToThousand((balance - 6000) / 2f);
                    PlayerData.PlayerData.Instance.Balance -= penalty;

                    Debug.LogWarning(
                        $"Over 6000 | Penalty: {penalty} | New Balance: {FormatMoney(PlayerData.PlayerData.Instance.Balance)}");
                    StartCoroutine(DisplayTextLetterByLetter(
                        $"! Balance Penalty !\n\n" +
                        $"Your balance exceeded €6.000!\n" +
                        $"Penalty: €{FormatMoney(penalty)}\n\n" +
                        $"Previous balance: €{FormatMoney(balance)}\n" +
                        $"New balance: €{FormatMoney(PlayerData.PlayerData.Instance.Balance)}"));
                    break;
                }
                case < 4000:
                    PlayerData.PlayerData.Instance.Balance += 2000;

                    Debug.LogWarning(
                        $"Under 4000 | New Balance: {FormatMoney(PlayerData.PlayerData.Instance.Balance)}");
                    StartCoroutine(DisplayTextLetterByLetter(
                        $"! Balance Bonus !\n\n" +
                        $"Your balance fell below €4.000!\n" +
                        $"Bonus received: €2.000\n\n" +
                        $"Previous balance: €{FormatMoney(balance)}\n" +
                        $"New balance: €{FormatMoney(PlayerData.PlayerData.Instance.Balance)}"));
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
