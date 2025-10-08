using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

        private readonly string[] debtBasedPhases =
        {
            "MarketScene", // Trading
            "PayDeptScene", // Debt payment
            "TakeALoanScene", // Take out loan
            "Loading",
            "MarketScene", // Trading
            "PayDeptScene", // Debt payment
            "TakeALoanScene", // Take out loan
            "Loading",
            "MarketScene", // Trading
            "MoneyCorrectionScene", // TODO money correction the loan + 10%, if the player doesnt have enough, it is taken from their points
            "MoneyToPointScene", // Pension point buying
            "DonatePointsScene", // Pension point donation
            "EndScene" // Pension Calculation
        };

        private readonly string[] sustainableMoneyPhases =
        {
            "MarketScene", // Trading
            "MoneyCorrectionScene", // Money correction
            "Loading",
            "MarketScene", // Trading
            "MoneyCorrectionScene", // Money correction
            "Loading",
            "MarketScene", // Trading
            "MoneyCorrectionScene", // Money correction
            "Loading",
            "MoneyToPointScene", // Pension point buying
            "DonatePointsScene", // Pension point donation
            "EndScene" // Pension Calculation
        };

        private readonly string[] interestAtIntervalsPhases =
        {
            "MarketScene", // Trading
            "MoneyCorrectionScene", // TODO pay 10% of current debt, at least 1000. fractions to next round, failure to pay added to debt.
            "PayDeptScene", // Debt payment
            "TakeALoanScene", // Take out loan
            "Loading",
            "MarketScene", // Trading
            "MoneyCorrectionScene", // TODO pay 10% of current debt, at least 1000. fractions to next round, failure to pay added to debt.
            "PayDeptScene", // Debt payment
            "TakeALoanScene", // Take out loan
            "Loading",
            "MarketScene", // Trading
            "MoneyCorrectionScene", // TODO money correction the debt + 10% plus the remaining interest from the previous round. if the player doesnt have enough, it is taken from their points
            "MoneyToPointScene", // Pension point buying
            "DonatePointsScene", // Pension point donation
            "EndScene" // Pension Calculation
        };

        private readonly string[] closedEconomyPhases =
        {
            "MarketScene", // Trading
            "MoneyCorrectionScene", // TODO pay 10% of current debt, at least 1000. fractions to next round, failure to pay added to debt. SENT TO BANK PLAYER
            "PayDeptScene", // Debt payment TODO if not the bank
            "TakeALoanScene", // Take out loan TODO if not the bank
            "Loading",
            "MarketScene", // Trading 
            "MoneyCorrectionScene", // TODO pay 10% of current debt, at least 1000. fractions to next round, failure to pay added to debt. SENT TO BANK PLAYER
            "PayDeptScene", // Debt payment TODO if not the bank
            "TakeALoanScene", // Take out loan TODO if not the bank
            "Loading",
            "MarketScene", // Trading
            "MoneyCorrectionScene", // TODO money correction the debt + 10% plus the remaining interest from the previous round. if the player doesnt have enough, it is taken from their points
            "MoneyToPointScene", // Pension point buying
            "DonatePointsScene", // Pension point donation
            "EndScene" // Pension Calculation
        };

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

        private string[] currentPhases;

        [SerializeField] private CardManager _cardManager;

        [SerializeField] private GameObject natsError;
        [SerializeField] private TMP_Text natsErrorTMP;

        private Stopwatch _sessionDuration;
        private bool _sessionIsActive = false;

        private float _timeLeft = 300f;

        // scenes
        [SerializeField] private GameObject CreateScene;

        // things from the hostscene
        [SerializeField] private Button abortSession;
        [SerializeField] private Button startSession;
        [SerializeField] private Button stopRound;
        [SerializeField] private Button startRound;
        [SerializeField] private Button endGame;

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

        private bool roundStarted = false;

        private void Start()
        {
            // Set up UI listeners immediately
            if (abortSession != null) abortSession.onClick.AddListener(AbortSessionOnClick);
            if (startSession != null) startSession.onClick.AddListener(StartSessionOnClick);
            if (stopRound != null) stopRound.onClick.AddListener(StopRoundOnClick);
            if (startRound != null) startRound.onClick.AddListener(StartRoundOnClick);
            if (endGame != null) endGame.onClick.AddListener(EndGameOnClick);

            // Wait for WebSocket client to be ready
            StartCoroutine(WaitForWebSocketClient());
        }

        private IEnumerator WaitForWebSocketClient()
        {
            // Wait until the WebSocket client is available
            while (Nats.NatsHost.C?.WebSocketClient == null)
            {
                yield return new WaitForSeconds(0.1f); // Check every 100ms
            }

            Debug.Log("WebSocket client found, setting up listeners");

            Nats.NatsHost.C.WebSocketClient.clientID = -1;

            // Set up event listeners once the client is ready
            Nats.NatsHost.C.WebSocketClient.OnHeartBeat += OnOnHeartBeat;
            Nats.NatsHost.C.WebSocketClient.OnJoinrequest += OnOnJoinrequest;
            Nats.NatsHost.C.WebSocketClient.OnCardHandIn += OnCardHandIn;
            Nats.NatsHost.C.WebSocketClient.OnContinue += OnOnContinue;
        }

        private void OnOnContinue(object sender, ContinueMessage e)
        {
            // TODO check round number and check for every player to be ready, maybe 80% and then continue.
        }

        private void EndGameOnClick()
        {
            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(),
                new EndGameMessage(DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode, -1));

            SessionData.Instance.Reset(false);

            // TODO
            // Player to endscreen loading screen where they get info that the host is setting up the next round.

            this.gameObject.SetActive(false);
            CreateScene.SetActive(true);
            // Host is sent to create screen again.
        }

        private void OnCardHandIn(object sender, CardHandInMessage msg)
        {
            var cards = _cardManager.TakeCards(4);

            int[] intArray = new int[4];

            foreach (var card in cards)
            {
                intArray.Append(card.ID);
            }


            Nats.NatsHost.C.Publish(msg.LobbyID.ToString(), new ConfirmHandInMessage(DateTime.Now.ToString("o"),
                msg.LobbyID,
                -1, msg.PlayerID, intArray), false);
        }

        public void ClearLogs()
        {
            if (activities != null)
            {
                for (int i = 0; i < activities.Count; i++)
                {
                    Destroy(activities[i].gameObject);
                }

                activities.Clear();
            }
        }

        private void OnEnable()
        {
            abortSession.interactable = false;
            startSession.interactable = true;
            stopRound.interactable = false;
            startRound.interactable = false;

            roundStarted = false;
            _sessionIsActive = false;

            ClearLogs();

            current_round = 0;

            currentPhases = SessionData.Instance.CurrentMoneySystem switch
            {
                MoneySystems.Sustainable => sustainableMoneyPhases,
                MoneySystems.DebtBased => debtBasedPhases,
                MoneySystems.InterestAtIntervals => throw new NotImplementedException(),
                MoneySystems.ClosedEconomy => throw new NotImplementedException(),
                MoneySystems.RealisticDebtDistribution => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException()
            };

            activities = new List<GameObject>();

            _sessionDuration = new Stopwatch();

            players ??= new Dictionary<int, PlayerListPrefab>();

            if (_progressionCards != null)
            {
                for (int i = 0; i < _progressionCards.Count; i++)
                {
                    Destroy(_progressionCards[i].gameObject);
                }

                _progressionCards.Clear();
            }

            _progressionCards = new Dictionary<int, ProgressionCard>();

            playerScrollView = GameObject.Find("PlayerScrollView");
            progressionPrefab = Resources.Load<GameObject>("Prefabs/Host/ProgressionCard");

            _sessionDuration.Reset();
            sessionDurationTMP.text = "";
            timeLeftTMP.text = "-";
            hostNameTMP.text = SessionData.Instance.HostName;
            gameModeTMP.text = SessionData.Instance.CurrentMoneySystem.ToString();
            seedTMP.text = SessionData.Instance.Seed.ToString();
            gameCodeTMP.text =
                $"{SessionData.Instance.LobbyCode.ToString().Substring(0, 3)} {SessionData.Instance.LobbyCode.ToString().Substring(3, 3)} {SessionData.Instance.LobbyCode.ToString().Substring(6, 3)}";
            roundDurationTMP.text = SessionData.Instance.RoundDuration.ToString();
            creationTime.text = DateTime.Now.ToString("HH:mm:ss");
            _sessionIsActive = true;

            curretnPhaseTMP.text = currentPhases[current_round].Split("Scene")[0];
            nextPhaseTMP.text = currentPhases[current_round + 1].Split("Scene")[0];

            for (int i = 0; i < currentPhases.Length; i++)
            {
                var phaseName = currentPhases[i].Split("Scene");
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
            Debug.Log($"Join request received - PlayerID in message: {msg.PlayerID}, Assigning ID: {playerId}");

            if (players.Any(record => record.Value.Name == msg.PlayerName))
            {
                RejectedMessage rejectedMessage = new RejectedMessage(DateTime.Now.ToString("o"), msg.LobbyID,
                    -1, msg.PlayerName, "PlayerNameAlreadyTaken",
                    $"{msg.PlayerName} is already taken in the session you are trying to join. \n Please fill in another name and try again.",
                    msg.RequestID);

                Nats.NatsHost.C.Publish(msg.LobbyID.ToString(), rejectedMessage);
                return;
            }

            // Publish the confirmation
            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new ConfirmJoinMessage(
                DateTime.Now.ToString("o"), SessionData.Instance.LobbyCode,
                -1,
                playerId,
                msg.PlayerName,
                msg.Age,
                msg.Gender,
                msg.RequestID));

            // Check if UI elements are ready
            if (playerScrollView == null)
            {
                Debug.LogError("playerScrollView is NULL! Cannot instantiate player UI.");
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
            plistprefab.ID = playerId; // Use the assigned ID, not msg.PlayerID
            plistprefab.Name = msg.PlayerName;
            plistprefab.Balance = 0;
            plistprefab.Points = 0;

            Debug.Log($"Adding player to dictionary - ID: {playerId}, Name: {msg.PlayerName}");
            players.Add(playerId, plistprefab);
            Debug.Log($"Dictionary now contains {players.Count} players");

            playerId++;
        }

        private void OnOnHeartBeat(object sender, HeartBeatMessage e)
        {
            //   TODO Timestamp parsing gaat niet goed, deze is null
            // Debug.LogWarning(e.DateTimeStamp);

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

            SessionData.Instance.Reset(true);

            CreateScene.SetActive(true);

            gameObject.SetActive(false);
        }

        private void StartSessionOnClick()
        {
            Debug.Log($"Starting session with {players.Count} players");
            foreach (var player in players)
            {
                Debug.Log($"Player {player.Key}: {player.Value.Name}");
            }

            _cardManager.StartGame(players);
            startRound.interactable = true;
            startSession.interactable = false;
            abortSession.interactable = true;
            _sessionDuration.Start();
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

            roundStarted = false;
        }

        private void StartRoundOnClick()
        {
            _progressionCards[current_round].Status.text = "Current";

            curretnPhaseTMP.text = currentPhases[current_round].Split("Scene")[0];
            nextPhaseTMP.text = currentPhases.Length == current_round + 1
                ? ""
                : currentPhases[current_round + 1].Split("Scene")[0];

            // TODO change this
            if (currentPhases[current_round].Contains("Market"))
            {
                _timeLeft = 300; // TODO get this from a variable
            }

            Nats.NatsHost.C.Publish(SessionData.Instance.LobbyCode.ToString(), new StartRoundMessage(
                DateTime.Now.ToString("o"),
                SessionData.Instance.LobbyCode,
                -1,
                current_round,
                currentPhases[current_round],
                100 // TODO CHANGE TO DURATION OF PHASE, GET FROM PHASE SYSTEM OF LUUK
            ));

            roundStarted = true;
        }

        private void Update()
        {
            if (current_round <= currentPhases.Length)
            {
                endGame.gameObject.SetActive(current_round == currentPhases.Length);
                if (current_round == currentPhases.Length)
                {
                    startRound.interactable = false;
                }
            }

            if (current_round < currentPhases.Length && roundStarted)
            {
                if (currentPhases[current_round].Contains("Market"))
                {
                    // TODO check if that round is active
                    if (_timeLeft > 0)
                    {
                        _timeLeft -= Time.deltaTime;
                        UpdateTimerDisplay();
                    }
                    else
                    {
                        _timeLeft = 0;
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

            /*if (_sessionIsActive)
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
            }*/

            Nats.NatsHost.C.HandleMessages();
        }

        void UpdateTimerDisplay()
        {
            int minutes = Mathf.FloorToInt(_timeLeft / 60);
            int seconds = Mathf.FloorToInt(_timeLeft % 60);
            timeLeftTMP.text = $"{minutes:D2}:{seconds:D2}";
        }
    }
}