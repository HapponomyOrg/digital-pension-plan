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
            // Calculate the total width of all cards (including their spacing)
            float totalCardWidth = cardList.Sum(card => card.GetComponent<RectTransform>().sizeDelta.x);
    
            // Get the available width from the parent RectTransform
            float availableWidth = rectTransform.rect.width;

            Debug.Log($"Total Card Width: {totalCardWidth}, Available Width: {availableWidth}");

            // If the total width of the cards exceeds the available width, adjust spacing
            if (totalCardWidth > availableWidth)
            {
                // Calculate how much the cards need to reduce their spacing
                float excessWidth = totalCardWidth - availableWidth;

                // Calculate the total number of gaps between cards
                int gapsCount = cardList.Count - 1;

                // Avoid division by zero for cases with a single card
                if (gapsCount <= 0)
                {
                    layoutGroup.spacing = 0; // No gaps if there's only one card
                    return;
                }

                // Calculate the new spacing based on the excess width divided by the number of gaps
                float newSpacing = Mathf.Max(minSpacing, 0 - (excessWidth / gapsCount));

                layoutGroup.spacing = newSpacing;

                Debug.Log($"Adjusted Spacing (overflow): {layoutGroup.spacing}");
            }
            else
            {
                // If there's enough space, use the maximum spacing value (which is 0)
                layoutGroup.spacing = 0;
                Debug.Log($"Adjusted Spacing (comfortable): 0");
            }
        }





    }
}
