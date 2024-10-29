using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CancelListingOverlay : MonoBehaviour
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        
        public void Open(string auctionId)
        {
            gameObject.SetActive(true);
            confirmButton.interactable = true;
            cancelButton.interactable = true;

            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                MarketManager.Instance.CancelListingRequest(auctionId);

                confirmButton.interactable = false;
                cancelButton.interactable = false;
            });
        }
    }
}