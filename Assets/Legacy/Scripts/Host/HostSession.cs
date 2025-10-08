using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NATS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[Obsolete]
public class HostSession : MonoBehaviour
{
    public int RoundNumber = 1;

    [SerializeField] private Button StartRoundButton;
    [SerializeField] private Button StopRoundButton;

    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private PlayerRecord Record;
    [SerializeField] private Transform RecordList;
    [SerializeField] private TMP_Text RoundIDText;
    [SerializeField] private TMP_Text RoundTimerText;
    [SerializeField] private Image TimerImage;
    [SerializeField] private TMP_Text LobbyID;
    private Dictionary<int, PlayerRecord> PlayerRecords = new Dictionary<int, PlayerRecord>();
    private List<int> keysToRemove = new List<int>();

    [SerializeField] public int AmountOfRounds = 4;
    [SerializeField] public int TotalRoundTime = 240;

    [SerializeField] private Deck.Deck deck;

    private float timer;
    private bool isPaused = false;
    private bool isStopped = false;

    private Dictionary<string, Listing> activeListings = new Dictionary<string, Listing>();

    private void Start()
    {
        NatsHost.C.OnHeartBeat += HandleHeartbeat;
        NatsHost.C.OnCancelListing += HandleCancelListing;
        NatsHost.C.OnListCards += HandleListCards;
        NatsHost.C.OnCardHandIn += HandleCardHandIn;
    }

    private void OnEnable()
    {
        RoundNumber = 1;
        RoundIDText.text = RoundNumber.ToString();
        activeListings = new Dictionary<string, Listing>();
        StartRoundButton.interactable = true;
        StopRoundButton.interactable = false;
        isPaused = true;
        isStopped = false;
        activeListings = new Dictionary<string, Listing>();
        LobbyID.text =
            $"{NatsHost.C.LobbyID.ToString().Substring(0, 3)} {NatsHost.C.LobbyID.ToString().Substring(3, 3)} {NatsHost.C.LobbyID.ToString().Substring(6, 3)}";
    }

    void HandleCardHandIn(object sender, CardHandInMessage msg)
    {
        List<Card> playerCards = deck.TakeCards(4);

        int[] handCards = new int[playerCards.Count];

        for (int j = 0; j < playerCards.Count; j++)
        {
            handCards[j] = playerCards[j].ID;
        }

        ConfirmHandInMessage confirmHandInMessage = new ConfirmHandInMessage(DateTime.Now.ToString("o"),
            NatsHost.C.LobbyID, -1, msg.PlayerID, handCards);
        NatsHost.C.Publish(NatsHost.C.LobbyID.ToString(), confirmHandInMessage);
    }

    void HandleListCards(object sender, ListCardsmessage msg)
    {
        var listing = new Listing(msg.PlayerName, msg.AuctionID, msg.Amount, msg.Cards);
        activeListings.Add(msg.AuctionID, listing);
    }

    void HandleHeartbeat(object sender, HeartBeatMessage msg)
    {
        // TODO() check this what is stored and what not.
        if (!PlayerRecords.ContainsKey(msg.PlayerID))
        {
            CreateRecord(msg);
        }
        else
        {
            // TODO() here i can add an something to show if online or offline
            PlayerRecords[msg.PlayerID].LastContact = DateTime.Parse(msg.DateTimeStamp);
            PlayerRecords[msg.PlayerID].Balance.text = msg.Balance.ToString();
            PlayerRecords[msg.PlayerID].Points.text = msg.Points.ToString();
        }
    }

    void HandleCancelListing(object sender, CancelListingMessage msg)
    {
        if (!activeListings.ContainsKey(msg.AuctionID))
        {
            print($"Can not find listing with AuctionID : {msg.AuctionID}");
            return;
        }

        ;
        activeListings.Remove(msg.AuctionID);

        var confirmMessage =
            new ConfirmCancelListingMessage(DateTime.Now.ToString("o"), NatsHost.C.LobbyID, -1, msg.AuctionID);
        NatsHost.C.Publish(NatsHost.C.LobbyID.ToString(), confirmMessage);
    }

    private void CreateRecord(HeartBeatMessage msg)
    {
        var record = Instantiate(Record, RecordList, true);
        record.gameObject.SetActive(true);
        record.PlayerID.text = msg.PlayerID.ToString();
        record.PlayerName.text = msg.PlayerName;
        record.Balance.text = msg.Balance.ToString();
        record.Cards.text = msg.Cards.ToString();
        record.Points.text = msg.Points.ToString();
        record.LastContact = DateTime.Parse(msg.DateTimeStamp);
        PlayerRecords.Add(msg.PlayerID, record);

        // Sort the PlayerRecords by PlayerID
        SortPlayerRecords();
    }

