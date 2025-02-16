using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Market.Scripts.UI.Displays
{
    public class DetailsCardDisplay : MonoBehaviour
    {
        [SerializeField] private Image card;
        [SerializeField] private TMP_Text amountDisplay;

        public void Init(int cardId, int amount)
        {
            card.sprite = Utilities.GameManager.Instance.CardLibrary.CardData(cardId).Art;
            amountDisplay.text = amount.ToString();
        }
    }
}
