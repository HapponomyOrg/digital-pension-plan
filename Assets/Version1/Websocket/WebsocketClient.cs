using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;
using Version1.Nats.Messages;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;

namespace Version1.Websocket
{
    public class WebSocketMessage
    {
        public string action;
        public string subject;
    }

    public class WebsocketClient
    {
        public event EventHandler<ListCardsmessage> OnListCards;
        public event EventHandler<BuyCardsRequestMessage> OnBuyCards;
        public event EventHandler<CancelListingMessage> OnCancelListing;
        public event EventHandler<DonateMoneyMessage> OnDonateMoney;
        public event EventHandler<DonatePointsMessage> OnDonatePoints;
        public event EventHandler<DeptUpdateMessage> OnDeptUpdate;
        public event EventHandler<CardHandInMessage> OnCardHandIn;
        public event EventHandler<HeartBeatMessage> OnHeartBeat;
        public event EventHandler<JoinRequestMessage> OnJoinrequest;
        public event EventHandler<CreateSessionMessage> OnCreateSession;
        public event EventHandler<StartGameMessage> OnStartGame;
        public event EventHandler<StartRoundMessage> OnStartRound;
        public event EventHandler<StopRoundMessage> OnStopRound;
        public event EventHandler<EndOfRoundsMessage> OnEndOfRounds;
        public event EventHandler<EndGameMessage> OnEndGame;
        public event EventHandler<ConfirmJoinMessage> OnConfirmJoin;
        public event EventHandler<RejectedMessage> OnRejected;
        public event EventHandler<ConfirmBuyMessage> OnConfirmBuy;
        public event EventHandler<ConfirmHandInMessage> OnConfirmHandIn;
        public event EventHandler<ConfirmCancelListingMessage> OnConfirmCancelListing;
        public event EventHandler<CreateBidMessage> OnMakeBidding;
        public event EventHandler<AcceptBidMessage> OnAcceptBidding;
        public event EventHandler<CancelBidMessage> OnCancelBidding;
        public event EventHandler<RejectBidMessage> OnRejectBidding;
        public event EventHandler<CounterBidMessage> OnRespondBidding;
        public event EventHandler<AcceptCounterBiddingMessage> OnAcceptCounterBidding;
        public event EventHandler<AbortSessionMessage> OnAbortSession;
        public event EventHandler<SkipRoundMessage> OnSkipRound;
        public event EventHandler<ContinueMessage> OnContinue;

        private WebSocket _webSocket;
        private string _uri;

        public int clientID;

        public WebsocketClient(string uri)
        {
            _uri = uri;
        }

        public async Task Connect()
        {
            Debug.Log("Connecting to websocket server");

            _webSocket = new WebSocket(_uri);

            _webSocket.OnOpen += () => { Debug.Log("WebSocket Connected!"); };

            _webSocket.OnError += (e) => { Debug.LogError($"WebSocket Error: {e}"); };

            _webSocket.OnClose += (e) => { Debug.Log($"WebSocket Closed: {e}"); };

            _webSocket.OnMessage += OnMessageReceived;

            // Connect to the server
            _ = _webSocket.Connect();
        }

