using System;
using UnityEngine;
using Version1.Nats.Messages;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;

namespace Version1.NetworkManager
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        public NetworkManager()
        {
            if (Instance != null) return;
            Instance = this;
        }


        private Nats.NatsClient _natsClient;

        public void SubscribeToSubject(string subject)
        {
            _natsClient.SubscribeToSubject(subject);
        }

        public void Publish(string sessionID, BaseMessage baseMessage, bool flushImmediately = true)
        {
            _natsClient.Publish(sessionID, baseMessage, flushImmediately);
        }

        private void Awake()
        {
            _natsClient = new Nats.NatsClient();
            
            _natsClient.OnJoinrequest += NatsClientOnOnJoinrequest;
            _natsClient.OnRejected += NatsClientOnOnRejected;
            _natsClient.OnAcceptBidding += NatsClientOnOnAcceptBidding;
            _natsClient.OnBuyCards += NatsClientOnOnBuyCards;
            _natsClient.OnCancelBidding += NatsClientOnOnCancelBidding;
            _natsClient.OnCancelListing += NatsClientOnOnCancelListing;
            _natsClient.OnConfirmBuy += NatsClientOnOnConfirmBuy;
            _natsClient.OnConfirmJoin += NatsClientOnOnConfirmJoin;
            _natsClient.OnCreateSession += NatsClientOnOnCreateSession;
            _natsClient.OnDeptUpdate += NatsClientOnOnDeptUpdate;
            _natsClient.OnDonateMoney += NatsClientOnOnDonateMoney;
            _natsClient.OnDonatePoints += NatsClientOnOnDonatePoints;
            _natsClient.OnEndGame += NatsClientOnOnEndGame;
            _natsClient.OnHeartBeat += NatsClientOnOnHeartBeat;
            _natsClient.OnListCards += NatsClientOnOnListCards;
            _natsClient.OnMakeBidding += NatsClientOnOnMakeBidding;
            _natsClient.OnRejectBidding += NatsClientOnOnRejectBidding;
            _natsClient.OnRespondBidding += NatsClientOnOnRespondBidding;
            _natsClient.OnStartGame += NatsClientOnOnStartGame;
            _natsClient.OnStartRound += NatsClientOnOnStartRound;
            _natsClient.OnStopRound += NatsClientOnOnStopRound;
            _natsClient.OnAcceptCounterBidding += NatsClientOnOnAcceptCounterBidding;
            _natsClient.OnCardHandIn += NatsClientOnOnCardHandIn;
            _natsClient.OnConfirmCancelListing += NatsClientOnOnConfirmCancelListing;
            _natsClient.OnConfirmHandIn += NatsClientOnOnConfirmHandIn;
            _natsClient.OnEndOfRounds += NatsClientOnOnEndOfRounds;
            _natsClient.OnConnect += NatsClientOnOnConnect;
            
            _natsClient.SubscribeToSubject("*");
        }


        private void FixedUpdate()
        {
            _natsClient.HandleMessages();
        }

        public void OnDestroy()
        {
            NatsClient.Instance.StopHeartbeat();
        }
        private void NatsClientOnOnConnect(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnEndOfRounds(object sender, EndOfRoundsMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnConfirmHandIn(object sender, ConfirmHandInMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnConfirmCancelListing(object sender, ConfirmCancelListingMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnCardHandIn(object sender, CardHandInMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnAcceptCounterBidding(object sender, AcceptCounterBiddingMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnStopRound(object sender, StopRoundMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnStartRound(object sender, StartRoundMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnStartGame(object sender, StartGameMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnRespondBidding(object sender, RespondBiddingMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnRejectBidding(object sender, RejectBiddingMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnMakeBidding(object sender, MakeBiddingMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnListCards(object sender, ListCardsmessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnHeartBeat(object sender, HeartBeatMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnEndGame(object sender, EndGameMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnDonatePoints(object sender, DonatePointsMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnDonateMoney(object sender, DonateMoneyMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnDeptUpdate(object sender, DeptUpdateMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnCreateSession(object sender, CreateSessionMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnConfirmJoin(object sender, ConfirmJoinMessage e)
        {
            _natsClient.StartHeartbeat();
        }

        private void NatsClientOnOnConfirmBuy(object sender, ConfirmBuyMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnCancelListing(object sender, CancelListingMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnCancelBidding(object sender, CancelBiddingMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnBuyCards(object sender, BuyCardsRequestMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnAcceptBidding(object sender, AcceptBiddingMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnRejected(object sender, RejectedMessage e)
        {
            throw new NotImplementedException();
        }

        private void NatsClientOnOnJoinrequest(object sender, JoinRequestMessage e)
        {
            throw new NotImplementedException();
        }
    }
}