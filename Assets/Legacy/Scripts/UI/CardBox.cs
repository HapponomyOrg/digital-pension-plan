using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    [Obsolete]
    public class CardBox : MonoBehaviour
    {
        [SerializeField] private RectTransform cardDetection;
        [SerializeField] private RectTransform displayTransform;

        protected List<UICard> cards;

        private void Start()
        {
            PlayerManager.Instance.CardReleased += (sender, args) =>
            {
                UpdateDisplay();
                LayoutRebuilder.ForceRebuildLayoutImmediate(displayTransform);
            };
        }

        protected virtual void UpdateDisplay()
        {
            cards = CardsInBox();

            foreach (var c in cards)
                c.transform.SetParent(displayTransform.transform);
        }

        private List<UICard> CardsInBox()
        {
            var objs = PlayerManager.Instance.UiCards;

            var cardsInBox = new List<UICard>();

            foreach (var card in objs)
            {
                if (CardOverlaps(card.Rect))
                    cardsInBox.Add(card);
            }

            return cardsInBox;
        }


        private bool CardOverlaps(RectTransform card)
        {
            // Convert the local positions to world positions
            var cardDetectionWorldPos = cardDetection.TransformPoint(cardDetection.rect.position);
            var cardWorldPos = card.TransformPoint(card.rect.position);

            // Create Rects in world space
            var cardDetectionRect = new Rect(cardDetectionWorldPos.x, cardDetectionWorldPos.y, cardDetection.rect.width,
                cardDetection.rect.height);
            var cardRect = new Rect(cardWorldPos.x, cardWorldPos.y, card.rect.width, card.rect.height);

            // print($"{cardRect.Overlaps(cardDetectionRect)}; {gameObject.name}");

            return cardRect.Overlaps(cardDetectionRect);
        }
    }
}
