using System;
using System.Collections.Generic;
using UnityEngine;

namespace NATS
{
    public class MessageSubject
    {
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