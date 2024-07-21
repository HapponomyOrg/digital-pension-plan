using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NATS;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class DonateScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text clientPointsDisplay;

    [SerializeField] private Dictionary<int, PlayerData> players;
    [SerializeField] private DonationDisplay prefab;
    [SerializeField] private GridLayoutGroup playerList;

    [SerializeField] private GameObject confirmationMenu;
    [SerializeField] private TMP_Text confirmationMessage;

    private int selectedPlayer;
    private int pointsToDonate;

    [SerializeField] private TMP_Text clientPointsConfirm;
    [SerializeField] private TMP_Text receiverPointsConfirm;
    [SerializeField] private TMP_Text pointsToDonateDisplay;

    private List<DonationDisplay> donationDisplays;

    private void Start()
    {
        PlayerManager.Instance.OnPointsChange += (sender, msg) =>
        {
            clientPointsDisplay.text = $"Points: {PlayerManager.Instance.Points.ToString()}";
            foreach (var display in donationDisplays)
            {
                display.DonateButton.interactable = PlayerManager.Instance.Points > 0;
            }

            GenerateDisplays();
        };

        NatsClient.C.OnDonatePoints += (sender, msg) =>
        {
            clientPointsDisplay.text = $"Points: {PlayerManager.Instance.Points.ToString()}";
            foreach (var display in donationDisplays)
            {
                display.DonateButton.interactable = PlayerManager.Instance.Points > 0;
            }

            GenerateDisplays();
        };
    }

    private void OnEnable()
    {
        donationDisplays = new List<DonationDisplay>();
        players = new Dictionary<int, PlayerData>();
        NatsClient.C.OnHeartBeat += HandleHeartbeat;
    }

    void HandleHeartbeat(object sender, HeartBeatMessage msg)
    {
        if (PlayerManager.Instance.LobbyID != msg.LobbyID)
        {
            Debug.LogWarning("Lobby id of heartbeat is not the same");
            return;
        }

        var playerData = new PlayerData();
        playerData.Name = msg.PlayerName;
        playerData.Points = msg.Points;

        if (!players.ContainsKey(msg.PlayerID))
        {
            players[msg.PlayerID] = playerData;
            GenerateDisplays();
        }
        else
        {
            players[msg.PlayerID] = playerData;
            foreach (var display in donationDisplays)
            {
                display.SetDisplay(playerData);
            }
        }
    }

    private void GenerateDisplays()
    {
        foreach (Transform child in playerList.transform)
            Destroy(child.gameObject);

        clientPointsDisplay.text = $"Points: {PlayerManager.Instance.Points.ToString()}";

        foreach (var player in players)
        {
            var display = Instantiate(prefab, playerList.transform);
            display.SetDisplay(player.Value);
            display.SetBackground(player.Key % 2 == 0);
            if (PlayerManager.Instance.Points <= 0)
                display.DonateButton.interactable = false;
            else
                display.DonateButton.onClick.AddListener(() => OpenConfirmation(player.Key));


            var newHeight = playerList.cellSize.y * players.Count;
            playerList.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }
    }

    public void ConfirmDonation()
    {
        var playerData = players[selectedPlayer];
        playerData.Points += pointsToDonate;
        players[selectedPlayer] = playerData;
        PlayerManager.Instance.Points -= pointsToDonate;
        GenerateDisplays();
        confirmationMenu.SetActive(false);

        var donateMessage = new DonatePointsMessage(DateTime.Now.ToString("o"), PlayerManager.Instance.LobbyID,
            PlayerManager.Instance.PlayerId, selectedPlayer, pointsToDonate);
        NatsClient.C.Publish(PlayerManager.Instance.LobbyID.ToString(), donateMessage);
    }

    private void OpenConfirmation(int playerId)
    {
        confirmationMenu.SetActive(true);
        confirmationMessage.text = $"How many points do you want to donate to {players[playerId].Name}?";
        selectedPlayer = playerId;
        pointsToDonate = 0;

        clientPointsConfirm.text = (PlayerManager.Instance.Points).ToString();
        receiverPointsConfirm.text = (players[selectedPlayer].Points).ToString();
        pointsToDonateDisplay.text = pointsToDonate.ToString();
    }

    public void ChangePointsToDonate(int amount)
    {
        pointsToDonate = Mathf.Clamp(pointsToDonate + amount, 0, PlayerManager.Instance.Points);

        clientPointsConfirm.text = (PlayerManager.Instance.Points - pointsToDonate).ToString();
        receiverPointsConfirm.text = (players[selectedPlayer].Points + pointsToDonate).ToString();

        pointsToDonateDisplay.text = pointsToDonate.ToString();
    }
}


[Serializable]
public struct PlayerData
{
    public string Name;
    public int Points;
}