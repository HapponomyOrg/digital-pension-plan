using UnityEngine;
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
        /*public string NATSURL = "nats://localhost:5432";*/

        public IConnection NATSConnection;

        protected AsyncSubscription AsyncSubscription;



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
}