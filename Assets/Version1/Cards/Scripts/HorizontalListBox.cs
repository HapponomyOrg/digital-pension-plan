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
    
        [SerializeField] public List<SelectableCard> cardList;
        [SerializeField] public SelectableCard cardPrefab;

        private void Start()
        {
            cardList = new List<SelectableCard>();

            var newCard = Instantiate(cardPrefab, this.transform);
            newCard.card.Value = 1;
            AddCard(newCard);
        
            var newCard2 = Instantiate(cardPrefab, this.transform);
            newCard2.card.Value = 2;
            AddCard(newCard2);
        }
    
        public void AddCard(SelectableCard card)
        {
            cardList.Add(card);
        
            card.currentBox = this;
        
            Reorganize();
        }

        public void RemoveCard(SelectableCard card)
        {
            if (!cardList.Contains(card)) return;
            cardList.Remove(card);
            card.oldBox = this;
            Reorganize();
        }

        private void Reorganize()
        {        
            cardList = cardList.OrderBy(card2 => card2.card.Value).ToList();
        
            for (int i = 0; i < cardList.Count; i++)
            {
                cardList[i].transform.SetSiblingIndex(i);
            }
        }
    }
}
