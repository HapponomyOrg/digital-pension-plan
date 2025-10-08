/*using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;
using Version1.Nats.Messages;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;

namespace Version1.Nats
{
    [System.Serializable]
    public class WebSocketMessage
    {
        public string action;
        public string subject;
        public string data;
    }

    public class NatsWrapper
    {
        private WebSocket _webSocket;
        private string _uri;
        private NatsClient _natsClient;
        private NatsHost _natsHost;

        public NatsWrapper(string uri)
        {
            _uri = uri;
            _natsClient = NatsClient.C; // Get the singleton instance
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
            await _webSocket.Connect();
        }

        private void OnMessageReceived(byte[] bytes)
        {
            try
            {
                var messageJson = System.Text.Encoding.UTF8.GetString(bytes);
                Debug.Log($"WebSocket Received: {messageJson}");

                // Parse the WebSocket message using Unity's JsonUtility
                var wsMessage = JsonUtility.FromJson<WebSocketMessage>(messageJson);

                if (wsMessage?.action == "message" && !string.IsNullOrEmpty(wsMessage.data))
                {
                    // Parse the data field to create a BaseMessage
                    var baseMessage = ParseMessageData(wsMessage.data);

                    if (baseMessage != null)
                    {

                        if (_natsClient != null)
                        {
                            // Add the message to the client's event queue
                            _natsClient.EventsReceived.Enqueue(baseMessage);   
                        } else if (_natsHost != null)
                        {
                            _natsHost.EventsReceived.Enqueue(baseMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error processing WebSocket message: {ex.Message}");
            }
        }

        private BaseMessage ParseMessageData(string data)
        {
            try
            {
                var parts = data.Split(',');
                var messageData = new Dictionary<string, string>();

                if (parts.Length > 0)
                {
                    messageData["Timestamp"] = parts[0].Trim();
                }
                
                for (int i = 1; i < parts.Length; i++)
                {
                    var part = parts[i].Trim();
                    var colonIndex = part.IndexOf(':');
                    if (colonIndex > 0)
                    {
                        var key = part.Substring(0, colonIndex).Trim();
                        var value = part.Substring(colonIndex + 1).Trim();
                        messageData[key] = value;
                    }
                }

                // Extract common fields
                var timestamp = GetValueOrDefault(messageData, "Timestamp", DateTime.Now.ToString("o"));
                var lobbyId = int.TryParse(GetValueOrDefault(messageData, "Lobby", "0"), out var lobby) ? lobby : 0;
                var subject = GetValueOrDefault(messageData, "Subject", "Unknown");
                var playerId = int.TryParse(GetValueOrDefault(messageData, "Player", "0"), out var player) ? player : 0;
                var playerName = GetValueOrDefault(messageData, "PlayerName", "Unknown");

                // Create appropriate message type based on subject
                return CreateMessageFromSubject(subject, timestamp, lobbyId, playerId, playerName, messageData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing message data: {ex.Message}");
                return null;
            }
        }

        // Helper method since GetValueOrDefault might not be available in older Unity versions
        private static string GetValueOrDefault(Dictionary<string, string> dict, string key, string defaultValue)
        {
            return dict.GetValueOrDefault(key, defaultValue);
        }

        private BaseMessage CreateMessageFromSubject(string subject, string timestamp, int lobbyId, int playerId,
            string playerName, Dictionary<string, string> messageData)
        {
            switch (subject)
            {
                case MessageSubject.JoinRequest:
                    var age = int.TryParse(GetValueOrDefault(messageData, "Age", "-1"), out var a) ? a : -1;
                    var gender = int.TryParse(GetValueOrDefault(messageData, "Gender", "-1"), out var g) ? g : -1;
                    var requestId = GetValueOrDefault(messageData, "RequestID", Guid.NewGuid().ToString());
                    return new Version1.Nats.Messages.Client.JoinRequestMessage(timestamp, lobbyId, playerId,
                        playerName, age, gender, requestId);

                case MessageSubject.HeartBeat:
                    var balance = int.TryParse(GetValueOrDefault(messageData, "Balance", "0"), out var bal) ? bal : 0;
                    var points = int.TryParse(GetValueOrDefault(messageData, "Points", "0"), out var pts) ? pts : 0;
                    var intArrayString0 = GetValueOrDefault(messageData, "Cards", "[]");
                    var cards0 = ParseIntArrayManual(intArrayString0);
                    var allPointsString = GetValueOrDefault(messageData, "AllPoints", "[]");
                    var allPoints = ParseIntArrayManual(allPointsString);
                    return new HeartBeatMessage(timestamp, lobbyId, playerId, playerName, balance, cards0, points, allPoints);

                case MessageSubject.ConfirmJoin:
                    var lobbyPlayerId =
                        int.TryParse(GetValueOrDefault(messageData, "LobbyPlayerID", "0"), out var lobbyPlayer)
                            ? lobbyPlayer
                            : 0;
                    var confirmRequestId = GetValueOrDefault(messageData, "RequestID", Guid.NewGuid().ToString());
                    var age2 = int.TryParse(GetValueOrDefault(messageData, "Age", "-1"), out var a2) ? a2 : -1;
                    var gender2 = int.TryParse(GetValueOrDefault(messageData, "Gender", "-1"), out var g2) ? g2 : -1;
                    return new ConfirmJoinMessage(timestamp, lobbyId, playerId,
                        lobbyPlayerId, playerName, age2, gender2, confirmRequestId);

                case MessageSubject.ListCards:
                    var auctionID = GetValueOrDefault(messageData, "AuctionID", "error");
                    var intArrayString = GetValueOrDefault(messageData, "Cards", "[]");
                    var cards = ParseIntArrayManual(intArrayString);
                    var amount = int.TryParse(GetValueOrDefault(messageData, "amount", "-1"), out var amt) ? amt : -1;

                    return new ListCardsmessage(timestamp, lobbyId, playerId, playerName, auctionID, cards, amount);
                case MessageSubject.BuyCards:
                    var auctionID2 = GetValueOrDefault(messageData, "AuctionID", "error");
                    return new BuyCardsRequestMessage(timestamp, lobbyId, playerId, auctionID2);
                case MessageSubject.CancelListing:
                    var auctionID3 = GetValueOrDefault(messageData, "AuctionID", "error");
                    return new CancelListingMessage(timestamp, lobbyId, playerId, auctionID3);
                case MessageSubject.DonateMoney:
                    var amount2 = int.TryParse(GetValueOrDefault(messageData, "Amount", "-1"), out var amt2)
                        ? amt2
                        : -1;
                    return new DonateMoneyMessage(timestamp, lobbyId, playerId, amount2);
                case MessageSubject.DonatePoints:
                    var receiver = int.TryParse(GetValueOrDefault(messageData, "Receiver", "-1"), out var rcvr)
                        ? rcvr
                        : -1;
                    var amount3 = int.TryParse(GetValueOrDefault(messageData, "Amount", "-1"), out var amt3)
                        ? amt3
                        : -1;
                    return new DonatePointsMessage(timestamp, lobbyId, playerId, playerName, receiver, amount3);
                case MessageSubject.CardHandIn:
                    var cardID = int.TryParse(GetValueOrDefault(messageData, "CardID", "-1"), out var crdid)
                        ? crdid
                        : -1;
                    var points2 = int.TryParse(GetValueOrDefault(messageData, "Points", "-1"), out var pts2)
                        ? pts2
                        : -1;
                    return new CardHandInMessage(timestamp, lobbyId, playerId, cardID, points2);
                case MessageSubject.CreateSession:
                    var sessionToken = int.TryParse(GetValueOrDefault(messageData, "SessionToken", "-1"), out var tkn)
                        ? tkn
                        : -1;
                    return new CreateSessionMessage(timestamp, lobbyId, playerId, sessionToken);
                case MessageSubject.StartGame:
                    var otherPlayerID =
                        int.TryParse(GetValueOrDefault(messageData, "OtherPlayerID", "-1"), out var plrid) ? plrid : -1;
                    var intArrayString2 = GetValueOrDefault(messageData, "Cards", "[]");
                    int[] cards2 = ParseIntArrayManual(intArrayString2);
                    var balance2 = int.TryParse(GetValueOrDefault(messageData, "Balance", "-1"), out var blns2)
                        ? blns2
                        : -1;
                    var interestMode = int.TryParse(GetValueOrDefault(messageData, "InterestMode", "-1"), out var md)
                        ? md
                        : -1;
                    return new StartGameMessage(timestamp, lobbyId, playerId, otherPlayerID, balance2, cards2,
                        interestMode);
                case MessageSubject.StartRound:
                    var roundNumber = int.TryParse(GetValueOrDefault(messageData, "RoundNumber", "-1"), out var nmbr)
                        ? nmbr
                        : -1;
                    var roundName = GetValueOrDefault(messageData, "RoundName", "unknown");
                    var duration = int.TryParse(GetValueOrDefault(messageData, "Duration", "-1"), out var drtn)
                        ? drtn
                        : -1;
                    return new StartRoundMessage(timestamp, lobbyId, playerId, roundNumber, roundName, duration);
                case MessageSubject.StopRound:
                    var roundNumber2 = int.TryParse(GetValueOrDefault(messageData, "RoundNumber", "-1"), out var nmbr2)
                        ? nmbr2
                        : -1;
                    return new StopRoundMessage(timestamp, lobbyId, playerId, roundNumber2);
                case MessageSubject.EndOfRounds:
                    return new EndOfRoundsMessage(timestamp, lobbyId, playerId);
                case MessageSubject.EndGame:
                    return new EndGameMessage(timestamp, lobbyId, playerId);
                case MessageSubject.Rejected:
                    var targetPlayer = GetValueOrDefault(messageData, "TargetPlayer", "unknown");
                    var referenceID = GetValueOrDefault(messageData, "ReferenceID", "unknown");
                    var message = GetValueOrDefault(messageData, "Message", "unknown");
                    var requestID = GetValueOrDefault(messageData, "RequestID", "unknown");

                    return new RejectedMessage(timestamp, lobbyId, playerId, targetPlayer, referenceID, message,
                        requestID);
                default:
                    Debug.LogWarning($"Unknown message subject: {subject}");
                    return null;
            }
        }

        public async Task Subscribe(string topic)
        {
            string message = $"{{\"action\": \"subscribe\", \"subject\": \"{topic}\"}}";
            await Send(message);
            Debug.Log($"Subscribed to: {topic}");
        }

        public async Task Unsubscribe(string topic)
        {
            string message = $"{{\"action\": \"unsubscribe\", \"subject\": \"{topic}\"}}";
            await Send(message);
        }

        public async Task Publish(string topic, BaseMessage content)
        {
            string serializedContent = content.ToString();
            string message =
                $"{{\"action\": \"publish\", \"subject\": \"{topic}\", \"data\": \"{serializedContent}\"}}";
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

        private static int[] ParseIntArrayManual(string jsonArray)
        {
            try
            {
                // Remove brackets and split
                jsonArray = jsonArray.Trim('[', ']', ' ');
                if (string.IsNullOrEmpty(jsonArray))
                    return Array.Empty<int>();

                var stringNumbers = jsonArray.Split(',');
                var result = new int[stringNumbers.Length];

                for (var i = 0; i < stringNumbers.Length; i++)
                {
                    if (int.TryParse(stringNumbers[i].Trim(), out var num))
                        result[i] = num;
                    else
                        return Array.Empty<int>(); // Return empty array on parse failure
                }

                return result;
            }
            catch
            {
                return Array.Empty<int>();
            }
        }
    }
}*/
