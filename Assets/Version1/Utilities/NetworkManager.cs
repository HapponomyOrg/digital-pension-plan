using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Version1.Nats;
using Version1.Nats.Messages;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;
using Version1.Websocket;

namespace Version1.Utilities
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        public int heartbeatInterval = 2;

        public event EventHandler<RejectedMessage> OnRejected;
        public event EventHandler<string> OnError;

        private bool initialized = false;

        public NetworkManager()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public WebsocketClient WebSocketClient;


        public async void Publish(string sessionID, BaseMessage baseMessage, bool flushImmediately = true)
        {
            try
            {
                await WebSocketClient.Publish(sessionID, baseMessage);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Nats: Error during publishing: {ex.Message}");
                OnError?.Invoke(this, "");
            }
        }

        public async void Subscribe(string sessionID)
        {
            if (initialized) 
                return;

            try
            {
                await WebSocketClient.Subscribe(sessionID);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Nats: Error during subscribing: {ex.Message}");
                OnError?.Invoke(this, "");
            }
        }

        private async void Awake()
        {
            DontDestroyOnLoad(gameObject);

            #if UNITY_WEBGL && !UNITY_EDITOR
                string url = Application.absoluteURL;
                System.Uri uri = new System.Uri(url);
                string wsUrl = $"ws://{uri.Host}:8080/ws";
                WebSocketClient = new WebsocketClient(wsUrl);
            #else
                // For testing in Unity Editor
                WebSocketClient = new WebsocketClient("ws://localhost:8080/ws");
            #endif

            try
            {
                await WebSocketClient.Connect();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to connect: {ex.Message}");
                OnError?.Invoke(this, "");
            }

            // Subscribe to NatsClient events
            WebSocketClient.OnRejected += NatsClientOnOnRejected;
            WebSocketClient.OnAcceptBidding += NatsClientOnOnAcceptBidding;
            WebSocketClient.OnBuyCards += NatsClientOnOnBuyCards;
            WebSocketClient.OnCancelBidding += NatsClientOnOnCancelBidding;
            WebSocketClient.OnCancelListing += NatsClientOnOnCancelListing;
            WebSocketClient.OnConfirmBuy += NatsClientOnOnConfirmBuy;
            WebSocketClient.OnConfirmJoin += NatsClientOnOnConfirmJoin;
            WebSocketClient.OnDonatePoints += NatsClientOnOnDonatePoints;
            WebSocketClient.OnEndGame += NatsClientOnOnEndGame;
            WebSocketClient.OnListCards += NatsClientOnOnListCards;
            WebSocketClient.OnMakeBidding += NatsClientOnOnMakeBidding;
            WebSocketClient.OnRejectBidding += NatsClientOnOnRejectBidding;
            WebSocketClient.OnRespondBidding += NatsClientOnOnRespondBidding;
            WebSocketClient.OnStartGame += NatsClientOnOnStartGame;
            WebSocketClient.OnStartRound += NatsClientOnOnStartRound;
            WebSocketClient.OnStopRound += NatsClientOnOnStopRound;
            WebSocketClient.OnAcceptCounterBidding += NatsClientOnOnAcceptCounterBidding;
            WebSocketClient.OnConfirmCancelListing += NatsClientOnOnConfirmCancelListing;
            WebSocketClient.OnConfirmHandIn += NatsClientOnOnConfirmHandIn;
            WebSocketClient.OnEndOfRounds += NatsClientOnOnEndOfRounds;
        }

        private void OnDestroy()
        {
            WebSocketClient?.Dispose();
        }

        private IEnumerator HeartbeatRoutine()
        {
            while (true)
            {
                var msg = new HeartBeatMessage(
                    DateTime.UtcNow.ToString("o"),
                    PlayerData.PlayerData.Instance.LobbyID,
                    PlayerData.PlayerData.Instance.PlayerId,
                    PlayerData.PlayerData.Instance.PlayerName,
                    PlayerData.PlayerData.Instance.Balance,
                    PlayerData.PlayerData.Instance.Cards.ToArray(),
                    PlayerData.PlayerData.Instance.Points,
                    PlayerData.PlayerData.Instance.AllPoints.ToArray()
                );

                Publish(PlayerData.PlayerData.Instance.LobbyID.ToString(), msg);

                yield return new WaitForSeconds(heartbeatInterval);
            }
        }

        void Update()
        {
            WebSocketClient?.DispatchMessageQueue();
        }

        private void NatsClientOnOnEndOfRounds(object sender, EndOfRoundsMessage e)
        {
            // TODO game phase system
        }

        private void NatsClientOnOnConfirmHandIn(object sender, ConfirmHandInMessage e)
        {
            PlayerData.PlayerData.Instance.ConfirmHandIn(e);
        }

        private void NatsClientOnOnConfirmCancelListing(object sender, ConfirmCancelListingMessage e)
        {
            // TODO MARKET FUNCTION
        }

        private void NatsClientOnOnAcceptCounterBidding(object sender, AcceptCounterBiddingMessage e)
        {
            // TODO MARKET FUNCTION
        }

        private void NatsClientOnOnStopRound(object sender, StopRoundMessage e)
        {
            // TODO game phase system
        }

        private void NatsClientOnOnStartRound(object sender, StartRoundMessage e)
        {
            Utilities.GameManager.Instance.LoadPhase(e.RoundNumber, e.RoundName);
        }

        private void NatsClientOnOnStartGame(object sender, StartGameMessage e)
        {
            if (e.OtherPlayerID != PlayerData.PlayerData.Instance.PlayerId) return;

            Utilities.GameManager.Instance.StartGame();
            PlayerData.PlayerData.Instance.StartGame(e);
        }

        private void NatsClientOnOnRespondBidding(object sender, CounterBidMessage e)
        {
            // TODO MARKET FUNCTION
        }

        private void NatsClientOnOnRejectBidding(object sender, RejectBidMessage e)
        {
            // TODO MARKET FUNCTION
        }

        private void NatsClientOnOnMakeBidding(object sender, CreateBidMessage e)
        {
            GameManager.Instance.MarketServices.CreateBidService.CreateBidHandler(sender, e);
            // TODO MARKET FUNCTION 
            //throw new NotImplementedException();
        }

        private void NatsClientOnOnListCards(object sender, ListCardsmessage e)
        {
            GameManager.Instance.MarketServices.CreateListingService.CreateListingHandler(sender, e);
        }

        private void NatsClientOnOnEndGame(object sender, EndGameMessage e)
        {
            PlayerData.PlayerData.Instance.ResetData();
        }

        private void NatsClientOnOnDonatePoints(object sender, DonatePointsMessage e)
        {
            PlayerData.PlayerData.Instance.PointsDonated(e);
        }

        private void NatsClientOnOnConfirmJoin(object sender, ConfirmJoinMessage e)
        {
            Debug.Log(PlayerData.PlayerData.Instance.RequestID);

            if (e.RequestID != PlayerData.PlayerData.Instance.RequestID) return;

            Debug.Log("Transfer to loading screen");

            PlayerData.PlayerData.Instance.PlayerId = e.LobbyPlayerID;
            NetworkManager.Instance.WebSocketClient.clientID = e.LobbyPlayerID;
            // TODO check this heartbeat thing
            //StartCoroutine(HeartbeatRoutine());
            SceneManager.LoadScene("Loading");
        }

        private void NatsClientOnOnConfirmBuy(object sender, ConfirmBuyMessage e)
        {
            // TODO MARKET FUNCTION
        }

        private void NatsClientOnOnCancelListing(object sender, CancelListingMessage e)
        {
            GameManager.Instance.MarketServices.CancelListingService.CancelListingHandler(sender, e);
        }

        private void NatsClientOnOnCancelBidding(object sender, CancelBidMessage e)
        {
            // TODO MARKET FUNCTION
        }

        private void NatsClientOnOnBuyCards(object sender, BuyCardsRequestMessage e)
        {
            // TODO MARKET FUNCTION
            //throw new NotImplementedException();
            
            GameManager.Instance.MarketServices.BuyListingService.BuyListingHandler(sender, e);
        }

        private void NatsClientOnOnAcceptBidding(object sender, AcceptBidMessage e)
        {
            // TODO MARKET FUNCTION
        }

        private void NatsClientOnOnRejected(object sender, RejectedMessage e)
        {
            OnRejected?.Invoke(sender, e);
        }
    }
}
