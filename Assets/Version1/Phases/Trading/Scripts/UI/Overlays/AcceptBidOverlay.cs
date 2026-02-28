using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Version1.Utilities;

namespace Version1.Market
{
    public class AcceptBidOverlay : MonoBehaviour
    {
        [SerializeField] private Button confirmButton;

        public void Open(Guid listingId, Bid bid)
        {
            gameObject.SetActive(true);

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => { Confirm(listingId, bid); });
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void Confirm(Guid listingId, Bid bid)
        {
            Utilities.GameManager.Instance.MarketServices.AcceptBidService.AcceptBidLocally(listingId, bid);
            Close();
        }
    }
}
