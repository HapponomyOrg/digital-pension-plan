using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Version1.Phases;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;
using Debug = UnityEngine.Debug;

namespace Version1.Host.Scripts
{
    public class Host : MonoBehaviour
    {
        private int currentRound = 0;

        private readonly string[] debtBasedPhases =
        {
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.PayDebtPhase.Name, PhaseLibrary.TakeALoanPhase.Name, PhaseLibrary.LoadingPhase.Name,
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.PayDebtPhase.Name, PhaseLibrary.TakeALoanPhase.Name, PhaseLibrary.LoadingPhase.Name,
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.MoneyToPointPhase.Name, PhaseLibrary.DonatePointsPhase.Name, PhaseLibrary.EndPhase.Name
        };

        private readonly string[] sustainableMoneyPhases =
        {
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.LoadingPhase.Name,
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.LoadingPhase.Name,
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.LoadingPhase.Name,
            PhaseLibrary.MoneyToPointPhase.Name, PhaseLibrary.DonatePointsPhase.Name, PhaseLibrary.EndPhase.Name
        };

        private readonly string[] interestAtIntervalsPhases =
        {
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.PayDebtPhase.Name, PhaseLibrary.TakeALoanPhase.Name, PhaseLibrary.LoadingPhase.Name,
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.PayDebtPhase.Name, PhaseLibrary.TakeALoanPhase.Name, PhaseLibrary.LoadingPhase.Name,
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.MoneyToPointPhase.Name, PhaseLibrary.DonatePointsPhase.Name, PhaseLibrary.EndPhase.Name
        };

        private readonly string[] closedEconomyPhases =
        {
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.PayDebtPhase.Name, PhaseLibrary.TakeALoanPhase.Name, PhaseLibrary.LoadingPhase.Name,
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.PayDebtPhase.Name, PhaseLibrary.TakeALoanPhase.Name, PhaseLibrary.LoadingPhase.Name,
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.MoneyToPointPhase.Name, PhaseLibrary.DonatePointsPhase.Name, PhaseLibrary.EndPhase.Name
        };

        private readonly string[] testPhases =
        {
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.LoadingPhase.Name,
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.LoadingPhase.Name,
            PhaseLibrary.MarketPhase.Name, PhaseLibrary.MoneyCorrectionPhase.Name, PhaseLibrary.LoadingPhase.Name,
            PhaseLibrary.MoneyToPointPhase.Name, PhaseLibrary.DonatePointsPhase.Name, PhaseLibrary.EndPhase.Name
        };

        private string[] currentPhases;

        [SerializeField] private CardManager cardManager;
        [SerializeField] private GameObject natsError;
        [SerializeField] private TMP_Text natsErrorTMP;

        private Stopwatch sessionDuration;
        private bool sessionIsActive = false;
        private float timeLeft = 300f;

        // Scenes
        [SerializeField] private GameObject createScene;

        // Buttons
        [SerializeField] private Button abortSession;
        [SerializeField] private Button startSession;
        [SerializeField] private Button stopRound;
        [SerializeField] private Button startRound;
        [SerializeField] private Button endGame;

        // Player management
        private int playerId = 0;
        private Dictionary<int, PlayerListPrefab> players;
        [SerializeField] private Transform playerListPrefab;
        [SerializeField] private GameObject playerScrollView;

        // Progression cards
        private Dictionary<int, ProgressionCard> progressionCards;
        [SerializeField] private GameObject progressionPrefab;
        [SerializeField] private Transform progressionScrollView;

        // Activity log
        private List<GameObject> activities;
        [SerializeField] private GameObject activityPrefab;
        [SerializeField] private GameObject activityScrollView;

        // UI Text elements
        [SerializeField] private TMP_Text hostNameTMP;
        [SerializeField] private TMP_Text gameModeTMP;
        [SerializeField] private TMP_Text seedTMP;
        [SerializeField] private TMP_Text gameCodeTMP;
        [SerializeField] private TMP_Text roundDurationTMP;
        [SerializeField] private TMP_Text creationTime;
        [SerializeField] private TMP_Text sessionDurationTMP;
        [SerializeField] private TMP_Text timeLeftTMP;
        [SerializeField] private TMP_Text currentPhaseTMP;
        [SerializeField] private TMP_Text nextPhaseTMP;

