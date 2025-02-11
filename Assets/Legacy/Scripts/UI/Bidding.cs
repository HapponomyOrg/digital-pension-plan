using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Obsolete]
public class Bidding
{
    public int SenderId { get; }
    public string Sender { get; }
    public string AuctionId { get; }
    public int OfferPrice { get; }

    public Bidding(int senderId, string sender, string auctionId, int offerPrice)
    {
        SenderId = senderId;
        Sender = sender;
        AuctionId = auctionId;
        OfferPrice = offerPrice;
    }
}
