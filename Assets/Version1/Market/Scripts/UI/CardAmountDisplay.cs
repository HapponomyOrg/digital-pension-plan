using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Market
{
    public class CardAmountDisplay : MonoBehaviour
    {
        [SerializeField] private Image imageDisplay;
        [SerializeField] private TMP_Text amountDisplay;

        public void SetDisplay(int cardId, int amount)
        {
            imageDisplay.sprite = Utilities.GameManager.Instance.CardLibrary.CardData(cardId).Art;
            amountDisplay.text = amount.ToString();
        }
    }
}