        private bool roundStarted = false;

        private void Start()
        {
            StartCoroutine(WaitForWebSocketClient());
        }

        private IEnumerator WaitForWebSocketClient()
        {
            while (Nats.NatsHost.C?.WebSocketClient == null)
            {
                yield return new WaitForSeconds(0.1f);
            }

            Debug.Log("WebSocket client found, setting up listeners");
            Nats.NatsHost.C.WebSocketClient.clientID = -1;

            // Add WebSocket listeners once
            AddWebSocketListeners();
        }

        private void OnEnable()
        {
            ResetSessionState();
            RemoveAllListeners();
            AddAllListeners();
            InitializeUI();
        }

        private void OnDisable()
        {
            RemoveAllListeners();
        }

        private void ResetSessionState()
        {
            roundStarted = false;
            sessionIsActive = false;
            currentRound = 0;

            ClearLogs();

            currentPhases = SessionData.Instance.CurrentMoneySystem switch
            {
                MoneySystems.Sustainable => sustainableMoneyPhases,
                MoneySystems.DebtBased => debtBasedPhases,
                MoneySystems.InterestAtIntervals => interestAtIntervalsPhases,
                MoneySystems.ClosedEconomy => closedEconomyPhases,
                MoneySystems.RealisticDebtDistribution => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException()
            };

            activities = new List<GameObject>();
            sessionDuration = new Stopwatch();
            players ??= new Dictionary<int, PlayerListPrefab>();

            ClearProgressionCards();
            progressionCards = new Dictionary<int, ProgressionCard>();
        }

        private void InitializeUI()
        {
            // Button states
            abortSession.interactable = false;
            startSession.interactable = true;
            stopRound.interactable = false;
            startRound.interactable = false;

            // Find required UI elements
            playerScrollView = GameObject.Find("PlayerScrollView");
            progressionPrefab = Resources.Load<GameObject>("Prefabs/Host/ProgressionCard");

            // Reset session info
            sessionDuration.Reset();
            sessionDurationTMP.text = "";
            timeLeftTMP.text = "-";
            hostNameTMP.text = SessionData.Instance.HostName;
            gameModeTMP.text = SessionData.Instance.CurrentMoneySystem.ToString();
            seedTMP.text = SessionData.Instance.Seed.ToString();
            gameCodeTMP.text = FormatGameCode(SessionData.Instance.LobbyCode);
            roundDurationTMP.text = SessionData.Instance.RoundDuration.ToString();
            creationTime.text = DateTime.Now.ToString("HH:mm:ss");

            sessionIsActive = true;

            // Set phase info
            currentPhaseTMP.text = currentPhases[currentRound].Split("Scene")[0];
            nextPhaseTMP.text = currentPhases[currentRound + 1].Split("Scene")[0];

            // Create progression cards
            CreateProgressionCards();
        }

        private void CreateProgressionCards()
        {
            for (var i = 0; i < currentPhases.Length; i++)
            {
                var phaseName = currentPhases[i].Split("Scene");
                var prefab = Instantiate(progressionPrefab, progressionScrollView);
                prefab.gameObject.SetActive(true);
                var card = prefab.GetComponent<ProgressionCard>();
                card.Name.text = phaseName[0];
                card.Status.text = "Not Started";
                progressionCards.Add(i, card);
            }
        }

        private void RemoveAllListeners()
        {
            // Remove button listeners
            if (abortSession != null) abortSession.onClick.RemoveAllListeners();
            if (startSession != null) startSession.onClick.RemoveAllListeners();
            if (stopRound != null) stopRound.onClick.RemoveAllListeners();
            if (startRound != null) startRound.onClick.RemoveAllListeners();
            if (endGame != null) endGame.onClick.RemoveAllListeners();
        }