        private void OnMessageReceived(byte[] bytes)
        {
            var messageJson = System.Text.Encoding.UTF8.GetString(bytes);

            Debug.LogWarning("Raw message: " + messageJson);

            try
            {
                var wsMessage = JsonUtility.FromJson<WebSocketMessage>(messageJson);

                //Debug.LogWarning($"{wsMessage.action} , {wsMessage.subject}");

                if (wsMessage.action == "message")
                {
                    int dataStart = messageJson.IndexOf("\"data\":", StringComparison.Ordinal);
                    int openBrace = messageJson.IndexOf('{', dataStart);
                    int braceCount = 0;
                    int closeBrace = -1;

                    for (int i = openBrace; i < messageJson.Length; i++)
                    {
                        if (messageJson[i] == '{') braceCount++;
                        else if (messageJson[i] == '}')
                        {
                            braceCount--;
                            if (braceCount == 0)
                            {
                                closeBrace = i;
                                break;
                            }
                        }
                    }

                    string dataJson = messageJson.Substring(openBrace, closeBrace - openBrace + 1);
                    //Debug.LogWarning("Extracted data: " + dataJson);

                    var innerData = JsonUtility.FromJson<BaseMessage>(dataJson);

                    if (innerData != null && !string.IsNullOrEmpty(innerData.Subject))
                    {
                        Debug.LogWarning($"Parsed Subject: {innerData.Subject}");
                        DispatchMessage(innerData.Subject, dataJson);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing message: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void DispatchMessage(string subject, string jsonData)
        {
            Debug.LogWarning($"DispatchMessage called with subject: {subject}");
            Debug.LogWarning($"JSON data being parsed: [{jsonData}]");
            Debug.LogWarning($"JSON length: {jsonData.Length}");

            var msg = JsonUtility.FromJson<BaseMessage>(jsonData);
            if (msg.PlayerID == PlayerData.PlayerData.Instance.PlayerId)
                return;

            try
            {
                switch (subject)
                {
                    case MessageSubject.ConfirmJoin:
                        Debug.Log("confrim join test");
                        OnConfirmJoin?.Invoke(this, JsonUtility.FromJson<ConfirmJoinMessage>(jsonData));
                        break;
                    case MessageSubject.DeptUpdate:
                        OnDeptUpdate?.Invoke(this, JsonUtility.FromJson<DeptUpdateMessage>(jsonData));
                        break;
                    case MessageSubject.StartGame:
                        OnStartGame?.Invoke(this, JsonUtility.FromJson<StartGameMessage>(jsonData));
                        break;
                    case MessageSubject.CardHandIn:
                        OnCardHandIn?.Invoke(this, JsonUtility.FromJson<CardHandInMessage>(jsonData));
                        break;
                    case MessageSubject.StopRound:
                        var stopRoundMsg = JsonUtility.FromJson<StopRoundMessage>(jsonData);
                        if (stopRoundMsg != null)
                        {
                            Debug.LogWarning($"Invoking OnStopRound event with {stopRoundMsg.RoundNumber}");
                            try
                            {
                                OnStopRound?.Invoke(this, stopRoundMsg);
                                Debug.LogWarning("OnStopRound invoked successfully");
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"Error in OnStopRound event handler: {ex.Message}\n{ex.StackTrace}");
                            }
                        }
                        break;
                    case MessageSubject.StartRound:
                        OnStartRound?.Invoke(this, JsonUtility.FromJson<StartRoundMessage>(jsonData));
                        break;
                    case MessageSubject.Rejected:
                        OnRejected?.Invoke(this, JsonUtility.FromJson<RejectedMessage>(jsonData));
                        break;
                    case MessageSubject.BuyCards:
                        OnBuyCards?.Invoke(this, JsonUtility.FromJson<BuyCardsRequestMessage>(jsonData));
                        break;
                    case MessageSubject.CancelListing:
                        OnCancelListing?.Invoke(this, JsonUtility.FromJson<CancelListingMessage>(jsonData));
                        break;
                    case MessageSubject.ConfirmBuy:
                        OnConfirmBuy?.Invoke(this, JsonUtility.FromJson<ConfirmBuyMessage>(jsonData));
                        break;
                    case MessageSubject.CreateSession:
                        OnCreateSession?.Invoke(this, JsonUtility.FromJson<CreateSessionMessage>(jsonData));
                        break;
                    case MessageSubject.DonateMoney:
                        OnDonateMoney?.Invoke(this, JsonUtility.FromJson<DonateMoneyMessage>(jsonData));
                        break;
                    case MessageSubject.DonatePoints:
                        OnDonatePoints?.Invoke(this, JsonUtility.FromJson<DonatePointsMessage>(jsonData));
                        break;
                    case MessageSubject.EndGame:
                        OnEndGame?.Invoke(this, JsonUtility.FromJson<EndGameMessage>(jsonData));
                        break;
                    case MessageSubject.HeartBeat:
                        OnHeartBeat?.Invoke(this, JsonUtility.FromJson<HeartBeatMessage>(jsonData));
                        break;
                    case MessageSubject.JoinRequest:
                        OnJoinrequest?.Invoke(this, JsonUtility.FromJson<JoinRequestMessage>(jsonData));
                        break;
                    case MessageSubject.EndOfRounds:
                        OnEndOfRounds?.Invoke(this, JsonUtility.FromJson<EndOfRoundsMessage>(jsonData));
                        break;
                    case MessageSubject.ConfirmCancelListing:
                        OnConfirmCancelListing?.Invoke(this,
                            JsonUtility.FromJson<ConfirmCancelListingMessage>(jsonData));
                        break;
                    case MessageSubject.ConfirmHandIn:
                        OnConfirmHandIn?.Invoke(this, JsonUtility.FromJson<ConfirmHandInMessage>(jsonData));
                        break;
                    case MessageSubject.ListCards:
                        OnListCards?.Invoke(this, JsonUtility.FromJson<ListCardsmessage>(jsonData));
                        break;
                    case MessageSubject.MakeBidding:
                        OnMakeBidding?.Invoke(this, JsonUtility.FromJson<CreateBidMessage>(jsonData));
                        break;
                    case MessageSubject.AcceptBidding:
                        OnAcceptBidding?.Invoke(this, JsonUtility.FromJson<AcceptBidMessage>(jsonData));
                        break;
                    case MessageSubject.CancelBidding:
                        OnCancelBidding?.Invoke(this, JsonUtility.FromJson<CancelBidMessage>(jsonData));
                        break;
                    case MessageSubject.RejectBidding:
                        OnRejectBidding?.Invoke(this, JsonUtility.FromJson<RejectBidMessage>(jsonData));
                        break;
                    case MessageSubject.RespondBidding:
                        OnRespondBidding?.Invoke(this, JsonUtility.FromJson<CounterBidMessage>(jsonData));
                        break;
                    case MessageSubject.AcceptCounterBidding:
                        OnAcceptCounterBidding?.Invoke(this,
                            JsonUtility.FromJson<AcceptCounterBiddingMessage>(jsonData));
                        break;
                    case MessageSubject.Continue:
                        OnContinue?.Invoke(this, JsonUtility.FromJson<ContinueMessage>(jsonData));
                        break;
                    case MessageSubject.AbortSession:
                        OnAbortSession?.Invoke(this, JsonUtility.FromJson<AbortSessionMessage>(jsonData));
                        break;
                    case MessageSubject.SkipRounds:
                        OnSkipRound?.Invoke(this, JsonUtility.FromJson<SkipRoundMessage>(jsonData));
                        break;
                    default:
                        throw new NotImplementedException("This message is not implemented");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing message data: {ex.Message}");
            }
        }

        private string ConvertToJson(Dictionary<string, string> data)
        {
            var jsonParts = new List<string>();

            foreach (var kvp in data)
            {
                if (kvp.Key == "Timestamp")
                {
                    jsonParts.Add($"\"DateTimeStamp\": \"{kvp.Value}\"");
                }
                else if (kvp.Key == "Cards")
                {
                    Debug.LogWarning("Cards" + kvp.Value);

                    var raw = kvp.Value.Trim();

                    string cardsJson;

                    if (raw.StartsWith("System.") || string.IsNullOrEmpty(raw))
                    {
                        cardsJson = "[]";
                    }
                    else
                    {
                        var cardValues = raw.Split(';', StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => int.TryParse(v, out var n) ? n.ToString() : $"\"{v}\"");

                        cardsJson = "[" + string.Join(",", cardValues) + "]";
                    }

                    jsonParts.Add($"\"Cards\": {cardsJson}");
                }
                else
                {
                    jsonParts.Add($"\"{kvp.Key}\": \"{kvp.Value}\"");
                }
            }

            return "{" + string.Join(",", jsonParts) + "}";
        }


        public async Task Subscribe(string topic)
        {
            string message = $"{{\"action\": \"subscribe\", \"subject\": \"{topic}\"}}";
            await Send(message);
            Debug.LogWarning($"Subscribed to: {topic}");
        }

        public async Task Unsubscribe(string topic)
        {
            string message = $"{{\"action\": \"unsubscribe\", \"subject\": \"{topic}\"}}";
            await Send(message);
        }

        public async Task Publish<T>(string topic, T content)
        {
            // Serialize the inner message to JSON
            string serializedContent = JsonUtility.ToJson(content);

            // Manually wrap it in a JSON message
            string message = $"{{\"action\": \"publish\", \"subject\": \"{topic}\", \"data\": {serializedContent}}}";

            Debug.Log($"Publishing: {message}");

            await Send(message);
        }


        private async Task Send(string message)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                Debug.LogError("WebSocket is not open. Cannot send message.");
                return;
            }

            try
            {
                await _webSocket.SendText(message);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending message: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _webSocket?.Close();
        }

        // Call this in Update() of a MonoBehaviour to process WebSocket messages
        public void DispatchMessageQueue()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _webSocket?.DispatchMessageQueue();
#endif
        }
    }
}
