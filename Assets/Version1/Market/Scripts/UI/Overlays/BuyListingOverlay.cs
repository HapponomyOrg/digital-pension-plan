using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Version1.Utilities;

namespace Version1.Market
{
    public class BuyListingOverlay : MonoBehaviour
    {
        [SerializeField] private Button confirmButton;

        public void Open(Listing listing)
        {
            gameObject.SetActive(true);

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => { Confirm(listing); });
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void Confirm(Listing listing)
        {
            Utilities.GameManager.Instance.MarketServices.BuyListingService.BuyListingLocally(listing);
            Close();
        }
    }
}
