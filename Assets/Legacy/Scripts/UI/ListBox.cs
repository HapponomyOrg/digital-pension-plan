using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [Obsolete]
    public class ListBox : CardBox
    {
        [SerializeField] private SellOverlay sellOverlay;

        [SerializeField] private Button submitButton;
        [SerializeField] private TMP_Text buttonText;

        protected override void UpdateDisplay()
        {
            base.UpdateDisplay();

            switch (cards.Count)
            {
                case 0:
                    submitButton.interactable = false;
                    buttonText.text = "List card";
                    break;
                case 1:
                    submitButton.interactable = true;
                    buttonText.text = "List card";
                    break;
                case > 1:
                    submitButton.interactable = true;
                    buttonText.text = "List cards";
                    break;
            }

            SetButtonListeners();
        }

        private void SetButtonListeners()
        {
            submitButton.onClick.RemoveAllListeners();

            if (cards.Count == 0)
                return;

            var cardIds = cards.Select(c => c.Card.ID).Select(dummy => (int)dummy).ToArray();

            submitButton.onClick.AddListener(() => sellOverlay.Open(cardIds));
        }

        public void DestroyCardsInBox()
        {
            var objs = cards.ToArray();
            foreach (var obj in objs)
            {
                PlayerManager.Instance.UiCards.Remove(obj);
                Destroy(obj.gameObject);
            }

            cards.Clear();

            UpdateDisplay();
        }
    }
}
