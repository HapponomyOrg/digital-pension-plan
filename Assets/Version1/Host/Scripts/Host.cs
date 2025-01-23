using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;
using Random = UnityEngine.Random;

namespace Version1.Host.Scripts
{
    public class Host : MonoBehaviour
    {
        private int Lobby;

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



        private void Start()
        {
            
            new Nats.NatsHost();

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
            Nats.NatsHost.C.Publish(Lobby.ToString(), new ConfirmJoinMessage(DateTime.Now.ToString("o"), Lobby,
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
                
                players.Add(e.PlayerID,player.GetComponent<playerlistprefab>());
            }
            else
            {
                players[e.PlayerID].LastPing = parsedDate;
                players[e.PlayerID].Name = e.PlayerName;
                players[e.PlayerID].Balance = e.Balance;
                players[e.PlayerID].Points = e.Points;
            }
            
            //TODO misschien willen we dit hieronder op een andere plek maar ik vind de update een beetje overdreven
            
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

        private void CreateSessionOnClick()
        {
            Nats.NatsHost.C.Publish(Lobby.ToString(), new CreateSessionMessage(DateTime.Now.ToString("o"), Lobby,
                -1,
                Lobby));
            Nats.NatsHost.C.SubscribeToSubject(Lobby.ToString());

            players = new Dictionary<int, playerlistprefab>();

            HostScene.SetActive(true);
            CreateScene.SetActive(false);
            
            playerScrolView = GameObject.Find("PlayerScrollView");
        }


        private void RegenerateButtonOnClick()
        {
            Lobby = Random.Range(100000000, 999999999);

            gameCodeInputField.text =
                $"{Lobby.ToString().Substring(0, 3)} {Lobby.ToString().Substring(3, 3)} {Lobby.ToString().Substring(6, 3)}";
        }

        private void EditButtonOnClick()
        {
            editButton.image.sprite = editButton.image.sprite == penSprite ? checkMarkSprite : penSprite;

            seedInputField.interactable = !seedInputField.interactable;
        }

        private void AbortSessionOnClick()
        {
            Nats.NatsHost.C.Publish(Lobby.ToString(), new AbortSessionMessage(DateTime.Now.ToString("o"), Lobby,
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
            Nats.NatsHost.C.Publish(Lobby.ToString(), new StopRoundMessage(
                DateTime.Now.ToString("o"),
                Lobby,
                -1,
                1 //TODO CHANGE TO ROUND NUMBER OR CHANGE TO STRING WITH PHASE NAME
            ));
        }

        private void ContinueOnClick()
        {
            Nats.NatsHost.C.Publish(Lobby.ToString(), new SkipRoundMessage(
                DateTime.Now.ToString("o"),
                Lobby,
                -1));
        }

        private void StartRoundOnClick()
        {
            Nats.NatsHost.C.Publish(Lobby.ToString(), new StartRoundMessage(
                DateTime.Now.ToString("o"),
                Lobby,
                -1,
                1, //TODO CHANGE TO ROUND NUMBER OR PHASE NAME
                100 // TODO CHANGE TO DURATION OF PHASE, GET FROM PHASE SYSTEM OF LUUK
            ));
        }

        private void Update()
        {
                Nats.NatsHost.C.HandleMessages();
        }
    }
}