using System;
using System.Collections.Generic;
using UnityEngine;

namespace NATS
{
    public class MessageSubject
    {
        
        public static Dictionary<string, Type> Subjects = new Dictionary<string, Type>        {
            { "BuyCards", typeof(BuyCardsRequestMessage) },
            { "ListCards", typeof(ListCardsmessage) },
            { "CancelListing", typeof(CancelListingMessage) },
            { "DonateMoney", typeof(DonateMoneyMessage) },
            { "DonatePoints", typeof(DonatePointsMessage) },
            { "DeptUpdate", typeof(DeptUpdateMessage) },
            { "CardHandIn", typeof(CardHandInMessage) },
            { "HeartBeat", typeof(HeartBeatMessage) },
            { "MakeBidding", typeof(MakeBiddingMessage) },
            { "AcceptBidding", typeof(AcceptBiddingMessage) },
            { "CancelBidding", typeof(CancelBiddingMessage) },
            { "RejectBidding", typeof(RejectBiddingMessage) },
            { "RespondBidding", typeof(RespondBiddingMessage) },
            { "AcceptCounterBidding", typeof(AcceptCounterBiddingMessage) },
            { "JoinRequest", typeof(JoinRequestMessage) },
            { "CreateSession", typeof(CreateSessionMessage) },
            { "StartGame", typeof(StartGameMessage) },
            { "StartRound", typeof(StartRoundMessage) },
            { "StopRound", typeof(StopRoundMessage) },
            { "EndOfRounds", typeof(EndOfRoundsMessage) },
            { "EndGame", typeof(EndGameMessage) },
            { "ConfirmJoin", typeof(ConfirmJoinMessage) },
            { "Rejected", typeof(RejectedMessage) },
            { "ConfirmBuy", typeof(ConfirmBuyMessage) },
            { "ConfirmHandIn", typeof(ConfirmHandInMessage) },
            { "ConfirmCancelListing", typeof(ConfirmCancelListingMessage) }
        };
        
         public const string
            // Client Gameplay
            None = "None",
            Base = "Base",
            ListCards = "ListCards",
            BuyCards = "BuyCards",
            CancelListing = "CancelListing",
            DonateMoney = "DonateMoney",
            DonatePoints = "DonatePoints",
            DeptUpdate = "DeptUpdate",
            CardHandIn = "CardHandIn",
            HeartBeat = "HeartBeat",
            MakeBidding = "MakeBidding",
            AcceptBidding = "AcceptBidding",
            CancelBidding = "CancelBidding",
            RejectBidding = "RejectBidding",
            RespondBidding = "RespondBidding",
            AcceptCounterBidding = "AcceptCounterBidding",
            // Client Misc.
            JoinRequest = "JoinRequest",
            // Host Hosting
            CreateSession = "CreateSession",
            StartGame = "StartGame",
            StartRound = "StartRound",
            StopRound = "StopRound",
            EndOfRounds = "EndOfRounds",
            EndGame = "EndGame",
            // Host Misc.
            ConfirmJoin = "ConfirmJoin",
            Rejected = "Rejected",
            ConfirmBuy = "ConfirmBuy",
            ConfirmHandIn = "ConfirmHandIn",
            ConfirmCancelListing = "ConfirmCancelListing";
    }
}