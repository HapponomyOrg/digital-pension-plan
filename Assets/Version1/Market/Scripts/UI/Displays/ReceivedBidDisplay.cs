using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Market.Scripts.UI.Displays
{
    public class ReceivedBidDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text bidderName;
        [SerializeField] private TMP_Text bidOffer;
        
        [SerializeField] private Button accept;
        [SerializeField] private Button counter;
        [SerializeField] private Button decline;

        public void Init(Bid bid, Dictionary<BidDisplayAction, Action> displayActions)
        {
            bidderName.text = bid.BidderName;
            bidOffer.text = bid.OfferedPrice.ToString("N0");
            
            accept.onClick.AddListener(displayActions[BidDisplayAction.Accept].Invoke);
            counter.onClick.AddListener(displayActions[BidDisplayAction.Counter].Invoke);
            decline.onClick.AddListener(displayActions[BidDisplayAction.Decline].Invoke);
        }
    }
}
