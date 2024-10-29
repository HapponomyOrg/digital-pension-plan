using System;
using System.Collections.Generic;
using System.Linq;
using NATS;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField] public Dictionary<int, PlayerDataEnd> players;
    [SerializeField] private EndScreenPoint prefab;
    [SerializeField] private GridLayoutGroup playerList;
    [SerializeField] private GameObject textColumn;
    [SerializeField] private GameObject columnBar;

    private List<EndScreenPoint> endScreenPoints;

    private void OnEnable()
    {
        PlayerManager.Instance.allPoints.Add(PlayerManager.Instance.Points);
        players = new Dictionary<int, PlayerDataEnd>();
        endScreenPoints = new List<EndScreenPoint>();
        if (NatsClient.C == null)
        {
            NatsHost.C.OnHeartBeat += HandleHeartbeat;
            //GenerateColumnNames(NatsHost.C.numGames);
        }
        else
        {
            NatsClient.C.OnHeartBeat += HandleHeartbeat;
           // GenerateColumnNames(GameManager.Instance.numGames);
            var playerData = new PlayerDataEnd();
            playerData.Name = PlayerManager.Instance.PlayerName;
            playerData.Points = PlayerManager.Instance.allPoints.ToArray();
            players[PlayerManager.Instance.PlayerId] = playerData;
            GenerateDisplays();
        }
    }

    // TODO REWORK THIS FUNCTION
    /*private void GenerateColumnNames(int _numGames)
    {
        var _columnName = Instantiate(textColumn, columnBar.transform);
        _columnName.GetComponent<TextMeshProUGUI>().text = $"Game {_numGames}";
    }*/

    void HandleHeartbeat(object sender, HeartBeatMessage msg)
    {
        if (PlayerManager.Instance.LobbyID != msg.LobbyID && NatsHost.C == null)
        {
            Debug.LogWarning("Lobby id of heartbeat is not the same");
            return;
        }

        var playerData = new PlayerDataEnd();
        playerData.Name = msg.PlayerName;
        playerData.Points = msg.AllPoints;

        if (!players.ContainsKey(msg.PlayerID))
        {
            players.Add(msg.PlayerID,playerData);
            GenerateDisplays();
        }
        else
        {
            players[msg.PlayerID] = playerData;

            foreach (var display in endScreenPoints)
            {
                display.SetDisplay(playerData);
            }
        }
    }

    private void GenerateDisplays()
    {
        foreach (Transform child in playerList.transform)
            Destroy(child.gameObject);

        var playersSorted = players.OrderByDescending(pair => pair.Value.LastPoint).ToArray();

        try
        {
            foreach (var player in playersSorted)
            {
                var display = Instantiate(prefab, playerList.transform);
                display.SetDisplay(player.Value);
                display.SetBackground(player.Key % 2 == 0);


                var newHeight = playerList.cellSize.y * players.Count;
                playerList.GetComponent<RectTransform>()
                    .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error during sorting: " + ex.Message);
        }
    }

    public void Update()
    {
        if (NatsHost.C != null) NatsHost.C.HandleMessages();
        if (NatsClient.C != null) NatsClient.C.HandleMessages();
    }
}

public struct PlayerDataEnd
{
    public string Name;
    public int[] Points;
    public int LastPoint => Points.Length > 0 ? Points[^1] : 0;
}