using Version1.Nats.Messages;
using Version1.Nats.Messages.Client;
using Version1.Nats.Messages.Host;

namespace Version1.Nats
{
    /*using UnityEngine;
 using NATS.Client;
 using System.Text;
 using System;
 using System.Collections.Generic;
 using System.Net;

 namespace NATS
 {
     public abstract class Connection
     {
         public string NATSURL = "nats://ec2-13-60-182-44.eu-north-1.compute.amazonaws.com:4222";
         /*public string NATSURL = "nats://localhost:5432";#1#

         protected IConnection NATSConnection;

         protected AsyncSubscription AsyncSubscription;s



         public Queue<BaseMessage> EventsReceived = new Queue<BaseMessage>();

         public event EventHandler OnConnect;
         // public event EventHandler OnDisconnect;
         // public event EventHandler OnReconnect;

         protected Connection()
         {
             Connect();
             Subscribe();
         }

         protected void OnDestroy()
         {
             NATSConnection.Close();
         }

         protected void OnDisable()
         {
             NATSConnection.Close();
         }

         private void Connect()
         {
             Debug.Log("Connectiong to NATS at " + NATSURL);

             Options opts = ConnectionFactory.GetDefaultOptions();
             opts.Url = NATSURL;
             opts.Timeout = 10000;

             NATSConnection = new ConnectionFactory().CreateConnection(opts);

             OnConnect?.Invoke(this, EventArgs.Empty);
             Debug.Log("Connected to NATS at " + NATSURL);
         }

         protected abstract void Subscribe();

         public void SubscribeToSubject(string str)
         {
             string subject = str + ".>";
             Debug.Log("Subscribing to: " + subject);
             NATSConnection.SubscribeAsync(subject, (sender, args) => { QueueMsg(args); });
         }

         private void QueueMsg(MsgHandlerEventArgs args)
         {
             string stringReceived = Encoding.UTF8.GetString(args.Message.Data);

             if (args.Message.Subject == null) return;

             string[] subjects = args.Message.Subject.Split('.');

             BaseMessage msg;

             switch (subjects[^1])
             {
                 case MessageSubject.ConfirmJoin:
                     msg = JsonUtility.FromJson<ConfirmJoinMessage>(stringReceived);
                     break;
                 case MessageSubject.DeptUpdate:
                     msg = JsonUtility.FromJson<DeptUpdateMessage>(stringReceived);
                     break;
                 case MessageSubject.StartGame:
                     msg = JsonUtility.FromJson<StartGameMessage>(stringReceived);
                     break;
                 case MessageSubject.CardHandIn:
                     msg = JsonUtility.FromJson<CardHandInMessage>(stringReceived);
                     break;
                 case MessageSubject.StopRound:
                     msg = JsonUtility.FromJson<StopRoundMessage>(stringReceived);
                     break;
                 case MessageSubject.StartRound:
                     msg = JsonUtility.FromJson<StartRoundMessage>(stringReceived);
                     break;
                 case MessageSubject.Rejected:
                     msg = JsonUtility.FromJson<RejectedMessage>(stringReceived);
                     break;
                 case MessageSubject.BuyCards:
                     msg = JsonUtility.FromJson<BuyCardsRequestMessage>(stringReceived);
                     break;
                 case MessageSubject.CancelListing:
                     msg = JsonUtility.FromJson<CancelListingMessage>(stringReceived);
                     break;
                 case MessageSubject.ConfirmBuy:
                     msg = JsonUtility.FromJson<ConfirmBuyMessage>(stringReceived);
                     break;
                 case MessageSubject.CreateSession:
                     msg = JsonUtility.FromJson<CreateSessionMessage>(stringReceived);
                     break;
                 case MessageSubject.DonateMoney:
                     msg = JsonUtility.FromJson<DonateMoneyMessage>(stringReceived);
                     break;
                 case MessageSubject.DonatePoints:
                     msg = JsonUtility.FromJson<DonatePointsMessage>(stringReceived);
                     break;
                 case MessageSubject.EndGame:
                     msg = JsonUtility.FromJson<EndGameMessage>(stringReceived);
                     break;
                 case MessageSubject.HeartBeat:
                     msg = JsonUtility.FromJson<HeartBeatMessage>(stringReceived);
                     break;
                 case MessageSubject.JoinRequest:
                     msg = JsonUtility.FromJson<JoinRequestMessage>(stringReceived);
                     break;
                 case MessageSubject.EndOfRounds:
                     msg = JsonUtility.FromJson<EndOfRoundsMessage>(stringReceived);
                     break;
                 case MessageSubject.ConfirmCancelListing:
                     msg = JsonUtility.FromJson<ConfirmCancelListingMessage>(stringReceived);
                     break;
                 case MessageSubject.ConfirmHandIn:
                     msg = JsonUtility.FromJson<ConfirmHandInMessage>(stringReceived);
                     break;
                 case MessageSubject.ListCards:
                     msg = JsonUtility.FromJson<ListCardsmessage>(stringReceived);
                     break;
                 case MessageSubject.MakeBidding:
                     msg = JsonUtility.FromJson<MakeBiddingMessage>(stringReceived);
                     break;
                 case MessageSubject.AcceptBidding:
                     msg = JsonUtility.FromJson<AcceptBiddingMessage>(stringReceived);
                     break;
                 case MessageSubject.CancelBidding:
                     msg = JsonUtility.FromJson<CancelBiddingMessage>(stringReceived);
                     break;
                 case MessageSubject.RejectBidding:
                     msg = JsonUtility.FromJson<RejectBiddingMessage>(stringReceived);
                     break;
                 case MessageSubject.RespondBidding:
                     msg = JsonUtility.FromJson<RespondBiddingMessage>(stringReceived);
                     break;
                 case MessageSubject.AcceptCounterBidding:
                     msg = JsonUtility.FromJson<AcceptCounterBiddingMessage>(stringReceived);
                     break;
                 default:
                     return;
             }
             EventsReceived.Enqueue(msg);
         }

         public void Publish(string sessionID, BaseMessage baseMessage, bool flushImmediately = true)
         {
             string subject = "" + sessionID + "." + baseMessage.Subject + "";

             string messageSerialized = JsonUtility.ToJson(baseMessage);
             byte[] messageEncoded = Encoding.UTF8.GetBytes(messageSerialized);

             NATSConnection.Publish(subject, messageEncoded);

             if (flushImmediately)
             {
                 NATSConnection.Flush();
             }

             if (baseMessage.Subject == MessageSubject.HeartBeat)
                 return;

             Debug.Log("Message sent " + messageSerialized + ", subject " + subject);
         }
     }
 }*/

