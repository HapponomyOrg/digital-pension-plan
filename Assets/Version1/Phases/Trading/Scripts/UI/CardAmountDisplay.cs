using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Phases.Trading.Scripts.UI
{
    public class CardAmountDisplay : MonoBehaviour
    {
        [SerializeField] private Image imageDisplay;
        [SerializeField] private TMP_Text amountDisplay;

        public void SetDisplay(int cardId, int amount, bool icon = false)
        {
            var cardData = Utilities.GameManager.Instance.CardLibrary.CardData(cardId);
            imageDisplay.sprite = icon ? cardData.Icon : cardData.Art;
            amountDisplay.text = amount.ToString();
        }
    }
}
