using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Version1.Host.Scripts;
using Version1.Nats.Messages.Client;
using Version1.Utilities;

namespace Version1.Phases.DonatePoints.scripts
{
    public class DonatePoints : MonoBehaviour
    {
        [SerializeField] private TMP_Text ownPointsTMP;
        [SerializeField] private TMP_Text otherPointsTMP;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text otherNameTMP;

        [SerializeField] private Button increaseButton;
        [SerializeField] private Button decreaseButton;

        [SerializeField] private Button donateButton;

        private int _pointsToDonate;

        private PlayerData.PlayerData _otherPlayer = new PlayerData.PlayerData();

        private Dictionary<int, PlayerListPrefab> _players;
        [SerializeField] private Transform playerListPrefab;
        [SerializeField] private Transform playerScrollView;

        private int _ownPoints;

        private int OwnPoints
        {
            get => _ownPoints;
            set
            {
                _ownPoints = value;
                ownPointsTMP.text = _ownPoints.ToString();
            }
        }

        private int _otherPoints;

        private int OtherPoints
        {
            get => _otherPoints;
            set
            {
                _otherPoints = value;
                otherPointsTMP.text = _otherPoints.ToString();
            }
        }

        private string _otherName;

        private string OtherName
        {
            get => _otherName;
            set
            {
                _otherName = value;
                otherNameTMP.text = _otherName;
            }
        }

        private void Start()
        {
            playerListPrefab = Resources.Load<Transform>("Prefabs/Phases/DonatePoints/PlayerlistPrefab");
            playerScrollView = GameObject.Find("PlayerScrollView").transform;
            
            Nats.NatsClient.C.OnDonatePoints += OnOnDonatePoints;

            Nats.NatsClient.C.OnHeartBeat += OnOnHeartBeat;
            otherNameTMP.text = "";
            otherPointsTMP.text = "0";

            _players = new Dictionary<int, PlayerListPrefab>();

            OwnPoints = PlayerData.PlayerData.Instance.Points;

            descriptionText.text = OwnPoints ! >= 1
                ? "Please click on another player if you want to donate your point?"
                : "Please click on another player if you want to donate one of your points?";

            increaseButton.onClick.RemoveAllListeners();
            decreaseButton.onClick.RemoveAllListeners();
            donateButton.onClick.RemoveAllListeners();

            increaseButton.onClick.AddListener(OnIncreases);
            decreaseButton.onClick.AddListener(OnDecrease);
            donateButton.onClick.AddListener(OnDonate);
        }

        private void OnOnDonatePoints(object sender, DonatePointsMessage e)
        {
            OwnPoints = PlayerData.PlayerData.Instance.Points;
        }

        private void OnDonate()
        {
            if (_pointsToDonate <= 0) return;

            PlayerData.PlayerData.Instance.Points = OwnPoints;

            NetworkManager.Instance.Publish(PlayerData.PlayerData.Instance.LobbyID.ToString(),
                new DonatePointsMessage(DateTime.Now.ToString("o"), PlayerData.PlayerData.Instance.LobbyID,
                    PlayerData.PlayerData.Instance.PlayerId, _otherPlayer.PlayerId, _pointsToDonate));

            if (OwnPoints == 0)
            {
                descriptionText.text = "Please click on another player if you want to donate your point?";
            }
            else if (OwnPoints >= 1)
            {
                descriptionText.text = "Please click on another player if you want to donate your point?";
            }
            else
            {
                descriptionText.text = "Please click on another player if you want to donate one of your points?";
            }

            otherNameTMP.text = "";
            OwnPoints = PlayerData.PlayerData.Instance.Points;
            OtherName = "";
            OtherPoints = 0;
            _pointsToDonate = 0;
            increaseButton.interactable = false;
            decreaseButton.interactable = false;
        }

        private void OnDecrease()
        {
            if (_pointsToDonate - 1 < 0) return;
            OwnPoints += 1;
            OtherPoints -= 1;
            _pointsToDonate -= 1;

            if (_pointsToDonate == 0) donateButton.interactable = false;
        }

        private void OnIncreases()
        {
            if (OwnPoints - 1 < 0) return;
            OwnPoints -= 1;
            OtherPoints += 1;
            _pointsToDonate += 1;

            donateButton.interactable = true;
        }

        private void OnOnHeartBeat(object sender, HeartBeatMessage e)
        {
            DateTime parsedDate = DateTime.Parse(e.DateTimeStamp);

            if (!_players.ContainsKey(e.PlayerID))
            {
                var player = Instantiate(playerListPrefab, playerScrollView);
                player.gameObject.SetActive(true);
                var plistprefab = player.GetComponent<PlayerListPrefab>();
                plistprefab.LastPing = parsedDate;
                plistprefab.ID = e.PlayerID;
                plistprefab.Name = e.PlayerName;
                plistprefab.Points = e.Points;
                
                var button = player.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnPlayerClick(plistprefab));

                _players.Add(e.PlayerID, plistprefab);
            }
            else
            {
                _players[e.PlayerID].LastPing = parsedDate;
                _players[e.PlayerID].ID = e.PlayerID;
                _players[e.PlayerID].Name = e.PlayerName;
                _players[e.PlayerID].Points = e.Points;

                if (OtherName != e.PlayerName || OtherPoints == e.Points) return;
                OtherPoints = e.Points + _pointsToDonate;
            }
        }

        private void OnPlayerClick(PlayerListPrefab player)
        {
            OtherPoints = player.Points + _pointsToDonate;
            OtherName = player.Name;
            descriptionText.text = OwnPoints ! >= 1
                ? $"Do you want to donate your point to {OtherName}?"
                : $"Do you want to donate some of your points to {OtherName}?";

            _otherPlayer.PlayerId = player.ID;
            
            increaseButton.interactable = true;
            decreaseButton.interactable = true;
        }

        public void SkipDonation()
        {
            SceneManager.LoadScene(Utilities.GameManager.LOADING);
        }

        /*private void OnEnable()
        {
            new Nats.NatsClient();
            Nats.NatsClient.C.Connect();
            Nats.NatsClient.C.SubscribeToSubject("0");

            Nats.NatsClient.C.OnHeartBeat += OnOnHeartBeat;
            otherNameTMP.text = "";
            otherPointsTMP.text = "0";

            players = new Dictionary<int, playerlistprefab>();

            OwnPoints = PlayerData.PlayerData.Instance.Points;

            descriptionText.text = OwnPoints ! >= 1
                ? "Do you want to donate your point?"
                : "Do you want to donate your points?";
        }*/

        private void Update()
        {
            //TODO misschien is dit in de update een beetje een overkill

            var keysToRemove = new List<int>();

            foreach (var player in _players)
            {
                if (DateTime.Now - TimeSpan.FromSeconds(5) > player.Value.LastPing)
                {
                    Destroy(player.Value.gameObject);
                    keysToRemove.Add(player.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _players.Remove(key);
            }
        }
    }
}