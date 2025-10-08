using TMPro;
using UnityEngine;

namespace Version1.Market.Scripts.UI.Displays
{
    public class ListingDetailsDisplay : MonoBehaviour
    {
        private Listing listing;

        [SerializeField] private TMP_Text sellerName;
        [SerializeField] private TMP_Text price;

        [SerializeField] private Transform cardList;
        [SerializeField] private ListingCardDisplay cardDisplay;

    }
}