    using UnityEngine;
    using NATS.Client;
    using System.Text;
    using System;
    using System.Collections.Generic;

    public abstract class Connection
    {
        //private const string DefaultNATSURL = "nats://ec2-13-60-182-44.eu-north-1.compute.amazonaws.com:4222";
        private const string DefaultNATSURL = "nats://127.0.0.1:4222";
        private const int ConnectionTimeout = 10000;

        public string NATSURL = DefaultNATSURL;
        protected IConnection NATSConnection;
        protected AsyncSubscription AsyncSubscription;
        public Queue<BaseMessage> EventsReceived = new Queue<BaseMessage>();

        public event EventHandler OnConnect;

        private readonly Dictionary<string, Type> messageTypes = new()
        {
            { MessageSubject.ConfirmJoin, typeof(ConfirmJoinMessage) },
            { MessageSubject.DeptUpdate, typeof(DeptUpdateMessage) },
            { MessageSubject.StartGame, typeof(StartGameMessage) },
            { MessageSubject.CardHandIn, typeof(CardHandInMessage) },
            { MessageSubject.StopRound, typeof(StopRoundMessage) },
            { MessageSubject.StartRound, typeof(StartRoundMessage) },
            { MessageSubject.Rejected, typeof(RejectedMessage) },
            { MessageSubject.BuyCards, typeof(BuyCardsRequestMessage) },
            { MessageSubject.CancelListing, typeof(CancelListingMessage) },
            { MessageSubject.ConfirmBuy, typeof(ConfirmBuyMessage) },
            { MessageSubject.CreateSession, typeof(CreateSessionMessage) },
            { MessageSubject.DonateMoney, typeof(DonateMoneyMessage) },
            { MessageSubject.DonatePoints, typeof(DonatePointsMessage) },
            { MessageSubject.EndGame, typeof(EndGameMessage) },
            { MessageSubject.HeartBeat, typeof(HeartBeatMessage) },
            { MessageSubject.JoinRequest, typeof(JoinRequestMessage) },
            { MessageSubject.EndOfRounds, typeof(EndOfRoundsMessage) },
            { MessageSubject.ConfirmCancelListing, typeof(ConfirmCancelListingMessage) },
            { MessageSubject.ConfirmHandIn, typeof(ConfirmHandInMessage) },
            { MessageSubject.ListCards, typeof(ListCardsmessage) },
            { MessageSubject.MakeBidding, typeof(MakeBiddingMessage) },
            { MessageSubject.AcceptBidding, typeof(AcceptBiddingMessage) },
            { MessageSubject.CancelBidding, typeof(CancelBiddingMessage) },
            { MessageSubject.RejectBidding, typeof(RejectBiddingMessage) },
            { MessageSubject.RespondBidding, typeof(RespondBiddingMessage) },
            { MessageSubject.AcceptCounterBidding, typeof(AcceptCounterBiddingMessage) },
            { MessageSubject.AbortSession, typeof(AbortSessionMessage) }
        };

