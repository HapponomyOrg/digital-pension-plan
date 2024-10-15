using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NATS;
using NATS.Client;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MessageTool : MonoBehaviour
{
    /// TODO
    /// remove this
    /// what i can do is that i check the type of message that it is and afterwords i spawn the right amount of input boxes
    /// than i map every input box to the right variable and afterwords i send it.

    // Dirty i know.
    [SerializeField] private GameObject messageCard;

    [SerializeField] private Transform messageList;
    
    [SerializeField]
    private TMP_Dropdown dropdown;

    [SerializeField] 
    private List<TMP_Text> inputFieldsText = new List<TMP_Text>();
    
    [SerializeField] 
    private List<TMP_InputField> inputFields = new List<TMP_InputField>();


    private int _fieldAmount = 0;
    
    void Start()
    {
        new NatsClient();
        NatsClient.C.NATSConnection.SubscribeAsync(">", (sender, args) => { QueueMsg(args); });;
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
            
            var newMessageCard = Instantiate(messageCard, messageList);

            var image = newMessageCard.GetComponentInChildren<Image>();

            image.color = Color.green;

            var tmpro = newMessageCard.GetComponentInChildren<TMP_Text>();

            tmpro.text = msg.ToString();
    }

    public void setMessageType()
    {
        foreach (var subject in MessageSubject.Subjects)
        {
            // Check if the dropdown selection matches the dictionary key
            if (dropdown.captionText.text.Contains(subject.Key))  // Correctly accessing the text
            {
                Type messageType = subject.Value;
                
                FieldInfo[] fields = messageType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                _fieldAmount = fields.Length;
                
                for (int i = 0; i < _fieldAmount; i++)
                {
                    inputFieldsText[i].text = fields[i].Name;
                }
            }
        }

    }

    
    // Message sent {"DateTimeStamp":"2024-09-12T16:42:26.2006094-04:00","Subject":"CreateSession","LobbyID":996919147,"PlayerID":1,"SessionToken":996919147}
    // subject 996919147.CreateSession
    public void sendMessage()
    {
        String msgString = "";

        for (int i = 0; i < _fieldAmount; i++)
        {
            String text = inputFields[i].text;
            
            if (int.TryParse(text, out int result))
            {
                msgString += $"\"{inputFieldsText[i].text}\":{result},";
            }
            
            msgString += $"\"{inputFieldsText[i].text}\":\"{text}\",";
        }
        
        if (msgString.EndsWith(","))
        {
            msgString = msgString.Substring(0, msgString.Length - 1);
        }
        
        msgString = "{" + msgString + "}";
        
        print(msgString);
        
        BasicMessage message = JsonUtility.FromJson<BasicMessage>(msgString);
        
        string baseSubject = message.Subject;
        int lobbyId = message.LobbyID;

        string subject = "" + lobbyId + "." + baseSubject + "";
        
        byte[] messageEncoded = Encoding.UTF8.GetBytes(msgString);

        NatsClient.C.NATSConnection.Publish(subject, messageEncoded);

    }

    /*
    private void Update()
    {
        if (NatsClient.C.EventsReceived.Count >= 1)
        {
            print("BINGO");
            var message = NatsClient.C.EventsReceived.Dequeue();

            if (message == null) return;

                        switch (subjects[^1])
            {
                case MessageSubject.ConfirmJoin:
                    msg = JsonUtility.FromJson<ConfirmJoinMessage>(message);
                    break;
                case MessageSubject.DeptUpdate:
                    msg = JsonUtility.FromJson<DeptUpdateMessage>(message);
                    break;
                case MessageSubject.StartGame:
                    msg = JsonUtility.FromJson<StartGameMessage>(message);
                    break;
                case MessageSubject.CardHandIn:
                    msg = JsonUtility.FromJson<CardHandInMessage>(message);
                    break;
                case MessageSubject.StopRound:
                    msg = JsonUtility.FromJson<StopRoundMessage>(message);
                    break;
                case MessageSubject.StartRound:
                    msg = JsonUtility.FromJson<StartRoundMessage>(message);
                    break;
                case MessageSubject.Rejected:
                    msg = JsonUtility.FromJson<RejectedMessage>(message);
                    break;
                case MessageSubject.BuyCards:
                    msg = JsonUtility.FromJson<BuyCardsRequestMessage>(message);
                    break;
                case MessageSubject.CancelListing:
                    msg = JsonUtility.FromJson<CancelListingMessage>(message);
                    break;
                case MessageSubject.ConfirmBuy:
                    msg = JsonUtility.FromJson<ConfirmBuyMessage>(message);
                    break;
                case MessageSubject.CreateSession:
                    msg = JsonUtility.FromJson<CreateSessionMessage>(message);
                    break;
                case MessageSubject.DonateMoney:
                    msg = JsonUtility.FromJson<DonateMoneyMessage>(message);
                    break;
                case MessageSubject.DonatePoints:
                    msg = JsonUtility.FromJson<DonatePointsMessage>(message);
                    break;
                case MessageSubject.EndGame:
                    msg = JsonUtility.FromJson<EndGameMessage>(message);
                    break;
                case MessageSubject.HeartBeat:
                    msg = JsonUtility.FromJson<HeartBeatMessage>(message);
                    break;
                case MessageSubject.JoinRequest:
                    msg = JsonUtility.FromJson<JoinRequestMessage>(message);
                    break;
                case MessageSubject.EndOfRounds:
                    msg = JsonUtility.FromJson<EndOfRoundsMessage>(message);
                    break;
                case MessageSubject.ConfirmCancelListing:
                    msg = JsonUtility.FromJson<ConfirmCancelListingMessage>(message);
                    break;
                case MessageSubject.ConfirmHandIn:
                    msg = JsonUtility.FromJson<ConfirmHandInMessage>(message);
                    break;
                case MessageSubject.ListCards:
                    msg = JsonUtility.FromJson<ListCardsmessage>(message);
                    break;
                case MessageSubject.MakeBidding:
                    msg = JsonUtility.FromJson<MakeBiddingMessage>(message);
                    break;
                case MessageSubject.AcceptBidding:
                    msg = JsonUtility.FromJson<AcceptBiddingMessage>(message);
                    break;
                case MessageSubject.CancelBidding:
                    msg = JsonUtility.FromJson<CancelBiddingMessage>(message);
                    break;
                case MessageSubject.RejectBidding:
                    msg = JsonUtility.FromJson<RejectBiddingMessage>(message);
                    break;
                case MessageSubject.RespondBidding:
                    msg = JsonUtility.FromJson<RespondBiddingMessage>(message);
                    break;
                case MessageSubject.AcceptCounterBidding:
                    msg = JsonUtility.FromJson<AcceptCounterBiddingMessage>(message);
                    break;
                default:
                    return;
            }
            
            var newMessageCard = Instantiate(messageCard, messageList, false);

            var image = newMessageCard.GetComponentInChildren<Image>();

            image.color = Color.green;

            var tmpro = newMessageCard.GetComponentInChildren<TMP_Text>();

            tmpro.text = message.ToString();
        }
    }*/
}

public class BasicMessage
{
    public string Subject;
    public int LobbyID;
}
