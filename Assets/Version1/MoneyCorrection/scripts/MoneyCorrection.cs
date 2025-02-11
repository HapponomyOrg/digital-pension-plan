using System;
using TMPro;
using UnityEngine;

namespace Version1.PayInterest.scripts
{
    public class PayInterest : MonoBehaviour
    {
        [SerializeField] private TMP_Text Text;
        
        private void OnEnable()
        {
            switch (PlayerData.PlayerData.Instance.CurrentMoneySystem)
            {
                case MoneySystems.Sustainable:
                    switch (PlayerData.PlayerData.Instance.Balance)
                    {
                        case < 4000:
                            PlayerData.PlayerData.Instance.Balance += 2000;
                            //TODO make ui
                            break;
                        case > 6000:
                        {
                            var amountToPay = Mathf.CeilToInt(PlayerData.PlayerData.Instance.Balance - 6000 / 2000f) * 1000;
                        
                            PlayerData.PlayerData.Instance.Balance -= amountToPay;

                            // TODO make ui
                            break;
                        }
                    }
                    break;
                case MoneySystems.DebtBased:
                    // TODO this is at the end of the game.
                    throw new NotImplementedException();
                    break;
                case MoneySystems.InterestAtIntervals:
                    int newInterest = (int)(PlayerData.PlayerData.Instance.Debt * 0.10);
                    var interestDue = PlayerData.PlayerData.Instance.InterestRemainder + newInterest;

                    int payment = 0;

                    //TODO check for last round
                    if (false)
                    {
                        // In the final payment, all debt and accumulated interest must be paid
                        PlayerData.PlayerData.Instance.Debt += interestDue; // Add remaining interest to the debt
                        interestDue = 0; // No more interest remains
                    }
                    else
                    {
                        if (interestDue >= 1000)
                        {
                            payment = Math.Min(interestDue / 1000 * 1000, PlayerData.PlayerData.Instance.Balance);

                            if (payment > 0)
                            {
                                PlayerData.PlayerData.Instance.Balance -= payment;
                                interestDue -= payment;
                            }
                        }
                        
                        if (payment == 0)
                        {
                            PlayerData.PlayerData.Instance.Debt += interestDue;
                            interestDue = 0;
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
    }
}
