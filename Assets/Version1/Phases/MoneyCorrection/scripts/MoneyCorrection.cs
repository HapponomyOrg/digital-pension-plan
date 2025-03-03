using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Version1.Phases.MoneyCorrection.scripts
{
    public class MoneyCorrection : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        private void Start()
        {
            switch (PlayerData.PlayerData.Instance.CurrentMoneySystem)
            {
                case MoneySystems.Sustainable:
                    switch (PlayerData.PlayerData.Instance.Balance)
                    {
                        case < 4000:
                            PlayerData.PlayerData.Instance.Balance += 2000;
                            StartCoroutine(DisplayTextLetterByLetter(
                                $"Because your balance is less then 4.000\n2.000 will be added to your balance\n\nYour balance is now {PlayerData.PlayerData.Instance.Balance.ToString("N0", new System.Globalization.CultureInfo("de-DE"))}"));
                            break;
                        case > 6000:
                        {
                            var amountToPay = roundToThousand((PlayerData.PlayerData.Instance.Balance - 6000) / 2);
                            PlayerData.PlayerData.Instance.Balance -= amountToPay;

                            StartCoroutine(DisplayTextLetterByLetter(
                                $"Because your balance is over 6.000\nyou will get a penalty of {amountToPay.ToString("N0", new System.Globalization.CultureInfo("de-DE"))}\n\nYour new balance is {PlayerData.PlayerData.Instance.Balance.ToString("N0", new System.Globalization.CultureInfo("de-DE"))}"));

                            break;
                        }
                    }

                    break;
                case MoneySystems.DebtBased:
                    // TODO this is at the end of the game.
                    throw new NotImplementedException();
                    break;
                case MoneySystems.InterestAtIntervals:
                    int newInterest =
                        (int)(PlayerData.PlayerData.Instance.Debt * 0.10); // Calculate 10% interest of current debt
                    var interestDue =
                        PlayerData.PlayerData.Instance.InterestRemainder +
                        newInterest; // Add remaining interest from previous round

                    int payment = 0;

                    //TODO final round
                    if (false) // You need to determine how to set `isFinalRound` (e.g., a flag or check)
                    {
                        // In the final payment, all debt and accumulated interest must be paid
                        PlayerData.PlayerData.Instance.Debt += interestDue; // Add remaining interest to the debt
                        interestDue = 0; // No more interest remains
                        payment = PlayerData.PlayerData.Instance.Debt; // Pay the full debt + interest
                        PlayerData.PlayerData.Instance.Balance -= payment; // Deduct from the player's balance

                        text.text =
                            $"You have a debt of {PlayerData.PlayerData.Instance.Debt} with an interest rate of 10%\n" +
                            $"All remaining debt and interest ({interestDue}) have been added to the debt.\n" +
                            $"Your total debt now is {PlayerData.PlayerData.Instance.Debt}. " +
                            $"You have paid {payment} from your balance, leaving you with {PlayerData.PlayerData.Instance.Balance}.";
                    }
                    else
                    {
                        // Check if interest due is at least 1,000
                        if (interestDue >= 1000)
                        {
                            // Calculate payment as the minimum of the available balance or the closest multiple of 1000
                            payment = Math.Min((interestDue / 1000) * 1000, PlayerData.PlayerData.Instance.Balance);

                            if (payment > 0)
                            {
                                // Update the debt and interest after the payment
                                PlayerData.PlayerData.Instance.Balance -= payment;
                                interestDue -= payment;

                                // If there's any leftover interest, it gets carried over to the next round
                                PlayerData.PlayerData.Instance.InterestRemainder = interestDue;


                                StartCoroutine(DisplayTextLetterByLetter(
                                    $"You have a debt of {PlayerData.PlayerData.Instance.Debt.ToString("N0", new System.Globalization.CultureInfo("de-DE"))} with an interest rate of 10%\n" +
                                    $"Remaining interest from last round: {PlayerData.PlayerData.Instance.InterestRemainder.ToString("N0", new System.Globalization.CultureInfo("de-DE"))}\n" +
                                    $"Interest due this round: {interestDue.ToString("N0", new System.Globalization.CultureInfo("de-DE"))}\n\n" +
                                    $"You paid {payment.ToString("N0", new System.Globalization.CultureInfo("de-DE"))}, leaving {interestDue.ToString("N0", new System.Globalization.CultureInfo("de-DE"))} remaining interest."));
                            }
                        }

                        // If the player didn't make a payment
                        if (payment == 0)
                        {
                            // If no payment was made, the interest is added to the total debt and carried over
                            PlayerData.PlayerData.Instance.Debt += interestDue;
                            PlayerData.PlayerData.Instance.InterestRemainder = interestDue;
                            interestDue = 0;


                            StartCoroutine(DisplayTextLetterByLetter(
                                $"You have a debt of {PlayerData.PlayerData.Instance.Debt.ToString("N0", new System.Globalization.CultureInfo("de-DE"))} with an interest rate of 10%\n" +
                                $"No payment was made. The interest of {interestDue.ToString("N0", new System.Globalization.CultureInfo("de-DE"))} has been added to your debt."));
                        }
                    }

                    break;

                case MoneySystems.ClosedEconomy:
                    throw new NotImplementedException();
                    // This is an addition to the interest at intervals/
                    // choose 1 player that is the bank.
                    // this player can not borrow money but gets the interest as income.
                    break;
                case MoneySystems.RealisticDebtDistribution:
                    throw new NotImplementedException();
                    // At the beginning the host sets the debt of players at the same random interval as the balance
                    // After that it is just the interest at intervals or debt based TODO check this
                    // If also closed economy. the bank does not have a debt, other players carry this debt.
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int roundToThousand(float value)
        {
            return (int)Math.Round(value / 1000, MidpointRounding.AwayFromZero) * 1000;
        }

        private IEnumerator DisplayTextLetterByLetter(string message)
        {
            // Start by resetting the text
            text.text = "";

            // The text you want to display

            // Loop through each character in the message and reveal it one by one
            foreach (char letter in message)
            {
                text.text += letter; // Add one letter at a time
                yield return new WaitForSeconds(0.03f); // Wait for a specified time before displaying the next letter
            }
        }
    }
}


// TODO 
//
// end screen
// use phase system for overlays I made
// setup aws
// 
//

// test take a loan en pay dept
// fix same colors in all screens I made


// DONE
// fix scroll rect of player list and progression in host screen
// card hand in overlay
// check donate point screen