        private void AddAllListeners()
        {
            // Add button listeners
            if (abortSession != null) abortSession.onClick.AddListener(AbortSessionOnClick);
            if (startSession != null) startSession.onClick.AddListener(StartSessionOnClick);
            if (stopRound != null) stopRound.onClick.AddListener(StopRoundOnClick);
            if (startRound != null) startRound.onClick.AddListener(StartRoundOnClick);
            if (endGame != null) endGame.onClick.AddListener(EndGameOnClick);
        }

        private void AddWebSocketListeners()
        {
            if (Nats.NatsHost.C?.WebSocketClient == null) return;

            Nats.NatsHost.C.WebSocketClient.OnHeartBeat += OnHeartBeat;
            Nats.NatsHost.C.WebSocketClient.OnJoinrequest += OnJoinRequest;
            Nats.NatsHost.C.WebSocketClient.OnCardHandIn += OnCardHandIn;
            Nats.NatsHost.C.WebSocketClient.OnContinue += OnContinue;
        }

        private void RemoveWebSocketListeners()
        {
            if (Nats.NatsHost.C?.WebSocketClient == null) return;

            Nats.NatsHost.C.WebSocketClient.OnHeartBeat -= OnHeartBeat;
            Nats.NatsHost.C.WebSocketClient.OnJoinrequest -= OnJoinRequest;
            Nats.NatsHost.C.WebSocketClient.OnCardHandIn -= OnCardHandIn;
            Nats.NatsHost.C.WebSocketClient.OnContinue -= OnContinue;
        }

        // WebSocket Event Handlers
        private void OnContinue(object sender, ContinueMessage e)
        {
            // TODO check round number and check for every player to be ready, maybe 80% and then continue.
        }

        private void OnCardHandIn(object sender, CardHandInMessage msg)
        {
            var cards = cardManager.TakeCards(4);
            var intArray = new int[4];

            foreach (var card in cards)
            {
                intArray.Append(card.ID);
            }

            Nats.NatsHost.C.Publish(msg.LobbyID.ToString(), new ConfirmHandInMessage(
                DateTime.Now.ToString("o"), msg.LobbyID, -1, msg.PlayerID, intArray), false);
        }

        private void OnJoinRequest(object sender, JoinRequestMessage msg)
        {
            Debug.Log($"Join request received - PlayerID in message: {msg.PlayerID}, Assigning ID: {playerId}");

            if (players.Any(record =>
                    string.Equals(record.Value.Name, msg.PlayerName, StringComparison.CurrentCultureIgnoreCase)))
            {
                RejectedMessage rejectedMessage = new RejectedMessage(
                    DateTime.Now.ToString("o"), msg.LobbyID, -1, msg.PlayerName,
                    "PlayerNameAlreadyTaken",
                    $"{msg.PlayerName} is already taken in the session you are trying to join. \n Please fill in another name and try again.",
                    msg.RequestID);

                Nats.NatsHost.C.Publish(msg.LobbyID.ToString(), rejectedMessage);
                return;
            }

            // Publish confirmation
            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new ConfirmJoinMessage(
                DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, -1,
                playerId, msg.PlayerName, msg.Age, msg.Gender, msg.RequestID));

            // Create player UI
            CreatePlayerUI(msg, playerId);
            playerId++;
        }

        private void CreatePlayerUI(JoinRequestMessage msg, int assignedPlayerId)
        {
            if (playerScrollView == null)
            {
                Debug.LogError("playerScrollView is NULL! Attempting to find it...");
                playerScrollView = GameObject.Find("PlayerScrollView");
                if (playerScrollView == null)
                {
                    Debug.LogError("Still cannot find PlayerScrollView!");
                    return;
                }
            }

            var player = Instantiate(playerListPrefab, playerScrollView.transform);
            if (player == null)
            {
                Debug.LogError("Failed to instantiate player prefab!");
                return;
            }

            player.gameObject.SetActive(true);
            var plistprefab = player.GetComponent<PlayerListPrefab>();

            if (plistprefab == null)
            {
                Debug.LogError("PlayerListPrefab component not found on instantiated object!");
                return;
            }

            plistprefab.LastPing = DateTime.Parse(msg.DateTimeStamp);
            plistprefab.ID = assignedPlayerId;
            plistprefab.Name = msg.PlayerName;
            plistprefab.Balance = 0;
            plistprefab.Points = 0;

            Debug.Log($"Adding player to dictionary - ID: {assignedPlayerId}, Name: {msg.PlayerName}");
            players.Add(assignedPlayerId, plistprefab);
            Debug.Log($"Dictionary now contains {players.Count} players");
        }

