using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Market.Scripts.UI
{
    public class ListingCardDisplay : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text amountDisplay;

        public void Init(int cardId, int amount)
        {
            Debug.Log($"card: {Utilities.GameManager.Instance.CardLibrary.CardData(cardId)}        Id:{cardId}");
            icon.sprite = Utilities.GameManager.Instance.CardLibrary.CardData(cardId).Icon;
            amountDisplay.text = amount.ToString();
        }
    }
}
