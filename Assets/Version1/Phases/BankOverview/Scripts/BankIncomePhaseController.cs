using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Version1.Host.Scripts;
using Version1.Nats.Messages.Client;
using Version1.Utilities;
using Version1.Utilities.NetworkManager;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.BankOverview.Scripts
{
    public class BankIncomePhaseController : MonoBehaviour, IPhaseController
    {
        // TODO B.Nierop now this is done quite weird because we cannot guarantee what comes first.
        // the network handler for the onpayinterest or loading of this script so thats why it is done twice.

        private int incomeAmount = 0;
        [SerializeField] private TMP_Text incomeText;
        [SerializeField] private Button continueText;

        [SerializeField] private PlayerListPrefab playerListPrefab;
        [SerializeField] private Transform list;

        private static readonly System.Globalization.CultureInfo deCulture = new("de-DE");

        private void Start()
        {
            GameManager.Instance.PhaseManager.CurrentPhaseController = this;
        }

        private void OnOnPayInterestToBank(object sender, PayInterestToBankMessage e)
        {
            var item = Instantiate(playerListPrefab, list);

            item.Name = e.PlayerName;
            item.Balance = e.Amount;

            var income = PlayerData.Instance.GetAllBankIncome();

            incomeAmount += e.Amount;

            incomeText.text = FormatMoney(incomeAmount);
        }

        public void StartPhase()
        {
            NetworkManager.Instance.GetWsContext().OnPayInterestToBank += OnOnPayInterestToBank;

            incomeAmount = 0;

            var income = PlayerData.Instance.GetAllBankIncome();

            foreach (Transform child in list)
            {
                Destroy(child.gameObject);
            }

            var totalIncome = income.Values.Sum();
            incomeText.text = FormatMoney(totalIncome);

            foreach (var entry in income)
            {
                var item = Instantiate(playerListPrefab, list);

                item.Name = entry.Key;
                item.Balance = entry.Value;
            }
        }


        public void Continue()
        {
            PlayerData.Instance.ResetBankIncome();
            NetworkManager.Instance.Publish(SessionData.Instance.LobbyCode.ToString(),
                new ContinueMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, PlayerData.Instance.PlayerId, GameManager.Instance.PhaseManager.CurrentRound()));
            SceneManager.LoadScene(PhaseLibrary.LoadingPhase.Scene);
        }

        public void StopPhase()
        {
            PlayerData.Instance.ResetBankIncome();
            NetworkManager.Instance.GetWsContext().OnPayInterestToBank -= OnOnPayInterestToBank;

        }

        private static string FormatMoney(int amount) => amount.ToString("N0", deCulture);
    }
}
