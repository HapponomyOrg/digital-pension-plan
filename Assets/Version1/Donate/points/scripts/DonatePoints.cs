using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Version1.Host.Scripts;
using Version1.Nats.Messages.Client;

namespace Version1.Donate.points.scripts
{
    public class DonatePoints : MonoBehaviour
    {
        [SerializeField] private TMP_Text ownPointsTMP;
        [SerializeField] private TMP_Text otherPointsTMP;
        [SerializeField] private TMP_Text otherNameTMP;
        [SerializeField] private TMP_Text otherName2TMP;

        [SerializeField] private Button increaseButton;
        [SerializeField] private Button decreaseButton;
        
        [SerializeField] private Button donateButton;

        private int _pointsToDonate;

        private PlayerData.PlayerData _otherPlayer;
        
        private Dictionary<int, playerlistprefab> players;
        [SerializeField] private Transform playerListPrefab;
        [SerializeField] private GameObject playerScrolView;
        
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

        private string OtherName        {
            get => _otherName;
            set
            {
                _otherName = value;
                otherNameTMP.text = _otherName;
                otherName2TMP.text = _otherName;
            }
        }

        private void Start()
        {
            
            
            
            
            // TODO place this heres
            //Nats.NatsClient.C.OnHeartBeat += OnOnHeartBeat;
            
            increaseButton.onClick.RemoveAllListeners();
            decreaseButton.onClick.RemoveAllListeners();
            donateButton.onClick.RemoveAllListeners();
            
            increaseButton.onClick.AddListener(OnIncreases);
            decreaseButton.onClick.AddListener(OnDecrease);
            donateButton.onClick.AddListener(OnDonate);
        }

        private void OnDonate()
        {
            if (_pointsToDonate == 0) return;

            PlayerData.PlayerData.Instance.Points = OwnPoints;
            
            Nats.NatsClient.C.Publish(PlayerData.PlayerData.Instance.LobbyID.ToString(), 
                new DonatePointsMessage( DateTime.Now.ToString("o"),PlayerData.PlayerData.Instance.LobbyID,PlayerData.PlayerData.Instance.PlayerId,_otherPlayer.PlayerId,_pointsToDonate));

        }

        private void OnDecrease()
        {
            if (_pointsToDonate - 1 == 0) return;
            OwnPoints -= 1;
            OtherPoints += 1;
            _pointsToDonate -= 1;
        }

        private void OnIncreases()
        {
            if (OwnPoints - 1 == 0) return;
            OwnPoints -= 1;
            OtherPoints += 1;
            _pointsToDonate += 1;
        }

        private void OnOnHeartBeat(object sender, HeartBeatMessage e)
        {
            // TODO logic of the listing of players from the host screen
            // when clicked on record set otherpoints to points of that player
            // make a donate button with a listener
            // make a continue button to go to next screen
            // set the name of the other player somewhere on the screen
            
            print("CALLED");
            
            DateTime parsedDate = DateTime.Parse(e.DateTimeStamp);
            
            if (!players.ContainsKey(e.PlayerID))
            {
                var player = Instantiate(playerListPrefab, playerScrolView.transform);
                player.gameObject.SetActive(true);
                var plistprefab = player.GetComponent<playerlistprefab>();
                plistprefab.LastPing = parsedDate;
                plistprefab.ID = e.PlayerID;
                plistprefab.Name = e.PlayerName;
                plistprefab.Points = e.Points;

                var button = player.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnPlayerClick(plistprefab));
                
                players.Add(e.PlayerID, plistprefab);
            }
            else
            {
                players[e.PlayerID].LastPing = parsedDate;
                players[e.PlayerID].ID = e.PlayerID;
                players[e.PlayerID].Name = e.PlayerName;
                players[e.PlayerID].Points = e.Points;

                if (OtherName != e.PlayerName || OtherPoints == e.Points) return;
                OtherPoints = e.Points;

            }
        }

        private void OnPlayerClick(playerlistprefab player)
        {
            OtherPoints = player.Points;
            OtherName = player.Name;
            otherNameTMP.text = OwnPoints !>= 1 ? $"Do you want to donate your point to {OtherName}?" : $"Do you want to donate some of your points to {OtherName}?";
        }

        private void OnEnable()
        {
            new Nats.NatsClient();
            Nats.NatsClient.C.Connect();
            Nats.NatsClient.C.SubscribeToSubject("0");
            
            Nats.NatsClient.C.OnHeartBeat += OnOnHeartBeat;
            
            
            players = new Dictionary<int, playerlistprefab>();
            
            OwnPoints = PlayerData.PlayerData.Instance.Points;

            otherNameTMP.text = OwnPoints !>= 1 ? "Do you want to donate your point?" : "Do you want to donate your points?";
        }
        
        private void Update()
        {
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
            
            //TODO remove this
            Nats.NatsClient.C.HandleMessages();
        }
    }
}
