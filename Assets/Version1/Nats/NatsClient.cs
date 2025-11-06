/*using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using Version1.Nats.Messages;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;

namespace Version1.Nats
{
    public class NatsClient : Connection
    {
        public static NatsClient C { get; private set; }
        
        // Make EventsReceived public so NatsWrapper can add messages to it
        public Queue<BaseMessage> EventsReceived { get; private set; }
        
        public NatsClient()
        {
            if (C != null) return;

            C = this;
            EventsReceived = new Queue<BaseMessage>();
        }
        
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
        

        protected override void Subscribe()
        {
        }
        
        public void HandleMessages()
        {
            if (EventsReceived.Count < 1) return;
            

            var message = EventsReceived.Dequeue();
            if (message == null || message.PlayerID == PlayerData.PlayerData.Instance.PlayerId) return;

            if (message.Subject != MessageSubject.HeartBeat)
            {
                Debug.Log($"Handled message: {message}");
            }
            
            DispatchMessage(message);
            Debug.Log("messages processed");

        }

        private void DispatchMessage(BaseMessage message)
        {
            switch (message.Subject)
            {
                case MessageSubject.ListCards:
                    OnListCards?.Invoke(this, (ListCardsmessage)message);
                    break;
                case MessageSubject.BuyCards:
                    OnBuyCards?.Invoke(this, (BuyCardsRequestMessage)message);
                    break;
                case MessageSubject.CancelListing:
                    OnCancelListing?.Invoke(this, (CancelListingMessage)message);
                    break;
                case MessageSubject.DonateMoney:
                    OnDonateMoney?.Invoke(this, (DonateMoneyMessage)message);
                    break;
                case MessageSubject.DonatePoints:
                    OnDonatePoints?.Invoke(this, (DonatePointsMessage)message);
                    break;
                case MessageSubject.DeptUpdate:
                    OnDeptUpdate?.Invoke(this, (DeptUpdateMessage)message);
                    break;
                case MessageSubject.CardHandIn:
                    OnCardHandIn?.Invoke(this, (CardHandInMessage)message);
                    break;
                case MessageSubject.HeartBeat:
                    OnHeartBeat?.Invoke(this, (HeartBeatMessage)message);
                    break;
                case MessageSubject.JoinRequest:
                    OnJoinrequest?.Invoke(this, (JoinRequestMessage)message);
                    break;
                case MessageSubject.CreateSession:
                    OnCreateSession?.Invoke(this, (CreateSessionMessage)message);
                    break;
                case MessageSubject.StartGame:
                    OnStartGame?.Invoke(this, (StartGameMessage)message);
                    break;
                case MessageSubject.StartRound:
                    OnStartRound?.Invoke(this, (StartRoundMessage)message);
                    OnStopRound = null;
                    break;
                case MessageSubject.StopRound:
                    OnStopRound?.Invoke(this, (StopRoundMessage)message);
                    OnStopRound = null;
                    break;
                case MessageSubject.EndOfRounds:
                    OnEndOfRounds?.Invoke(this, (EndOfRoundsMessage)message);
                    break;
                case MessageSubject.EndGame:
                    OnEndGame?.Invoke(this, (EndGameMessage)message);
                    break;
                case MessageSubject.ConfirmJoin:
                    OnConfirmJoin?.Invoke(this, (ConfirmJoinMessage)message);
                    break;
                case MessageSubject.Rejected:
                    OnRejected?.Invoke(this, (RejectedMessage)message);
                    break;
                case MessageSubject.ConfirmBuy:
                    OnConfirmBuy?.Invoke(this, (ConfirmBuyMessage)message);
                    break;
                case MessageSubject.ConfirmHandIn:
                    OnConfirmHandIn?.Invoke(this, (ConfirmHandInMessage)message);
                    break;
                case MessageSubject.ConfirmCancelListing:
                    OnConfirmCancelListing?.Invoke(this, (ConfirmCancelListingMessage)message);
                    break;
                case MessageSubject.MakeBidding:
                    OnMakeBidding?.Invoke(this, (CreateBidMessage)message);
                    break;
                case MessageSubject.AcceptBidding:
                    OnAcceptBidding?.Invoke(this, (AcceptBidMessage)message);
                    break;
                case MessageSubject.CancelBidding:
                    OnCancelBidding?.Invoke(this, (CancelBidMessage)message);
                    break;
                case MessageSubject.RejectBidding:
                    OnRejectBidding?.Invoke(this, (RejectBidMessage)message);
                    break;
                case MessageSubject.RespondBidding:
                    OnRespondBidding?.Invoke(this, (CounterBidMessage)message);
                    break;
                case MessageSubject.AcceptCounterBidding:
                    OnAcceptCounterBidding?.Invoke(this, (AcceptCounterBiddingMessage)message);
                    break;
                case MessageSubject.AbortSession:
                    OnAbortSession?.Invoke(this, (AbortSessionMessage)message);
                    break;                
                case MessageSubject.SkipRounds:
                    OnSkipRound?.Invoke(this, (SkipRoundMessage)message);
                    break;
                
                default:
                    Debug.LogWarning($"Unknown message subject: {message.Subject}");
                    break;
            }
        }
    }
}*/