        private void OnHeartBeat(object sender, HeartBeatMessage e)
        {
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

        // Button Click Handlers
        private void AbortSessionOnClick()
        {
            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(),
                new AbortSessionMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, -1));

            SessionData.Instance.Reset(true);
            createScene.SetActive(true);
            gameObject.SetActive(false);
        }

        private void StartSessionOnClick()
        {
            Debug.Log($"Starting session with {players.Count} players");
            foreach (var player in players)
            {
                Debug.Log($"Player {player.Key}: {player.Value.Name}");
            }

            cardManager.StartGame(players);
            startRound.interactable = true;
            startSession.interactable = false;
            abortSession.interactable = true;
            sessionDuration.Start();
        }

        private void StopRoundOnClick()
        {
            progressionCards[currentRound].Status.text = "Finished";

            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(),
                new StopRoundMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, -1, currentRound));

            currentRound++;
            roundStarted = false;
        }

        private void StartRoundOnClick()
        {
            progressionCards[currentRound].Status.text = "Current";

            currentPhaseTMP.text = currentPhases[currentRound].Split("Scene")[0];
            nextPhaseTMP.text = currentPhases.Length == currentRound + 1
                ? ""
                : currentPhases[currentRound + 1].Split("Scene")[0];

            if (currentPhases[currentRound].Contains("Market"))
            {
                timeLeft = 300;
            }

            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new StartRoundMessage(
                DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, -1,
                currentRound, currentPhases[currentRound], 100));

            roundStarted = true;
        }

        private void EndGameOnClick()
        {
            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(),
                new EndGameMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, -1));

            SessionData.Instance.Reset(false);
            gameObject.SetActive(false);
            createScene.SetActive(true);
        }

        // Utility Methods
        public void ClearLogs()
        {
            if (activities != null)
            {
                foreach (var activity in activities)
                {
                    Destroy(activity.gameObject);
                }

                activities.Clear();
            }
        }

        private void ClearProgressionCards()
        {
            if (progressionCards == null) return;
            foreach (var card in progressionCards.Values)
            {
                Destroy(card.gameObject);
            }

            progressionCards.Clear();
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

        private string FormatGameCode(int code)
        {
            string codeStr = code.ToString();
            return $"{codeStr.Substring(0, 3)} {codeStr.Substring(3, 3)} {codeStr.Substring(6, 3)}";
        }

        private void UpdateTimerDisplay()
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60);
            int seconds = Mathf.FloorToInt(timeLeft % 60);
            timeLeftTMP.text = $"{minutes:D2}:{seconds:D2}";
        }

        private void Update()
        {
            // Handle end game button visibility
            if (currentRound <= currentPhases.Length)
            {
                endGame.gameObject.SetActive(currentRound == currentPhases.Length);
                if (currentRound == currentPhases.Length)
                {
                    startRound.interactable = false;
                }
            }

            // Handle round timer
            if (currentRound < currentPhases.Length && roundStarted)
            {
                if (currentPhases[currentRound].Contains("Market"))
                {
                    if (timeLeft > 0)
                    {
                        timeLeft -= Time.deltaTime;
                        UpdateTimerDisplay();
                    }
                    else
                    {
                        timeLeft = 0;
                        StopRoundOnClick();
                    }
                }
                else
                {
                    timeLeftTMP.text = "-";
                }
            }
            else
            {
                timeLeftTMP.text = "-";
            }

            Nats.NatsHost.C.HandleMessages();
        }

        private void OnDestroy()
        {
            RemoveWebSocketListeners();
        }
    }
}
