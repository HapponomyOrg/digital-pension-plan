using System;
using System.Collections.Generic;
using UnityEngine;
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
            }
        }
    }
}
