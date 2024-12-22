using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Version1.Cards.Scripts
{
    public class HorizontalListBox : MonoBehaviour
    {
    
        [SerializeField] public RectTransform rectTransform;
        [SerializeField] private HorizontalLayoutGroup layoutGroup;
    
        [SerializeField] public List<UiCard> cardList;
        [SerializeField] public UiCard cardPrefab;
        
        [SerializeField] private float minSpacing = 5f;
        [SerializeField] private float maxSpacing = 20f;

        private void Start()
        {
            cardList = new List<UiCard>();

            for (int i = 0; i < 10; i++)
            {
                var newCard = Instantiate(cardPrefab, this.transform);
                newCard.name = $"Card{i}";
                AddCard(newCard);   
            }
        }
    
        public void AddCard(UiCard card)
        {
            cardList.Add(card);
        
            card.currentBox = this;
        
            Reorganize();
        }

        public void RemoveCard(UiCard card)
        {
            if (!cardList.Contains(card)) return;
            cardList.Remove(card);
            card.oldBox = this;
            Reorganize();
        }

        private void Reorganize()
        {        
            cardList = cardList.OrderBy(card2 => card2.cardData.Value).ToList();
        
            for (int i = 0; i < cardList.Count; i++)
            {
                cardList[i].transform.SetSiblingIndex(i);
            }

            AdjustSpacing();
        }
        
        private void AdjustSpacing()
        {
            float totalCardWidth = cardList.Sum(card => card.GetComponent<RectTransform>().sizeDelta.x);
            
            float availableWidth = rectTransform.rect.width;
            
            if (totalCardWidth > availableWidth)
            {
                float excessWidth = totalCardWidth - availableWidth;

                int gapsCount = cardList.Count - 1;

                if (gapsCount <= 0)
                {
                    layoutGroup.spacing = 0;
                    return;
                }
                float newSpacing = Mathf.Max(minSpacing, 0 - (excessWidth / gapsCount));

                layoutGroup.spacing = newSpacing;
            }
            else
            {
                layoutGroup.spacing = 0;
            }
        }
    }
}
