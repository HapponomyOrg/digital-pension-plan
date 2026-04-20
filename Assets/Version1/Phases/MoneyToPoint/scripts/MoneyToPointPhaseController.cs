using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Version1.Host.Scripts;
using Version1.Nats.Messages.Client;
using Version1.Utilities;
using Version1.Utilities.NetworkManager;
using Version1.Utilities.PlayerData;

namespace Version1.Phases.MoneyToPoint.scripts
{
    public class MoneyToPointPhaseController : MonoBehaviour, IPhaseController
    {
        [SerializeField] private TMP_Text MoneyTMP;
        [SerializeField] private TMP_Text PointTMP;

        private int balance;

        private int Balance
        {
            get => balance;
            set
            {
                balance = value;
                MoneyTMP.text = balance.ToString("N0", new System.Globalization.CultureInfo("de-DE"));
            }
        }

        private int points;

        private int Points
        {
            get => points;
            set
            {
                points = value;
                PointTMP.text = points.ToString();
            }
        }

        private void Start()
        {
            // Start the phase
            Utilities.GameManager.Instance.PhaseManager.CurrentPhaseController = this;
        }

        private IEnumerator StartCountDown()
        {
            yield return new WaitForSeconds(1);

            while (Balance > 0)
            {
                // TODO if we change the money system so it is not 1000 change this
                Balance -= 1000;
                yield return new WaitForSeconds(0.2f);

                if (Balance % 2000 == 0)
                {
                    Points += 1;
                }
            }

            PlayerData.Instance.Balance = 0;
            PlayerData.Instance.Points = Points;

        }

        public void Continue()
        {
            NetworkManager.Instance.Publish(SessionData.Instance.LobbyCode.ToString(),
                new ContinueMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, PlayerData.Instance.PlayerId, GameManager.Instance.PhaseManager.CurrentRound()));
            SceneManager.LoadScene(Utilities.GameManager.LOADING);
        }

        public void StartPhase()
        {
            Balance = PlayerData.Instance.Balance;
            Points = PlayerData.Instance.Points;

            MoneyTMP.text = Balance.ToString("N0", new System.Globalization.CultureInfo("de-DE"));
            PointTMP.text = Points.ToString();

            StartCoroutine(StartCountDown());
        }

        public void StopPhase()
        {

        }

        public void OnDestroy()
        {
            Utilities.GameManager.Instance.PhaseManager.CurrentPhaseController = null;
        }
    }
}
