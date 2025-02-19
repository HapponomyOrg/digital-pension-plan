using System;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            DontDestroyOnLoad(gameObject);
            
            _natsClient = new Nats.NatsClient();
            
            //_natsClient.OnJoinrequest += NatsClientOnOnJoinrequest;
            _natsClient.OnRejected += NatsClientOnOnRejected;
            _natsClient.OnAcceptBidding += NatsClientOnOnAcceptBidding;
            _natsClient.OnBuyCards += NatsClientOnOnBuyCards;
            _natsClient.OnCancelBidding += NatsClientOnOnCancelBidding;
            _natsClient.OnCancelListing += NatsClientOnOnCancelListing;
            _natsClient.OnConfirmBuy += NatsClientOnOnConfirmBuy;
            _natsClient.OnConfirmJoin += NatsClientOnOnConfirmJoin;
            //_natsClient.OnCreateSession += NatsClientOnOnCreateSession;
            //_natsClient.OnDeptUpdate += NatsClientOnOnDeptUpdate;
            //_natsClient.OnDonateMoney += NatsClientOnOnDonateMoney;
            _natsClient.OnDonatePoints += NatsClientOnOnDonatePoints;
            _natsClient.OnEndGame += NatsClientOnOnEndGame;
            //_natsClient.OnHeartBeat += NatsClientOnOnHeartBeat;
            _natsClient.OnListCards += NatsClientOnOnListCards;
            _natsClient.OnMakeBidding += NatsClientOnOnMakeBidding;
            _natsClient.OnRejectBidding += NatsClientOnOnRejectBidding;
            _natsClient.OnRespondBidding += NatsClientOnOnRespondBidding;
            _natsClient.OnStartGame += NatsClientOnOnStartGame;
            _natsClient.OnStartRound += NatsClientOnOnStartRound;
            _natsClient.OnStopRound += NatsClientOnOnStopRound;
            _natsClient.OnAcceptCounterBidding += NatsClientOnOnAcceptCounterBidding;
            //_natsClient.OnCardHandIn += NatsClientOnOnCardHandIn;
            _natsClient.OnConfirmCancelListing += NatsClientOnOnConfirmCancelListing;
            _natsClient.OnConfirmHandIn += NatsClientOnOnConfirmHandIn;
            _natsClient.OnEndOfRounds += NatsClientOnOnEndOfRounds;
            //_natsClient.OnConnect += NatsClientOnOnConnect;
            
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
        
        // No idea where we use this
        /*private void NatsClientOnOnConnect(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }*/

        private void NatsClientOnOnEndOfRounds(object sender, EndOfRoundsMessage e)
        {
            // TODO game phase system
            throw new NotImplementedException();
        }

        private void NatsClientOnOnConfirmHandIn(object sender, ConfirmHandInMessage e)
        {
            PlayerData.PlayerData.Instance.ConfirmHandIn(e);
        }

        private void NatsClientOnOnConfirmCancelListing(object sender, ConfirmCancelListingMessage e)
        {
            // TODO MARKET FUNCTION
            throw new NotImplementedException();
        }

        // Host function
        /*private void NatsClientOnOnCardHandIn(object sender, CardHandInMessage e)
        {
            throw new NotImplementedException();
        }*/

        private void NatsClientOnOnAcceptCounterBidding(object sender, AcceptCounterBiddingMessage e)
        {
            // TODO MARKET FUNCTION
            throw new NotImplementedException();
        }

        private void NatsClientOnOnStopRound(object sender, StopRoundMessage e)
        {
            // TODO game phase system
            throw new NotImplementedException();
        }

        private void NatsClientOnOnStartRound(object sender, StartRoundMessage e)
        {
            Utilities.GameManager.Instance.LoadPhase(e.RoundNumber);
        }

        private void NatsClientOnOnStartGame(object sender, StartGameMessage e)
        {
            Utilities.GameManager.Instance.StartGame();
            PlayerData.PlayerData.Instance.StartGame(e);
            
        }

        private void NatsClientOnOnRespondBidding(object sender, RespondBiddingMessage e)
        {
            // TODO MARKET FUNCTION
            throw new NotImplementedException();
        }

        private void NatsClientOnOnRejectBidding(object sender, RejectBiddingMessage e)
        {
            // TODO MARKET FUNCTION
            throw new NotImplementedException();
        }

        private void NatsClientOnOnMakeBidding(object sender, MakeBiddingMessage e)
        {
            // TODO MARKET FUNCTION 
            throw new NotImplementedException();
        }

        private void NatsClientOnOnListCards(object sender, ListCardsmessage e)
        {
            // TODO MARKET FUNCTION
            throw new NotImplementedException();
        }

        // Host Function
        /*private void NatsClientOnOnHeartBeat(object sender, HeartBeatMessage e)
        {
            return;
        }*/

        private void NatsClientOnOnEndGame(object sender, EndGameMessage e)
        {
            PlayerData.PlayerData.Instance.ResetData();
        }

        private void NatsClientOnOnDonatePoints(object sender, DonatePointsMessage e)
        {
            PlayerData.PlayerData.Instance.PointsDonated(e);
        }

        // Database function
        /*private void NatsClientOnOnDonateMoney(object sender, DonateMoneyMessage e)
        {
            throw new NotImplementedException();
        }*/
        
        // No clue where we use this for
        /*private void NatsClientOnOnDeptUpdate(object sender, DeptUpdateMessage e)
        {
            throw new NotImplementedException();
        }*/

        // Database function
        /*private void NatsClientOnOnCreateSession(object sender, CreateSessionMessage e)
        {
            throw new NotImplementedException();
        }*/

        private void NatsClientOnOnConfirmJoin(object sender, ConfirmJoinMessage e)
        {
            _natsClient.StartHeartbeat();
            SceneManager.LoadScene("Loading");
            PlayerData.PlayerData.Instance.PlayerId = e.LobbyPlayerID;
        }

        private void NatsClientOnOnConfirmBuy(object sender, ConfirmBuyMessage e)
        {
            // TODO MARKET FUNCTION
            throw new NotImplementedException();
        }

        private void NatsClientOnOnCancelListing(object sender, CancelListingMessage e)
        {
            // TODO MARKET FUNCTION
            throw new NotImplementedException();
        }

        private void NatsClientOnOnCancelBidding(object sender, CancelBiddingMessage e)
        {
            // TODO MARKET FUNCTION
            throw new NotImplementedException();
        }

        private void NatsClientOnOnBuyCards(object sender, BuyCardsRequestMessage e)
        {
            // TODO MARKET FUNCTION
            throw new NotImplementedException();
        }

        private void NatsClientOnOnAcceptBidding(object sender, AcceptBiddingMessage e)
        {
            // TODO MARKET FUNCTION
            throw new NotImplementedException();
        }

        private void NatsClientOnOnRejected(object sender, RejectedMessage e)
        {
            // TODO MARKET FUNCTION
            throw new NotImplementedException();
        }

        /*private void NatsClientOnOnJoinrequest(object sender, JoinRequestMessage e)
        {
            throw new NotImplementedException();
        }*/

        private void Update()
        {
            _natsClient.HandleMessages();
        }
    }
}