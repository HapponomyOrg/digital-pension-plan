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
                    throw new NotImplementedException();
                    break;
                case MoneySystems.InterestAtIntervals:
                    throw new NotImplementedException();
                    break;
                case MoneySystems.ClosedEconomy:
                    throw new NotImplementedException();
                    break;
                case MoneySystems.RealisticDebtDistribution:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
