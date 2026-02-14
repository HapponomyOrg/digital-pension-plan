using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Version1.Nats.Messages;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;
using Version1.Phases;
using Version1.Websocket;

namespace Version1.Utilities
{
    public class NetworkManager :  MonoBehaviour , INetworkManager
    {
        public static NetworkManager Instance { get; private set; }

        public int heartbeatInterval = 2;
        private Coroutine heartbeatCoroutine;

        public event EventHandler<RejectedMessage> OnRejected;
        public event EventHandler<string> OnError;

        private bool initialized = false;

        public NetworkManager()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public WebsocketClient WebSocketClient;

        public async void Publish(string sessionID, BaseMessage baseMessage)
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

            Debug.Log("NetworkManager Awake started");

#if UNITY_WEBGL && !UNITY_EDITOR
    // Dynamically get the host from the current URL
    string url = Application.absoluteURL;
    System.Uri uri = new System.Uri(url);

    // Build WebSocket URL from current page host
    string wsUrl = $"ws://{uri.Host}:8080/ws";

    Debug.Log($"Connecting to WebSocket at: {wsUrl}");
    WebSocketClient = new WebsocketClient(wsUrl);
#else
            // For testing in Unity Editor
            WebSocketClient = new WebsocketClient("ws://localhost:8080/ws");
#endif


            Debug.Log("WebSocketClient created");

            // IMPORTANT: Subscribe to ALL events BEFORE connecting
            // Subscribe to NatsClient events
            WebSocketClient.OnRejected += NatsClientOnOnRejected;
            //WebSocketClient.OnConfirmBuy += NatsClientOnOnConfirmBuy;
            WebSocketClient.OnConfirmJoin += NatsClientOnOnConfirmJoin;
            WebSocketClient.OnDonatePoints += NatsClientOnOnDonatePoints;
            WebSocketClient.OnEndGame += NatsClientOnOnEndGame;

            // Listings
            WebSocketClient.OnCreateListing += CreateListing;
            WebSocketClient.OnCancelListing += CancelListing;
            WebSocketClient.OnBuyListing += BuyListing;

            // Bids
            WebSocketClient.OnCreateBid += CreateBid;
            WebSocketClient.OnCancelBid += CancelBid;
            WebSocketClient.OnAcceptBid += AcceptBid;
            WebSocketClient.OnAcceptCounterBid += AcceptCounterBid;
            WebSocketClient.OnCounterBid += CounterBid;
            WebSocketClient.OnRejectBid += RejectBid;
            WebSocketClient.OnRejectCounterBid += RejectCounterBid;


            WebSocketClient.OnStartGame += NatsClientOnOnStartGame;
            WebSocketClient.OnStartRound += NatsClientOnOnStartRound;
            WebSocketClient.OnStopRound += NatsClientOnOnStopRound;
            //WebSocketClient.OnConfirmCancelListing += NatsClientOnOnConfirmCancelListing;
            WebSocketClient.OnConfirmHandIn += NatsClientOnOnConfirmHandIn;
            WebSocketClient.OnEndOfRounds += NatsClientOnOnEndOfRounds;
            WebSocketClient.OnOpen += NatsClientOnOpen;

            Debug.Log("All events subscribed");

            try
            {
                Debug.Log("Attempting to connect to WebSocket...");
                await WebSocketClient.Connect();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to connect: {ex.Message}\nStack: {ex.StackTrace}");
                OnError?.Invoke(this, ex.Message);
            }
        }

        private void NatsClientOnOpen(object sender, bool e)
        {
            Debug.Log($"Heartbeat coroutine is null: {heartbeatCoroutine == null}");

            if (heartbeatCoroutine != null) return;

            Debug.Log("About to start heartbeat coroutine...");
            heartbeatCoroutine = StartCoroutine(HeartbeatRoutine());
            Debug.Log($"Heartbeat started, coroutine reference: {heartbeatCoroutine != null}");
        }

        private void WebSocketClientOnOnError(object sender, string e)
        {
            Debug.LogError($"WebSocket Error: {e}");
            OnError?.Invoke(sender, e);
        }

        private void OnDestroy()
        {
            if (heartbeatCoroutine != null)
            {
                StopCoroutine(heartbeatCoroutine);
                heartbeatCoroutine = null;
                Debug.Log("Heartbeat stopped");
            }

            WebSocketClient?.Dispose();
        }

        private IEnumerator HeartbeatRoutine()
        {
            while (true)
            {
                if (PlayerData.PlayerData.Instance == null)
                {
                    Debug.LogWarning("PlayerData.Instance is null, skipping heartbeat");
                    yield return new WaitForSeconds(heartbeatInterval);
                    continue;
                }

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

/*        private void NatsClientOnOnConfirmCancelListing(object sender, ConfirmCancelListingMessage e)
        {
            // TODO MARKET FUNCTION
        }
*/


        private void NatsClientOnOnStopRound(object sender, StopRoundMessage e)
        {
            var phaseController = GameManager.Instance.PhaseManager.CurrentPhaseController;
            if (phaseController != null)
                phaseController.StopPhase();
        }

        private void NatsClientOnOnStartRound(object sender, StartRoundMessage e)
        {
            GameManager.Instance.PhaseManager.LoadPhase(e.RoundNumber, e.RoundName);
        }

        private void NatsClientOnOnStartGame(object sender, StartGameMessage e)
        {
            if (e.OtherPlayerID != PlayerData.PlayerData.Instance.PlayerId) return;

            GameManager.Instance.PhaseManager.StartPhases(e);
            PlayerData.PlayerData.Instance.StartGame(e);
        }

        private void NatsClientOnOnRejected(object sender, RejectedMessage e)
        {
            OnRejected?.Invoke(sender, e);
        }

        private void NatsClientOnOnEndGame(object sender, EndGameMessage e)
        {
            PlayerData.PlayerData.Instance.ResetData();
            SceneManager.LoadScene(PhaseLibrary.LoadingPhase.Scene);
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

        /*private void NatsClientOnOnConfirmBuy(object sender, ConfirmBuyMessage e)
        {
            // TODO MARKET FUNCTION
        }*/


        #region Listing

        private void CreateListing(object sender, ListCardsmessage e)
            => GameManager.Instance.MarketServices.CreateListingService.CreateListingHandler(e);

        private void CancelListing(object sender, CancelListingMessage e)
            => GameManager.Instance.MarketServices.CancelListingService.CancelListingHandler(e);

        private void BuyListing(object sender, BuyCardsRequestMessage e)
            => GameManager.Instance.MarketServices.BuyListingService.BuyListingHandler(e);

        #endregion


        #region Bid

        private void CreateBid(object sender, CreateBidMessage e)
            => GameManager.Instance.MarketServices.CreateBidService.CreateBidHandler(e);

        private void CancelBid(object sender, CancelBidMessage e)
            => GameManager.Instance.MarketServices.CancelBidService.CancelBidHandler(e);

        private void AcceptBid(object sender, AcceptBidMessage e)
            => GameManager.Instance.MarketServices.AcceptBidService.AcceptBidHandler(e);

        private void AcceptCounterBid(object sender, AcceptCounterBiddingMessage e)
        {
            // TODO MARKET FUNCTION
        }


        private void CounterBid(object sender, CounterBidMessage e)
        {
            // TODO MARKET FUNCTION
        }

        private void RejectBid(object sender, RejectBidMessage e)
            => GameManager.Instance.MarketServices.RejectBidService.RejectBidHandler(e);

        private void RejectCounterBid(object sender, RejectCounterBidMessage e)
        {
            // TODO MARKET FUNCTION
        }

        #endregion
    }
}
