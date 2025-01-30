using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Version1.Cards.Scripts
{
    public class PlayerHand : MonoBehaviour
    {
        [SerializeField] public RectTransform rectTransform;
        [SerializeField] private HorizontalLayoutGroup layoutGroup;

        [SerializeField] public List<UiCard> cardList;
        [SerializeField] public UiCard cardPrefab;

        [SerializeField] private float minSpacing = 5f;
        [SerializeField] private float maxSpacing = 20f;

        [SerializeField] private CardLibrary cardLibrary;

        private void Start()
        {
            cardList = new List<UiCard>();

            PlayerData.PlayerData.Instance.OnCardsChange += PlayerDataOnOnCardsChange;
        }

        private void PlayerDataOnOnCardsChange(object sender, List<int> updatedCards)
        {
            Debug.Log("PlayerDataChanged");
            
            var updatedCardCounts = updatedCards.GroupBy(id => id)
                .ToDictionary(group => group.Key, group => group.Count());
            
            var currentCardCounts = cardList.GroupBy(card => card.cardData.ID)
                .ToDictionary(group => group.Key, group => group.Count());
            
            foreach (var cardId in updatedCardCounts.Keys)
            {
                int currentCount = currentCardCounts.ContainsKey(cardId) ? currentCardCounts[cardId] : 0;
                int requiredCount = updatedCardCounts[cardId];

                while (currentCount < requiredCount)
                {
                    AddCardById(cardId);
                    currentCount++;
                }
            }
            
            foreach (var cardId in currentCardCounts.Keys)
            {
                int currentCount = currentCardCounts[cardId];
                int requiredCount = updatedCardCounts.ContainsKey(cardId) ? updatedCardCounts[cardId] : 0;

                while (currentCount > requiredCount)
                {
                    var cardToRemove = cardList.First(card => card.cardData.ID == cardId);
                    RemoveCard(cardToRemove);
                    currentCount--;
                }
            }

            Reorganize();
        }


        private void AddCardById(int cardId)
        {
            Debug.Log("Card is being added");
            var cardInfo = cardLibrary.CardData(cardId);

            if (cardInfo == null)
            {
                Debug.LogWarning($"Card ID {cardId} not found in the library.");
                return;
            }

            var newCard = Instantiate(cardPrefab, this.transform);
            newCard.name = cardInfo.name;
            
            newCard.Initialize(cardInfo);

            AddCard(newCard);
        }

        public void AddCard(UiCard card)
        {
            cardList.Add(card);
            card.currentBox = this;
        }

        public void RemoveCard(UiCard card)
        {
            if (!cardList.Contains(card)) return;

            cardList.Remove(card);
            card.oldBox = this;

            Destroy(card.gameObject);
        }

        private void Reorganize()
        {
            cardList = cardList.OrderBy(card => card.cardData.Value).ToList();

            for (int i = 0; i < cardList.Count; i++)
            {
                cardList[i].transform.SetSiblingIndex(i);
            }

            AdjustSpacing();
        }

        private void AdjustSpacing()
        {
            if (cardList.Count == 0) return;

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

                float newSpacing = Mathf.Max(minSpacing, maxSpacing - (excessWidth / gapsCount));

                layoutGroup.spacing = newSpacing;
            }
            else
            {
                if (cardList.Count == 1)
                {
                    layoutGroup.spacing = 0;
                }
                else
                {
                    layoutGroup.spacing = Mathf.Min(maxSpacing,
                        maxSpacing - (availableWidth - totalCardWidth) / (cardList.Count - 1));
                }
            }
        }
    }
}