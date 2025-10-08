using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
[Obsolete]
public class ReceivedBiddingDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private TMP_Text priceDisplay;

    [SerializeField] private Button accept;
    [SerializeField] private Button respond;
    [SerializeField] private Button reject;

    public int BidderId { get; private set; }
    public int Bid { get; private set; }

    public void SetDisplay(Listing listing, Bidding bidding)
    {
        nameDisplay.text = bidding.Sender;
        priceDisplay.text = bidding.OfferPrice.ToString();

        Bid = bidding.OfferPrice;
        BidderId = bidding.SenderId;

        // TODO THIS IS A MAGIC NUMBER AND CAN BE CHANGED TO THE STEPSIZE
        if ((listing.Price - 1000) == bidding.OfferPrice)
            respond.interactable = false;

        accept.onClick.AddListener(() => { MarketManager.Instance.BiddingAccepted(bidding); });
        respond.onClick.AddListener(() => { MarketManager.Instance.OpenRespondToBiddingOverlay(this, bidding); });
        reject.onClick.AddListener(() => { MarketManager.Instance.BiddingRejected(this, bidding); });
    }
}
