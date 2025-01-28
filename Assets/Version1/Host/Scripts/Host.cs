using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;
using Random = UnityEngine.Random;

namespace Version1.Host.Scripts
{
    public class Host : MonoBehaviour
    {
        [SerializeField] private GameObject natsError;
        [SerializeField] private TMP_Text natsErrorTMP;
        
        private string _hostName = "";
        private int _seed = 0;
        private int _gameMode = 0; //TODO change this to enum types
        private int _lobby = 0;

        private int _roundDuration = 4; // in minutes
        private Stopwatch _sessionDuration;
        private bool _sessionIsActive = false;


        // CreateScene
        [SerializeField] private GameObject CreateScene;
        [SerializeField] private GameObject HostScene;

        [SerializeField] private Sprite checkMarkSprite;
        [SerializeField] private Sprite penSprite;

        [SerializeField] private TMP_InputField hostInputField;

        [SerializeField] private Button editButton;
        [SerializeField] private TMP_InputField seedInputField;

        [SerializeField] private TMP_Dropdown gameModeDropDown;

        [SerializeField] private Button regenerateButton;
        [SerializeField] private TMP_InputField gameCodeInputField;

        [SerializeField] private Button createSession;

        [SerializeField] private Button abortSession;
        [SerializeField] private Button startSession;
        [SerializeField] private Button stopRound;
        [SerializeField] private Button skipRound;
        [SerializeField] private Button startRound;

        private int playerId = 0;
        private Dictionary<int, playerlistprefab> players;
        [SerializeField] private Transform playerListPrefab;
        [SerializeField] private GameObject playerScrolView;

        private List<GameObject> activities;
        [SerializeField] private GameObject activityPrefab;
        [SerializeField] private GameObject activityScrollView;

        [SerializeField] private GameObject seedInputError;
        [SerializeField] private GameObject gameCodeError;
        


        // HostScene

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
            new Nats.NatsHost();
            
            Nats.NatsHost.C.onError += (sender, s) =>
            {
                natsError.SetActive(true);
                natsErrorTMP.text = s;
            };

            Nats.NatsHost.C.Connect();
            
            activities = new List<GameObject>();
            editButton.onClick.AddListener(EditButtonOnClick);
            regenerateButton.onClick.AddListener(RegenerateButtonOnClick);
            createSession.onClick.AddListener(CreateSessionOnClick);

            abortSession.onClick.AddListener(AbortSessionOnClick);
            startSession.onClick.AddListener(StartSessionOnClick);
            stopRound.onClick.AddListener(StopRoundOnClick);
            skipRound.onClick.AddListener(ContinueOnClick);
            startRound.onClick.AddListener(StartRoundOnClick);

            Nats.NatsHost.C.OnHeartBeat += OnOnHeartBeat;
            Nats.NatsHost.C.OnJoinrequest += OnOnJoinrequest;
            Nats.NatsHost.C.MessageLog += OnMessageLog;

            hostInputField.onValueChanged.AddListener((val) => { _hostName = val; });

            seedInputField.onValueChanged.AddListener((val) => {
                if (int.TryParse(val, out int result))
                {
                    _seed = result;
                    seedInputError.SetActive(false);
                }
                else
                {
                   seedInputError.SetActive(true);
                }});

            gameModeDropDown.onValueChanged.AddListener((val) => { _gameMode = val; });
            
            gameCodeInputField.onValueChanged.AddListener((val) =>
                {

                    var str = val.Replace(" ", "");
                    
                    if (int.TryParse(str, out int result))
                    {
                        // Check if the input contains only digits (0-9)
                        if (!System.Text.RegularExpressions.Regex.IsMatch(str, @"^\d*$"))
                        {
                            gameCodeError.SetActive(true);
                        }
                        
                        _lobby = result;
                        gameCodeError.SetActive(false);
                    }
                    else
                    {
                        gameCodeError.SetActive(true);
                    }
                })
                ;

            _sessionDuration = new Stopwatch();

            _hostName = "";
            _seed = 0;
            _gameMode = 0;
            _lobby = Random.Range(100000000, 999999999);
            
            gameCodeInputField.text =
                $"{_lobby.ToString().Substring(0, 3)} {_lobby.ToString().Substring(3, 3)} {_lobby.ToString().Substring(6, 3)}";
            
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
            Nats.NatsHost.C.Publish(_lobby.ToString(), new ConfirmJoinMessage(DateTime.Now.ToString("o"), _lobby,
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

        private void CreateSessionOnClick()
        {
            Nats.NatsHost.C.Publish(_lobby.ToString(), new CreateSessionMessage(DateTime.Now.ToString("o"), _lobby,
                -1,
                _lobby));
            Nats.NatsHost.C.SubscribeToSubject(_lobby.ToString());

            players = new Dictionary<int, playerlistprefab>();

            HostScene.SetActive(true);
            CreateScene.SetActive(false);

            playerScrolView = GameObject.Find("PlayerScrollView");

            hostNameTMP.text = _hostName;
            gameModeTMP.text = _gameMode.ToString(); //TODO enum type oid
            seedTMP.text = _seed.ToString();
            gameCodeTMP.text =
                $"{_lobby.ToString().Substring(0, 3)} {_lobby.ToString().Substring(3, 3)} {_lobby.ToString().Substring(6, 3)}";
            roundDurationTMP.text = _roundDuration.ToString();
            creationTime.text = DateTime.Now.ToString("HH:mm:ss");
            _sessionDuration.Start();
            _sessionIsActive = true;
        }


        private void RegenerateButtonOnClick()
        {
            _lobby = Random.Range(100000000, 999999999);

            gameCodeInputField.text =
                $"{_lobby.ToString().Substring(0, 3)} {_lobby.ToString().Substring(3, 3)} {_lobby.ToString().Substring(6, 3)}";
        }

        private void EditButtonOnClick()
        {
            editButton.image.sprite = editButton.image.sprite == penSprite ? checkMarkSprite : penSprite;

            seedInputField.interactable = !seedInputField.interactable;
        }

        private void AbortSessionOnClick()
        {
            Nats.NatsHost.C.Publish(_lobby.ToString(), new AbortSessionMessage(DateTime.Now.ToString("o"), _lobby,
                -1));
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
            Nats.NatsHost.C.Publish(_lobby.ToString(), new StopRoundMessage(
                DateTime.Now.ToString("o"),
                _lobby,
                -1,
                1 //TODO CHANGE TO ROUND NUMBER OR CHANGE TO STRING WITH PHASE NAME
            ));
        }

        private void ContinueOnClick()
        {
            Nats.NatsHost.C.Publish(_lobby.ToString(), new SkipRoundMessage(
                DateTime.Now.ToString("o"),
                _lobby,
                -1));
        }

        private void StartRoundOnClick()
        {
            Nats.NatsHost.C.Publish(_lobby.ToString(), new StartRoundMessage(
                DateTime.Now.ToString("o"),
                _lobby,
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

            if (_hostName == "" || _lobby == 0)
            {
                createSession.interactable = false;
            }
            else
            {
                createSession.interactable = true;
            }


            Nats.NatsHost.C.HandleMessages();
        }
    }
}