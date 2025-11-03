using System.Collections.Generic;
using UnityEngine;
using System;
using Version1.Nats.Messages;
using Version1.Nats.Messages.Client;
using Version1.Websocket;

namespace Version1.Nats
{
    public class NatsHost
    {
        public WebsocketClient WebSocketClient;
        private static NatsHost _instance;

        public static NatsHost C
        {
            get
            {
                if (_instance == null)
                    _instance = new NatsHost();
                return _instance;
            }
        }

        private NatsHost()
        {
        #if UNITY_WEBGL && !UNITY_EDITOR
            string url = Application.absoluteURL;
            System.Uri uri = new System.Uri(url);
            string wsUrl = $"ws://{uri.Host}:8080/ws";
            WebSocketClient = new WebsocketClient(wsUrl);
        #else
            // For testing in Unity Editor
            WebSocketClient = new WebsocketClient("ws://localhost:8080/ws");
        #endif
        }

        /*public event EventHandler<ListCardsmessage> OnListCards;
        public event EventHandler<BuyCardsRequestMessage> OnBuyCards;
        public event EventHandler<CancelListingMessage> OnCancelListing;
        public event EventHandler<DonateMoneyMessage> OnDonateMoney;
        public event EventHandler<DonatePointsMessage> OnDonatePoints;
        public event EventHandler<DeptUpdateMessage> OnDeptUpdate;
        public event EventHandler<CardHandInMessage> OnCardHandIn;
        public event EventHandler<HeartBeatMessage> OnHeartBeat;
        public event EventHandler<JoinRequestMessage> OnJoinrequest;
        public event EventHandler<ContinueMessage> OnContinue;
        public event EventHandler<string> MessageLog;*/

        public async void Subscribe(string sessionID)
        {
            try
            {
                await WebSocketClient.Subscribe(sessionID);
                throw new NotImplementedException("Subscribed");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Nats: Error during subscribing: {ex.Message}");
                //OnError?.Invoke(this, "");
            }
        }

        public async void Publish(string sessionID, BaseMessage baseMessage, bool flushImmediately = true)
        {
            try
            {
                await WebSocketClient.Publish(sessionID, baseMessage);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Nats: Error during publishing: {ex.Message}");
                //OnError?.Invoke(this, "");
            }
        }

        public void HandleMessages()
        {
            WebSocketClient.DispatchMessageQueue();
        }
    }
}
