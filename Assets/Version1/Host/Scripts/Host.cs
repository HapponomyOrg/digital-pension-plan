using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;

namespace Version1.Host.Scripts
{
    public class Host : MonoBehaviour
    {
        [SerializeField] private GameObject natsError;
        [SerializeField] private TMP_Text natsErrorTMP;

        private Stopwatch _sessionDuration;
        private bool _sessionIsActive = false;

        // scenes
        [SerializeField] private GameObject CreateScene;

        // things from the hostscene
        [SerializeField] private Button abortSession;
        [SerializeField] private Button startSession;
        [SerializeField] private Button stopRound;
        [SerializeField] private Button skipRound;
        [SerializeField] private Button startRound;

        // things from the hostscene
        private int playerId = 0;
        private Dictionary<int, playerlistprefab> players;
        [SerializeField] private Transform playerListPrefab;
        [SerializeField] private GameObject playerScrolView;

        // things from the hostscene
        private List<GameObject> activities;
        [SerializeField] private GameObject activityPrefab;
        [SerializeField] private GameObject activityScrollView;
        [SerializeField] private TMP_Text hostNameTMP;
        [SerializeField] private TMP_Text gameModeTMP;
        [SerializeField] private TMP_Text seedTMP;
        [SerializeField] private TMP_Text gameCodeTMP;
        [SerializeField] private TMP_Text roundDurationTMP;
        [SerializeField] private TMP_Text creationTime;
        [SerializeField] private TMP_Text sessionDurationTMP;
        [SerializeField] private TMP_Text timeLeftTMP;
        [SerializeField] private TMP_Text phaseTypeTMP;
        [SerializeField] private TMP_Text nextPhaseTMP;


        private void Start()
        {
            abortSession.onClick.AddListener(AbortSessionOnClick);
            startSession.onClick.AddListener(StartSessionOnClick);
            stopRound.onClick.AddListener(StopRoundOnClick);
            skipRound.onClick.AddListener(ContinueOnClick);
            startRound.onClick.AddListener(StartRoundOnClick);

            // hostscene
            Nats.NatsHost.C.OnHeartBeat += OnOnHeartBeat;
            Nats.NatsHost.C.OnJoinrequest += OnOnJoinrequest;
            Nats.NatsHost.C.MessageLog += OnMessageLog;
        }

        private void OnEnable()
        {
            activities = new List<GameObject>();

            _sessionDuration = new Stopwatch();

            players = new Dictionary<int, playerlistprefab>();

            playerScrolView = GameObject.Find("PlayerScrollView");

            hostNameTMP.text = SessionData.Instance.HostName;
            gameModeTMP.text = SessionData.Instance.GameMode.ToString(); //TODO enum type oid
            seedTMP.text = SessionData.Instance.Seed.ToString();
            gameCodeTMP.text =
                $"{SessionData.Instance.LobbyCode.ToString().Substring(0, 3)} {SessionData.Instance.LobbyCode.ToString().Substring(3, 3)} {SessionData.Instance.LobbyCode.ToString().Substring(6, 3)}";
            roundDurationTMP.text = SessionData.Instance.RoundDuration.ToString();
            creationTime.text = DateTime.Now.ToString("HH:mm:ss");
            _sessionDuration.Start();
            _sessionIsActive = true;
        }

        private void OnMessageLog(object sender, string e)
        {
            var activity = Instantiate(activityPrefab, activityScrollView.transform);
            activity.GetComponentInChildren<TMP_Text>().text = e;

            activities.Add(activity);

            if (activities.Count > 20)
            {
                Destroy(activities[0]);
                activities.RemoveAt(0);
            }
        }

        private void OnOnJoinrequest(object sender, JoinRequestMessage e)
        {
            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new ConfirmJoinMessage(
                DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode,
                -1,
                playerId,
                e.PlayerName,
                e.Age,
                e.Gender));

            players.Add(playerId, new playerlistprefab(e.PlayerName, playerId, 0, DateTime.Now));
        }

        private void OnOnHeartBeat(object sender, HeartBeatMessage e)
        {
            DateTime parsedDate = DateTime.Parse(e.DateTimeStamp);

            if (!players.ContainsKey(e.PlayerID))
            {
                var player = Instantiate(playerListPrefab, playerScrolView.transform);
                player.GetComponent<playerlistprefab>().LastPing = parsedDate;
                player.GetComponent<playerlistprefab>().Name = e.PlayerName;
                player.GetComponent<playerlistprefab>().Balance = e.Balance;
                player.GetComponent<playerlistprefab>().Points = e.Points;

                players.Add(e.PlayerID, player.GetComponent<playerlistprefab>());
            }
            else
            {
                players[e.PlayerID].LastPing = parsedDate;
                players[e.PlayerID].Name = e.PlayerName;
                players[e.PlayerID].Balance = e.Balance;
                players[e.PlayerID].Points = e.Points;
            }
        }

        private void AbortSessionOnClick()
        {
            // TODO maybe confirmation

            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new AbortSessionMessage(
                DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode,
                -1));

            CreateScene.SetActive(true);

            gameObject.SetActive(false);
        }

        private void StartSessionOnClick()
        {
            // TODO SEND A MESSAGE TO ALL PLAYERS WITH THEIR INFO
            /*Nats.NatsHost.C.Publish(Lobby.ToString(), new (DateTime.Now.ToString("o"), Lobby,
                1,
                Lobby));*/
        }

        private void StopRoundOnClick()
        {
            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new StopRoundMessage(
                DateTime.Now.ToString("o"),
                SessionData.Instance.LobbyCode,
                -1,
                1 //TODO CHANGE TO ROUND NUMBER OR CHANGE TO STRING WITH PHASE NAME
            ));
        }

        private void ContinueOnClick()
        {
            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new SkipRoundMessage(
                DateTime.Now.ToString("o"),
                SessionData.Instance.LobbyCode,
                -1));
        }

        private void StartRoundOnClick()
        {
            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new StartRoundMessage(
                DateTime.Now.ToString("o"),
                SessionData.Instance.LobbyCode,
                -1,
                1, //TODO CHANGE TO ROUND NUMBER OR PHASE NAME
                100 // TODO CHANGE TO DURATION OF PHASE, GET FROM PHASE SYSTEM OF LUUK
            ));
        }

        private void Update()
        {
            if (_sessionIsActive)
            {
                float totalSeconds = (float)_sessionDuration.Elapsed.TotalSeconds;

                int hours = Mathf.FloorToInt(totalSeconds / 3600);
                int minutes = Mathf.FloorToInt((totalSeconds % 3600) / 60);
                int seconds = Mathf.FloorToInt(totalSeconds % 60);

                sessionDurationTMP.text = hours > 0 ? $"{hours}:{minutes:D2}:{seconds:D2}" : $"{minutes}:{seconds:D2}";

                //TODO misschien is dit in de update een beetje een overkill

                var keysToRemove = new List<int>();

                foreach (var player in players)
                {
                    if (DateTime.Now - TimeSpan.FromSeconds(5) > player.Value.LastPing)
                    {
                        Destroy(player.Value.gameObject);
                        keysToRemove.Add(player.Key);
                    }
                }

                foreach (var key in keysToRemove)
                {
                    players.Remove(key);
                }
            }

            Nats.NatsHost.C.HandleMessages();
        }
    }
}