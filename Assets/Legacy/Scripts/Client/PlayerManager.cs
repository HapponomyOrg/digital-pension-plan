using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Legacy;
using NATS.Client;
using TMPro;
using UI;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
[Obsolete]
public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;

    public static PlayerManager Instance
    {
        get
        {
            if (!instance) instance = new PlayerManager();
            return instance;
        }
        private set { instance = value; }
    }

    private int balance = 0;

    public int Balance
    {
        get => balance;
        set
        {
            balance = value;
            MarketManager.Instance.UpdateBalance();
        }
    }
    
    private int points = 0;

    public int Points
    {
        get => points;
        set
        {
            points = value;
            OnPointsChange?.Invoke(null, null);
        }
    }

    [field: SerializeField] public CardLibrary CardLibrary { get; private set; }

    public string PlayerName = "";
    public int Age = 0;
    public string Gender = "";
    public int PlayerId = 456;
    public int LobbyID = 0;
    public int Dept = 11000;
    public int IntrestRemainder = 0;
    public List<int> allPoints;

    public bool RoundIsActive = false;


    [field: SerializeField] public List<int> Cards;
    [field: SerializeField] public List<UICard> UiCards { get; private set; }
    [SerializeField] private Transform playerHand;
    [SerializeField] private Transform defaultUICardParent;
    [SerializeField] private UICard uiCardPrefab;
    [SerializeField] private CardHandInOverlay cardHandInOverlay;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private List<TMP_Text> LobbyIDText;
    public event EventHandler CardReleased;

    public event EventHandler OnBalanceChange;
    public event EventHandler OnPointsChange;
    

    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;
    }

    public void ResetAtributes()
    {
        PlayerName = "";
        Age = 0;
        Gender = "";
        PlayerId = 1;
        LobbyID = 0;
        balance = 0;
        points = 0;
        Dept = 0;
        IntrestRemainder = 0;
    }

    private void Start()
    {
        NatsClient.Instance.OnStartGame += (sender, msg) =>
        {
            if (msg.OtherPlayerID != PlayerId)
                return;

            Balance = msg.Balance;
            pointsText.text = Points.ToString();

            Cards = msg.Cards.ToList();
            CreateCards(Cards);
        };

        NatsClient.Instance.OnConfirmJoin += (sender, msg) =>
        {
            if (PlayerName != msg.PlayerName) return;

            // TODO() what if player wants to join mid game
            print("ConfirmMessage");
            PlayerId = msg.LobbyPlayerID;
            LobbyID = msg.LobbyID;

            foreach (var text in LobbyIDText)
            {
                text.text =
                    $"LobbyID : {msg.LobbyID.ToString().Substring(0, 3)} {msg.LobbyID.ToString().Substring(3, 3)} {msg.LobbyID.ToString().Substring(6, 3)}";
            }

            NatsClient.Instance.StartHeartbeat();
        };

        NatsClient.Instance.OnDonatePoints += (sender, msg) =>
        {
            if (msg.Receiver != PlayerManager.Instance.PlayerId)
                return;
            Points += msg.Amount;
            OnPointsChange?.Invoke(null, null);
        };

        NatsClient.Instance.OnStartRound += (sender,msg) =>
        {
            CheckForSet();
        };

        NatsClient.Instance.OnConfirmHandIn += (sender, msg) =>
        {
            if (msg.Receiver == PlayerManager.Instance.PlayerId)
            {
                AddCards(msg.Cards);   
            }
        };

        NatsClient.Instance.OnEndOfRounds += (sender, msg) =>
        {
            if (GameManager.Instance.GameMode == 1)
            {
                int pointRemoval = Dept / 1000;
                AddPoints(-pointRemoval);
            }
        };

        OnPointsChange += (sender, Msg) => { pointsText.text = Points.ToString(); };
    }

    public void SetBalance(int newBalance)
    {
        Balance = newBalance;
        OnBalanceChange?.Invoke(null, null);
    }

    public void RemoveBalance(int toRemove)
    {
        Balance -= toRemove;
        OnBalanceChange?.Invoke(null, null);
    }

    public void AddPoints(int _points)
    {
        points += _points;
        OnPointsChange?.Invoke(null, null);
    }

    public void OnDestroy()
    {
        NatsClient.Instance.StopHeartbeat();
    }

    public void OnDisable()
    {
        NatsClient.Instance.StopHeartbeat();
    }

    private void CreateCards(List<int> cards)
    {
        foreach (var card in cards)
        {
            var data = CardLibrary.CardData((byte)card);
            var uiCard = Instantiate(uiCardPrefab, playerHand);
            uiCard.Card = data;
            uiCard.DefaultParent = defaultUICardParent;
            UiCards.Add(uiCard);
        }

        SortCards();
    }

    public void RemoveCards(int[] cards)
    {
        foreach (var card in cards)
        {
            Cards.Remove(card);
        }
    }

    public void ReleaseCard(UICard card)
    {
        CardReleased?.Invoke(this, EventArgs.Empty);
        SortCards();
    }

    public void AddCards(int[] cards)
    {
        foreach (var c in cards)
        {
            Cards.Add(c);
        }

        CreateCards(cards.ToList());
    }

    public void CheckForSet()
    {
        int? result = Cards
            .GroupBy(n => n)
            .Where(g => g.Count() == 4)
            .Select(g => (int?)g.Key)
            .FirstOrDefault() ?? null;
        
        if (result == null)
            return;
        cardHandInOverlay.Open(CardLibrary.CardData((byte)result));
    }

    public void SortCards()
    {
        var l = UiCards.OrderBy(card => card.Card.ID).ToList();

        for (int i = 0; i < l.Count; i++)
        {
            l[i].transform.SetSiblingIndex(i);
        }
    }
}