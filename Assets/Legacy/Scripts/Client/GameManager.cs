using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
[Obsolete]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject gameScene, lobbyScene, logginScene, intrestScene, payDeptOrTakeLoan, pointsDonateScene, endGameScene;

        [SerializeField] private Timer timer;


        public int Round;
        private GameObject currentScene;
        private int gameMode;
        public int numGames = 0;

        public int GameMode => gameMode;

        public static GameManager Instance { get; private set; }

        public GameManager()
        {
            if (Instance != null) return;

            Instance = this;
        }

        private void Start()
        {
            new NatsClient();

        currentScene = logginScene;
        NatsClient.Instance.OnConfirmJoin += (sender, msg) => { ChangeScene(lobbyScene); };
        NatsClient.Instance.OnStartGame += (sender, msg) =>
        {
            numGames++;
            PlayerManager.Instance.RoundIsActive = false;
            gameMode = msg.IntrestMode;
            print(msg);
            ChangeScene(gameScene);
            MarketManager.Instance.UpdateBalance();
        };
        NatsClient.Instance.OnStopRound += (sender, msg) =>
        {
            MarketManager.Instance.RemoveAllBiddings();
            PlayerManager.Instance.RoundIsActive = false;
            timer.StopTimer();
            intrestScene.SetActive(true);
        };
        NatsClient.Instance.OnStartRound += (sender, msg) =>
        {
            PlayerManager.Instance.RoundIsActive = true;
            intrestScene.SetActive(false);
            StartCoroutine(timer.StartTimer(msg.Duration));
            Round++;
        };
        NatsClient.Instance.OnEndOfRounds += (sender, msg) =>
        {
            PlayerManager.Instance.RoundIsActive = false;
            NatsClient.Instance.StopHeartbeat();
            NatsClient.Instance.HeartbeatInterval = 300;
            NatsClient.Instance.StartHeartbeat();

            ChangeScene(pointsDonateScene);
        };

        NatsClient.Instance.OnEndGame += (sender, msg) =>
        {
            PlayerManager.Instance.allPoints.Add(PlayerManager.Instance.Points);
            ChangeScene(endGameScene);
        };
    }

        public void ChangeScene(GameObject newScene)
        {
            currentScene.SetActive(false);
            newScene.SetActive(true);
            currentScene = newScene;
        }

        public void Update()
        {
            NatsClient.Instance.HandleMessages();
        }

    public void OnDestroy()
    {
        NatsClient.Instance.StopHeartbeat();
    }
}
