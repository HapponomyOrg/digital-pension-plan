using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
[Obsolete]
public class Listing
{
    public string Seller { get; }
    public string AuctionId { get; }
    public int Price { get; }
    public int[] Cards { get; }

    private List<BiddingDisplay> biddings = new List<BiddingDisplay>();

    public Listing(string seller, string auctionId, int price, int[] cards)
    {
        Seller = seller;
        AuctionId = auctionId;
        Price = price;
        Cards = cards;
    }

    public void RemoveAllBiddings(bool accepted)
    {
        var biddingsInList = biddings.Count;
        for (var i = 0; i < biddingsInList; i++)
        {
            if (!biddings[i])
                continue;

            biddings[i].Remove(accepted);
            biddings.RemoveAt(i);
        }
    }

    public void AddBidding(BiddingDisplay biddingDisplay)
    {
        biddings.Add(biddingDisplay);
    }
}
