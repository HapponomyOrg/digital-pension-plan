using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Phases.Trading.CardHandIn.Scripts
{
    public class CardScript : MonoBehaviour
    {
        [SerializeField] private Image imageDisplay;

        public void SetDisplay(int cardId, int amount, bool icon = false)
        {
            var cardData = Utilities.GameManager.Instance.CardLibrary.CardData(cardId);
            imageDisplay.sprite = icon ? cardData.Icon : cardData.Art;
        }
    }
}
