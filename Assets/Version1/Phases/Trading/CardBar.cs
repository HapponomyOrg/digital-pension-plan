using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Version1.Market.Scripts.UI.Displays;

namespace Version1.Phases.Trading
{
    public class CardBar : MonoBehaviour
    {
        [SerializeField] private Transform cardList;
        [SerializeField] private DetailsCardDisplay cardDisplay;
        
        
        public void Init()
        {
            GenerateCardDisplays();

            PlayerData.PlayerData.Instance.OnCardsChange += (sender, ints) => { GenerateCardDisplays(); };
        }
        
        private void GenerateCardDisplays()
        {
            foreach (Transform child in cardList)
                Destroy(child.gameObject);
            
            
            var cardAmounts = new Dictionary<int, int>();
            foreach (var cardId in PlayerData.PlayerData.Instance.Cards)
            {
                cardAmounts[cardId] = cardAmounts.TryGetValue(cardId, out var amount)
                    ? amount + 1 
                    : 1;
            }

            foreach (var cardAmount in cardAmounts)
            {
                var obj = Instantiate(cardDisplay, cardList);
                obj.Init(cardAmount.Key, cardAmount.Value);

                if (cardAmount.Value >= 4)
                {
                    obj.gameObject.GetComponent<Image>().color = Color.yellow;
                    var button = obj.gameObject.AddComponent<Button>();
                    button.onClick.AddListener(() => { HandInCards(cardAmount.Key); });
                }
            }
        }

        private void HandInCards(int card)
        {
            Debug.Log("Hand in cards");
            for (var i = 0; i < 4; i++)
            {
                PlayerData.PlayerData.Instance.RemoveCard(card);
            }
            PlayerData.PlayerData.Instance.AddPoints(Utilities.GameManager.Instance.CardLibrary.CardData(card).Value);
        }
    }
}