    private void SortPlayerRecords()
    {
        var sortedRecords = PlayerRecords.OrderBy(kv => kv.Key).ToList();

        foreach (var kvp in sortedRecords)
        {
            kvp.Value.transform.SetSiblingIndex(sortedRecords.IndexOf(kvp));
        }
    }

    public void StartRound()
    {
        StartRoundButton.interactable = false;
        StopRoundButton.interactable = true;
        StartRoundMessage startRoundMessage =
            new StartRoundMessage(DateTime.Now.ToString("o"), NatsHost.C.LobbyID, -1, RoundNumber,
                (ushort)TotalRoundTime);
        NatsHost.C.Publish(NatsHost.C.LobbyID.ToString(), startRoundMessage);
        StartTimer();
    }

    void StartTimer()
    {
        timer = TotalRoundTime;
        isPaused = false;
        if (!isStopped) StartCoroutine(Timer());
    }

    public void StopRound()
    {
        PauseTimer();
        if (RoundNumber == AmountOfRounds)
        {
            EndOfRounds();
        }
        else
        {
            StopRoundMessage stopRoundMessage = new StopRoundMessage(DateTime.Now.ToString("o"), NatsHost.C.LobbyID,
                -1, RoundNumber);
            NatsHost.C.Publish(NatsHost.C.LobbyID.ToString(), stopRoundMessage);
            RoundNumber++;
            RoundIDText.text = RoundNumber.ToString();
        }

        StartRoundButton.interactable = true;
        StopRoundButton.interactable = false;
    }


    /*// TODO() make a resume and stop round message and implementation
    public void EndGame()
    {
        EndGameMessage endGameMessage =
            new EndGameMessage(DateTime.Now.ToString("o"), NatsHost.C.LobbyID, -1);
        NatsHost.C.Publish(NatsHost.C.LobbyID.ToString(), endGameMessage);
    }*/


    public void EndOfRounds()
    {
        EndOfRoundsMessage endOfRoundsMessage = new EndOfRoundsMessage(DateTime.Now.ToString("o"), NatsHost.C.LobbyID, -1);
        NatsHost.C.Publish(NatsHost.C.LobbyID.ToString(), endOfRoundsMessage);
        endGameCanvas.SetActive(true);
        gameObject.SetActive(false);
    }

    private void PauseTimer()
    {
        isPaused = true;
        isStopped = true;
    }

    IEnumerator Timer()
    {
        while (timer > 0f)
        {
            if (!isPaused)
            {
                int minutes = Mathf.FloorToInt(timer / 60f);
                int seconds = Mathf.FloorToInt(timer % 60f);

                if (RoundTimerText)
                {
                    if (timer < 11 && Mathf.FloorToInt(timer % 2) == 0)
                    {
                        RoundTimerText.color = Color.red;
                    }
                    else
                    {
                        RoundTimerText.color = Color.white;
                    }

                    RoundTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                }

                if (TimerImage)
                {
                    TimerImage.fillAmount = Mathf.InverseLerp(0, TotalRoundTime, timer);
                }

                timer -= Time.deltaTime;
            }

            yield return null;
        }

        if (RoundNumber == AmountOfRounds)
        {
            EndOfRounds();
        }
        else
        {
            StopRoundMessage stopRoundMessage = new StopRoundMessage(DateTime.Now.ToString("o"), NatsHost.C.LobbyID,
                -1, RoundNumber);
            NatsHost.C.Publish(NatsHost.C.LobbyID.ToString(), stopRoundMessage);
            RoundNumber++;
            RoundIDText.text = RoundNumber.ToString();
        }

        StopRoundButton.interactable = false;
        StartRoundButton.interactable = true;
    }

    public void Update()
    {
        NatsHost.C.HandleMessages();
        foreach (var key in PlayerRecords.Keys)
        {
            var record = PlayerRecords[key];
            TimeSpan timeSinceLastContact = DateTime.Now - record.LastContact;
            if (timeSinceLastContact.TotalSeconds > 20)
            {
                Debug.Log("Removed " + record.PlayerName + " " + record.LastContact);
                keysToRemove.Add(key);
                Destroy(record.gameObject);
            }
        }

        //TODO() THIS IS A HUGE AMOUNT OF WORK but if a certain ID is removed we want to rearrange the amount of players.
        foreach (var key in keysToRemove)
        {
            PlayerRecords.Remove(key);
        }
    }
}
