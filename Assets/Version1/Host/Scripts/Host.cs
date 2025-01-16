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
        private Dictionary<int, PlayerInfo> players;
        
        
        
        
        
        


        private void Start()
        {
            new Nats.NatsHost();
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
        }

        private void OnOnJoinrequest(object sender, JoinRequestMessage e)
        {
            Nats.NatsHost.C.Publish(Lobby.ToString(),new ConfirmJoinMessage(DateTime.Now.ToString("o"), Lobby,
                -1,
                playerId,
                e.PlayerName,
                e.Age,
                e.Gender));
            
            players.Add(playerId,new  PlayerInfo(e.PlayerName,playerId,new List<int>(),DateTime.Now));
            
        }

        private void OnOnHeartBeat(object sender, HeartBeatMessage e)
        {
            // TODO heartbeat logic
        }

        private void CreateSessionOnClick()
        {
            Nats.NatsHost.C.Publish(Lobby.ToString(), new CreateSessionMessage(DateTime.Now.ToString("o"), Lobby,
                -1,
                Lobby));
            Nats.NatsHost.C.SubscribeToSubject(Lobby.ToString());
            
            players = new Dictionary<int, PlayerInfo>();
            
            HostScene.SetActive(true);
            CreateScene.SetActive(false);
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
    }

    struct PlayerInfo
    {
        public string Name;
        public int Id;
        public List<int> Score;
        public DateTime LastContact;

        public PlayerInfo(string name, int id, List<int> score, DateTime lastContact)
        {
            Name = name;
            Id = id;
            Score = score;
            LastContact = lastContact;
        }
    } 
}