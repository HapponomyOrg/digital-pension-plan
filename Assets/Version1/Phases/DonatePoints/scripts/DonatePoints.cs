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

        [SerializeField] private GameObject ToasterPrefab;
        [SerializeField] private Transform ToasterList;

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


        private void OnOnDonatePoints(object sender, DonatePointsMessage e)
        {
            var myId = PlayerData.PlayerData.Instance.PlayerId;
            var myName = PlayerData.PlayerData.Instance.PlayerName;

            Debug.LogWarning($"[{myName} (ID:{myId})] Received DonatePoints - From: {e.PlayerName} (ID:{e.PlayerID}), To: Receiver ID:{e.Receiver}, Amount: {e.Amount}");

            // Only the receiver should process this
            if (myId != e.Receiver)
            {
                Debug.LogWarning($"[{myName}] This message is not for me. My ID: {myId}, Receiver ID: {e.Receiver}");
                return;
            }

            Debug.LogWarning($"[{myName}] I AM the receiver! Adding {e.Amount} points");

            OwnPoints += e.Amount;
            PlayerData.PlayerData.Instance.Points = OwnPoints;

            var toaster = Instantiate(ToasterPrefab, ToasterList);
            toaster.GetComponent<ToasterScript>().toasterText.text =
                $"Congratulations! You received {e.Amount} point{(e.Amount != 1 ? "s" : "")} from {e.PlayerName}";
        }

        private void OnDonate()
        {
            if (_pointsToDonate <= 0) return;

            PlayerData.PlayerData.Instance.Points = OwnPoints;

            NetworkManager.Instance.Publish(PlayerData.PlayerData.Instance.LobbyID.ToString(),
                new DonatePointsMessage(DateTime.Now.ToString("o"), PlayerData.PlayerData.Instance.LobbyID,
                    PlayerData.PlayerData.Instance.PlayerId, PlayerData.PlayerData.Instance.PlayerName, _otherPlayer.PlayerId, _pointsToDonate));

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
            Debug.Log($"Received heartbeat from PlayerID: {e.PlayerID}, Name: {e.PlayerName}, Points: {e.Points}");

            DateTime parsedDate = DateTime.Parse(e.DateTimeStamp);

            if (!_players.ContainsKey(e.PlayerID))
            {
                Debug.Log($"Adding new player: {e.PlayerName} (ID: {e.PlayerID})");

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
                Debug.Log($"Updating existing player: {e.PlayerName} (ID: {e.PlayerID})");

                _players[e.PlayerID].LastPing = parsedDate;
                _players[e.PlayerID].ID = e.PlayerID;
                _players[e.PlayerID].Name = e.PlayerName;
                _players[e.PlayerID].Points = e.Points;

                if (OtherName == e.PlayerName && OtherPoints != e.Points)
                {
                    OtherPoints = e.Points + _pointsToDonate;
                }
            }
        }

        private void OnPlayerClick(PlayerListPrefab player)
        {
            OtherPoints = player.Points + _pointsToDonate;
            OtherName = player.Name;
            descriptionText.text = OwnPoints! >= 1
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
    }
}
