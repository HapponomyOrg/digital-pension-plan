using System;
using System.Collections.Generic;
using System.Threading;
using NATS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Host
{
    [Obsolete]
    public class CreateSession : MonoBehaviour
    {
        [SerializeField] private Deck.Deck deck;
        [SerializeField] private PlayerRecord Record;
        [SerializeField] private Transform RecordList;
        [SerializeField] private TMP_Text LobbyID;
        [SerializeField] private TMP_Text AmountofPlayers;
        [SerializeField] private Button InBalanceButton;
        [SerializeField] private Button BalanceButton;
        [SerializeField] private TMP_Dropdown DropDown;


        [SerializeField] public GameObject HostSessionCanvas;

        private Dictionary<int, PlayerRecord> PlayerRecords = new Dictionary<int, PlayerRecord>();
        private List<int> keysToRemove = new List<int>();
        private CancellationTokenSource cancellationTokenSource;

        public int Lobby = 0;

        // -1 is reserved for the host
        private byte playerNumber = 1;

        /*
         *  BalanceMode is a setting for the host that can toggle a balanced system or an unbalanced system
         */
        private bool InBalanceMode = false;

        /*
         *  Interest is a setting for the host that can toggle between different interest calculations.
         */
        private int InterestMode = 0;

        public void StartGame()
        {
            NatsHost.C.LobbyID = Lobby;
            NatsHost.C.numGames++;
            deck.StartGame(PlayerRecords.Count, InterestMode, InBalanceMode);
            HostSessionCanvas.SetActive(true);
            gameObject.SetActive(false);
        }

        public void CreateLobby()
        {
            Lobby = Random.Range(100000000, 999999999);
            NatsHost.C.SubscribeToSubject(Lobby.ToString());
            PlayerRecords.Clear();
            keysToRemove.Clear();
            playerNumber = 1;

            LobbyID.text =
                $"{Lobby.ToString().Substring(0, 3)} {Lobby.ToString().Substring(3, 3)} {Lobby.ToString().Substring(6, 3)}";


            var msg = new CreateSessionMessage(DateTime.Now.ToString("o"), Lobby,
                playerNumber,
                Lobby);

            NatsHost.C.Publish(Lobby.ToString(), msg);

            NatsHost.C.OnJoinrequest += HandleJoinSession;
        }

        private void Start()
        {
            new NatsHost();

            NatsHost.C.OnHeartBeat += HandleHeartbeat;

            BalanceButton.onClick.AddListener(() =>
            {
                BalanceButton.gameObject.SetActive(false);
                InBalanceMode = false;
                InBalanceButton.gameObject.SetActive(true);
                Debug.Log("False");
            });
            InBalanceButton.onClick.AddListener(() =>
            {
                BalanceButton.gameObject.SetActive(true);
                InBalanceMode = true;
                InBalanceButton.gameObject.SetActive(false);
                Debug.Log("True");
            });

            DropDown.onValueChanged.AddListener(val => InterestMode = val);
        }

        private void Update()
        {
            AmountofPlayers.text = PlayerRecords.Count.ToString();

            NatsHost.C.HandleMessages();

            foreach (var record in PlayerRecords)
            {
                TimeSpan timeSinceLastContact = DateTime.Now - record.Value.LastContact;
                if (timeSinceLastContact.TotalSeconds > 20)
                {
                    Debug.Log("Removed " + record.Value.PlayerName + " " + record.Value.LastContact);
                    keysToRemove.Add(record.Key);
                    Destroy(record.Value.gameObject);
                }
            }

            //TODO() THIS IS A HUGE AMOUNT OF WORK but if a certain ID is removed we want to rearrange the amount of players.
            foreach (var key in keysToRemove)
            {
                PlayerRecords.Remove(key);
            }
        }

        void HandleJoinSession(object sender, JoinRequestMessage msg)
        {
            Debug.Log(msg);
            if (msg.LobbyID != Lobby)
            {
                Debug.Log("Lobby is not the same");
                return;
            }

            foreach (var record in PlayerRecords)
            {
                if (record.Value.PlayerName.text == msg.PlayerName)
                {
                    RejectedMessage rejectedMessage = new RejectedMessage(DateTime.Now.ToString("o"), msg.LobbyID,
                        -1, msg.PlayerName, "PlayerNameAlreadyTaken",
                        $"{msg.PlayerName} is already taken in the session you are trying to join. \n Please fill in another name and try again.");

                    NatsHost.C.Publish(msg.LobbyID.ToString(), rejectedMessage);
                    return;
                }
            }

            // Checks if player already exists
            while (PlayerRecords.ContainsKey(playerNumber)) playerNumber++;
            HeartBeatMessage heartBeatMessage = new HeartBeatMessage(DateTime.Now.ToString("o"), msg.LobbyID,
                playerNumber,
                msg.PlayerName, 0, new int[] { 0, 0, 0, 0 }, 0, Array.Empty<int>());

            CreateRecord(heartBeatMessage);

            ConfirmJoinMessage confirmMsg =
                new ConfirmJoinMessage(DateTime.Now.ToString("o"), msg.LobbyID, -1, playerNumber, msg.PlayerName, msg.Age, msg.Gender);
            NatsHost.C.Publish(Lobby.ToString(), confirmMsg);
            playerNumber++;
        }

        void HandleHeartbeat(object sender, HeartBeatMessage msg)
        {
            if (Lobby != msg.LobbyID)
            {
                Debug.LogWarning("Lobby id of heartbeat is not the same");
                return;
            }

            if (!PlayerRecords.ContainsKey(msg.PlayerID))
            {
                CreateRecord(msg);
            }
            else
            {
                PlayerRecords[msg.PlayerID].LastContact = DateTime.Parse(msg.DateTimeStamp);
            }
        }

        private void CreateRecord(HeartBeatMessage msg)
        {
            var record = Instantiate(Record, RecordList, true);
            record.gameObject.SetActive(true);
            record.PlayerID.text = msg.PlayerID.ToString();
            record.PlayerName.text = msg.PlayerName;
            record.LastContact = DateTime.Parse(msg.DateTimeStamp);
            PlayerRecords.Add(msg.PlayerID, record);
        }
    }
}
