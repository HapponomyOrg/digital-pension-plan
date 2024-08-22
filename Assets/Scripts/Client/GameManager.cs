using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject gameScene, lobbyScene, logginScene, intrestScene,payDeptOrTakeLoan, pointsDonateScene, endGameScene;

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
        NatsClient.C.OnConfirmJoin += (sender, msg) => { ChangeScene(lobbyScene); };
        NatsClient.C.OnStartGame += (sender, msg) =>
        {
            numGames++;
            PlayerManager.Instance.RoundIsActive = false;
            gameMode = msg.IntrestMode;
            print(msg);
            ChangeScene(gameScene);
            MarketManager.Instance.UpdateBalance();
        };
        NatsClient.C.OnStopRound += (sender, msg) =>
        {
            MarketManager.Instance.RemoveAllBiddings();
            PlayerManager.Instance.RoundIsActive = false;
            timer.StopTimer();
            intrestScene.SetActive(true);
        };
        NatsClient.C.OnStartRound += (sender, msg) =>
        {
            PlayerManager.Instance.RoundIsActive = true;
            intrestScene.SetActive(false);
            StartCoroutine(timer.StartTimer(msg.Duration));
            Round++;
        };
        NatsClient.C.OnEndOfRounds += (sender, msg) =>
        {
            PlayerManager.Instance.RoundIsActive = false;
            NatsClient.C.StopHeartbeat();
            NatsClient.C.HeartbeatInterval = 300;
            NatsClient.C.StartHeartbeat();
            // TODO here set to point removeal screen after dept
            ChangeScene(pointsDonateScene);
        };
        
        NatsClient.C.OnEndGame += (sender, msg) =>
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
        NatsClient.C.HandleMessages();
    }

    public void OnDestroy()
    {
        NatsClient.C.StopHeartbeat();
    }
}