        protected Connection()
        {
            Connect();
            Subscribe();
        }

        protected void OnDestroy() => CloseConnection();

        protected void OnDisable() => CloseConnection();

        private void Connect()
        {
            Debug.Log("Connecting to NATS at " + NATSURL);
            try
            {
                Options opts = ConnectionFactory.GetDefaultOptions();
                opts.Url = NATSURL;
                opts.Timeout = ConnectionTimeout;
                NATSConnection = new ConnectionFactory().CreateConnection(opts);
                OnConnect?.Invoke(this, EventArgs.Empty);
                Debug.Log("Connected to NATS at " + NATSURL);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to connect to NATS: " + ex.Message);
            }
        }

        protected abstract void Subscribe();

        public void SubscribeToSubject(string subject)
        {
            string fullSubject = $"{subject}.>";
            Debug.Log("Subscribing to: " + fullSubject);
            NATSConnection.SubscribeAsync(fullSubject, (sender, args) => { QueueMsg(args); });
        }

        private void QueueMsg(MsgHandlerEventArgs args)
        {
            if (args.Message.Subject == null) return;

            string stringReceived = Encoding.UTF8.GetString(args.Message.Data);
            string subjectKey = args.Message.Subject.Split('.')[^1];

            if (messageTypes.TryGetValue(subjectKey, out Type messageType))
            {
                BaseMessage msg = (BaseMessage)JsonUtility.FromJson(stringReceived, messageType);
                EventsReceived.Enqueue(msg);
            }
        }

        public void Publish(string sessionID, BaseMessage baseMessage, bool flushImmediately = true)
        {
            string subject = $"{sessionID}.{baseMessage.Subject}";
            string messageSerialized = JsonUtility.ToJson(baseMessage);
            byte[] messageEncoded = Encoding.UTF8.GetBytes(messageSerialized);

            try
            {
                NATSConnection.Publish(subject, messageEncoded);
                if (flushImmediately) NATSConnection.Flush();

                if (baseMessage.Subject != MessageSubject.HeartBeat)
                    Debug.Log("Message sent: " + messageSerialized + ", subject: " + subject);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to publish message: " + ex.Message);
            }
        }

        private void CloseConnection()
        {
            if (NATSConnection != null && !NATSConnection.IsClosed())
            {
                NATSConnection.Close();
                Debug.Log("NATS connection closed.");
            }
        }
    }
}