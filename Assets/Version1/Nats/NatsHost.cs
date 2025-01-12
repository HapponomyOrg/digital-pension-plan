using System.Collections.Generic;
using UnityEngine;
using System;
using Version1.Nats.Messages;
using Version1.Nats.Messages.Client;

namespace Version1.Nats
{
    public class NatsHost : Connection
    {
        public int LobbyID = 0;
        public int numGames = 0;

        public static NatsHost C { get; private set; }

        public event EventHandler<ListCardsmessage> OnListCards;
        public event EventHandler<BuyCardsRequestMessage> OnBuyCards;
        public event EventHandler<CancelListingMessage> OnCancelListing;
        public event EventHandler<DonateMoneyMessage> OnDonateMoney;
        public event EventHandler<DonatePointsMessage> OnDonatePoints;
        public event EventHandler<DeptUpdateMessage> OnDeptUpdate;
        public event EventHandler<CardHandInMessage> OnCardHandIn;
        public event EventHandler<HeartBeatMessage> OnHeartBeat;
        public event EventHandler<JoinRequestMessage> OnJoinrequest;


        public NatsHost()
        {
            if (C != null) return;

            C = this;
            EventsReceived = new Queue<BaseMessage>();
        }

        protected override void Subscribe()
        {
        }

        // TODO() maybe we want to move this handle messages void into the function above.
        public void HandleMessages()
        {
            if (EventsReceived.Count < 1) return;
            var message = EventsReceived.Dequeue();

            switch (message.Subject)
            {
                case MessageSubject.ListCards:
                    OnListCards?.Invoke(null, (ListCardsmessage)message);
                    break;
                case MessageSubject.BuyCards:
                    OnBuyCards?.Invoke(null, (BuyCardsRequestMessage)message);
                    break;
                case MessageSubject.CancelListing:
                    OnCancelListing?.Invoke(null, (CancelListingMessage)message);
                    break;
                case MessageSubject.DonateMoney:
                    OnDonateMoney?.Invoke(null, (DonateMoneyMessage)message);
                    break;
                case MessageSubject.DonatePoints:
                    OnDonatePoints?.Invoke(null, (DonatePointsMessage)message);
                    break;
                case MessageSubject.DeptUpdate:
                    OnDeptUpdate?.Invoke(null, (DeptUpdateMessage)message);
                    break;
                case MessageSubject.CardHandIn:
                    OnCardHandIn?.Invoke(null, (CardHandInMessage)message);
                    break;
                case MessageSubject.HeartBeat:
                    OnHeartBeat?.Invoke(null, (HeartBeatMessage)message);
                    break;
                case MessageSubject.JoinRequest:
                    OnJoinrequest?.Invoke(null, (JoinRequestMessage)message);
                    break;
                default:
                    Debug.Log($"{message.Subject} is not a known subject");
                    break;
            }
        }
    }
}