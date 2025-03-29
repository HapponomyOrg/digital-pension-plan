using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;
using Debug = UnityEngine.Debug;

namespace Version1.Host.Scripts
{
    public class Host : MonoBehaviour
    {
        private int current_round = 0;

        private readonly string[] testPhases =
        {
            "MarketScene",
            "MoneyCorrectionScene",
            "Loading",
            "MarketScene",
            "MoneyCorrectionScene",
            "Loading",
            "MarketScene",
            "MoneyCorrectionScene",
            "Loading",
            "MoneyToPointScene",
            "DonatePointsScene",
            "EndScene"
        };

        [SerializeField] private CardManager _cardManager;

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
        [SerializeField] private Button startRound;

        // things from the hostscene
        private int playerId = 0;
        private Dictionary<int, PlayerListPrefab> players;
        [SerializeField] private Transform playerListPrefab;
        [SerializeField] private GameObject playerScrollView;

        private Dictionary<int, ProgressionCard> _progressionCards;
        [SerializeField] private GameObject progressionPrefab;
        [SerializeField] private Transform progressionScrollView;

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
        [SerializeField] private TMP_Text curretnPhaseTMP;
        [SerializeField] private TMP_Text nextPhaseTMP;
        
        

        private void Start()
        {
            abortSession.onClick.AddListener(AbortSessionOnClick);
            startSession.onClick.AddListener(StartSessionOnClick);
            stopRound.onClick.AddListener(StopRoundOnClick);
            startRound.onClick.AddListener(StartRoundOnClick);

            // hostscene
            Nats.NatsHost.C.OnHeartBeat += OnOnHeartBeat;
            Nats.NatsHost.C.OnJoinrequest += OnOnJoinrequest;
            Nats.NatsHost.C.MessageLog += OnMessageLog;

            Init();
        }

        private void Init()
        {
            activities = new List<GameObject>();

            _sessionDuration = new Stopwatch();

            players = new Dictionary<int, PlayerListPrefab>();
            _progressionCards = new Dictionary<int, ProgressionCard>();
            
            playerScrollView = GameObject.Find("PlayerScrollView");
            progressionPrefab = Resources.Load<GameObject>("Prefabs/Host/ProgressionCard");


            hostNameTMP.text = SessionData.Instance.HostName;
            gameModeTMP.text = SessionData.Instance.CurrentMoneySystem.ToString();
            seedTMP.text = SessionData.Instance.Seed.ToString();
            gameCodeTMP.text =
                $"{SessionData.Instance.LobbyCode.ToString().Substring(0, 3)} {SessionData.Instance.LobbyCode.ToString().Substring(3, 3)} {SessionData.Instance.LobbyCode.ToString().Substring(6, 3)}";
            roundDurationTMP.text = SessionData.Instance.RoundDuration.ToString();
            creationTime.text = DateTime.Now.ToString("HH:mm:ss");
            _sessionDuration.Start();
            _sessionIsActive = true;

            curretnPhaseTMP.text = testPhases[current_round].Split("Scene")[0];
            nextPhaseTMP.text = testPhases[current_round + 1].Split("Scene")[0];

            for (int i = 0; i < testPhases.Length; i++)
            {
                var phaseName = testPhases[i].Split("Scene");
                var prefab = Instantiate(progressionPrefab, progressionScrollView);
                prefab.gameObject.SetActive(true);
                var card = prefab.GetComponent<ProgressionCard>();
                card.Name.text = phaseName[0];
                card.Status.text = "Not Started";
                _progressionCards.Add(i, card);
            }
        }

        private void OnMessageLog(object sender, string e)
        {
            var activity = Instantiate(activityPrefab, activityScrollView.transform);
            activity.GetComponentInChildren<TMP_Text>().text = e;
            activity.SetActive(true);

            activities.Add(activity);

            if (activities.Count > 20)
            {
                Destroy(activities[0]);
                activities.RemoveAt(0);
            }
        }

        private void OnOnJoinrequest(object sender, JoinRequestMessage msg)
        {
            if (players.Any(record => record.Value.Name == msg.PlayerName))
            {
                RejectedMessage rejectedMessage = new RejectedMessage(DateTime.Now.ToString("o"), msg.LobbyID,
                    -1, msg.PlayerName, "PlayerNameAlreadyTaken",
                    $"{msg.PlayerName} is already taken in the session you are trying to join. \n Please fill in another name and try again.",
                    msg.RequestID);

                Nats.NatsHost.C.Publish(msg.LobbyID.ToString(), rejectedMessage);
                return;
            }


            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new ConfirmJoinMessage(
                DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode,
                -1,
                playerId,
                msg.PlayerName,
                msg.Age,
                msg.Gender,
                msg.RequestID));

            var player = Instantiate(playerListPrefab, playerScrollView.transform);
            player.gameObject.SetActive(true);
            var plistprefab = player.GetComponent<PlayerListPrefab>();
            plistprefab.LastPing = DateTime.Parse(msg.DateTimeStamp);
            plistprefab.ID = msg.PlayerID;
            plistprefab.Name = msg.PlayerName;
            plistprefab.Balance = 0;
            plistprefab.Points = 0;

            players.Add(playerId, plistprefab);

            playerId++;
        }

        private void OnOnHeartBeat(object sender, HeartBeatMessage e)
        {
            Debug.Log("HEARTBEAT");

            DateTime parsedDate = DateTime.Parse(e.DateTimeStamp);

            if (!players.ContainsKey(e.PlayerID))
            {
                var player = Instantiate(playerListPrefab, playerScrollView.transform);
                player.gameObject.SetActive(true);
                var plistprefab = player.GetComponent<PlayerListPrefab>();
                plistprefab.LastPing = parsedDate;
                plistprefab.ID = e.PlayerID;
                plistprefab.Name = e.PlayerName;
                plistprefab.Balance = e.Balance;
                plistprefab.Points = e.Points;

                players.Add(e.PlayerID, plistprefab);
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
            _cardManager.StartGame(players);

            startRound.interactable = true;
        }

        private void StopRoundOnClick()
        {
            _progressionCards[current_round].Status.text = "Finished";
            
            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new StopRoundMessage(
                DateTime.Now.ToString("o"),
                SessionData.Instance.LobbyCode,
                -1,
                current_round
            ));
            current_round++;
        }

        private void StartRoundOnClick()
        {
            _progressionCards[current_round].Status.text = "Current";

            curretnPhaseTMP.text = testPhases[current_round].Split("Scene")[0];
            nextPhaseTMP.text = testPhases.Length == current_round + 1 ? "" : testPhases[current_round + 1].Split("Scene")[0];
            
            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new StartRoundMessage(
                DateTime.Now.ToString("o"),
                SessionData.Instance.LobbyCode,
                -1,
                current_round,
                testPhases[current_round],
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