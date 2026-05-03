using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Version1.Market;
using Version1.Utilities;

namespace Version1.Phases.Trading.Scripts.UI.Overlays
{
    public class CancelBidOverlay : MonoBehaviour
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
            Utilities.GameManager.Instance.MarketServices.CancelBidService.CancelBidLocally(listingId, bid);
            Close();
        }
    }
}
