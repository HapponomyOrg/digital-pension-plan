/*using System.Collections.Generic;
using UnityEngine;
using NATS;
using System;
using System.Threading;
using System.Threading.Tasks;
using Connection = NATS.Connection;

public class NatsClient : Connection
{
    private CancellationTokenSource _cancellationTokenSource;

    public int HeartbeatInterval = 5000;
    public static NatsClient C { get; private set; }

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
    public event EventHandler<MakeBiddingMessage> OnMakeBidding;
    public event EventHandler<AcceptBiddingMessage> OnAcceptBidding;
    public event EventHandler<CancelBiddingMessage> OnCancelBidding;
    public event EventHandler<RejectBiddingMessage> OnRejectBidding;
    public event EventHandler<RespondBiddingMessage> OnRespondBidding;
    public event EventHandler<AcceptCounterBiddingMessage> OnAcceptCounterBidding;


    public NatsClient()
    {
        if (C != null) return;

        C = this;
        EventsReceived = new Queue<BaseMessage>();
    }

    public void StartHeartbeat()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        Task.Run(async () =>
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                Debug.Log("HEARTBEAT");
                
                HeartBeatMessage msg = new HeartBeatMessage(DateTime.Now.ToString("o"),
                    PlayerManager.Instance.LobbyID, PlayerManager.Instance.PlayerId,
                    PlayerManager.Instance.PlayerName, PlayerManager.Instance.Balance,
                    PlayerManager.Instance.Cards.ToArray(), PlayerManager.Instance.Points, PlayerManager.Instance.allPoints.ToArray());

                Publish(PlayerManager.Instance.LobbyID.ToString(), msg);

                await Task.Delay(HeartbeatInterval);
            }
        }, _cancellationTokenSource.Token);
    }

    public void StopHeartbeat()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }

    protected override void Subscribe()
    {
    }

    // TODO() maybe we want to move this handle messages void into the function above.
    public void HandleMessages()
    {
        if (EventsReceived.Count >= 1)
        {
            var message = EventsReceived.Dequeue();

            if (message == null) return;

            if (message.PlayerID == PlayerManager.Instance.PlayerId) return;

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
                case MessageSubject.CreateSession:
                    OnCreateSession?.Invoke(null, (CreateSessionMessage)message);
                    break;
                case MessageSubject.StartGame:
                    OnStartGame?.Invoke(null, (StartGameMessage)message);
                    break;
                case MessageSubject.StartRound:
                    OnStartRound?.Invoke(null, (StartRoundMessage)message);
                    break;
                case MessageSubject.StopRound:
                    OnStopRound?.Invoke(null, (StopRoundMessage)message);
                    break;
                case MessageSubject.EndOfRounds:
                    OnEndOfRounds?.Invoke(null, (EndOfRoundsMessage)message);
                    break;
                case MessageSubject.EndGame:
                    OnEndGame?.Invoke(null, (EndGameMessage)message);
                    break;
                case MessageSubject.ConfirmJoin:
                    OnConfirmJoin?.Invoke(null, (ConfirmJoinMessage)message);
                    break;
                case MessageSubject.Rejected:
                    OnRejected?.Invoke(null, (RejectedMessage)message);
                    break;
                case MessageSubject.ConfirmBuy:
                    OnConfirmBuy?.Invoke(null, (ConfirmBuyMessage)message);
                    break;
                case MessageSubject.ConfirmHandIn:
                    OnConfirmHandIn?.Invoke(null, (ConfirmHandInMessage)message);
                    break;
                case MessageSubject.ConfirmCancelListing:
                    OnConfirmCancelListing?.Invoke(null, (ConfirmCancelListingMessage)message);
                    break;
                case MessageSubject.MakeBidding:
                    OnMakeBidding?.Invoke(null, (MakeBiddingMessage)message);
                    break;
                case MessageSubject.AcceptBidding:
                    OnAcceptBidding?.Invoke(null, (AcceptBiddingMessage)message);
                    break;
                case MessageSubject.CancelBidding:
                    OnCancelBidding?.Invoke(null, (CancelBiddingMessage)message);
                    break;
                case MessageSubject.RejectBidding:
                    OnRejectBidding?.Invoke(null, (RejectBiddingMessage)message);
                    break;
                case MessageSubject.RespondBidding:
                    OnRespondBidding?.Invoke(null, (RespondBiddingMessage)message);
                    break;
                case MessageSubject.AcceptCounterBidding:
                    OnAcceptCounterBidding?.Invoke(null, (AcceptCounterBiddingMessage)message);
                    break;
                default:
                    Debug.Log($"{message.Subject} is not a known subject");
                    break;
            }
        }
    }
}*/

using System.Collections.Generic;
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
        public event EventHandler<MakeBiddingMessage> OnMakeBidding;
        public event EventHandler<AcceptBiddingMessage> OnAcceptBidding;
        public event EventHandler<CancelBiddingMessage> OnCancelBidding;
        public event EventHandler<RejectBiddingMessage> OnRejectBidding;
        public event EventHandler<RespondBiddingMessage> OnRespondBidding;
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
            
            DispatchMessage(message);
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
                    OnMakeBidding?.Invoke(this, (MakeBiddingMessage)message);
                    break;
                case MessageSubject.AcceptBidding:
                    OnAcceptBidding?.Invoke(this, (AcceptBiddingMessage)message);
                    break;
                case MessageSubject.CancelBidding:
                    OnCancelBidding?.Invoke(this, (CancelBiddingMessage)message);
                    break;
                case MessageSubject.RejectBidding:
                    OnRejectBidding?.Invoke(this, (RejectBiddingMessage)message);
                    break;
                case MessageSubject.RespondBidding:
                    OnRespondBidding?.Invoke(this, (RespondBiddingMessage)message);
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
}
